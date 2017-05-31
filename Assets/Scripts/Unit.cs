using UnityEngine;
using System;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

	// tileX and tileY represent the correct map-tile position
	// for this piece.  Note that this doesn't necessarily mean
	// the world-space coordinates, because our map might be scaled
	// or offset or something of that nature.  Also, during movement
	// animations, we are going to be somewhere in between tiles.
	public int tileX;
	public int tileY;
    public int awareness;

	public TileMapFile map;

    public GameObject playerIcon;

	// Our pathfinding info.  Null if we have no destination ordered.
	public List<Node> currentPath = null;

    // How far this unit can move in one turn. Note that some tiles cost extra.
    int moveSpeed = 1;
    float remainingMovement=1;
    private bool endTurn = false;

    public enum Direction
    {
        North = 1, South, East, West, NE, NW, SW, SE
    };

    public Direction lastDirection = 0;

    
    void Update()
    {
        // Draw our debug line showing the pathfinding!
        // NOTE: This won't appear in the actual game view.

        /*
		if(currentPath != null) {
			int currNode = 0;

			while( currNode < currentPath.Count-1 ) {

				Vector3 start = map.TileCoordToWorldCoord( currentPath[currNode].x, currentPath[currNode].y ) + 
					new Vector3(0, 0, -0.5f) ;
				Vector3 end   = map.TileCoordToWorldCoord( currentPath[currNode+1].x, currentPath[currNode+1].y )  + 
					new Vector3(0, 0, -0.5f) ;

				Debug.DrawLine(start, end, Color.red);

				currNode++;
			}
		}
        */
		// Have we moved our visible piece close enough to the target tile that we can
		// advance to the next step in our pathfinding?
		//if(Vector3.Distance(transform.position, map.TileCoordToWorldCoord( tileX, tileY )) < 0.1f)
			//AdvancePathing();

		// Smoothly animate towards the correct map tile.
		//transform.position = Vector3.Lerp(transform.position, map.TileCoordToWorldCoord( tileX, tileY ), 5f * Time.deltaTime);
	    
        Direction direction = 0;
        int rotation = (int)Mathf.RoundToInt(playerIcon.transform.rotation.eulerAngles.y / 90.0f);
        /*
        if ((Mathf.RoundToInt(tileX) == map.stairsUp.X && Mathf.RoundToInt(tileY) == map.stairsUp.Y) && map.mapLevel != 0)
        {
            transform.GetChild(1).transform.position = new Vector3(transform.GetChild(1).transform.position.x, 1.5f, transform.GetChild(1).transform.position.z);
        }
        else if (Mathf.RoundToInt(tileX) == map.stairsDown.X && Mathf.RoundToInt(tileY) == map.stairsDown.Y)
        {
            transform.GetChild(1).transform.position = new Vector3(transform.GetChild(1).transform.position.x, 0.5f, transform.GetChild(1).transform.position.z);
        }
        else
        {
            transform.GetChild(1).transform.position = new Vector3(transform.GetChild(1).transform.position.x, 1, transform.GetChild(1).transform.position.z);
        }   
        
        if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.Keypad2)) // Up
        {
            switch (rotation)
            {
                case 0:
                    direction = Direction.North;
                    break;
                case 1:
                    direction = Direction.West;
                    break;
                case 2:
                    direction = Direction.South;
                    break;
                case 3:
                    direction = Direction.East;
                    break;
                default:
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.Keypad7)) // NW
        {
            switch (rotation)
            {
                case 0:
                    direction = Direction.SE;
                    break;
                case 1:
                    direction = Direction.SW;
                    break;
                case 2:
                    direction = Direction.NW;
                    break;
                case 3:
                    direction = Direction.NE;
                    break;
                default:
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.Keypad9)) // NE
        {
            switch (rotation)
            {
                case 0:
                    direction = Direction.SW;
                    break;
                case 1:
                    direction = Direction.NW;
                    break;
                case 2:
                    direction = Direction.NE;
                    break;
                case 3:
                    direction = Direction.SE;
                    break;
                default:
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.Keypad8)) // Down
        {
            switch (rotation)
            {
                case 0:
                    direction = Direction.South;
                    break;
                case 1:
                    direction = Direction.East;
                    break;
                case 2:
                    direction = Direction.North;
                    break;
                case 3:
                    direction = Direction.West;
                    break;
                default:
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Keypad1)) // SW
        {
            switch (rotation)
            {
                case 0:
                    direction = Direction.NE;
                    break;
                case 1:
                    direction = Direction.SE;
                    break;
                case 2:
                    direction = Direction.SW;
                    break;
                case 3:
                    direction = Direction.NW;
                    break;
                default:
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.N) || Input.GetKeyDown(KeyCode.Keypad3)) // SE
        {
            switch (rotation)
            {
                case 0:
                    direction = Direction.NW;
                    break;
                case 1:
                    direction = Direction.NE;
                    break;
                case 2:
                    direction = Direction.SE;
                    break;
                case 3:
                    direction = Direction.SW;
                    break;
                default:
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.Keypad4)) // Left
        {
            switch (rotation)
            {
                case 0:
                    direction = Direction.West;
                    break;
                case 1:
                    direction = Direction.South;
                    break;
                case 2:
                    direction = Direction.East;
                    break;
                case 3:
                    direction = Direction.North;
                    break;
                default:
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.Keypad6)) // Right
        {
            switch (rotation)
            {
                case 0:
                    direction = Direction.East;
                    break;
                case 1:
                    direction = Direction.North;
                    break;
                case 2:
                    direction = Direction.West;
                    break;
                case 3:
                    direction = Direction.South;
                    break;
                default:
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            //map.GeneratePathTo(map.stairsDown.X, map.stairsDown.Y);
            //if (currentPath != null)
            //{
            //    NextTurn();
            //    endTurn = true;
            //}
            Debug.Log("Stairs Down : " + map.stairsDown.X + "," + map.stairsDown.Y);
            endTurn = true;
        }
        else if (Input.GetKeyDown(KeyCode.Period))
        {
            if (tileX == map.stairsDown.X && tileY == map.stairsDown.Y)
            {
                map.TakeStairsDown();
                endTurn = true;

            }
        }
        else if (Input.GetKeyDown(KeyCode.Comma))
        {
            if (map.stairsUp != null)
            {
                if (tileX == map.stairsUp.X && tileY == map.stairsUp.Y)
                {
                    map.TakeStairsUp();
                    endTurn = true;
                }
            }
        }
        switch (direction)
        {
            case Direction.North:
                if (map.UnitCanEnterTile(tileX, tileY + 1))
                {
                    tileY++;
                    endTurn = true;
                }
                break;
            case Direction.South:
                if (map.UnitCanEnterTile(tileX, tileY - 1))
                {
                    tileY--;
                    endTurn = true;
                }
                break;
            case Direction.East:
                if (map.UnitCanEnterTile(tileX - 1, tileY))
                {
                    tileX--;
                    endTurn = true;
                }
                break;
            case Direction.West:
                if (map.UnitCanEnterTile(tileX + 1, tileY))
                {
                    tileX++;
                    endTurn = true;
                }
                break;
            case Direction.NW:
                if (map.UnitCanEnterTile(tileX - 1, tileY + 1))
                {
                    tileX--;
                    tileY++;
                    endTurn = true;
                }
                break;
            case Direction.NE:
                if (map.UnitCanEnterTile(tileX + 1, tileY + 1))
                {
                    tileX++;
                    tileY++;
                    endTurn = true;
                }
                break;
            case Direction.SE:
                if (map.UnitCanEnterTile(tileX + 1, tileY - 1))
                {
                    tileX++;
                    tileY--;
                    endTurn = true;
                }
                break;
            case Direction.SW:
                if (map.UnitCanEnterTile(tileX - 1, tileY - 1))
                {
                    tileX--;
                    tileY--;
                    endTurn = true;
                }
                break;
            default:
                break;
        }

        if (map.tiles[tileX, tileY].tileType == 4)
        {
            map.OpenDoor(tileX, tileY);
        }
        transform.position = Vector3.Lerp(transform.position, map.TileCoordToWorldCoord(tileX, tileY), 5f * Time.deltaTime);
        if (direction > 0) lastDirection = direction;
        if (endTurn)
        {
            map.EndPlayersTurn();
            
            endTurn = false;
        }
        */
        map.GenerateMapVisual();
    }

    /*
	// Advances our pathfinding progress by one tile.
	void AdvancePathing() {
		if(currentPath==null)
			return;

		if(remainingMovement <= 0)
			return;

		// Teleport us to our correct "current" position, in case we
		// haven't finished the animation yet.
		transform.position = map.TileCoordToWorldCoord( tileX, tileY );

        // Get cost from current tile to next tile
        remainingMovement -= map.CostToEnterTile(currentPath[0].x, currentPath[0].y, currentPath[1].x, currentPath[1].y);
		// Move us to the next tile in the sequence
		tileX = currentPath[1].x;
		tileY = currentPath[1].y;
        
		// Remove the old "current" tile from the pathfinding list
		currentPath.RemoveAt(0);
		
		if(currentPath.Count == 1) {
			// We only have one tile left in the path, and that tile MUST be our ultimate
			// destination -- and we are standing on it!
			// So let's just clear our pathfinding info.
			currentPath = null;
		} 
    }*/


    // The "Next Turn" button calls this.
    public void NextTurn() {
        // Make sure to wrap-up any outstanding movement left over.
        //AdvancePathing();
		
		// Reset our available movement points.
		remainingMovement = moveSpeed;
	} 
    
}
