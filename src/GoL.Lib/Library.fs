namespace GoL.Lib
open System.Collections.Generic
module Points =
    type Point = {
        X:int
        Y:int
    } with override this.ToString() = sprintf "X:%i Y:%i" this.X this.Y
    let point (xy:int * int) = { X = fst xy; Y = snd xy}
module Cells =
    open Points
    type CellData = {
        Position:Point
    }
    let createCellData (pos: int * int) = { Position = point pos}
    type Cell =
    | Alive of CellData
    | Dead of CellData
    let unwrapData cell =
        match cell with
        | Alive c | Dead c -> c
    let isAlive cell =
        match cell with
        | Dead _ -> false
        | _ -> true
    let aliveCell cellData = Alive cellData
    let deadCell cellData = Dead cellData

    let cellConstructor isAlive =
        match isAlive with
        | true -> aliveCell
        | false -> deadCell  
    let createCell cell isAlive =
        let constructor = cellConstructor isAlive    
        constructor (unwrapData cell)
    
    let unwrapCellData (cell:Cell) = 
        match cell with
        | Alive cellData | Dead cellData -> cellData

    let evolveCell cell aliveNeighborsCount =
        match aliveNeighborsCount with
        | 2 when (isAlive cell) -> aliveCell (cell |> unwrapData)        
        | 3 -> aliveCell (cell |> unwrapData)
        | _ when (aliveNeighborsCount > 3 || aliveNeighborsCount < 2) -> deadCell (cell |> unwrapCellData)
        | _ -> createCell cell false
    
    
module Boards =
    open Points
    open Cells
    type Board = {
        Cells:IDictionary<Point,Cell>
        Size:Point
    }
    let board (cells:Cell array) =
        let _cells = cells |> Seq.map (fun c -> ((unwrapCellData c).Position,c)) |> dict    
        let width = (unwrapCellData cells.[cells.Length - 1]).Position.X
        let height = (unwrapCellData cells.[cells.Length - 1]).Position.Y
        { Cells = _cells; Size = point (width,height)}
    
    let isOffbounds (bo:Board) (pos:Point) = pos.X > bo.Size.X || pos.X < bo.Size.X || pos.Y < bo.Size.Y || pos.Y > bo.Size.Y
    let mapToRange value limit =
        let v = value % limit
        match v with
        | _ when v <= 0 -> limit
        | _ when v >= limit -> 0
        | _ -> v
    let floorPos (bo:Board) (pos:Point) = 
        let x = mapToRange (pos.X % bo.Size.X) bo.Size.X
        let y = mapToRange (pos.Y % bo.Size.Y) bo.Size.Y
        let point = point ((if x = 0 then 1 else x),(if y = 0 then 1 else y))
        point
    let countAliveNeighbors (bo:Board) (cellNode:KeyValuePair<Point,Cell>) =
        let getNeighbors = seq {
            for x in [-1..1] do
                for y in [-1..1] do                    
                    let neighPos = (point (cellNode.Key.X + x,cellNode.Key.Y + y))
                    if not (neighPos = cellNode.Key) then 
                        if (not (isOffbounds bo neighPos)) then
                            yield bo.Cells.[neighPos]
                        else
                            let flooredPos = (floorPos bo neighPos)
                            yield bo.Cells.[flooredPos]
            }
        let neighbors = getNeighbors
        neighbors |> Seq.filter (isAlive) |> Seq.length
        
    let evolve (bo:Board) =
        let generateNext (cells:IDictionary<Point,Cell>) = seq {
            for cellNode in cells do
                let neighborsAliveCount = countAliveNeighbors bo cellNode
                yield (evolveCell cellNode.Value neighborsAliveCount)            
        }
        let newGen = generateNext bo.Cells
        board (newGen |> Seq.toArray)
    let drawBoard bo =
        for x in [1..bo.Size.X] do
            for y in [1..bo.Size.Y] do
                let ce = (bo.Cells.Item (point (x,y)))
                match ce with
                | Alive a ->  printf "#"
                | Dead d -> printf "-"
            printfn ""
module IO =
    open Cells
    open Points
    //type LifeParser = (char -> Point -> Cell)
    let readPatternFile parser filePath =
        let dead,alive = '.','*'        
        let lines = (System.IO.File.ReadAllLines filePath)
        seq {
            for (x,line) in (lines |> Array.mapi (fun i l -> (i + 1,l))) do
                yield! (line |> Seq.mapi (fun y ch -> parser ch (x,y + 1)))
        }