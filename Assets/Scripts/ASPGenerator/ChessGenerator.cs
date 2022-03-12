using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessGenerator : CheckeredPathGenerator
{
    [SerializeField] ASPPiece[] pieces;

    protected override string getASPCode()
    {
        return base.getASPCode() + piecePathRules;
    }

    protected override string getCheckeredTileRule(int x, int y, string tileColor)
    {
        return $" checkered({x},{y},{tileColor}) :- piece_path({x},{y}), tile({x},{y},{tile_types.filled}).\n";
    }

    string piecePathRules { get { return generatePiecePathRules(); } }

    string generatePiecePathRules()
    {
        //string aspCode = $@"

        //    piece_path(XX,YY) :- piece_path(XX+1, YY), tile(XX,YY,{tile_types.filled}).
        //    piece_path(XX,YY) :- piece_path(XX-1, YY), tile(XX,YY,{tile_types.filled}).
        //    piece_path(XX,YY) :- piece_path(XX, YY+1), tile(XX,YY,{tile_types.filled}).
        //    piece_path(XX,YY) :- piece_path(XX, YY-1), tile(XX,YY,{tile_types.filled}).

        //    :- checkered(XX,YY,_), not piece_path(XX,YY).

        //    piece_path(1,1) :- checkered(1,1,_).
        //";

        //return aspCode;


        //return generatePiecePathRules("king_white", new Vector2Int(2, 2)) + generatePiecePathRules("king_black", new Vector2Int(19, 19));
        string aspCode = "";
        foreach (ASPPiece piece in pieces)
        {
            aspCode += generatePiecePathRules(piece, piece.Start);
        }

        aspCode += $@" 

            king_overlap(XX,YY) :- king_white_path(XX,YY), king_black_path(XX,YY).
            :- not king_overlap(_,_).
        ";
        return aspCode;
    }
    string generatePiecePathRules(string piece, Vector2Int start)
    {
        string aspCode = $@"
            
            {piece}_path(XX,YY) :- {piece}_path(XX+1, YY), tile(XX,YY,{tile_types.filled}).
            {piece}_path(XX,YY) :- {piece}_path(XX-1, YY), tile(XX,YY,{tile_types.filled}).
            {piece}_path(XX,YY) :- {piece}_path(XX, YY+1), tile(XX,YY,{tile_types.filled}).
            {piece}_path(XX,YY) :- {piece}_path(XX, YY-1), tile(XX,YY,{tile_types.filled}).

            :- {piece}_path(XX,YY), not checkered(XX,YY,_).
            piece_path(XX,YY) :- {piece}_path(XX,YY).
            :- {piece}_path(XX,YY), not tile(XX,YY,{tile_types.filled}).
            
            {piece}_path({start.x},{start.y}).
        ";

        return aspCode;
    }

    string generatePiecePathRules(ASPPiece piece, Vector2Int start)
    {
        string aspCode = $@"
            
            :- {piece.Name}_path(XX,YY), not checkered(XX,YY,_).
            piece_path(XX,YY) :- {piece.Name}_path(XX,YY).
            :- {piece.Name}_path(XX,YY), not tile(XX,YY,{tile_types.filled}).
            
            {piece.Name}_path({start.x},{start.y}).
        ";
        foreach (ASPPiece.Move move in piece.Moves)
        {
            aspCode += $"{piece.Name}_path(XX,YY) :- {piece.Name}_path(XX+{move.dx}, YY+{move.dy}), tile(XX,YY,{tile_types.filled}).";
        }

        return aspCode;
    }
}
