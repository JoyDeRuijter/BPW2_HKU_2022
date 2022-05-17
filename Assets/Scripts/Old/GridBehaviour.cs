// Written by Joy de Ruijter
using System.Collections.Generic;
using UnityEngine;

public class GridBehaviour : MonoBehaviour
{
    #region Variables

    [Header("Grid Properties")]
    [SerializeField] private int rows;
    [SerializeField] private int columns;
    [SerializeField] private int scale = 1;
    [SerializeField] private GameObject gridTile;
    [SerializeField] private Vector3 leftBottomLocation = Vector3.zero;

    [Header("References")]
    [Space(10)]
    [SerializeField] private PlayerGridMovement playerGridMovement;
    [SerializeField] private new Camera camera;
    
    private int startX, startY, endX = 2, endY = 2;
    private bool findDistance;

    private GameObject previousClickedTile;
    private GameObject currentClickedTile;
    
    private GameObject[,] gridArray;

    private List<GameObject> path = new List<GameObject>();
    private List<Transform> pathTransforms = new List<Transform>();

    #endregion

    private void Awake()
    {
        gridArray = new GameObject[rows, columns];

        startX = (int)(playerGridMovement.transform.position.x + 1);
        startY = (int)(playerGridMovement.transform.position.z);

        if (gridTile)
            GenerateGrid();
        else
            Debug.Log("Missing gridTile prefab");

        previousClickedTile = null;
        currentClickedTile = null;
    }

    private void Update()
    {
        // Clamp the end destination so this can never be a position outside the grid
        if(endX > rows - 1)
            endX = Mathf.Clamp(endX, 0, rows - 1);
        if(endY > columns - 1)
            endY = Mathf.Clamp(endY, 0, columns - 1);

        // Reset the grid, tiles and path when the player has stopped moving
        if (playerGridMovement.stoppedMoving)
            ResetAll();

        // If there was a click on a tile, set everything in motion with findDistance, if not, findDistance remains false
        findDistance = OnClick();

        // Set everything in motion to prepare the grid for player movement
        if (findDistance)
        { 
            SetDistance();
            SetPath();
            LightUpPath();
            SetPathTransforms();
            playerGridMovement.SetWayPoints(pathTransforms);
            findDistance = false;
        }
    }

    // Generates the grid
    private void GenerateGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject obj = Instantiate(gridTile, new Vector3(leftBottomLocation.x + scale * i, leftBottomLocation.y, leftBottomLocation.z + scale * j), Quaternion.identity);
                obj.transform.SetParent(gameObject.transform);
                obj.GetComponent<GridStat>().x = i;
                obj.GetComponent<GridStat>().y = j;
                gridArray[i, j] = obj;
            }
        }
    }

    // Initializes the tiles in the grid
    private void InitialSetup()
    {
        foreach (GameObject obj in gridArray)
        {
            obj.GetComponent<GridStat>().visited = -1;
        }
        gridArray[startX, startY].GetComponent<GridStat>().visited = 0;
    }

    // Resets the grid, tiles, waypoints, path, recalculates the starting positions and reanables the player to move again
    private void ResetAll()
    {
        DimAllPaths();
        playerGridMovement.EmptyWayPoints();
        playerGridMovement.currentWayPoint = 0;
        startX = endX;
        startY = endY;
        path.Clear();
        playerGridMovement.stoppedMoving = false;
    }

    // Re-initializes the tiles in the grid and determines the distance
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

    // Set the path by refilling the path list with tiles the player has to walk on in order to get the shortest route to the target position
    private void SetPath()
    {
        int step;
        int x = endX; 
        int y = endY;
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

        for (; step > -1; step--)
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

    // Test the given direction
    private bool TestDirection(int x, int y, int step, int direction)
    {
        switch (direction) // 1 = up, 2 = right, 3 = down, 4 = left
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

    // Test all four possible directions
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

    // Set the tile as visited with the step number
    private void SetVisited(int x, int y, int step)
    {
        if (gridArray[x, y])
            gridArray[x, y].GetComponent<GridStat>().visited = step;
    }

    // Return the tile gameobject that is closest to the targetlocation, from a list of all tiles
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

    // Fill the list pathTransforms with the transforms of all the tile objects in path (and reverse it, otherwise it's in the wrong order)
    private void SetPathTransforms()
    {
        pathTransforms.Clear();

        foreach (GameObject obj in path)
            pathTransforms.Add(obj.transform);

        pathTransforms.Reverse();
    }

    // Change the material of all tiles that are in the path and the start tile, excluding the clicked tile
    private void LightUpPath()
    {
        foreach (GameObject obj in path)
        {
            if (obj.GetComponent<GridStat>().visited > 0 && obj != currentClickedTile)
                obj.GetComponent<GridStat>().SetPathMaterial();
        }
        gridArray[startX, startY].GetComponent<GridStat>().SetPathMaterial();
    }

    // Change the material of all tiles that are in the path and the start tile back to the default material
    private void DimAllPaths()
    {
        foreach (GameObject obj in path)
        {
            if (obj.GetComponent<GridStat>().visited > 0)
                obj.GetComponent<GridStat>().SetDefaultMaterial();
        }
        gridArray[startX, startY].GetComponent<GridStat>().SetDefaultMaterial();
    }

    // Shoot a raycast when there has been a mouse click, if it hits a tile, set it as new currentClickedTile, change the end position accordingly
    private bool OnClick()
    {
        if (path.Count != 0)
            return false;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
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

                    return true;
                }
            }
        }
        return false;
    }
}
