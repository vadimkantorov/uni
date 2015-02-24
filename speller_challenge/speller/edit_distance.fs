module Speller.EditDistance

open Speller.Constants
open System
open System.IO
open Memoization

type EditDistanceParams = {
    getSuffixCost : String -> int -> int -> String -> int -> int -> float;
    moreIsBetter : bool;
}

let editDistance (parameters : EditDistanceParams) (source : String) (target : String) : (String * String) * float =
    let rec editDistance' =
        memoize (function
            | (0, 0) -> ((0, 0), 0.0)
            | (sourcePrefix, targetPrefix) ->
                let distanceBySuffixes sourceSuffix targetSuffix =
                    let suffixCost = parameters.getSuffixCost source sourcePrefix sourceSuffix target targetPrefix targetSuffix
                    let (_, newDistance) = editDistance'.[((sourcePrefix - sourceSuffix), (targetPrefix - targetSuffix))]
                    suffixCost + newDistance
                let suffixLengths = seq {
                    for a in 0 .. sourcePrefix do
                    for b in 0 .. targetPrefix do
                    if a > 0 || b > 0 then yield (a, b)
                }
                let selector = if parameters.moreIsBetter then Seq.maxBy else Seq.minBy
                let processCandidate cand =
                    let dist = distanceBySuffixes (fst cand) (snd cand)
                    (cand, dist)
                suffixLengths
                |> Seq.map processCandidate
                |> selector snd
        )
    let ((s1, s2), distance) = editDistance'.[(source.Length, target.Length)]
    ((source.Substring(source.Length - s1), target.Substring(target.Length - s2)), distance)

let editSequence parameters source target =
    let rec editSequence' source target =
        match (source, target) with
        | ("", "") -> []
        | _ ->
            let ((s1, s2), _) = editDistance parameters source target
            (s1, s2)::(editSequence' (source.Substring(0, source.Length - s1.Length))) (target.Substring(0, target.Length - s2.Length))
    List.rev (editSequence' source target)

let edit1Params = {
    moreIsBetter = false;
    getSuffixCost = fun source sourcePrefix sourceSuffix target targetPrefix targetSuffix ->
        if sourceSuffix > 1 || targetSuffix > 1
            then infinity
            else
                if sourceSuffix = 1 && targetSuffix = 1 && source.[sourcePrefix - 1] = target.[targetPrefix - 1]
                    then 0.0
                    else 1.0;
}

let genericEditParams (editProbs : Map<string*string, float>) = {
    moreIsBetter = true;
    getSuffixCost = fun source sourcePrefix sourceSuffix target targetPrefix targetSuffix ->
        let strip (str : String) prefix suffix = str.Substring(prefix - suffix, suffix)
        let ss = strip source sourcePrefix sourceSuffix
        let tt = strip target targetPrefix targetSuffix
        let key = (tt, ss)
        if ss = tt then
            log10 (1.0 - misspellingRate)
        else if not(editProbs.ContainsKey(key)) then
            -infinity
        else
            editProbs.[key]
}