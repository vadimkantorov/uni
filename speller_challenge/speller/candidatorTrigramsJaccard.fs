module Speller.Candidators.TrigramsJaccard
open System


let trigramsJaccard (correctWords : seq<string>) =
    let trigrams (word:string) =
        seq {for i in 0..word.Length-3 -> word.Substring(i, 3).GetHashCode() } |> Set.ofSeq
    let trigramsPrecalc = correctWords |> Seq.map trigrams |> Seq.zip correctWords |> Map.ofSeq
    let jaccard a b = float (Set.intersect a b |> Set.count) / float (Set.union a b |> Set.count)
    let bigramCandidates (word: String) =
        let ourTrigrams = trigrams word
        trigramsPrecalc
        |> Seq.map (fun pair -> (pair.Key, jaccard ourTrigrams pair.Value))
        |> Seq.sortBy (snd >> (~-))
        |> Seq.take 40
        |> Seq.map fst
        |> Seq.toArray
    bigramCandidates