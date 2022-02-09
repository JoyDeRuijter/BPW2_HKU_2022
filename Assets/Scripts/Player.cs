// Written by Joy de Ruijter
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Variables

    [SerializeField] private Transform[] wayPoints;
    private int currentWayPoint = 0;
    private Rigidbody rb;
    [SerializeField] private float moveSpeed = 5;

    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        
    }

    private void Movement()
    {
        if (Vector3.Distance(transform.position, wayPoints[currentWayPoint].position) < 0.25f)
        {
            currentWayPoint += 1;
            currentWayPoint = currentWayPoint%wayPoints.Length;
        }
        Vector3 dir = (wayPoints[currentWayPoint].position - transform.position).normalized;
    }
}
