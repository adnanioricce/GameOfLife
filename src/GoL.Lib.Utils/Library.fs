namespace GoL.Lib.Utils

module IO =
    open GoL.Lib.Cells
    open GoL.Lib.Points
    //type LifeParser = (char -> Point -> Cell)
    let readPattern parser lines =
        seq {
            for (x,line) in (lines |> Array.mapi (fun i l -> (i + 1,l))) do
                yield! (line |> Seq.mapi (fun y ch -> parser ch (x,y + 1)))
        }
    let readPatternFile parser filePath =
        let lines = (System.IO.File.ReadAllLines filePath)
        readPattern parser lines
