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

    private void LookAtPlayer()
    {
        Vector3 playerPosition = new Vector3(gameManager.player.xPos, gameManager.player.yPos, 6f);
        Vector3 lookDirection = (playerPosition - transform.position).normalized;
        switch (lookDirection)
        {
            case Vector3 v when v.Equals(Vector3.up):
                transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(0f, 0f, 0f), 6f);
                break;
            case Vector3 v when v.Equals(Vector3.down):
                transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(0f, 0f, 180f), 6f);
                break;
            case Vector3 v when v.Equals(Vector3.right):
                transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(0f, 0f, -90f), 6f);
                break;
            case Vector3 v when v.Equals(Vector3.left):
                transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(0f, 0f, 90f), 6f);
                break;

        }
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

