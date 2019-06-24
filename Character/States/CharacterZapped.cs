using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterZapped : IStates
{
    private Character myMaster;
    private CharacterState stateMachine;
    private Vector3 direction;
    private float timeEntered;
    private float timeToExit = 4f;
    private bool canExit = false;

    public void Enter()
    {
        timeEntered = Time.time;
        switch (myMaster.zapState)
        {
            case 1:
                myMaster.animator.SetBool("isZapped", true);
                break;
            case 2:
                myMaster.animator.SetBool("isZapped", true);
                break;
            case 3:
                //Placeholder
                if (GameManager.instance.currentAvatar.stateMachine.currentState != GameManager.instance.currentAvatar.stateMachine.death)
                {
                    GameManager.instance.currentAvatar.animator.SetBool("deathByTrap", true);
                    GameManager.instance.currentAvatar.stateMachine.ChangeState(GameManager.instance.currentAvatar.stateMachine.death);
                }
                break;
        }
    }

    public void StateUpdate()
    {
        MoveInput();
        myMaster.MoveCharacter(direction);
        if (Time.time - timeEntered >= timeToExit)
            canExit = true;
    }

    public void Exit()
    {
        myMaster.animator.SetBool("isZapped", false);
    }

    public void MoveInput()
    {
        direction = InputsManager.instance.leftStick;

        if (canExit)
        {
            if (!myMaster.onGround)
                stateMachine.ChangeState(stateMachine.fall);

            else if (direction.magnitude != 0f)
                stateMachine.ChangeState(stateMachine.locomotion);

            else
                stateMachine.ChangeState(stateMachine.idle);

        }
    }

    #region Constructor

    public CharacterZapped(Character myMaster, CharacterState stateMachine)
    {
        this.myMaster = myMaster;
        this.stateMachine = stateMachine;
    }

    #endregion

    public void IfStateChange()
    {

    }
}
