open GoL.Lib.Points
open GoL.Lib.Cells
open GoL.Lib.Boards
open GoL.Lib.IO
open GoL.Lib.Cells
open System.Collections.Generic
let parser value position =
    match value with
    | '.' when value = '.' -> deadCell (createCellData position)
    | '*' when value = '*' -> aliveCell (createCellData position)
    | _ -> failwithf "error on position %O" position
// Define a function to construct a message to print       
//TODO:Test other patterns
[<EntryPoint>]
let main argv =
    let rec loop transform state count =
        if count = 0 then
            evolve state
        else
            transform state
            loop transform (evolve state) (count - 1)    
    let checkerboard = board ((readPatternFile parser "./Content/Patterns/txt/glider.txt") |> Seq.toArray)    
    (loop (fun state -> 
        printfn ""
        state |> drawBoard) checkerboard 3) |> ignore
    0