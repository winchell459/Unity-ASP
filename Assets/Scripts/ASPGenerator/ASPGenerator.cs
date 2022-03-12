using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASPGenerator : MonoBehaviour
{
    [SerializeField] protected int cpus = 4, timeout = 100;
    [SerializeField] protected Clingo.ClingoSolver Solver;
    [SerializeField] protected ASPMap map;
    [SerializeField] protected MapKey mapKey;
    protected bool waitingOnClingo;

    protected System.Action<Clingo.AnswerSet, string> satisfiableCallBack;
    protected System.Action<string> unsatisfiableCallBack;
    protected System.Action<int, string> timedoutCallBack;
    protected System.Action<string, string> errorCallBack;

    protected string filename;
    protected string jobID = "";

    [SerializeField] bool runOnAwake = false;

    private void Awake()
    {
        if (runOnAwake)
        {
            InitializeGenerator(SATISFIABLE, UNSATISFIABLE, TIMEDOUT, ERROR);
            startGenerator();
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        if (waitingOnClingo)
        {
            if (Solver.SolverStatus == Clingo.ClingoSolver.Status.SATISFIABLE)
            {
                //map.DisplayMap(Solver.answerSet,mapKey);
                satisfiableCallBack(Solver.answerSet, jobID);
                //waitingOnClingo = false;
                finalizeGenerator();
            }
            else if (Solver.SolverStatus == Clingo.ClingoSolver.Status.UNSATISFIABLE)
            {

                unsatisfiableCallBack(jobID);
                //waitingOnClingo = false;
                finalizeGenerator();
            }
            else if (Solver.SolverStatus == Clingo.ClingoSolver.Status.ERROR)
            {

                errorCallBack(Solver.ClingoConsoleError, jobID);
                //waitingOnClingo = false;
                finalizeGenerator();
            }
            else if (Solver.SolverStatus == Clingo.ClingoSolver.Status.TIMEDOUT)
            {

                timedoutCallBack(timeout, jobID);
                //waitingOnClingo = false;
                finalizeGenerator();
            }


        }
    }


    public void StartGenerator()
    {

        initializeGenerator();
        startGenerator();
    }

    protected string aspCode { get { return getASPCode(); } }

    virtual protected string getASPCode()
    {
        string aspCode = @"

                #const max_width = 40.
                #const max_height = 40.

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
    public void InitializeGenerator(System.Action<Clingo.AnswerSet, string> satifiableCallBack, System.Action<string> unsatifiableCallBack, System.Action<int, string> timedoutCallBack, System.Action<string, string> errorCallBack)
    {
        this.satisfiableCallBack = satifiableCallBack;
        this.unsatisfiableCallBack = unsatifiableCallBack;
        this.timedoutCallBack = timedoutCallBack;
        this.errorCallBack = errorCallBack;

    }
    public void InitializeGenerator(int cpus, int timeout)
    {
        this.cpus = cpus;
        this.timeout = timeout;
    }

    virtual protected void initializeGenerator()
    {
        filename = Clingo.ClingoUtil.CreateFile(aspCode);
        Solver.maxDuration = timeout + 10;
    }

    virtual protected void startGenerator()
    {
        Solver.Solve(filename, getAdditionalParameters());
        waitingOnClingo = true;
    }

    virtual protected void finalizeGenerator()
    {
        waitingOnClingo = false;
    }

    virtual protected string getAdditionalParameters()
    {
        return $" --parallel-mode {cpus} --time-limit={timeout}";
    }

    virtual protected void SATISFIABLE(Clingo.AnswerSet answerSet, string jobID)
    {
        map.DisplayMap(answerSet, mapKey);
        map.AdjustCamera();
        Debug.LogWarning("SATISFIABLE unimplemented");
    }

    virtual protected void UNSATISFIABLE(string jobID)
    {
        Debug.LogWarning("UNSATISFIABLE unimplemented");
    }

    virtual protected void TIMEDOUT(int time, string jobID)
    {
        Debug.LogWarning("TIMEDOUT unimplemented");
    }

    virtual protected void ERROR(string error, string jobID)
    {
        Debug.LogWarning("ERROR unimplemented");
    }
}

