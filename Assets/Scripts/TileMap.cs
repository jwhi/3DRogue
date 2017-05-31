using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;
//using RogueSharpRLNetSamples.Core;
//using RogueSharpRLNetSamples.Systems;
//using RogueSharpRLNetSamples.Monsters;
//using RogueSharpRLNetSamples;

public class TileMap : MonoBehaviour {

	public GameObject selectedUnit;
    public string[] dungeonLayout;
    public TileType[] tileTypes;

	public Tiles[,] tiles;
	private Node[,] graph;
    
	public int mapSizeX = 80;
	public int mapSizeY = 45;
    public int maxRooms = 30;
    public int maxRoomSize = 5;
    public int minRoomSize = 3;

    public int mapLevel = 0;

    //private List<DungeonMap> dungeonMap;
    private Unit player;
    //public Stairs stairsDown;
    //public Stairs stairsUp;

    public EnemyType[] enemyTypes;

    //private CommandSystem commandSystem;

    enum Layers : int { FOV = 9, Discovered, Undiscovered };
    
    void Awake()
    {
        Application.targetFrameRate = 60;
    }
    void Start()
    {
        tiles = new Tiles[mapSizeX, mapSizeY];
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                tiles[x, y] = new Tiles();
            }
        }
        //dungeonMap = new DungeonMap();
        //IMapCreationStrategy<Map> mapCreationStrategy = new RandomRoomsMapCreationStrategy<Map>(mapSizeX, mapSizeY, maxRooms, maxRoomSize, minRoomSize);
        //dungeonMap = (DungeonMap)Map.Create(mapCreationStrategy);
        //dungeonMap = new List<DungeonMap>();
        //MapGenerator mapGenerator = new MapGenerator(mapSizeX, mapSizeY, maxRooms, maxRoomSize, minRoomSize, mapLevel);
        //dungeonMap.Add(mapGenerator.CreateMap());
        //string dungeon = dungeonMap[0].ToString();
        //dungeonLayout = dungeon.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);
        string fileName = "dungeon_layout.txt";
        //File.WriteAllText(fileName, dungeon);

        // Setup the selectedUnit's variable
        player = selectedUnit.GetComponent<Unit>();
        //player.transform.position = new Vector3(mapGenerator.Player_X, 0, mapGenerator.Player_Y);
        player.tileX = Mathf.RoundToInt(selectedUnit.transform.position.x);
        player.tileY = Mathf.RoundToInt(selectedUnit.transform.position.z);
        //player.map = this;
        //stairsDown = dungeonMap[0].StairsDown;
        //stairsUp = dungeonMap[0].StairsUp;

        //commandSystem = new CommandSystem(dungeonMap[0]);

        for (int i = 0; i < 3; i++)
        {
            //RogueSharp.Point monsterLocation = dungeonMap[mapLevel].GetRandomLocation();
           // if (monsterLocation.X != player.tileX && monsterLocation.Y != player.tileY)
           // {
                //Goblin goblin = Goblin.Create(mapLevel);
                //goblin.Speed = 3;
                //goblin.X = monsterLocation.X;
                //goblin.Y = monsterLocation.Y;

                //dungeonMap[mapLevel].AddMonster(goblin);
           // }
        }

        DrawMonsters();
        GeneratePathfindingGraph();
        GenerateMapData();
        GenerateMapVisual();
    }

    // This method will be called any time we move the player to update field-of-view
    public void UpdatePlayerFieldOfView()
    {
        // Compute the field-of-view based on the player's location and awareness
        //dungeonMap[mapLevel].ComputeFov(player.tileX, player.tileY, player.awareness, true);
        // Mark all cells in field-of-view as having been explored
       // foreach (RogueSharp.ICell cell in dungeonMap[mapLevel].GetAllCells())
        //{
        //    if (dungeonMap[mapLevel].IsInFov(cell.X, cell.Y))
        //    {
               // dungeonMap[mapLevel].SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
        //    }
       // }
       /*
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                tiles[x, y].isExplored = dungeonMap[mapLevel].IsExplored(x, y);
                tiles[x, y].inFOV = dungeonMap[mapLevel].IsInFov(x, y);
            }
        }
        */
    }

    void GenerateMapData()
    {
    /*
        System.Random rnd = new System.Random();
        string dungeon = dungeonMap[mapLevel].ToString();
        
        dungeonLayout = dungeon.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                if (dungeonMap[mapLevel].StairsUp.X == x && dungeonMap[mapLevel].StairsUp.Y == y && mapLevel > 0)
                    tiles[x, y].tileType = 5;
                else if (dungeonMap[mapLevel].StairsDown.X == x && dungeonMap[mapLevel].StairsDown.Y == y)
                    tiles[x, y].tileType = 6;
                else
                {
                    switch (dungeonLayout[y][x])
                    {
                        case '#':
                            tiles[x, y].tileType = 2;
                            break;
                        case '.':
                            tiles[x, y].tileType = (rnd.Next(1, 100) > 30) ? 0 : 1;
                            break;
                        case '%':
                            tiles[x, y].tileType = 3; // outofpov
                            break;
                        case 's':
                            tiles[x, y].tileType = 4; // door
                            break;
                        default:
                            tiles[x, y].tileType = 0;
                            break;

                    }
                }
            }
        }*/
    }

	void GeneratePathfindingGraph() {
		// Initialize the array
		graph = new Node[mapSizeX,mapSizeY];

		// Initialize a Node for each spot in the array
		for(int x=0; x < mapSizeX; x++) {
			for(int y=0; y < mapSizeY; y++) {
				graph[x,y] = new Node();
				graph[x,y].x = x;
				graph[x,y].y = y;
			}
		}

		// Now that all the nodes exist, calculate their neighbours
		for(int x=0; x < mapSizeX; x++) {
			for(int y=0; y < mapSizeY; y++) {

				// This is the 4-way connection version:
/*				if(x > 0)
					graph[x,y].neighbours.Add( graph[x-1, y] );
				if(x < mapSizeX-1)
					graph[x,y].neighbours.Add( graph[x+1, y] );
				if(y > 0)
					graph[x,y].neighbours.Add( graph[x, y-1] );
				if(y < mapSizeY-1)
					graph[x,y].neighbours.Add( graph[x, y+1] );
*/

				// This is the 8-way connection version (allows diagonal movement)
				// Try left
				if(x > 0) {
					graph[x,y].neighbours.Add( graph[x-1, y] );
					if(y > 0)
						graph[x,y].neighbours.Add( graph[x-1, y-1] );
					if(y < mapSizeY-1)
						graph[x,y].neighbours.Add( graph[x-1, y+1] );
				}

				// Try Right
				if(x < mapSizeX-1) {
					graph[x,y].neighbours.Add( graph[x+1, y] );
					if(y > 0)
						graph[x,y].neighbours.Add( graph[x+1, y-1] );
					if(y < mapSizeY-1)
						graph[x,y].neighbours.Add( graph[x+1, y+1] );
				}

				// Try straight up and down
				if(y > 0)
					graph[x,y].neighbours.Add( graph[x, y-1] );
				if(y < mapSizeY-1)
					graph[x,y].neighbours.Add( graph[x, y+1] );

				// This also works with 6-way hexes and n-way variable areas (like EU4)
			}
		}
	}

	public void GenerateMapVisual()
    {
        /*
        UpdatePlayerFieldOfView();

        for (int x=0; x < mapSizeX; x++) {
			for(int y=0; y < mapSizeY; y++) {
                if (tiles[x, y].isExplored && !(tiles[x, y].isDrawn))
                {
                    TileType tt = tileTypes[tiles[x, y].tileType];
                    GameObject go;
                    tiles[x, y].isDrawn = true;
                    int rndRot = Random.Range(1, 3);
                    int randomTile = Random.Range(1, 10);
                    
                    if (tt.name == "Wall")
                    { go = (GameObject)Instantiate(tt.tileVisualPrefab, new Vector3(x, 1, y), Quaternion.identity); SetLayerRecursively(go, (int)Layers.Discovered); }
                    else if (tt.name == "Floor") { go = (GameObject)Instantiate((randomTile < 4) ? tt.tileVisualPrefabAlt : tt.tileVisualPrefab, new Vector3(x, 0, y), Quaternion.identity); go.transform.Rotate(new Vector3(0, (rndRot * 90), 0)); SetLayerRecursively(go, (int)Layers.Discovered); }
                    else if (tt.name == "Swamp") { go = (GameObject)Instantiate(tt.tileVisualPrefab, new Vector3(x, 0, y), Quaternion.identity); SetLayerRecursively(go, (int)Layers.Discovered); }
                    else if (tt.name == "Door") {
                        go = (GameObject)Instantiate(tt.tileVisualPrefab, new Vector3(x, 0, y), Quaternion.identity);
                        SetLayerRecursively(go, (int)Layers.Discovered);

                        if ((tileTypes[tiles[x, y + 1].tileType].name == "Floor") && (tiles[x, y + 1].isExplored))
                        {
                            go.transform.Rotate(new Vector3(0, 0, 0));
                        }
                        else if ((tileTypes[tiles[x, y - 1].tileType].name == "Floor") && (tiles[x, y - 1].isExplored))
                        {
                            go.transform.Rotate(new Vector3(0, 180, 0));
                        }
                        else if ((tileTypes[tiles[x - 1, y].tileType].name == "Floor") && (tiles[x - 1, y].isExplored))
                        {
                            go.transform.Rotate(new Vector3(0, 270, 0));
                        }
                        else if ((tileTypes[tiles[x + 1, y].tileType].name == "Floor") && (tiles[x + 1, y].isExplored))
                        {
                            go.transform.Rotate(new Vector3(0, 90, 0));
                        }
                        else if (tileTypes[tiles[x, y + 1].tileType].name == "Wall" && tileTypes[tiles[x, y - 1].tileType].name == "Wall")
                        {
                            go.transform.Rotate(new Vector3(0, 90, 0));
                        }
                    }
                    else if (tt.name == "StairsUp")
                    {
                        go = (GameObject)Instantiate(tt.tileVisualPrefab, new Vector3(x, 1, y), Quaternion.identity); SetLayerRecursively(go, (int)Layers.Discovered);
                        if ((tileTypes[tiles[x, y + 1].tileType].name == "Wall"))
                        {
                            go.transform.Rotate(new Vector3(0, 0, 0));
                        }
                        else if ((tileTypes[tiles[x, y - 1].tileType].name == "Wall"))
                        {
                            go.transform.Rotate(new Vector3(0, 180, 0));
                        }
                        else if ((tileTypes[tiles[x - 1, y].tileType].name == "Wall"))
                        {
                            go.transform.Rotate(new Vector3(0, 270, 0));
                        }
                        else if ((tileTypes[tiles[x + 1, y].tileType].name == "Wall"))
                        {
                            go.transform.Rotate(new Vector3(0, 90, 0));
                        }
                    } else if (tt.name == "StairsDown")
                    {
                        go = (GameObject)Instantiate(tt.tileVisualPrefab, new Vector3(x, 0, y), Quaternion.identity); SetLayerRecursively(go, (int)Layers.Discovered);
                    }
                }

                //ClickableTile ct = go.GetComponent<ClickableTile>();
                //ct.tileX = x;
                //ct.tileY = y;
                //ct.map = this;
            }
        }
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            int tileX = Mathf.RoundToInt(enemy.transform.position.x);
            int tileY = Mathf.RoundToInt(enemy.transform.position.z);
            if (tiles[tileX,tileY].inFOV)
            {
                SetLayerRecursively(enemy, (int)Layers.FOV);
            } else
            {
                SetLayerRecursively(enemy, (int)Layers.Undiscovered);
            }
        }

        foreach (GameObject dungeon in GameObject.FindGameObjectsWithTag("Dungeon"))
        {
            int tileX = Mathf.RoundToInt(dungeon.transform.position.x);
            int tileY = Mathf.RoundToInt(dungeon.transform.position.z);

            if (tiles[tileX, tileY].inFOV)
            {
                SetLayerRecursively(dungeon, (int)Layers.FOV);

            }
            else
            {
                SetLayerRecursively(dungeon, (int)Layers.Discovered);
            }
        }

        foreach (GameObject stairs in GameObject.FindGameObjectsWithTag("Stairs"))
        {
            int tileX = Mathf.RoundToInt(stairs.transform.position.x);
            int tileY = Mathf.RoundToInt(stairs.transform.position.z);

            if (tiles[tileX, tileY].inFOV)
            {
                SetLayerRecursively(stairs, (int)Layers.FOV);
                //stairs.transform.GetChild(5).GetComponent<Light>().enabled = true;
                stairs.GetComponentInChildren<Light>().enabled = true;
            }
            else
            {
                SetLayerRecursively(stairs, (int)Layers.Discovered);
                //stairs.transform.GetChild(5).GetComponent<Light>().enabled = false;
                stairs.GetComponentInChildren<Light>().enabled = false;
            }
        }

        foreach (GameObject door in GameObject.FindGameObjectsWithTag("Door"))
        {
            int tileX = Mathf.RoundToInt(door.transform.position.x);
            int tileY = Mathf.RoundToInt(door.transform.position.z);

            if (tiles[tileX, tileY].inFOV)
            {
                SetLayerRecursively(door, (int)Layers.FOV);
            }
            else
            {
                SetLayerRecursively(door, (int)Layers.Discovered);
            }
        }

        string miniMapString = "";
        for (int y = mapSizeY; y > 0; y--)
        {
            for (int x = 0; x < mapSizeX; x++)
            {
                if (tiles[x, y - 1].isExplored)
                {
                    if (x == player.tileX && y - 1 == player.tileY)
                    {
                        miniMapString += "<color=#fcf3cf>@</color>";
                    } else if (dungeonMap[mapLevel].GetMonsterAt(x,y-1) != null && tiles[x,y-1].inFOV) {
                        miniMapString += "<color=green>" + enemyTypes[0].Character + "</color>";
                    }
                    else
                    {
                        if (tiles[x, y - 1].tileType == 4) { miniMapString += "<color=#" + ColorUtility.ToHtmlStringRGBA(tileTypes[tiles[x,y-1].tileType].color) + ">" + ((dungeonMap[mapLevel].GetDoor(x, y - 1).IsOpen) ? '-' : '+') + "</color>"; }
                        else
                        {
                            miniMapString += "<color=#" + ColorUtility.ToHtmlStringRGBA(tileTypes[tiles[x, y - 1].tileType].color) + ">" +  tileTypes[tiles[x, y - 1].tileType].Character + "</color>";
                        }
                    }
                }
                else { miniMapString += ' '; }
            }
            miniMapString += "\n";
        }
        Text minimap = GameObject.Find("MiniMap").GetComponent<Text>();
        minimap.text = miniMapString;
        /*
        GameObject[] dungeonTiles = GameObject.FindGameObjectsWithTag("DungeonColor");

        foreach (GameObject dungeonTile in dungeonTiles)
        {

            int tileX = Mathf.RoundToInt(dungeonTile.transform.position.x);
            int tileY = Mathf.RoundToInt(dungeonTile.transform.position.z);
           
            Renderer renderer = dungeonTile.GetComponent<Renderer>();
            if (tiles[tileX, tileY].inFOV)
            {
                // renderer.material.SetColor("_EmissionColor", renderer.material.GetColor("_Color")*0.4f);
            } else
            {
                //renderer.material.SetColor("_EmissionColor", Color.black);
            }
        }
        */
        CheckDoors();
    }

	public Vector3 TileCoordToWorldCoord(int x, int y) {
		return new Vector3(x, 0, y);
	}

	public bool UnitCanEnterTile(int x, int y) {
        /*
        // We could test the unit's walk/hover/fly type against various
        // terrain flags here to see if they are allowed to enter the tile.
        if (dungeonMap[mapLevel].GetMonsterAt(x, y) == null)
            return tileTypes[tiles[x, y].tileType].isWalkable;
        else return false; */
        return false;
	}

	public void GeneratePathTo(int x, int y) {
        // Clear out our unit's old path.
		selectedUnit.GetComponent<Unit>().currentPath = null;

		if( UnitCanEnterTile(x,y) == false ) {
			// We probably clicked on a mountain or something, so just quit out.
			return;
		}

		Dictionary<Node, float> dist = new Dictionary<Node, float>();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

		// Setup the "Q" -- the list of nodes we haven't checked yet.
		List<Node> unvisited = new List<Node>();
        
        Node source = graph[
            GameObject.Find("Unit").GetComponent<Unit>().tileX,
            GameObject.Find("Unit").GetComponent<Unit>().tileY];
        
		
		Node target = graph[
		                    x, 
		                    y
		                    ];
		
		dist[source] = 0;
		prev[source] = null;

		// Initialize everything to have INFINITY distance, since
		// we don't know any better right now. Also, it's possible
		// that some nodes CAN'T be reached from the source,
		// which would make INFINITY a reasonable value
		foreach(Node v in graph) {
			if(v != source) {
				dist[v] = Mathf.Infinity;
				prev[v] = null;
			}

			unvisited.Add(v);
		}

		while(unvisited.Count > 0) {
			// "u" is going to be the unvisited node with the smallest distance.
			Node u = null;

			foreach(Node possibleU in unvisited) {
				if(u == null || dist[possibleU] < dist[u]) {
					u = possibleU;
				}
			}

			if(u == target) {
				break;	// Exit the while loop!
			}

			unvisited.Remove(u);

			foreach(Node v in u.neighbours) {
				//float alt = dist[u] + u.DistanceTo(v);
				float alt = dist[u];
				if( alt < dist[v] ) {
					dist[v] = alt;
					prev[v] = u;
				}
			}
		}

		// If we get there, the either we found the shortest route
		// to our target, or there is no route at ALL to our target.

		if(prev[target] == null) {
			// No route between our target and the source
			return;
		}

		List<Node> currentPath = new List<Node>();

		Node curr = target;

		// Step through the "prev" chain and add it to our path
		while(curr != null) {
			currentPath.Add(curr);
			curr = prev[curr];
		}

		// Right now, currentPath describes a route from out target to our source
		// So we need to invert it!

		currentPath.Reverse();

		selectedUnit.GetComponent<Unit>().currentPath = currentPath;
	}

    public void OpenDoor(int x, int y)
    {
        //dungeonMap[mapLevel].OpenDoor(x, y);

        //foreach (GameObject door in GameObject.FindGameObjectsWithTag("Door"))
        //{
        //    if (door.transform.position.x == x && door.transform.position.z == y)
        //        door.GetComponent<Animator>().SetTrigger("Open");
        //}

        //GenerateMapVisual();
    }

    public void CheckDoors()
    { /*
        foreach (GameObject door in GameObject.FindGameObjectsWithTag("Door"))
        {
            int doorX = Mathf.RoundToInt(door.transform.position.x);
            int doorY = Mathf.RoundToInt(door.transform.position.z);
            if (dungeonMap[mapLevel].Doors != null)
            {

                try
                {
                    bool doorOpen = (dungeonMap[mapLevel].GetDoor(doorX, doorY).IsOpen);
                    if (doorOpen) door.GetComponent<Animator>().SetTrigger("Open");

                } catch (System.NullReferenceException e)
                {
                    Debug.Log(e);
                }
            }
        }*/
    }

    // https://forum.unity3d.com/threads/change-gameobject-layer-at-run-time-wont-apply-to-child.10091/
    void SetLayerRecursively(GameObject obj, int newLayer  )
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public float CostToEnterTile(int sourceX, int sourceY, int targetX, int targetY)
    {

        //ileType tt = tileTypes[tiles[targetX, targetY].tileType];

        if (UnitCanEnterTile(targetX, targetY) == false)
            return Mathf.Infinity;

        float cost = 1;

        if (sourceX != targetX && sourceY != targetY)
        {
            // We are moving diagonally!  Fudge the cost for tie-breaking
            // Purely a cosmetic thing!
            cost += 0.001f;
        }

        return cost;

    }

    public void TakeStairsDown()
    {
        /*
        mapLevel += 1;

        DeleteMapVisual();

        tiles = new Tiles[mapSizeX, mapSizeY];
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                tiles[x, y] = new Tiles();
            }
        }

        //dungeonMap = new DungeonMap();
        //IMapCreationStrategy<Map> mapCreationStrategy = new RandomRoomsMapCreationStrategy<Map>(mapSizeX, mapSizeY, maxRooms, maxRoomSize, minRoomSize);
        //dungeonMap = (DungeonMap)Map.Create(mapCreationStrategy);
        if (mapLevel >= dungeonMap.Count) // New dungeon level?
        {

            MapGenerator mapGenerator = new MapGenerator(mapSizeX, mapSizeY, maxRooms, maxRoomSize, minRoomSize, mapLevel);
            dungeonMap.Add(mapGenerator.CreateMap());
            player = selectedUnit.GetComponent<Unit>();
            commandSystem = new CommandSystem(dungeonMap[mapLevel]);
            for (int i = 0; i < 5; i++)
            {
                Goblin goblin = Goblin.Create(mapLevel);
                RogueSharp.Point monsterLocation = dungeonMap[mapLevel].GetRandomLocation();
                goblin.X = monsterLocation.X;
                goblin.Y = monsterLocation.Y;

                dungeonMap[mapLevel].AddMonster(goblin);
            }
        }
        else // going to a previsited level? place next to down stairs
        {

            player = selectedUnit.GetComponent<Unit>();
            player.transform.position = new Vector3(dungeonMap[mapLevel].StairsUp.X, 0, dungeonMap[mapLevel].StairsUp.Y);
            
        }
        string dungeon = dungeonMap[mapLevel].ToString();
        dungeonLayout = dungeon.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);


        stairsDown = dungeonMap[mapLevel].StairsDown;
        stairsUp = dungeonMap[mapLevel].StairsUp;

        player.transform.position = new Vector3(dungeonMap[mapLevel].StairsUp.X, 0, dungeonMap[mapLevel].StairsUp.Y);
        player.tileX = Mathf.RoundToInt(selectedUnit.transform.position.x);
        player.tileY = Mathf.RoundToInt(selectedUnit.transform.position.z);
        player.map = this;
        GeneratePathfindingGraph();
        GenerateMapData();
        GenerateMapVisual();
        DrawMonsters(); */
        
    }

    public void TakeStairsUp()
    {
        mapLevel -= 1;

        DeleteMapVisual();

        tiles = new Tiles[mapSizeX, mapSizeY];
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                tiles[x, y] = new Tiles();
            }
        }

        //dungeonMap = new DungeonMap();
        //IMapCreationStrategy<Map> mapCreationStrategy = new RandomRoomsMapCreationStrategy<Map>(mapSizeX, mapSizeY, maxRooms, maxRoomSize, minRoomSize);
        //dungeonMap = (DungeonMap)Map.Create(mapCreationStrategy);
        /*
        if (mapLevel >= dungeonMap.Count) // New dungeon level?
        {
            MapGenerator mapGenerator = new MapGenerator(mapSizeX, mapSizeY, maxRooms, maxRoomSize, minRoomSize, mapLevel);
            dungeonMap.Add(mapGenerator.CreateMap());
            player = selectedUnit.GetComponent<Unit>();
            commandSystem = new CommandSystem(dungeonMap[mapLevel]);
            player.transform.position = new Vector3(mapGenerator.Player_X, 0, mapGenerator.Player_Y);
        }
        else // going to a previsited level? place next to down stairs
        {
            player = selectedUnit.GetComponent<Unit>();
            player.transform.position = new Vector3(dungeonMap[mapLevel].StairsDown.X, 0, dungeonMap[mapLevel].StairsDown.Y);
        }

        stairsDown = dungeonMap[mapLevel].StairsDown;
        stairsUp = dungeonMap[mapLevel].StairsUp;
        
        player.tileX = Mathf.RoundToInt(selectedUnit.transform.position.x);
        player.tileY = Mathf.RoundToInt(selectedUnit.transform.position.z);
        player.map = this;
        GeneratePathfindingGraph();
        GenerateMapData();
        GenerateMapVisual();
        DrawMonsters();
        */
    }

    public void EndPlayersTurn()
    {
        /*
        dungeonMap[mapLevel].player.X = Mathf.RoundToInt(GameObject.Find("Unit").GetComponent<Unit>().tileX);
        dungeonMap[mapLevel].player.Y = Mathf.RoundToInt(GameObject.Find("Unit").GetComponent<Unit>().tileY);
        foreach (GameObject monsterIcon in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Monster monster = dungeonMap[mapLevel].GetMonsterAt(Mathf.RoundToInt(monsterIcon.transform.position.x), Mathf.RoundToInt(monsterIcon.transform.position.z));
            if (monster != null) 
            {
                commandSystem.dungeonMap = dungeonMap[mapLevel];
                monster.PerformAction(commandSystem);
                monsterIcon.transform.position = new Vector3(monster.X, 0, monster.Y);
                //monsterIcon.transform.position = Vector3.Lerp(transform.position, TileCoordToWorldCoord(monster.X, monster.Y), 5f * Time.deltaTime);
                
            }
        }
        */
    }

    private void DeleteMapVisual()
    {
        foreach (GameObject oldDungeon in GameObject.FindGameObjectsWithTag("Dungeon"))
        {
            GameObject.Destroy(oldDungeon);
        }
        foreach (GameObject oldDoors in GameObject.FindGameObjectsWithTag("Door"))
        {
            GameObject.Destroy(oldDoors);
        }
        foreach (GameObject oldStairs in GameObject.FindGameObjectsWithTag("Stairs"))
        {
            GameObject.Destroy(oldStairs);
        }
        foreach (GameObject oldEnemies in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            GameObject.Destroy(oldEnemies);
        }

    }

    public void DrawMonsters()
    {
        /*
        foreach (Monster monster in dungeonMap[mapLevel].GetMonsters())
        {
            GameObject go = (GameObject)Instantiate(enemyTypes[0].tileVisualPrefab, new Vector3(monster.X, 0, monster.Y), Quaternion.identity); SetLayerRecursively(go, (int)Layers.Undiscovered);

        }
        */
    }
}
