using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;

// Stairs x and y coordinate with bool if stairs up or stairs down
public class Stairs { int X, Y; bool Up; }

public class Door { public int X, Y; public bool IsOpen; public Door(int x, int y, bool isOpen) { X = x; Y = y; IsOpen = isOpen; } }

/* PlayerInfo class stores information about the player that comes from
 * the PlayerInfo file from Angband. Level is the character's level, depth
 * is the current depth of the dungeon.
 * CurrentHP and MaxHP will be used to affect visual appearance of character icon.
 */
public class PlayerInfo { public int CurrentHP, MaxHP, Level, Depth; }

public class TileMapFile : MonoBehaviour
{
    /*
     * TileType data is stored in Unity scene. TileType class has tile name,
     * Unity prefab to be displayed for tile, if the tile is able to be walked on by
     * player which is not needed when reading data from Angband, Character to be
     * displayed in the ASCII representation of the dungeon map, and color of mini
     * map character. These values are kept in TileData enum as well to help readability
     * 0: Cave floor
     * 1: Wall
     * 2: Door
     * 3: Stairs up
     * 4: Stairs down
     * 5: Empty
     */
    public TileType[] TileTypes;

    /*
     * Tile data is created and updated in this file. Tile data created based on
     * DungeonMap, WindowMap, and LightingMap values.
     * Tile variables are:
     * int TileType: Index of Tile Data stored in TileTypes array. Based on information in DungeonMap and WindowMap
     * bool InFOV: Tile is lit since in the player's FOV. Light information for that tile is 1 in LightingMap
     * bool InWindow: Repaced IsExplored which was for when player has visited the tile before and should remain drawn. InWindow is set for tiles in Angband game window needed to be drawn
     * bool IsDrawn: True if the tile is currently displayed on screen to prevent duplicate tiles. Only tiles in WindowMap are drawn.
     */
    public Tile[,] Tiles;

    /*
     * lastModified stores when all the files were last updated by Angband. Only updates
     * information when files are updated.
     */
    private System.DateTime[] LastModified;

    /* Object in the scene with Player model is dragged here from Unity */
    public GameObject PlayerObject;

    /* Variable that stores that data saved in the PlayerObject's Unit script */
    private Unit player;

    /*
     * File locations for all the text files from Angband.
     * Allow to be customized in future.
     * OffsetFile stores where the WindowMap starts in relation to the DungeonMapFile
     * Provided image in Notes folder ("Notes/Offset Explanation.png") that helps explain.
     * Area outlined in blue is roughly what is stored by WindowMap
     * Red Square in top left of blue outline is the location that is stored in the
     * OffsetFile as x and y values where x is distance from left border of DungeonMap
     * and y is the distance from the top of DungeonMap
     */
    private string DungeonMapFile = @"Z:\build\games\_MapInfo.txt";
    private string WindowMapFile = @"Z:\build\games\_WindowMap.txt";
    private string FeatureMapFile = @"Z:\build\games\_FeatureMap.txt";
    private string LightingMapFile = @"Z:\build\games\_LightingMap.txt";
    private string PlayerInfoFile = @"Z:\build\games\_PlayerInfo.txt";
    private string OffsetFile = @"Z:\build\games\_OffsetFile.txt";

    /*
     * Information read from Angband files stored here.
     * These strings are split and are used to create the string arrays
     * String arrays are what are used to render the map visual, but These
     * strings are useful to check if a map content was updated or not.
     * Angband creates files even if the content is the same so modified DateTime
     * for the files can be different but content will be the same.
     */
    private string DungeonMapText;
    private string WindowMapText;
    private string FeatureMapText;
    private string PlayerInfoText;

    public string[] DungeonMap; // Full map of the dungeon floor from Angband
    public string[] WindowMap; // Portion of the map displayed in Angband game window
    public string[] FeatureMap; // Covers same area as WindowMap but includes icons for loot, player, and moster locations
    public string[] LightingMap; // Lighting values for the potion of the map displayed in Angband game window

    /*
     * OffsetFile only keeps these two values in the file so there isn't a need to store
     * the files contents in a string or a string[] like the rest of the files
     */
    private int WindowOffsetX;
    private int WindowOffsetY;

    private int MapSizeX;
    private int MapSizeY;

    private List<Door> Doors; // All the doors on the current map

    /*
    * Corresponds to layer values in Unity for the scene.
    * FOV layer is lit with an Area light attatched to player so tiles within
    * FOV are brighter than others. Discovered are still able to be seen in the
    * scene but are lit only by the scene's ambient light. Undiscovered tiles are
    * not seen by the main camera and are not drawn.
    * An exception to Undiscovered layer not being drawn are enemies in original 3D Rogue project.
    * Enemies are either in FOV or Undiscovered. Undiscovered enemies are still seen
    * in the editor, but not seen in Game view.
    */
    enum Layers : int { FOV = 9, Discovered, Undiscovered };

    /*
     * Files enum is used to increase readability of LastModified array
     */
    enum Files : int { DungeonMap = 0, WindowMap, FeatureMap, LightingMap, OffsetValues };

    /*
     * TileData enum will be used when assigned tile type to tiles array. Values for
     * the different tile data are determined by their location in the TileType array
     * that is defined in the Unity scene.
     */
    enum TileData : int { Floor = 0, Wall, Door, StairsUp, StairsDown, Empty };



    /*
     * Called when the game is first run, but may be able to just include in Start().
     * Target Frame Rate is 60 since camera movement speed is based on framerate.
     * Had troubles in Window builds of the game with camera movement being too
     * quick from usability standpoint.
     * I wanted the application to run in background since for the time being, game
     * can only be controlled in Angband game window, so running in background
     * allows us to see the scene updated while focused on Angband.
     */
    void Awake()
    {
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
    }

    void Start()
    {
        player = PlayerObject.GetComponent<Unit>();
        LastModified = new System.DateTime[5];
        Doors = new List<Door>();
        
        player.map = this;
    }

    void Update()
    {
        if ((LastModified[(int)Files.DungeonMap] != File.GetLastWriteTime(DungeonMapFile)) ||
            (LastModified[(int)Files.WindowMap] != File.GetLastWriteTime(WindowMapFile)) ||
            (LastModified[(int)Files.FeatureMap] != File.GetLastWriteTime(FeatureMapFile)) ||
            (LastModified[(int)Files.LightingMap] != File.GetLastWriteTime(PlayerInfoFile)) ||
            (LastModified[(int)Files.OffsetValues] != File.GetLastWriteTime(OffsetFile)))
        {
            CreateMapData(); // If any files have been modified, update the arrays that contain the file contents
            GenerateMapVisual();
        }
    }
    /*
     * Parses the text files and stores them in the proper string array and string.
     * DungeonMap needs to be saved first, then the offset values, then FeatureMap
     * to determine which tiles to draw and their tile type. After features are stored
     * into tiles array, we can place the player, loot, and other map data.
     */
    void CreateMapData()
    {
        if (IsFileReady(DungeonMapFile)) // If file is not currently being written to by Angband, read the file
        {
            LastModified[(int)Files.DungeonMap] = File.GetLastWriteTime(DungeonMapFile);
            string tmp = File.ReadAllText(DungeonMapFile);

            // Only update the tiles array if the map has changed
            if (tmp.CompareTo(DungeonMapText) != 0)
            {
                DungeonMapText = tmp;
                DeleteMapVisual();
                DungeonMap = DungeonMapText.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);
                MapSizeY = DungeonMap.Length; // Height of the map is the number of lines of the DungeonMapFile

                // DungeonMap lines are different lengths for some reason. Find the longest and set that to the width of the map
                MapSizeX = 0;
                for (int i = 0; i < MapSizeY; i++)
                {
                    if (MapSizeX < DungeonMap[i].Length)
                    {
                        MapSizeX = DungeonMap[i].Length;
                    }
                }

                Tiles = new Tile[MapSizeX, MapSizeY];

                for (int y = 0; y < MapSizeY; y++)
                {
                    for (int x = 0; x < MapSizeX; x++)
                    {
                        try
                        {
                            Tiles[x, y] = new Tile();
                        }
                        catch (System.IndexOutOfRangeException e)
                        {
                            Debug.Log(e.Message);
                            // Set IndexOutOfRangeException to the new exception's InnerException.
                            throw new System.ArgumentOutOfRangeException("index parameter is out of range.", e);
                        }
                    }
                    
                    // Dungeon Map rows are not even which causes some OutOfRangeExceptions on some rows of Tiles[,]
                    for (int x = 0; x < DungeonMap[y].Length; x++) { 
                        try
                        {
                            switch (DungeonMap[y][x])
                            {
                                case '.':
                                    Tiles[x, y].TileType = (int)TileData.Floor;
                                    break;
                                case '#':
                                    Tiles[x, y].TileType = (int)TileData.Wall;
                                    break;
                                case '%':
                                    Tiles[x, y].TileType = (int)TileData.Wall; // used for ores in angband. Will be different tile type in future
                                    break;
                                case '+':
                                    Tiles[x, y].TileType = (int)TileData.Door;
                                    Doors.Add(new Door(x, y, false)); // + is a closed door
                                    break;
                                case '\'':
                                    Tiles[x, y].TileType = (int)TileData.Door;
                                    Doors.Add(new Door(x, y, true)); // ' is an open door
                                    break;
                                case '<':
                                    Tiles[x, y].TileType = (int)TileData.StairsUp;
                                    break;
                                case '>':
                                    Tiles[x, y].TileType = (int)TileData.StairsDown;
                                    break;
                                default:
                                    Tiles[x, y].TileType = (int)TileData.Empty;
                                    break;
                            }
                        }
                        catch (System.IndexOutOfRangeException e)
                        {
                            Debug.Log(e.Message);
                            // Set IndexOutOfRangeException to the new exception's InnerException.
                            throw new System.ArgumentOutOfRangeException("index parameter is out of range.", e);
                        }
                    }
                }
            }
        }

        if (IsFileReady(OffsetFile))
        {
            string[] offsetValues = File.ReadAllText(OffsetFile).Split(new string[] { " " }, System.StringSplitOptions.None);
            WindowOffsetX = int.Parse(offsetValues[0]);
            WindowOffsetY = int.Parse(offsetValues[1]);
        }

        // Sets which tiles are to be drawn.
        if (IsFileReady(WindowMapFile) && Tiles != null)
        {
            LastModified[(int)Files.WindowMap] = File.GetLastWriteTime(WindowMapFile);
            string tmp = File.ReadAllText(WindowMapFile);

            if (tmp.CompareTo(WindowMapText) != 0)
            {
                WindowMapText = tmp;
                WindowMap = WindowMapText.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);

                for (int y = 0; y < MapSizeY; y++)
                {
                    for (int x = 0; x < MapSizeX; x++)
                    {
                        try
                        {
                            Tiles[x, y].InWindow = false;
                        }
                        catch (System.IndexOutOfRangeException e)
                        {
                            Debug.Log(e.Message);
                            // Set IndexOutOfRangeException to the new exception's InnerException.
                            throw new System.ArgumentOutOfRangeException("index parameter is out of range.", e);
                        }
                    }
                }

                for (int y = 0; y < WindowMap.Length; y++)
                {
                    for (int x = 0; x < WindowMap[y].Length; x++)
                    {
                        if (WindowMap[y][x] != ' ')
                        {
                            try
                            {
                                Tiles[WindowOffsetX + x, WindowOffsetY + y].InWindow = true;
                            }
                            catch (System.IndexOutOfRangeException e)
                            {
                                Debug.Log(e.Message);
                                // Set IndexOutOfRangeException to the new exception's InnerException.
                                throw new System.ArgumentOutOfRangeException("index parameter is out of range.", e);
                            }
                        }
                    }
                }

                // After InWindow is updated for all tiles, delete tiles that should no longer be rendered
                DeleteOffWindowMapVisuals();
            }
        }

        if (IsFileReady(FeatureMapFile))
        {
            LastModified[(int)Files.FeatureMap] = File.GetLastWriteTime(FeatureMapFile);
            string tmp = File.ReadAllText(FeatureMapFile);

            if (tmp.CompareTo(FeatureMapText) != 0)
            {
                FeatureMapText = tmp;
                FeatureMap = FeatureMapText.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);

                for (int y = 0; y < FeatureMap.Length; y++)
                {
                    for (int x = 0; x < FeatureMap[y].Length; x++)
                    {
                        if (FeatureMap[y][x] == '@') // Finds location of player on map and updates player
                        {
                            player.tileX = WindowOffsetX + x;
                            player.tileY = WindowOffsetY + y;
                        }
                    }
                }
            }
        }
    }

    void GenerateMapVisual()
    {
        for (int x = 0; x < MapSizeX; x++)
        {
            for (int y = 0; y < MapSizeY; y++)
            {
                if (Tiles[x, y] != null)
                {
                    if (Tiles[x, y].InWindow)
                    {
                        if (!(Tiles[x, y].IsDrawn))
                        {
                            int worldX = x;
                            int worldY = MapSizeY - y;
                            TileType tt = TileTypes[Tiles[x, y].TileType];
                            GameObject go;
                            Tiles[x, y].IsDrawn = true;

                            if (tt.name == "Floor")
                            {
                                go = (GameObject)Instantiate(tt.tileVisualPrefab, new Vector3(worldX, 0, worldY), Quaternion.identity);
                            }
                            else if (tt.name == "Wall")
                            {
                                go = (GameObject)Instantiate(tt.tileVisualPrefab, new Vector3(worldX, 1, worldY), Quaternion.identity);
                            }
                            else if (tt.name == "Door")
                            {
                                go = (GameObject)Instantiate(tt.tileVisualPrefab, new Vector3(worldX, 0, worldY), Quaternion.identity);
                                //SetLayerRecursively(go, (int)Layers.Discovered);

                                // Rotates the door based on the floors and wall surrounding the door tile
                                if ((TileTypes[Tiles[x, y + 1].TileType].name == "Floor") && (Tiles[x, y + 1].InWindow))
                                {
                                    go.transform.Rotate(new Vector3(0, 0, 0));
                                }
                                else if ((TileTypes[Tiles[x, y - 1].TileType].name == "Floor") && (Tiles[x, y - 1].InWindow))
                                {
                                    go.transform.Rotate(new Vector3(0, 180, 0));
                                }
                                else if ((TileTypes[Tiles[x - 1, y].TileType].name == "Floor") && (Tiles[x - 1, y].InWindow))
                                {
                                    go.transform.Rotate(new Vector3(0, 270, 0));
                                }
                                else if ((TileTypes[Tiles[x + 1, y].TileType].name == "Floor") && (Tiles[x + 1, y].InWindow))
                                {
                                    go.transform.Rotate(new Vector3(0, 90, 0));
                                }
                                else if (TileTypes[Tiles[x, y + 1].TileType].name == "Wall" && TileTypes[Tiles[x, y - 1].TileType].name == "Wall")
                                {
                                    go.transform.Rotate(new Vector3(0, 90, 0));
                                }
                            }
                            else if (tt.name == "StairsUp")
                            {
                                go = (GameObject)Instantiate(tt.tileVisualPrefab, new Vector3(worldX, 1, worldY), Quaternion.identity);
                                if ((TileTypes[Tiles[x, y + 1].TileType].name == "Wall"))
                                {
                                    go.transform.Rotate(new Vector3(0, 0, 0));
                                }
                                else if ((TileTypes[Tiles[x, y - 1].TileType].name == "Wall"))
                                {
                                    go.transform.Rotate(new Vector3(0, 180, 0));
                                }
                                else if ((TileTypes[Tiles[x - 1, y].TileType].name == "Wall"))
                                {
                                    go.transform.Rotate(new Vector3(0, 270, 0));
                                }
                                else if ((TileTypes[Tiles[x + 1, y].TileType].name == "Wall"))
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
                }
            }
        }
    }

    public Vector3 TileCoordToWorldCoord(int x, int y)
    {
        return new Vector3(x, 0, MapSizeY - y);
    }

    private void DeleteOffWindowMapVisuals()
    {
        foreach (GameObject oldDungeon in GameObject.FindGameObjectsWithTag("Dungeon"))
        {
            int tileX = Mathf.RoundToInt(oldDungeon.transform.position.x);
            int tileY = Mathf.RoundToInt(MapSizeY - oldDungeon.transform.position.z);
            if (!Tiles[tileX,tileY].InWindow)
            {
                Tiles[tileX, tileY].IsDrawn = false;
                GameObject.Destroy(oldDungeon);
            }
        }
        foreach (GameObject oldDoors in GameObject.FindGameObjectsWithTag("Door"))
        {
            int tileX = Mathf.RoundToInt(oldDoors.transform.position.x);
            int tileY = Mathf.RoundToInt(MapSizeY - oldDoors.transform.position.z);
            if (!Tiles[tileX, tileY].InWindow)
            {
                Tiles[tileX, tileY].IsDrawn = false;
                GameObject.Destroy(oldDoors);
            }
        }
        foreach (GameObject oldStairs in GameObject.FindGameObjectsWithTag("Stairs"))
        {
            int tileX = Mathf.RoundToInt(oldStairs.transform.position.x);
            int tileY = Mathf.RoundToInt(MapSizeY - oldStairs.transform.position.z);
            if (!Tiles[tileX, tileY].InWindow)
            {
                Tiles[tileX, tileY].IsDrawn = false;
                GameObject.Destroy(oldStairs);
            }
        }
    }

    // Used only when you want to delete all game objects on the screen except for the player
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
}
