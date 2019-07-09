using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadHunterChase : IStates
{
    private HeadHunterNME myMaster;
    private float angle;
    private int cpt;
    private float timeInState;
    private float timeToKill = 2f;
    private RaycastHit hit;

    #region Constructor

    public HeadHunterChase(HeadHunterNME myMaster)
    {
        this.myMaster = myMaster;
    }

    #endregion

    public void Enter()
    {
        myMaster.animator.SetBool("isChasing", true);
        myMaster.agent.isStopped = false;
        cpt = 0;
        timeInState = 0f;
        InputsManager.instance.SetVibration(1f, 1.5f, true);
        myMaster.fovRadius *= 2f;
    }

    public void IfStateChange()
    {

    }

    public void StateUpdate()
    {
        timeInState += Time.deltaTime;

        myMaster.LookForPlayer();
        myMaster.animator.SetBool("isDetectingBlank", myMaster.playerDetected);

        //Regarde le joueur en x,z
        if (myMaster.playerDetected && timeInState < timeToKill)
        {
            Vector3 lookAtPos = GameManager.instance.currentAvatar.tr.position;
            lookAtPos.y = myMaster.tr.position.y;
            myMaster.tr.LookAt(lookAtPos);
        }

        //Detection
        if (!myMaster.playerDetected)
        {
            myMaster.searchDestination = GameManager.instance.currentAvatar.tr.position;
            myMaster.stateMachine.ChangeState(myMaster.search);
        }

        //Optimisation de navMesh
        cpt++;

        if (cpt % 10 == 0)
            myMaster.agent.SetDestination(GameManager.instance.currentAvatar.tr.position);

        angle = Vector3.SignedAngle(myMaster.tr.forward, myMaster.agent.destination - myMaster.tr.position, myMaster.tr.up);
        myMaster.animator.SetFloat("turn", angle);

        //Kill selon la distance
        if (Vector3.Distance(myMaster.tr.position, GameManager.instance.currentAvatar.tr.position) <= myMaster.killDistance && GameManager.instance.currentAvatar.stateMachine.currentState != GameManager.instance.currentAvatar.stateMachine.death)
        {
            if (timeInState >= timeToKill && GameManager.instance.currentAvatar.controller.enabled)
            {
                Physics.Linecast(myMaster.tr.position + Vector3.up, GameManager.instance.currentAvatar.tr.position + Vector3.up, out hit);
                if (hit.collider.CompareTag("Player"))
                {
                    GameManager.instance.currentAvatar.animator.SetBool("deathByTrap", true);
                    GameManager.instance.currentAvatar.stateMachine.ChangeState(GameManager.instance.currentAvatar.stateMachine.death);
                    myMaster.playerDetected = false;
                    myMaster.Invoke("ResetPos", 3f);
                    myMaster.stateMachine.ChangeState(myMaster.search);
                }
            }
        }
    }

    public void Exit()
    {
        myMaster.animator.SetBool("isChasing", false);
        myMaster.fovRadius /= 2f;
    }
}