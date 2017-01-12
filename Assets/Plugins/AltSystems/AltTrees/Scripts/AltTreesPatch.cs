using UnityEngine;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace AltSystems.AltTrees
{
    [System.Serializable]
    public class AltTreesPatch
    {
        public AltTreePrototypes[] prototypes = new AltTreePrototypes[0];
        [HideInInspector]
        [System.NonSerialized]
        public AltTreesTrees[] trees = new AltTreesTrees[0];
        [HideInInspector]
        [System.NonSerialized]
        public AltTreesTrees[] treesNoGroup = new AltTreesTrees[0];

        [System.NonSerialized]
        public int treesCount = 0;
        [System.NonSerialized]
        public int treesEmptyCount = 0;
        [System.NonSerialized]
        public int[] treesEmpty;
        [System.NonSerialized]
        public int treesNoGroupCount = 0;
        [System.NonSerialized]
        public int treesNoGroupEmptyCount = 0;
        [System.NonSerialized]
        public int[] treesNoGroupEmpty;

        #if UNITY_EDITOR
            AltTreesDataLinks dataLinks = null;
        #endif
        


        public float sizeQuad = 0f;
        public Vector2 minPosXZ = new Vector2(2000, 2000);
        public Vector2 maxPosXZ = new Vector2(0, 0);
    
        public TextAsset treesData;
        public TextAsset treesNoGroupData;
    
        public int maxLOD = 5;
	    public int startBillboardsLOD = 3;
        public bool draw = true;
        public bool drawDebugPutches = false;
	    public bool drawDebugPutchesStar = false;
	    public GameObject cube;
        public AltTreesManager altTreesManager = null;
        public AltTreesManagerData altTreesManagerData;
        public AltTrees altTrees = null;

        public Vector3 step = new Vector3(0,0,0);
        public int stepX = 0;
        public int stepY = 0;

        [System.NonSerialized]
        public int brushSize = 5;
        [System.NonSerialized]
        public int treeCount = 5;
        [System.NonSerialized]
        public int idPlacingPrototype = 0;
        [System.NonSerialized]
        public int speedPlace = 2;
        [System.NonSerialized]
        public bool randomRotation = true;


        [System.NonSerialized]
        public int altTreesId = -1;
        [System.NonSerialized]
        public List<GameObject> rendersDebug = new List<GameObject>();

        public List<AltTreesTrees> tempTrees = new List<AltTreesTrees>();
        
        #if UNITY_EDITOR
            string pathStr = "";
        #endif

        public AltTreesPatch(int _stepX, int _stepY)
        {
            step.x = _stepX;
            step.z = _stepY;
            stepX = _stepX;
            stepY = _stepY;
        }


        public void Init(AltTreesManager _altTreesManager, AltTrees _altTrees, AltTreesManagerData _altTreesManagerData)
        {
            altTreesManager = _altTreesManager;
            altTrees = _altTrees;
            altTreesManagerData = _altTreesManagerData;

            if (treesData == null)
            {
                #if UNITY_EDITOR
                    treesData = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesData/" + altTrees.getIdManager() + "/treesData_" + stepX + "_" + stepY + ".bytes", typeof(TextAsset));
                    EditorUtility.SetDirty(altTrees.altTreesManagerData);
                #endif
                if (treesData == null)
                {
                    treesCount = 0;
                    treesEmptyCount = 0;
                
                    trees = null;
                    trees = new AltTreesTrees[0];
                    treesEmpty = new int[0];
                }
            }
            if (treesNoGroupData == null)
            {
                #if UNITY_EDITOR
                    treesNoGroupData = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesData/" + altTrees.getIdManager() + "/treesData_" + stepX + "_" + stepY + "_objs.bytes", typeof(TextAsset));
                    EditorUtility.SetDirty(altTrees.altTreesManagerData);
                #endif
                if (treesNoGroupData == null)
                {
                    treesNoGroupCount = 0;
                    treesNoGroupEmptyCount = 0;

                    treesNoGroup = null;
                    treesNoGroup = new AltTreesTrees[0];
                    treesNoGroupEmpty = new int[0];
                }
            }
            if (treesData != null)
            {
                byte[] bytesTemp;
                #if UNITY_EDITOR
                    pathStr = AssetDatabase.GetAssetPath(treesData);
                    if(pathStr != "")
                        bytesTemp = File.ReadAllBytes(pathStr);
                    else
                        bytesTemp = treesData.bytes;
                #else
                    bytesTemp = treesData.bytes;
                #endif
                int version = AltUtilities.ReadBytesInt(bytesTemp, 0);

                if (version == 1)
                {
                    treesCount = AltUtilities.ReadBytesInt(bytesTemp, 4);
                    treesEmptyCount = AltUtilities.ReadBytesInt(bytesTemp, 8);

                    trees = null;
                    trees = new AltTreesTrees[treesCount];
                    treesEmpty = new int[treesEmptyCount];

                    Vector3 _pos = new Vector3();
                    int _idPrototype;
                    Color _color = new Color();
                    Color _colorBark = new Color();
                    float _rotation;
                    float _heightScale;
                    float _widthScale;

                    for (int i = 0; i < treesCount; i++)
                    {
                        _pos = AltUtilities.ReadBytesVector3(bytesTemp, 12 + i * 60 + 0);
                        _idPrototype = AltUtilities.ReadBytesInt(bytesTemp, 12 + i * 60 + 12);
                        _color = AltUtilities.ReadBytesColor(bytesTemp, 12 + i * 60 + 16);
                        _colorBark = AltUtilities.ReadBytesColor(bytesTemp, 12 + i * 60 + 32);
                        _rotation = AltUtilities.ReadBytesFloat(bytesTemp, 12 + i * 60 + 48);
                        _heightScale = AltUtilities.ReadBytesFloat(bytesTemp, 12 + i * 60 + 52);
                        _widthScale = AltUtilities.ReadBytesFloat(bytesTemp, 12 + i * 60 + 56);

                        AltTreesTrees att = new AltTreesTrees(_pos, i, _idPrototype, _color, _colorBark, Color.white, _rotation, _heightScale, _widthScale, this);

                        trees[i] = att;
                    }

                    for (int i = 0; i < treesEmptyCount; i++)
                    {
                        treesEmpty[i] = AltUtilities.ReadBytesInt(bytesTemp, 12 + treesCount * 60 + i * 4);
                        if (treesEmpty[i] < trees.Length)
                            trees[treesEmpty[i]].noNull = false;
                        else
                            Debug.Log("treesEmpty[i] = " + treesEmpty[i] + ", trees.Length = " + trees.Length);
                    }
                }

                bytesTemp = null;
            }
            if (treesNoGroupData != null)
            {
                byte[] bytesTemp;
                #if UNITY_EDITOR
                    pathStr = AssetDatabase.GetAssetPath(treesNoGroupData);
                    if(pathStr != "")
                        bytesTemp = File.ReadAllBytes(pathStr);
                    else
                        bytesTemp = treesNoGroupData.bytes;
                #else
                    bytesTemp = treesNoGroupData.bytes;
                #endif
                int version = AltUtilities.ReadBytesInt(bytesTemp, 0);

                if (version == 1)
                {
                    treesNoGroupCount = AltUtilities.ReadBytesInt(bytesTemp, 4);
                    treesNoGroupEmptyCount = AltUtilities.ReadBytesInt(bytesTemp, 8);

                    treesNoGroup = null;
                    treesNoGroup = new AltTreesTrees[treesNoGroupCount];
                    treesNoGroupEmpty = new int[treesNoGroupEmptyCount];

                    Vector3 _pos = new Vector3();
                    int _idPrototype;
                    Color _color = new Color();
                    Color _colorBark = new Color();
                    float _rotation;
                    float _heightScale;
                    float _widthScale;

                    for (int i = 0; i < treesNoGroupCount; i++)
                    {
                        _pos = AltUtilities.ReadBytesVector3(bytesTemp, 12 + i * 60 + 0);
                        _idPrototype = AltUtilities.ReadBytesInt(bytesTemp, 12 + i * 60 + 12);
                        _color = AltUtilities.ReadBytesColor(bytesTemp, 12 + i * 60 + 16);
                        _colorBark = AltUtilities.ReadBytesColor(bytesTemp, 12 + i * 60 + 32);
                        _rotation = AltUtilities.ReadBytesFloat(bytesTemp, 12 + i * 60 + 48);
                        _heightScale = AltUtilities.ReadBytesFloat(bytesTemp, 12 + i * 60 + 52);
                        _widthScale = AltUtilities.ReadBytesFloat(bytesTemp, 12 + i * 60 + 56);

                        AltTreesTrees att = new AltTreesTrees(_pos, i, _idPrototype, _color, _colorBark, Color.white, _rotation, _heightScale, _widthScale, this);

                        treesNoGroup[i] = att;
                    }

                    for (int i = 0; i < treesNoGroupEmptyCount; i++)
                    {
                        treesNoGroupEmpty[i] = AltUtilities.ReadBytesInt(bytesTemp, 12 + treesNoGroupCount * 60 + i * 4);
                        if (treesNoGroupEmpty[i] < treesNoGroup.Length)
                            treesNoGroup[treesNoGroupEmpty[i]].noNull = false;
                        else
                            Debug.Log("treesNoGroupEmpty[i] = " + treesNoGroupEmpty[i] + ", treesNoGroup.Length = " + treesNoGroup.Length);
                    }
                }

                bytesTemp = null;
            }
            System.GC.Collect();

            bool isStop = false;
            for (int i = 0; i < prototypes.Length; i++)
            {
                if (prototypes[i].tree != null && prototypes[i].tree.isObject != prototypes[i].isObject)
                {
                    checkTreeOrObject(i);
                    isStop = true;
                }

                #if UNITY_EDITOR
                {
                    if (prototypes[i].tree != null)
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

                        if(dataLinks.getId(prototypes[i].tree) == -1)
                        {
                            string[] filesTemp = Directory.GetFiles("Assets", prototypes[i].tree.name + ".spm", SearchOption.AllDirectories);
                            GameObject goT = null;
                            if(filesTemp.Length > 0 && filesTemp[0].Length > 0)
                                goT = (GameObject)AssetDatabase.LoadAssetAtPath(filesTemp[0], typeof(GameObject));

                            dataLinks.addTree(goT, prototypes[i].tree, prototypes[i].tree.id);

                            

                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                            EditorUtility.SetDirty(dataLinks);
                        }
                    }
                }
                #endif
            }

            if (isStop)
            {
                Init(_altTreesManager, _altTrees, _altTreesManagerData);
            }
        }

        public void EditDataFile(bool isTreesNoGroup, List<int> changedTrees, int addedTreesCount, List<int> removedTrees, int editingTree = -1)
        {
            #if UNITY_EDITOR
            {
                if ((changedTrees != null && changedTrees.Count > 0) || addedTreesCount > 0 || (removedTrees != null && removedTrees.Count > 0) || editingTree!=-1)
                {
                    byte[] bytes4Temp = new byte[4];
                    byte[] bytes60Temp = new byte[60];
                    if (!isTreesNoGroup)
                    {
                        if (!File.Exists("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesData/" + altTrees.getIdManager() + "/treesData_" + stepX + "_" + stepY + ".bytes"))
                        {
                            using (BinaryWriter writer = new BinaryWriter(File.Open("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesData/" + altTrees.getIdManager() + "/treesData_" + stepX + "_" + stepY + ".bytes", FileMode.Create)))
                            {
                                byte[] bytes12Temp = new byte[12];
                                AltUtilities.WriteBytes(1, bytes12Temp, 0);
                                AltUtilities.WriteBytes(0, bytes12Temp, 4);
                                AltUtilities.WriteBytes(0, bytes12Temp, 8);
                                writer.Write(bytes12Temp);
                            }
                            #if UNITY_EDITOR
                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();
                                treesData = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesData/" + altTrees.getIdManager() + "/treesData_" + stepX + "_" + stepY + ".bytes", typeof(TextAsset));
                                EditorUtility.SetDirty(altTrees.altTreesManagerData);
                            #endif
                        }
                        using (BinaryWriter writer = new BinaryWriter(File.Open("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesData/" + altTrees.getIdManager() + "/treesData_" + stepX + "_" + stepY + ".bytes", FileMode.Open)))
                        {
                            if(addedTreesCount > 0)
                            {
                                AltUtilities.WriteBytes(treesCount, bytes4Temp, 0);
                                writer.Seek(4, SeekOrigin.Begin);
                                writer.Write(bytes4Temp);
                                writer.Seek(12 + 60 * (treesCount - addedTreesCount), SeekOrigin.Begin);
                                for (int i = treesCount - addedTreesCount; i < treesCount; i++)
                                {
                                    AltUtilities.WriteBytes(trees[i].pos, bytes60Temp, 0);
                                    AltUtilities.WriteBytes(trees[i].idPrototype, bytes60Temp, 12);
                                    AltUtilities.WriteBytes(trees[i].color, bytes60Temp, 16);
                                    AltUtilities.WriteBytes(trees[i].colorBark, bytes60Temp, 32);
                                    AltUtilities.WriteBytes(trees[i].rotation, bytes60Temp, 48);
                                    AltUtilities.WriteBytes(trees[i].heightScale, bytes60Temp, 52);
                                    AltUtilities.WriteBytes(trees[i].widthScale, bytes60Temp, 56);
                                    writer.Write(bytes60Temp);
                                }

                                writer.Seek(12 + 60 * treesCount, SeekOrigin.Begin);
                                for (int i = 0; i < treesEmptyCount; i++)
                                {
                                    AltUtilities.WriteBytes(treesEmpty[i], bytes4Temp, 0);
                                    writer.Write(bytes4Temp);
                                }
                            }
                            if(changedTrees != null && changedTrees.Count > 0)
                            {
                                AltUtilities.WriteBytes(treesEmptyCount, bytes4Temp, 0);
                                writer.Seek(8, SeekOrigin.Begin);
                                writer.Write(bytes4Temp);
                                writer.Seek(12 + 60 * treesCount, SeekOrigin.Begin);

                                for (int i = 0; i < treesEmptyCount; i++)
                                {
                                    AltUtilities.WriteBytes(treesEmpty[i], bytes4Temp, 0);
                                    writer.Write(bytes4Temp);
                                }

                                for (int i = 0; i < changedTrees.Count; i++)
                                {
                                    writer.Seek(12 + changedTrees[i] * 60, SeekOrigin.Begin);

                                    AltUtilities.WriteBytes(trees[changedTrees[i]].pos, bytes60Temp, 0);
                                    AltUtilities.WriteBytes(trees[changedTrees[i]].idPrototype, bytes60Temp, 12);
                                    AltUtilities.WriteBytes(trees[changedTrees[i]].color, bytes60Temp, 16);
                                    AltUtilities.WriteBytes(trees[changedTrees[i]].colorBark, bytes60Temp, 32);
                                    AltUtilities.WriteBytes(trees[changedTrees[i]].rotation, bytes60Temp, 48);
                                    AltUtilities.WriteBytes(trees[changedTrees[i]].heightScale, bytes60Temp, 52);
                                    AltUtilities.WriteBytes(trees[changedTrees[i]].widthScale, bytes60Temp, 56);
                                    writer.Write(bytes60Temp);
                                }
                                changedTrees.Clear();
                            }
                            if(removedTrees != null && removedTrees.Count > 0)
                            {
                                treesEmptyCount += removedTrees.Count;
                                for(int i = 0; i < treesEmpty.Length; i++)
                                {
                                    removedTrees.Add(treesEmpty[i]);
                                }
                                treesEmpty = removedTrees.ToArray();

                                AltUtilities.WriteBytes(treesEmptyCount, bytes4Temp, 0);
                                writer.Seek(8, SeekOrigin.Begin);
                                writer.Write(bytes4Temp);
                                writer.Seek(12 + 60 * treesCount, SeekOrigin.Begin);
                                for (int i = 0; i < treesEmptyCount; i++)
                                {
                                    AltUtilities.WriteBytes(treesEmpty[i], bytes4Temp, 0);
                                    writer.Write(bytes4Temp);
                                }
                            }
                            if(editingTree != -1)
                            {
                                writer.Seek(12 + editingTree * 60, SeekOrigin.Begin);

                                AltUtilities.WriteBytes(trees[editingTree].pos, bytes60Temp, 0);
                                AltUtilities.WriteBytes(trees[editingTree].idPrototype, bytes60Temp, 12);
                                AltUtilities.WriteBytes(trees[editingTree].color, bytes60Temp, 16);
                                AltUtilities.WriteBytes(trees[editingTree].colorBark, bytes60Temp, 32);
                                AltUtilities.WriteBytes(trees[editingTree].rotation, bytes60Temp, 48);
                                AltUtilities.WriteBytes(trees[editingTree].heightScale, bytes60Temp, 52);
                                AltUtilities.WriteBytes(trees[editingTree].widthScale, bytes60Temp, 56);
                                writer.Write(bytes60Temp);
                            }
                        }
                    }
                    else
                    {
                        if (!File.Exists("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesData/" + altTrees.getIdManager() + "/treesData_" + stepX + "_" + stepY + "_objs.bytes"))
                        {
                            using (BinaryWriter writer = new BinaryWriter(File.Open("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesData/" + altTrees.getIdManager() + "/treesData_" + stepX + "_" + stepY + "_objs.bytes", FileMode.Create)))
                            {
                                byte[] bytes12Temp = new byte[12];
                                AltUtilities.WriteBytes(1, bytes12Temp, 0);
                                AltUtilities.WriteBytes(0, bytes12Temp, 4);
                                AltUtilities.WriteBytes(0, bytes12Temp, 8);
                                writer.Write(bytes12Temp);
                            }
                            #if UNITY_EDITOR
                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();
                                treesNoGroupData = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesData/" + altTrees.getIdManager() + "/treesData_" + stepX + "_" + stepY + "_objs.bytes", typeof(TextAsset));
                                EditorUtility.SetDirty(altTrees.altTreesManagerData);
                            #endif
                        }
                        using (BinaryWriter writer = new BinaryWriter(File.Open("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesData/" + altTrees.getIdManager() + "/treesData_" + stepX + "_" + stepY + "_objs.bytes", FileMode.Open)))
                        {
                            if (addedTreesCount > 0)
                            {
                                AltUtilities.WriteBytes(treesNoGroupCount, bytes4Temp, 0);
                                writer.Seek(4, SeekOrigin.Begin);
                                writer.Write(bytes4Temp);
                                writer.Seek(12 + 60 * (treesNoGroupCount - addedTreesCount), SeekOrigin.Begin);

                                for (int i = treesNoGroupCount - addedTreesCount; i < treesNoGroupCount; i++)
                                {
                                    AltUtilities.WriteBytes(treesNoGroup[i].pos, bytes60Temp, 0);
                                    AltUtilities.WriteBytes(treesNoGroup[i].idPrototype, bytes60Temp, 12);
                                    AltUtilities.WriteBytes(treesNoGroup[i].color, bytes60Temp, 16);
                                    AltUtilities.WriteBytes(treesNoGroup[i].colorBark, bytes60Temp, 32);
                                    AltUtilities.WriteBytes(treesNoGroup[i].rotation, bytes60Temp, 48);
                                    AltUtilities.WriteBytes(treesNoGroup[i].heightScale, bytes60Temp, 52);
                                    AltUtilities.WriteBytes(treesNoGroup[i].widthScale, bytes60Temp, 56);
                                    writer.Write(bytes60Temp);
                                }

                                writer.Seek(12 + 60 * treesNoGroupCount, SeekOrigin.Begin);
                                for (int i = 0; i < treesNoGroupEmptyCount; i++)
                                {
                                    AltUtilities.WriteBytes(treesNoGroupEmpty[i], bytes4Temp, 0);
                                    writer.Write(bytes4Temp);
                                }
                            }
                            if (changedTrees != null && changedTrees.Count > 0)
                            {
                                AltUtilities.WriteBytes(treesNoGroupEmptyCount, bytes4Temp, 0);
                                writer.Seek(8, SeekOrigin.Begin);
                                writer.Write(bytes4Temp);
                                writer.Seek(12 + 60 * treesNoGroupCount, SeekOrigin.Begin);

                                for (int i = 0; i < treesNoGroupEmptyCount; i++)
                                {
                                    AltUtilities.WriteBytes(treesNoGroupEmpty[i], bytes4Temp, 0);
                                    writer.Write(bytes4Temp);
                                }

                                for (int i = 0; i < changedTrees.Count; i++)
                                {
                                    writer.Seek(12 + changedTrees[i] * 60, SeekOrigin.Begin);

                                    AltUtilities.WriteBytes(treesNoGroup[changedTrees[i]].pos, bytes60Temp, 0);
                                    AltUtilities.WriteBytes(treesNoGroup[changedTrees[i]].idPrototype, bytes60Temp, 12);
                                    AltUtilities.WriteBytes(treesNoGroup[changedTrees[i]].color, bytes60Temp, 16);
                                    AltUtilities.WriteBytes(treesNoGroup[changedTrees[i]].colorBark, bytes60Temp, 32);
                                    AltUtilities.WriteBytes(treesNoGroup[changedTrees[i]].rotation, bytes60Temp, 48);
                                    AltUtilities.WriteBytes(treesNoGroup[changedTrees[i]].heightScale, bytes60Temp, 52);
                                    AltUtilities.WriteBytes(treesNoGroup[changedTrees[i]].widthScale, bytes60Temp, 56);
                                    writer.Write(bytes60Temp);
                                }
                                changedTrees.Clear();
                            }
                            if (removedTrees != null && removedTrees.Count > 0)
                            {
                                treesNoGroupEmptyCount += removedTrees.Count;
                                for (int i = 0; i < treesNoGroupEmpty.Length; i++)
                                {
                                    removedTrees.Add(treesNoGroupEmpty[i]);
                                }
                                treesNoGroupEmpty = removedTrees.ToArray();

                                AltUtilities.WriteBytes(treesNoGroupEmptyCount, bytes4Temp, 0);
                                writer.Seek(8, SeekOrigin.Begin);
                                writer.Write(bytes4Temp);
                                writer.Seek(12 + 60 * treesNoGroupCount, SeekOrigin.Begin);
                                for (int i = 0; i < treesNoGroupEmptyCount; i++)
                                {
                                    AltUtilities.WriteBytes(treesNoGroupEmpty[i], bytes4Temp, 0);
                                    writer.Write(bytes4Temp);
                                }
                            }
                            if (editingTree != -1)
                            {
                                writer.Seek(12 + editingTree * 60, SeekOrigin.Begin);

                                AltUtilities.WriteBytes(treesNoGroup[editingTree].pos, bytes60Temp, 0);
                                AltUtilities.WriteBytes(treesNoGroup[editingTree].idPrototype, bytes60Temp, 12);
                                AltUtilities.WriteBytes(treesNoGroup[editingTree].color, bytes60Temp, 16);
                                AltUtilities.WriteBytes(treesNoGroup[editingTree].colorBark, bytes60Temp, 32);
                                AltUtilities.WriteBytes(treesNoGroup[editingTree].rotation, bytes60Temp, 48);
                                AltUtilities.WriteBytes(treesNoGroup[editingTree].heightScale, bytes60Temp, 52);
                                AltUtilities.WriteBytes(treesNoGroup[editingTree].widthScale, bytes60Temp, 56);
                                writer.Write(bytes60Temp);
                            }
                        }
                    }
                }
            }
            #endif
        }


        public void addTrees(AddTreesStruct[] positions, bool randomRotation, bool isRandomHeight, float height, float heightRandom,
                            bool lockWidthToHeight, bool isRandomWidth, float width, float widthRandom, Color32 hueLeaves, Color32 hueBark, bool isRandomHueLeaves, bool isRandomHueBark)
        {
            altTreesManager.addTrees(positions, altTreesId, randomRotation, isRandomHeight, height, heightRandom, lockWidthToHeight, isRandomWidth, width, widthRandom, hueLeaves, hueBark, isRandomHueLeaves, isRandomHueBark);
        }

        public void addTreesImport(ImportTreesStruct[] trees)
        {
            altTreesManager.addTreesImport(trees, altTreesId);
        }

        public bool removeTrees(Vector2 pos, float radius, List<int> removedTrees, List<int> removedTreesNoGroup, AltTree _tree = null)
        {
            if (altTreesManager.removeTrees(pos, radius, this, removedTrees, removedTreesNoGroup, _tree))
            {
                recalculateBound();
                return true;
            }
            else
                return false;
        }

        public bool removeTrees(AltTreesTrees[] att, Vector2 pos, float sizeX, float sizeZ, List<int> removedTrees, List<int> removedTreesNoGroup)
        {
            if (altTreesManager.removeTrees(att, pos, sizeX, sizeZ, this, removedTrees, removedTreesNoGroup))
            {
                recalculateBound();
                return true;
            }
            else
                return false;
        }

        public AltTreesTrees[] getTreesForExport(Vector2 pos, float sizeX, float sizeZ)
        {
            return altTreesManager.getTreesForExport(pos, sizeX, sizeZ, this);
        }

        public void checkTreePrototype(int id, AltTree tree)
        {
            bool isOk = false;
            for (int i = 0; i < prototypes.Length; i++)
            {
                if (prototypes[i].isEnabled && prototypes[i].tree.id == id)
                {
                    isOk = true;
                    break;
                }
            }
            if (!isOk)
            {
                AltTreePrototypes[] protTemp = prototypes;
                prototypes = new AltTreePrototypes[protTemp.Length + 1];
                for (int i = 0; i < protTemp.Length; i++)
                {
                    prototypes[i] = protTemp[i];
                }
                prototypes[prototypes.Length - 1] = new AltTreePrototypes();
                prototypes[prototypes.Length - 1].tree = tree;
                prototypes[prototypes.Length - 1].isObject = tree.isObject;
                altTreesManager.addInitObjPool(tree);

                #if UNITY_EDITOR
                    EditorUtility.SetDirty(altTrees.altTreesManagerData);
                #endif
            }
        }

        public void checkTreeOrObject(int id)
        {
            if (prototypes[id].isObject != prototypes[id].tree.isObject)
            {
                AltTreesTrees[] treesTemp = trees;
                AltTreesTrees[] objectsTemp = treesNoGroup;
                int count = 0;
                bool boolTemp = false;

                if (prototypes[id].isObject)
                {
                    for (int i = 0; i < treesNoGroupCount; i++)
                    {
                        if (objectsTemp[i].idPrototype == prototypes[id].tree.id)
                        {
                            boolTemp = false;
                            for (int j = 0; j < treesNoGroupEmptyCount; j++)
                            {
                                if(treesNoGroupEmpty[j] == i)
                                {
                                    boolTemp = true;
                                    break;
                                }
                            }
                            if(!boolTemp)
                                count++;
                        }
                    }
                    treesNoGroup = new AltTreesTrees[treesNoGroupCount - treesNoGroupEmptyCount - count];
                    trees = new AltTreesTrees[treesCount - treesEmptyCount + count];

                    int treesIndx = 0;
                    int treesNoGroupIndx = 0;

                    for (int i = 0; i < objectsTemp.Length; i++)
                    {
                        boolTemp = false;
                        for (int j = 0; j < treesNoGroupEmptyCount; j++)
                        {
                            if (treesNoGroupEmpty[j] == i)
                            {
                                boolTemp = true;
                                break;
                            }
                        }
                        if (!boolTemp)
                        {
                            if (objectsTemp[i].idPrototype == prototypes[id].tree.id)
                            {
                                trees[treesIndx] = objectsTemp[i];
                                treesIndx++;
                            }
                            else
                            {
                                treesNoGroup[treesNoGroupIndx] = objectsTemp[i];
                                treesNoGroupIndx++;
                            }
                        }
                    }
                    for (int i = 0; i < treesTemp.Length; i++)
                    {
                        boolTemp = false;
                        for (int j = 0; j < treesEmptyCount; j++)
                        {
                            if (treesEmpty[j] == i)
                            {
                                boolTemp = true;
                                break;
                            }
                        }
                        if (!boolTemp)
                        {
                            trees[treesIndx] = treesTemp[i];
                            treesIndx++;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < treesCount; i++)
                    {
                        if (treesTemp[i].idPrototype == prototypes[id].tree.id)
                        {
                            boolTemp = false;
                            for (int j = 0; j < treesEmptyCount; j++)
                            {
                                if (treesEmpty[j] == i)
                                {
                                    boolTemp = true;
                                    break;
                                }
                            }
                            if (!boolTemp)
                                count++;
                        }
                    }
                    trees = new AltTreesTrees[treesCount - treesEmptyCount - count];
                    treesNoGroup = new AltTreesTrees[treesNoGroupCount - treesNoGroupEmptyCount + count];

                    int treesIndx = 0;
                    int treesNoGroupIndx = 0;

                    for (int i = 0; i < treesTemp.Length; i++)
                    {
                        boolTemp = false;
                        for (int j = 0; j < treesEmptyCount; j++)
                        {
                            if (treesEmpty[j] == i)
                            {
                                boolTemp = true;
                                break;
                            }
                        }
                        if (!boolTemp)
                        {
                            if (treesTemp[i].idPrototype == prototypes[id].tree.id)
                            {
                                treesNoGroup[treesNoGroupIndx] = treesTemp[i];
                                treesNoGroupIndx++;
                            }
                            else
                            {
                                trees[treesIndx] = treesTemp[i];
                                treesIndx++;
                            }
                        }
                    }
                    for (int i = 0; i < objectsTemp.Length; i++)
                    {
                        boolTemp = false;
                        for (int j = 0; j < treesNoGroupEmptyCount; j++)
                        {
                            if (treesNoGroupEmpty[j] == i)
                            {
                                boolTemp = true;
                                break;
                            }
                        }
                        if (!boolTemp)
                        {
                            treesNoGroup[treesNoGroupIndx] = objectsTemp[i];
                            treesNoGroupIndx++;
                        }
                    }
                }
                prototypes[id].isObject = prototypes[id].tree.isObject;

                treesCount = trees.Length;
                treesEmptyCount = 0;
                treesEmpty = new int[0];

                treesNoGroupCount = treesNoGroup.Length;
                treesNoGroupEmptyCount = 0;
                treesNoGroupEmpty = new int[0];

                byte[] bytes4Temp = new byte[4];
                byte[] bytes60Temp = new byte[60];

                using (BinaryWriter writer = new BinaryWriter(File.Open("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesData/" + altTrees.getIdManager() + "/treesData_" + stepX + "_" + stepY + ".bytes", FileMode.Create)))
                {
                    AltUtilities.WriteBytes(1, bytes4Temp, 0);
                    writer.Write(bytes4Temp);
                    AltUtilities.WriteBytes(treesCount, bytes4Temp, 0);
                    writer.Write(bytes4Temp);
                    AltUtilities.WriteBytes(0, bytes4Temp, 0);
                    writer.Write(bytes4Temp);
                
                    for (int i = 0; i < treesCount; i++)
                    {
                        AltUtilities.WriteBytes(trees[i].pos, bytes60Temp, 0);
                        AltUtilities.WriteBytes(trees[i].idPrototype, bytes60Temp, 12);
                        AltUtilities.WriteBytes(trees[i].color, bytes60Temp, 16);
                        AltUtilities.WriteBytes(trees[i].colorBark, bytes60Temp, 32);
                        AltUtilities.WriteBytes(trees[i].rotation, bytes60Temp, 48);
                        AltUtilities.WriteBytes(trees[i].heightScale, bytes60Temp, 52);
                        AltUtilities.WriteBytes(trees[i].widthScale, bytes60Temp, 56);
                        writer.Write(bytes60Temp);
                    }
                }

                using (BinaryWriter writer = new BinaryWriter(File.Open("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesData/" + altTrees.getIdManager() + "/treesData_" + stepX + "_" + stepY + "_objs.bytes", FileMode.Create)))
                {
                    AltUtilities.WriteBytes(1, bytes4Temp, 0);
                    writer.Write(bytes4Temp);
                    AltUtilities.WriteBytes(treesNoGroupCount, bytes4Temp, 0);
                    writer.Write(bytes4Temp);
                    AltUtilities.WriteBytes(0, bytes4Temp, 0);
                    writer.Write(bytes4Temp);

                    for (int i = 0; i < treesNoGroupCount; i++)
                    {
                        AltUtilities.WriteBytes(treesNoGroup[i].pos, bytes60Temp, 0);
                        AltUtilities.WriteBytes(treesNoGroup[i].idPrototype, bytes60Temp, 12);
                        AltUtilities.WriteBytes(treesNoGroup[i].color, bytes60Temp, 16);
                        AltUtilities.WriteBytes(treesNoGroup[i].colorBark, bytes60Temp, 32);
                        AltUtilities.WriteBytes(treesNoGroup[i].rotation, bytes60Temp, 48);
                        AltUtilities.WriteBytes(treesNoGroup[i].heightScale, bytes60Temp, 52);
                        AltUtilities.WriteBytes(treesNoGroup[i].widthScale, bytes60Temp, 56);
                        writer.Write(bytes60Temp);
                    }
                }

                #if UNITY_EDITOR
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    EditorUtility.SetDirty(altTrees.altTreesManagerData);
                #endif
            }
        }

        public Vector3 getTreePosLocal(Vector3 pos, Vector3 jump, float sizePatch)
        {
            return (pos - (step - jump) * sizePatch) / sizePatch;
        }

        public void recalculateBound()
        {
            minPosXZ = new Vector2(1000000, 1000000);
            maxPosXZ = new Vector2(-1000000, -1000000);

            if (trees != null)
            {
                for (int j = 0; j < trees.Length; j++)
                {
                    if (trees[j] != null)
                    {
                        if (trees[j].pos.x < minPosXZ.x)
                            minPosXZ.x = trees[j].pos.x;
                        if (trees[j].pos.x > maxPosXZ.x)
                            maxPosXZ.x = trees[j].pos.x;
                        if (trees[j].pos.z < minPosXZ.y)
                            minPosXZ.y = trees[j].pos.z;
                        if (trees[j].pos.z > maxPosXZ.y)
                            maxPosXZ.y = trees[j].pos.z;
                    }
                }
            }
            if (treesNoGroup != null)
            {
                for (int j = 0; j < treesNoGroup.Length; j++)
                {
                    if (treesNoGroup[j] != null)
                    {
                        if (treesNoGroup[j].pos.x < minPosXZ.x)
                            minPosXZ.x = treesNoGroup[j].pos.x;
                        if (treesNoGroup[j].pos.x > maxPosXZ.x)
                            maxPosXZ.x = treesNoGroup[j].pos.x;
                        if (treesNoGroup[j].pos.z < minPosXZ.y)
                            minPosXZ.y = treesNoGroup[j].pos.z;
                        if (treesNoGroup[j].pos.z > maxPosXZ.y)
                            maxPosXZ.y = treesNoGroup[j].pos.z;
                    }
                }
            }

            sizeQuad = Mathf.Max(maxPosXZ.x - minPosXZ.x, maxPosXZ.y - minPosXZ.y);
        }

        public AltTree getAltTreePrototype(int idPrototype)
        {
            for (int j = 0; j < prototypes.Length; j++)
            {
                if (prototypes[j].tree.id == idPrototype)
                    return prototypes[j].tree;
            }
            return null;
        }
    }

    public class AddTreesStruct
    {
        public Vector3 pos;
        public AltTree altTree;

        public AddTreesStruct(Vector3 _pos, AltTree _altTree)
        {
            pos = _pos;
            altTree = _altTree;
        }
    }

    public class ImportTreesStruct
    {
        public Vector3 pos;
        public int idPlacingPrototype = -1;
        public Color color;
        public Color colorBark;
        public Color lightmapColor;
        public float rotation;
        public float heightScale;
        public float widthScale;
        public bool isObject = false;
        public AltTree altTree;

        public ImportTreesStruct(Vector3 _pos, int _idPlacingPrototype, Color _colorLeaves, Color _colorBark, Color _lightmapColor, float _rotation, float _heightScale, float _widthScale, bool _isObject, AltTree _altTree)
        {
            pos = _pos;
            idPlacingPrototype = _idPlacingPrototype;
            color = _colorLeaves;
            colorBark = _colorBark;
            lightmapColor = _lightmapColor;
            rotation = _rotation;
            heightScale = _heightScale;
            widthScale = _widthScale;
            isObject = _isObject;
            altTree = _altTree;
        }
    }

    [System.Serializable]
    public class AltTreePrototypes
    {
        [System.NonSerialized]
        public bool isEnabled = true;
        public bool isObject = false;
        public AltTree tree = null;
        public int idBundle = -1;
    }

    public class AltTreesTrees
    {
        public Vector3 pos;
        public Vector2 pos2D;
        public int idTree;
        public int idPrototype;
        public int idPrototypeIndex;

        public Color color = new Color(1, 1, 1, 0);
        public Color colorBark = new Color(1, 1, 1, 0);
        public Color lightmapColor;
        public float rotation;
        public float heightScale;
        public float widthScale;
        public float maxScaleSquare;

        [System.NonSerialized]
        public GameObject go;
        [System.NonSerialized]
        public GameObject collider;
        public int currentLOD = -1;
        public int currentCrossFadeId = -1;
        public int currentCrossFadeLOD = -1;
        public float crossFadeTime = 0f;
        [System.NonSerialized]
        public GameObject goCrossFade;
        [System.NonSerialized]
        public MeshRenderer crossFadeBillboardMeshRenderer;
        [System.NonSerialized]
        public MeshRenderer crossFadeTreeMeshRenderer;
        [System.NonSerialized]
        public int countCheckLODs = 0;

        public bool noNull = false;

        [System.NonSerialized]
        public AltTreesPatch altTreesPatch;

        public AltTreesTrees(Vector3 _pos, int _idTree, int _idPrototype, Color _colorLeaves, Color _colorBark, Color _lightmapColor, float _rotation, float _heightScale, float _widthScale, AltTreesPatch _altTreesPatch)
        {
            pos = _pos;
            pos2D = new Vector2(pos.x, pos.z);
            idTree = _idTree;
            idPrototype = _idPrototype;
            idPrototypeIndex = -1;

            color = _colorLeaves;
            colorBark = _colorBark;
            lightmapColor = _lightmapColor;
            rotation = _rotation;
            heightScale = _heightScale;
            widthScale = _widthScale;
            maxScaleSquare = Mathf.Max(heightScale, widthScale);
            maxScaleSquare *= maxScaleSquare;

            altTreesPatch = _altTreesPatch;

            noNull = true;
        }

        public AltTreesTrees(AltTreesTrees att, int _idTree)
        {
            pos = att.pos;
            pos2D = att.pos2D;
            idTree = _idTree;
            idPrototype = att.idPrototype;
            idPrototypeIndex = -1;

            color = att.color;
            lightmapColor = att.lightmapColor;
            rotation = att.rotation;
            heightScale = att.heightScale;
            widthScale = att.widthScale;
            maxScaleSquare = Mathf.Max(heightScale, widthScale);
            maxScaleSquare *= maxScaleSquare;

            altTreesPatch = att.altTreesPatch;

            noNull = true;

            currentLOD = att.currentLOD;
        }

        Vector3 temp;
        public Vector3 getPosWorld()
        {
            temp.x = (pos.x + altTreesPatch.step.x - altTreesPatch.altTreesManager.jump.x) * altTreesPatch.altTrees.altTreesManagerData.sizePatch;
            temp.y = (pos.y + altTreesPatch.step.y - altTreesPatch.altTreesManager.jump.y) * altTreesPatch.altTrees.altTreesManagerData.sizePatch;
            temp.z = (pos.z + altTreesPatch.step.z - altTreesPatch.altTreesManager.jump.z) * altTreesPatch.altTrees.altTreesManagerData.sizePatch;

            return temp;
        }

        public Vector2 get2DPosWorld()
        {
            temp.x = (pos.x + altTreesPatch.step.x - altTreesPatch.altTreesManager.jump.x) * altTreesPatch.altTrees.altTreesManagerData.sizePatch;
            temp.z = (pos.z + altTreesPatch.step.z - altTreesPatch.altTreesManager.jump.z) * altTreesPatch.altTrees.altTreesManagerData.sizePatch;

            return new Vector2(temp.x, temp.z);
        }

        public Vector3 getPosWorldBillboard()
        {
            temp.x = pos.x * altTreesPatch.altTrees.altTreesManagerData.sizePatch;
            temp.y = pos.y * altTreesPatch.altTrees.altTreesManagerData.sizePatch;
            temp.z = pos.z * altTreesPatch.altTrees.altTreesManagerData.sizePatch;

            return temp;
        }
    }
}


