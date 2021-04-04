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
    | Empty of CellData
    let empty = Empty (createCellData (-1,-1))
    let unwrapData cell =
        match cell with
        | Alive c | Dead c | Empty c -> c
    let isAlive cell =
        match cell with
        | Alive a -> true
        | _ -> false
    
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
        | Alive cellData | Dead cellData | Empty cellData -> cellData

    let evolveCell cell aliveNeighborsCount =
        match aliveNeighborsCount with
        | 2 when (isAlive cell) -> aliveCell (cell |> unwrapData)        
        | 3 -> aliveCell (cell |> unwrapData)
        | _ when (aliveNeighborsCount > 3 || aliveNeighborsCount < 2) -> deadCell (cell |> unwrapCellData)
        | _ -> deadCell (cell |> unwrapCellData)
    
    
module Boards =
    open Points
    open Cells
    type Board = {
        Cells:IDictionary<Point,Cell>
        LiveCells:Cell array        
        Size:Point
    }
    let board (cells:Cell array) =        
        let _cells = cells |> Seq.map (fun c -> ((unwrapCellData c).Position,c)) |> dict    
        let width = (unwrapCellData (cells |> Array.last)).Position.X
        let height = (unwrapCellData (cells |> Array.last)).Position.Y
        { Cells = _cells; Size = point (width,height);LiveCells = cells |> Array.filter isAlive}    
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
    let countAliveNeighbors (bo:Board) (cellNode:Cell) =
        let getCell pos =
            match bo.Cells.ContainsKey(pos) with
            | true -> bo.Cells.[pos]
            | false -> empty
        let getNeighbors = seq {
            for x in [-1..1] do
                for y in [-1..1] do
                    let cell = cellNode |> unwrapCellData
                    let neighPos = (point (cell.Position.X + x,cell.Position.Y + y))
                    if not (neighPos = cell.Position) then 
                        if (not (isOffbounds bo neighPos)) then                            
                            yield bo.Cells.[neighPos]
                        else
                            let flooredPos = (floorPos bo neighPos)
                            yield bo.Cells.[flooredPos]
            }        
        getNeighbors |> Seq.filter (isAlive) |> Seq.length
        
    let evolve (bo:Board) =
        let replace (nextList:IDictionary<Cell,Cell>)  =
            bo.Cells |> Seq.map (fun i -> if nextList.ContainsKey(i.Value) then nextList.[i.Value] else i.Value)
                
        let generateNext (cells:Cell array) = 
            let cells = seq {
                for cellNode in cells do
                    let neighborsAliveCount = countAliveNeighbors bo cellNode
                    yield (cellNode,(evolveCell cellNode neighborsAliveCount))
            }
            cells |> dict
        let newGen = (generateNext (bo.Cells |> Seq.map (fun c -> c.Value) |> Seq.toArray))
        board (newGen |> Seq.map (fun c -> c.Value) |> Seq.toArray)