namespace Seminario
    
module Main =
    open System
    open Deedle
    open FSharp.Charting


    [<EntryPoint>]
    let main argv = 
        let dates  = 
          [ DateTime(2013,1,1); 
            DateTime(2013,1,4); 
            DateTime(2013,1,8) ]
        let values = 
          [ 10.0; 20.0; 30.0 ]
        let first = Series(dates, values)
    
        let dateRange (first:System.DateTime) count = (*[omit:(...)]*)
          seq { for i in 0 .. (count - 1) -> first.AddDays(float i) }(*[/omit]*)
      
        let rand count =
          let rnd = System.Random()
          seq { for i in 0 .. (count - 1) -> rnd.NextDouble() }(*[/omit]*)
      
        let second = Series(dateRange (DateTime(2013,1,1)) 10000, rand 10000)

        let df1 = Frame(["first"; "second"], [first; second])

        let withRank =
            df1
            |> Frame.denseRank "second" 10000 "secondRank"

        let sorted =
            withRank
            |> Frame.sortRows "secondRank"
        sorted.Print()

        printfn "%A" argv
        0
