using UnityEngine;
using UnityEditor;

namespace AltSystems.AltTrees.Editor
{
    public class OffMouseSelect : MonoBehaviour
    {

        static SceneView.OnSceneFunc onSceneGUIFunc;
        static bool placing = false;


        public static void enable()
        {
            onSceneGUIFunc = OffMouseSelect.OnSceneGUI_sfdfg43rhe2;
            SceneView.onSceneGUIDelegate += onSceneGUIFunc;
            placing = true;
        }

        public static void disable()
        {
            SceneView.onSceneGUIDelegate -= onSceneGUIFunc;
            placing = false;
        }

        public static void OnSceneGUI_sfdfg43rhe2(SceneView sceneView)
        {
            if (placing)
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
    }
}