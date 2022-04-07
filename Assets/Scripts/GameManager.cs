// Written by Joy de Ruijter
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    #region Variables

    [Header("References")]
    [SerializeField] private Camera mainCamera;

    [HideInInspector] public Tile selectedTile;
    [HideInInspector] public Tile lastClickedTile;
    [HideInInspector] public GameState gameState;

    private Tile previouslySelectedTile;
    [HideInInspector] public Player player;
    private List<Unit>units = new List<Unit>();
    [HideInInspector] public List<Enemy>enemies = new List<Enemy>();
    [HideInInspector] public List<Tile>tiles = new List<Tile>();
    private int beginIndex;
    [HideInInspector] public Dungeon.DungeonGenerator dungeonGenerator;
    private int activeEnemy;

    #endregion

    private void Start()
    {
        dungeonGenerator = FindObjectOfType<Dungeon.DungeonGenerator>();
        player = FindObjectOfType<Player>();
        units = FindObjectsOfType<Unit>().Select(unit => unit).ToList();
        foreach (Unit unit in units)
        {
            if (unit.gameObject.TryGetComponent<Player>(out Player player))
                continue;
            enemies.Add(unit.gameObject.GetComponent<Enemy>());
        }
        tiles = FindObjectsOfType<Tile>().Select(tile => tile).ToList();
        DetermineFirstUnitTurn();
    }

    private void Update()
    {
        MoveCameraToPlayer();
        ActOnGameState();
    }

    #region UnitTurns

    public enum GameState { PlayerTurn, EnemyTurn}

    private void DetermineFirstUnitTurn()
    {
        for(int i = 0; i < units.Count; i++)
        {
            if (units[i].TryGetComponent<Player>(out Player player))
                beginIndex = i;
        }
        units[beginIndex].unitState = Unit.UnitStates.StartTurn;
    }

    private void ActOnGameState()
    {
        switch (gameState)
        { 
            case GameState.PlayerTurn:
                CheckUnitRange(selectedTile, player);
                if (player.unitState == Unit.UnitStates.Waiting)
                    player.unitState = Unit.UnitStates.StartTurn;
                if (player.unitState == Unit.UnitStates.Action)
                    TileToUnitMovement(player);
                if (player.unitState == Unit.UnitStates.EndTurn)
                {
                    activeEnemy = 0;
                    player.completedAction = false;
                    gameState = GameState.EnemyTurn;
                    player.unitState = Unit.UnitStates.Waiting;
                }
                break;

            case GameState.EnemyTurn:
                if (enemies == null)
                    gameState = GameState.PlayerTurn;
                CheckUnitRange(selectedTile, enemies[activeEnemy]);
                if (enemies[activeEnemy].unitState == Unit.UnitStates.Waiting)
                    enemies[activeEnemy].unitState = Unit.UnitStates.StartTurn;
                if (enemies[activeEnemy].unitState == Unit.UnitStates.Action)
                    TileToUnitMovement(enemies[activeEnemy]);
                if (enemies[activeEnemy].unitState == Unit.UnitStates.EndTurn)
                {
                    enemies[activeEnemy].unitState = Unit.UnitStates.Waiting;
                    enemies[activeEnemy].completedAction = false;
                    if (activeEnemy == enemies.Count - 1)
                        gameState = GameState.PlayerTurn;
                    else
                        activeEnemy++;
                }
                break;
        }
    }


    #endregion

    #region TileToUnitMovement
    // Check if the given tile is in range of the unit and if so, act accordingly
    private void CheckUnitRange(Tile tile, Unit unit)
    {
        if (unit.isMoving || tile == null)
            return;

        Vector2Int tilePosition = new Vector2Int(tile.xPos, tile.yPos);
        Vector3Int tilePositionDungeon = new Vector3Int(tile.xPos, tile.yPos, 0);
        Vector2Int unitPosition = new Vector2Int(unit.xPos, unit.yPos);
        float distance = Vector2Int.Distance(tilePosition, unitPosition);

        if (distance == 1 && dungeonGenerator.dungeon[tilePositionDungeon] == Dungeon.TileType.Floor &&
            (unit.unitState == Unit.UnitStates.StartTurn || unit.unitState == Unit.UnitStates.Action))
        {
            tile.isInUnitRange = true;
            tile.highLight.SetActive(true);
        }
        else
            tile.isInUnitRange = false;
    }

    // If there is a new selected tile and it's in range of the player, give the player a new targetposition
    private void TileToUnitMovement(Unit unit)
    {
        if (selectedTile != null && previouslySelectedTile != selectedTile && !unit.isMoving)
        {
            CheckUnitRange(selectedTile.GetComponent<Tile>(), unit);
            previouslySelectedTile = selectedTile.GetComponent<Tile>();
        }

        if (Input.GetMouseButtonDown(0) && !unit.isMoving && lastClickedTile != null &&
            lastClickedTile.transform.position == selectedTile.transform.position && WhatIsOnTile(selectedTile) == "Empty")
            unit.targetPosition = new Vector3Int(lastClickedTile.xPos, lastClickedTile.yPos, -1);
        else if (Input.GetMouseButtonDown(0) && !unit.isMoving && lastClickedTile != null &&
            lastClickedTile.transform.position == selectedTile.transform.position && WhatIsOnTile(selectedTile) != "Empty")
        {
            if (unit.GetComponent<Player>() != null) // Therefore is the player unit
            {
                if (WhatIsOnTile(selectedTile) != "Player" && WhatIsOnTile(selectedTile) != "Empty") // Change this later when items or another type is added to the whatisontile method
                {
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if (enemies[i].name == WhatIsOnTile(selectedTile))
                            unit.Attack(unit, enemies[i]);
                    }
                }
            }
            else
            {
                if (WhatIsOnTile(selectedTile) == "Player")
                    unit.Attack(unit, player);
                else if (WhatIsOnTile(selectedTile) != "Empty")
                    Debug.Log(unit.name + " can't attack his brothers");
            }
        }
    }
    #endregion

    #region TileOccupation

    public string WhatIsOnTile(Tile tile)
    {
        if (player.xPos == tile.xPos && player.yPos == tile.yPos)
            return "Player";

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].xPos == tile.xPos && enemies[i].yPos == tile.yPos)
                return enemies[i].name;
        }
        // check for items/obstacles/e.g.

        return "Empty";
    }

    #endregion

    #region HelperFunctions

    public Tile PositionToTile(Vector3Int tilePosition)
    {
        foreach (Tile tile in tiles)
        {
            if (tile.xPos == tilePosition.x)
            {
                if (tile.yPos == tilePosition.y)
                    return tile;
            }
        }
        return null;
    }

    public void UpdateUnitAndEnemyLists() // Very performance heavy, only use when you have no other choice
    { 
        units.Clear();
        enemies.Clear();
        units = FindObjectsOfType<Unit>().Select(unit => unit).ToList();
        foreach (Unit unit in units)
        {
            if (unit.gameObject.TryGetComponent<Player>(out Player player))
                continue;
            enemies.Add(unit.gameObject.GetComponent<Enemy>());
        }
    }

    public void RemoveEnemyFromLists(Enemy enemy)
    { 
        enemies.Remove(enemy);
        units.Remove(enemy);
    }

    #endregion

    #region CameraMovement

    private void MoveCameraToPlayer()
    {
        Vector3 playerPosition = new Vector3 (player.transform.position.x, player.transform.position.y, -10);
        mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, playerPosition, Time.deltaTime * 1f);
    }

    #endregion
}