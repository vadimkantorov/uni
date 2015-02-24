#load "constants.fs"
#load "Memoization.fs"
#load "edit_distance.fs"
#load "spellingCorpus.fs"

open Speller.Constants
open Speller.EditDistance
open Speller.SpellingCorpus
open System.IO
open System

let adjacentEdits windowSize editSeq =
    let (leftSides', rightSides') = List.unzip editSeq
    let (leftSides, rightSides) = (List.toArray leftSides', List.toArray rightSides')
    let subEdit start count (arr : String[]) = String.concat "" arr.[start..start+count-1]
    seq {
        for n in 1 .. windowSize do
            for i in 0 .. leftSides.Length - n do
                yield (subEdit i n leftSides, subEdit i n rightSides)
    } |> Seq.filter (fun (src, dst) -> src <> dst && src.Length > 0 && dst.Length > 0)

let editProbabilities corpus =
    let defaultWindowSize = 3
    
    let processPair (src, dst) =
        printfn "%s->%s" src dst
        adjacentEdits defaultWindowSize (editSequence edit1Params src dst)

    let correctionsAndCounts =
        corpus
        |> Seq.collect processPair
        |> Seq.countBy id
        |> Seq.toArray
    
    let leftSideCounts = 
        correctionsAndCounts
        |> Seq.groupBy (fst >> fst)
        |> Seq.map (fun g -> (fst g, Seq.sumBy (snd) (snd g)))
        |> Map.ofSeq
    let prob alpha cnt =
        log10 (misspellingRate * (float cnt / float leftSideCounts.[alpha]))
    
    correctionsAndCounts
    |> Seq.map (fun ((alpha, beta), cnt) -> ((alpha, beta), prob alpha cnt))
    |> Map.ofSeq

let lines = editProbabilities (readSpellingCorpus spellingCorpusPath) |> Seq.map (fun pair -> sprintf "%s %s %f" (fst pair.Key) (snd pair.Key) pair.Value) |> Seq.toArray
File.WriteAllLines(probabilitiesPath, lines)