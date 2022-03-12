using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : ASPGenerator
{
    [SerializeField] protected int boardWidth = 10, boardHeight = 10;


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
    string pathRules { get { return generatePathRules(); } }

    string generatePathRules()
    {

        string aspCode = $@"

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
        bool degugging = true;
        string aspCodeDebug = $@"
            :- start(XX,_), XX != max_width.
            :- end(XX,_), XX != 1.

            :- start(_,YY), YY != max_height-1.
            :- end(_,YY), YY != 2.
        ";

        return aspCode + (degugging? aspCodeDebug:"");
    }

    

    protected override void startGenerator()
    {
        string filename = Clingo.ClingoUtil.CreateFile(aspCode);
        Solver.Solve(filename, getAdditionalParameters());
        waitingOnClingo = true;
    }

    protected override string getAdditionalParameters()
    {
        return $" -c max_width={boardWidth} -c max_height={boardHeight} " + base.getAdditionalParameters();
    }
}

