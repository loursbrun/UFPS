using UnityEngine;
using UnityEditor;

namespace AltSystems.AltBackup.Editor
{
    public class AboutAltBackup : EditorWindow
    {

        void OnGUI()
        {
            GUIStyle sty = new GUIStyle();
            sty.alignment = TextAnchor.MiddleLeft;
            sty.fontSize = 12;


            if (EditorGUIUtility.isProSkin)
                sty.normal.textColor = Color.white;
            else
                sty.normal.textColor = Color.black;


            GUI.Label(new Rect(10, 10, 200, 20), "<b>AltBackup</b> Terrain System <b>v" + AltBackupTerrains.version + "</b>", sty);

            GUI.Label(new Rect(30, 27, 200, 20), "by AltSystems", sty);

            if (EditorGUIUtility.isProSkin)
            {
                GUI.Label(new Rect(10, 80, 200, 20), " - <color=#808080ff>Demo Video</color>", sty);
                GUI.Label(new Rect(10, 100, 200, 20), " - <color=#808080ff>Forum Thread</color>", sty);
                GUI.Label(new Rect(10, 120, 200, 20), " - E-mail: <color=#808080ff>AltSystemsUnity@gmail.com</color>", sty);
            }
            else
            {
                GUI.Label(new Rect(10, 80, 200, 20), " - <color=#0000ffff>Demo Video</color>", sty);
                GUI.Label(new Rect(10, 100, 200, 20), " - <color=#0000ffff>Forum Thread</color>", sty);
                GUI.Label(new Rect(10, 120, 200, 20), " - E-mail: <color=#0000ffff>AltSystemsUnity@gmail.com</color>", sty);
            }


            if (GUI.Button(new Rect(22, 82, 95, 14), "", sty))
                Application.OpenURL("https://youtu.be/9Bc74YbFQ_0");
            EditorGUIUtility.AddCursorRect(new Rect(22, 82, 95, 14), MouseCursor.Link);

            if (GUI.Button(new Rect(22, 102, 88, 14), "", sty))
                Application.OpenURL("http://forum.unity3d.com/threads/393990/");
            EditorGUIUtility.AddCursorRect(new Rect(22, 102, 88, 14), MouseCursor.Link);

            if (GUI.Button(new Rect(56, 122, 200, 14), "", sty))
                Application.OpenURL("mailto:AltSystemsUnity@gmail.com");
            EditorGUIUtility.AddCursorRect(new Rect(56, 122, 200, 14), MouseCursor.Link);

        }


    }
}
