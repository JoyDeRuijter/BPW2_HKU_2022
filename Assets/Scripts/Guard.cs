// Written by Joy de Ruijter
using UnityEngine;

public class Guard : MonoBehaviour
{
    #region Variables

    [HideInInspector] public int xPos;
    [HideInInspector] public int yPos;
    [HideInInspector] public int roomID;
    private UIManager uiManager;
    private GameManager gameManager;
    private bool hasShowedDialogue;

    #endregion

    private void Start()
    {
        uiManager = UIManager.instance;
        gameManager = GameManager.instance;
    }

    private void Update()
    {
        if (gameManager.player.roomID == roomID && !hasShowedDialogue)
            ShowDialogue();
        else if (gameManager.player.roomID != roomID)
            hasShowedDialogue = false;
    }

    private void ShowDialogue()
    {
        if (gameManager.player.coins < 10)
        {
            StartCoroutine(uiManager.ShowPopup(20f, uiManager.acquireDialogue));
            hasShowedDialogue = true;
        }
        else if (gameManager.player.coins == 10)
        {
            StartCoroutine(uiManager.ShowPopup(20f, uiManager.giveDialogue));
            hasShowedDialogue = true;
        }
    }

    public void HideDialogue(GameObject dialogueObject)
    { 
        dialogueObject.SetActive(false);
        hasShowedDialogue = true;
    }
}

