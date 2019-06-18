
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLocomotion : IStates
{
    private Character myMaster;
    private CharacterState stateMachine;
    private Vector3 direction;
    private int cpt = 0;
    public float timeInState;
    private float angle;

    public void Enter()
    {

    }

    public void StateUpdate()
    {
        timeInState += Time.deltaTime;
        MoveInput();
        myMaster.MoveCharacter(direction);
    }

    public void Exit()
    {
        timeInState = 0f;
        myMaster.animator.SetFloat("X", 0f);
    }

    public void MoveInput()
    {
        direction = InputsManager.instance.leftStick;

        if (!myMaster.onGround && myMaster.currentPlatform == null)
            stateMachine.ChangeState(stateMachine.fall);

        else if (InputsManager.instance.jump && GameManager.instance.spcLvlMan.jumpUnlocked)
        {
            myMaster.currentPlatform = null;
            myMaster.initialDirection = direction;
            myMaster.onGround = false;
            stateMachine.ChangeState(stateMachine.jump);
        }

        else if (InputsManager.instance.crouch && myMaster.canCrouch)
            stateMachine.ChangeState(stateMachine.crouch);

        else if (direction.magnitude < 1f)
        {
            cpt++;
            if (cpt >= 5 && direction.magnitude < 1f)
            {
                myMaster.animator.SetFloat("vitesse", direction.magnitude);
                if (direction.magnitude == 0f)
                    stateMachine.ChangeState(stateMachine.idle);
            }
        }
        else
        {
            cpt = 0;
            myMaster.animator.SetFloat("vitesse", direction.magnitude);
        }
    }

    #region Constructor
    public CharacterLocomotion(Character myMaster, CharacterState stateMachine)
    {
        this.myMaster = myMaster;
        this.stateMachine = stateMachine;
    }
    #endregion

    public void IfStateChange()
    {

    }
}


