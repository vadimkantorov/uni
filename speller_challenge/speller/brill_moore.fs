module Speller.BrillMoore
 
open System
open System.Collections
open System.Collections.Specialized
open Speller.EditDistance
open Speller.NGrams

type BrillMooreSpeller(candidator : string -> string [], editProbs : Map<(String*String),float>) =
    let dictionary = candidator ""
    let dictionaryTrie = FastEditDistance.createDictionaryTrie dictionary
    let ruleTrie = FastEditDistance.createRuleTrie (editProbs |> Map.toSeq |> Seq.toArray)
    
    
    interface ISpeller with
        member this.CorrectWord word =
            let corrected, prob = FastEditDistance.processWord dictionaryTrie ruleTrie word
            (dictionary, [ (corrected, prob) ])

            (*let candidates = candidator word
            printf "CANDS: %d" candidates.Length
        
            let bing = System.Diagnostics.Stopwatch.StartNew()
            let sourceModel = getUnigramProbabilities candidates
            bing.Stop()
            printfn " BING: %d msecs" (int bing.Elapsed.TotalMilliseconds)

            let processWord cand =
                let result = snd (editDistance (genericEditParams editProbs) word cand)
                result + sourceModel.[cand]
        
            (candidates,
                candidates
                |> Seq.map processWord
                |> Seq.zip candidates
                |> Seq.sortBy (snd >> (~-))
                |> Seq.toList)*)

        member this.CorrectQuery(q : String) =
            let corrected = q.Split() |> Seq.map ((this :> ISpeller).CorrectWord >> snd >> Seq.head >> fst) |> String.concat " "
            [|(corrected, 1.0)|]

        