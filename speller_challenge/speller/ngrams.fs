module Speller.NGrams

open Speller.Constants
open System
open System.IO
open System.Net
open Retry

let private authToken = "becb2f1b-07b3-4de2-b520-97771d32ea7b"

let private postRequestAndParseFloats (url : String) (ngramList : seq<String[]>) = 
    let request = ngramList |> Seq.map (String.concat " ") |> (String.concat Environment.NewLine)
    let r = retry {
        let result = WebClient().UploadString(url, request)
        File.AppendAllLines("log.txt", [""; sprintf "URL: %s" url; sprintf "NGRAMS: %A" ngramList; sprintf "RESULT: %s" result])
        return result.Trim([|'['; ']'|]).Split([|','|]) |> Seq.map float |> Seq.zip ngramList |> Seq.toArray
    }
    
    match (run r (RetryPolicies.Retry(5, TimeSpan.FromSeconds(10.0)))) with
        | RetrySuccess(resp) -> resp
        | _ -> failwith "NO INTERNET"
    

let private getNGramConditionals order (ngramList : seq<String[]>) =
    let batch = 800
    let arr = ngramList |> Seq.toArray
    seq {
        for i in 0 .. batch.. arr.Length-1 do
            let url = sprintf "http://web-ngram.research.microsoft.com/rest/lookup.svc/bing-query/jun09/%d/cp?u=%s&format=json" order authToken
            yield! postRequestAndParseFloats url (Array.sub arr i (min batch (arr.Length - i)))
    } |> Map.ofSeq

let getUnigramProbabilities (ngramList: string[]) =
    let filePath x = Path.Combine(unigramsBingCache, x) 
    let toCache = ngramList |> Seq.filter (filePath >> File.Exists >> not)                   
                            |> Seq.map (Array.create 1)
                    
    let tmp = getNGramConditionals 1 toCache
    for x in toCache do
        File.WriteAllText(filePath (Seq.head x), string tmp.[x])

    ngramList |> Seq.map (filePath >> File.ReadAllText >> float) |> Seq.zip ngramList |> Map.ofSeq

let getTrigramConditionals x = getNGramConditionals 3 x

let getNgramProbabilities (ngramList : seq<String[]>) = 
    let crawl (order, ngramList) = 
        let url = sprintf "http://web-ngram.research.microsoft.com/rest/lookup.svc/bing-body/apr10/%d/jp?u=%s&format=json" order authToken
        postRequestAndParseFloats url ngramList

    ngramList |> Seq.groupBy (fun x -> x.Length) |> Seq.filter (fst >> ((>=) 5)) |> Seq.map crawl |> Seq.concat |> Map.ofSeq
    
    