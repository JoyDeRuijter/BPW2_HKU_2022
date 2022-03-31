// Written by Joy de Ruijter
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Variables

    [HideInInspector] public int xPos;
    [HideInInspector] public int yPos;

    #endregion

    private void Awake()
    {
        xPos = (int)transform.position.x;
        yPos = (int)transform.position.y;
    }
}
