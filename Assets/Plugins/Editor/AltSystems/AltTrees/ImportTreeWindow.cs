using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace AltSystems.AltTrees.Editor
{
    public class ImportTreeWindow : EditorWindow
    {
        static public GameObject tree;
        static AltTreesDataLinks dataLinks = null;
        static int status = 0;
        static Texture2D texture;
        static int typeTree = 0;

        [MenuItem("Assets/Convert to AltTree")]
        static void menu()
        {
            tree = Selection.gameObjects[0];
            status = 0;
            texture = null;
            typeTree = 0;
            mats.Clear();
            matsBark.Clear();
            matsLeaves.Clear();
            isObject = false;
            ate = null;
            terrainTempImport = null;
            isDeleteTreesFromTerrain = false;
            isDeleteTreesFromAltTrees = false;
            Convert();
        }

        [MenuItem("Assets/Convert to AltTree", true)]
        static bool menuValidate()
        {
            if (Selection.gameObjects != null && Selection.gameObjects.Length == 1)
            {
                if (Selection.gameObjects[0] != Selection.gameObjects[0].transform.root.gameObject)
                    return false;
                if (Selection.gameObjects[0].GetComponent<Tree>() != null)
                    return true;
                if (Selection.gameObjects[0].GetComponent<LODGroup>() != null)
                    return true;
                if (Selection.gameObjects[0].GetComponent<MeshFilter>() != null && Selection.gameObjects[0].GetComponent<MeshFilter>().sharedMesh != null)
                    return true;
            }

            return false;
        }

        static List<Material> mats = new List<Material>();
        static List<Material> matsBark = new List<Material>();
        static List<Material> matsLeaves = new List<Material>();
        Vector2 sroll = new Vector2();
        Vector2 sroll2 = new Vector2();
        static bool isObject = false;
        int cco = 0;

        static int textureSize = 0;
        static int normalmapSize = 0;

        void OnGUI()
        {
            GUIStyle sty = new GUIStyle();
            sty.alignment = TextAnchor.MiddleCenter;
            sty.fontSize = 16;
            sty.fontStyle = FontStyle.Bold;

            GUIStyle sty2 = new GUIStyle();
            sty2.fontStyle = FontStyle.Bold;

            if (tree == null)
            {
                this.Close();
                return;
            }


            getDataLinks();

            if (status == 2)
            {

                AltTree treeTemp = dataLinks.getAltTree(tree);

                GUI.Label(new Rect(0, 30, 400, 30), "This tree has already been converted!", sty);

                if (GUI.Button(new Rect(20, 120, 170, 20), "Select current AltTree"))
                {
                    Selection.activeObject = treeTemp;
                    this.Close();
                }
                if (GUI.Button(new Rect(210, 120, 170, 20), "Cancel"))
                {
                    this.Close();
                }
            }
            else if (status == 0)
            {
                GUI.Box(new Rect(292, 5, 140, 140), "");

                if (texture != null)
                {
                    texture.hideFlags = HideFlags.DontSave;
                    GUI.DrawTexture(new Rect(298, 11, 128, 128), texture);
                }
                else
                {
                    cco++;
                    texture = (Texture2D)AssetPreview.GetAssetPreview(tree);

                    if (cco == 5)
                    {
                        cco = 0;
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(tree), ImportAssetOptions.ForceUpdate);
                    }
                    Repaint();
                }

                isObject = GUI.Toggle(new Rect(261, 160, 128, 25), isObject, "is Object");


                GUI.Box(new Rect(258, 179, 204, 70), "", EditorStyles.helpBox);
                {
                    GUI.Label(new Rect(262, 182, 200, 25), "Billboard Settings:", sty2);

                    GUI.Label(new Rect(261, 210, 200, 25), "Texture Size:");
                    textureSize = EditorGUI.IntPopup(new Rect(400, 210, 58, 15), textureSize, new string[] { "4096", "2048", "1024", "512", "256", "128" }, new int[] { 4096, 2048, 1024, 512, 256, 128 });

                    GUI.Label(new Rect(261, 230, 200, 25), "Normalmap Size:");
                    normalmapSize = EditorGUI.IntPopup(new Rect(400, 230, 58, 15), normalmapSize, new string[] { "None", "4096", "2048", "1024", "512", "256", "128" }, new int[] { 0, 4096, 2048, 1024, 512, 256, 128 });
                }


                sty.fontSize = 14;
                GUI.Label(new Rect(5, 20, 220, 30), "Choose import settings: ", sty);

                typeTree = EditorGUI.Popup(new Rect(5, 50, 240, 20), typeTree, new string[] { "----", "SpeedTree", "Tree Creator", "Mesh Tree" });





                if (typeTree == 1 || typeTree == 2 || typeTree == 3)
                {
                    sty.fontSize = 11;
                    GUI.Label(new Rect(5, 65, 140, 30), "Configure materials: ", sty);

                    GUI.Box(new Rect(5, 90, 245, 230), "");


                    GUI.Box(new Rect(5, 90, 245, 115), "");
                    sty.fontSize = 13;
                    GUI.Label(new Rect(3, 87, 80, 30), "Leaves: ", sty);
                    sroll = GUI.BeginScrollView(new Rect(5, 110, 240, 90), sroll, new Rect(5, 110, 225, matsLeaves.Count * 20 + 15));
                    {
                        for (int i = 0; i < matsLeaves.Count; i++)
                        {
                            EditorGUI.ObjectField(new Rect(10, 120 + i * 20, 150, 16), matsLeaves[i], typeof(Material), false);
                            if (GUI.Button(new Rect(162, 120 + i * 20, 65, 16), "to bark"))
                            {
                                matsBark.Add(matsLeaves[i]);
                                matsLeaves.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                    GUI.EndScrollView();
                
                    sty.fontSize = 13;
                    GUI.Label(new Rect(5, 202, 60, 30), "Bark: ", sty);
                    sroll2 = GUI.BeginScrollView(new Rect(5, 225, 240, 90), sroll2, new Rect(5, 225, 225, matsBark.Count * 20 + 15));
                    {
                        for (int i = 0; i < matsBark.Count; i++)
                        {
                            EditorGUI.ObjectField(new Rect(10, 235 + i * 20, 150, 16), matsBark[i], typeof(Material), false);
                            if (GUI.Button(new Rect(162, 235 + i * 20, 65, 16), "to leaves"))
                            {
                                matsLeaves.Add(matsBark[i]);
                                matsBark.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                    GUI.EndScrollView();

                    if (GUI.Button(new Rect(100, 325, 200, 20), "Start conversion"))
                    {
                        status = 1;
                        this.Close();
                        Convert();
                        return;
                    }
                }
            }
        }

        static AltTrees_Editor ate;
        static bool isImport = false;
        static Terrain terrainTempImport;
        static bool isDeleteTreesFromTerrain = false;
        static bool isDeleteTreesFromAltTrees = false;

        static public void ConvertTree(GameObject _tree, AltTrees_Editor _ate, Terrain _terrainTempImport, bool _isDeleteTreesFromTerrain, bool _isDeleteTreesFromAltTrees)
        {
            tree = _tree;
            ate = _ate;
            isImport = true;
            terrainTempImport = _terrainTempImport;
            isDeleteTreesFromTerrain = _isDeleteTreesFromTerrain;
            isDeleteTreesFromAltTrees = _isDeleteTreesFromAltTrees;


            status = 0;
            texture = null;
            typeTree = 0;
            mats.Clear();
            matsBark.Clear();
            matsLeaves.Clear();
            isObject = false;


            Convert();
        }


        static void Convert()
        {
            getDataLinks();

            AltTree treeTemp = null;

            if (dataLinks.getAltTree(tree) == null && status == 1 && typeTree != 0)
            {
                EditorUtility.DisplayProgressBar("Convert tree... ", "Converting... ", 0.0f);

                GameObject goTemp = new GameObject("altTree");
                goTemp.transform.position = Vector3.zero;
                goTemp.transform.rotation = Quaternion.identity;
                treeTemp = (AltTree)goTemp.AddComponent(typeof(AltTree));
                treeTemp.id = dataLinks.getUniqueIdTree();
                treeTemp.isObject = isObject;
                treeTemp.version = 1;


                List<Material> matsBarkTemp = new List<Material>();
                List<Material> matsLeavesTemp = new List<Material>();

                if (!System.IO.Directory.Exists("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees"))
                {
                    System.IO.Directory.CreateDirectory("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees");
                }
                if (!System.IO.Directory.Exists("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources"))
                {
                    System.IO.Directory.CreateDirectory("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources");
                }
                string assetPathAndName = "";

                if (AssetDatabase.GenerateUniqueAssetPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/" + tree.name + ".prefab").Equals("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/" + tree.name + ".prefab"))
                    assetPathAndName = "Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/" + tree.name + ".prefab";
                else
                    assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/" + tree.name + "_2.prefab");

                string[] nameTreeArray = assetPathAndName.Split('/');
                nameTreeArray = nameTreeArray[nameTreeArray.Length - 1].Split('.');
                string folderMat = "";

                if (AssetDatabase.GenerateUniqueAssetPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + tree.name).Equals("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + tree.name))
                    folderMat = "Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + tree.name;
                else
                    folderMat = AssetDatabase.GenerateUniqueAssetPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + tree.name + "_2");
                nameTreeArray = folderMat.Split('/');
                folderMat = nameTreeArray[nameTreeArray.Length - 1];
                
                treeTemp.folderResources = folderMat;
                
                System.IO.Directory.CreateDirectory("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderMat);
                System.IO.Directory.CreateDirectory("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderMat + "/Billboard");
                System.IO.Directory.CreateDirectory("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderMat + "/Materials");


                Dictionary<Material, Material> matsTempDictionary = new Dictionary<Material, Material>();

                treeTemp.hueVariationLeaves = new Color32(255, 0, 0, 26);
                treeTemp.hueVariationBark = new Color32(0, 0, 0, 100);

                if (treeTemp.shaderAntialiasing == null)
                    treeTemp.shaderAntialiasing = (Shader)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/Shaders/FXAA3Console.shader", typeof(Shader));
                if (treeTemp.shaderBillboard == null)
                    treeTemp.shaderBillboard = (Shader)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/Shaders/BillboardAltTree.shader", typeof(Shader));
                if (treeTemp.shaderBillboardGroup == null)
                    treeTemp.shaderBillboardGroup = (Shader)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/Shaders/BillboardGroupAltTree.shader", typeof(Shader));
                if (treeTemp.shaderNormalsToScreen == null)
                    treeTemp.shaderNormalsToScreen = (Shader)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/Shaders/ScreenNormals.shader", typeof(Shader));
                Shader shaderBark = null;
                Shader shaderLeaves = null;
                Shader shaderBarkBump = null;
                Shader shaderLeavesBump = null;

                if (typeTree == 1)
                {
                    shaderLeaves = (Shader)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/Shaders/SpeedTreeAltTree.shader", typeof(Shader));
                    shaderBark = shaderLeaves;


                    treeTemp.windMode = 1;

                    if (Directory.Exists("AltSystems/AltTrees/SpeedTreeWindParameters") && File.Exists("AltSystems/AltTrees/SpeedTreeWindParameters/" + tree.name + ".altWSTParams"))
                    {
                        treeTemp.windParams_ST = new float[704];
                        treeTemp.windParamsUp_ST = new bool[704];

                        using (BinaryReader reader = new BinaryReader(File.Open("AltSystems/AltTrees/SpeedTreeWindParameters/" + tree.name + ".altWSTParams", FileMode.Open)))
                        {
                            for (int i = 0; i < 704; i++)
                            {
                                treeTemp.windParamsUp_ST[i] = reader.ReadBoolean();
                                treeTemp.windParams_ST[i] = reader.ReadSingle();
                            }
                        }
                        treeTemp.loadedConfig = true;
                    }
                }
                else if (typeTree == 2)
                {
                    shaderLeaves = (Shader)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/Shaders/TreeCreatorLeavesAltTree.shader", typeof(Shader));
                    shaderBark = (Shader)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/Shaders/TreeCreatorBarkAltTree.shader", typeof(Shader));

                    treeTemp.windMode = 2;
                }
                else if (typeTree == 3)
                {
                    shaderLeaves = (Shader)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/Shaders/LeavesAltTree.shader", typeof(Shader));
                    shaderBark = (Shader)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/Shaders/BarkAltTree.shader", typeof(Shader));
                    shaderLeavesBump = (Shader)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/Shaders/LeavesBumpAltTree.shader", typeof(Shader));
                    shaderBarkBump = (Shader)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/Shaders/BarkBumpAltTree.shader", typeof(Shader));
                }
                if (shaderBarkBump == null)
                    shaderBarkBump = shaderBark;
                if (shaderLeavesBump == null)
                    shaderLeavesBump = shaderLeaves;

                LODGroup lodGroupTemp = tree.GetComponent<LODGroup>();
                Material[] matsTemp;
                float sizeTemp = 0f;

                if (lodGroupTemp != null)
                {
                    SerializedObject objj = new SerializedObject(lodGroupTemp);

                    //LOD[] lodsTemp = lodGroupTemp.GetLODs();
                    Renderer[] lodsTemp = new Renderer[lodGroupTemp.lodCount];

                    for (int t = 0; t < lodGroupTemp.lodCount; t++)
                    {
                        SerializedProperty property = objj.FindProperty("m_LODs.Array.data[" + t + "].renderers");
                        if(property.arraySize > 0 && (property.GetArrayElementAtIndex(0).FindPropertyRelative("renderer").objectReferenceValue as Renderer).GetComponent<BillboardRenderer>() == null)
                            lodsTemp[t] = (property.GetArrayElementAtIndex(0).FindPropertyRelative("renderer").objectReferenceValue as Renderer);
                    }
                    
                    int countLods = 0;
                    for (int j = 0; j < lodsTemp.Length; j++)
                    {
                        if (lodsTemp[j] != null)
                            countLods++;
                    }
                    treeTemp.lods = new GameObject[countLods];
                    treeTemp.distances = new float[countLods - 1];
                    treeTemp.distancesSquares = new float[countLods - 1];
                    countLods = 0;

                    for (int j = 0; j < lodsTemp.Length; j++)
                    {
                        if (lodsTemp[j] != null)
                        {
                            GameObject lodTemp = (GameObject)Instantiate(lodsTemp[j].gameObject, Vector3.zero, Quaternion.identity);
                            lodTemp.transform.parent = goTemp.transform;
                            lodTemp.name = lodTemp.name.Replace("(Clone)", "");

                            if (j == 0)
                            {
                                sizeTemp = Mathf.Max(lodTemp.GetComponent<MeshRenderer>().bounds.size.y, Mathf.Max(lodTemp.GetComponent<MeshRenderer>().bounds.size.x, lodTemp.GetComponent<MeshRenderer>().bounds.size.z));
                            }
                            if (lodTemp.GetComponent<Tree>() != null)
                                DestroyImmediate(lodTemp.GetComponent<Tree>());

                            if (j != 0)
                                lodTemp.SetActive(false);
                            else
                            {
                                if (lodTemp.GetComponent<MeshRenderer>() != null && lodTemp.GetComponent<MeshRenderer>().sharedMaterials.Length > 0 && lodTemp.GetComponent<MeshRenderer>().sharedMaterials[0].HasProperty("_HueVariation"))
                                    treeTemp.hueVariationLeaves = lodTemp.GetComponent<MeshRenderer>().sharedMaterials[0].GetColor("_HueVariation");
                            }

                            if (lodTemp.GetComponent<MeshRenderer>() != null && lodTemp.GetComponent<MeshRenderer>().sharedMaterials.Length > 0)
                            {
                                matsTemp = lodTemp.GetComponent<MeshRenderer>().sharedMaterials;
                                for (int g = 0; g < matsTemp.Length; g++)
                                {
                                    if (matsTempDictionary.ContainsKey(matsTemp[g]))
                                    {
                                        matsTemp[g] = matsTempDictionary[matsTemp[g]];
                                    }
                                    else
                                    {
                                        Material mTemp = new Material(matsTemp[g]);
                                        mTemp.shaderKeywords = matsTemp[g].shaderKeywords;

                                        string matPathAndName = "";
                                        if (AssetDatabase.GenerateUniqueAssetPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderMat + "/Materials/" + matsTemp[g].name + ".mat").Equals("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderMat + "/Materials/" + matsTemp[g].name + ".mat"))
                                            matPathAndName = "Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderMat + "/Materials/" + matsTemp[g].name + ".mat";
                                        else
                                            matPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderMat + "/Materials/" + matsTemp[g].name + "_2.mat");

                                        if (matsTemp[g].HasProperty("_BumpSpecMap") || matsTemp[g].HasProperty("_BumpMap"))
                                        {
                                            if (matsBark.Contains(matsTemp[g]))
                                                mTemp.shader = shaderBarkBump;
                                            else if (matsLeaves.Contains(matsTemp[g]))
                                                mTemp.shader = shaderLeavesBump;
                                            else
                                                Debug.LogError("material is not Bark and not Leaves, bumped");
                                        }
                                        else
                                        {
                                            if (matsBark.Contains(matsTemp[g]))
                                                mTemp.shader = shaderBark;
                                            else if (matsLeaves.Contains(matsTemp[g]))
                                                mTemp.shader = shaderLeaves;
                                            else
                                                Debug.LogError("material is not Bark and not Leaves");
                                        }

                                        if (matsTemp[g].HasProperty("_BumpSpecMap") && mTemp.HasProperty("_BumpMap"))
                                            mTemp.SetTexture("_BumpMap", matsTemp[g].GetTexture("_BumpSpecMap"));
                                        if (matsTemp[g].HasProperty("_TranslucencyMap") && mTemp.HasProperty("_GlossMap"))
                                            mTemp.SetTexture("_GlossMap", matsTemp[g].GetTexture("_TranslucencyMap"));
                                        if (matsTemp[g].HasProperty("_ShadowTex") && mTemp.HasProperty("_ShadowOffset"))
                                            mTemp.SetTexture("_ShadowOffset", matsTemp[g].GetTexture("_ShadowTex"));
                                        if (matsTemp[g].HasProperty("_HueVariation"))
                                            mTemp.SetColor("_HueVariation", new Color(1f, 0.5f, 0.0f, 0.0f));

                                        AssetDatabase.CreateAsset(mTemp, matPathAndName);
                                        AssetDatabase.SaveAssets();
                                        AssetDatabase.Refresh();

                                        if (matsLeaves.Contains(matsTemp[g]))
                                            matsLeavesTemp.Add((Material)AssetDatabase.LoadAssetAtPath(matPathAndName, typeof(Material)));
                                        if (matsBark.Contains(matsTemp[g]))
                                            matsBarkTemp.Add((Material)AssetDatabase.LoadAssetAtPath(matPathAndName, typeof(Material)));

                                        matsTempDictionary.Add(matsTemp[g], (Material)AssetDatabase.LoadAssetAtPath(matPathAndName, typeof(Material)));
                                        matsTemp[g] = matsTempDictionary[matsTemp[g]];
                                    }
                                }
                                lodTemp.GetComponent<MeshRenderer>().sharedMaterials = matsTemp;
                            }

                            treeTemp.lods[countLods] = lodTemp;
                            if (countLods != 0)
                            {
                                if (countLods == 1)
                                {
                                    treeTemp.distances[countLods - 1] = sizeTemp * (8f / (float)System.Math.Pow(sizeTemp, 1.0 / 3.0));
                                    treeTemp.distancesSquares[countLods - 1] = treeTemp.distances[countLods - 1] * treeTemp.distances[countLods - 1];
                                }
                                else
                                {
                                    treeTemp.distances[countLods - 1] = treeTemp.distances[countLods - 2] * 1.5f;
                                    treeTemp.distancesSquares[countLods - 1] = treeTemp.distances[countLods - 1] * treeTemp.distances[countLods - 1];
                                }
                            }
                            countLods++;
                        }
                    }
                }
                else
                {
                    treeTemp.lods = new GameObject[1];
                    treeTemp.distances = new float[0];
                    treeTemp.distancesSquares = new float[0];

                    GameObject lodTemp = (GameObject)Instantiate(tree.gameObject, Vector3.zero, Quaternion.identity);
                    lodTemp.transform.parent = goTemp.transform;
                    lodTemp.name = lodTemp.name.Replace("(Clone)", "");

                    sizeTemp = Mathf.Max(lodTemp.GetComponent<MeshRenderer>().bounds.size.y, Mathf.Max(lodTemp.GetComponent<MeshRenderer>().bounds.size.x, lodTemp.GetComponent<MeshRenderer>().bounds.size.z));

                    foreach (Transform tr in lodTemp.transform)
                    {
                        DestroyImmediate(tr.gameObject);
                    }

                    if (lodTemp.GetComponent<Tree>() != null)
                        DestroyImmediate(lodTemp.GetComponent<Tree>());

                    if (lodTemp.GetComponent<MeshRenderer>() != null && lodTemp.GetComponent<MeshRenderer>().sharedMaterials.Length > 0 && lodTemp.GetComponent<MeshRenderer>().sharedMaterials[0].HasProperty("_HueVariation"))
                        treeTemp.hueVariationLeaves = lodTemp.GetComponent<MeshRenderer>().sharedMaterials[0].GetColor("_HueVariation");

                    if (lodTemp.GetComponent<MeshRenderer>() != null && lodTemp.GetComponent<MeshRenderer>().sharedMaterials.Length > 0)
                    {
                        matsTemp = lodTemp.GetComponent<MeshRenderer>().sharedMaterials;
                        for (int g = 0; g < matsTemp.Length; g++)
                        {
                            if (matsTempDictionary.ContainsKey(matsTemp[g]))
                            {
                                matsTemp[g] = matsTempDictionary[matsTemp[g]];
                            }
                            else
                            {
                                Material mTemp = new Material(matsTemp[g]);
                                mTemp.shaderKeywords = matsTemp[g].shaderKeywords;

                                string matPathAndName = "";
                                if (AssetDatabase.GenerateUniqueAssetPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderMat + "/Materials/" + matsTemp[g].name + ".mat").Equals("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderMat + "/Materials/" + matsTemp[g].name + ".mat"))
                                    matPathAndName = "Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderMat + "/Materials/" + matsTemp[g].name + ".mat";
                                else
                                    matPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderMat + "/Materials/" + matsTemp[g].name + "_2.mat");

                                if (matsTemp[g].HasProperty("_BumpSpecMap") || matsTemp[g].HasProperty("_BumpMap"))
                                {
                                    if (matsBark.Contains(matsTemp[g]))
                                        mTemp.shader = shaderBarkBump;
                                    else if (matsLeaves.Contains(matsTemp[g]))
                                        mTemp.shader = shaderLeavesBump;
                                    else
                                        Debug.LogError("material is not Bark and not Leaves, bumped");
                                }
                                else
                                {
                                    if (matsBark.Contains(matsTemp[g]))
                                        mTemp.shader = shaderBark;
                                    else if (matsLeaves.Contains(matsTemp[g]))
                                        mTemp.shader = shaderLeaves;
                                    else
                                        Debug.LogError("material is not Bark and not Leaves");
                                }

                                if (matsTemp[g].HasProperty("_BumpSpecMap") && mTemp.HasProperty("_BumpMap"))
                                    mTemp.SetTexture("_BumpMap", matsTemp[g].GetTexture("_BumpSpecMap"));
                                if (matsTemp[g].HasProperty("_TranslucencyMap") && mTemp.HasProperty("_GlossMap"))
                                    mTemp.SetTexture("_GlossMap", matsTemp[g].GetTexture("_TranslucencyMap"));
                                if (matsTemp[g].HasProperty("_ShadowTex") && mTemp.HasProperty("_ShadowOffset"))
                                    mTemp.SetTexture("_ShadowOffset", matsTemp[g].GetTexture("_ShadowTex"));

                                AssetDatabase.CreateAsset(mTemp, matPathAndName);
                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();

                                if (matsLeaves.Contains(matsTemp[g]))
                                    matsLeavesTemp.Add((Material)AssetDatabase.LoadAssetAtPath(matPathAndName, typeof(Material)));
                                if (matsBark.Contains(matsTemp[g]))
                                    matsBarkTemp.Add((Material)AssetDatabase.LoadAssetAtPath(matPathAndName, typeof(Material)));

                                matsTempDictionary.Add(matsTemp[g], (Material)AssetDatabase.LoadAssetAtPath(matPathAndName, typeof(Material)));
                                matsTemp[g] = matsTempDictionary[matsTemp[g]];
                            }
                        }
                        lodTemp.GetComponent<MeshRenderer>().sharedMaterials = matsTemp;
                        EditorUtility.SetDirty(lodTemp);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }


                    treeTemp.lods[0] = lodTemp;
                }
                if (treeTemp.distances.Length > 0)
                {
                    treeTemp.distancePlaneBillboard = treeTemp.distances[treeTemp.distances.Length - 1] * 1.5f;
                    treeTemp.distancePlaneBillboardSquare = treeTemp.distancePlaneBillboard * treeTemp.distancePlaneBillboard;
                }
                else
                {
                    treeTemp.distancePlaneBillboard = sizeTemp * (8f / (float)System.Math.Pow(sizeTemp, 1.0 / 3.0)) * 1.5f;
                    treeTemp.distancePlaneBillboardSquare = treeTemp.distancePlaneBillboard * treeTemp.distancePlaneBillboard;
                }

                treeTemp.distanceCulling = treeTemp.distancePlaneBillboard * 2.5f;
                treeTemp.distanceCullingSquare = treeTemp.distanceCulling * treeTemp.distanceCulling;


                GameObject coll = new GameObject("colliders");
                coll.transform.parent = goTemp.transform;
                coll.transform.localPosition = Vector3.zero;
                coll.transform.localRotation = Quaternion.identity;
                GameObject collBillboards = new GameObject("colliderBillboards");
                collBillboards.transform.parent = goTemp.transform;
                collBillboards.transform.localPosition = Vector3.zero;
                collBillboards.transform.localRotation = Quaternion.identity;

                bool isOk = false;
                bool isOk2 = false;

                foreach (Transform tr in tree.transform)
                {
                    if (tr.GetComponent<BoxCollider>() != null || tr.GetComponent<CapsuleCollider>() != null || tr.GetComponent<SphereCollider>() != null || tr.GetComponent<MeshCollider>() != null)
                    {
                        GameObject collGO = new GameObject(tr.name);
                        collGO.transform.parent = coll.transform;
                        collGO.transform.localPosition = tr.localPosition;
                        collGO.transform.localRotation = tr.localRotation;
                        collGO.transform.localScale = tr.localScale;

                        if (tr.GetComponent<BoxCollider>() != null)
                        {
                            if (UnityEditorInternal.ComponentUtility.CopyComponent(tr.GetComponent<BoxCollider>()))
                            {
                                UnityEditorInternal.ComponentUtility.PasteComponentValues(collGO.AddComponent<BoxCollider>());
                            }
                        }
                        if (tr.GetComponent<CapsuleCollider>() != null)
                        {
                            if (UnityEditorInternal.ComponentUtility.CopyComponent(tr.GetComponent<CapsuleCollider>()))
                            {
                                UnityEditorInternal.ComponentUtility.PasteComponentValues(collGO.AddComponent<CapsuleCollider>());
                            }
                        }
                        if (tr.GetComponent<SphereCollider>() != null)
                        {
                            if (UnityEditorInternal.ComponentUtility.CopyComponent(tr.GetComponent<SphereCollider>()))
                            {
                                UnityEditorInternal.ComponentUtility.PasteComponentValues(collGO.AddComponent<SphereCollider>());
                            }
                        }
                        if (tr.GetComponent<MeshCollider>() != null)
                        {
                            if (UnityEditorInternal.ComponentUtility.CopyComponent(tr.GetComponent<MeshCollider>()))
                            {
                                UnityEditorInternal.ComponentUtility.PasteComponentValues(collGO.AddComponent<MeshCollider>());
                            }
                        }



                        isOk = true;

                        if (tr.name.Equals("CollisionObject0"))
                        {
                            GameObject collGO2 = new GameObject(tr.name);
                            collGO2.transform.parent = collBillboards.transform;
                            collGO2.transform.localPosition = tr.localPosition;
                            collGO2.transform.localRotation = tr.localRotation;
                            collGO2.transform.localScale = tr.localScale;


                            if (tr.GetComponent<BoxCollider>() != null)
                            {
                                if (UnityEditorInternal.ComponentUtility.CopyComponent(tr.GetComponent<BoxCollider>()))
                                {
                                    UnityEditorInternal.ComponentUtility.PasteComponentValues(collGO2.AddComponent<BoxCollider>());
                                }
                            }
                            if (tr.GetComponent<CapsuleCollider>() != null)
                            {
                                if (UnityEditorInternal.ComponentUtility.CopyComponent(tr.GetComponent<CapsuleCollider>()))
                                {
                                    UnityEditorInternal.ComponentUtility.PasteComponentValues(collGO2.AddComponent<CapsuleCollider>());
                                }
                            }
                            if (tr.GetComponent<SphereCollider>() != null)
                            {
                                if (UnityEditorInternal.ComponentUtility.CopyComponent(tr.GetComponent<SphereCollider>()))
                                {
                                    UnityEditorInternal.ComponentUtility.PasteComponentValues(collGO2.AddComponent<SphereCollider>());
                                }
                            }
                            if (tr.GetComponent<MeshCollider>() != null)
                            {
                                if (UnityEditorInternal.ComponentUtility.CopyComponent(tr.GetComponent<MeshCollider>()))
                                {
                                    UnityEditorInternal.ComponentUtility.PasteComponentValues(collGO2.AddComponent<MeshCollider>());
                                }
                            }

                            isOk2 = true;
                        }
                    }
                }
                if (isOk)
                {
                    treeTemp.colliders = coll;
                }
                else
                {
                    DestroyImmediate(coll);
                }
                if (isOk2)
                {
                    treeTemp.billboardColliders = collBillboards;
                }
                else
                {
                    DestroyImmediate(collBillboards);
                }



                treeTemp.leavesMaterials = matsLeavesTemp.ToArray();
                treeTemp.barkMaterials = matsBarkTemp.ToArray();

                treeTemp.sizeTextureBillboard = textureSize;
                if (normalmapSize == 0)
                {
                    treeTemp.isNormalmapBillboard = false;
                    treeTemp.sizeNormalsBillboard = 128;
                }
                else
                {
                    treeTemp.isNormalmapBillboard = true;
                    treeTemp.sizeNormalsBillboard = normalmapSize;
                }


                PrefabUtility.CreatePrefab(assetPathAndName, goTemp);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                dataLinks.addTree(tree, (AltTree)AssetDatabase.LoadAssetAtPath(assetPathAndName, typeof(AltTree)), treeTemp.id);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.SetDirty(dataLinks);

                EditorUtility.DisplayProgressBar("Convert tree... ", "Generating Texture Billboard... ", 0.5f);

                ((AltTree)AssetDatabase.LoadAssetAtPath(assetPathAndName, typeof(AltTree))).getTextureBillboard(false);
                EditorUtility.SetDirty((AltTree)AssetDatabase.LoadAssetAtPath(assetPathAndName, typeof(AltTree)));
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                DestroyImmediate(goTemp);

                if (!isImport)
                    Selection.activeObject = (AltTree)AssetDatabase.LoadAssetAtPath(assetPathAndName, typeof(AltTree));
                else
                {
                    isImport = false;

                    ate.terrainTempImport = terrainTempImport;
                    ate.isDeleteTreesFromTerrain = isDeleteTreesFromTerrain;
                    ate.isDeleteTreesFromAltTrees = isDeleteTreesFromAltTrees;
                    tree = null;
                    status = 0;
                    texture = null;
                    typeTree = 0;
                    mats.Clear();
                    matsBark.Clear();
                    matsLeaves.Clear();
                    isObject = false;
                    EditorUtility.ClearProgressBar();
                    ate.Import();
                    return;
                }


                EditorUtility.ClearProgressBar();
            }
            else
            {
                if (dataLinks.getAltTree(tree) != null)
                    status = 2;
                else
                {
                    float sizeTemp = 0f;

                    Material[] matsTemp;
                    LODGroup lodGroupTemp = tree.GetComponent<LODGroup>();
                    mats.Clear();


                    if (lodGroupTemp != null)
                    {
                        //LOD[] lodsTemp = lodGroupTemp.GetLODs();

                        SerializedObject objj = new SerializedObject(lodGroupTemp);
                        SerializedProperty property2 = null;

                        //LOD[] lodsTemp = lodGroupTemp.GetLODs();
                        Renderer[] lodsTemp = new Renderer[lodGroupTemp.lodCount];

                        for (int t = 0; t < lodGroupTemp.lodCount; t++)
                        {
                            SerializedProperty property = objj.FindProperty("m_LODs.Array.data[" + t + "].renderers");
                            if (property.arraySize > 0)
                                lodsTemp[t] = (property.GetArrayElementAtIndex(0).FindPropertyRelative("renderer").objectReferenceValue as Renderer);
                        }

                        property2 = objj.FindProperty("m_LODs.Array.data[0].fadeMode");
                        if(property2 == null)
                            property2 = objj.FindProperty("m_FadeMode");


                        if (property2 != null && property2.intValue == 2)
                            typeTree = 1;

                        if (lodsTemp != null)
                        {
                            for (int j = 0; j < lodsTemp.Length; j++)
                            {
                                if (lodsTemp[j] != null)
                                {
                                    if (lodsTemp[j].gameObject.GetComponent<MeshRenderer>() != null && lodsTemp[j].gameObject.GetComponent<MeshRenderer>().sharedMaterials.Length > 0)
                                    {
                                        if(j == 0)
                                            sizeTemp = Mathf.Max(lodsTemp[j].GetComponent<MeshRenderer>().bounds.size.y, Mathf.Max(lodsTemp[j].GetComponent<MeshRenderer>().bounds.size.x, lodsTemp[j].GetComponent<MeshRenderer>().bounds.size.z));
                                        
                                        matsTemp = lodsTemp[j].gameObject.GetComponent<MeshRenderer>().sharedMaterials;
                                        for (int g = 0; g < matsTemp.Length; g++)
                                        {
                                            if (!mats.Contains(matsTemp[g]))
                                                mats.Add(matsTemp[g]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tree.GetComponent<Tree>() != null)
                            typeTree = 2;
                        else
                            typeTree = 3;

                        if (tree.GetComponent<MeshRenderer>() != null && tree.GetComponent<MeshRenderer>().sharedMaterials.Length > 0)
                        {
                            sizeTemp = Mathf.Max(tree.GetComponent<MeshRenderer>().bounds.size.y, Mathf.Max(tree.GetComponent<MeshRenderer>().bounds.size.x, tree.GetComponent<MeshRenderer>().bounds.size.z));
                            
                            matsTemp = tree.GetComponent<MeshRenderer>().sharedMaterials;
                            for (int g = 0; g < matsTemp.Length; g++)
                            {
                                if (!mats.Contains(matsTemp[g]))
                                    mats.Add(matsTemp[g]);
                            }
                        }
                    }

                    float sizeT = 0f;

                    sizeT = sizeTemp * 130f;
                    if (sizeT < 128)
                        sizeT = 128;
                    else if (sizeT < 256)
                        sizeT = 256;
                    else if(sizeT < 512)
                        sizeT = 512;
                    else if (sizeT < 1024)
                        sizeT = 1024;
                    else if(sizeT < 2048)
                        sizeT = 2048;
                    else if (sizeT > 2048)
                        sizeT = 2048;

                    textureSize = (int)sizeT;
                    normalmapSize = (int)(textureSize / 2f);
                    if (normalmapSize < 128)
                        normalmapSize = 128;

                    for (int i = mats.Count - 1; i >= 0; i--)
                    {
                        bool isBark = false;
                        string nameTemp = mats[i].name.ToLower();

                        if (nameTemp.IndexOf("branch") >= 0 || nameTemp.IndexOf("bark") >= 0)
                            isBark = true;
                        else
                        {
                            string[] shaderKeywords = mats[i].shaderKeywords;
                            for (int j = 0; j < shaderKeywords.Length; j++)
                            {
                                if (shaderKeywords[j].ToLower().IndexOf("branch") >= 0 || shaderKeywords[j].ToLower().IndexOf("bark") >= 0)
                                    isBark = true;
                            }
                        }

                        if (isBark)
                            matsBark.Add(mats[i]);
                        else
                            matsLeaves.Add(mats[i]);
                    }
                }

                ImportTreeWindow w = (ImportTreeWindow)EditorWindow.GetWindow(typeof(ImportTreeWindow), true, "Import Tree");
                if (status == 2)
                {
                    w.minSize = new Vector2(500, 170);
                    w.maxSize = new Vector2(500, 170);
                }
                else
                {
                    w.minSize = new Vector2(500, 350);
                    w.maxSize = new Vector2(500, 350);
                }
                AltSystems.AltBackup.Editor.CenterOnMainEditorWindow.CenterOnMainWin(w);
            }
        }

        static void getDataLinks()
        {
            if (dataLinks == null)
            {
                if (!System.IO.Directory.Exists("Assets/Plugins/AltSystems/AltTrees/DataBase"))
                {
                    System.IO.Directory.CreateDirectory("Assets/Plugins/AltSystems/AltTrees/DataBase");
                }

                if (!System.IO.File.Exists("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesDataLinks.asset"))
                {
                    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<AltTreesDataLinks>(), "Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesDataLinks.asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                dataLinks = (AltTreesDataLinks)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesDataLinks.asset", typeof(AltTreesDataLinks));
            }
        }
    }
}