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

    private Tile previouslySelectedTile;
    private Player player;
    private List<Unit>units = new List<Unit>();
    private int beginIndex;
    private Dungeon.DungeonGenerator dungeonGenerator;

    #endregion

    private void Start()
    {
        dungeonGenerator = FindObjectOfType<Dungeon.DungeonGenerator>();
        player = FindObjectOfType<Player>();
        units = FindObjectsOfType<Unit>().Select(unit => unit).ToList();
        DetermineFirstUnitTurn();
    }

    private void Update()
    {
        TileToUnitMovement(units[beginIndex]);
        MoveCameraToPlayer();
    }

    #region UnitTurns

    private void DetermineFirstUnitTurn()
    {
        for(int i = 0; i < units.Count; i++)
        {
            if (units[i].TryGetComponent<Player>(out Player player))
                beginIndex = i;
        }
        units[beginIndex].unitState = Unit.UnitStates.StartTurn;
    }



    private IEnumerator TurnTimer(float seconds)
    { 
        yield return new WaitForSeconds(seconds);
    }

    #endregion

    #region TileToUnitMovement
    // Check if the given tile is in range of the unit and if so, act accordingly
    private void CheckUnitRange(Tile tile, Unit unit)
    {
        if (unit.isMoving)
            return;

        Vector2Int tilePosition = new Vector2Int(tile.xPos, tile.yPos);
        Vector3Int tilePositionDungeon = new Vector3Int(tile.xPos, tile.yPos, 0);
        Vector2Int unitPosition = new Vector2Int(unit.xPos, unit.yPos);
        float distance = Vector2Int.Distance(tilePosition, unitPosition);

        if (distance == 1 && dungeonGenerator.dungeon[tilePositionDungeon] == Dungeon.TileType.Floor)
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

        if (Input.GetMouseButtonDown(0) && !unit.isMoving)
            unit.targetPosition = new Vector3Int(lastClickedTile.xPos, lastClickedTile.yPos, -1);
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
