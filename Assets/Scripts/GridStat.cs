// Written by Joy de Ruijter
using UnityEngine;

public class GridStat : MonoBehaviour
{
    #region Variables

    public int visited = -1;
    public int x = 0;
    public int y = 0;

    [SerializeField] private Material defaultMat;
    [SerializeField] private Material pathMat;
    [SerializeField] private Material targetMat;
    [HideInInspector] public bool isTargetTile = false;

    #endregion

    public void SetPathMaterial()
    {
        GetComponent<MeshRenderer>().material = pathMat;
    }

    public void SetDefaultMaterial()
    { 
        GetComponent<MeshRenderer>().material = defaultMat;
    }

    public void SetTargetMaterial()
    { 
        GetComponent<MeshRenderer>().material = targetMat;
    }
}
