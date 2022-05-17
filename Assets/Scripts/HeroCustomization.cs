// Written by Joy de Ruijter
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class HeroCustomization : MonoBehaviour
{
    #region Variables

    [Header("Materials")]
    [SerializeField] private Material[] materials;

    [Header("Texts")]
    [Space(10)]
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text playerNameInput;
    [SerializeField] private TMP_Text armorName;
    [SerializeField] private TMP_Text weaponName;

    [Header("References")]
    [Space(10)]
    [SerializeField] private SkinnedMeshRenderer character;
    [SerializeField] private MeshRenderer[] weapons;
    [SerializeField] private GameObject[] weaponObjects;
    [SerializeField] private GameObject[] camWeaponObjects;

    private int armorIndex = 0;
    private int weaponIndex = 0;
    private string[] armorNames = new string[4];
    private string[] weaponNames = new string[3];

    private SavePlayerData saver;

    #endregion

    private void Awake()
    {
        InitializeArmorNames();
        InitializeWeaponNames();
    }

    private void Start()
    {
        saver = SavePlayerData.instance;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * 10);
    }

    private void InitializeArmorNames()
    {
        armorNames[0] = "Blood Tusk Plating";
        armorNames[1] = "Loyal King's Plating";
        armorNames[2] = "Forest Kissed Plating";
        armorNames[3] = "Nocturnal Owl Plating";
    }

    private void InitializeWeaponNames()
    {
        weaponNames[0] = "Sword of Heroes";
        weaponNames[1] = "Hammer of Mountains";
        weaponNames[2] = "Mace of Suffering";
    }

    public void LeftArmor()
    { 
        if (armorIndex >= 1)
            armorIndex--;
        else
            armorIndex = materials.Length - 1;

        UpdateArmor();
    }

    public void RightArmor()
    {
        if (armorIndex < materials.Length - 1)
            armorIndex++;
        else
            armorIndex = 0;

        UpdateArmor();
    }

    private void UpdateArmor()
    { 
        character.material = materials[armorIndex];

        foreach (var weapon in weapons)
            weapon.material = materials[armorIndex];

        armorName.text = armorNames[armorIndex].ToUpper();
    }

    public void LeftWeapon()
    { 
        if (weaponIndex >= 1)
            weaponIndex--;
        else
            weaponIndex = weaponObjects.Length - 1;

        UpdateWeapon();
    }

    public void RightWeapon()
    {
        if (weaponIndex < weaponObjects.Length - 1)
            weaponIndex++;
        else
            weaponIndex = 0;

        UpdateWeapon();
    }

    private void UpdateWeapon()
    {
        foreach (var weapon in weaponObjects)
            weapon.SetActive(false);

        foreach (var weapon in camWeaponObjects)
            weapon.SetActive(false);

        weaponObjects[weaponIndex].SetActive(true);
        camWeaponObjects[weaponIndex].SetActive(true);

        weaponName.text = weaponNames[weaponIndex].ToUpper();
    }

    public void UpdateName()
    {
        playerName.text = playerNameInput.text;
    }

    public void Continue(string scene)
    {
        saver.CreatePlayerData(playerName.text, weaponIndex, armorIndex);
        saver.SaveData("SaveData");

        SceneManager.LoadScene(scene);
    }
}
