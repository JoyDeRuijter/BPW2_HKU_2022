// Written by Joy de Ruijter
using UnityEngine;

public class GridTile : MonoBehaviour
{
    #region Variables

    [Header("Current tile status")]
    public TileStatus status;
    public bool isTargetTile;

    [Header("Tile Materials")]
    [Space(10)]
    [SerializeField] private Material defaultMat;
    [SerializeField] private Material pathMat;
    [SerializeField] private Material targetMat;

    [HideInInspector] public int visited = -1;
    [HideInInspector] public int x;
    [HideInInspector] public int y;

    #endregion

    private void Awake()
    {
        x = (int)transform.position.x;
        y = (int)transform.position.y;
    }

    private void Update()
    {
        
    }

    // Set material to default material
    public void SetDefaultMaterial()
    {
        GetComponentInChildren<MeshRenderer>().material = defaultMat;
    }

    // Set material to path material
    public void SetPathMaterial()
    {
        GetComponentInChildren<MeshRenderer>().material = pathMat;
    }

    // Set material to target material
    public void SetTargetMaterial()
    {
        GetComponentInChildren<MeshRenderer>().material = targetMat;
    }
}

public enum TileStatus { Clean, Player, Effect, Obstacle, Interactable }

