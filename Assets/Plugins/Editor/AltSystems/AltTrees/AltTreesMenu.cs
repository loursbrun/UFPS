using UnityEngine;
using UnityEditor;
using AltSystems.AltBackup.Editor;
using AltSystems.AltBackup;

namespace AltSystems.AltTrees.Editor
{
    public class AltTreesMenu : EditorWindow
    {
        [MenuItem("Window/AltSystems/AltTrees/Create AltTrees", false, 1)] static void Create ()
	    {
            GameObject go = new GameObject("AltTrees", typeof(AltTrees));
		    go.transform.position = Vector3.zero;
		    go.transform.rotation = Quaternion.identity;

            int idTemp = Random.Range(100000000, 999999999);

            if (!System.IO.Directory.Exists("Assets/Plugins/AltSystems/AltTrees/DataBase"))
            {
                System.IO.Directory.CreateDirectory("Assets/Plugins/AltSystems/AltTrees/DataBase");
            }

            if (!System.IO.Directory.Exists("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesData"))
            {
                System.IO.Directory.CreateDirectory("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesData");
            }

            while (System.IO.Directory.Exists("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesData/" + idTemp))
            {
                idTemp = Random.Range(100000000, 999999999);
            }

            System.IO.Directory.CreateDirectory("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesData/" + idTemp);

            go.GetComponent<AltTrees>().setIdManager(idTemp);

            if (!System.IO.File.Exists("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesDataLinks.asset"))
		    {
                AssetDatabase.CreateAsset(CreateInstance<AltTreesDataLinks>(), "Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesDataLinks.asset");
			
			    AssetDatabase.SaveAssets ();
			    AssetDatabase.Refresh();
		    }

		    Undo.RegisterCreatedObjectUndo (go, "Create AltTrees");

		    Selection.activeGameObject = go;
        }

        [MenuItem("GameObject/AltSystems/AltTrees", false)] static void Create2()
        {
            Create();
        }

        [MenuItem("Window/AltSystems/AltTrees/Create Wind", false, 2)] static void CreateWind()
        {
            GameObject go = new GameObject("AltWind", typeof(AltWind));
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;

            Undo.RegisterCreatedObjectUndo(go, "Create AltWind");

            Selection.activeGameObject = go;
        }

        [MenuItem("GameObject/AltSystems/Wind", false)] static void CreateWind2()
        {
            CreateWind();
        }

        [MenuItem("Window/AltSystems/AltTrees/Documentation", false, 51)]
        static void Subscription()
        {
            Application.OpenURL("http://altsystems-unity.net/AltTrees/Documentation/");
        }


        [MenuItem("Window/AltSystems/AltTrees/About...", false, 52)]
        static void About()
        {

            AboutAltTrees w = (AboutAltTrees)EditorWindow.GetWindow(typeof(AboutAltTrees), true, "About...");
            w.minSize = new Vector2(300, 200);
            w.maxSize = new Vector2(300, 200);
            CenterOnMainEditorWindow.CenterOnMainWin(w);
        }

    }
}