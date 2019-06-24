using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTurn : IStates
{
    private Character myMaster;
    private CharacterState stateMachine;
    public float timeInState;
    public float turnTime;
    private Vector3 direction;

    public void Enter()
    {
        timeInState = 0;
    }

    public void StateUpdate()
    {
        MoveInput();
        timeInState += Time.deltaTime;
        if (!myMaster.onGround)
            stateMachine.ChangeState(stateMachine.fall);

        if (timeInState >= turnTime)
        {
            if (direction != Vector3.zero)
                stateMachine.ChangeState(stateMachine.locomotion);
            else
                stateMachine.ChangeState(stateMachine.idle);
        }
    }

    public void Exit()
    {
        myMaster.MoveCharacter(direction);
        myMaster.animator.SetFloat("directionAngle", 0f);
    }

    public void MoveInput()
    {
        direction = InputsManager.instance.leftStick;
    }

    #region Constructor

    public CharacterTurn(Character myMaster, CharacterState stateMachine)
    {
        this.myMaster = myMaster;
        this.stateMachine = stateMachine;
    }
    #endregion

    public void IfStateChange()
    {

    }
}
