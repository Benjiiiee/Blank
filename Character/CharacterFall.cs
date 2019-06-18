using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFall : IStates
{

    private Character myMaster;
    private CharacterState stateMachine;
    private Vector3 direction;
    private Transform hitTransform;
    private Vector3 desiredPos;
    private Vector3 highest;
    private Vector3 delta;
    private float timeInState;
    private float timeToJump = 0.1f;

    public void Enter()
    {
        myMaster.currentPlatform = null;
        timeInState = 0f;

        if (stateMachine.previousState != stateMachine.jump)
            myMaster.animator.SetBool("isFalling", true);
    }

    public void StateUpdate()
    {
        timeInState += Time.deltaTime;

        if (!GameManager.instance.cheatMenu.cheatMenuActive)
        {
            MoveInput();
            myMaster.MoveCharacter(direction);
            if (direction == Vector3.zero)
                myMaster.velocityZ -= myMaster.drag;

            myMaster.animator.SetFloat("vitesse", direction.magnitude);
           

            if (myMaster.velocityY < 0f && myMaster.rayHitDistance <= 1f && myMaster.rayHitDistance >= 0.06f)
            {
                myMaster.animator.SetBool("isJumping", false);
                myMaster.animator.SetBool("isFalling", false);
            }

            myMaster.CheckLedgeGrab();
        }
    }

    public void Exit()
    {
        myMaster.velocityY = 0f;
        myMaster.velocityZ = 0f;
        myMaster.animator.SetBool("isJumping", false);
        myMaster.animator.SetBool("isFalling", false);
    }

    public void MoveInput()
    {
        direction = InputsManager.instance.leftStick;

        if (timeInState <= timeToJump && InputsManager.instance.jump && myMaster.stateMachine.previousState != myMaster.stateMachine.jump)
            myMaster.stateMachine.ChangeState(myMaster.stateMachine.jump);

        else if (InputsManager.instance.crouch)
        {
            myMaster.isCrouching = true;
            myMaster.animator.SetBool("crouch", true);
        }

        else if (myMaster.onGround)
        {
            if (myMaster.isCrouching)
                stateMachine.ChangeState(stateMachine.crouch);

            else if (InputsManager.instance.jump)
                stateMachine.ChangeState(stateMachine.jump);

            else if (direction.magnitude != 0f)
                stateMachine.ChangeState(stateMachine.locomotion);

            else
                stateMachine.ChangeState(stateMachine.idle);
        }
        
    }

    #region Constructor

    public CharacterFall(Character myMaster, CharacterState stateMachine)
    {
        this.myMaster = myMaster;
        this.stateMachine = stateMachine;
    }

    #endregion

    public void IfStateChange()
    {

    }
}
