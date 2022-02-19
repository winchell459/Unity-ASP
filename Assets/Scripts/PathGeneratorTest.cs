using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGeneratorTest : ASPGenerator
{
    public enum tile_types
    {
        filled,
        empty
    }
    protected override string getASPCode()
    {
        return fieldrules;
    }

    string fieldrules = $@"

        #const max_width = 20.
        #const max_height = 20.

        width(1..max_width).
        height(1..max_height).

        tile_type({tile_types.filled};{tile_types.empty}).

        1{{tile(XX, YY, Type): tile_type(Type)}}1 :- width(XX), height(YY).

";

    protected override void startGenerator()
    {
        string filename = Clingo.ClingoUtil.CreateFile(aspCode);
        Solver.Solve(filename);
        waitingOnClingo = true;
    }
}
