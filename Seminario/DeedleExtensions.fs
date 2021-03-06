﻿namespace Seminario

[<AutoOpen>]
module DeedleExtensions =
    open Deedle
    open System
    (*
    type Frame<'TRowKey, 'TColumnKey
            when 'TRowKey : equality
            and 'TColumnKey : equality> with
            *)

    type Frame<'TRowKey, 'TColumnKey
            when 'TRowKey : equality
            and 'TColumnKey : equality> with
        static member denseRankSlow column (groups:int) rankName frame =
            let frameLength = Frame.countRows frame |> float
            let chunkSize =  frameLength / (float groups) |> Math.Ceiling |> int64

            let sorted =
                frame
                |> Frame.sortRows column

            let ranks =
                Frame.getCol column frame
                |> Series.map(fun k _ ->
                    int ((Frame.indexForKey k sorted) / chunkSize)
                )

            let clone = frame.Clone()
            clone.AddColumn(rankName, ranks)
            clone

        static member denseRank column (groups:int) rankName frame =
            let frameLength = Frame.countRows frame
            let chunkSize =  (float frameLength) / (float groups) |> Math.Ceiling

            let sorted =
                frame
                |> Frame.sortRows column

            let sortedKeys = Frame.getRowKeys sorted

            let ranksArr = Array.zeroCreate frameLength

            sortedKeys
            |> Seq.iteri (fun index _ -> ranksArr.[index] <- index / (int chunkSize))

            let ranks = Series(sortedKeys, ranksArr)
            let clone = frame.Clone()
            clone.AddColumn(rankName, ranks)
            clone

        static member indexRowsByDateTime dateColumnName frame =
            let toDateTime dateColumnName (os:ObjectSeries<'TColumnKey>) =
                (os.Get dateColumnName :?> string)
                |> DateTime.Parse
            Frame.indexRowsUsing (toDateTime dateColumnName) frame

        static member mapColumnValues inputColumn f frame =
            let values =
                Frame.getCol inputColumn frame
                |> Series.values
                |> Seq.map f
            Series(frame.RowKeys, values)

        static member mapColumnValuesIntoNewColumn inputColumn f outputColumn frame =
            Frame.addCol outputColumn (Frame.mapColumnValues inputColumn f frame) frame
        
        static member shiftColumn column offset (frame:Frame<_,_>) =
            let clone = frame.Clone()
            clone?column <-
                clone?column
                |> Series.shift offset
            clone

        static member nDaysBeforeDay days day (data:Frame<_,_>) =
            data.GetSubrange(None, Some (day, Indices.Exclusive))
            |> Frame.takeLast days

        // index for row with key
        // index starting at 0
        static member indexForKey (key:'K) (frame:Frame<'K,_>) : int64 =
            frame.RowIndex.Locate key
            |> frame.RowIndex.AddressOperations.OffsetOf

        // position of row with key.
        // Index beginning with 1
        static member positionForKey (key:'K) (frame:Frame<'K,_>) =
            Frame.indexForKey key frame
            |> (+) (int64 1)

        static member nRowsBeforeKey rows key (data:Frame<_,_>) =
            data
            |> Frame.take (int (Frame.indexForKey key data))
            |> Frame.takeLast rows

        static member getRowKeys (frame:Frame<_,_>) : seq<'a> =
            frame.RowKeys

    type Series<'K, 'V when 'K : equality> with

        (*
        let RiskMetrics lambda startingVariance (series:Series<DateTime,float>) =
            let next pastVariance' pastReturn' =
                match pastVariance', pastReturn' with
                | Some pastVariance, Some pastReturn -> Some (lambda * pastVariance + (1.-lambda) * pastReturn ** 2.)
                | _ -> pastVariance'
            Series.shift 1 series
            |> Series.scanAllValues next (Some startingVariance)
        *)
