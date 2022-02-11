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
        public bool[] neighbors;
    }

    public ASPTile[] Tiles;

    public string GetTileRules()
    {
        string tile_rules = "";
        //generate tiles rules here

        return tile_rules;
    }
}
