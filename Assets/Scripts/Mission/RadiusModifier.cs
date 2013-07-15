using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

[assembly: ExtensionAssembly()]

[ProcessingPriority(1)]
public class RadiusModifier : BuildGraph.IProcessGrid {
	#region IProcessGrid implementation
	//Post process the grid
	public void ProcessGrid (BuildGraph builder)
	{
		//Make a list of nodes to flag as unwalkable
		//we can't immediately update them or the
		//whole thing would go horribly wrong as it
		//scanned its own output!
		var unwalkable = new List<BuildGraph.GridPosition>();
		//Run through the grid
		for(var x = 0; x < builder.width; x ++)
		{
			for(var y = 0; y < builder.height; y++)
			{
				//Get a current position
				var currentPosition = new BuildGraph.GridPosition { x = x, y = y };
				//Get all of the neighbours within 2 grid units and see if any
				//of them are not walkable
				if(builder.GetNeighbours(currentPosition, 2).Select(cell=>builder.GetCell(cell)).Any(gc=>!gc.walkable))
				{
					//If so add this cell to the unwalkable
					//list
					unwalkable.Add(currentPosition);
				}
			}
		}
		//Update the map
		foreach(var unwalk in unwalkable)
		{
			builder.GetCell(unwalk).walkable = false;
		}
		
		
		
	}
	#endregion
	
	

	
}
