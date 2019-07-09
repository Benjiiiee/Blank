using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefCamera : MonoBehaviour
{
    public void ResetCamera()
    {
        GameManager.instance.currentCamera.camFSM.ChangeState(GameManager.instance.currentCamera.resetState);
    }

    public void Lookat(Transform lookAtTarget)
    {
        // GameManager.instance.currentCamera.revealState.timer = 2f;
        // GameManager.instance.currentCamera.camFSM.ChangeState(GameManager.instance.currentCamera.revealState);
        // GameManager.instance.currentCamera.tr.LookAt(lookAtTarget.position);

        GameManager.instance.currentCamera.revealState.target = lookAtTarget;
        GameManager.instance.currentCamera.camFSM.ChangeState(GameManager.instance.currentCamera.revealState);
    }
}
