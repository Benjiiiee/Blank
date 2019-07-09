using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadHunterSearchInPlace : IStates
{
    private HeadHunterNME myMaster;
    private float timeInState;
    private float timeToExit = 2f;
    public bool isDoor = false;

    public void Enter()
    {
        if (isDoor)
            myMaster.animator.SetBool("door", true);
        else
            myMaster.animator.SetBool("searchInPlace", true);
        timeInState = 0f;
    }

    public void IfStateChange()
    {

    }

    public void StateUpdate()
    {
        timeInState += Time.deltaTime;
        myMaster.animator.SetBool("isDetectingBlank", myMaster.playerDetected);

        //Detection
        if (myMaster.playerDetected)
            myMaster.stateMachine.ChangeState(myMaster.chase);

        else if (timeInState >= timeToExit)
            myMaster.stateMachine.ChangeState(myMaster.patrol);
    }

    public void Exit()
    {
        myMaster.animator.SetBool("door", false);
        myMaster.animator.SetBool("searchInPlace", false);
        isDoor = false;
    }

    #region Constructor

    public HeadHunterSearchInPlace(HeadHunterNME myMaster)
    {
        this.myMaster = myMaster;
    }

    #endregion
}
