using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : ASPGenerator
{
    [SerializeField] int boardWidth = 10, boardHeight = 10;
    [SerializeField] ASPTileRules terrainTiles;

    public enum tile_types
    {
        filled,
        empty
    }
    protected override string getASPCode()
    {
        return fieldrules + pathRules + pathDebugRules + terrainTiles.GetTileRules();
    }

    string fieldrules = $@"

        #const max_width = 10.
        #const max_height = 10.

        width(1..max_width).
        height(1..max_height).

        tile_type({tile_types.filled};{tile_types.empty}).

        1{{tile(XX, YY, Type): tile_type(Type)}}1 :- width(XX), height(YY).

";
    string pathRules = $@"

        1{{start(XX,YY): tile(XX,YY,{tile_types.empty})}}1.
        1{{end(XX,YY): tile(XX,YY,{tile_types.empty})}}1.
        :- start(XX,YY), end(XX,YY).

        path(XX,YY) :- start(XX,YY).

        %% add empty to path if neighboring path
        path(XX,YY) :- tile(XX,YY, {tile_types.empty}), path(XX-1,YY).
        path(XX,YY) :- tile(XX,YY, {tile_types.empty}), path(XX+1,YY).
        path(XX,YY) :- tile(XX,YY, {tile_types.empty}), path(XX,YY-1).
        path(XX,YY) :- tile(XX,YY, {tile_types.empty}), path(XX,YY+1).

        :- end(XX,YY), not path(XX,YY).




    ";

    string pathDebugRules = $@"

        :- end(XX,_), not XX == max_width.
        :- start(XX,_), not XX == 1.
        %:- end(_,YY), not YY == max_height.
        %:- start(_,YY), not YY == 1.

        

        :- tile(XX,YY,{tile_types.empty}), not path(XX,YY).
        :- Count = {{tile(_,_,Type)}}, tile_type(Type), Count == 0.

        checkard_tiles(white; black).
        :- Count = {{checkard(_,_,Type)}}, checkard_tiles(Type), Count == 0.

        checkard(XX,YY,white) :- tile(XX,YY,{tile_types.filled}), XX == YY.
        checkard(XX,YY,black) :- tile(XX,YY,{tile_types.filled}), XX == YY + 1.

        checkard(XX,YY,white) :- tile(XX,YY,{tile_types.filled}), checkard(XX,YY + J, white), J = 2*(1..max_height).
        checkard(XX,YY,white) :- tile(XX,YY,{tile_types.filled}), checkard(XX + I,YY, white), I = 2*(1..max_width).
        checkard(XX,YY,white) :- tile(XX,YY,{tile_types.filled}), checkard(XX,YY - J, white), J = 2*(1..max_height).
        checkard(XX,YY,white) :- tile(XX,YY,{tile_types.filled}), checkard(XX - I,YY, white), I = 2*(1..max_width).

        checkard(XX,YY,white) :- tile(XX,YY,{tile_types.filled}), checkard(XX - D,YY - D, white), D = (1..max_width).
        checkard(XX,YY,white) :- tile(XX,YY,{tile_types.filled}), checkard(XX + D,YY - D, white), D = (1..max_width).
        checkard(XX,YY,white) :- tile(XX,YY,{tile_types.filled}), checkard(XX + D,YY + D, white), D = (1..max_width).
        checkard(XX,YY,white) :- tile(XX,YY,{tile_types.filled}), checkard(XX - D,YY + D, white), D = (1..max_width).

        checkard(XX,YY,black) :- tile(XX,YY,{tile_types.filled}), checkard(XX,YY + J, black), J = 2*(1..max_height).
        checkard(XX,YY,black) :- tile(XX,YY,{tile_types.filled}), checkard(XX + I,YY, black), I = 2*(1..max_width).
        checkard(XX,YY,black) :- tile(XX,YY,{tile_types.filled}), checkard(XX,YY - J, black), J = 2*(1..max_height).
        checkard(XX,YY,black) :- tile(XX,YY,{tile_types.filled}), checkard(XX - I,YY, black), I = 2*(1..max_width).

        checkard(XX,YY,black) :- tile(XX,YY,{tile_types.filled}), checkard(XX - D,YY - D, black), D = (1..max_width).
        checkard(XX,YY,black) :- tile(XX,YY,{tile_types.filled}), checkard(XX + D,YY - D, black), D = (1..max_width).
        checkard(XX,YY,black) :- tile(XX,YY,{tile_types.filled}), checkard(XX + D,YY + D, black), D = (1..max_width).
        checkard(XX,YY,black) :- tile(XX,YY,{tile_types.filled}), checkard(XX - D,YY + D, black), D = (1..max_width).

        %:- Count = {{checkard(_,_,_)}}, Count < 64.
    ";

    

    protected override void startGenerator()
    {
        string filename = Clingo.ClingoUtil.CreateFile(aspCode);
        Solver.Solve(filename, getAdditionalParameters());
        waitingOnClingo = true;
    }

    protected override string getAdditionalParameters()
    {
        return $"-c max_width={boardWidth} -c max_height={boardHeight} " + base.getAdditionalParameters();
    }
}