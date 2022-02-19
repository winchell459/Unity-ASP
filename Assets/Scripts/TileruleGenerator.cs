using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileruleGenerator : ASPGenerator
{
    [SerializeField] protected ASPTileRules tileRules;

    protected override void startGenerator()
    {
        string filename = Clingo.ClingoUtil.CreateFile(aspCode + tileRules.GetTileRules());
        Solver.Solve(filename);
        waitingOnClingo = true;
    }

    protected override string getASPCode()
    {
        string aspCode = $@"

        #const max_width = 10.
        #const max_height = 10.

        tile_types(filled;empty).
        width(1..max_width).
        height(1..max_height).

        1{{tile(XX,YY,Type): tile_types(Type)}}1 :- width(XX), height(YY).

        ";
        return aspCode;
    }
}
