using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadHunterStunned : IStates
{
    private HeadHunterNME myMaster;
    private float timeInState;
    private float timeToExit = 2.5f;

    #region Constructor

    public HeadHunterStunned(HeadHunterNME myMaster)
    {
        this.myMaster = myMaster;
    }

    #endregion


    public void Enter()
    {
        timeInState = 0f;
        myMaster.animator.SetBool("isStun", true);
        myMaster.animator.SetBool("isDetectingBlank", false);

    }

    public void IfStateChange()
    {

    }

    public void StateUpdate()
    {

        timeInState += Time.deltaTime;

        if (timeInState >= timeToExit)
            myMaster.stateMachine.ChangeState(myMaster.stateMachine.previousState);

    }

    public void Exit()
    {
        myMaster.animator.SetBool("isStun", false);
    }
}
