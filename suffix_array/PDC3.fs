module SA.PDC3
open System
open System.Linq
open Microsoft.FSharp.Collections

let triplet (s: int[]) i = (s.[i], s.[i+1], s.[i+2])
let name (S: ('a*int) []) = 
    let int b = if b then 1 else 0

    let zo = [1] @( Seq.pairwise S |> PSeq.ordered |> PSeq.map (fun (l,r) -> int(fst l <> fst r)) |> PSeq.toList)

    let hasDups = PSeq.tryFind ((=) 0) zo |> Option.isSome
    let partialSums = List.scan (+) 0 zo |> Seq.toArray
    let named = PSeq.mapi (fun i x -> (partialSums.[i+1], snd x)) S |> PSeq.toArray
    (named, hasDups)

let stableSortBy f A = Seq.sortBy f A |> Seq.toArray

let rec buildSuffixArray' (s: int[]) : int[] = 
    let oldN = s.Length
    let s = Array.append s [|0;0;0;0|]
    let lastOneSafeN = oldN//if oldN%3 = 1 then oldN else oldN-1
    let newN = s.Length
    
    let S = 
        seq{0..lastOneSafeN}
        |> PSeq.filter (fun i -> i%3 <> 0) 
        |> PSeq.map (fun i -> (triplet s i, i))
        |> PSeq.sortBy fst 
        |> PSeq.toArray
    let mutable P, hasDups = name S
    if hasDups then
        //printfn "Recurse call"
        P <- PSeq.sortBy (fun (r,i) -> (i%3, i/3)) P |> PSeq.toArray
        let vals = P |> Array.map fst
        let inds = P |> Array.map snd
        let SA12 = buildSuffixArray' vals
        P <- SA12 |> PSeq.mapi (fun i suffNum -> (i, inds.[suffNum])) |> PSeq.toArray
    
    let SA = Array.zeroCreate newN
    for i in 0..P.Length-1 do
        SA.[snd P.[i]] <- fst P.[i]
        
    let rec cmp i1 i2 =
        if s.[i1] <> s.[i2] then
            Operators.compare s.[i1] s.[i2]
        else
            let j1 = i1%3
            let j2 = i2%3
            if j1 + j2 <= 1 then // 0 + 0/1
                Operators.compare SA.[i1+1] SA.[i2+1]
            else if j1 + j2 > 2 || j1 = 1 && j2 = 1 then // 1 + 1/2
                Operators.compare SA.[i1] SA.[i2]
            else // 0 + 2 -> 1 + 0
                cmp (i1+1) (i2+1)
    ([|0..oldN-1|] |> Array.sortWith cmp)

let buildSuffixArray (s: String) =
    s |> Seq.map (fun x -> int x - int 'a' + 1) |> Seq.toArray |> buildSuffixArray'