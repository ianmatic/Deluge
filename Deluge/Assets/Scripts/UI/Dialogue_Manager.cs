using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue_Manager : MonoBehaviour
{
    // FIELDS
    // Pulling other data
    public GameObject main_ui_manager_object;      // Turn off the main UI while text is on screen
    private UI_Manager main_ui_manager;

    // Setup Fields
    public TextAsset inputFile;
    public List<string> speakerList;        // List of speakers (ordered)
    public List<string> dialogueList;       // List of dialogues (ordered)
    bool displaying = true;

    // Displayed text fields
    public GameObject textNameGO;
    public GameObject textBodyGO;
    private Text textName;
    private Text textBody;
    private int currentTextLine = 0;

    // Dialogue UI Setup
    public GameObject gui_tint;
    public GameObject gui_overlay;
    public GameObject gui_currentCharacter;
    private List<GameObject> gui_elements;


    // Start is called before the first frame update
    void Start()
    {
        // Main UI manager setup
        main_ui_manager = main_ui_manager_object.GetComponent<UI_Manager>();

        // Instantiating lists
        speakerList = new List<string>();
        dialogueList = new List<string>();
        gui_elements = new List<GameObject>();

        // Add pointers to the Text on the UI
        textName = textNameGO.GetComponent<Text>();
        textBody = textBodyGO.GetComponent<Text>();

        // Populating GUI Elements list
        gui_elements.Add(gui_tint);
        gui_elements.Add(gui_overlay);
        gui_elements.Add(gui_currentCharacter);
        gui_elements.Add(textNameGO);
        gui_elements.Add(textBodyGO);

        // Move dialogue offscreen by default
        ToggleAssets();
    }

    /// <summary>
    /// Displays the basic UI that holds all dialogue assets
    /// </summary>
    public void ToggleAssets()
    {
        // Turn of the main ui overlay
        main_ui_manager.ToggleAssets();

        // Create a neccesary elements
        Vector3 tempPos = Vector3.zero;
        int modifier = 100000;

        // If the gui isn't currently displaying, move items back on screen
        if (!displaying)
        {
            modifier *= -1;
        }

        // Apply the movement modifier to the xpos of every object in gui_elements
        foreach (GameObject go in gui_elements)
        {
            tempPos = go.transform.position;
            tempPos.x += modifier;
            go.transform.position = tempPos;
        }

        // Update the displaying bool
        displaying = !displaying;
    }

    /// <summary>
    /// Read in the current section's text body and store it in currentDialogue
    /// </summary>
    /// <param name="current"></param>
    private void ReadText(TextAsset current)
    {
        // Clear out whatever was is leftover in the dialogue save-offs
        speakerList.Clear();
        dialogueList.Clear();

        // Read in the text splitting by "#" signs
        string fullTextBody = current.text;                     // EX Output:
        string[] tempStringDump = fullTextBody.Split('#');      // {}         <-- First one empty
        for (int i = 1; i < tempStringDump.Length; i++)         // {PLAYER}
        {                                                       // {Lorem ipsum dolor sit amet...\n}
            if (i % 2 == 1)                                     // {NPC}
            {                                                   // {Nemo enim ipsam voluptatem...\n}
                speakerList.Add(tempStringDump[i]);
            }
            else
            { 
                dialogueList.Add(tempStringDump[i]);
            }
        }
        
        // Remove \n from the end of each dialogue line
        for(int i = 0; i > dialogueList.Count; i++)
        {
            //Simplified Version
            dialogueList[i] = dialogueList[i].Substring(0, dialogueList[i].Length - 2);
        }
    }

    /// <summary>
    /// Updates the displayed GUI text to the next applicable line in the set
    /// </summary>
    public void UpdateText()
    {
        if (currentTextLine < speakerList.Count - 1)  //make sure there are more sections to display
        {
            currentTextLine++;
            textBody.text = dialogueList[currentTextLine];
            textName.text = speakerList[currentTextLine];
        }
        //end dialogue
        else
        {
            ToggleAssets();
            displaying = false;
            currentTextLine = 0;

            //Unpause
            GameData.ToggleGameplayPaused();
        }

    }

    public void TriggerDialogue(TextAsset current)
    {
        // Flip the assets to show this screen
        ToggleAssets();

        // Read in and update the GUI with the current text lines
        ReadText(current);

        //start at beginning
        textBody.text = dialogueList[0];
        textName.text = speakerList[0];
    }

    public void EndDialogue()
    {
        // Flip the assets to hide this screen
        ToggleAssets();

        currentTextLine = 0;
    }
}
