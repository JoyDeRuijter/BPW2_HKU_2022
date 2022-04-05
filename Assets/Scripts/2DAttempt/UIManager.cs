// Written by Joy de Ruijter
using UnityEngine;
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

    [Header("References")]
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject statusBars;
    [SerializeField] private Image HeroImage;
    [SerializeField] private Sprite[] HeroShots;

    private Animator playerPanelAnim;

    #endregion

    private void Start()
    {
        playerPanelAnim = playerPanel.GetComponent<Animator>();
    }

    private void Update()
    {
        
    }

    public void OnPlayerPanelButtonClick()
    {
        playerPanelAnim.SetBool("ShouldOpen", !playerPanelAnim.GetBool("ShouldOpen"));
        Debug.Log("Detects panel button click");
    }

    public void ChangeHeroImage(int index)
    {
        HeroImage.sprite = HeroShots[index];
    }
}
