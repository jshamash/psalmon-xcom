using System;
using UnityEngine;
using UnityEditor;
using Mixamo.UnityStore;

public class MixamoStoreWindow : EditorWindow
{
	System.DateTime lastOnGUITime = System.DateTime.Now;
	
	private const string MXE = "pro";
	
	
	private MixamoStore Store
	{
		get
		{
			if( _store == null )
				_store = new MixamoStore(this , MXE);
			
			return _store;
		}
	}
	
	[MenuItem ("Window/Mixamo Store #%1")]
	static void Init()
	{
		MixamoStoreWindow window = (MixamoStoreWindow) EditorWindow.GetWindow(typeof(MixamoStoreWindow), false, "Mixamo Store");
		window.wantsMouseMove = false;
		window.Show();
	}
	
	void OnGUI()
	{
		Store.OnGUI();
	}
	
	void Update()
	{
		if (this == EditorWindow.mouseOverWindow && ((System.DateTime.Now - lastOnGUITime).TotalSeconds > (1.0f / 20.0f)))
		{
			lastOnGUITime = System.DateTime.Now;
			this.Repaint();
		}
		
		Store.Update();
	}
	
	void OnDestroy()
	{
		Store.OnDestroy();
	}

	private MixamoStore _store = null;
}

