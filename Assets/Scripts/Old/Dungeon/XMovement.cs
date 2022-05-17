// Written by Joy de Ruijter
using UnityEngine;

public class Movement : MonoBehaviour
{
    #region Variables

    [Header("References")]
    public GameObject gameManager;

    [Header("Properties")]
    [Space(10)]
    [SerializeField] private int maxSteps = 3;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float turnSpeed = 6;

    [HideInInspector] public int currentWayPoint = 0;
    [HideInInspector] public bool stoppedMoving;


    private GameObject previousClickedTile;
    private GameObject currentClickedTile;

    private int startX, startY, endX = 2, endY = 2;
    private bool findDistance;

    private Vector3Int wayPointPosition = Vector3Int.zero;
    private Rigidbody rb;
    private Animator anim;
    private Transform[] wayPoints;
    private Grid grid;
    private new Camera camera;

    #endregion

    private void Awake()
    {
        grid = gameManager.GetComponent<Grid>();
        camera = gameManager.GetComponent<CameraManager>().camera;
        wayPoints = new Transform[maxSteps];
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        OnClick();
    }

    private void OnClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.gameObject.GetComponentInParent<GridTile>() != null)
                {
                    Debug.Log("Clicked a tile");
                    if (currentClickedTile != null)
                    { 
                        previousClickedTile = currentClickedTile;
                        GridTile previousGridTile = previousClickedTile.GetComponentInChildren<GridTile>();
                        previousGridTile.isTargetTile = false;
                        previousGridTile.SetDefaultMaterial();
                    }
                    currentClickedTile = hit.transform.parent.gameObject;
                    GridTile currentGridTile = currentClickedTile.GetComponentInChildren<GridTile>();
                    currentGridTile.isTargetTile = true;
                    currentGridTile.SetTargetMaterial();

                    endX = currentGridTile.x;
                    endY = currentGridTile.y;
                }
            }
        }
    }
}
