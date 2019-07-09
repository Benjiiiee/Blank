using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefAvatar : MonoBehaviour
{
    public void SetTrigger(string triggerName)
    {
        GameManager.instance.currentAvatar.animator.SetTrigger(triggerName);
    }

    public void ResetTrigger(string triggerName)
    {
        GameManager.instance.currentAvatar.animator.ResetTrigger(triggerName);
    }

    public void DisableInput()
    {
        GameManager.instance.currentAvatar.DisableInput();
    }

    public void EnableInput()
    {
        GameManager.instance.currentAvatar.EnableInput();
    }

    public void SetMove(float vitesse)
    {
        GameManager.instance.currentAvatar.animator.SetFloat("vitesse", vitesse);
    }

    public void LookAt(Transform lookAtPos)
    {
        Vector3 temp = lookAtPos.position;
        temp.y = GameManager.instance.currentAvatar.tr.position.y;
        GameManager.instance.currentAvatar.tr.LookAt(temp);
    }
}
