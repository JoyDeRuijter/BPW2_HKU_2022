// Written by Joy de Ruijter
using UnityEngine;

public class Enemy : Unit
{
    #region Variables

    [HideInInspector] public EnemyType enemyType;

    #endregion

    public override void Update()
    {
        base.Update();

        if(unitState == UnitStates.Action)
            CalculateNewTargetPosition();
    }

    private void CalculateNewTargetPosition()
    {
        Vector3Int playerPosition = new Vector3Int(gameManager.player.xPos, gameManager.player.yPos, -1);
        Vector3Int enemyPosition = new Vector3Int(xPos, yPos, -1);
        Vector3Int direction = (playerPosition - enemyPosition);

        if (direction.x != 0)       // Player is on the right or the left of the enemy
        {
            Vector3Int possibleTile;

            if (direction.x < 0)    // Player is on the left of the enemy
                possibleTile = new Vector3Int(xPos - 1, yPos, 0);
            else                    // Player is on the right of the enemy
                possibleTile = new Vector3Int(xPos + 1, yPos, 0);

            if (IsPossibleTile(possibleTile))
                return;
        }
        if (direction.y != 0)       // Player is above or below the enemy
        {
            Vector3Int possibleTile;
            if (direction.y < 0)    // Player is lower than the enemy
                possibleTile = new Vector3Int(xPos, yPos - 1, 0);
            else                    // Player is higher than the enemy
                possibleTile = new Vector3Int(xPos, yPos + 1, 0);

            if (IsPossibleTile(possibleTile))
                return;
        }
        Debug.Log("Enemy: " + name + " could not find a new targetposition");
    }

    private bool IsPossibleTile(Vector3Int possibleTilePosition)
    {
        if (gameManager.dungeonGenerator.dungeon[possibleTilePosition] == Dungeon.TileType.Floor)
        {
            targetPosition = new Vector3Int(possibleTilePosition.x, possibleTilePosition.y, -1);
            return true;
        }
        else
            return false;
    }

}

public enum EnemyType { Goblin, GoblinChief, GoblinBoss}
