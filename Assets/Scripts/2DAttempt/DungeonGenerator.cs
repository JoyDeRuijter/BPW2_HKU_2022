// Written by Joy de Ruijter
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dungeon
{
    public enum TileType { Floor, Wall };

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

        [Header("References")]
        [Space(10)]
        [SerializeField] Transform cam;
        [SerializeField] GameObject playerPrefab;

        public Dictionary<Vector3Int, TileType> dungeon = new Dictionary<Vector3Int, TileType>();
        public List<Room> rooms = new List<Room>();

        [HideInInspector] public GameObject player;

        #endregion

        private void Awake()
        {
            Generate();
            PlaceCamera();
            PlacePlayer();
        }

        public void Generate()
        {
            // Doors?

            for (int i = 0; i < numberOfRooms; i++)
            {
                int minX = Random.Range(0, gridWidth);
                int maxX = minX + Random.Range(minRoomSize, maxRoomSize + 1);
                int minY = Random.Range(0, gridHeight);
                int maxY = minY + Random.Range(minRoomSize, maxRoomSize + 1);

                Room room = new Room(minX, maxX, minY, maxY);

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
            int x = 0;
            for (x = posOne.x; x != posTwo.x; x += dirX)
            {
                Vector3Int position = new Vector3Int(x, posOne.y, 0);

                if (dungeon.ContainsKey(position))
                    continue;

                dungeon.Add(position, TileType.Floor);
            }

            int dirY = posTwo.y > posOne.y ? 1 : -1;
            for (int y = posOne.y; y != posTwo.y; y += dirY)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                if (dungeon.ContainsKey(position))
                    continue;

                dungeon.Add(position, TileType.Floor);
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
                        spawnedFloortile.Initialize(isOffset);
                        break;

                    case TileType.Wall:
                        Vector3Int wallPosition = new Vector3Int(kvp.Key.x, kvp.Key.y, 0);
                        var spawnedWalltile = Instantiate(wallPrefab, wallPosition, Quaternion.identity, transform);
                        spawnedWalltile.name = "FloorTile_" + kvp.Key.x + "_" + kvp.Key.y;
                        spawnedWalltile.Initialize(false);
                        break;
                }
            }
        }

        private void PlaceCamera()
        {
            Vector3Int startPos = rooms[0].GetCenter();
            cam.transform.position = new Vector3((float)startPos.x, (float)startPos.y, cam.transform.position.z);
        }

        private void PlacePlayer()
        {
            Vector3Int startPos = rooms[0].GetCenter();
            player = Instantiate(playerPrefab, new Vector3((float)startPos.x, (float)startPos.y, -1), Quaternion.identity);
        }

    }

    public class Room
    {
        #region Variables

        public int minX, maxX, minY, maxY;

        public Room(int _minX, int _maxX, int _minY, int _maxY)
        {
            minX = _minX;
            maxX = _maxX;
            minY = _minY;
            maxY = _maxY;
        }

        #endregion

        public Vector3Int GetCenter()
        {
            return new Vector3Int(Mathf.RoundToInt(Mathf.Lerp(minX, maxX, 0.5f)), Mathf.RoundToInt(Mathf.Lerp(minY, maxY, 0.5f)), 0);
        }
    }
}


