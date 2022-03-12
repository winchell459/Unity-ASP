using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckeredPathGenerator : PathGenerator
{
    
    
    protected override string getASPCode()
    {
        return base.getASPCode() + checkeredRules;
    }

    string checkeredRules { get { return generateCheckeredRules(boardWidth, boardHeight); } }

    string generateCheckeredRules(int boardWidth, int boardHeight)
    {
        string aspCode = $@"
            
        ";

        for (int y = 1; y <= boardHeight; y += 1)
        {
            for (int x = 1; x <= boardWidth; x += 1)
            {
                string tileColor = "white";
                if (Mathf.Abs(x - y) % 2 == 1)
                {
                    tileColor = "black";
                }
                //aspCode += $" 0{{checkered({x},{y},{tileColor})}}1 :- tile({x},{y},{tile_types.filled}).\n";
                aspCode += getCheckeredTileRule(x, y, tileColor);
            }
        }

        return aspCode;
    }

    protected virtual string getCheckeredTileRule(int x, int y, string tileColor)
    {
        return $" checkered({x},{y},{tileColor}) :-  tile({x},{y},{tile_types.filled}).\n";
    }
    
}
