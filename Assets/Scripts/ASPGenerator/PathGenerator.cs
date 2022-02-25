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
        :- end(_,YY), YY == max_height.
        :- end(_,YY), YY == 1.
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