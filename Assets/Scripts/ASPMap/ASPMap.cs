using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ASPMap : MonoBehaviour
{
    protected int width, height;

    public abstract void DisplayMap(Dictionary<string, List<List<string>>> answerset, MapKey mapKey);
    public abstract void AdjustCamera();
}
