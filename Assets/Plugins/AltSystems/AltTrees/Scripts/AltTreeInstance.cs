using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace AltSystems.AltTrees
{
    public class AltTreeInstance : MonoBehaviour
    {
        [HideInInspector]
        public AltTreesManager manager = null;
        [HideInInspector]
        public int altTreesId;
        [HideInInspector]
        public int idTree;
        [HideInInspector]
        public Vector3 scaleTempStar = Vector3.zero;
        [HideInInspector]
        public bool isCrossFade = false;
        [HideInInspector]
        public bool isObject = true;

        public Color hueLeave;
        public Color hueBark;
        [HideInInspector]
        public Color hueLeaveStar;
        [HideInInspector]
        public Color hueBarkStar;


        void OnDrawGizmosSelected()
        {
            #if UNITY_EDITOR
            {
                if (Selection.gameObjects.Length > 1)
                {
                    for (int i = 0; i < Selection.gameObjects.Length; i++)
                    {
                        if (Selection.gameObjects[i] == gameObject)
                        {
                            Selection.activeTransform = gameObject.transform;
                            manager.setSelectionTrees(new Transform[] { Selection.activeTransform });
                        }
                    }
                }
                }
            #endif
        }
    }

    public class AtiTemp
    {
        public AltTreesPatch altTrees;
        public int altTreesId;
        public int idTree;
        public bool isObject;

        public AtiTemp(AltTreesPatch _altTrees, int _altTreesId, int _idTree, bool _isObject)
        {
            altTrees = _altTrees;
            altTreesId = _altTreesId;
            idTree = _idTree;
            isObject = _isObject;
        }
    }
}