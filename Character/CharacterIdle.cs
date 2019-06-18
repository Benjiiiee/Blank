
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIdle : IStates
{
    private Character myMaster;
    private CharacterState stateMachine;
    private Vector3 direction;
    public float timeInState;

    public void Enter()
    {
        timeInState = 0f;
    }

    public void StateUpdate()
    {
        timeInState += Time.deltaTime;
        MoveInput();
        myMaster.MoveCharacter(direction);
        myMaster.animator.SetFloat("vitesse", direction.magnitude);
    }

    public void Exit()
    {

    }

    public void MoveInput()
    {
        direction = InputsManager.instance.leftStick;

        if (!myMaster.onGround && myMaster.currentPlatform == null)
            stateMachine.ChangeState(stateMachine.fall);

        else if (InputsManager.instance.jump && GameManager.instance.spcLvlMan.jumpUnlocked)
        {
            myMaster.currentPlatform = null;
            myMaster.onGround = false;
            stateMachine.ChangeState(stateMachine.jump);
        }

        else if (direction.magnitude != 0f)
            stateMachine.ChangeState(stateMachine.locomotion);

        else if (InputsManager.instance.crouch && myMaster.canCrouch)
            stateMachine.ChangeState(stateMachine.crouch);
    }

    #region Constructor
    public CharacterIdle(Character myMaster, CharacterState stateMachine)
    {
        this.myMaster = myMaster;
        this.stateMachine = stateMachine;
    }
    #endregion

    public void IfStateChange()
    {

    }
}