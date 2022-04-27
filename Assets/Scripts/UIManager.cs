// Written by Joy de Ruijter
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class UIManager : MonoBehaviour
{
    #region Singleton

    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    #region Variables
    [Header("Is this the Main Menu?")]
    [SerializeField] private bool isMainMenu;

    [Header("Is this the Customization Menu?")]
    [Space(10)]
    [SerializeField] private bool isCustomizationMenu;

    [Header("References")]
    [Space(10)]
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject healPopup;
    [SerializeField] private GameObject damagePopup;
    [SerializeField] private Slider experienceBar;
    [SerializeField] private Slider staminaBar;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image HeroImage;
    [SerializeField] private Image weaponImage;
    [SerializeField] private Sprite[] HeroShots;
    [SerializeField] private Sprite[] weaponShots;
    [SerializeField] private Material[] materials;
    [SerializeField] private AudioSource clickSound;
    [SerializeField] private Button healButton;
    [SerializeField] private Button damageButton;
    [SerializeField] private TMP_Text heroName;
    [SerializeField] private SkinnedMeshRenderer playerCharacter;

    private Animator playerPanelAnim;
    private GameManager gameManager;
    private Player player;

    private SavePlayerData saver;
    private PlayerData playerData;
    
    #endregion

    private void Start()
    {
        gameManager = GameManager.instance;

        if (isMainMenu || isCustomizationMenu)
            return;

        playerPanelAnim = playerPanel.GetComponent<Animator>();

        saver = SavePlayerData.instance;

        playerData = saver.LoadData("SaveData");
        player = FindObjectOfType<Player>();
        playerCharacter = player.gameObject.transform.Find("Character").transform.Find("Character_Hero_Knight_Male").GetComponent<SkinnedMeshRenderer>();
        InitializePlayerData();
    }

    private void Update()
    {
        if (isMainMenu || isCustomizationMenu)
            return;

        if (playerPanelAnim.GetCurrentAnimatorStateInfo(0).IsName("PlayerPanelOpenStatic") || playerPanelAnim.GetCurrentAnimatorStateInfo(0).IsName("PlayerPanelOpen"))
        {
            UpdateHealthBar();
            UpdateStaminaBar();
        }

        if (player != null && player.stamina != 100)
        {
            healButton.interactable = false;
            damageButton.interactable = false;
        }
        else if (player != null && player.stamina == 100)
        {
            healButton.interactable = true;
            damageButton.interactable = true;
        }
    }

    private void InitializePlayerData()
    {
        heroName.text = playerData.heroName;
        HeroImage.sprite = HeroShots[playerData.armorIndex];
        weaponImage.sprite = weaponShots[playerData.weaponIndex];
        playerCharacter.material = materials[playerData.armorIndex];
    }

    public void QuitGame()
    {
        //PlayClickSound();
        StartCoroutine(Wait(0.5f));
        Debug.Log("Quit game");
        Application.Quit();
    }

    public void PlayScene(string scene)
    {
        //PlayClickSound();
        StartCoroutine(Wait(0.5f));
        SceneManager.LoadScene(scene);
    }

    private IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    private IEnumerator ShowPopup(float seconds, GameObject popup)
    { 
        popup.SetActive(true);
        yield return new WaitForSeconds(seconds);
        popup.SetActive(false);
    }

    private void PlayClickSound()
    {
        clickSound.Play();
    }

    public void OnPlayerPanelButtonClick()
    {
        playerPanelAnim.SetBool("ShouldOpen", !playerPanelAnim.GetBool("ShouldOpen"));
    }

    public void ChangeHeroImage(int index)
    {
        HeroImage.sprite = HeroShots[index];
    }

    private void UpdateHealthBar()
    {
        player = gameManager.player;
        healthBar.value = player.GetHealth();
    }

    private void UpdateStaminaBar()
    {
        player = gameManager.player;
        staminaBar.value = player.GetStamina();
    }

    public void HealAbility()
    {
        player = gameManager.player;
        StartCoroutine(ShowPopup(2f, healPopup));
        player.UseAbility(0);
    }

    public void DamageAbility()
    {
        player = gameManager.player;
        StartCoroutine(ShowPopup(2f, damagePopup));
        player.UseAbility(1);
    }
}
