#load "trie.fs"
#load "fast_edit_distance.fs"

open Speller.FastEditDistance

let editProbs = [|
    (("ab","cd"), 0.1);
    (("ab","ed"), 0.2);
    (("cb","cd"), 0.3);
    (("cb","ed"), 0.4);
|]
let trie = createRuleTrie editProbs
printfn "%A" trie