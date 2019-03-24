using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestData : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> items;

    [HideInInspector]
    public bool opened = false;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Entity>().type = entityType.chest;
        EventManager.OnChestOpen += OnPlayerPrompt;


        //Fill chest with items
    }

    // Update is called once per frame
    void Update()
    {
        if (opened)
        {
            GameObject.FindGameObjectWithTag("manager").GetComponent<ShaderManager>().TintGreen(gameObject);
        }
        else
        {
            GameObject.FindGameObjectWithTag("manager").GetComponent<ShaderManager>().TintRedPulse(gameObject);
        }

    }

    /// <summary>
    /// Activates chest contents, subscribed to OnPlayerPrompt event
    /// </summary>
    void OnPlayerPrompt()
    {
        //Open chest UI or loot chest (whichever we decide to do)
        opened = true;
    }
}
