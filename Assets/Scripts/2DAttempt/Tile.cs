// Written by Joy de Ruijter
using UnityEngine;

public class Tile : MonoBehaviour
{
    #region Variables

    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private GameObject highLight;

    #endregion

    public void Initialize(bool isOffset)
    {
        renderer.color = isOffset ? offsetColor : baseColor;
    }

    private void OnMouseEnter()
    {
        highLight.SetActive(true);
    }

    private void OnMouseExit()
    {
        highLight.SetActive(false);
    }
}
