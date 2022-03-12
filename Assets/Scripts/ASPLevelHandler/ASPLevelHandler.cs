using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ASPLevelHandler : MonoBehaviour
{
    virtual protected void initializeGenerator(ASPGenerator generator)
    {
        generator.InitializeGenerator(SATISFIABLE, UNSATISFIABLE, TIMEDOUT, ERROR);
    }

    protected abstract void SATISFIABLE(Clingo.AnswerSet answerSet, string jobID);
    protected abstract void UNSATISFIABLE(string jobID);
    protected abstract void TIMEDOUT(int time, string jobID);
    protected abstract void ERROR(string error, string jobID);
}
