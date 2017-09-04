namespace Seminario
    
module Main =
    open System
    open Deedle
    open FSharp.Charting


    [<EntryPoint>]
    let main argv = 
    
        let dateRange (first:System.DateTime) count = (*[omit:(...)]*)
          seq { for i in 0 .. (count - 1) -> first.AddSeconds(float i) }(*[/omit]*)
      
        let rand count =
          let rnd = System.Random()
          seq { for i in 0 .. (count - 1) -> rnd.NextDouble() }(*[/omit]*)
      
        let second = Series(dateRange (DateTime(1990,1,1)) 1000000, rand 1000000)

        let df1 = Frame(["second"], [second])

        
        let stopWatch2 = System.Diagnostics.Stopwatch.StartNew()

        let withRank2 =
            df1
            |> Frame.denseRankSlow "second" 1000 "secondRank"

        let sorted2 =
            withRank2
            |> Frame.sortRows "secondRank"
        sorted2.Print()

        stopWatch2.Stop()
        printfn "dense rank slow: %f" stopWatch2.Elapsed.TotalMilliseconds



        let stopWatch1 = System.Diagnostics.Stopwatch.StartNew()

        let withRank =
            df1
            |> Frame.denseRank "second" 1000 "secondRank"

        let sorted =
            withRank
            |> Frame.sortRows "secondRank"
        sorted.Print()
        stopWatch1.Stop()
        printfn "dense rank: %f" stopWatch1.Elapsed.TotalMilliseconds


        0
