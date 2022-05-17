// Written by Joy de Ruijter
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    #region Variables

    [Header("Properties")]
    public PotionItem potionItem;
    public Image icon;
    public Button removeButton;

    #endregion

    public void AddPotionItem(PotionItem newPotionItem)
    { 
        potionItem = newPotionItem;
        icon.sprite = potionItem.icon;
        icon.enabled = true;
        removeButton.interactable = true;
    }

    public void ClearSlot()
    {
        potionItem = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
    }

    public void OnRemoveButton()
    {
        Inventory.instance.Remove(potionItem);
    }

    public void UsePotionItem()
    { 
        if (potionItem != null)
            potionItem.Use();
    }
}
