// Written by Joy de Ruijter
using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    #region Variables

    [Header ("Unit Properties")]
    public new string name;
    public float movementSpeed;
    public int level;
    public int damage;
    public int maxHealth;

    [HideInInspector] public int currentHealth;
    [HideInInspector] public int xPos;
    [HideInInspector] public int yPos;
    [HideInInspector] public Vector3Int targetPosition;
    [HideInInspector] public bool isMoving;
    public UnitStates unitState;
    [HideInInspector] public bool completedAction;

    #endregion

    private void Start()
    {
        xPos = (int)transform.position.x;
        yPos = (int)transform.position.y;
        targetPosition = new Vector3Int(xPos, yPos, -1);
        unitState = UnitStates.Waiting;
    }

    private void Update()
    {
        ActOnState();
        StartCoroutine(CheckForMovement());
    }

    #region UnitMovement

    private IEnumerator Move()
    {
        if (transform.position != targetPosition && targetPosition != Vector3Int.zero)
        {
            while (transform.position != targetPosition)
            { 
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * movementSpeed);
                yield return new WaitForSeconds(4f);
            }
            xPos = (int)transform.position.x;
            yPos = (int)transform.position.y;
            completedAction = true;
        }
    }

    private IEnumerator CheckForMovement()
    {
        Vector3 startPosition = transform.position;
        yield return new WaitForSeconds(0.3f);

        if (transform.position != startPosition)
            isMoving = true;
        else
            isMoving = false;

    }

    #endregion

    #region UnitStates

    public enum UnitStates {Waiting, StartTurn, Action, EndTurn};

    private void ActOnState()
    {
        switch (unitState)
        {
            case UnitStates.Waiting:
                Debug.Log(name + " is waiting on its turn");
                targetPosition = new Vector3Int(xPos, yPos, -1);
                completedAction = false;
                break;
            case UnitStates.StartTurn:
                Debug.Log("Start turn " + name);
                // Indicator UI that it's the unit's turn with ienumerator?
                targetPosition = new Vector3Int(xPos, yPos, -1);
                unitState = UnitStates.Action;
                break;
            case UnitStates.Action:
                Debug.Log("Action turn " + name);
                //while (unitState == UnitStates.Action && !completedAction)
                    StartCoroutine(Move());
                if (completedAction)
                    unitState = UnitStates.EndTurn;
                break;
            case UnitStates.EndTurn:
                Debug.Log("End turn " + name);
                break;
            default:
                Debug.Log(name + " has no UnitState");
                break;
        }    
    }

    #endregion
}
