// Written by Joy de Ruijter
using System.Collections.Generic;
using UnityEngine;

public class PlayerGridMovement : MonoBehaviour
{
    #region Variables

    [Header ("Player Properties")]
    [SerializeField] private int maxSteps = 100;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float turnSpeed = 6;
    
    [HideInInspector] public int currentWayPoint = 0;
    [HideInInspector] public bool stoppedMoving;
    
    private float xOffset = -0.5f, zOffset = 0.5f;

    private Vector3 wayPointPosition = Vector3.zero;

    private Rigidbody rb;
    private Animator anim;

    private Transform[] wayPoints;
    
    #endregion

    private void Awake()
    {
        wayPoints = new Transform[maxSteps];
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Movement();
        TestAnimations();
    }

    // Move the rigidbody of the player in the direction of the waypoint
    private void Movement()
    {
        if (wayPoints[0] == null)
            return;

        wayPointPosition = WayPointPosition(wayPoints);

        if (Vector3.Distance(transform.position, wayPointPosition) < 0.04)
        {
            stoppedMoving = StoppedMoving();
            if (stoppedMoving)
            {
                anim.SetBool("IsWalking", false);
                return;
            }  
            else
                anim.SetBool("IsWalking", true);

            currentWayPoint += 1;
            currentWayPoint %= wayPoints.Length;
        }

        Vector3 dir = (wayPointPosition - transform.position).normalized;
        RotatePlayer();
        rb.MovePosition(transform.position + dir * moveSpeed * Time.deltaTime);
    }

    private void RotatePlayer()
    {
        Vector3 lookDirection = wayPointPosition - transform.position;
        lookDirection.y = 0.0001f;
        Quaternion rotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
    }

    // Set the correct waypoint position for the current waypoint using the player offsets, so they are in the middle of the tile
    private Vector3 WayPointPosition(Transform[] wayPoints)
    {
        wayPointPosition.Set((wayPoints[currentWayPoint].position.x + xOffset), wayPoints[currentWayPoint].position.y, (wayPoints[currentWayPoint].position.z + zOffset));
        return wayPointPosition;
    }

    // Check if the player stopped moving if there are no future waypoints
    private bool StoppedMoving()
    {
        return (wayPoints[currentWayPoint + 1] == null);
    }

    // HAS A TODO INSIDE!
    // Get all the transforms from pathTransforms list into the waypoints array
    public void SetWayPoints(List<Transform> pathTransforms)
    {
        // TODO: fix the bug that freezes the game after this event, should just not create a path at all and notify the player in the UI
        // Or make a hover over GUI thing that shows the player what steps he can and can not make, maybe even make the unreachable tiles unclickable as well
        // to terminate this problem for good.
        if (pathTransforms.Count > maxSteps)
        { 
            Debug.Log("Waypoint can't be reached with current maxSteps"); 
            return;
        }

        int indexCounter = 0;
        foreach (Transform t in pathTransforms)
        {
            wayPoints[indexCounter] = t;
            indexCounter++;
        }
    }

    // Empty the array wayPoints
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

    // Test method for animations, place this in another player script later
    private void TestAnimations()
    {
        if (Input.GetKeyDown(KeyCode.A))
            anim.SetBool("Attack", true);
        if (Input.GetKeyUp(KeyCode.A))
            anim.SetBool("Attack", false);
    }
}
