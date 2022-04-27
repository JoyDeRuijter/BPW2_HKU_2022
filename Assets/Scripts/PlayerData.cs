// Written by Joy de Ruijter
using UnityEngine;

public class PlayerData
{
    #region Variables

    public string heroName;
    public int weaponIndex;
    public int armorIndex;

    #endregion

    public PlayerData(string heroName, int weaponIndex, int armorIndex)
    { 
        this.heroName = heroName;
        this.weaponIndex = weaponIndex;
        this.armorIndex = armorIndex;
    }
}
