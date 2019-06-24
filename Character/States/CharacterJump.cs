
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterJump : IStates
{
    private Character myMaster;
    private CharacterState stateMachine;
    private Vector3 direction;
    private Transform hitTransform;
    private Vector3 desiredPos;
    private float timeInState;
    private float timeToExit = 0.1f;


    public void Enter()
    {
        myMaster.currentPlatform = null;
        timeInState = 0f;
        myMaster.velocityY = myMaster.impulsionY;
        myMaster.animator.SetBool("isJumping", true);
        if (stateMachine.previousState == stateMachine.locomotion || InputsManager.instance.leftStick.magnitude != 0f)
        {
            myMaster.velocityZ = myMaster.impulsionZ;
            myMaster.animator.SetTrigger("jumpForward");
        }
        else
        {
            myMaster.animator.SetTrigger("jumpInPlace");
        }
    }

    public void StateUpdate()
    {
        timeInState += Time.deltaTime;

        MoveInput();

        myMaster.MoveCharacter(direction);

        if (direction == Vector3.zero)
            myMaster.velocityZ -= myMaster.drag;

        myMaster.animator.SetFloat("vitesse", direction.magnitude);

        if(timeInState >= timeToExit)
            stateMachine.ChangeState(stateMachine.fall);

        myMaster.CheckLedgeGrab();
    }

    public void Exit()
    {

    }

    public void MoveInput()
    {
        if (timeInState >= timeToExit && InputsManager.instance.jumpHigh)
            myMaster.velocityY += myMaster.impulsionLongJump;
            
        direction = InputsManager.instance.leftStick;
    }

    #region Constructor
    public CharacterJump(Character myMaster, CharacterState stateMachine)
    {
        this.myMaster = myMaster;
        this.stateMachine = stateMachine;
    }
    #endregion

    public void IfStateChange()
    {

    }
}