module Speller.QuerySegmentation

open System
open System.Collections
open System.Collections.Specialized
open Speller.NGrams

let segmentations (qq : String[]) =
    let qq = Array.rev qq
    let rec enumSegmentations' acc ind = 
        if ind = qq.Length then
            seq {yield acc}
        else match acc with
             | [] -> enumSegmentations' [[qq.[0]]] 1
             | hd::tail -> seq {
                                yield! enumSegmentations' ((qq.[ind] :: hd)::tail) (ind + 1)
                                yield! enumSegmentations' ([qq.[ind]]::hd::tail) (ind + 1)
                                }
    enumSegmentations' [] 0 |> Seq.map (List.map List.toArray >> List.toArray)

let scoreSegmentation (segmentation : String[][]) =
    let probs = getNgramProbabilities segmentation
    let scorePhrase (segment: String[]) =
        let words = segment.Length
        //float (pown words words) * 4.0 * (10.0 ** (probs.[segment] + 20.0))
        float (pown words words) * (10.0 ** (probs.[segment]))
    
    let segmentOk (segment : String[]) = if segment.Length < 2 then true else segment.Length <= 5 && probs.[segment] >= -10.0
    let ok = Seq.forall segmentOk segmentation
    if ok then
        segmentation |> Seq.sumBy scorePhrase
    else
        -1.0

let segmentQuery (q : String) = 
    let qq = q.Split()

    let scoredSegmentations = segmentations qq |> Seq.map (fun s -> (s, scoreSegmentation s))
    printfn "%A" scoredSegmentations
    scoredSegmentations |> Seq.sortBy (snd >> (~-)) |> Seq.toArray

let Main = 
    //printfn "%A" (10.0 ** (getNgramProbabilities [[|"toronto blue jays"|]] |> Map.toArray |> Seq.head |> snd))
    //printfn "%A" (10.0 ** (getUnigramProbabilities [|"right"|] |> Map.toArray |> Seq.head |> snd))
    printfn "%A" (segmentQuery "property tax in marshall co ky")
    