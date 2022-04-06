// Written by Joy de Ruijter
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace XDungeon
{
    public class XDungeonGenerator : MonoBehaviour
    {
        #region Variables

        public GameObject floorPrefab;
        public GameObject wallPrefab;

        public int gridWidth = 100;
        public int gridHeight = 100;
        public int minRoomSize = 3;
        public int maxRoomSize = 8;
        public int numRooms = 10;
        public Dictionary<Vector3Int, XTiletype> Xdungeon = new Dictionary<Vector3Int, XTiletype>();
        public List<XRoom> XroomList = new List<XRoom>();
        public Dictionary<Vector3Int, GameObject> XfloorList = new Dictionary<Vector3Int, GameObject>();

        #endregion

        private void Awake()
        {
            Generate();
        }

        public void Generate()
        {
            // Doors?

            for (int i = 0; i < numRooms; i++)
            {
                int minX = Random.Range(0, gridWidth);
                int maxX = minX + Random.Range(minRoomSize, maxRoomSize + 1);
                int minZ = Random.Range(0, gridHeight);
                int maxZ = minZ + Random.Range(minRoomSize, maxRoomSize + 1);

                XRoom room = new XRoom(minX, maxX, minZ, maxZ);

                if (RoomFitsInDungeon(room))
                    AddRoomToDungeon(room);
                else
                    i--;
            }

            for (int i = 0; i < XroomList.Count; i++)
            { 
                XRoom room = XroomList[i];
                XRoom otherRoom = XroomList[(i + Random.Range(1, XroomList.Count)) % XroomList.Count];
                ConnectRooms(room, otherRoom);
            }

            AllocateWalls();
            SpawnDungeon();
        }

        public void AllocateWalls()
        {
            var keys = Xdungeon.Keys.ToList();
            foreach (var kvp in keys)
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        Vector3Int newPos = kvp + new Vector3Int(x, 0, z);

                        if (Xdungeon.ContainsKey(newPos))
                            continue;
                        Xdungeon.Add(newPos, XTiletype.Wall);
                    }
                }
            }
        }

        public void ConnectRooms(XRoom _roomOne, XRoom _roomTwo)
        {
            Vector3Int posOne = _roomOne.GetCenter();
            Vector3Int posTwo = _roomTwo.GetCenter();
            
            int dirX = posTwo.x > posOne.x ? 1 : -1;
            int x = 0;
            for (x = posOne.x; x != posTwo.x; x += dirX)
            {
                Vector3Int position = new Vector3Int(x, 0, posOne.z);

                if (Xdungeon.ContainsKey(position))
                    continue;

                Xdungeon.Add(position, XTiletype.Floor);
            }

            int dirZ = posTwo.z > posOne.z ? 1 : -1;
            for (int z = posOne.z; z != posTwo.z; z += dirZ)
            {
                Vector3Int position = new Vector3Int(x, 0, z);

                if (Xdungeon.ContainsKey(position))
                    continue;

                Xdungeon.Add(position, XTiletype.Floor);
            }

        }

        public void SpawnDungeon()
        {
            foreach (KeyValuePair<Vector3Int, XTiletype> kvp in Xdungeon)
            {
                switch (kvp.Value)
                {
                    case XTiletype.Floor:
                        XfloorList.Add(kvp.Key, (Instantiate(floorPrefab, kvp.Key, Quaternion.identity, transform)));
                        break;
                    case XTiletype.Wall:
                        Vector3Int wallPosition = new Vector3Int(kvp.Key.x, 1, kvp.Key.z);
                        Instantiate(wallPrefab, wallPosition, Quaternion.identity, transform);
                        break;
                }
            }
        }

        public void AddRoomToDungeon(XRoom room)
        {
            for (int x = room.minX; x <= room.maxX; x++)
            {
                for (int z = room.minZ; z <= room.maxZ; z++)
                {
                    Xdungeon.Add(new Vector3Int(x, 0, z), XTiletype.Floor);
                }
            }
            XroomList.Add(room);
        }

        public bool RoomFitsInDungeon(XRoom room)
        {
            for (int x = room.minX - 1; x <= room.maxX + 1; x++)
            {
                for (int z = room.minZ - 1; z <= room.maxZ + 1; z++)
                {
                    if (Xdungeon.ContainsKey(new Vector3Int(x, 0, z)))
                        return false;
                }
            }
            return true;
        }

    }


    public enum XTiletype { Floor, Wall}

    public class XRoom
    {
        #region Variables

        public int minX, maxX, minZ, maxZ;

        public XRoom(int _minX, int _maxX, int _minZ, int _maxZ)
        { 
            minX = _minX;
            maxX = _maxX;
            minZ = _minZ;
            maxZ = _maxZ;
        }

        #endregion

        public Vector3Int GetCenter()
        {
            return new Vector3Int(Mathf.RoundToInt(Mathf.Lerp(minX, maxX, 0.5f)), 0, Mathf.RoundToInt(Mathf.Lerp(minZ, maxZ, 0.5f)));
        }
    }
}

