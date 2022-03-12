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

    [SerializeField] States centerTile = States.empty, neighborTile = States.filled;

    public enum States
    {
        either,
        filled,
        empty
    }

    protected virtual string getCenterTileType()
    {
        return centerTile.ToString();
    }

    protected virtual string getNeighborTileType()
    {
        return neighborTile.ToString();
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
        if (!isEmpty) return "not";
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
                    if (tileRule.neighbors[j] != States.either && permutation[j] != (tileRule.neighbors[j] == neighborTile)) found = false;
                }
                if (found) missing = false;
            }
            if (missing) missingTiles.Add(permutation);
        }


        return missingTiles;
    }

    public Sprite GetTile(string[,] map, Vector2Int pos)
    {
        bool[] neighbors = getNeighbors(map, pos, neighborTile.ToString());
        Sprite sprite = null;
        foreach (ASPTile tileRule in Tiles)
        {
            if (isMatching(tileRule, neighbors) && !sprite) sprite = tileRule.sprite;
            else if (isMatching(tileRule, neighbors)) Debug.LogWarning("Multiple sprites matching.");
        }
        return sprite;
    }

    bool isMatching(ASPTile tile, bool[] neighbors)
    {
        bool match = true;
        for (int i = 0; i < 8; i += 1)
        {
            //if (tile.mustHave[i] && tile.emptyPlacement[i] != neighbors[i]) match = false;

            if (tile.neighbors[i] != States.either && neighbors[i] != (tile.neighbors[i] == centerTile)) match = false;

        }
        return match;
    }

    private bool[] getNeighbors(string[,] map, Vector2Int pos, string falseTileType)
    {
        bool[] neighbors = new bool[8];

        int h = pos.y;
        int w = pos.x;

        int width = map.GetUpperBound(0) + 1;
        int height = map.GetUpperBound(1) + 1;
        //Debug.Log($"height: {height} | width: {width}");
        //neighbors[0]
        if (h == height - 1 && w == 0) neighbors[0] = map[w, h] == falseTileType ? false : true;
        else if (h == height - 1) neighbors[0] = map[w - 1, h] == falseTileType ? false : true;
        else if (w == 0) neighbors[0] = map[w, h + 1] == falseTileType ? false : true;
        else neighbors[0] = w > 0 && h < height - 1 && map[w - 1, h + 1] == falseTileType ? false : true;

        //neighbors[1]
        if (h < height - 1) neighbors[1] = map[w, h + 1] == falseTileType ? false : true;
        else neighbors[1] = map[w, h] == falseTileType ? false : true;

        //neighbors[2]
        if (w == width - 1 && h == height - 1) neighbors[2] = map[w, h] == falseTileType ? false : true;
        else if (w == width - 1) neighbors[2] = map[w, h + 1] == falseTileType ? false : true;
        else if (h == height - 1) neighbors[2] = map[w + 1, h] == falseTileType ? false : true;
        else neighbors[2] = w < width - 1 && h < height - 1 && map[w + 1, h + 1] == falseTileType ? false : true;


        //neighbors[3]
        if (w > 0) neighbors[3] = map[w - 1, h] == falseTileType ? false : true;
        else neighbors[3] = map[w, h] == falseTileType ? false : true;
        //neighbors[4]
        if (w < width - 1) neighbors[4] = map[w + 1, h] == falseTileType ? false : true;
        else neighbors[4] = map[w, h] == falseTileType ? false : true;


        //neighbors[5]
        if (w == 0 && h == 0) neighbors[5] = map[w, h] == falseTileType ? false : true;
        else if (w == 0) neighbors[5] = map[w, h - 1] == falseTileType ? false : true;
        else if (h == 0) neighbors[5] = map[w - 1, h] == falseTileType ? false : true;
        else neighbors[5] = w > 0 && h > 0 && map[w - 1, h - 1] == falseTileType ? false : true;

        //neighbors[6]
        if (h > 0) neighbors[6] = map[w, h - 1] == falseTileType ? false : true;
        else neighbors[6] = map[w, h] == falseTileType ? false : true;

        //neighbors[7]
        if (w == width - 1 && h == 0) neighbors[7] = map[w, h] == falseTileType ? false : true;
        else if (w == width - 1) neighbors[7] = map[w, h - 1] == falseTileType ? false : true;
        else if (h == 0) neighbors[7] = map[w + 1, h] == falseTileType ? false : true;
        else neighbors[7] = w < width - 1 && h > 0 && map[w + 1, h - 1] == falseTileType ? false : true;

        return neighbors;
    }

    
}
