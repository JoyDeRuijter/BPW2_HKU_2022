// Written by Joy de Ruijter
using System.IO;
using UnityEngine;

public class SavePlayerData : MonoBehaviour
{
    #region Singleton

    public static SavePlayerData instance;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region Variables

    private PlayerData playerData;
    private string path = "";

    [HideInInspector] public string tempData;

    #endregion

    public void CreatePlayerData(string heroName, int weaponIndex, int armorIndex)
    {
        playerData = new PlayerData(heroName, weaponIndex, armorIndex);
    }

    private void SetPath(string fileName)
    {
        path = Application.dataPath + Path.AltDirectorySeparatorChar + fileName + ".json";
    }

    public void SaveData(string fileName)
    {
        SetPath(fileName);
        string savePath = path;
        string json = JsonUtility.ToJson(playerData);
        using StreamWriter writer = new StreamWriter(savePath);
        writer.Write(json);
        writer.Close();
        writer.Dispose();
    }

    public PlayerData LoadData(string fileName)
    {
        SetPath(fileName);
        using StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);
        reader.Close();
        reader.Dispose();
        return data;
    }
}
