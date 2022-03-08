using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ASPPiece", menuName = "ASP/Piece")]
public class ASPPiece : ScriptableObject
{
    public string Name;
    public Move[] Moves;
    public Vector2Int Start;
    [System.Serializable]
    public struct Move
    {
        public int dx, dy;
    }
}
