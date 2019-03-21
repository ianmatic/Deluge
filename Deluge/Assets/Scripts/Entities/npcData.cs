using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcData : MonoBehaviour
{
    public List<TextAsset> dialogueList;
    private TextAsset currentDialogue;
    public GameObject dialogueManager;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        //don't do new List of TextAssets for dialogueList
        //since it is filled by Unity

        dialogueManager = GameObject.Find("dialogue_ui");
        GetComponent<Entity>().type = entityType.npc;
        player = GameObject.FindGameObjectWithTag("Player");

        //Fill dialogue list in Unity
        //first dialogue is current
        currentDialogue = dialogueList[0];

        EventManager.OnDialogueBegin += OnPlayerPrompt;
        EventManager.OnDialogueContinue += OnPlayerContinue;
        EventManager.OnDialogueExit += OnPlayerEnd;
    }

    /// <summary>
    /// Activates when the player interacts with this npc, subscribed to OnDialogueBegin
    /// </summary>
    public void OnPlayerPrompt()
    {
        //Update current dialogue here if needed
        

        //hardcoded for now
        if (currentDialogue == dialogueList[1])
        {
            currentDialogue = dialogueList[2];
        }
        else if (GameObject.FindGameObjectWithTag("chest").GetComponent<ChestData>().opened)
        {
            currentDialogue = dialogueList[1];
        }



        GameData.ToggleGameplayPaused();

        dialogueManager.GetComponent<Dialogue_Manager>().TriggerDialogue(currentDialogue);
    }

    /// <summary>
    /// Activates when the player progresses in dialogue, subscribed to OnDialogueContinue
    /// </summary>
    public void OnPlayerContinue()
    {
        dialogueManager.GetComponent<Dialogue_Manager>().UpdateText();
    }

    /// <summary>
    /// Activates when the player ends interaction with npc, subscribed to OnDialogueExit
    /// </summary>
    public void OnPlayerEnd()
    { 
        //Update current dialogue here if needed

        GameData.ToggleGameplayPaused();

        dialogueManager.GetComponent<Dialogue_Manager>().EndDialogue();
    }
}
