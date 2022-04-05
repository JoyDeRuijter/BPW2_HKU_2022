// Written by Joy de Ruijter
using UnityEngine;

public class Player : Unit
{
    #region Variables

    [Header("Materials")]
    [SerializeField] private Material[] materials;

    [Header("References")]
    [Space(10)]
    [SerializeField] private SkinnedMeshRenderer character;

    private int index = 0;

    #endregion

    public override void Update()
    {
        base.Update();

        // Purely for testing right now, should be used in a customization menu of some sorts or selecting an armor in inventory
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (index >= materials.Length) 
                index = 0;
            character.material = materials[index];
            uiManager.ChangeHeroImage(index);
            index++;    
        }
    }
}
