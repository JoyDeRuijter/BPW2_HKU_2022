// Written by Joy de Ruijter
using UnityEngine;
using DG.Tweening;

public class LadyGhost : MonoBehaviour
{
    #region Variables

    private Animator anim;
    private UIManager uiManager;

    #endregion

    private void Start()
    {
        anim = GetComponent<Animator>();
        uiManager = UIManager.instance;
    }

    private void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Drowned") && transform != null)
            transform.DOMoveY(3f, 12f);

        if (transform.position.y >= 2.5f)
            uiManager.PlayScene("EndTitle");

    }
}
