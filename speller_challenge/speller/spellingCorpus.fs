module Speller.SpellingCorpus

open System
open System.IO
open System.Text.RegularExpressions

let readSpellingCorpus corpusPath=
    let corpusLines = File.ReadAllLines corpusPath
    let getPairs (line : String) =
        let tokens = line.Split([|"->"|], StringSplitOptions.RemoveEmptyEntries) |> Seq.map (fun x -> x.ToLower()) |> Seq.toArray
        let leftSide = tokens.[0]
        let rightSides = tokens.[1].Split([|", "|], StringSplitOptions.RemoveEmptyEntries)
        rightSides |> Seq.map (fun rs -> (leftSide, rs))
    let goodWord w = Regex.IsMatch(w, "^[a-z]+$")
    corpusLines
    |> Seq.collect getPairs
    |> Seq.map (fun (a, b) -> (a.ToLower(), b.ToLower()))
    |> Seq.filter (fun (a, b) -> (goodWord a) && (goodWord b))
    |> Seq.toArray

let private shuffle (src : 'a array) = 
    let rng = new Random()
    let array = Array.copy src
    let n = array.Length
    for x in 1..n do
        let i = n-x
        let j = rng.Next(i+1)
        let tmp = array.[i]
        array.[i] <- array.[j]
        array.[j] <- tmp
    array

let splitSpellingCorpus corpus =
    let ratio = 0.8
    let corpus = corpus |> shuffle
    let trainingLinesCnt = int (ratio*(float corpus.Length))
    let toLines pairs = pairs |> Seq.map (fun pair -> sprintf "%s->%s" (fst pair) (snd pair)) |> Seq.toArray
    let trainingLines = Seq.take trainingLinesCnt corpus  |> toLines
    let testLines = Seq.skip trainingLinesCnt corpus |> toLines
    (trainingLines, testLines)
