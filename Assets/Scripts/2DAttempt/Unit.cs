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

    [Header("SFX")]
    [SerializeField] private ParticleSystem startTurnEffect;
    [SerializeField] private Outline outliner;

    [HideInInspector] public int currentHealth;
    [HideInInspector] public int xPos;
    [HideInInspector] public int yPos;
    [HideInInspector] public Vector3Int targetPosition;
    [HideInInspector] public bool isMoving;
    public UnitStates unitState;
    [HideInInspector] public bool completedAction;


    private bool hasPlayedVfx;

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
                yield return new WaitForSeconds(1f);
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
                targetPosition = new Vector3Int(xPos, yPos, -1);
                completedAction = false;
                hasPlayedVfx = false;
                outliner.enabled = false;
                break;
            case UnitStates.StartTurn:
                // Indicator UI that it's the unit's turn with ienumerator?
                //if (!hasPlayedVfx)
                //{ 
                //    startTurnEffect.Play();
                //    hasPlayedVfx = true;
                //    Debug.Log("Play vfx");
                //}
                outliner.enabled = true;
                targetPosition = new Vector3Int(xPos, yPos, -1);
                unitState = UnitStates.Action;
                break;
            case UnitStates.Action:
                StartCoroutine(Move());
                if (completedAction)
                    unitState = UnitStates.EndTurn;
                hasPlayedVfx = false;
                break;
            case UnitStates.EndTurn:
                outliner.enabled = false;
               hasPlayedVfx = false;
                break;
            default:
                break;
        }    
    }

    #endregion
}
