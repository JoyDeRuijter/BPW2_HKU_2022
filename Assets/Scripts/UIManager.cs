// Written by Joy de Ruijter
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


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

    [Header("References")]
    [Space(10)]
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject experienceBar;
    [SerializeField] private GameObject staminaBar;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private TMP_Text experienceText;
    [SerializeField] private TMP_Text staminaText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image HeroImage;
    [SerializeField] private Sprite[] HeroShots;
    [SerializeField] private AudioSource clickSound;

    private Animator playerPanelAnim;
    private GameManager gameManager;
    private Player player;
    

    #endregion

    private void Start()
    {
        gameManager = GameManager.instance;
        player = gameManager.player;

        if (isMainMenu)
            return;

        playerPanelAnim = playerPanel.GetComponent<Animator>();
    }

    private void Update()
    {
        //UpdateHealthBar();
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
        healthBar.GetComponent<Slider>().value = player.currentHealth;
        healthText.text = player.currentHealth.ToString();
    }
}
