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
    public int stamina = 100;
    [HideInInspector] public int experience = 0;

    public float timerValue = 50f;

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

        StaminaRegeneration();
    }

    private void StaminaRegeneration()
    {
        if (timerValue > 0f)
            timerValue--;
        else
            timerValue = 0f;

        if (stamina < 100 && timerValue == 0f)
        {
            stamina++;
            timerValue = 50f;
        }
        else if (stamina >= 100)
        {
            stamina = 100;
        }
    }

    protected override void EndTurn()
    {
        base.EndTurn();
        gameManager.SwitchTurnState();
    }

    public void UseAbility(int abilityIndex)
    {
        if (abilityIndex == 0) // HEAL
        {
            Debug.Log("Player used HEAL-ability");

            if (currentHealth + 25 <= maxHealth)
                currentHealth += 25;
            else
                currentHealth = maxHealth;

            stamina = 0;
        }

        else if (abilityIndex == 1) // INCREASE DAMAGE
        {
            Debug.Log("Player used DAMAGE-ability");

            damage += 5;
            stamina = 0;
        }
    }

    public int GetStamina()
    {
        return stamina;
    }
}
