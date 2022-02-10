// Written by Joy de Ruijter
using System.Collections.Generic;
using UnityEngine;

public class GridBehaviour : MonoBehaviour
{
    #region Variables

    public bool findDistance = false;
    public int rows;
    public int columns;
    public int scale = 1;
    public GameObject gridPrefab;
    public Vector3 leftBottomLocation = Vector3.zero;
    public GameObject[,] gridArray;
    public int startX;
    public int startY;
    public int endX = 2;
    public int endY = 2;
    public List<GameObject> path = new List<GameObject>();
    
    private List<Transform> pathTransforms = new List<Transform>();
    //private PlayerManager playerManager;
    [SerializeField] private PlayerGridMovement player;
    [SerializeField] private Camera camera;
    private GameObject previousClickedTile;
    private GameObject currentClickedTile;

    #endregion

    private void Awake()
    {
        //playerManager = PlayerManager.instance;
        //player = playerManager.player.GetComponent<PlayerGridMovement>();

        gridArray = new GameObject[rows, columns];

        startX = (int)(player.transform.position.x + 1);
        startY = (int)player.transform.position.z;


        if (gridPrefab)
            GenerateGrid();
        else
            Debug.Log("Missing gridprefab");

        previousClickedTile = null;
        currentClickedTile = null;
    }

    private void Update()
    {
        if(endX > rows - 1)
            endX = Mathf.Clamp(endX, 0, rows - 1);

        if(endY > columns - 1)
            endY = Mathf.Clamp(endY, 0, columns - 1);

        if (player.stoppedMoving)
        {
            DimAllPaths();
            player.EmptyWayPoints();
            player.currentWayPoint = 0;
            startX = endX;
            startY = endY;
            path.Clear();
            player.stoppedMoving = false;
        }

        OnClick();

        if (findDistance)
        { 
            SetDistance();
            SetPath();
            LightUpPath();
            SetPathTransforms();
            player.SetWayPoints(pathTransforms);
            findDistance = false;
        }
    }

    private void GenerateGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject obj = Instantiate(gridPrefab, new Vector3(leftBottomLocation.x + scale * i, leftBottomLocation.y, leftBottomLocation.z + scale * j), Quaternion.identity);
                obj.transform.SetParent(gameObject.transform);
                obj.GetComponent<GridStat>().x = i;
                obj.GetComponent<GridStat>().y = j;
                gridArray[i, j] = obj;
            }
        }
    }

    private void SetDistance()
    { 
        InitialSetup();
        for (int step = 1; step < rows * columns; step++)
        {
            foreach (GameObject obj in gridArray)
            {
                if (obj && obj.GetComponent<GridStat>().visited == step - 1)
                    TestFourDirections(obj.GetComponent<GridStat>().x, obj.GetComponent<GridStat>().y, step);
            }
        }                
    }

    private void SetPath()
    {
        int step;
        int x = endX; 
        int y = endY;
        Debug.Log(x + " - " + y);
        List<GameObject> tempList = new List<GameObject>();
        path.Clear();
        if (gridArray[endX, endY] && gridArray[endX, endY].GetComponent<GridStat>().visited > 0)
        {
            path.Add(gridArray[x, y]);
            step = gridArray[x, y].GetComponent<GridStat>().visited - 1;
        }
        else
        {
            Debug.Log("Can't reach the desired location");
            return;
        }

        for (int i = step; step > -1; step--)
        {
            if (TestDirection(x, y, step, 1))
                tempList.Add(gridArray[x, y + 1]);
            if (TestDirection(x, y, step, 2))
                tempList.Add(gridArray[x + 1, y]);
            if (TestDirection(x, y, step, 3))
                tempList.Add(gridArray[x, y - 1]);
            if (TestDirection(x, y, step, 4))
                tempList.Add(gridArray[x - 1, y]);

            GameObject tempObj = FindClosest(gridArray[endX, endY].transform, tempList);
            path.Add(tempObj);
            x = tempObj.GetComponent<GridStat>().x;
            y = tempObj.GetComponent<GridStat>().y;
            tempList.Clear();
        }
    }

    private void InitialSetup()
    {
        foreach (GameObject obj in gridArray)
        {
            obj.GetComponent<GridStat>().visited = -1;
        }
        gridArray[startX, startY].GetComponent<GridStat>().visited = 0;
    }

    private bool TestDirection(int x, int y, int step, int direction)
    {
        //1 = up, 2 = right, 3 = down, 4 = left
        switch (direction)
        {
            case 1:
                if (y + 1 < columns && gridArray[x, y + 1] && gridArray[x, y + 1].GetComponent<GridStat>().visited == step)
                    return true;
                else 
                    return false;
            case 2:
                if (x + 1 < rows && gridArray[x + 1, y] && gridArray[x + 1, y].GetComponent<GridStat>().visited == step)
                    return true;
                else
                    return false;
            case 3:
                if (y - 1 > -1 && gridArray[x, y - 1] && gridArray[x, y - 1].GetComponent<GridStat>().visited == step)
                    return true;
                else
                    return false;
            case 4:
                return (x - 1 > -1 && gridArray[x - 1, y] && gridArray[x - 1, y].GetComponent<GridStat>().visited == step);
        }
        return false;
    }

    private void TestFourDirections(int x, int y, int step)
    {
        if (TestDirection(x, y, -1, 1))
            SetVisited(x, y + 1, step);
        if (TestDirection(x, y, -1, 2))
            SetVisited(x + 1, y, step);
        if (TestDirection(x, y, -1, 3))
            SetVisited(x, y - 1, step);
        if (TestDirection(x, y, -1, 4))
            SetVisited(x - 1, y, step);
    }

    private void SetVisited(int x, int y, int step)
    {
        if (gridArray[x, y])
            gridArray[x, y].GetComponent<GridStat>().visited = step;
    }

    private GameObject FindClosest(Transform targetLocation, List<GameObject> list)
    {
        float currentDistance = scale * rows * columns;
        int indexNumber = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (Vector3.Distance(targetLocation.position, list[i].transform.position) < currentDistance)
            {
                currentDistance = Vector3.Distance(targetLocation.position, list[i].transform.position);
                indexNumber = i;
            }
        }
        return list[indexNumber];
    }

    private void SetPathTransforms()
    {
        pathTransforms.Clear();

        foreach (GameObject obj in path)
            pathTransforms.Add(obj.transform);

        pathTransforms.Reverse();
    }

    private void LightUpPath()
    {
        foreach (GameObject obj in path)
        {
            if (obj.GetComponent<GridStat>().visited > 0 && obj != currentClickedTile)
                obj.GetComponent<GridStat>().SetPathMaterial();
        }
        gridArray[startX, startY].GetComponent<GridStat>().SetPathMaterial();
    }

    private void DimAllPaths()
    {
        foreach (GameObject obj in path)
        {
            if (obj.GetComponent<GridStat>().visited > 0)
                obj.GetComponent<GridStat>().SetDefaultMaterial();
        }
        gridArray[startX, startY].GetComponent<GridStat>().SetDefaultMaterial();
    }

    private void OnClick()
    {
        if (Input.GetMouseButtonDown(0))
        { 
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            { 
                if (hit.transform.gameObject.GetComponent<GridStat>() != null)
                {
                    if (currentClickedTile != null)
                    {
                        previousClickedTile = currentClickedTile;
                        GridStat previousClickedTileStat = previousClickedTile.GetComponent<GridStat>();
                        previousClickedTileStat.isTargetTile = false;
                        previousClickedTileStat.SetDefaultMaterial();
                    }     
                    currentClickedTile = hit.transform.gameObject;
                    GridStat currentClickedTileStat = currentClickedTile.GetComponent<GridStat>();
                    currentClickedTileStat.isTargetTile = true;  
                    currentClickedTileStat.SetTargetMaterial();

                    endX = currentClickedTileStat.x;
                    endY = currentClickedTileStat.y;
                    findDistance = true;
                }
            }
        }
    }
}
