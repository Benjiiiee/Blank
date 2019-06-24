using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadHunterSearch : IStates
{
    private HeadHunterNME myMaster;
    private float angle;
    private float timeInState;
    private float timeToDetect = 2f;

    #region Constructor

    public HeadHunterSearch(HeadHunterNME myMaster)
    {
        this.myMaster = myMaster;
    }

    #endregion

    public void Enter()
    {
        myMaster.animator.SetBool("isSearching", true);
        myMaster.agent.destination = myMaster.searchDestination;

        timeInState = 0f;
    }

    public void IfStateChange()
    {

    }

    public void StateUpdate()
    {
        timeInState += Time.deltaTime;

        if(timeInState >= timeToDetect)
            myMaster.LookForPlayer();

        myMaster.animator.SetBool("isDetectingBlank", myMaster.playerDetected);

        angle = Vector3.SignedAngle(myMaster.tr.forward, myMaster.agent.destination - myMaster.tr.position, myMaster.tr.up);
        myMaster.animator.SetFloat("turn", angle);

        //Detection
        if (myMaster.playerDetected)
            myMaster.stateMachine.ChangeState(myMaster.chase);

        else if (myMaster.agent.remainingDistance <= 0.5f)
            myMaster.stateMachine.ChangeState(myMaster.patrol);
    }

    public void Exit()
    {
        myMaster.animator.SetBool("isSearching", false);
    }
}
