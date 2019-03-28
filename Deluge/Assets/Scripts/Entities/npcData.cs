using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcData : MonoBehaviour
{
    //Set in inspector
    public List<TextAsset> dialogueList;
    public GameObject reward;

    [HideInInspector]
    public GameObject dialogueManager;

    private TextAsset currentDialogue;
    private GameObject player;

    [HideInInspector]
    public bool questActive = false;
    [HideInInspector]
    public bool questConcluded = false;



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


        //update face direction
        switch (player.GetComponent<Entity>().direction)
        {
            case FaceDirection.backward:
                GetComponent<Entity>().direction = FaceDirection.forward;
                break;
            case FaceDirection.forward:
                GetComponent<Entity>().direction = FaceDirection.backward;
                break;
            case FaceDirection.left:
                GetComponent<Entity>().direction = FaceDirection.right;
                break;
            case FaceDirection.right:
                GetComponent<Entity>().direction = FaceDirection.left;
                break;
        }
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

        //quest hasn't been completed yet
        if (!questConcluded)
        {
            //quest start
            if (!GameObject.FindGameObjectWithTag("chest").GetComponent<ChestData>().opened && !questActive)
            {
                FindObjectOfType<AudioManager>().PlaySound("questStartSound");

                //set quest to active
                questActive = true;
            }
            //quest end
            else if (GameObject.FindGameObjectWithTag("chest").GetComponent<ChestData>().opened)
            {
                GivePlayerReward();
                FindObjectOfType<AudioManager>().PlaySound("questEndSound");
                questConcluded = true;
                questActive = false;
            }
        }


        GameData.ToggleGameplayPaused();

        dialogueManager.GetComponent<Dialogue_Manager>().EndDialogue();
    }

    public void GivePlayerReward()
    {
        player.GetComponent<Inventory>().AddItem(reward.GetComponent<Item>());
        player.GetComponent<Inventory>().currentOffHand = reward.GetComponent<Item>();
    }
}
