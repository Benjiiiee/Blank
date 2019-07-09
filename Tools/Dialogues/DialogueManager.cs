/// Créé le: 08/04/19
/// Par: Jonathan Galipeau-Mann 
/// Dernière modification: 12/04/19
/// Par: Maxime Phaneuf


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    Dictionary<string, Dialogue> dialogues = new Dictionary<string, Dialogue>();

    void Awake()
    {
        SingletonSetup();
        GetDialogueDict();
    }

    /// <summary>
    /// Remplir le dictionnaire dialogues
    /// </summary>
    void GetDialogueDict()
    {
        dialogues = TSVToCustomObject.GetDialogueFromTSV();
    }

    /// <summary>
    /// Retourne un objet Dialogue par son ID
    /// </summary>
    public Dialogue GetDialogue(string ID)
    {
        return dialogues[ID];
    }

    /// <summary>
    /// Retourne une ligne spécifique(index) d'un dialogue
    /// </summary>
    public Dialogue.Line GetLine(Dialogue d, int index)
    {
        return d.lines[index];
    }

    /// <summary>
    /// Retourne toutes les lignes d'un dialogue dans une List<>
    /// </summary>
    public List<Dialogue.Line> GetLines(Dialogue d)
    {
        return d.lines;
    }

    /// <summary>
    /// Retourne le Type d'une Line
    /// </summary>
    public string GetType(Dialogue.Line line)
    {
        return line.type;
    }
    /// <summary>
    /// Retourne le Speaker d'une Line
    /// </summary>
    public string GetSpeaker(Dialogue.Line line)
    {
        return line.speaker;
    }

    /// <summary>
    /// Retourne le Text d'une Line
    /// </summary>
    public string GetText(Dialogue.Line line)
    {
        return line.text;
    }

    private void SingletonSetup()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
