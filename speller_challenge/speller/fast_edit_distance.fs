module Speller.FastEditDistance

open System

type DicTrie = Trie.Trie<float []>
type RuleTrie = Trie.Trie<Trie.Trie<float>>

let createDictionaryTrie dictionary =
    let trieSize = 700000
    let vectorSize = 15
    let createArray () = Array.create<'float> vectorSize -infinity
    let res = Trie.create trieSize dictionary createArray (fun _ -> createArray ())
    res.Data.[0].[0] <- 0.0
    res

let createBetaTrie betas =
    let betaTrieSize = 100
    let dic = betas |> Map.ofSeq
    Trie.create betaTrieSize (betas |> Seq.map fst) (fun () -> 0.0) dic.get_Item

let createRuleTrie (editProbs : (seq<(String*String)*float>)) =
    let revStr (s:String) = new String(s.ToCharArray() |> Array.rev)
    let selectBeta ((alpha, beta), prob) = (beta, prob)
        
    let raw = editProbs
                |> Seq.append (Seq.singleton (("",""), 0.0))
                |> Seq.map (fun ((alpha, beta), prob) -> ((revStr alpha, revStr beta), prob) )
                |> Seq.groupBy (fun ((alpha, _), _) -> alpha)
                |> Seq.map (fun (key, items) -> (key, createBetaTrie (items |> Seq.map selectBeta)))
                |> Map.ofSeq
    
    let alphaTrieSize = 10000
    Trie.create alphaTrieSize (raw |> Seq.map (fun x -> x.Key)) (fun () -> Trie.empty 0 (fun x -> 0.0)) raw.get_Item

let processWord (t : DicTrie) (r : RuleTrie) (err : String) =
    let alphaVisit curNode dicNode alphaNode =
        let betaTrie = r.Data.[alphaNode]
        for i = 1 to err.Length do
            let mutable betaNode = 0
            let mutable errIndex = i+1 //изначально правая часть пустая. по-другому: инициализировать betaNode
            while betaNode <> -1 && errIndex <> 0 do // правая часть правила - [errIndex, i]
                if betaTrie.Final.[betaNode] then
                    let estimate = t.Data.[dicNode].[errIndex - 1] //0 - epsilon, 1 - one char
                                        + betaTrie.Data.[betaNode]
                    let oldEstimate = t.Data.[curNode].[i]

                    //printfn "betaNode: %d" betaNode
                    //printfn "%f %f" t.Data.[dicNode].[errIndex - 1] betaTrie.Data.[betaNode]
                    //printfn "Old:%f New%f" oldEstimate estimate
                    //printfn "i: %d errIndex: %d" i errIndex

                    t.Data.[curNode].[i] <- max oldEstimate estimate
                    //printfn "[%d, %d] = %f" curNode i t.Data.[curNode].[i]
                if errIndex > 1 then // no next iteration will happen
                    betaNode <- betaTrie.Children.[betaNode, int err.[errIndex-2] - int 'a']
                errIndex <- errIndex-1

    let dfsVisit curNode =
        if curNode <> 0 then
            for i = 0 to err.Length do
                t.Data.[curNode].[i] <- -infinity
        
        let mutable dicNode = curNode
        let mutable alphaNode = 0
        while dicNode <> -1 && alphaNode <> -1 do
            if r.Final.[alphaNode] then
                if curNode <> dicNode then
                    //printfn "curNode = %d, dicNode = %d, alphaNode = %d" curNode dicNode alphaNode
                    alphaVisit curNode dicNode alphaNode
            if dicNode <> 0 then // there will be no next iteration
                alphaNode <- r.Children.[alphaNode, t.ParentChar.[dicNode]]
            dicNode <- t.Parent.[dicNode]

    let rec dfs curNode =
        dfsVisit curNode
        let mutable maxEst = t.Data.[curNode].[err.Length]
        let mutable maxNode = curNode

        if not(t.Final.[curNode]) then
            maxEst <- -infinity
            maxNode <- -1
        else
            maxNode <- maxNode
        for i = 0 to t.AbcSize-1 do
            let v = t.Children.[curNode, i]
            if v <> -1 then
                let est, node = dfs v
                if est > maxEst then
                    maxEst <- est
                    maxNode <- node

        (maxEst, maxNode)

    let (maxProb, maxNode) = dfs 0
    // printfn "maxProb = %f, curNode = %d" maxProb maxNode
    let mutable res = ""
    
    if maxNode = -1 then
        res <- err
    else
        let mutable curNode = maxNode
        while curNode <> 0 do
            res <- (char (t.ParentChar.[curNode] + (int 'a'))).ToString() + res
            curNode <- t.Parent.[curNode]
    (res, maxProb)