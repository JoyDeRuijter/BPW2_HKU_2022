// Written by Joy de Ruijter
using System.IO;
using UnityEngine;

public class SavePlayerData : MonoBehaviour
{
    #region Variables

    #region Singleton

    public static SavePlayerData instance;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    private PlayerData playerData;
    private string path = "";
    private string persistentPath = "";

    [HideInInspector] public string tempData;

    #endregion

    public void CreatePlayerData(string heroName, int weaponIndex, int armorIndex)
    {
        playerData = new PlayerData(heroName, weaponIndex, armorIndex);
    }

    private void SetPaths(string fileName)
    {
        path = Application.dataPath + Path.AltDirectorySeparatorChar + fileName + ".json";
        persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + fileName + ".json";
    }

    public void SaveData(string fileName)
    {
        SetPaths(fileName);
        string savePath = path;
        string json = JsonUtility.ToJson(playerData);
        using StreamWriter writer = new StreamWriter(savePath);
        writer.Write(json);
        writer.Close();
        writer.Dispose();
    }

    public PlayerData LoadData(string fileName)
    {
        SetPaths(fileName);
        using StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);
        reader.Close();
        reader.Dispose();
        return data;
    }
}
