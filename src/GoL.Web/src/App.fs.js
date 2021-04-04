import { equalArrays, createAtom } from "./.fable/fable-library.3.1.1/Util.js";
import { length, mapIndexed, collect, delay, rangeNumber, map } from "./.fable/fable-library.3.1.1/Seq.js";
import { printf, toText, replicate } from "./.fable/fable-library.3.1.1/String.js";
import { ofSeq } from "./.fable/fable-library.3.1.1/List.js";
import { mapIndexed as mapIndexed_1, tryFind } from "./.fable/fable-library.3.1.1/Array.js";
import { Boards_evolve, Cells_unwrapCellData, Boards_board, Cells_aliveCell, Cells_createCellData, Cells_deadCell } from "../../GoL.Lib/Library.fs.js";
import * as p5 from "p5";

export const width = 64;

export const height = 64;

export const lifeWindow = createAtom(Array.from(map((s) => replicate(height, "."), ofSeq(rangeNumber(1, 1, width)))));

export const coords = (() => {
    const x = (~(~(height / 2))) | 0;
    const y = (~(~(width / 2))) | 0;
    return [[x, y], [x - 1, y], [x, y - 1], [x, y + 1], [x + 1, y - 1]];
})();

export function toCell(po_0, po_1) {
    const po = [po_0, po_1];
    const y = po[1] | 0;
    const x = po[0] | 0;
    const matchValue = tryFind((p) => equalArrays(p, [x, y]), coords);
    if (matchValue == null) {
        return Cells_deadCell(Cells_createCellData(x, y));
    }
    else {
        const v = matchValue;
        return Cells_aliveCell(Cells_createCellData(x, y));
    }
}

export const pointsToFill = delay(() => collect((matchValue) => {
    const x = matchValue[0] | 0;
    const line = matchValue[1];
    return mapIndexed((y, c) => toCell(x, y + 1), line.split(""));
}, mapIndexed_1((i, l) => [i + 1, l], lifeWindow())));

export const gameBoard = createAtom(Boards_board(Array.from(pointsToFill)));

export function drawBoard(bo, p) {
    const arr = bo.LiveCells;
    for (let idx = 0; idx <= (arr.length - 1); idx++) {
        const cell = arr[idx];
        const data = Cells_unwrapCellData(cell);
        p.fill(0);
        const value = p.rect(data.Position.X * 16, data.Position.Y * 16, 16, 16);
        void value;
    }
}

export const counter = createAtom(0);

export const sketch = (o) => {
    const p = o;
    p.setup = (() => {
        const value_1 = p.createCanvas(window.innerWidth, window.innerHeight);
        void value_1;
        drawBoard(gameBoard(), p);
    });
    p.draw = (() => {
        let arg10, arg10_1;
        p.background(255);
        gameBoard(Boards_evolve(gameBoard()), true);
        counter(counter() + 1, true);
        drawBoard(gameBoard(), p);
        p.textSize(32);
        p.fill(0);
        p.text((arg10 = (counter() | 0), toText(printf("generation:%i"))(arg10)), window.innerWidth - 720, 64, 32, 90);
        p.textSize(32);
        p.fill(0);
        p.text((arg10_1 = (length(gameBoard().LiveCells) | 0), toText(printf("population:%i"))(arg10_1)), window.innerWidth - 720, 96, 32, 90);
    });
};

export const entrypoint = new p5(sketch);

