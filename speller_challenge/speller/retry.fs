[<AutoOpen>]
module Retry
open System
open System.Threading
 
type ShouldRetry = ShouldRetry of (RetryCount * LastException -> bool * RetryDelay)
and RetryCount = int
and LastException = exn
and RetryDelay = TimeSpan
type RetryPolicy = RetryPolicy of ShouldRetry
   
type RetryPolicies() =
    static member NoRetry () : RetryPolicy =
        RetryPolicy( ShouldRetry (fun (retryCount, _) -> (retryCount < 1, TimeSpan.Zero)) )
    static member Retry (currentRetryCount : int , intervalBewteenRetries : RetryDelay) : RetryPolicy =
        RetryPolicy( ShouldRetry (fun (retryCount, _) -> (currentRetryCount < retryCount, intervalBewteenRetries)))
    static member Retry (currentRetryCount : int) : RetryPolicy =
        RetryPolicies.Retry(currentRetryCount, TimeSpan.Zero)
 
type RetryResult<'a> =
    | RetrySuccess of 'a
    | RetryFailure of exn
   
// Reader + Exception Monad DataType
type Retry<'a> = Retry of (RetryPolicy -> RetryResult<'a>)
 
type RetryBuilder() =
    member self.Return (value : 'a) : Retry<'a> = Retry (fun retryPolicy -> RetrySuccess value)
 
    member self.Bind (retry : Retry<'a>, bindFunc : 'a -> Retry<'b>) : Retry<'b> =
        Retry (fun retryPolicy ->
            let (Retry retryFunc) = retry
            match retryFunc retryPolicy with
            | RetrySuccess value ->
                let (Retry retryFunc') = bindFunc value
                retryFunc' retryPolicy
            | RetryFailure exn -> RetryFailure exn )
 
    member self.Delay (f : unit -> Retry<'a>) : Retry<'a> =
        Retry (fun retryPolicy ->
            let mutable resultCell : option<RetryResult<'a>> = None
            let mutable lastExceptionCell : exn = null
            let (RetryPolicy(ShouldRetry shouldRetry)) = retryPolicy
            let mutable canRetryCell : bool = true
            let mutable currentRetryCountCell : int = 0
            while canRetryCell do
                try
                    let (Retry retryFunc) = f ()
                    let result = retryFunc retryPolicy
                    resultCell <- Some result
                    canRetryCell <- false
                with e ->
                    lastExceptionCell <- e
                    currentRetryCountCell <- 1 + currentRetryCountCell
                    match shouldRetry(currentRetryCountCell, lastExceptionCell) with
                    | (true, retryDelay) ->
                        canRetryCell <- true
                        Thread.Sleep(retryDelay)
                    | (false, _) -> canRetryCell <- false
 
            match resultCell with
            | Some result -> result
            | None -> RetryFailure lastExceptionCell )
 


let retry = new RetryBuilder()
// override default policy
let retryWithPolicy (retryPolicy : RetryPolicy) (retry : Retry<'a>) =
    Retry (fun _ -> let (Retry retryFunc) = retry in retryFunc retryPolicy)

let run (retry : Retry<'a>) (retryPolicy : RetryPolicy) : RetryResult<'a> =
    let (Retry retryFunc) = retry
    retryFunc retryPolicy
