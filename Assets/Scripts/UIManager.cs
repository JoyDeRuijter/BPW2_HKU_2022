// Written by Joy de Ruijter
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


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
    [SerializeField] private GameObject statusBars;
    [SerializeField] private Image HeroImage;
    [SerializeField] private Sprite[] HeroShots;
    [SerializeField] private AudioSource clickSound;

    private Animator playerPanelAnim;
    

    #endregion

    private void Start()
    {
        if (isMainMenu)
            return;

        playerPanelAnim = playerPanel.GetComponent<Animator>();
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
}
