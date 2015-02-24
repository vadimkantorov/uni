module Trie

open System

type Trie<'payload> = {
    Data : 'payload[];
    Children : int[,];
    Final : bool[];
    ParentChar : int[];
    Parent : int[];
    mutable Next : int;
    AbcSize : int
}

let add (t : Trie<'payload>) (s : String) (data : 'payload) =
    let rec addToTrie' strIndex currentNode =
        if strIndex = s.Length then
            t.Final.[currentNode] <- true
            t.Data.[currentNode] <- data
        else
            let ch = int s.[strIndex] - int 'a'
            if t.Children.[currentNode, ch] = -1 then
                t.Children.[currentNode, ch] <- t.Next
                t.ParentChar.[t.Next] <- ch
                t.Parent.[t.Next] <- currentNode
                t.Next <- t.Next + 1
            addToTrie' (strIndex + 1) t.Children.[currentNode, ch]
    addToTrie' 0 0

let empty<'payload> size initData = 
    let abcSize = 26
    {
        AbcSize = abcSize;
        Data = Array.init<'payload> size (fun _ -> initData ());
        Children = Array2D.create size abcSize -1;
        Parent = Array.create size -1;
        ParentChar = Array.create size -1;
        Final = Array.zeroCreate size;
        Next = 1;
    }

let create<'payload> size strs (initData : unit -> 'payload) (createFinalData : String -> 'payload) = 
    let res = empty size initData
    for word in strs do
        add res word (createFinalData word)
    res

