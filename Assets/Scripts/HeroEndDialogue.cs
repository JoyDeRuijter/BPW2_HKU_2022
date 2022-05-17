// Written by Joy de Ruijter
using UnityEngine;
using TMPro;

public class HeroEndDialogue : MonoBehaviour
{
    #region Variables

    private SkinnedMeshRenderer meshRenderer;
    private SavePlayerData saver;
    private PlayerData playerData;

    [Header("References")]
    [SerializeField] private Material[] materials;
    [SerializeField] private TMP_Text text;

    #endregion

    private void Start()
    {
        saver = SavePlayerData.instance;
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        playerData = saver.LoadData("SaveData");
        meshRenderer.material = materials[playerData.armorIndex];
        text.text = "Because of your bravery, " + playerData.heroName + " got to say his last words to his deceased lover. \n He is forever grateful < 3";
    }

    private void Update()
    {
        
    }
}
