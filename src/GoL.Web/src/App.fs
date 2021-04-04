module App
open Fable.Core
open Fable.Import
open Browser.Dom
open GoL.Lib.Boards
open GoL.Lib.Cells
open GoL.Lib.Points
// Mutable variable to count the number of times we clicked the button
let width = 64
let height = 64
let mutable lifeWindow = ([1..width] |> Seq.map (fun s -> "." |> (String.replicate height))) |> Seq.toArray
let coords =
    let x = height / 2    
    let y = width / 2    
    [|(x,y);(x - 1,y);(x,y - 1);(x,y + 1);(x + 1,y - 1)|]
let toCell po =
    let x,y = po
    match coords |> Array.tryFind (fun p -> p = (x,y)) with
    | Some v -> aliveCell (createCellData (x,y))
    | None -> deadCell (createCellData (x,y))
let pointsToFill = seq {
    for (x,line) in (lifeWindow |> Array.mapi (fun i l -> (i + 1,l))) do
        yield! (line |> Seq.mapi (fun y c -> (toCell (x,y + 1))))
}
let mutable gameBoard = board (pointsToFill |> Seq.toArray)
let drawBoard (bo:Board) (p:p5) = 
    for cell in bo.LiveCells do
        let data = (unwrapCellData cell)
        p.fill(0.0 |> U4.Case1)
        p.rect(float data.Position.X * 16.0,float data.Position.Y * 16.0,16.0,16.0) |> ignore    
let mutable counter = 0
let sketch =
    new System.Func<obj,unit>(
        fun o ->
            let p = o |> unbox<p5>
            p.setup <- fun () ->
                p.createCanvas(window.innerWidth,window.innerHeight) |> ignore
                drawBoard gameBoard p
                
                
            p.draw <- fun () ->
                p.background(255.0 |> U4.Case1)                
                gameBoard <- gameBoard |> evolve
                counter <- counter + 1
                drawBoard gameBoard p                
                p.textSize(32.0) |> ignore
                p.fill(0.0 |> U4.Case1)             
                p.text(sprintf "generation:%i" counter,window.innerWidth - 720.0,64.0,32.0,90.0) |> ignore
                p.textSize(32.0) |> ignore
                p.fill(0.0 |> U4.Case1)                
                p.text(sprintf "population:%i" (gameBoard.LiveCells |> Seq.length),window.innerWidth - 720.0,96.0,32.0,90.0) |> ignore
                ()
    )

let entrypoint = p5(sketch)