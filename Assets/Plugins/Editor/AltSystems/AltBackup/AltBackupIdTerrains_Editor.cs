using UnityEngine;
using UnityEditor;

namespace AltSystems.AltBackup.Editor
{
    [CustomEditor(typeof(AltBackupIdTerrains))]
    public class AltBackupIdTerrains_Editor : UnityEditor.Editor
    {
        GUIStyle sty;

        public void OnEnable()
        {
            sty = new GUIStyle();
            sty.wordWrap = true;
        }

        public override void OnInspectorGUI()
        {
            sty.alignment = TextAnchor.MiddleCenter;
            sty.fontSize = 14;
            sty.fontStyle = FontStyle.Bold;

            if (EditorGUIUtility.isProSkin)
                sty.normal.textColor = Color.white;
            else
                sty.normal.textColor = Color.black;

            GUILayout.Space(30);
            GUILayout.Label("This file stores a number of terrains, textures, trees and objects for backups.", sty);
            GUILayout.Space(20);
            sty.normal.textColor = Color.red;
            GUILayout.Label("Please, do not move, do not delete, do not change it!", sty);
            GUILayout.Space(40);

            if (EditorGUIUtility.isProSkin)
                sty.normal.textColor = Color.white;
            else
                sty.normal.textColor = Color.black;

            sty.alignment = TextAnchor.MiddleLeft;
            sty.fontSize = 12;
            sty.fontStyle = FontStyle.Normal;
            GUILayout.Label("This file is used only in the editor. In the bild it is not included.", sty);
        }
    }
}
