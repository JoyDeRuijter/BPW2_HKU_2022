// Written by Joy de Ruijter
using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    #region Singleton

    public static Inventory instance;

    private void Awake()
    {
        if (instance != null)
            return;

        instance = this;
    }

    #endregion

    #region Variables

    public int capacity = 6;
    public List<PotionItem> potionItems = new List<PotionItem>();
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    #endregion

    private void Start()
    {
        if (potionItems != null)
            onItemChangedCallback.Invoke();
    }

    public bool Add(PotionItem potionItem)
    {
        if (potionItems.Count >= capacity)
        {
            Debug.Log("Inventory is full!");
            return false;
        }
        potionItems.Add(potionItem);

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();

        return true;
    }

    public void Remove(PotionItem potionItem)
    { 
        potionItems.Remove(potionItem);

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }
}
