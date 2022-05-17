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
    [HideInInspector] public int stamina = 100;
    [HideInInspector] public int experience = 0;
    [HideInInspector] public int coins = 0;

    [HideInInspector] public float timerValue = 50f;

    public bool isInRoom;

    #endregion

    public override void Update()
    {
        base.Update();

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
        isInRoom = IsInRoom();
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

    public void CollectCoin()
    {
        if (coins >= 10)
            return;
        coins++;
        uiManager.AddCoinObject();
    }

    public void AddExperience(int xp)
    {
        if (experience + xp <= 100)
            experience += xp;
        else
            experience = 100;

        uiManager.UpdateExperience(experience);
    }

    public void AddHealth(int health)
    {
        if (currentHealth + health <= maxHealth)
            currentHealth += health;
        else
            currentHealth = maxHealth;

        uiManager.UpdateHealthBar();
    }

    public void AddStamina(int _stamina)
    {
        if (stamina + _stamina <= 100)
            stamina += _stamina;
        else
            stamina = 100;

        uiManager.UpdateStaminaBar();
    }

    public bool IsInRoom()
    {
        Vector3Int playerTile = new Vector3Int(xPos, yPos, 0);
        if (dungeonGenerator.dungeon[playerTile] == Dungeon.TileType.Floor)
        {
            if (roomID == -1)
                roomID = dungeonGenerator.UnitRoomID(this);
            return true;
        }
        else
        {
            roomID = -1;
            return false;
        }
    }
}
