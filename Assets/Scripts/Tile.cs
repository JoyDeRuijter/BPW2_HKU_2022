// Written by Joy de Ruijter
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    #region Variables

    [Header("Tile Colors")]
    [SerializeField] private Color baseColor;
    [SerializeField] private Color offsetColor;

    [Header("References")]
    [Space (10)]
    [SerializeField] private SpriteRenderer spriteRenderer;
    public GameObject highLight;

    [HideInInspector] public bool isOccupied;
    [HideInInspector] public bool isInUnitRange;
    [HideInInspector] public int xPos;
    [HideInInspector] public int yPos;

    private GameManager gameManager;

    #endregion

    private void Start()
    {
        gameManager = GameManager.instance;
        xPos = (int)transform.position.x;
        yPos = (int)transform.position.y;
    }

    public void Initialize(bool isOffset)
    {
        spriteRenderer.color = isOffset ? offsetColor : baseColor;
    }

    private void Update()
    {
        if (gameManager.gameState == GameManager.GameState.EnemyTurn && highLight.activeSelf == true)
            highLight.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (gameManager.WhatIsOnTile(this) == "Empty")
            isOccupied = false;
        else
            isOccupied = true;
    }

    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (gameManager.player.unitState != Unit.UnitStates.Action && gameManager.selectedTile != null)
        {
            gameManager.selectedTile.highLight.SetActive(false);
            return;
        }

        gameManager.selectedTile = this;
    }

    private void OnMouseExit()
    {
        highLight.SetActive(false);

        if (gameManager.selectedTile != null)
            gameManager.selectedTile.highLight.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (gameManager.player.unitState != Unit.UnitStates.Action && gameManager.selectedTile != null) 
        {
            gameManager.selectedTile.highLight.SetActive(false);
            return;
        }

        if (isInUnitRange)
            gameManager.lastClickedTile = this;
    }
}
