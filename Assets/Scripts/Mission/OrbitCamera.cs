using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[AddComponentMenu("Camera-Control/Terrain Avoiding Orbit-Follow")]
public class OrbitCamera : MonoBehaviour {
	
	public enum ControlModes
	{
		none,
		leftButton, 
		rightButton
	}
	
	/// <summary>
	/// The target object being followed.
	/// </summary>
	public GameObject target;
	public float x=0.2f;
	public float y=0.3f;
	public float z = 0.9f;
	
	public float xSpeed = 2.5f;
	public float ySpeed = 5f;
	public float zSpeed = 5f;
	
	public float cutWeighting = 1f;
	public float riseWeighting = 4f;
	public float zoomWeighting = 1.5f;
	
	[HideInInspector]
	public float xMin = 0;
	[HideInInspector]
	public float xMax = 360;
	public float yMin = 0;
	public float yMax = 100;
	public float zMin = 6f;
	public float zMax = 20;
	public float dampingTime = 1f;
	
	/// <summary>
	/// Camera Control
	/// </summary>
	public ControlModes ControlMode = ControlModes.leftButton;
	
	public Vector3 targetOffset = new Vector3(0,0.7f,0);
	
	private Vector3 lookPosition
	{
		get
		{
			return target.transform.position + targetOffset;
		}
	}

	[HideInInspector]
	public Vector3 velocity;
	private Vector3 requestedPosition;
	
	/// <summary>
	/// Holds the different trial results and actions
	/// </summary>
	public class ActionTest
	{
		private OrbitCamera _oc;
		public ActionTest(OrbitCamera oc)
		{
			_oc = oc;
		}
		public Action<ActionTest> action;
		public float distanceMoved;
		public bool success;
		public Vector3 targetPosition;
		public Action chosen;
		public bool Test()
		{
			RaycastHit hit;
			var relative = targetPosition - _oc.lookPosition;
			return success = !Physics.Raycast (_oc.lookPosition, relative, out hit, relative.magnitude + 0.5f);
		}
		public bool Test(GameObject target, ref RaycastHit hit)
		{
			var relative = targetPosition - _oc.lookPosition;
			return success = !Physics.Raycast (_oc.lookPosition, relative, out hit, relative.magnitude + 0.5f);
		}
	}
	
	//The actions that will be tested
	private List<ActionTest> _actions = new List<ActionTest>();
	
	void Awake()
	{
		_actions.Add(new ActionTest(this) { action = MoveCloser });
		_actions.Add(new ActionTest(this) { action = RiseUp });
		_actions.Add(new ActionTest(this) { action = SwitchAngle90 });
	}
	
	// Use this for initialization
	void Start () {
		//Remove a rigid body if we have one
		if(rigidbody != null)
		{
			Component.Destroy(rigidbody);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
		if((ControlMode == ControlModes.leftButton && Input.GetMouseButton(0)) || (ControlMode == ControlModes.rightButton && Input.GetMouseButton(1)))
		{
			x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
			y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
			
		}
		if(ControlMode != ControlModes.none)
		{
			z -= Input.GetAxis("Mouse ScrollWheel") * zSpeed * 0.02f;
		}
		x = x > 1 ? x - 1 :  (x < 1 ? x + 1 : x);
		y = Mathf.Clamp01(y);
		z = Mathf.Clamp01(z);
	}
	
	void LateUpdate()
	{
		//Test the points around the camera
		
		var rotation = Quaternion.Euler(y * (yMax - yMin) + yMin, x * (xMax - xMin) + xMin,0);
		requestedPosition = Vector3.SmoothDamp(transform.position, rotation * new Vector3(0,0,-( z * (zMax - zMin) + zMin )) + lookPosition, ref velocity, dampingTime);
		//Ensure the camera is always above the terrain
		if(Terrain.activeTerrain != null) requestedPosition.y = Mathf.Clamp(requestedPosition.y, Terrain.activeTerrain.SampleHeight(requestedPosition)+1, 10000);
		var relative = requestedPosition - lookPosition;
		RaycastHit hit;
		//Check if the desired camera position is a problem
		if(Physics.Raycast (lookPosition, relative, out hit, relative.magnitude + 0.5f))
		{
			//Try each action
			foreach(var a in _actions)
			{
				a.action(a);
			}
			var best = _actions.Where(a=>a.success).OrderBy(a=>a.distanceMoved).FirstOrDefault();
			if(best == null)
			{
				best = _actions.OrderBy(a=>a.distanceMoved).First();
			}
			
			
			
			if(best.chosen != null)
			{
				best.chosen();
			}
			
			requestedPosition = Vector3.SmoothDamp(transform.position, best.targetPosition, ref velocity, dampingTime/2);
			
		}
		
		transform.position = requestedPosition;
		var relativePos = lookPosition - transform.position;
		transform.rotation = Quaternion.LookRotation(relativePos);
		
	}
	
	//Abrupt camera change
	void SwitchAngle90(ActionTest t)
	{
		var rotation = Quaternion.Euler(y * (yMax - yMin) + yMin, (x+0.25f) * (xMax - xMin) + xMin,0);
		t.targetPosition = rotation * new Vector3(0,0,-( z * (zMax - zMin) + zMin )) + lookPosition;
		if(Terrain.activeTerrain != null) t.targetPosition.y = Mathf.Clamp(t.targetPosition.y, Terrain.activeTerrain.SampleHeight(t.targetPosition)+1, 10000);
		t.chosen = ()=>{ x += 0.25f;  transform.position = t.targetPosition;};
		t.Test();
		if(!t.success)
		{
			rotation = Quaternion.Euler(y * (yMax - yMin) + yMin, (x-0.25f) * (xMax - xMin) + xMin,0);
			t.targetPosition = rotation * new Vector3(0,0,-( z * (zMax - zMin) + zMin )) + lookPosition;
			if(Terrain.activeTerrain != null) t.targetPosition.y = Mathf.Clamp(t.targetPosition.y, Terrain.activeTerrain.SampleHeight(t.targetPosition)+1, 10000);
			t.chosen = ()=> { x -= 0.25f; transform.position = t.targetPosition;};
			t.Test();
		}
		if(!t.success)
		{
			rotation = Quaternion.Euler(y * (yMax - yMin) + yMin, (x+0.35f) * (xMax - xMin) + xMin,0);
			t.targetPosition = rotation * new Vector3(0,0,-( z * (zMax - zMin) + zMin )) + lookPosition;
			if(Terrain.activeTerrain != null) t.targetPosition.y = Mathf.Clamp(t.targetPosition.y, Terrain.activeTerrain.SampleHeight(t.targetPosition)+1, 10000);
			t.chosen = ()=> { x += 0.35f; transform.position = t.targetPosition;};
			t.Test();
		}
		if(!t.success)
		{
			rotation = Quaternion.Euler(y * (yMax - yMin) + yMin, (x-0.35f) * (xMax - xMin) + xMin,0);
			t.targetPosition = rotation * new Vector3(0,0,-( z * (zMax - zMin) + zMin )) + lookPosition;
			if(Terrain.activeTerrain != null) t.targetPosition.y = Mathf.Clamp(t.targetPosition.y, Terrain.activeTerrain.SampleHeight(t.targetPosition)+1, 10000);
			t.chosen = ()=> { x -= 0.35f; transform.position = t.targetPosition;};
			t.Test();
		}
		t.distanceMoved = (transform.position - t.targetPosition).magnitude * cutWeighting;
	}
	
	//Get closer
	void MoveCloser(ActionTest t)
	{
		var rotation = Quaternion.Euler(y * (yMax - yMin) + yMin, x * (xMax - xMin) + xMin,0);
		var relative = requestedPosition - lookPosition;
		RaycastHit hit;
		Physics.Raycast (lookPosition, relative, out hit, relative.magnitude + 0.5f);
		var distance = Mathf.Clamp01((hit.distance - zMin)/(zMax - zMin) - 0.01f);
		t.targetPosition = rotation * new Vector3(0,0,-( distance * (zMax - zMin) + zMin )) + lookPosition;
		if(Terrain.activeTerrain != null) t.targetPosition.y = Mathf.Clamp(t.targetPosition.y, Terrain.activeTerrain.SampleHeight(t.targetPosition)+1, 10000);
		t.chosen = () => {transform.position = t.targetPosition; z = z - 0.01f;};
		t.Test();
		t.distanceMoved = (transform.position - t.targetPosition).magnitude * zoomWeighting;
		
	}
	
	//Rise above an object and look downwards
	void RiseUp(ActionTest t)
	{
		t.targetPosition = requestedPosition;
		t.success = false;
		var offset = 0.002f;
		while(!t.success && y+offset < 1)
		{
			var rotation = Quaternion.Euler(y+offset * (yMax - yMin) + yMin, x * (xMax - xMin) + xMin,0);
			t.targetPosition = rotation * new Vector3(0,0,-( z * (zMax - zMin) + zMin )) + lookPosition;
			if(Terrain.activeTerrain != null) t.targetPosition.y = Mathf.Clamp(t.targetPosition.y, Terrain.activeTerrain.SampleHeight(t.targetPosition)+1, 10000);
			t.chosen = () => {transform.position = t.targetPosition; y = y + offset; };
			t.Test();
			t.distanceMoved = (transform.position - t.targetPosition).magnitude*riseWeighting;
			offset = offset + 0.02f;
		}
				
		
	}

}