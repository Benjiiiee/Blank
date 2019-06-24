using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDialogue : IStates
{
    private Character myMaster;
    private CharacterState stateMachine;


    public void Enter()
    {
        myMaster.inputDisabled = true;
        myMaster.animator.SetFloat("vitesse", 0f);
        myMaster.animator.SetBool("crouch", false);
        InputsManager.instance.cameraInputsAreDisabled = true;
        InputsManager.instance.gameplayInputsForDialog = true;
        
    }

    public void StateUpdate()
    {
        if (InputsManager.instance.jump)
            DialogueManager.instance.PlayDialogue();
    }

    public void Exit()
    {
        myMaster.inputDisabled = false;
        InputsManager.instance.cameraInputsAreDisabled = false;
        InputsManager.instance.gameplayInputsForDialog = false;
    }

    #region Constructor

    public CharacterDialogue(Character myMaster, CharacterState stateMachine)
    {
        this.myMaster = myMaster;
        this.stateMachine = stateMachine;
    }
    #endregion

    public void IfStateChange()
    {

    }
}
