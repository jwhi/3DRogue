using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ClickableTile : MonoBehaviour {

	public int tileX;
	public int tileY;
	public TileMap map;

	void OnMouseUp() {

		if(EventSystem.current.IsPointerOverGameObject())
			return;

		//map.GeneratePathTo(tileX, tileY);
	}

}
