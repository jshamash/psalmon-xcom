using UnityEngine;
using System.Collections;

public class BuildFacilities : MonoBehaviour {
	
	public baseMenu menu;
	public FacilityGrid grid;
	
	// Layer containing grid items
	public LayerMask placementLayerMask;
	
	// Materials for grid squares
	private Material originalMat;
	private Material neighbourMat;
	public Material hoverMat;
	public Material clearedMat;
	public Material unclickableMat;
	
	private GameObject lastHitObj;
	private GameObject lastHitNeighbour;
	
	private Base activeBase;
	
	void drawGrid() {
		for (int y = 0; y < 5; y++)
		{
			for (int x = 0; x < 5; x++)
			{				
				Facility facility = activeBase.getFacility(x, y);
				GameObject o = grid.getObjectAtPosition(x, y);
				
				if (facility != null)
				{
					o.tag = "PlacementPlane_taken";
					o.renderer.material = facility.getMaterial();
				}
				else
				{
					// Is it part two of a facility?
					if (x > 0)
					{
						facility = activeBase.getFacility(x-1, y);
						if (facility != null && facility.isLarge())
						{
							// Yes -- fill this grid space and move on
							o.tag = "PlacementPlane_taken";
							o.renderer.material = facility.getExtensionMaterial();
							continue;
						}
					}
					o.tag = "PlacementPlane_open";
					o.renderer.material = clearedMat;
				}
			}
		}
	}
	// Update is called once per frame
	void Update () {
		if (activeBase != menu.currentBase)
		{
			activeBase = menu.currentBase;
			drawGrid();
		}
		if (menu.isFacilitiesMode())
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 1000, placementLayerMask))
			{
				// Our ray hit something in the placement layer
				if (lastHitObj)
				{
					lastHitObj.renderer.material = originalMat;
				}
				if (lastHitNeighbour)
				{
					lastHitNeighbour.renderer.material = neighbourMat;
				}
				
				// Set the last hit object to what we just hit. Save its material so it can be restored later.
				lastHitObj = hit.collider.gameObject;
				originalMat = lastHitObj.renderer.material;
				
				// If it has a neighbour, store it and save its material.
				int x = lastHitObj.GetComponent<GridPosition>().grid_x;
				int y = lastHitObj.GetComponent<GridPosition>().grid_y;
				lastHitNeighbour = grid.getObjectAtPosition(x+1, y);
				if (lastHitNeighbour)
				{
					neighbourMat = lastHitNeighbour.renderer.material;
				}
				
				// Add hover effect if this one is clickable and a facility is selected.
				if (lastHitObj.tag == "PlacementPlane_open" && menu.getSelectedFacility() != null)
				{
					lastHitObj.renderer.material = hoverMat;
					
					// if we're on a 2-space facility
					if (menu.getSelectedFacility().isLarge())
					{
						// Unclickable if the neighbour is not available
						if (lastHitNeighbour == null || lastHitNeighbour.tag != "PlacementPlane_open")
							lastHitObj.renderer.material = unclickableMat;
						else
						{
							// Unclickable if nothing is adjacent
							if (!hasAdjacentFacility(lastHitObj) && !hasAdjacentFacility(lastHitNeighbour))
							{
								lastHitObj.renderer.material = unclickableMat;
								lastHitNeighbour.renderer.material = unclickableMat;
							}
							else
							{
								lastHitNeighbour.renderer.material = hoverMat;
							}
						}
					}
					else
					{
						if (!hasAdjacentFacility(lastHitObj))
							lastHitObj.renderer.material = unclickableMat;
					}
				}
			}
			else
			{
				// Our ray didn't hit anything
				if (lastHitObj)
				{
					lastHitObj.renderer.material = originalMat;
					lastHitObj = null;
				}
				if (lastHitNeighbour)
				{
					lastHitNeighbour.renderer.material = neighbourMat;
					lastHitNeighbour = null;
				}
			}
			
			if (Input.GetMouseButtonDown(0) && lastHitObj)
			{
				// A grid spot was clicked.
				if (lastHitObj.tag == "PlacementPlane_open")
				{
					// We can place something here.
					Facility addedFac = menu.getSelectedFacility();
					if (addedFac != null)
					{
						// Can't place more than one hangar
						if (addedFac.getName () == "Hangar" && activeBase.facilityCount("Hangar") > 0)
						{
							menu.cantBuildHangar();
						}
						
						// if it's large and has a legit neighbour, place the facility and its neighbour
						// if it's not large, just place the facility.
						else if (addedFac.isLarge())
						{
							if (lastHitNeighbour != null && lastHitNeighbour.tag == "PlacementPlane_open")
							{
								// Make sure there is at least one adjacent facility
								if (hasAdjacentFacility(lastHitObj) || hasAdjacentFacility(lastHitNeighbour))
								{
									// Update base
									GridPosition pos = lastHitObj.GetComponent<GridPosition>();
									if (!activeBase.addFacility(addedFac, pos.grid_x, pos.grid_y))
									{
										// Didn't work
										menu.cantAffordFacility();
									}
									else
									{
										// Place first half
										lastHitObj.renderer.material = addedFac.getMaterial();
										lastHitObj.tag = "PlacementPlane_taken";
										originalMat = lastHitObj.renderer.material;
										
										// Place second half
										lastHitNeighbour.renderer.material = addedFac.getExtensionMaterial();
										lastHitNeighbour.tag = "PlacementPlane_taken";
										neighbourMat = lastHitNeighbour.renderer.material;
									}
								}
							}
						}
						else
						{
							if (hasAdjacentFacility(lastHitObj))
							{
								// Update base
								GridPosition pos = lastHitObj.GetComponent<GridPosition>();
								if (!activeBase.addFacility(addedFac, pos.grid_x, pos.grid_y))
								{
									// Didn't work, base not updated.
									menu.cantAffordFacility();
								}
								else
								{
									lastHitObj.renderer.material = addedFac.getMaterial();
									lastHitObj.tag = "PlacementPlane_taken";
									originalMat = lastHitObj.renderer.material;
								}
							}
						}
					}
				}
			}
		}			
	}
	
	private bool hasAdjacentFacility(GameObject obj)
	{
		int x = obj.GetComponent<GridPosition>().grid_x;
		int y = obj.GetComponent<GridPosition>().grid_y;
		
		GameObject adjacentObj;
		if ((adjacentObj = grid.getObjectAtPosition(x+1, y)) != null && adjacentObj.tag == "PlacementPlane_taken")
			return true;
		if ((adjacentObj = grid.getObjectAtPosition(x-1, y)) != null && adjacentObj.tag == "PlacementPlane_taken")
			return true;
		if ((adjacentObj = grid.getObjectAtPosition(x, y+1)) != null && adjacentObj.tag == "PlacementPlane_taken")
			return true;
		if ((adjacentObj = grid.getObjectAtPosition(x, y-1)) != null && adjacentObj.tag == "PlacementPlane_taken")
			return true;
		
		return false;
	}
}
