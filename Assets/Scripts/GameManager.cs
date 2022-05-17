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
        player = FindObjectOfType<Player>();
        dungeonGenerator = FindObjectOfType<Dungeon.DungeonGenerator>();
    }

    #endregion

    #region Variables

    [Header("References")]
    [SerializeField] private Camera mainCamera;

    [HideInInspector] public Tile selectedTile;
    [HideInInspector] public Tile lastClickedTile;
    [HideInInspector] public GameState gameState;

    private Tile previouslySelectedTile;
    private List<Unit>units = new List<Unit>();
    private int beginIndex;
    private int activeEnemy;
    public List<Enemy>enemies = new List<Enemy>();
    public List<Enemy>enemiesInRoom = new List<Enemy>();
    [HideInInspector] public Player player;
    [HideInInspector] public List<Tile>tiles = new List<Tile>();
    [HideInInspector] public Dungeon.DungeonGenerator dungeonGenerator;

    #endregion

    private void Start()
    {
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
            if (units[i].TryGetComponent<Player>(out _))
                beginIndex = i;
        }
        units[beginIndex].unitState = Unit.UnitStates.StartTurn;
    }

    public void SwitchTurnState()
    {
        if (gameState == GameState.PlayerTurn)
        {
            activeEnemy = 0;
            gameState = GameState.EnemyTurn;
        }
        else
            gameState = GameState.PlayerTurn;
    }

    public void GoToNextEnemy()
    {
        if (enemiesInRoom.Count != 0 && activeEnemy == enemiesInRoom.Count - 1)
            SwitchTurnState();
        else
            activeEnemy++;
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
                break;

            case GameState.EnemyTurn:
                if (player.isInRoom)
                    UpdateEnemiesInRoom(player.roomID);
                else
                {
                    gameState = GameState.PlayerTurn;
                    break;
                }
                if (enemiesInRoom == null || enemiesInRoom.Count == 0)
                {
                    gameState = GameState.PlayerTurn;
                    break;
                }
                CheckUnitRange(selectedTile, enemiesInRoom[activeEnemy]);
                if (enemiesInRoom[activeEnemy].unitState == Unit.UnitStates.Waiting)
                    enemiesInRoom[activeEnemy].unitState = Unit.UnitStates.StartTurn;
                if (enemiesInRoom[activeEnemy].unitState == Unit.UnitStates.Action)
                    TileToUnitMovement(enemiesInRoom[activeEnemy]);
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

        if (distance == 1 && (dungeonGenerator.dungeon[tilePositionDungeon] == Dungeon.TileType.Floor || dungeonGenerator.dungeon[tilePositionDungeon] == Dungeon.TileType.Corridor) &&
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
                    string attackedEnemy = "";
                    for (int i = 0; i < enemiesInRoom.Count; i++)
                    { 
                        if (enemiesInRoom[i].name == WhatIsOnTile(selectedTile) && enemiesInRoom[i].name != attackedEnemy)
                        {
                            attackedEnemy = enemiesInRoom[i].name;
                            unit.Attack(unit, enemiesInRoom[i]);
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region TileOccupation

    public string WhatIsOnTile(Tile tile)
    {
        if (player.xPos == tile.xPos && player.yPos == tile.yPos)
            return "Player";

        for (int i = 0; i < enemiesInRoom.Count; i++)
        {
            if (enemiesInRoom[i].xPos == tile.xPos && enemiesInRoom[i].yPos == tile.yPos)
                return enemiesInRoom[i].name;
        }

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

    public void RemoveEnemyFromLists(Enemy enemy)
    { 
        enemies.Remove(enemy);
        enemiesInRoom.Remove(enemy);
        units.Remove(enemy);
    }

    public void UpdateEnemiesInRoom(int playerRoomID)
    { 
        enemiesInRoom.Clear();
        foreach (Enemy enemy in enemies)
        { 
            if(enemy.roomID == playerRoomID)
                enemiesInRoom.Add(enemy);
        }
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
