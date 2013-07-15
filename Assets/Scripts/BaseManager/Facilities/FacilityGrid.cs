using UnityEngine;
using System.Collections;

public class FacilityGrid : MonoBehaviour {

	public GameObject getObjectAtPosition(int x, int y)
	{
		GridPosition[] positions = gameObject.GetComponentsInChildren<GridPosition>();
		
		foreach (GridPosition pos in positions)
		{
			if (pos.grid_x == x && pos.grid_y == y)
				return pos.gameObject;
		}
		return null;
	}
}
