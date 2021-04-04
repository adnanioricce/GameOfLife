import { printf, toText } from "../GoL.Web/src/.fable/fable-library.3.1.1/String.js";
import { Union, Record } from "../GoL.Web/src/.fable/fable-library.3.1.1/Types.js";
import { array_type, class_type, union_type, record_type, int32_type } from "../GoL.Web/src/.fable/fable-library.3.1.1/Reflection.js";
import { Dictionary } from "../GoL.Web/src/.fable/fable-library.3.1.1/MutableMap.js";
import { filter, length, rangeNumber, empty, singleton, collect, delay, map } from "../GoL.Web/src/.fable/fable-library.3.1.1/Seq.js";
import { safeHash, equals } from "../GoL.Web/src/.fable/fable-library.3.1.1/Util.js";
import { last } from "../GoL.Web/src/.fable/fable-library.3.1.1/Array.js";
import { getItemFromDict } from "../GoL.Web/src/.fable/fable-library.3.1.1/MapUtil.js";
import { ofSeq } from "../GoL.Web/src/.fable/fable-library.3.1.1/List.js";

export class Points_Point extends Record {
    constructor(X, Y) {
        super();
        this.X = (X | 0);
        this.Y = (Y | 0);
    }
    toString() {
        const this$ = this;
        return toText(printf("X:%i Y:%i"))(this$.X)(this$.Y);
    }
}

export function Points_Point$reflection() {
    return record_type("GoL.Lib.Points.Point", [], Points_Point, () => [["X", int32_type], ["Y", int32_type]]);
}

export function Points_point(xy_0, xy_1) {
    const xy = [xy_0, xy_1];
    return new Points_Point(xy[0], xy[1]);
}

export class Cells_CellData extends Record {
    constructor(Position) {
        super();
        this.Position = Position;
    }
}

export function Cells_CellData$reflection() {
    return record_type("GoL.Lib.Cells.CellData", [], Cells_CellData, () => [["Position", Points_Point$reflection()]]);
}

export function Cells_createCellData(pos_0, pos_1) {
    const pos = [pos_0, pos_1];
    return new Cells_CellData(Points_point(pos[0], pos[1]));
}

export class Cells_Cell extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Alive", "Dead", "Empty"];
    }
}

export function Cells_Cell$reflection() {
    return union_type("GoL.Lib.Cells.Cell", [], Cells_Cell, () => [[["Item", Cells_CellData$reflection()]], [["Item", Cells_CellData$reflection()]], [["Item", Cells_CellData$reflection()]]]);
}

export const Cells_empty = new Cells_Cell(2, Cells_createCellData(-1, -1));

export function Cells_unwrapData(cell) {
    let pattern_matching_result, c;
    switch (cell.tag) {
        case 1: {
            pattern_matching_result = 0;
            c = cell.fields[0];
            break;
        }
        case 2: {
            pattern_matching_result = 0;
            c = cell.fields[0];
            break;
        }
        default: {
            pattern_matching_result = 0;
            c = cell.fields[0];
        }
    }
    switch (pattern_matching_result) {
        case 0: {
            return c;
        }
    }
}

export function Cells_isAlive(cell) {
    if (cell.tag === 0) {
        const a = cell.fields[0];
        return true;
    }
    else {
        return false;
    }
}

export function Cells_aliveCell(cellData) {
    return new Cells_Cell(0, cellData);
}

export function Cells_deadCell(cellData) {
    return new Cells_Cell(1, cellData);
}

export function Cells_cellConstructor(isAlive) {
    if (isAlive) {
        return Cells_aliveCell;
    }
    else {
        return Cells_deadCell;
    }
}

export function Cells_createCell(cell, isAlive) {
    const constructor = Cells_cellConstructor(isAlive);
    return constructor(Cells_unwrapData(cell));
}

export function Cells_unwrapCellData(cell) {
    let pattern_matching_result, cellData;
    switch (cell.tag) {
        case 1: {
            pattern_matching_result = 0;
            cellData = cell.fields[0];
            break;
        }
        case 2: {
            pattern_matching_result = 0;
            cellData = cell.fields[0];
            break;
        }
        default: {
            pattern_matching_result = 0;
            cellData = cell.fields[0];
        }
    }
    switch (pattern_matching_result) {
        case 0: {
            return cellData;
        }
    }
}

export function Cells_evolveCell(cell, aliveNeighborsCount) {
    let pattern_matching_result;
    if (aliveNeighborsCount === 2) {
        if (Cells_isAlive(cell)) {
            pattern_matching_result = 0;
        }
        else {
            pattern_matching_result = 1;
        }
    }
    else {
        pattern_matching_result = 1;
    }
    switch (pattern_matching_result) {
        case 0: {
            return Cells_aliveCell(Cells_unwrapData(cell));
        }
        case 1: {
            if (aliveNeighborsCount === 3) {
                return Cells_aliveCell(Cells_unwrapData(cell));
            }
            else if ((aliveNeighborsCount > 3) ? true : (aliveNeighborsCount < 2)) {
                return Cells_deadCell(Cells_unwrapCellData(cell));
            }
            else {
                return Cells_deadCell(Cells_unwrapCellData(cell));
            }
        }
    }
}

export class Boards_Board extends Record {
    constructor(Cells, LiveCells, Size) {
        super();
        this.Cells = Cells;
        this.LiveCells = LiveCells;
        this.Size = Size;
    }
}

export function Boards_Board$reflection() {
    return record_type("GoL.Lib.Boards.Board", [], Boards_Board, () => [["Cells", class_type("System.Collections.Generic.IDictionary`2", [Points_Point$reflection(), Cells_Cell$reflection()])], ["LiveCells", array_type(Cells_Cell$reflection())], ["Size", Points_Point$reflection()]]);
}

export function Boards_board(cells) {
    const _cells = new Dictionary(map((c) => [Cells_unwrapCellData(c).Position, c], cells), {
        Equals: equals,
        GetHashCode: safeHash,
    });
    const width = Cells_unwrapCellData(last(cells)).Position.X | 0;
    const height = Cells_unwrapCellData(last(cells)).Position.Y | 0;
    const Size = Points_point(width, height);
    return new Boards_Board(_cells, cells.filter(Cells_isAlive), Size);
}

export function Boards_isOffbounds(bo, pos) {
    if (((pos.X > bo.Size.X) ? true : (pos.X < bo.Size.X)) ? true : (pos.Y < bo.Size.Y)) {
        return true;
    }
    else {
        return pos.Y > bo.Size.Y;
    }
}

export function Boards_mapToRange(value, limit) {
    const v = (value % limit) | 0;
    if (v <= 0) {
        return limit | 0;
    }
    else if (v >= limit) {
        return 0;
    }
    else {
        return v | 0;
    }
}

export function Boards_floorPos(bo, pos) {
    const x = Boards_mapToRange(pos.X % bo.Size.X, bo.Size.X) | 0;
    const y = Boards_mapToRange(pos.Y % bo.Size.Y, bo.Size.Y) | 0;
    const point = Points_point((x === 0) ? 1 : x, (y === 0) ? 1 : y);
    return point;
}

export function Boards_countAliveNeighbors(bo, cellNode) {
    const getCell = (pos) => {
        if (bo.Cells.has(pos)) {
            return getItemFromDict(bo.Cells, pos);
        }
        else {
            return Cells_empty;
        }
    };
    const getNeighbors = delay(() => collect((x) => collect((y) => {
        const cell_1 = Cells_unwrapCellData(cellNode);
        const neighPos = Points_point(cell_1.Position.X + x, cell_1.Position.Y + y);
        if (!equals(neighPos, cell_1.Position)) {
            if (!Boards_isOffbounds(bo, neighPos)) {
                return singleton(getItemFromDict(bo.Cells, neighPos));
            }
            else {
                const flooredPos = Boards_floorPos(bo, neighPos);
                return singleton(getItemFromDict(bo.Cells, flooredPos));
            }
        }
        else {
            return empty();
        }
    }, ofSeq(rangeNumber(-1, 1, 1))), ofSeq(rangeNumber(-1, 1, 1))));
    return length(filter(Cells_isAlive, getNeighbors)) | 0;
}

export function Boards_evolve(bo) {
    const replace = (nextList) => map((i) => {
        if (nextList.has(i[1])) {
            return getItemFromDict(nextList, i[1]);
        }
        else {
            return i[1];
        }
    }, bo.Cells);
    const generateNext = (cells) => {
        const cells_1 = delay(() => collect((cellNode) => {
            const neighborsAliveCount = Boards_countAliveNeighbors(bo, cellNode) | 0;
            return singleton([cellNode, Cells_evolveCell(cellNode, neighborsAliveCount)]);
        }, cells));
        return new Dictionary(cells_1, {
            Equals: equals,
            GetHashCode: safeHash,
        });
    };
    const newGen = generateNext(Array.from(map((c) => c[1], bo.Cells)));
    return Boards_board(Array.from(map((c_1) => c_1[1], newGen)));
}

