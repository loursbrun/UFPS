using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace AltSystems.AltTrees
{
    [ExecuteInEditMode]
    public class AltTrees : MonoBehaviour
    {
        [SerializeField]
        private int idManager = -1;
        public AltTreesManagerData altTreesManagerData;

    
        [System.NonSerialized]
        public AltTreesManager altTreesManager = null;
        [System.NonSerialized]
        bool isInit = false;
        [System.NonSerialized]
        public int reInitTimer = -1;

        public int menuId = 1;
        public int idTreeSelected = -1;
        public int brushSize = 20;
        public int treeCount = 5;
        public int speedPlace = 2;
        public bool randomRotation = true;

        public float height = 0.3f;
        public float heightRandom = 1.0f;
        public bool isRandomHeight = true;

        public bool lockWidthToHeight = true;
        public float width = 0.3f;
        public float widthRandom = 1.0f;
        public bool isRandomWidth = true;

        public bool isRandomHueLeaves = true;
        public bool isRandomHueBark = true;

        public float angleLimit = 30f;
        public int countInit = 0;

        [System.NonSerialized]
        public bool dataLinksCorrupted = false;
        [System.NonSerialized]
        public bool dataLinksCorruptedLogged = false;

        void Update()
	    {
            if (Application.isPlaying)
            {
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;
                transform.localScale = Vector3.zero;

                UpdateFunk();
            }
	    }


        public void dataLinksIsCorrupted()
        {
            dataLinksCorrupted = true;
            
            if (!dataLinksCorruptedLogged)
            {
                dataLinksCorruptedLogged = true;
                Debug.LogError("The AltTreesDataLinks file is corrupted. Select the file \"Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesDataLinks.asset\", and assign a script \"AltTreesDataLinks\" on the missing script.");
            }
            countInit = 0;
        }

        public void Init(bool _draw = false)
        {
            //Debug.Log("Init");

            #if UNITY_EDITOR
            {
                AltTreesDataLinks dataLinks = null;
                if (!System.IO.Directory.Exists("Assets/Plugins/AltSystems/AltTrees/DataBase"))
                    System.IO.Directory.CreateDirectory("Assets/Plugins/AltSystems/AltTrees/DataBase");

                if (!System.IO.File.Exists("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesDataLinks.asset"))
                {
                    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<AltTreesDataLinks>(), "Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesDataLinks.asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                dataLinks = (AltTreesDataLinks)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesDataLinks.asset", typeof(AltTreesDataLinks));

                if (dataLinks == null)
                    dataLinksIsCorrupted();
                else
                {
                    dataLinksCorrupted = false;
                    dataLinksCorruptedLogged = false;

                    if (dataLinks.checkTreeVersionsStatus())
                    {
                        countInit = 0;
                        return;
                    }
                }
            }
            #endif

            if(dataLinksCorrupted)
                return;

            GameObject goInstAltTreesManager = new GameObject("altTrees Manager");
            goInstAltTreesManager.hideFlags = HideFlags.HideAndDontSave;
            altTreesManager = goInstAltTreesManager.AddComponent<AltTreesManager>();
            altTreesManager.Init(this);

            if (altTreesManagerData.draw || _draw)
            {
                for (int i = 0; i < altTreesManagerData.patches.Length; i++)
                {
                    altTreesManagerData.patches[i].Init(altTreesManager, this, altTreesManagerData);
                    altTreesManager.addAltTrees(altTreesManagerData.patches[i]);
                }
            }
            isInit = true;
            countInit = 0;
        }

        public void ReInit(bool _draw = false)
        {
            if(altTreesManager != null)
                altTreesManager.destroy(true);
            Init(_draw);
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                #if UNITY_EDITOR
                {
                    AltTreesManager.camEditor = SceneView.lastActiveSceneView.camera;
                }
                #endif

                if(reInitTimer > 0)
                {
					bool isCompiling = false;
					#if UNITY_EDITOR
						isCompiling = EditorApplication.isCompiling;
					#endif
					
					if(!isCompiling)
						reInitTimer--;
                    if(reInitTimer == 0)
                    {
                        reInitTimer = -1;
                        ReInit();
                        return;
                    }
                }

                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;
                transform.localScale = Vector3.zero;

                UpdateFunk();
            }
        }

        void UpdateFunk()
        {
            if (!this.enabled || !altTreesManagerData.draw)
                return;

            if (!isInit)
            {
				bool isCompiling = false;
				#if UNITY_EDITOR
					isCompiling = EditorApplication.isCompiling;
				#endif
				
                if (countInit > 3 && !isCompiling)
                    Init();
                else
                    countInit++;
                return;
            }

            if (!Application.isPlaying && altTreesManager != null)
                altTreesManager.UpdateFunk();
        }

        public AltTreesPatch getPatch(Vector3 pos, int _sizePatch)
        {
            int stepX = Mathf.FloorToInt(pos.x / ((float)_sizePatch));
            int stepY = Mathf.FloorToInt(pos.z / ((float)_sizePatch));

            for (int i = 0; i < altTreesManagerData.patches.Length; i++)
            {
                if (altTreesManagerData.patches[i].stepX == stepX && altTreesManagerData.patches[i].stepY == stepY)
                    return altTreesManagerData.patches[i];
            }

            return null;
        }

        public AltTreesPatch[] getPatches(Vector3 pos, float radius)
        {
            List<AltTreesPatch> listPatches = new List<AltTreesPatch>();

            int stepXmin = Mathf.FloorToInt((pos.x - radius) / ((float)altTreesManagerData.sizePatch));
            int stepXmax = Mathf.FloorToInt((pos.x + radius) / ((float)altTreesManagerData.sizePatch));
            int stepYmin = Mathf.FloorToInt((pos.z - radius) / ((float)altTreesManagerData.sizePatch));
            int stepYmax = Mathf.FloorToInt((pos.z + radius) / ((float)altTreesManagerData.sizePatch));


            for (int n = stepXmin; n <= stepXmax; n++)
            {
                for (int m = stepYmin; m <= stepYmax; m++)
                {
                    for (int i = 0; i < altTreesManagerData.patches.Length; i++)
                    {
                        if (altTreesManagerData.patches[i].stepX == n && altTreesManagerData.patches[i].stepY == m)
                        {
                            listPatches.Add(altTreesManagerData.patches[i]);
                            break;
                        }
                    }
                }
            }

            return listPatches.ToArray();
        }

        public AltTreesPatch[] getPatches(Vector3 pos, float sizeX, float sizeZ)
        {
            List<AltTreesPatch> listPatches = new List<AltTreesPatch>();

            int stepXmin = Mathf.FloorToInt(pos.x / ((float)altTreesManagerData.sizePatch));
            int stepXmax = Mathf.FloorToInt((pos.x + sizeX) / ((float)altTreesManagerData.sizePatch));
            int stepYmin = Mathf.FloorToInt(pos.z / ((float)altTreesManagerData.sizePatch));
            int stepYmax = Mathf.FloorToInt((pos.z + sizeZ) / ((float)altTreesManagerData.sizePatch));


            for (int n = stepXmin; n <= stepXmax; n++)
            {
                for (int m = stepYmin; m <= stepYmax; m++)
                {
                    for (int i = 0; i < altTreesManagerData.patches.Length; i++)
                    {
                        if (altTreesManagerData.patches[i].stepX == n && altTreesManagerData.patches[i].stepY == m)
                        {
                            listPatches.Add(altTreesManagerData.patches[i]);
                            break;
                        }
                    }
                }
            }

            return listPatches.ToArray();
        }
    
    

        public void Log(string str, Object obj = null)
        {
            #if UNITY_EDITOR
                if (altTreesManagerData.debugLog)
                    Debug.Log(str, obj);
            #else
                if (altTreesManagerData.debugLogInBilds)
                    Debug.Log(str, obj);
            #endif
        }

        public void LogWarning(string str, Object obj = null)
        {
            #if UNITY_EDITOR
                if (altTreesManagerData.debugLog)
                    Debug.LogWarning(str, obj);
            #else
                if (altTreesManagerData.debugLogInBilds)
                    Debug.LogWarning(str, obj);
            #endif
        }

        public void LogError(string str, Object obj = null)
        {
            #if UNITY_EDITOR
                if (altTreesManagerData.debugLog)
                    Debug.LogError(str, obj);
            #else
                if (altTreesManagerData.debugLogInBilds)
                    Debug.LogError(str, obj);
            #endif
        }



        void OnDestroy()
        {
            //Debug.Log("OnDestroy " + this.name, this);
            if (altTreesManager != null)
                altTreesManager.destroy(true);
            isInit = false;
        }

        void OnDisable()
        {
            //Debug.Log("OnDisable " + this.name, this);
            if (altTreesManager != null)
                altTreesManager.destroy(true);
            isInit = false;
        }

        void OnApplicationQuit()
        {
            //Debug.Log("OnApplicationQuit " + this.name, this);
            if (altTreesManager != null)
                altTreesManager.destroy(true);
            isInit = false;
        }

        /*void OnLevelWasLoaded(int level)
        {
            //Debug.Log("OnLevelWasLoaded " + this.name);
            if (altTreesManager != null)
                altTreesManager.destroy(true);
            isInit = false;
        }*/

        public void setIdManager(int id)
        {
            if (idManager == -1)
                idManager = id;
        }


        public int getIdManager()
        {
            return idManager;
        }


    }
}