module Speller.EditDistance

open System
open System.IO
open Memoization

type EditDistanceParams = {
    combineCosts : float -> float -> float;
    getSuffixCost : String -> int -> int -> String -> int -> int -> float option;
    moreIsBetter : bool;
}

let editDistance (parameters : EditDistanceParams) (source : String) (target : String) : (String * String) * float =
    let rec editDistance' =
        memoize (fun (sourcePrefix, targetPrefix) ->
            match (sourcePrefix, targetPrefix) with
            | (0, 0) -> ((0, 0), 0.0)
            | _ ->
                let distanceBySuffixes sourceSuffix targetSuffix =
                    let suffixCost = parameters.getSuffixCost source sourcePrefix sourceSuffix target targetPrefix targetSuffix
                    match suffixCost with
                    | Some(suffixCost) ->
                        let (_, newDistance) = editDistance'.[((sourcePrefix - sourceSuffix), (targetPrefix - targetSuffix))]
                        Some(parameters.combineCosts suffixCost newDistance)
                    | None -> None
                let suffixLengths = seq {
                    for a in 0 .. sourcePrefix do
                    for b in 0 .. targetPrefix do
                    if a > 0 || b > 0 then yield (a, b)
                }
                let selector = if parameters.moreIsBetter then Seq.maxBy else Seq.minBy
                suffixLengths
                |> Seq.map (fun ss -> (ss, distanceBySuffixes (fst ss) (snd ss)))
                |> Seq.collect (fun (ss, maybeDistance) -> match maybeDistance with | Some(x) -> Seq.singleton (ss, x) | None -> Seq.empty)
                |> selector snd
        )
    let ((s1, s2), distance) = editDistance'.[(source.Length, target.Length)]
    ((source.Substring(source.Length - s1), target.Substring(target.Length - s2)), distance)

let edit1SuffixCost (a : String) aPrefix aSuffix (b : String) bPrefix bSuffix =
    if aSuffix > 1 || bSuffix > 1
        then None
        else
            if aSuffix = 1 && bSuffix = 1 && a.[aPrefix - 1] = b.[bPrefix - 1]
                then Some(0.0)
                else Some(1.0)

let edit1Params = {
    combineCosts = (+);
    getSuffixCost = edit1SuffixCost;
    moreIsBetter = false;
}

let editSequence source target =
    let rec editSequence' source target =
        match (source, target) with
        | ("", "") -> []
        | _ ->
            let ((s1, s2), _) = editDistance edit1Params source target
            (s1, s2)::(editSequence' (source.Substring(0, source.Length - s1.Length))) (target.Substring(0, target.Length - s2.Length))
    List.rev (editSequence' source target)

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
    corpus
    |> Seq.collect (fun (src, dst) -> adjacentEdits defaultWindowSize (editSequence src dst))
    |> Seq.countBy id
    |> Seq.sortBy (snd >> (~-))

let wikipediaCorpus =
    let corpusPath = "H:\Speller\Gramota\Data\wikipedia_spelling.txt"
    let corpusLines = File.ReadAllLines corpusPath
    let getPairs (line : String) =
        printfn "%s" line
        let tokens = line.Split([|"->"|], StringSplitOptions.RemoveEmptyEntries)
        let leftSide = tokens.[0]
        let rightSides = tokens.[1].Split([|", "|], StringSplitOptions.RemoveEmptyEntries)
        rightSides |> Seq.map (fun rs -> (leftSide, rs))
    corpusLines |> Seq.collect getPairs