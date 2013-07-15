using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;

[ProcessingPriority(2)]
public class PinchPointIdentifier : BuildGraph.IProcessGrid, BuildGraph.IVisualizeGrid
{
	
	public class PinchPointGrid
	{
		public int[,] cells;
	    public List<BuildGraph.GridPosition> pinchPoints;
	}

    public class PinchInfo
    {
        public BuildGraph.GridPosition position;
        public int pinch;
    }

    #region IProcessGrid implementation
	public void ProcessGrid (BuildGraph builder)
	{

		var grid = builder.cells.Get<PinchPointGrid>();
	    var points = new List<PinchInfo>();
		grid.cells = new int[builder.width, builder.height];
		foreach(var cell in builder.allWalkableCells)
		{
			grid.cells[cell.x, cell.y] = builder.GetNeighbours(cell, 10).Count(c=>!builder.GetCell(c).walkable);
		    points.Add(new PinchInfo {
		                                 pinch = grid.cells[cell.x, cell.y],
		                                 position = cell
		                             });
		}
	    grid.pinchPoints = points.OrderByDescending(p => p.pinch).GroupBy(p=>Mathf.FloorToInt(p.position.x/20) + Mathf.FloorToInt(p.position.y/20)*2000).Select(grp=>grp.First()).Select(p => p.position).Take(20).ToList();

	}
	#endregion

    private static PinchPointGrid grid;
    public void Visualize(BuildGraph builder, BuildGraph.GridPosition position)
    {
        if(grid == null)
            grid = builder.cells.Get<PinchPointGrid>();
        if (grid.pinchPoints.Contains(position))
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(builder.GetWorldPosition(position) + Vector3.up*2.5f, Vector3.one*Mathf.Lerp(0.5f, 1f, grid.cells[position.x, position.y]/grid.cells[grid.pinchPoints[0].x, grid.pinchPoints[0].y]));
        }
    }
}

