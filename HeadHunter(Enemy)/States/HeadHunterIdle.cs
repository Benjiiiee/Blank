using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadHunterIdle : IStates
{
    private HeadHunterNME myMaster;

    #region Constructor

    public HeadHunterIdle(HeadHunterNME myMaster)
    {
        this.myMaster = myMaster;
    }

    #endregion

    public void Enter()
    {

    }

    public void IfStateChange()
    {

    }

    public void StateUpdate()
    {    
        if(myMaster.pathObject != null)
            myMaster.stateMachine.ChangeState(myMaster.patrol);

        myMaster.LookForPlayer();
        myMaster.animator.SetBool("isDetectingBlank", myMaster.playerDetected);

        if (myMaster.playerDetected)
            myMaster.stateMachine.ChangeState(myMaster.chase);
    }

    public void Exit()
    {

    }
}
