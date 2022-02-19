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

    protected virtual string getCenterTileType()
    {
        return "empty";
    }

    protected virtual string getNeighborTileType()
    {
        return "filled";
    }

    public virtual string GetTileRules()
    {
        string tile_rules = "";
        //generate tiles rules here
        List<bool[]> missingRules = getMissingTiles(Tiles);

        Debug.Log("missingRules.Count: " + missingRules.Count);
        

        foreach (bool[] missingTile in missingRules)
        {
            tile_rules += $@"
                :- tile(XX,YY,{getCenterTileType()}),
                {getNot(missingTile[0])} tile(XX-1, YY+1, {getNeighborTileType()}),
                {getNot(missingTile[1])} tile(XX, YY+1, {getNeighborTileType()}),
                {getNot(missingTile[2])} tile(XX+1, YY+1, {getNeighborTileType()}),
                {getNot(missingTile[3])} tile(XX-1, YY, {getNeighborTileType()}),
                {getNot(missingTile[4])} tile(XX+1, YY, {getNeighborTileType()}),
                {getNot(missingTile[5])} tile(XX-1, YY-1, {getNeighborTileType()}),
                {getNot(missingTile[6])} tile(XX, YY-1, {getNeighborTileType()}),
                {getNot(missingTile[7])} tile(XX+1, YY-1, {getNeighborTileType()}).
                
            ";
        }

        return tile_rules;
    }
    string getNot(bool isEmpty)
    {
        if (isEmpty) return "not";
        else return "";
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
