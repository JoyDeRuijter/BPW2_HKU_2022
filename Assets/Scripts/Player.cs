// Written by Joy de Ruijter
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Variables

    [SerializeField] private int maxSteps;
    [SerializeField] private Transform[] wayPoints;
    private int currentWayPoint = 0;
    private Rigidbody rb;
    [SerializeField] private float moveSpeed = 5;
    private float xOffset = -0.5f;
    private float zOffset = 0.5f;
    [HideInInspector] public bool stoppedMoving;

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
        bool arrayIsEmpty = true;
        for (int i = 0; i < wayPoints.Length; i++)
        {
            if(wayPoints[i] != null)
                arrayIsEmpty = false;
            break;
        }
        if (arrayIsEmpty)
            return;

        Vector3 wayPointPosition = new Vector3((wayPoints[currentWayPoint].position.x + xOffset), wayPoints[currentWayPoint].position.y, (wayPoints[currentWayPoint].position.z + zOffset));

        if (!arrayIsEmpty && Vector3.Distance(transform.position, wayPointPosition) < 0.025)
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

        for (int i = 0; i < wayPoints.Length; i++)
        {
            if (wayPoints[i] != null)
                wayPoints[i] = null;
        }

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
}
