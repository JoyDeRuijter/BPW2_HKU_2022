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
    [SerializeField] private Slider experienceBar;
    [SerializeField] private Slider staminaBar;
    [SerializeField] private Slider healthBar;
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

        if (isMainMenu)
            return;
        playerPanelAnim = playerPanel.GetComponent<Animator>();
    }

    private void Update()
    {
        if (playerPanelAnim.GetCurrentAnimatorStateInfo(0).IsName("PlayerPanelOpenStatic"))
            UpdateHealthBar();
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
        player = gameManager.player;
        healthBar.value = player.GetHealth();
    }
}
