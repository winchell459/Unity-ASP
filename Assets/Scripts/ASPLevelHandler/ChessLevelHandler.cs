using System.Collections;
using System.Collections.Generic;
using Clingo;
using UnityEngine;

public class ChessLevelHandler : ASPLevelHandler
{
    [SerializeField] ASPGenerator generator;
    [SerializeField] MapKeyTileRule mapKeyTileRule;
    [SerializeField] MapTileRule mapTileRule;
    [SerializeField] MapPixel mapPixel;
    [SerializeField] MapKeyPixel mapKeyPixel;

    protected override void ERROR(string error, string jobID)
    {
        Debug.LogWarning(error);
    }

    protected override void SATISFIABLE(AnswerSet answerSet, string jobID)
    {
        mapTileRule.DisplayMap(answerSet, mapKeyTileRule);
        mapTileRule.AdjustCamera();
        mapPixel.DisplayMap(answerSet, mapKeyPixel);
    }

    protected override void TIMEDOUT(int time, string jobID)
    {
        throw new System.NotImplementedException();
    }

    protected override void UNSATISFIABLE(string jobID)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        initializeGenerator(generator);
        generator.StartGenerator();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
