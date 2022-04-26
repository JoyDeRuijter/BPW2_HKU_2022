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

    [Header("References")]
    [SerializeField] private Outline outliner;
    [SerializeField] private Animator anim;

    public int currentHealth;
    [HideInInspector] public int xPos;
    [HideInInspector] public int yPos;
    [HideInInspector] public Vector3Int targetPosition;
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool doingAction;
    public UnitStates unitState;
    public bool completedAction;

    [HideInInspector] public UIManager uiManager;
    [HideInInspector] public GameManager gameManager;

    #endregion

    public virtual void Start()
    {
        xPos = (int)transform.position.x;
        yPos = (int)transform.position.y;
        targetPosition = new Vector3Int(xPos, yPos, -1);
        unitState = UnitStates.Waiting;
        uiManager = UIManager.instance;
        gameManager = GameManager.instance;
        currentHealth = maxHealth;
    }

    public virtual void Update()
    {
        if (doingAction)
            return;
        ActOnState();
        StartCoroutine(CheckForMovement());
    }

    #region UnitMovement

    private IEnumerator Move()
    {
        if (transform.position != targetPosition && targetPosition != Vector3Int.zero)
        {
            Rotate(targetPosition);
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

    private void Rotate(Vector3Int targetPosition)
    {
        Vector3 lookDirection = (targetPosition - transform.position).normalized;
        switch (lookDirection)
        { 
            case Vector3 v when v.Equals(Vector3.up):
                transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(0f, 0f, 0f), 6f);
                break;
            case Vector3 v when v.Equals(Vector3.down):
                transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(0f, 0f, 180f), 6f);
                break;
            case Vector3 v when v.Equals(Vector3.right):
                transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(0f, 0f, -90f), 6f);
                break;
            case Vector3 v when v.Equals(Vector3.left):
                transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(0f, 0f, 90f), 6f);
                break;

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

    #region UnitActions

    public void Attack(Unit attacker, Unit victim)
    {
        anim.SetBool("Attack", true);

        Vector3Int lookPosition = new Vector3Int(victim.xPos, victim.yPos, -1);
        Rotate(lookPosition);
        doingAction = true;
        StartCoroutine(DamageDelay(1f, attacker, victim));
        //Debug.Log("!SLAASHH! " + attacker.name + " just attacked " + victim.name);
        StartCoroutine(AttackDelay(2f));
    }

    public void Die(Unit victim)
    {
        if (victim.GetComponent<Player>() != null)
        {
            Debug.Log("Player died");
            return;
        }
        victim.unitState = UnitStates.EndTurn;
        gameManager.RemoveEnemyFromLists(victim.GetComponent<Enemy>());
        Destroy(victim.gameObject);
    }

    private IEnumerator AttackDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Debug.Log("AttackDelay");
        completedAction = true;
        doingAction = false;
        anim.SetBool("Attack", false);
    }

    private IEnumerator DamageDelay(float seconds, Unit attacker, Unit victim)
    {
        yield return new WaitForSeconds(seconds);
        if (victim.currentHealth - attacker.damage <= 0)
        {
            victim.currentHealth = 0;
            Die(victim);
        }
        else
            victim.currentHealth -= attacker.damage;
    }

    #endregion

    #region UnitStates

    public enum UnitStates {Waiting, StartTurn, Action, EndTurn};

    protected virtual void EndTurn()
    {
        Debug.Log("ENDTURN " + gameObject.name);
        completedAction = false;
        unitState = UnitStates.Waiting;
    }

    private void ActOnState()
    {
        switch (unitState)
        {
            case UnitStates.Waiting:
                targetPosition = new Vector3Int(xPos, yPos, -1);
                completedAction = false;
                outliner.enabled = false;
                break;
            case UnitStates.StartTurn:
                outliner.enabled = true;
                targetPosition = new Vector3Int(xPos, yPos, -1);
                unitState = UnitStates.Action;
                break;
            case UnitStates.Action:
                StartCoroutine(Move());
                if (completedAction)
                {
                    Debug.Log("completedAction" + gameObject.name);
                    unitState = UnitStates.EndTurn;
                }
                break;
            case UnitStates.EndTurn:
                EndTurn();
                //Debug.Log("endturn" + gameObject.name);
                outliner.enabled = false;
                break;
            default:
                break;
        }    
    }

    #endregion

    #region UnitGetters

    public int GetHealth()
    {
        return currentHealth;
    }

    #endregion
}
