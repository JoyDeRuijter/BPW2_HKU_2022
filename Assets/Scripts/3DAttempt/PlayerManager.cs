// Written by Joy de Ruijter
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Singleton

    public static PlayerManager instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    #region Variables

    [Header ("Player Reference")]
    public GameObject player;

    #endregion
}
