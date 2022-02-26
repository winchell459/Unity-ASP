using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : ASPGenerator
{
    

    public enum tile_types
    {
        filled,
        empty
    }
    protected override string getASPCode()
    {
        return fieldrules + pathRules + ((MapKeyTileRule)mapKey).dict["empty"].GetTileRules();
    }

    string fieldrules = $@"

        #const max_width = 40.
        #const max_height = 40.

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

   

    protected override void startGenerator()
    {
        string filename = Clingo.ClingoUtil.CreateFile(aspCode);
        Solver.Solve(filename);
        waitingOnClingo = true;
    }
}