using UnityEngine;
using UnityEditor;

namespace AltSystems.AltBackup.Editor
{
    public class AltBackupTerrains : EditorWindow
    {
        static public string version = "1.1";


        [MenuItem("Window/AltSystems/AltBackup/Create backup terrain", false, 11)]
        static void Create()
        {

            AltBackupTerrainsCreate w = (AltBackupTerrainsCreate)EditorWindow.GetWindow(typeof(AltBackupTerrainsCreate), true, "Create backup");
            w.minSize = new Vector2(400, 300);
            w.maxSize = new Vector2(400, 300);
            CenterOnMainEditorWindow.CenterOnMainWin(w);
        }

        [MenuItem("Window/AltSystems/AltBackup/Restore backup terrain", false, 12)]
        static void Restore()
        {

            AltBackupTerrainsRestore w = (AltBackupTerrainsRestore)EditorWindow.GetWindow(typeof(AltBackupTerrainsRestore), true, "Restore Terrain");
            w.minSize = new Vector2(620, 500);
            w.maxSize = new Vector2(620, 500);
            CenterOnMainEditorWindow.CenterOnMainWin(w);
        }


        [MenuItem("Window/AltSystems/AltBackup/About...", false, 13)]
        static void About()
        {

            AboutAltBackup w = (AboutAltBackup)EditorWindow.GetWindow(typeof(AboutAltBackup), true, "About...");
            w.minSize = new Vector2(300, 200);
            w.maxSize = new Vector2(300, 200);
            CenterOnMainEditorWindow.CenterOnMainWin(w);
        }

        [MenuItem("Window/AltSystems/News", false, 51)]
        static void News()
        {
            AltSystemsNewsWindow.Init(0);
        }

        [MenuItem("Window/AltSystems/Subscribe to Newsletters", false, 52)]
        static void Subscription()
        {
            Application.OpenURL("http://altsystems-unity.net/subscription.php?unity=" + Application.unityVersion + "&asset=AltBackup&ver=" + AltSystemsNewsCheck.version);
        }

    }
}
