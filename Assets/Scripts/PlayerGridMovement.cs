// Written by Joy de Ruijter
using System.Collections.Generic;
using UnityEngine;

public class PlayerGridMovement : MonoBehaviour
{
    #region Variables

    [SerializeField] private int maxSteps;
    [SerializeField] private Transform[] wayPoints;
    [HideInInspector] public int currentWayPoint = 0;
    private Rigidbody rb;
    [SerializeField] private float moveSpeed = 5;
    private float xOffset = -0.5f;
    private float zOffset = 0.5f;
    [HideInInspector] public bool stoppedMoving;
    private Vector3 wayPointPosition = Vector3.zero;

    #endregion

    private void Awake()
    {
        wayPoints = new Transform[maxSteps];
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        if (wayPoints[0] == null)
            return;

        wayPointPosition.Set((wayPoints[currentWayPoint].position.x + xOffset), wayPoints[currentWayPoint].position.y, (wayPoints[currentWayPoint].position.z + zOffset));

        if (Vector3.Distance(transform.position, wayPointPosition) < 0.04)
        {
            if (wayPoints[currentWayPoint + 1] == null)
            {
                stoppedMoving = true;
                return;
            }
            else
                stoppedMoving = false;

            currentWayPoint += 1;
            currentWayPoint = currentWayPoint%wayPoints.Length;
        }

        Vector3 dir = (wayPointPosition - transform.position).normalized;
        rb.MovePosition(transform.position + dir * moveSpeed * Time.deltaTime);
    }

    public void SetWayPoints(List<Transform> pathTransforms)
    {
        int indexCounter = 0;

        if (pathTransforms.Count > maxSteps)
        { 
            Debug.Log("Waypoint can't be reached with current maxSteps");
            return;
        }

        foreach (Transform t in pathTransforms)
        {
            wayPoints[indexCounter] = t;
            indexCounter++;
        }
    }

    public void EmptyWayPoints()
    {
        for (int i = 0; i < wayPoints.Length; i++)
        {
            if (wayPoints[i] != null)
                wayPoints[i] = null;
            else
                break;
        }
    }
}
