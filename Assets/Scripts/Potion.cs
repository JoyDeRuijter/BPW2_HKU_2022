// Written by Joy de Ruijter
using UnityEngine;

public class Potion : MonoBehaviour
{
    #region Variables

    [SerializeField] PotionItem potionItem;
    [HideInInspector] public int roomID;
    [HideInInspector] public int xPos;
    [HideInInspector] public int yPos;

    private GameManager gameManager;
    private Dungeon.DungeonGenerator dungeonGenerator;

    #endregion

    private void Start()
    {
        gameManager = GameManager.instance;
        dungeonGenerator = gameManager.dungeonGenerator;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInChildren<Player>() == null)
            return;
        PickUp();
    }

    private void PickUp()
    {
        bool wasPickedUp = Inventory.instance.Add(potionItem);

        if (!wasPickedUp)
            return;

        dungeonGenerator.rooms[roomID].DeleteAPotion(xPos, yPos);
        Destroy(gameObject);
    }

}
