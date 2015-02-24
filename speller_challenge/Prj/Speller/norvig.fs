namespace Speller
 
open System

type NorvigSpeller(words : seq<String>) =
    let alphabet = seq { 'a' .. 'z' }
    let deletes (word : String) = seq { for i in { 0 .. word.Length - 1 } -> word.Substring(0, i) + word.Substring(i + 1) }
    let inserts (word : String) = seq { for i in { 0 .. word.Length } do for ch in alphabet -> word.Substring(0, i) + ch.ToString() + word.Substring(i) }
    let transposes (word : String) = seq { for i in { 0 .. word.Length - 2 } -> word.Substring(0, i) + word.Substring(i + 1, 1) + word.Substring(i, 1) + word.Substring(i + 2) }
    let replaces (word : String) = seq {
        for i in { 0 .. word.Length - 1 } do
            for ch in alphabet do
                let have = word.[i]
                if ch <> have then
                    yield word.Substring(0, i) + ch.ToString() + word.Substring(i + 1)
    }

    let wordDict = words |> Seq.countBy id |> Map.ofSeq
    
    let edits1 word = Seq.map (fun f -> f word) [deletes; inserts; transposes; replaces] |> Seq.concat
    
    let known = Seq.filter wordDict.ContainsKey
    let correctWord word =
        let candidates =
            let (<|>) a b = if Seq.isEmpty a then b else a
            let onlyWord = Seq.singleton word
            known onlyWord <|> known (edits1 word) <|> known (edits1 word |> Seq.collect edits1)
        if Seq.isEmpty candidates then word else Seq.maxBy wordDict.get_Item candidates

    interface ISpeller with
        member this.CorrectQuery(q : String) =
            String.concat " " (q.Split [|' '|] |> Seq.ofArray |> Seq.map correctWord)