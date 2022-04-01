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
    [HideInInspector] public UnitStates unitState = UnitStates.Waiting;
    [HideInInspector] public bool completedAction;

    #endregion

    private void Start()
    {
        xPos = (int)transform.position.x;
        yPos = (int)transform.position.y;
    }

    private void Update()
    {
        ActOnState();
        Move();
        CheckForMovement();
    }

    #region UnitMovement

    private void Move()
    {
        if (transform.position != targetPosition && targetPosition != Vector3Int.zero)
        {
            float step = movementSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * step);
            xPos = (int)transform.position.x;
            yPos = (int)transform.position.y;
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
                completedAction = false;
                break;
            case UnitStates.StartTurn:
                Debug.Log("Start turn " + name);
                break;
            case UnitStates.Action:
                Debug.Log("Action turn " + name);
                //Move();
                //StartCoroutine(CheckForMovement());
                completedAction = true;
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
