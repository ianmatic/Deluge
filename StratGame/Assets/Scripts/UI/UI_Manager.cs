using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    // FIELDS
    // Main
    public bool displaying = true;
    private List<GameObject> ui_objects;
    public GameObject uiHolder;

    // Icons
    public GameObject iconMain;
    private int iconMainTracker;
    public GameObject iconHat;
    public GameObject iconTorso;
    public GameObject iconOffhand;

    public Texture iconMainSpearIMG;
    public Texture iconMainAxeIMG;
    public Texture iconMainBowIMG;

    // Bars
    public GameObject healthBar;
    private float healthBarWidthInitial;
    private float healthBarWidthCurrent;
    public GameObject timeBar;

    // Text
    public GameObject healthTextObject;
    private Text healthText;
    public GameObject timeTextObject;
    private Text timeText;

    // External Info
    public GameObject player;
    private Entity playerData;
    int playerHealth;
    int playerMaxHealth;

    // Recyclable Components
    Vector3 tempVec3;

    void Start()
    {
        // Initializing components
        // Player Data
        player = GameObject.FindGameObjectWithTag("Player");
        playerData = player.GetComponent<Entity>();

        // Bug testing values
        playerData.health = 20;     //TODO: Set these values in player
        playerData.maxHealth = 34;  //TODO: See above

        playerHealth = 0;
        playerMaxHealth = 0;

        // Pull the text component from the two text ui assets
        healthText = healthTextObject.GetComponent<Text>();
        timeText = timeTextObject.GetComponent<Text>();

        // Health Bar Data
        healthBarWidthInitial = healthBar.GetComponent<Transform>().localScale.x;
        healthBarWidthCurrent = healthBarWidthInitial;

        // Icon tracking
        iconMainTracker = 1;

        // Add all objects to the ui_objects list for easy manipulation
        ui_objects = new List<GameObject>();
        ui_objects.Add(iconMain);
        ui_objects.Add(iconOffhand);
        ui_objects.Add(iconTorso);
        ui_objects.Add(iconHat);
        ui_objects.Add(healthBar);
        ui_objects.Add(timeBar);
        ui_objects.Add(uiHolder);
        ui_objects.Add(healthTextObject);
        ui_objects.Add(timeTextObject);

        // Turn the UI on
        ToggleAssets();
    }

    // Update is called once per frame
    void Update()
    {
        AdjustHealth();
        AdjustTime();

        // (For debugging) Moves all assets on or off screen when we tap '3'
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            ToggleAssets();
        }
    }

    /// <summary>
    /// Updates the player's health in the UI
    /// </summary>
    private void AdjustHealth()
    {
        // TODO: Get health from player
        playerHealth = playerData.health;
        playerMaxHealth = playerData.maxHealth; //TODO: Optimize pulling max health

        // Health Bar -- Sets it as a proportion of the original width base on current HP percent
        healthBarWidthCurrent = ((float)playerHealth / (float)playerMaxHealth) * healthBarWidthInitial;
        tempVec3 = healthBar.transform.localScale;
        tempVec3.x = healthBarWidthCurrent;
        healthBar.transform.localScale = tempVec3;

        // Health Text
        healthText.text = playerHealth + "/" + playerMaxHealth;

    }

    /// <summary>
    /// Updates the time remaining in the UI
    /// </summary>
    private void AdjustTime()
    {
        //Get the time
        float time = player.GetComponent<Timer>().remainingTime;

        // TODO: Update visuals to fit
        timeText.text = time + "s";
        
    }

    /// <summary>
    /// Cycles through the main weapon
    /// </summary>
    public void CycleIconMain()
    {
        if(iconMainTracker == 3)
        {
            iconMainTracker = 1;
        }
        else
        {
            iconMainTracker++;
        }

        switch (iconMainTracker)
        {
            case 1:
                iconMain.GetComponent<RawImage>().texture = iconMainSpearIMG;
                break;
            case 2:
                iconMain.GetComponent<RawImage>().texture = iconMainBowIMG;
                break;
            case 3:
                iconMain.GetComponent<RawImage>().texture = iconMainAxeIMG;
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Moves all the assets way off screen or back on screen when triggered
    /// </summary>
    public void ToggleAssets()
    {
        int mod;
        if (displaying)
        {
            mod = 1000;
        }
        else
        {
            mod = -1000;
        }

        // Temporary vector to replace each position w/
        Vector3 tempPosition = Vector3.zero;

        // Modify the position of each item based on the modifier
        foreach (GameObject ui_item in ui_objects)
        {
            tempPosition = ui_item.transform.position;
            tempPosition.x += mod;
            ui_item.transform.position = tempPosition;
        }

        // Flip displaying
        displaying = !displaying;
    }


}
