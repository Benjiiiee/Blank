using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInteract : IStates
{
    private Character myMaster;
    private CharacterState stateMachine;
    public float timeInState;
    public float interactTime;

    public void Enter()
    {
        timeInState = 0;
        myMaster.controller.enabled = false;
        myMaster.onGround = true;
        myMaster.animator.SetFloat("vitesse", 0f);
    }

    public void StateUpdate()
    {
        timeInState += Time.deltaTime;

        if (timeInState >= interactTime)
            stateMachine.ChangeState(stateMachine.idle);
    }

    public void Exit()
    {
        myMaster.controller.enabled = true;
    }

    #region Constructor

    public CharacterInteract(Character myMaster, CharacterState stateMachine)
    {
        this.myMaster = myMaster;
        this.stateMachine = stateMachine;
    }
    #endregion

    public void IfStateChange()
    {

    }
}
