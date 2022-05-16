// Written by Joy de Ruijter
using UnityEngine;

//TODO: MAKE THE ENEMY ONLY MOVE WHEN THEY ARE IN THE SAME ROOM AS THE PLAYER
//      BEST DO THAT IN THE STATEMACHINE, SINCE THEY SHOULDN'T EVEN GET A TURN WHEN THEY ARE NOT IN THE SAME ROOM

public class Enemy : Unit
{
    #region Variables

    [HideInInspector] public EnemyType enemyType;
    //[HideInInspector] public int roomID;

    #endregion

    public override void Update()
    {
        base.Update();

        if(unitState == UnitStates.Action && !doingAction)
            CalculateNewTargetPosition();
    }

    protected override void EndTurn()
    {
        base.EndTurn();
        gameManager.GoToNextEnemy();
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
        if (AlternativePossibleTile())
            return;
        else
            completedAction = true;
    }

    private bool AlternativePossibleTile()
    {
        if (IsPossibleTile(new Vector3Int(xPos + 1, yPos, 0)))
            return true;
        else if (IsPossibleTile(new Vector3Int(xPos - 1, yPos, 0)))
            return true;
        else if (IsPossibleTile(new Vector3Int(xPos, yPos + 1, 0)))
            return true;
        else if (IsPossibleTile(new Vector3Int(xPos, yPos - 1, 0)))
            return true;
        else return false;
    }

    private bool IsPossibleTile(Vector3Int possibleTilePosition)
    {
        Tile possibleTile = gameManager.PositionToTile(possibleTilePosition);
        if (gameManager.dungeonGenerator.dungeon[possibleTilePosition] == Dungeon.TileType.Floor && !possibleTile.isOccupied)
        {
            targetPosition = new Vector3Int(possibleTilePosition.x, possibleTilePosition.y, -1);
            return true;
        }
        else if (gameManager.dungeonGenerator.dungeon[possibleTilePosition] == Dungeon.TileType.Floor && gameManager.WhatIsOnTile(possibleTile) == "Player" && !completedAction)
        {
            Attack(this, gameManager.player);
            return true;
        }
        else
            return false;
    }

}

public enum EnemyType { Normal, Boss}
