using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System;

namespace AltSystems.AltTrees.Editor
{
	public class InstallerAltTrees : EditorWindow
	{
		bool isCenter = false;

		void OnGUI()
		{
			if (!isCenter)
			{
				InstallerAltTrees window = this;
				window.minSize = new Vector2(350, 250);
				window.maxSize = new Vector2(350, 250);
                AltBackup.Editor.CenterOnMainEditorWindow.CenterOnMainWin(window);
				isCenter = true;
			}


			GUIStyle sty = new GUIStyle();
			sty.richText = true;

			sty.fontSize = 15;
			sty.fontStyle = FontStyle.Bold;
			GUI.Label(new Rect(138, 10, 300, 30), "Excellent!", sty);
			GUI.Label(new Rect(85, 30, 300, 30), "Installation successful!", sty);
			
			sty.fontSize = 12;
			sty.fontStyle = FontStyle.Normal;
			if (System.IO.File.Exists("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/cfg.txt"))
			{
				GUI.Label(new Rect(55, 80, 300, 30), "Older scripts moved to the folder", sty);
				GUI.Label(new Rect(2, 95, 300, 30), "\"project folder/AltSystems/AltTrees/OldScriptsTemp\"", sty);
			}

			GUI.Label(new Rect(15, 150, 300, 30), "Get started here: <b>Window/AltSystems/AltTrees</b>", sty);

			GUI.Label(new Rect(50, 200, 300, 30), "<b>Thank you for choosing AltSystems! ;)</b>", sty);

        
			if (GUI.Button(new Rect(100, 220, 150, 18), "Exit"))
			{
				exit();
			}
		}

		void exit()
		{
			this.Close();
		}
	}
}