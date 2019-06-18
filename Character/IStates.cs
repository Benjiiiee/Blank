using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStates
{
    void Enter();

    void IfStateChange();

    void StateUpdate();

    void Exit();
}

