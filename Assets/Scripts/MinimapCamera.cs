// Written by Joy de Ruijter
using UnityEngine;
using DG.Tweening;

public class MinimapCamera : MonoBehaviour
{
    #region Variables

    private GameManager gameManager;
    private Player player;

    private int xPos;
    private int yPos;

    #endregion

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    private void Update()
    {
        MoveToPlayerPosition();
    }

    private void MoveToPlayerPosition()
    {
        player = gameManager.player;
        xPos = player.xPos;
        yPos = player.yPos;

        Vector3 newPosition = new Vector3(xPos, yPos, -25f);

        if (transform.position != newPosition)
            transform.DOMove(newPosition, 2f);
    }
}
