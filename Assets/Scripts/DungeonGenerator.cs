// Written by Valentijn Muijrers
// Edited and added on by Joy de Ruijter
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dungeon
{
    public enum TileType { Floor, Wall, Corridor };

    public class DungeonGenerator : MonoBehaviour
    {
        #region Variables

        [Header("Tile Prefabs")]
        [SerializeField] private Tile floorPrefab;
        [SerializeField] private Tile wallPrefab;

        [Header("Dungeon Settings")]
        [Space(10)]
        [SerializeField] private int gridWidth;
        [SerializeField] private int gridHeight;
        [SerializeField] private int minRoomSize;
        [SerializeField] private int maxRoomSize;
        [SerializeField] private int numberOfRooms;
        [SerializeField] private int numberOfEnemies;

        [Header("References")]
        [Space(10)]
        [SerializeField] Transform cam;
        [SerializeField] GameObject playerPrefab;
        [SerializeField] GameObject guardPrefab;
        [SerializeField] GameObject[] enemyPrefabs;
        [SerializeField] GameObject[] potionPrefabs;

        public Dictionary<Vector3Int, TileType> dungeon = new Dictionary<Vector3Int, TileType>();
        public List<Room> rooms = new List<Room>();
        [HideInInspector] public GameObject player;

        #endregion

        private void Awake()
        {
            Generate();
            AllocateCamera();
            AllocatePlayer();
            AllocateEnemies();
            AllocatePotions();
            AllocateGuard();
        }

        #region Dungeon

        public void Generate()
        {
            for (int i = 0; i < numberOfRooms; i++)
            {
                int minX = Random.Range(0, gridWidth);
                int maxX = minX + Random.Range(minRoomSize, maxRoomSize + 1);
                int minY = Random.Range(0, gridHeight);
                int maxY = minY + Random.Range(minRoomSize, maxRoomSize + 1);

                Room room = new Room(minX, maxX, minY, maxY);
                room.ID = i;

                if (RoomFitsInDungeon(room))
                    AddRoomToDungeon(room);
                else
                    i--;
            }

            for (int i = 0; i < rooms.Count; i++)
            {
                Room room = rooms[i];
                Room otherRoom = rooms[(i + Random.Range(1, rooms.Count)) % rooms.Count];
                ConnectRooms(room, otherRoom);
            }

            AllocateWalls();
            SpawnDungeon();
        }

        public void AddRoomToDungeon(Room room)
        {
            for (int x = room.minX; x <= room.maxX; x++)
            {
                for (int y = room.minY; y <= room.maxY; y++)
                {
                    dungeon.Add(new Vector3Int(x, y, 0), TileType.Floor);
                }
            }
            rooms.Add(room);
        }

        public bool RoomFitsInDungeon(Room room)
        {
            for (int x = room.minX - 1; x <= room.maxX + 1; x++)
            {
                for (int y = room.minY - 1; y <= room.maxY + 1; y++)
                {
                    if (dungeon.ContainsKey(new Vector3Int(x, y, 0)))
                        return false;
                }
            }
            return true;
        }

        public void AllocateWalls()
        {
            var keys = dungeon.Keys.ToList();
            foreach (var kvp in keys)
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Vector3Int newPos = kvp + new Vector3Int(x, y, 0);

                        if (dungeon.ContainsKey(newPos))
                            continue;
                        dungeon.Add(newPos, TileType.Wall);
                    }
                }
            }
        }

        public void ConnectRooms(Room _roomOne, Room _roomTwo)
        {
            Vector3Int posOne = _roomOne.GetCenter();
            Vector3Int posTwo = _roomTwo.GetCenter();

            int dirX = posTwo.x > posOne.x ? 1 : -1;
            int x;
            for (x = posOne.x; x != posTwo.x; x += dirX)
            {
                Vector3Int position = new Vector3Int(x, posOne.y, 0);

                if (dungeon.ContainsKey(position))
                    continue;

                dungeon.Add(position, TileType.Corridor);
            }

            int dirY = posTwo.y > posOne.y ? 1 : -1;
            for (int y = posOne.y; y != posTwo.y; y += dirY)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                if (dungeon.ContainsKey(position))
                    continue;

                dungeon.Add(position, TileType.Corridor);
            }
        }

        public void SpawnDungeon()
        {
            foreach (KeyValuePair<Vector3Int, TileType> kvp in dungeon)
            {
                switch (kvp.Value)
                {
                    case TileType.Floor:
                        var spawnedFloortile = Instantiate(floorPrefab, kvp.Key, Quaternion.identity, transform);
                        spawnedFloortile.name = "FloorTile_" + kvp.Key.x + "_" + kvp.Key.y;
                        var isOffset = (kvp.Key.x % 2 == 0 && kvp.Key.y % 2 != 0) || (kvp.Key.x % 2 != 0 && kvp.Key.y % 2 == 0);
                        if ((kvp.Key.x >= rooms[rooms.Count - 1].minX && kvp.Key.x <= rooms[rooms.Count - 1].maxX) &&
                            (kvp.Key.y >= rooms[rooms.Count - 1].minY && kvp.Key.y <= rooms[rooms.Count - 1].maxY))
                            spawnedFloortile.InitializeEndRoom(isOffset);
                        else
                            spawnedFloortile.Initialize(isOffset);
                        break;

                    case TileType.Wall:
                        Vector3Int wallPosition = new Vector3Int(kvp.Key.x, kvp.Key.y, 0);
                        var spawnedWalltile = Instantiate(wallPrefab, wallPosition, Quaternion.identity, transform);
                        spawnedWalltile.name = "WallTile_" + kvp.Key.x + "_" + kvp.Key.y;
                        spawnedWalltile.Initialize(false);
                        break;

                    case TileType.Corridor:
                        var spawnedCorridortile = Instantiate(floorPrefab, kvp.Key, Quaternion.identity, transform);
                        spawnedCorridortile.name = "CorridorTile_" + kvp.Key.x + "_" + kvp.Key.y;
                        var isOffset2 = (kvp.Key.x % 2 == 0 && kvp.Key.y % 2 != 0) || (kvp.Key.x % 2 != 0 && kvp.Key.y % 2 == 0);
                        spawnedCorridortile.Initialize(isOffset2);
                        break;
                }
            }
        }

        #endregion

        #region Allocation Other Entities

        private void AllocateCamera()
        {
            Vector3Int startPos = rooms[0].GetCenter();
            cam.transform.position = new Vector3((float)startPos.x, (float)startPos.y, cam.transform.position.z);
        }

        private void AllocatePlayer()
        {
            Vector3Int startPos = rooms[0].GetCenter();
            player = Instantiate(playerPrefab, new Vector3((float)startPos.x, (float)startPos.y, -1), Quaternion.identity);
            player.GetComponent<Player>().name = "Player";
            player.GetComponent<Player>().roomID = rooms[0].ID;
            rooms[0].occupiedTiles.Add(startPos);
        }

        private void AllocateEnemies()
        {
            for (int i = 0; i < rooms.Count - 1; i++)
            {
                int _numberOfEnemies = Random.Range(numberOfEnemies - 1, numberOfEnemies + 1);
                for (int j = 0; j < _numberOfEnemies; j++)
                {
                    Vector3Int pendingPosition = rooms[i].GetRandomTile();

                    if (!RoomTileIsOccupied(rooms[i], pendingPosition))
                    {
                        Vector3Int spawnPosition = new Vector3Int(pendingPosition.x, pendingPosition.y, -1);
                        GameObject newEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], spawnPosition, Quaternion.identity);
                        newEnemy.GetComponent<Enemy>().enemyType = EnemyType.Normal;
                        newEnemy.GetComponent<Enemy>().name = newEnemy.GetComponent<Enemy>().enemyType.ToString() + "_" + j + "_" + i;
                        newEnemy.GetComponent<Enemy>().roomID = rooms[i].ID;
                        rooms[i].occupiedTiles.Add(pendingPosition);
                    }
                }
            }       
        }

        private void AllocatePotions()
        {
            for (int i = 0; i < rooms.Count - 1; i++)
            {
                int _numberOfPotions = Random.Range(0, 3);
                for (int j = 0; j < _numberOfPotions; j++)
                {
                    Vector3Int pendingPosition = rooms[i].GetRandomTile();
                    if (!RoomTileIsOccupied(rooms[i], pendingPosition))
                    {
                        Vector3Int spawnPosition = new Vector3Int(pendingPosition.x, pendingPosition.y, -1);
                        GameObject newPotion = Instantiate(potionPrefabs[Random.Range(0, potionPrefabs.Length)], spawnPosition, Quaternion.identity);
                        newPotion.GetComponent<Potion>().roomID = rooms[i].ID;
                        newPotion.GetComponent<Potion>().xPos = spawnPosition.x;
                        newPotion.GetComponent<Potion>().yPos = spawnPosition.y;
                        rooms[i].occupiedTiles.Add(pendingPosition);
                        rooms[i].potionPositions.Add(spawnPosition);
                    }
                }
            }
        }

        private void AllocateGuard()
        {
            Vector3Int startPos = rooms[rooms.Count - 1].GetCenter();
            GameObject guard = Instantiate(guardPrefab, new Vector3((float)startPos.x, (float)startPos.y, -1), Quaternion.identity);
            guard.GetComponent<Guard>().roomID = rooms[rooms.Count - 1].ID;
            guard.GetComponent<Guard>().xPos = startPos.x;
            guard.GetComponent<Guard>().yPos = startPos.y;
            rooms[rooms.Count - 1].occupiedTiles.Add(startPos); ;
        }

        #endregion

        #region Helper Functions

        private bool RoomTileIsOccupied(Room room, Vector3Int pendingPosition)
        {
            if (room.occupiedTiles.Count == 0)
                return false;

            for (int i = 0; i < room.occupiedTiles.Count; i++)
            {
                if (room.occupiedTiles[i] == pendingPosition)
                    return true;
            }
            return false;
        }

        public int UnitRoomID(Unit unit)
        {
           Vector3Int unitPosition = new Vector3Int(unit.xPos, unit.yPos, 0);
            if (dungeon.ContainsKey(unitPosition) && dungeon[unitPosition] == TileType.Floor)
            {
                for (int i = 0; i < rooms.Count; i++)
                {
                    if (rooms[i].HasThisTile(unit.xPos, unit.yPos))
                        return rooms[i].ID;
                }
                Debug.Log("No rooms where found with a tile on the position of " + unit.name);
            }
            return -1;
        }

        #endregion
    }

    public class Room
    {
        #region Variables

        public int ID;
        public int minX, maxX, minY, maxY;
        public List<Vector3Int> occupiedTiles = new List<Vector3Int>();
        public List<Vector3Int> potionPositions = new List<Vector3Int>();

        public Room(int _minX, int _maxX, int _minY, int _maxY)
        {
            minX = _minX;
            maxX = _maxX;
            minY = _minY;
            maxY = _maxY;
        }

        #endregion

        #region Get Tile Functions

        public Vector3Int GetCenter()
        {
            return new Vector3Int(Mathf.RoundToInt(Mathf.Lerp(minX, maxX, 0.5f)), Mathf.RoundToInt(Mathf.Lerp(minY, maxY, 0.5f)), 0);
        }

        public Vector3Int GetRandomTile()
        {
            return new Vector3Int(Mathf.RoundToInt(Random.Range(minX, maxX + 1)), Mathf.RoundToInt(Random.Range(minY, maxY + 1)), 0);
        }

        #endregion

        #region Other Functions

        public bool HasThisTile(int xPos, int yPos)
        {
            if ((xPos >= minX && xPos <= maxX) && (yPos >= minY && yPos <= maxY))
                return true;
            return false;
        }

        public bool HasAPotion(int xPos, int yPos)
        {
            if (potionPositions.Count == 0)
                return false;

            for (int i = 0; i < potionPositions.Count; i++)
            {
                if (potionPositions[i].x == xPos && potionPositions[i].y == yPos)
                    return true;
            }
            return false;
        }

        public void DeleteAPotion(int xPos, int yPos)
        {
            if (potionPositions.Count == 0)
                return;

            for (int i = 0; i < potionPositions.Count; i++)
            {
                if (potionPositions[i].x == xPos && potionPositions[i].y == yPos)
                    potionPositions.Remove(potionPositions[i]);
            }
        }

        #endregion
    }
}


