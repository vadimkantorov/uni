module SA.Naive

open System

let buildSuffixArray (s:String) = 
    let s = s// + "$"
    let suffixes = seq {0 .. s.Length-1} |> Seq.map s.Substring |> Seq.toArray
    seq {0 .. s.Length-1} |> Seq.sortBy (Array.get suffixes) |> Seq.toArray

let check f = 
    let generateRandomStrings = seq {
        let rnd = Random()
        let rndSeq = Seq.initInfinite (fun _ -> char (int 'a' + rnd.Next(5)))
        for k = 0 to 100 do
            yield rndSeq |> Seq.take 10000 |> Seq.map string |> String.concat ""
        }
    
    let dt = DateTime.Now
    let corr = generateRandomStrings |> Seq.forall (fun s -> 
        let na = buildSuffixArray s
        let ev = f s
        if na = ev then true
        else 
            printfn "Input  : %A" s
            printfn "Wrong  : %A" na
            printfn "Correct: %A" ev
            false )
    printfn "Time: %d secs" (int((DateTime.Now - dt).TotalSeconds))
    if corr then
        printfn "Ok!"
    else
        printfn "Wrong!"

let Main = 
    //let s = "daaaa"
    //printfn "Wrong  : %A" (SA.DC3.buildSuffixArray s)
    //printfn "Correct: %A" (buildSuffixArray s)
    check SA.PDC3.buildSuffixArray