// Written by Joy de Ruijter
using UnityEngine;

public class GridStat : MonoBehaviour
{
    #region Variables

    [Header("Tile Materials")]
    [SerializeField] private Material defaultMat;
    [SerializeField] private Material pathMat;
    [SerializeField] private Material targetMat;

    [HideInInspector] public int visited = -1;
    [HideInInspector] public int x = 0;
    [HideInInspector] public int y = 0;
    [HideInInspector] public bool isTargetTile = false;

    #endregion

    // Set material to default material
    public void SetDefaultMaterial()
    { 
        GetComponent<MeshRenderer>().material = defaultMat;
    }

    // Set material to path material
    public void SetPathMaterial()
    {
        GetComponent<MeshRenderer>().material = pathMat;
    }

    // Set material to target material
    public void SetTargetMaterial()
    { 
        GetComponent<MeshRenderer>().material = targetMat;
    }
}
