using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcInterest : MonoBehaviour
{
    //Set in inspector
    public TextAsset questInfo;

    enum QuestType
    {
        retrieval,
        elimination
    }



    private QuestType type;

    // Start is called before the first frame update
    void Start()
    {
        //Here load in questInfo and set variables
        //Ex: quest = questLoader.ReadQuest("questInfo");

        switch (type)
        {
            case QuestType.retrieval:
                //Load in items to retrieve here
                break;
            case QuestType.elimination:
                //Load in enemies to kill here
                break;
            default:
                break;

        }


        //hardcoded quest for now
        type = QuestType.retrieval;
    }

    // Update is called once per frame
    void Update()
    {
        switch (type)
        {
            case QuestType.retrieval:
                break;
            case QuestType.elimination:
                break;
            default:
                break;
        }
    }
}
