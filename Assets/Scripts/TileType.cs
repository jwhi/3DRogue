using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileType {

	public string name;
	public GameObject tileVisualPrefab;
    public GameObject tileVisualPrefabAlt;
	public bool isWalkable = true;
    public char Character;
    public Color color;
}
