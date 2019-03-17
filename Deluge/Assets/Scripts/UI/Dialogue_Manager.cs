﻿using System.Collections;
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
    private int currentTextLine;
    private int maxTextLine;

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

        // Populate the speaker & dialogue lists
        ReadText(inputFile);

        // Update the UI to hold the first item from each list
        UpdateText();

        // Populating GUI Elements list
        gui_elements.Add(gui_tint);
        gui_elements.Add(gui_overlay);
        gui_elements.Add(gui_currentCharacter);
        gui_elements.Add(textNameGO);
        gui_elements.Add(textBodyGO);

        // Set up max text count & initialize current text line
        currentTextLine = -1;               // defaults to -1 so we can tell if text has been
        maxTextLine = speakerList.Count;    // read in yet or not -- also so UpdateText starts at 0

        // Move dialogue offscreen by default
        ToggleAssets();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the UI is toggled
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleAssets();
        }

        // Check if dialogue should be updated
        if (Input.GetKeyDown(KeyCode.Y))
        {
            UpdateText();
        }
    }

    /// <summary>
    /// Displays the basic UI that holds all dialogue assets
    /// </summary>
    void ToggleAssets()
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
    void ReadText(TextAsset current)
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

        int bugtesting = 3;
    }

    /// <summary>
    /// Updates the displayed GUI text to the next applicable line in the set
    /// </summary>
    private void UpdateText()
    {
        if (currentTextLine < maxTextLine)  //make sure there are more sections to display
        {
            currentTextLine++;
            textBody.text = dialogueList[currentTextLine];
            textName.text = speakerList[currentTextLine];
        }
    }
}