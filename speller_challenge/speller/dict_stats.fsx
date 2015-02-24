#load "constants.fs"

open System
open System.IO

open Speller.Constants

let words = File.ReadAllLines dictionaryPath |> Seq.toArray

let lengthStats = words |> Seq.map (fun x -> x.Length) |> Seq.countBy id |> Seq.sortBy (snd >> (~-)) |> Seq.toArray
for l in lengthStats do
    printfn "%d %d" (fst l) (snd l)

let bgStats = words |> Seq.collect (fun x -> Seq.pairwise x) |> Seq.countBy id |> Seq.sortBy (snd >> (~-)) |> Seq.take 40 |> Seq.toArray
for l in bgStats do
    printfn "%A %d" (fst l) (snd l)

let triples (x : String) = seq { for i = 0 to x.Length - 3 do yield (x.[i], x.[i+1], x.[i+2]) }

let tgStats = words |> Seq.collect triples |> Seq.countBy id |> Seq.sortBy (snd >> (~-)) |> Seq.take 40 |> Seq.toArray
for l in tgStats do
    printfn "%A %d" (fst l) (snd l)