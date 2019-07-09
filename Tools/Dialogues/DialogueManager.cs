/// Créé le: 08/04/19
/// Par: Jonathan Galipeau-Mann 
/// Dernière modification: 19/06/19
/// Par: Benjamin Chouinard

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public Queue<string> sentences;
    public DialogueTrigger myTrigger;


    void Awake()
    {
        SingletonSetup();
    }

    private void Start()
    {
        GameManager.instance.dialogueManager = this;
        sentences = new Queue<string>();
    }

    private void SingletonSetup()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    /// <summary> Commence le dialogue </summary>
    /// <param name="dialogueSpeaker"> Nom de celui qui parle </param>
    /// <param name="dialogues"> Tableau de dialogues </param>
    public void StartDialogue(string dialogueSpeaker, string[] dialogues)
    {
        //Setup avatar et camera
        GameManager.instance.currentAvatar.stateMachine.ChangeState(GameManager.instance.currentAvatar.stateMachine.dialogue);

        //Active le canevas et les texte
        UIManager.instance.dialogueBox.SetActive(true);
        UIManager.instance.speaker.text = dialogueSpeaker;

        sentences.Clear();

        foreach(string dialogue in dialogues)
        {
            sentences.Enqueue(dialogue);
        }

        PlayDialogue();
    }

    /// <summary> Commence le dialogue </summary>
    /// <param name="position"> Position du lookAt </param>
    /// <param name="dialogueSpeaker"> Nom de celui qui parle </param>
    /// <param name="dialogues"> Tableau de dialogues </param>
    public void StartDialogue(Transform position, string dialogueSpeaker, string[] dialogues)
    {
        //Setup avatar et camera
        GameManager.instance.refAvatar.LookAt(position);
        GameManager.instance.refCam.ResetCamera();

        GameManager.instance.currentAvatar.stateMachine.ChangeState(GameManager.instance.currentAvatar.stateMachine.dialogue);

        //Active le canevas et les texte
        UIManager.instance.dialogueBox.SetActive(true);
        UIManager.instance.speaker.text = dialogueSpeaker;

        sentences.Clear();

        foreach (string dialogue in dialogues)
        {
            sentences.Enqueue(dialogue);
        }

        PlayDialogue();
    }

    /// <summary> Affiche un bloc de dialogue </summary>
    public void PlayDialogue()
    {
        if (sentences.Count != 0)
            UIManager.instance.dialogue.text = sentences.Dequeue();
        else
            EndDialogue();
    }

    /// <summary> Ferme le dialogue box et commence un scripted event si necessaire </summary>
    private void EndDialogue()
    {
        if (myTrigger.eventIsFollowing)
            myTrigger.TriggerScriptedEvent();
        UIManager.instance.dialogueBox.SetActive(false);
        GameManager.instance.currentAvatar.stateMachine.ChangeState(GameManager.instance.currentAvatar.stateMachine.previousState);
    }
}
