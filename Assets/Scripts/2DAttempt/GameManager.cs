// Written by Joy de Ruijter
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
    private PlayerMovement playerMovement;

    #endregion

    private void Start()
    {
        player = FindObjectOfType<Player>();
        playerMovement = player.gameObject.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        TileToPlayerMovement();
        MoveCameraToPlayer();
    }

    #region TileToPlayermovement
    // Check if the given tile is in range of the player and if so, act accordingly
    private void CheckPlayerRange(Tile tile)
    {
        if (playerMovement.isMoving)
            return;

        Vector2Int tilePosition = new Vector2Int(tile.xPos, tile.yPos);
        Vector2Int playerPosition = new Vector2Int(player.xPos, player.yPos);
        float distance = Vector2Int.Distance(tilePosition, playerPosition);

        if (distance == 1)
        {
            tile.isInPlayerRange = true;
            tile.highLight.SetActive(true);
        }
        else
            tile.isInPlayerRange = false;
    }

    // If there is a new selected tile and it's in range of the player, give the player a new targetposition
    private void TileToPlayerMovement()
    {
        if (selectedTile != null && previouslySelectedTile != selectedTile && !playerMovement.isMoving)
        {
            CheckPlayerRange(selectedTile.GetComponent<Tile>());
            previouslySelectedTile = selectedTile.GetComponent<Tile>();
        }

        if (Input.GetMouseButtonDown(0) && !playerMovement.isMoving)
            playerMovement.targetPosition = new Vector3Int(lastClickedTile.xPos, lastClickedTile.yPos, -1);
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
