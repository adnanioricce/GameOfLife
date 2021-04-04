open GoL.Lib.Points
open GoL.Lib.Cells
open GoL.Lib.Boards
open GoL.Lib.Utils.IO
open GoL.Lib.Cells
open System.Collections.Generic
let parser value position =
    match value with
    | '.' when value = '.' -> deadCell (createCellData position)
    | '*' when value = '*' -> aliveCell (createCellData position)
    | _ -> failwithf "error on position %O" position

let drawBoard bo =
    for x in [1..bo.Size.X] do
        for y in [1..bo.Size.Y] do
            let ce = (bo.Cells.Item (point (x,y)))
            match ce with
            | Alive a ->  printf "#"
            | Dead d -> printf "-"
        printfn ""
// Define a function to construct a message to print       
//TODO:Test other patterns
[<EntryPoint>]
let main argv =
    let pattern = if argv.Length = 0 then None else Some (argv.[0] |> int)
    // match pattern with
    // | 1 -> 
    let rec loop transform state count =
        if count = 0 then
            evolve state
        else
            transform state
            loop transform (evolve state) (count - 1)    
    let checkerboard = board ((readPatternFile parser "./Content/Patterns/txt/r_pentomino.txt") |> Seq.toArray)    
    let rec loopEvolve state =
        let input = System.Console.ReadKey()
        match input.Key with
        | System.ConsoleKey.Escape -> ()
        | _ -> 
            printfn ""
            drawBoard state
            loopEvolve (state |> evolve)        
    printfn "press escape to leave or enter any key to continue"
    loopEvolve checkerboard    
    0