open System
open System.Globalization
open System.IO
open System.Net

let url query = sprintf "http://cqa.1gb.ru/Speller/default.aspx?q=%s" query
let queries = File.ReadAllLines "queries.txt" |> Seq.map (fun x -> x.Split([|'\t'|]) |> Seq.toList) |> Seq.toArray

let ours i query =
    printfn "%d/%d: [%s]" (i + 1) queries.Length query
    let sw = System.Diagnostics.Stopwatch()
    sw.Start()
    use client = new WebClient()
    let resp = client.DownloadString(url query)
    sw.Stop()

    let suggestions = resp
                        .Split([|'\r';'\n'|], StringSplitOptions.RemoveEmptyEntries)
                        |> Seq.map (fun x ->
                                            let ss = x.Split([|'\t'|])
                                            (ss.[0], Convert.ToDouble(ss.[1], CultureInfo.InvariantCulture))
                                    )
    (sw.Elapsed.TotalSeconds, suggestions)

let computeRecPrec i query =
    let their = Set(List.tail query)
    let (latency, our) = ours i (List.head query)
    let inTheir = our |> Seq.filter (fun x -> their.Contains(fst x)) |> Seq.toArray
    let precision = inTheir |> Seq.sumBy(snd)
    let recall = float inTheir.Length / float their.Count
    printfn "Prec=%f, Rec=%f, latency=%f" precision recall latency
    ((precision, recall), latency)

let recPrec = queries |> Seq.mapi computeRecPrec |> Seq.toArray
let eP = recPrec |> Seq.averageBy (fst >> fst)
let eR = recPrec |> Seq.averageBy (fst >> snd)
let overallLatency = recPrec |> Seq.sumBy snd
let F1 = 2.0*(eP*eR)/(eP+eR)

printfn "F1 = %f, overall latency = %f" F1 overallLatency