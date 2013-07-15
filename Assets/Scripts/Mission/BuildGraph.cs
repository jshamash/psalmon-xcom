using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;



[AttributeUsage(AttributeTargets.Assembly)]
public class ExtensionAssembly : Attribute
{
}


[AttributeUsage(AttributeTargets.Class)]
public class ProcessingPriority : Attribute
{
	public int priority;
	public ProcessingPriority(int newPriority)
	{
		priority = newPriority;
	}
}




public class BuildGraph : MonoBehaviour
{
	
    public LayerMask culling = new LayerMask();
	
	//Returns an iterator of all cells of the gridmap
	public IEnumerable<GridPosition> allCells
	{
		get
		{
			for(var x = 0; x < width; x++)
			{
				for(var y = 0; y < height; y++)
				{
					yield return new GridPosition { x = x, y = y };
				}
			}
		}
			
	}
	
	//returns an iterator of all cells that the player is available to walk into
    public IEnumerable<GridPosition> allWalkableCells
    {
        get
        {
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var pos = new GridPosition { x = x, y = y };
                    if (GetCell(pos).walkable)
                        yield return pos;
                }
            }
        }

    }
	
	
	//Interface to process a grid after 
	//calculation
	public interface IProcessGrid
	{
		void ProcessGrid(BuildGraph builder);
	}

    public interface IVisualizeGrid
    {
        void Visualize(BuildGraph builder, GridPosition position);
    }
	

	//List of active processors
    static List<IProcessGrid> processors;
	
	static bool _initialized;
	
	
	static void Initialize()
	{
		if(_initialized)
			return;
		_initialized = true;
		//Get a list of suitable processors from the system	
		processors = AppDomain.CurrentDomain
			.GetAssemblies()
			.Where(a=>a.IsDefined(typeof(ExtensionAssembly), true))
			.SelectMany(asm=>asm.GetTypes()
					.Where(t=>typeof(IProcessGrid).IsAssignableFrom(t) && t.IsDefined(typeof(ProcessingPriority), true))
			)
			.Select(t=>Activator.CreateInstance(t) as IProcessGrid).ToList();
	}
	
	//Representation of a grid cell
	public class GridCell
	{
		public bool walkable;
		public float height;
	}
	
	//Size in world units of a cell
	public float cellSize = 0.5f;
	//Width and height of the map in cells
	public int width, height;
	//Active map of the world
	public GridCell[,] cells;
	//Bounds for scanning
	Bounds bounds;
	//Top left corner of the bounds
	Vector3 topLeftCorner;
	//Mask indicating the walkable layers
	public LayerMask walkableLayer;
	
	//Singleton instance
	public static BuildGraph instance;
	
	void Awake()
	{
		instance = this;
		renderer.enabled = false;
	}
	
	
	//Build a navmesh
	public void Scan()
	{
		Initialize();
		
		//Scan the bounds of the model and create a grid
		bounds = renderer.bounds;
		//Work out the top left corner
		topLeftCorner = bounds.center - bounds.extents + new Vector3(0, bounds.size.y, 0);
		//Calculate the dimensions of the grid map
		width = Mathf.RoundToInt(bounds.size.x / cellSize);
		height = Mathf.RoundToInt(bounds.size.z / cellSize);
		//Create the grid map
		cells = new GridCell[width, height];
		//Scan for walkable terrain in each cell
		for(var x = 0; x < width; x ++)
		{
			for(var y = 0; y < height; y++)
			{
				//Get the position for a ray
				var currentPosition = topLeftCorner + new Vector3(x * cellSize, 0, y * cellSize);
				RaycastHit hit;
				//Create a cell for the grid
				var cell = new GridCell();
				cells[x, y] = cell;
				//Cast the ray
				if(Physics.Raycast(currentPosition, -Vector3.up, out hit, bounds.size.y))
				{
					//The height of the highest item in the cell
					cell.height = hit.point.y;
					//Test if the thing we hit was walkable
					if(((1 << hit.collider.gameObject.layer) & walkableLayer) != 0)
					{
						//Flag the cell as walkable
						cell.walkable = true;
					}
					//Test lowest possible height
					if(cell.height<=27){
						//Flag the cell as walkable
						cell.walkable = true;
					}
					//Try to make crawler not part of obstacles
					if(hit.transform.tag=="Crawler"){
						cell.walkable = true;
					}
				}
				
			}
		}
		
		//Run post processing on the grid
		foreach(var processor in processors.OrderBy(p=>p.GetType().IsDefined(typeof(ProcessingPriority), true) ? 
			(p.GetType().GetCustomAttributes(typeof(ProcessingPriority), true)[0] as ProcessingPriority).priority : 1000)
			)
			
		{
			processor.ProcessGrid(this);
		}
		
	}
	
	
	public Vector3 checkIfWalkable(Vector3 wantedPosition){
		GridPosition tempPosition = GetGridPosition(wantedPosition);
		/*if(cells[tempPosition.grid_x,tempPosition.grid_y].walkable){
			
		}*/
		return new Vector3(0,0,0);
	}
	
	//Represents a position on the grid
	public struct GridPosition
	{
		//Return a debugging string
		public override string ToString ()
		{
			return string.Format ("[GridPosition {0}, {1}]", x, y);
		}
		
		//Have a static one that is 0,0 for checking
		public static GridPosition zero = new GridPosition {x = 0, y = 0};
		//Coordinates
		public int x;
		public int y;
		
		//Manhattan distance to another cell
		public int Distance(GridPosition other)
		{
			return Mathf.Abs(other.x - x) + Mathf.Abs(other.y - y);
		}
		
		//Add two grid positions
		public static GridPosition operator + (GridPosition p1, GridPosition position)
		{
			return new GridPosition { x = p1.x + position.x, y = p1.y + position.y };
		}
		
		//Check if two grid positions are equal
		public override bool Equals (object obj)
		{
			if(!(obj is GridPosition))
				return false;
			var gp = (GridPosition)obj;
			return gp.x == x && gp.y == y;
		}
		
		//Get a hash code for the grid position
		public override int GetHashCode ()
		{
			return x.GetHashCode() ^ y.GetHashCode();
		}
		
		//Equality operator for two grid positions
		public static bool operator == (GridPosition p1, GridPosition p2)
		{
			return p1.Equals(p2);
		}
		
		//Inequality operator for two grid positions
		public static bool operator != (GridPosition p1, GridPosition p2)
		{
			return !p1.Equals(p2);
		}
		
	} 
	
	
	//Get all of the valid neighbourds of a particular grid position
	public List<GridPosition> GetNeighbours(GridPosition gridPosition, int distance = 1)
	{
		var neighbours = new List<GridPosition>();
		//Get the neighbours within x distance	
		for(var x = -distance; x <= distance; x ++ )
		{
			for(var y = -distance; y <= distance; y ++)
			{
				var currentPosition = gridPosition + new GridPosition { x = x, y = y };		
				//Verify that that the cell is in the actual map
				if(currentPosition.x >= 0 && currentPosition.y >= 0 && currentPosition.x < width && currentPosition.y < height)
					neighbours.Add(currentPosition);
			}
		}
		return neighbours;
	}

	//Convert a world position into a grid position
	public GridPosition GetGridPosition(Vector3 worldPosition)
	{
		worldPosition -= topLeftCorner;
		return new GridPosition { x = Mathf.FloorToInt(worldPosition.x / cellSize), y = Mathf.FloorToInt(worldPosition.z / cellSize) };
	}
	//Get the cell at a particular grid position
	public GridCell GetCell(GridPosition gridPosition)
	{
		return cells[gridPosition.x, gridPosition.y];
	}
	
	//Convert a grid position into a world position on the navmesh
	public Vector3 GetWorldPosition(GridPosition gridPosition)
	{
		var worldPosition = new Vector3(gridPosition.x * cellSize, GetCell(gridPosition).height, gridPosition.y * cellSize);
		return worldPosition + new Vector3(topLeftCorner.x, 0, topLeftCorner.z);
	}
	
	//A* Node
	public class Node
	{
		public float g_score = float.MaxValue;
		public float f_score;
		//The parent that yielded the current g_score
		public GridPosition cameFrom = new GridPosition();
	}
	
	public interface IAmIntelligent
	{
		float GetWeight(GridPosition from, GridPosition to);
	}
	
	
	//Find a path between two positions
	public List<Vector3> SeekPath(Vector3 startPosition, Vector3 endPosition, IAmIntelligent intelligence = null)
	{
		
		//Scan if we don't have a map yet (did'nt scan yet)
		if(cells == null)
			Scan();
		
		//Start and end in grid coordinates
		var start = GetGridPosition(startPosition);
		var end = GetGridPosition(endPosition);
		endPosition.y = GetWorldPosition(end).y;
			
		//Set of considered nodes
		var closedSet = new Dictionary<GridPosition, Node>();
		//Set of all nodes processed (so we can rebuild the path)
		var map = new Dictionary<GridPosition, Node>();
		//Set of nodes yet to be considered
		var openSet = new Dictionary<GridPosition, Node>();
		
		//Set the f and g score for the start node
			//Create a node for the start
			var startNode = new Node { f_score = end.Distance(start) };
			//No cost
			startNode.g_score = 0;
		
		//Add the start node to the map and opensets 
			map[start] = startNode;
			openSet[start] = startNode;
		
		
		//While we have nodes in our openSet
		while(openSet.Count > 0 )
		{	
			//Get the best possible node
			var best = openSet.Aggregate((c,n)=>c.Value.f_score < n.Value.f_score ? c : n);
			
			//Remove it from the open set and add it to the closed set
				openSet.Remove(best.Key);
				closedSet[best.Key] = best.Value;
			
			//Have we reached the target? if yes, build route
			if(best.Key == end)
			{
				//Recreate the path
					var path = new List<Vector3>();
					var scan = best.Value;
				
				//Add the actual end position
					path.Add(endPosition);
				//Scan backwards from the end of the path
				//until scan.cameFrom is 0
				while(scan != null && scan.cameFrom != GridPosition.zero)
				{
					//Add the current node to the START of the path thereby reversing the direction of the list
						path.Insert(0, GetWorldPosition(scan.cameFrom));
					//Get the next node
						scan = map[scan.cameFrom];
				}
				//Update the caller
				return path;
			}
			
			//Get all of the neighbours of the current cell that are walkable
			foreach(var cell in GetNeighbours(best.Key).Where(c=>GetCell(c).walkable))
			{
				//Have we processed this already?
					if(closedSet.ContainsKey(cell))
					{
						continue;
					}
				
				//Work out the cost to the neighbour via the current node
				var tentativeGScore = best.Value.g_score + GetWeightOfMovingBetween(best.Key, cell);
				if(intelligence != null)
					tentativeGScore += intelligence.GetWeight(best.Key, cell);
				Node currentNode;
				//Is the neighbour already open?
				if(!openSet.TryGetValue(cell, out currentNode))
				{
					//If not then create a node for it
					//this will have a maximum g_score
					currentNode = new Node();
					//Add it to the map and the open set
					map[cell] = currentNode;
					openSet[cell] = currentNode;
				}
				//Is the new g_score lower than the
				//current one?
				if(currentNode.g_score > tentativeGScore)
				{
					//Update the openset node with this
					//new better way of getting there
					currentNode.g_score = tentativeGScore;
					currentNode.cameFrom = best.Key;
					currentNode.f_score = tentativeGScore + cell.Distance(end);
				}
				
			}
			
			
			
		}
		 return null;
		
	}
	
	//Calculate the cost of moving between two nodes
	//later this can take into consideration more than
	//just distane
	float GetWeightOfMovingBetween(GridPosition p1, GridPosition p2)
	{
		return 1;
	}
	
	
	//Draw a representation of the map
	void OnDrawGizmosSelected()
	{
		if(cells == null || width == 0 || height == 0)
			return;
		
		for(var x = 0; x < width; x++)
		{
			for(var y = 0; y < height; y++)
			{
				var cell = cells[x,y];
				Gizmos.color = cell.walkable ? Color.green : Color.red;
				var drawPosition = topLeftCorner + new Vector3(((float)x+0.5f) * cellSize, 0, ((float)y + 0.5f) * cellSize);
				drawPosition.y = cell.height;
				Gizmos.DrawCube(drawPosition, 
					Vector3.one * cellSize * 0.7f);
                foreach (var visualizer in processors.Select(p => p as IVisualizeGrid).Where(p => p != null))
                {
                    visualizer.Visualize(this,
                                         new GridPosition
                                             {
                                                 x = x,
                                                 y = y
                                             });
			    
				}
			}
		}
	}
	
	//Checks if position is correct.  If not, gets the closest position
	public Vector3 getAvailablePosition(Vector3 desiredPos){
		GridPosition desiredGridPos = GetGridPosition(desiredPos);
		
		if(cells[desiredGridPos.x,desiredGridPos.y].walkable){
			return desiredPos;	
		}else{
			width = Mathf.RoundToInt(bounds.size.x / cellSize);
			height = Mathf.RoundToInt(bounds.size.z / cellSize);
			int x = desiredGridPos.x,y = desiredGridPos.y, increaser=1;
			bool finishedLoop = false;
			while(!finishedLoop){
				if(x+increaser < width){
					if(cells[x+increaser,y].walkable){
						desiredGridPos.x = x+increaser;
						return GetWorldPosition(desiredGridPos);
					}
				}
				if(x-increaser>=0){
					if(cells[x-increaser,y].walkable){
						desiredGridPos.x = x-increaser;
						return GetWorldPosition(desiredGridPos);
					}
				}
				if(y+increaser < height){
					if(cells[x,y+increaser].walkable){
						desiredGridPos.y = y+increaser;
						return GetWorldPosition(desiredGridPos);
					}
				}
				if(y-increaser>=0){
					if(cells[x,y-increaser].walkable){
						desiredGridPos.y = y-increaser;
						return GetWorldPosition(desiredGridPos);
					}
				}
				increaser++;
				if(increaser==50)
					finishedLoop=true;
			}
		}
		return desiredPos;
	}
	
}
