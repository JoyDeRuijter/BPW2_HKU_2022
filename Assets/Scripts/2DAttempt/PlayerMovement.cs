// Written by Joy de Ruijter
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [Header ("Properties")]
    [SerializeField] private float movementSpeed = 150f;
    [HideInInspector] public Vector3Int targetPosition;
    [HideInInspector] public bool isMoving;
    private Player player;

    #endregion

    private void Start()
    {
        player = gameObject.GetComponent<Player>();
    }

    private void Update()
    {
        Move();
        StartCoroutine(CheckForMovement());
    }

    public void Move()
    {
        if (transform.position != targetPosition && targetPosition != Vector3Int.zero)
        {
            float step = movementSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * step);
            player.xPos = (int)transform.position.x;
            player.yPos = (int)transform.position.y;
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
}
