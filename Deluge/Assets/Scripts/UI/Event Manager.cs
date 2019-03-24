using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    //subscribe to for when dialouge exit occurs
    public delegate void DialogueExitAction();

    public static event DialogueExitAction OnDialogueExit;

    //accessor to be used in other classes
    public static void DialogueExit()
    {
        if (OnDialogueExit != null)
        {
            OnDialogueExit();
        }
    }


    //subscribe to for when dialogue continue occurs
    public delegate void DialogueContinueAction();

    public static event DialogueContinueAction OnDialogueContinue;

    //accessor to be used in other classes
    public static void DialogueContinue()
    {
        if (OnDialogueContinue != null)
        {
            OnDialogueContinue();
        }
    }


    //subsribe to for when dialogue begin occurs
    public delegate void DialogueBeginAction();

    public static event DialogueBeginAction OnDialogueBegin;

    //accessor to be used in other classes
    public static void DialogueBegin()
    {
        if (OnDialogueBegin != null)
        {
            OnDialogueBegin();
        }
    }

    //subsribe to for when chest opens
    public delegate void ChestOpenAction();

    public static event ChestOpenAction OnChestOpen;

    //accessor to be used in other classes
    public static void ChestOpen()
    {
        if (OnChestOpen != null)
        {
            OnChestOpen();
        }
    }
}
