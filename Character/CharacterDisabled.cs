using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDisabled : IStates
{
    private Character myMaster;
    private CharacterState stateMachine;

    public void Enter()
    {
        myMaster.inputDisabled = true;
        myMaster.animator.SetFloat("vitesse", 0f);
    }

    public void StateUpdate()
    {

    }

    public void Exit()
    {
        myMaster.inputDisabled = false;
    }

    #region Constructor
    public CharacterDisabled(Character myMaster, CharacterState stateMachine)
    {
        this.myMaster = myMaster;
        this.stateMachine = stateMachine;
    }
    #endregion

    public void IfStateChange()
    {

    }

}
