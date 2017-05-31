using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;

//using RogueSharpRLNetSamples.Core;
//using RogueSharpRLNetSamples.Systems;
//using RogueSharpRLNetSamples.Monsters;
//using RogueSharpRLNetSamples;

public class Stairs
{
    int x;
    int y;
}

public class TileMapFile : MonoBehaviour
{
    public GameObject selectedUnit;
    public string[] dungeonLayout;
    public string[] tempdungeon;
    public string[] minimapText;
    public string[] lightingText;
    public TileType[] tileTypes;

    public Tiles[,] tiles;

    int mapSizeX = 80;
    int mapSizeY = 45;
    int maxRooms = 30;
    int maxRoomSize = 5;
    int minRoomSize = 3;

    int mapLevel = 0;

    private System.DateTime[] lastModified;
    private string dungeonCharacterFile = @"Z:\angband\src\_dungeonCharacter.txt";
    private string dungeonFloorCharacterFile = @"Z:\angband\src\_mapInfo.txt";
    private string dungeonLightingFile = @"Z:\angband\src\_dungeonLighting.txt";
    private string playerInfoFile = @"Z:\angband\src\_playerInfo.txt";
    private string offsetFile = @"Z:\angband\src\_offsetFile.txt";
    private string dungeonCharacterText;
    private string dungeonFloorText;
    private string dungeonLightingText;
    private string playerInfoText;

    private int ViewOffset_X;
    private int ViewOffset_Y;

    private bool FOV_CHANGED;

    //private List<DungeonMap> dungeonMap;
    private Unit player;
    public Stairs stairsDown;
    public Stairs stairsUp;

    public EnemyType[] enemyTypes;

    //private CommandSystem commandSystem;

    enum Layers : int { FOV = 9, Discovered, Undiscovered };

    void Awake()
    {
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
    }
    void Start()
    {
        lastModified = new System.DateTime[5];
        FOV_CHANGED = false;
        /*
        lastModified[0] = File.GetLastWriteTime(dungeonCharacterFile);
        lastModified[1] = File.GetLastWriteTime(dungeonFloorCharacterFile);
        lastModified[2] = File.GetLastWriteTime(dungeonLightingFile);
        lastModified[3] = File.GetLastWriteTime(playerInfoFile);
        lastModified[4] = File.GetLastWriteTime(offsetFile);
        
        while (!IsFileReady(dungeonCharacterFile))
        {

        }
        dungeonCharacterText = File.ReadAllText(dungeonCharacterFile);
        while (!IsFileReady(dungeonFloorCharacterFile))
        {

        }
        dungeonFloorText = File.ReadAllText(dungeonFloorCharacterFile);
        while (!IsFileReady(dungeonLightingFile))
        {

        }
        dungeonLightingText = File.ReadAllText(dungeonLightingFile);
        while (!IsFileReady(playerInfoFile))
        {

        }
        playerInfoText = File.ReadAllText(playerInfoFile);
        while (!IsFileReady(offsetFile))
        {

        }
        string[] offsetValues = File.ReadAllText(offsetFile).Split(new string[] { " " }, System.StringSplitOptions.None);
        ViewOffset_X = int.Parse(offsetValues[0]);
        ViewOffset_Y = int.Parse(offsetValues[1]);
        Debug.Log(ViewOffset_X + " " + ViewOffset_Y);


        dungeonLayout = dungeonFloorText.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);
        mapSizeY = dungeonLayout.Length-1;
        mapSizeX = dungeonLayout[0].Length - 1;

        for (int i = 0; i < dungeonLayout.Length / 2; i++)
        {
            string tmp = dungeonLayout[i];
            dungeonLayout[i] = dungeonLayout[dungeonLayout.Length - i - 1];
            dungeonLayout[dungeonLayout.Length - i - 1] = tmp;
        }
        

        minimapText = dungeonCharacterText.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);
        
        


        for (int y = 0; y < minimapText.Length-1; y++)
        {
            for (int x = 0; x < minimapText[y].Length; x++)
            {
                if (minimapText[y][x] == '@')
                {
                    player.tileX = x + ViewOffset_X;
                    player.tileY = dungeonLayout.Length - (ViewOffset_Y + y + 1);
                    selectedUnit.transform.position = new Vector3(x + ViewOffset_X, 0, dungeonLayout.Length - (ViewOffset_Y + y +1));
                }
            }
        }


        tiles = new Tiles[mapSizeX, mapSizeY];
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                tiles[x, y] = new Tiles();
            }
        }
        */
        player = selectedUnit.GetComponent<Unit>();
        player.map = this;
        CreateArrays();
        GenerateMapData();
        GenerateMapVisual();
    }

    void Update()
    {
        if ((lastModified[0] != File.GetLastWriteTime(dungeonCharacterFile)) ||
            (lastModified[1] != File.GetLastWriteTime(dungeonFloorCharacterFile)) ||
            (lastModified[2] != File.GetLastWriteTime(dungeonLightingFile)) ||
            (lastModified[3] != File.GetLastWriteTime(playerInfoFile)) ||
            (lastModified[4] != File.GetLastWriteTime(offsetFile)))
        {
            CreateArrays();
        }
        if (FOV_CHANGED)
        {
            Debug.Log("FOV");
            UpdateFOV();
        }
    }

    void CreateArrays ()
    {
        lastModified[0] = File.GetLastWriteTime(dungeonCharacterFile);
        lastModified[1] = File.GetLastWriteTime(dungeonFloorCharacterFile);
        lastModified[2] = File.GetLastWriteTime(dungeonLightingFile);
        lastModified[3] = File.GetLastWriteTime(playerInfoFile);
        lastModified[4] = File.GetLastWriteTime(offsetFile);

        

        
        while (!IsFileReady(dungeonFloorCharacterFile)) { }
        string tmpDungeonFloorText = File.ReadAllText(dungeonFloorCharacterFile);
        if (tmpDungeonFloorText.CompareTo(dungeonFloorText) != 0)
        {
            dungeonFloorText = tmpDungeonFloorText;
            DeleteMapVisual();
            dungeonLayout = dungeonFloorText.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);
            
            // dungeonLayout = flipArray(dungeonLayout);
            
            mapSizeY = dungeonLayout.Length - 1;
            mapSizeX = dungeonLayout[mapSizeY-1].Length - 1;

            tiles = new Tiles[mapSizeX, mapSizeY];
            for (int x = 0; x < mapSizeX; x++)
            {
                for (int y = 0; y < mapSizeY; y++)
                {
                    tiles[x, y] = new Tiles();
                }
            }
        }
        
        // Player info has player stats
        while (!IsFileReady(playerInfoFile)) { }
        string tmpPlayerInfoText = File.ReadAllText(playerInfoFile);
        if (tmpPlayerInfoText.CompareTo(playerInfoText) != 0)
        {
            playerInfoText = tmpPlayerInfoText;
        }

        // Offset file determines the location the local map relative to the floor map
        while (!IsFileReady(offsetFile))
        {

        }
        string[] offsetValues = File.ReadAllText(offsetFile).Split(new string[] { " " }, System.StringSplitOptions.None);
        ViewOffset_X = int.Parse(offsetValues[0]);
        ViewOffset_Y = int.Parse(offsetValues[1]);

        // Just a small portion of the map that is displayed within the angband game window
        while (!IsFileReady(dungeonCharacterFile)) { }
        string tmpDungeonCharacterText = File.ReadAllText(dungeonCharacterFile);
        if (tmpDungeonCharacterText.CompareTo(dungeonCharacterText) != 0)
        {
            dungeonCharacterText = tmpDungeonCharacterText;
            minimapText = dungeonCharacterText.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);
            // minimapText = flipArray(minimapText);

            for (int y = 0; y < minimapText.Length - 1; y++)
            {
                for (int x = 0; x < minimapText[y].Length; x++)
                {
                    if (minimapText[y][x] == '@')
                    {
                        player.tileX = x + ViewOffset_X;
                        player.tileY = dungeonLayout.Length - (ViewOffset_Y + y + 1);
                        selectedUnit.transform.position = new Vector3(x + ViewOffset_X, 0, dungeonLayout.Length - (ViewOffset_Y + y + 1));
                    }
                }
            }

        }

        // Lighting file determines what is in FOV
        while (!IsFileReady(dungeonLightingFile)) { }
        string tmpDungeonLightingText = File.ReadAllText(dungeonLightingFile);
        if (tmpDungeonLightingText.CompareTo(dungeonLightingText) != 0)
        {
            dungeonLightingText = tmpDungeonLightingText;
            lightingText = dungeonLightingText.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);
            FOV_CHANGED = true;
        }
    }

    void GenerateMapData()
    {
        System.Random rnd = new System.Random();
        

        for (int y = 0; y < dungeonLayout.Length-1; y++)
        {
            for (int x = 0; x < dungeonLayout[y].Length-1; x++)
            {
                switch (dungeonLayout[y][x])
                {
                    case '#':
                        tiles[x, y].tileType = 2;
                        break;
                    case '.':
                        tiles[x, y].tileType = 0; //(rnd.Next(1, 100) > 30) ? 0 : 1;
                        break;
                    case '%':
                        // tiles[x, y].tileType = 3; // outofpov
                        tiles[x, y].tileType = 2; // used for ores in angband
                        break;
                    case '+':
                        tiles[x, y].tileType = 4; // door
                        break;
                    case '<':
                        tiles[x, y].tileType = 5; // stairs up
                        break;
                    case '>':
                        tiles[x, y].tileType = 6; // stairs down
                        break;
                    default:
                        tiles[x, y].tileType = 3; // Empty
                        break;
                }
            }
        }
    }


    public void GenerateMapVisual()
    {

        GenerateMapData();
        //UpdatePlayerFieldOfView();

        for (int x = 0; x < mapSizeX; x++) {
            for (int y = 0; y < mapSizeY; y++) {
                if (tiles[x,y].isExplored) {
                    if (!(tiles[x, y].isDrawn))
                    {
                        int worldX = x;
                        int worldY = mapSizeY - y;

                        TileType tt = tileTypes[tiles[x, y].tileType];
                        GameObject go;
                        tiles[x, y].isDrawn = true;

                        if (tt.name == "Wall")
                        {
                            //go = (GameObject)Instantiate(tt.tileVisualPrefab, new Vector3(x, 1, y), Quaternion.identity);
                            //SetLayerRecursively(go, (int)Layers.Undiscovered);
                        }
                        else if (tt.name == "Floor")
                        {
                            go = (GameObject)Instantiate(tt.tileVisualPrefab, new Vector3(worldX, 0, worldY), Quaternion.identity);
                        }
                        else if (tt.name == "Swamp") { go = (GameObject)Instantiate(tt.tileVisualPrefab, new Vector3(worldX, 0, worldY), Quaternion.identity); SetLayerRecursively(go, (int)Layers.Discovered); }
                        else if (tt.name == "Door")
                        {
                            go = (GameObject)Instantiate(tt.tileVisualPrefab, new Vector3(worldX, 0, worldY), Quaternion.identity);
                            //SetLayerRecursively(go, (int)Layers.Discovered);

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
                            go = (GameObject)Instantiate(tt.tileVisualPrefab, new Vector3(worldX, 1, worldY), Quaternion.identity);
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
                        }
                        else if (tt.name == "StairsDown")
                        {
                            go = (GameObject)Instantiate(tt.tileVisualPrefab, new Vector3(worldX, 0, worldY), Quaternion.identity);
                        }


                    }
                }
                //ClickableTile ct = go.GetComponent<ClickableTile>();
                //ct.tileX = x;
                //ct.tileY = y;
                //ct.map = this;
                
            }
        }
        /*
        
        
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

            if (tiles[tileX, tileY].isExplored)
            {
                SetLayerRecursively(door, (int)Layers.Discovered);
            }
            if (tiles[tileX, tileY].inFOV)
            {
                SetLayerRecursively(door, (int)Layers.FOV);
                Debug.Log("FOV");
            }
        }
        /*
        string miniMapString = "";
        for (int y = 0; y < mapSizeY; y++)
        {
            for (int x = 0; x < mapSizeX; x++)
            {
                if (x == player.tileX && y == player.tileY)
                {
                    miniMapString += "<color=#fcf3cf>@</color>";
                }
                else if (tiles[x, y].tileType == 3)
                { // empty tyle
                    miniMapString += ' ';
                } else
                {
                   miniMapString += "<color=#" + ColorUtility.ToHtmlStringRGBA(tileTypes[tiles[x, y].tileType].color) + ">" + tileTypes[tiles[x, y].tileType].Character + "</color>";
                }
            }
            miniMapString += "\n";
        }
        Text minimap = GameObject.Find("MiniMap").GetComponent<Text>();
        minimap.text = miniMapString;
        */
        //GameObject[] dungeonTiles = GameObject.FindGameObjectsWithTag("DungeonColor");
        /*
        foreach (GameObject dungeonTile in dungeonTiles)
        {

            int tileX = Mathf.RoundToInt(dungeonTile.transform.position.x);
            int tileY = Mathf.RoundToInt(dungeonTile.transform.position.z);
           
        }*/
    }

    void UpdateFOV()
    {
        // Called when Lighting file is updated
        // selectedUnit.transform.position = new Vector3(x + ViewOffset_X, 0, dungeonLayout.Length - (ViewOffset_Y + y + 1));
        Debug.Log("updating from array");
        for (int y = 0; y < minimapText.Length - 1; y++)
        {
            for (int x = 0; x < minimapText[y].Length && x < lightingText[y].Length; x++)
            {
                if (lightingText[y][x] == '1' && minimapText[y][x] == '.')
                {
                    Debug.Log("Fov X: " + (x + ViewOffset_X) + " Fov Y: " + (y + ViewOffset_Y));
                    tiles[x + ViewOffset_X, y + ViewOffset_Y].inFOV = true;
                    tiles[x + ViewOffset_X, y + ViewOffset_Y].isExplored = true;
                }
                else if (lightingText[y][x] == '8' && minimapText[y][x] != ' ')
                {
                    tiles[x + ViewOffset_X, y + ViewOffset_Y].inFOV = false;
                    tiles[x + ViewOffset_X, y + ViewOffset_Y].isExplored = true;
                }
            }
        }

        Debug.Log("Going through game objects.");
        foreach (GameObject dungeon in GameObject.FindGameObjectsWithTag("Dungeon"))
        {
            int tileX = Mathf.RoundToInt(dungeon.transform.position.x);
            int tileY = Mathf.RoundToInt(dungeon.transform.position.z);
            
            if (tiles[tileX, tileY].inFOV)
            {
                Debug.Log("WHAAAAT");
                SetLayerRecursively(dungeon, (int)Layers.FOV);
            }
            else
            {
                SetLayerRecursively(dungeon, (int)Layers.Discovered);
            }
        }

        Debug.Log("FOV DONE");
        FOV_CHANGED = false;
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

    //http://stackoverflow.com/questions/1406808/wait-for-file-to-be-freed-by-process thanks gordon
    public static bool IsFileReady(string sFilename)
    {
        // If the file can be opened for exclusive access it means that the file
        // is no longer locked by another process.
        try
        {
            using (FileStream inputStream = File.Open(sFilename, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                if (inputStream.Length > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }
        catch (System.Exception)
        {
            return false;
        }
    }

    private string[] flipArray(string[] array)
    {
        for (int i = 0; i < array.Length / 2; i++)
        {
            string tmp = array[i];
            array[i] = array[array.Length - i - 1];
            array[array.Length - i - 1] = tmp;
        }
        return array;
    }
}