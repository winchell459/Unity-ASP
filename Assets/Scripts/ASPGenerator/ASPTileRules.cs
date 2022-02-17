using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileRule", menuName = "ASP/TileRules/TileRule")]
public class ASPTileRules : ScriptableObject
{
    [System.Serializable]
    public struct ASPTile
    {
        public string name;
        public Sprite sprite;
        public States[] neighbors;
    }

    public ASPTile[] Tiles;

    public enum States
    {
        either,
        filled,
        empty
    }

    public virtual string GetTileRules()
    {
        string tile_rules = "";
        //generate tiles rules here

        return tile_rules;
    }

    protected List<bool[]> getMissingTiles(ASPTile[] tileRules)
    {
        List<bool[]> missingTiles = new List<bool[]>();
        for(int i = 0; i < 256; i += 1)
        {
            bool[] permutation = new bool[8];
            int num = i;
            for(int j = 7; j >= 0; j -= 1)
            {
                int placeValue = num / (int)Mathf.Pow(2, j);
                if (placeValue == 1) permutation[j] = true;
                num = num % (int)Mathf.Pow(2, j);
            }

            bool missing = true;
            foreach(ASPTile tileRule in tileRules)
            {
                bool found = true;
                for(int j = 0; j < 8; j += 1)
                {
                    if (tileRule.neighbors[j] != States.either && permutation[j] != (tileRule.neighbors[j] == States.filled)) found = false;
                }
                if (found) missing = false;
            }
            if (missing) missingTiles.Add(permutation);
        }


        return missingTiles;
    }
}
