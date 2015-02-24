module Main

open Speller
open Speller.BrillMoore
open Speller.EditDistance
open Speller.SpellingCorpus
open Speller.Constants
open Speller.Candidators.BigramsBitCounting
open Speller.Candidators.TrigramsJaccard
open System
open System.Diagnostics
open System.IO

let dictionary = File.ReadAllLines dictionaryPath |> Seq.filter (fun x -> x.Length <= 15) |> Seq.toArray

let editProbs =
    let singleLetterRules = seq{'a'..'z'} |> Seq.map (fun x -> ((x.ToString(),x.ToString()),log10 (1.0 - misspellingRate)))
    
    File.ReadAllLines probabilitiesPath
    |> Seq.map (fun x -> x.Split([|' '|], StringSplitOptions.RemoveEmptyEntries))
    |> Seq.filter (fun x -> x.Length = 3)
    |> Seq.map (fun tokens -> ((tokens.[0], tokens.[1]), float tokens.[2]))
    |> Seq.append singleLetterRules
    |> Map.ofSeq

let evaluateSpeller (speller : ISpeller) =
    File.WriteAllText("errors.txt","")
    let correct = speller.CorrectWord
    let testCorpus = readSpellingCorpus testSpellingCorpusPath
    let size = testCorpus.Length
    let totalWatch = System.Diagnostics.Stopwatch.StartNew()
    let success = testCorpus |> Seq.sumBy (fun (wro, cor) ->    
                                                                printfn ""
                                                                printfn "WORD %s" wro
                                                                let sw = System.Diagnostics.Stopwatch.StartNew()
                                                                let cands, (corrected, zzz)::_= correct wro
                                                                sw.Stop()
                                                                let secs = int sw.Elapsed.TotalSeconds
                                                                if cor = corrected then
                                                                    System.Console.ForegroundColor <- ConsoleColor.Green
                                                                    printfn "RIGHT %d secs" secs
                                                                    System.Console.ForegroundColor <- ConsoleColor.Gray
                                                                    1 
                                                                else
                                                                    System.Console.ForegroundColor <- ConsoleColor.Red
                                                                    let yes = if Seq.exists ((=) cor) cands then "IN" else "OUT"
                                                                    let dic = if not(Seq.exists ((=) cor) dictionary) then "NO DIC!" else ""
                                                                    printfn "WRONG %s %s %d secs: '%s' instead of '%s'" yes dic secs corrected cor
                                                                    let err = sprintf "'%s':%s %s PROPOSED '%s' INSTEAD OF '%s'" yes dic wro  corrected cor
                                                                    File.AppendAllLines("errors.txt", [err])
                                                                    System.Console.ForegroundColor <- ConsoleColor.Gray
                                                                    0
                                                                )
    totalWatch.Stop()
    
    File.AppendAllText("errors.txt", (sprintf "RESULT: %d out of %d" success size))
    printfn "RESULT: %d out of %d" success size
    printfn "TOTAL TIME: %d mins" (int totalWatch.Elapsed.TotalMinutes)

(*let Main = 
    let speller = BrillMooreSpeller((fun _ -> dictionary), editProbs)
    evaluateSpeller speller*)

let Main_ =
    printfn "%A" (editDistance (genericEditParams editProbs) "akgsual" "actual")

let Main = 
    let dictionaryTrie = FastEditDistance.createDictionaryTrie ["actual"]
    let ruleTrie = FastEditDistance.createRuleTrie (editProbs |> Map.toSeq |> Seq.toArray)
    printfn "IT'S STARTING!!!!1111"
    let s = Stopwatch.StartNew()
    printfn "%A" (FastEditDistance.processWord dictionaryTrie ruleTrie "akgsual")
    printfn "%A" s.Elapsed

