#load "constants.fs"

open Speller.Constants
open System.Text.RegularExpressions
open System.IO
open System

let lines = 
    let c (s:string) (pattern:string) = s.Contains pattern || s.Contains ((Char.ToUpper pattern.[0]).ToString() + pattern.Substring(1))
    let nc (s:string) (pattern:string) = not(c s pattern)

    let lines = File.ReadAllLines "../Data/english_words.tsv"
    let wordFromLine (x:string) = x.Split([|'\t'|]).[1].ToLower()
    let linesAndWords = lines |> Seq.map wordFromLine |> Seq.zip lines |> Seq.toList
    let badLine x = c x "misspelling" 
                            || c x "obsolete"
                            || c x "alternative spelling"
                            || c x "archaic"
                            || c x "dated"

    
    let maybeBadWords, maybeGoodWords  = linesAndWords |> List.partition (fst >> badLine)
    let smaybeBadWords, smaybeGoodWords = Set.ofList maybeBadWords |> Set.map snd, Set.ofList maybeGoodWords |> Set.map snd
    

    let badBadWords = smaybeGoodWords |> Set.filter (fun x -> x.EndsWith("s") && smaybeBadWords.Contains(x.[..x.Length-2]))

    let smaybeBadWords = badBadWords |> Set.union smaybeBadWords
    let smaybeGoodWords = badBadWords |> Set.difference smaybeGoodWords
    let excl =  Set.difference smaybeBadWords smaybeGoodWords
    lines
    |> Seq.map wordFromLine
    |> Seq.filter (excl.Contains >> not)
    |> Seq.filter (fun x -> Regex.IsMatch(x, "^[a-z]+$"))
    |> Seq.distinct
    |> Seq.toArray
    
File.WriteAllLines(dictionaryPath, lines)