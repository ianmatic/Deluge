using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcData : MonoBehaviour
{
    public TextAsset currentDialogue;
    public GameObject dialogueManager;
    private GameObject player;
    private bool inDialogue = false;

    //makes dialogue close after 1 frame, prevents bug
    private bool justLeftDialogue = false;

    // Start is called before the first frame update
    void Start()
    {
        dialogueManager = GameObject.FindGameObjectWithTag("dialogueManager");
        GetComponent<Entity>().type = entityType.npc;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //put before inDialogue so it occurs on next frame
        //exits player from dialouge after 1 frame of attempt
        if (justLeftDialogue)
        {
            player.GetComponent<PlayerData>().interacting = false;
            justLeftDialogue = false;
        }

        if (inDialogue)
        {
            //press space to advance dialogue
            if (Input.GetKeyDown(KeyCode.Space))
            {
                dialogueManager.GetComponent<Dialogue_Manager>().UpdateText();
            }

            //Exit dialogue
            if (Input.GetKeyDown(KeyCode.E))
            {
                //placeholder, need an "exitDialogue()" method in Dialogue_Manager
                dialogueManager.GetComponent<Dialogue_Manager>().TriggerDialogue(currentDialogue);
                inDialogue = false;
                justLeftDialogue = true;
            }
        }
    }

    /// <summary>
    /// Activates when the player interacts with this npc
    /// </summary>
    public void OnPlayerPrompt()
    {
        //temporary until importing dialogue is sorted
        currentDialogue = dialogueManager.GetComponent<Dialogue_Manager>().inputFile;

        //active text dialogue
        dialogueManager.GetComponent<Dialogue_Manager>().TriggerDialogue(currentDialogue);
        inDialogue = true;
    }
}
