// Written by Joy de Ruijter
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    #region Variables

    public Dictionary<Vector3Int, TileStatus> grid = new Dictionary<Vector3Int, TileStatus>();
    [SerializeField] private Dungeon.DungeonGenerator dungeonGen;

    #endregion

    private void Start()
    {
        LoadGrid();
    }

    private void Update()
    {
        
    }

    private void LoadGrid()
    {
        foreach (var kvp in dungeonGen.floorList)
        {
            grid.Add(kvp.Key, kvp.Value.GetComponent<GridTile>().status);
        }
    }

    /*
    public bool IsClickable(Vector3Int tilePosition)
    {
        foreach (var kvp in grid)
        {
            if (kvp.Key != tilePosition)
                continue;

            switch (kvp.Value)
            {
                case TileStatus.Obstacle:
                    return false;

                case TileStatus.Clean:
                    // Player can stand on it
                    return true;

                case TileStatus.Effect:
                    // Player can stand on it but an effect will be triggered
                    return true;

                case TileStatus.Interactable:
                    // Player will interact with this tile, behaviour depends on type of interactable
                    return true;

                default:
                    return false;
            }
        }
        return false;
    }
    */
}


