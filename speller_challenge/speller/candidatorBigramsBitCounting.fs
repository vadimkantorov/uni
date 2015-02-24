module Speller.Candidators.BigramsBitCounting
open Speller
open System


let bigramsBitCounting (correctWords : seq<string>) =
    let vectorSize = 11
    let createVector () = Array.zeroCreate<uint64> vectorSize

    let naiveBitCount num =
        let mutable res = 0
        let mutable num1 = num
        while num1 <> 0 do
            res <- res + 1
            num1 <- (num1 &&& (num1 - 1))
        res

    let bitCountCache = Array.init (1 <<< 16) naiveBitCount
    let fastBitCount (num : uint64) =
        let mutable res = 0
        let mutable num1 = num
        while num1 <> 0UL do
            let truncated = num1 &&& 0xFFFFUL
            res <- bitCountCache.[int truncated]
            num1 <- num1 >>> 16
        res
        
    let intersect (v1 : uint64[]) (v2 : uint64[]) =
        let mutable res = 0
        for i = 0 to vectorSize - 1 do
            res <- res + (fastBitCount (v1.[i] &&& v2.[i]))
        res

    let bigramVector word =
        let ival (ch:Char) = int ch - int 'a'
        let bigrams = Seq.pairwise word |> Seq.map (fun (a, b) -> 26*(ival a) + (ival b))
        let res = createVector ()
        for bg in bigrams do
            let idx = bg/64
            res.[idx] <- (res.[idx] ||| (1UL <<< bg%64))
        res
    
    let precalculatedBigrams = correctWords |> Seq.map bigramVector |> Seq.zip correctWords |> Map.ofSeq
    let smallWords = correctWords |> Seq.filter (fun x -> x.Length <= 4) |> Seq.toArray
    let candidates (word: String) =
        if word.Length <= 4 then 
            smallWords
        else
            let ourVector = bigramVector word
            precalculatedBigrams
            |> Seq.map (fun pair -> (pair.Key, intersect ourVector pair.Value))
            |> Seq.sortBy (snd >> (~-))
            |> Seq.take 40
            |> Seq.map fst
            |> Seq.toArray
    candidates