
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState : MonoBehaviour
{
    private Character myMaster;
    public IStates currentState;
    public IStates previousState;

    public CharacterIdle idle;
    public CharacterLocomotion locomotion;
    public CharacterJump jump;
    public CharacterFall fall;
    public CharacterClimb climb;
    public CharacterCrouch crouch;
    public CharacterZapped zapped;
    public CharacterTurn turn;
    public CharacterInteract interact;
    public CharacterDeath death;
    public CharacterDialogue dialogue;
    public CharacterDisabled disabled;
    public CharacterEventTrain train;

    private void Start()
    {
        myMaster = GetComponent<Character>();

        idle = new CharacterIdle(myMaster, this);
        locomotion = new CharacterLocomotion(myMaster, this);
        jump = new CharacterJump(myMaster, this);
        fall = new CharacterFall(myMaster, this);
        climb = new CharacterClimb(myMaster, this);
        crouch = new CharacterCrouch(myMaster, this);
        zapped = new CharacterZapped(myMaster, this);
        turn = new CharacterTurn(myMaster, this);
        interact = new CharacterInteract(myMaster, this);
        death = new CharacterDeath(myMaster, this);
        dialogue = new CharacterDialogue(myMaster, this);
        disabled = new CharacterDisabled(myMaster, this);
        train = new CharacterEventTrain(myMaster, this);

        currentState = idle;

        //Special event niveau 1
        if (!GameManager.instance.spcLvlMan.hasHead)
            ChangeState(train);
    }

    private void Update()
    {
        if(Time.timeScale != 0f)
            currentState.StateUpdate();
    }

    /// <summary> Appel la fonction Exit() du vieux state et le Enter() du nouveau </summary>
    /// <param name="newState"> Nouveau state </param>
    public void ChangeState(IStates newState)
    {
        currentState.Exit();
        previousState = currentState;
        currentState = newState;
        currentState.Enter();
    }

    /// <summary> Retourne au state precedent </summary>
    public void SwitchToPreviousState()
    {
        currentState.Exit();
        currentState = previousState;
        currentState.Enter();
    }
}