using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterClimb : IStates
{
    private Character myMaster;
    private CharacterState stateMachine;
    private float timeStarted;
    private float climbTime = 1.3f;
    private Vector3 direction;

    public void Enter()
    {
        timeStarted = Time.time;
    }

    public void StateUpdate()
    {
        MoveInput();

        myMaster.animator.SetFloat("vitesse", direction.magnitude);

        if (Time.time - timeStarted >= climbTime)
        {
            if(direction != Vector3.zero)
                stateMachine.ChangeState(stateMachine.locomotion);
            else
                stateMachine.ChangeState(stateMachine.idle);
        }
    }

    public void Exit()
    {
        myMaster.isClimbing = false;
        myMaster.controller.enabled = true;
        myMaster.animator.SetFloat("directionAngle", 0f);
        myMaster.animator.SetFloat("vitesse", direction.magnitude);
    }

    public void MoveInput()
    {
        direction = InputsManager.instance.leftStick;
    }

    #region Constructor

    public CharacterClimb(Character myMaster, CharacterState stateMachine)
    {
        this.myMaster = myMaster;
        this.stateMachine = stateMachine;
    }
    #endregion


    public void IfStateChange()
    {

    }
}
