using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private int deviationRate = 15;
    [SerializeField] private int roomRate = 15;
    [SerializeField] private int maxRouteLength;
    [SerializeField] private int maxRoutes = 50;
    [SerializeField] private EnemyPool enemyPool;
    [SerializeField] private GameObject enemiesContainer;
    [SerializeField] private GameObject player;
    [SerializeField] private Tile backgrTile;
    [SerializeField] private Tile topWallTile;
    [SerializeField] private Tile botWallTile;
    [SerializeField] private Tilemap groundMap;
    [SerializeField] private Tilemap backgrMap;
    [SerializeField] private Tilemap wallMap;
    [SerializeField] private List<Tile> groundTiles;

    private bool enemySpawned;
    private int routeCount = 0;

    private void Start()
    {
        int x = 0;
        int y = 0;
        int routeLength = 0;
        GenerateRoom(x, y, 1);
        Vector2Int previousPos = new Vector2Int(x, y);
        y += 3;
        GenerateRoom(x, y, 1);
        NewRoute(x, y, routeLength, previousPos);

        FillWalls();
    }

    private void FillWalls()
    {
        BoundsInt bounds = groundMap.cellBounds;
        for (int xMap = bounds.xMin - 10; xMap <= bounds.xMax + 10; xMap++)
        {
            for (int yMap = bounds.yMin - 10; yMap <= bounds.yMax + 10; yMap++)
            {
                Vector3Int pos = new Vector3Int(xMap, yMap, 0);
                Vector3Int posBelow = new Vector3Int(xMap, yMap - 1, 0);
                Vector3Int posAbove = new Vector3Int(xMap, yMap + 1, 0);
                TileBase tile = groundMap.GetTile(pos);
                TileBase tileBelow = groundMap.GetTile(posBelow);
                TileBase tileAbove = groundMap.GetTile(posAbove);
                if (tile == null)
                {
                    backgrMap.SetTile(pos, backgrTile);
                    if (tileBelow != null)
                    {
                        wallMap.SetTile(pos, topWallTile);
                    }
                    else if (tileAbove != null)
                    {
                        wallMap.SetTile(pos, botWallTile);
                    }
                }
            }
        }
    }

    private void NewRoute(int x, int y, int routeLength, Vector2Int previousPos)
    {
        if (routeCount < maxRoutes)
        {
            routeCount++;
            while (++routeLength < maxRouteLength)
            {
                //Initialize
                bool routeUsed = false;
                int xOffset = x - previousPos.x;
                int yOffset = y - previousPos.y;
                int roomSize = 1; //Corridor size
                if (Random.Range(1, 100) <= roomRate)
                    roomSize = Random.Range(3, 6);
                previousPos = new Vector2Int(x, y);

                //Go Straight
                if (Random.Range(1, 100) <= deviationRate)
                {
                    if (routeUsed)
                    {
                        GenerateRoom(previousPos.x + xOffset, previousPos.y + yOffset, roomSize);
                        NewRoute(previousPos.x + xOffset, previousPos.y + yOffset, Random.Range(routeLength, maxRouteLength), previousPos);
                    }
                    else
                    {
                        x = previousPos.x + xOffset;
                        y = previousPos.y + yOffset;
                        GenerateRoom(x, y, roomSize);
                        routeUsed = true;
                    }
                }

                //Go left
                if (Random.Range(1, 100) <= deviationRate)
                {
                    if (routeUsed)
                    {
                        GenerateRoom(previousPos.x - yOffset, previousPos.y + xOffset, roomSize);
                        NewRoute(previousPos.x - yOffset, previousPos.y + xOffset, Random.Range(routeLength, maxRouteLength), previousPos);
                    }
                    else
                    {
                        y = previousPos.y + xOffset;
                        x = previousPos.x - yOffset;
                        GenerateRoom(x, y, roomSize);
                        routeUsed = true;
                    }
                }
                //Go right
                if (Random.Range(1, 100) <= deviationRate)
                {
                    if (routeUsed)
                    {
                        GenerateRoom(previousPos.x + yOffset, previousPos.y - xOffset, roomSize);
                        NewRoute(previousPos.x + yOffset, previousPos.y - xOffset, Random.Range(routeLength, maxRouteLength), previousPos);
                    }
                    else
                    {
                        y = previousPos.y - xOffset;
                        x = previousPos.x + yOffset;
                        GenerateRoom(x, y, roomSize);
                        routeUsed = true;
                    }
                }

                if (!routeUsed)
                {
                    x = previousPos.x + xOffset;
                    y = previousPos.y + yOffset;
                    GenerateRoom(x, y, roomSize);
                }
            }
        }
    }

    private void GenerateRoom(int x, int y, int radius)
    {
        for (int tileX = x - radius; tileX <= x + radius; tileX++)
        {
            for (int tileY = y - radius; tileY <= y + radius; tileY++)
            {
                Tile tile = groundTiles[Random.Range(0, groundTiles.Count)];

                Vector3Int tilePos = new Vector3Int(tileX, tileY, 0);
                groundMap.SetTile(tilePos, tile);
            }
        }

        if (!enemySpawned && x != 0 && y != 0)
        {
            enemySpawned = true;
            Instantiate(enemyPool.GetRandomMonster(), new Vector3(x + (radius / 2), y + (radius / 2), 0), Quaternion.identity, enemiesContainer.transform);
        }
        else if (Random.Range(0f, 100f) <= 10f && x != 0 && y != 0)
        {
            Instantiate(enemyPool.GetRandomMonster(), new Vector3(x + (radius / 2), y + (radius / 2), 0), Quaternion.identity, enemiesContainer.transform);
        }
    }
}
