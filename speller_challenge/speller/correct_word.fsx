#load "Memoization.fs"
#load "edit_distance.fs"
#load "common.fs"
#load "ngrams.fs"
#load "constants.fs"
#load "spellingCorpus.fs"
#load "brill_moore.fs"

open Speller
open Speller.BrillMoore
open Speller.EditDistance
open Speller.SpellingCorpus
open Speller.Constants
open System
open System.IO

let dictionary = File.ReadAllLines dictionaryPath

let editProbs =
    File.ReadAllLines probabilitiesPath
    |> Seq.map (fun x -> x.Split([|' '|], StringSplitOptions.RemoveEmptyEntries))
    |> Seq.filter (fun x -> x.Length = 3)
    |> Seq.map (fun tokens -> ((tokens.[0], tokens.[1]), float tokens.[2]))
    |> Map.ofSeq

let testSpeller correct =
    let testCorpus = readSpellingCorpus testSpellingCorpusPath
    let size = testCorpus.Length
    let success = testCorpus |> Seq.sumBy (fun (wro, cor) -> if cor = (correct wro |> Seq.head |> fst) then 1 else 0)
    (success, size)

let speller =  new BrillMooreSpeller(dictionary, editProbs)
printfn "RESULT: %d out of %d", (testSpeller speller.CorrectWord)
//printfn "RESULT: %s" (speller.CorrectQuery("esesntially"))
