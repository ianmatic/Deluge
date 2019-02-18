using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    // FIELDS
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
    public Text healthText;
    public Text timeText;

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

        // Health Bar Data
        healthBarWidthInitial = healthBar.GetComponent<Transform>().localScale.x;
        healthBarWidthCurrent = healthBarWidthInitial;

        // Icon tracking
        iconMainTracker = 1;
    }

    // Update is called once per frame
    void Update()
    {
        adjustHealth();
        adjustTime();
    }

    /// <summary>
    /// Updates the player's health in the UI
    /// </summary>
    private void adjustHealth()
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
    private void adjustTime()
    {
        // TODO: Set up time system & pull time remaining

        // TODO: Update visuals to fit
    }

    /// <summary>
    /// Cycles through the main weapon
    /// </summary>
    public void cycleIconMain()
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
}
