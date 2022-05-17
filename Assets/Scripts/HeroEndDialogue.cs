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
    [SerializeField] private bool isIntro;
    [SerializeField] private Material[] materials;
    [SerializeField] private TMP_Text text;

    #endregion

    private void Start()
    {
        saver = SavePlayerData.instance;
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        playerData = saver.LoadData("SaveData");
        meshRenderer.material = materials[playerData.armorIndex];

        if (isIntro)
        {
            text.text = playerData.heroName + " you have been assigned a very important task. In order to speak to your deceased lover for one last time, you " +
                "shall have to enter the Forgotten Goblin Dungeon and find the stone golem. He will ask something in return for this favor..." +
                "BUT no time to waste! GO NOW!";
        }
        else
        { 
            text.text = "Because of your bravery, " + playerData.heroName + " got to say his last words to their deceased lover. \n He is forever grateful < 3";
        }
    }
}
