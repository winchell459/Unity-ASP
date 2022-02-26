using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapKeyTileRule", menuName = "ASPMap/MapKey/MapKeyTileRule")]
public class MapKeyTileRule : MapKey
{
    public MapObjectKey<ASPTileRules> dict;
}
