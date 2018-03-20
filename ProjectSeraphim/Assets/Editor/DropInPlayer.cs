using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class DropInPlayer
{

	private static void OnSceneGUI (SceneView sv)
    {
        Event e = Event.current;
        Debug.Log("1");

		if(Event.current.keyCode == KeyCode.G)
        {
            Debug.Log("It worked.");
            EditorApplication.Beep();
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }
	}

}
