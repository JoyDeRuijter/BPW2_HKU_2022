// Written by Joy de Ruijter
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject statusBars;

    private Animator playerPanelAnim;

    #endregion

    private void Awake()
    {
        playerPanelAnim = playerPanel.GetComponent<Animator>();
    }

    private void Update()
    {
        
    }

    public void OnPlayerPanelButtonClick()
    {
        playerPanelAnim.SetBool("ShouldOpen", !playerPanelAnim.GetBool("ShouldOpen"));
    }
}
