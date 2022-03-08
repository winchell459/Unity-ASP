using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : ASPGenerator
{
    [SerializeField] int boardWidth = 10, boardHeight = 10;
    [SerializeField] ASPPiece[] pieces;

    public enum tile_types
    {
        filled,
        empty
    }
    protected override string getASPCode()
    {
        return fieldrules + pathRules + ((MapKeyTileRule)mapKey).dict["empty"].GetTileRules() + checkeredRules;
    }

    string fieldrules = $@"

        #const max_width = 40.
        #const max_height = 40.

        width(1..max_width).
        height(1..max_height).

        tile_type({tile_types.filled};{tile_types.empty}).

        1{{tile(XX, YY, Type): tile_type(Type)}}1 :- width(XX), height(YY).

";
    string pathRules { get { return generatePathRules(); } }

    string generatePathRules()
    {

        string aspCode = $@"

        1{{start(XX,YY): tile(XX,YY,{tile_types.empty})}}1.
        1{{end(XX,YY): tile(XX,YY,{tile_types.empty})}}1.
        :- start(XX,YY), end(XX,YY).

        path(XX,YY) :- start(XX,YY).

        %% add empty to path if neighboring path
        path(XX,YY) :- tile(XX,YY, {tile_types.empty}), path(XX-1,YY).
        path(XX,YY) :- tile(XX,YY, {tile_types.empty}), path(XX+1,YY).
        path(XX,YY) :- tile(XX,YY, {tile_types.empty}), path(XX,YY-1).
        path(XX,YY) :- tile(XX,YY, {tile_types.empty}), path(XX,YY+1).

        :- end(XX,YY), not path(XX,YY).


        

        ";
        bool degugging = true;
        string aspCodeDebug = $@"
            :- start(XX,_), XX != max_width.
            :- end(XX,_), XX != 1.

            :- start(_,YY), YY != max_height-1.
            :- end(_,YY), YY != 2.
        ";

        return aspCode + (degugging? aspCodeDebug:"");
    }

    string checkeredRules { get { return generateCheckeredRules(boardWidth,boardHeight); } }

    string generateCheckeredRules(int boardWidth, int boardHeight)
    {
        string aspCode = $@"
            
        ";

        for(int y = 1; y <= boardHeight; y += 1)
        {
            for(int x = 1; x <= boardWidth; x += 1)
            {
                string tileColor = "white";
                if(Mathf.Abs(x-y)%2 == 1)
                {
                    tileColor = "black";
                }
                //aspCode += $" 0{{checkered({x},{y},{tileColor})}}1 :- tile({x},{y},{tile_types.filled}).\n";
                aspCode += $" checkered({x},{y},{tileColor}) :- piece_path({x},{y}), tile({x},{y},{tile_types.filled}).\n";
            }
        }

        return aspCode + piecePathRules;
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
        foreach(ASPPiece piece in pieces)
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
        foreach(ASPPiece.Move move in piece.Moves)
        {
            aspCode += $"{piece.Name}_path(XX,YY) :- {piece.Name}_path(XX+{move.dx}, YY+{move.dy}), tile(XX,YY,{tile_types.filled}).";
        }

        return aspCode;
    }

    protected override void startGenerator()
    {
        string filename = Clingo.ClingoUtil.CreateFile(aspCode);
        Solver.Solve(filename, getAdditionalParameters());
        waitingOnClingo = true;
    }

    protected override string getAdditionalParameters()
    {
        return $" -c max_width={boardWidth} -c max_height={boardHeight} " + base.getAdditionalParameters();
    }
}

