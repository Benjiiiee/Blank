using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCrouch : IStates
{
    private Character myMaster;
    private CharacterState stateMachine;
    private Vector3 direction;

    public void Enter()
    {
        myMaster.animator.SetBool("crouch", true);
        myMaster.isCrouching = true;
        myMaster.controller.height /= 2;
    }

    public void StateUpdate()
    {
        MoveInput();
        myMaster.MoveCharacter(direction);
        myMaster.animator.SetFloat("vitesse", direction.magnitude);
    }

    public void Exit()
    {
        myMaster.animator.SetBool("crouch", false);
        myMaster.controller.height *= 2;
    }

    public void MoveInput()
    {
        direction = InputsManager.instance.leftStick;

        if (!myMaster.onGround)
            stateMachine.ChangeState(stateMachine.fall);

        else if (InputsManager.instance.jump || InputsManager.instance.crouch)
        {
            if (myMaster.canDecrouch)
            {
                myMaster.isCrouching = false;

                if (direction.magnitude != 0f)
                    stateMachine.ChangeState(stateMachine.locomotion);
                else
                    stateMachine.ChangeState(stateMachine.idle);
            }
        }
    }

    #region Constructor

    public CharacterCrouch(Character myMaster, CharacterState stateMachine)
    {
        this.myMaster = myMaster;
        this.stateMachine = stateMachine;
    }

    #endregion

    public void IfStateChange()
    {

    }

}
