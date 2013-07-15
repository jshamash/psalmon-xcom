using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(BuildGraph))]
public class BuildGraphEditor : Editor {

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		if(GUILayout.Button("Scan"))
		{
			(target as BuildGraph).Scan();
			HandleUtility.Repaint();
		}
	}
}
