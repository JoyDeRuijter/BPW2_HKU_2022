// Written by Joy de Ruijter
using UnityEngine;

public class Potion : MonoBehaviour
{
    #region Variables

    public enum PotionType{ Health, Experience, Stamina }
    public PotionType potionType;
    [HideInInspector] public int roomID;
    [HideInInspector] public int xPos;
    [HideInInspector] public int yPos;

    private SphereCollider sphereCollider;
    private GameManager gameManager;
    private Dungeon.DungeonGenerator dungeonGenerator;

    #endregion

    private void Start()
    {
        gameManager = GameManager.instance;
        dungeonGenerator = gameManager.dungeonGenerator;
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInChildren<Player>() == null)
            return;

        Debug.Log("Player collected a " + potionType + " potion");
        // collect the potion
        dungeonGenerator.rooms[roomID].DeleteAPotion(xPos, yPos);
        Destroy(gameObject);
    }


}
