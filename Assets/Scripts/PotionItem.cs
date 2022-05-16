// Written by Joy de Ruijter
using UnityEngine;

[CreateAssetMenu(fileName = "New PotionItem", menuName = "Items/PotionItem")]
public class PotionItem : ScriptableObject
{
    #region Variables

    public enum PotionType { Health, Experience, Stamina }
    public PotionType potionType;

    new public string name = "Insert Item Name";
    public Sprite icon = null;

    #endregion

    public void Use()
    {
        Player player = FindObjectOfType<Player>();
        UIManager uiManager = FindObjectOfType<UIManager>();

        switch (potionType)
        {
            case PotionType.Health:
                player.AddHealth(20);
                uiManager.ShowPotionPopup(2f, uiManager.potionPopupHealth);
                break;
            case PotionType.Experience:
                uiManager.ShowPotionPopup(2f, uiManager.potionPopupExperience);
                player.AddExperience(20);
                break;
            case PotionType.Stamina:
                uiManager.ShowPotionPopup(2f, uiManager.potionPopupStamina);
                player.AddStamina(20);
                break;
            default:
                break;
        }

        Inventory.instance.Remove(this);
    }

    public void RemoveFromInventory()
    {
        Inventory.instance.Remove(this);
    }
}
