using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASPGenerator : MonoBehaviour
{
    public Clingo.ClingoSolver Solver;
    [SerializeField] protected ASPMap map;
    [SerializeField] protected MapKey mapKey;
    protected bool waitingOnClingo;

    [SerializeField] int cpus = 4;
    

    private void Start()
    {
        //startJob();
        startGenerator();
    }
    // Update is called once per frame
    void Update()
    {
        if (waitingOnClingo)
        {
            if(Solver.SolverStatus == Clingo.ClingoSolver.Status.SATISFIABLE)
            {
                //map.DisplayMap(Solver.answerSet,mapKey);
                SATISFIABLE();
                waitingOnClingo = false;
            }
            else if(Solver.SolverStatus == Clingo.ClingoSolver.Status.UNSATISFIABLE)
            {
                Debug.LogWarning("UNSATISFIABLE");
                UNSATISFIABLE();
                waitingOnClingo = false;
            }
            else if (Solver.SolverStatus == Clingo.ClingoSolver.Status.ERROR)
            {
                Debug.LogWarning("ERROR");
                ERROR();
                waitingOnClingo = false;
            }
            else if (Solver.SolverStatus == Clingo.ClingoSolver.Status.TIMEDOUT)
            {
                Debug.LogWarning("TIMEDOUT");
                TIMEDOUT();
                waitingOnClingo = false;
            }
        }
    }

    void startJob()
    {
        string filename = Clingo.ClingoUtil.CreateFile(aspCode);
        Solver.Solve(filename);
        waitingOnClingo = true;
    }

    protected string aspCode { get { return getASPCode(); } }

    protected virtual string getASPCode()
    {
        string aspCode = @"

        
        #const max_width = 8.
        #const max_height = 8.

        width(1..max_width).
        height(1..max_height).

        tile_type(filled;empty).

        1{tile(XX, YY, Type): tile_type(Type)}1 :- width(XX), height(YY).

        :- tile(X1,Y1, filled), tile(X2,Y2,filled), X1 == X2, Y1 != Y2.
        :- tile(X1,Y1, filled), tile(X2,Y2,filled), Y1 == Y2, X1 != X2.

        :- Count = {tile(_,_,filled)}, Count != max_width.
  
        :- tile(X1,Y1, filled), tile(X2,Y2,filled), X1 == X2 + Offset, Y1 == Y2 + Offset, width(Offset).
        :- tile(X1,Y1, filled), tile(X2,Y2,filled), X1 == X2 + Offset, Y1 == Y2 - Offset, width(Offset).


    ";
        return aspCode;
    }

    virtual protected void initializeGenerator()
    {

    }

    virtual protected void startGenerator()
    {
        string filename = Clingo.ClingoUtil.CreateFile(aspCode);
        Solver.Solve(filename);
        waitingOnClingo = true;
    }

    virtual protected void finalizeGenerator()
    {
        
    }

    virtual protected string getAdditionalParameters()
    {
        return $" --parallel-mode {cpus} ";
    }

    virtual protected void SATISFIABLE()
    {
        map.DisplayMap(Solver.answerSet, mapKey);
        Debug.LogWarning("SATISFIABLE unimplemented");
    }

    virtual protected void UNSATISFIABLE()
    {
        Debug.LogWarning("UNSATISFIABLE unimplemented");
    }

    virtual protected void TIMEDOUT()
    {
        Debug.LogWarning("TIMEDOUT unimplemented");
    }

    virtual protected void ERROR()
    {
        Debug.LogWarning("ERROR unimplemented");
    }
}
