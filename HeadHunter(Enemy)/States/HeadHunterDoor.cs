using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadHunterDoor : IStates
{
    private HeadHunterNME myMaster;

    public void Enter()
    {

    }

    public void IfStateChange()
    {

    }

    public void StateUpdate()
    {

    }

    public void Exit()
    {

    }

    #region Constructor

    public HeadHunterDoor(HeadHunterNME myMaster)
    {
        this.myMaster = myMaster;
    }

    #endregion
}
