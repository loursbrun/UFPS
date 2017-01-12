using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using AltSystems.AltTrees;

namespace AltSystems.AltTrees.Editor
{
    [CustomEditor(typeof(AltTreeInstance))]
    [CanEditMultipleObjects]
    public class AltTreeInstance_Editor : UnityEditor.Editor
    {
        AltTreeInstance obj = null;
        AltTreesDataLinks dataLinks = null;

        void OnDisable()
        {
            obj = (AltTreeInstance)target;
            getDataLinks();

            #if UNITY_5_5 || UNITY_5_6 || UNITY_5_7
                
            #else
                if (obj != null)
                    EditorUtility.SetSelectedWireframeHidden(obj.gameObject.GetComponent<MeshRenderer>(), false);
            #endif
        
            AltTreesPatch[] atArray = obj.manager.saveTrees();

            if (atArray != null && atArray.Length > 0)
            {
                for (int i = 0; i < atArray.Length; i++)
                {
                    bool isStopDelete = false;
                    for (int j = 0; j < atArray[i].trees.Length; j++)
                    {
                        if (atArray[i].trees[j] != null && atArray[i].trees[j].noNull)
                        {
                            isStopDelete = true;
                            break;
                        }
                    }
                    for (int j = 0; j < atArray[i].treesNoGroup.Length; j++)
                    {
                        if (atArray[i].treesNoGroup[j] != null && atArray[i].treesNoGroup[j].noNull)
                        {
                            isStopDelete = true;
                            break;
                        }
                    }

                    if (!isStopDelete)
                        removePatch(atArray[i]);
                    else
                        EditorUtility.SetDirty(atArray[i].altTreesManagerData);
                }
            }

            bool isStop = false;
            AltTreeInstance ati = null;

            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                ati = Selection.transforms[i].GetComponent<AltTreeInstance>();
                if (ati != null)
                {
                    isStop = true;
                    break;
                }
            }

            for (int i = 0; i < obj.manager.altTreesMain.altTreesManagerData.patches.Length; i++)
            {
                if (obj.manager.altTreesMain.altTreesManagerData.patches[i].tempTrees != null && obj.manager.altTreesMain.altTreesManagerData.patches[i].tempTrees.Count > 0)
                {
                    int treesBillb = 0;
                    int treesNoBillb = 0;
                    int treesBillbSch = 0;
                    int treesNoBillbSch = 0;
                    List<int> addTreesBillb = new List<int>();
                    List<int> addTreesNoBillb = new List<int>();

                    for (int p = 0; p < obj.manager.altTreesMain.altTreesManagerData.patches[i].tempTrees.Count; p++)
                    {
                        if (!dataLinks.getAltTree(obj.manager.altTreesMain.altTreesManagerData.patches[i].tempTrees[p].idPrototype).isObject)
                            treesBillb++;
                        else
                            treesNoBillb++;
                    }
                
                    obj.manager.altTreesMain.altTreesManagerData.patches[i].Init(obj.manager, obj.manager.altTreesMain, obj.manager.altTreesMain.altTreesManagerData);

                    obj.manager.altTreesMain.altTreesManagerData.patches[i].prototypes = new AltTreePrototypes[0];
                    obj.manager.altTreesMain.altTreesManagerData.patches[i].trees = new AltTreesTrees[treesBillb];
                    obj.manager.altTreesMain.altTreesManagerData.patches[i].treesNoGroup = new AltTreesTrees[treesNoBillb];
                    obj.manager.altTreesMain.altTreesManagerData.patches[i].treesCount = treesBillb;
                    obj.manager.altTreesMain.altTreesManagerData.patches[i].treesNoGroupCount = treesNoBillb;

                    for (int j = 0; j < obj.manager.altTreesMain.altTreesManagerData.patches[i].tempTrees.Count; j++)
                    {
                        if (!dataLinks.getAltTree(obj.manager.altTreesMain.altTreesManagerData.patches[i].tempTrees[j].idPrototype).isObject)
                        {
                            addTreesBillb.Add(treesBillbSch);
                            obj.manager.altTreesMain.altTreesManagerData.patches[i].trees[treesBillbSch] = obj.manager.altTreesMain.altTreesManagerData.patches[i].tempTrees[j];
                            obj.manager.altTreesMain.altTreesManagerData.patches[i].trees[treesBillbSch].idTree = treesBillbSch;
                            obj.manager.altTreesMain.altTreesManagerData.patches[i].trees[treesBillbSch].altTreesPatch = obj.manager.altTreesMain.altTreesManagerData.patches[i];
                            obj.manager.altTreesMain.altTreesManagerData.patches[i].checkTreePrototype(obj.manager.altTreesMain.altTreesManagerData.patches[i].trees[treesBillbSch].idPrototype, dataLinks.getAltTree(obj.manager.altTreesMain.altTreesManagerData.patches[i].trees[treesBillbSch].idPrototype));
                            treesBillbSch++;
                        }
                        else
                        {
                            addTreesNoBillb.Add(treesNoBillbSch);
                            obj.manager.altTreesMain.altTreesManagerData.patches[i].treesNoGroup[treesNoBillbSch] = obj.manager.altTreesMain.altTreesManagerData.patches[i].tempTrees[j];
                            obj.manager.altTreesMain.altTreesManagerData.patches[i].treesNoGroup[treesNoBillbSch].idTree = treesNoBillbSch;
                            obj.manager.altTreesMain.altTreesManagerData.patches[i].treesNoGroup[treesNoBillbSch].altTreesPatch = obj.manager.altTreesMain.altTreesManagerData.patches[i];
                            obj.manager.altTreesMain.altTreesManagerData.patches[i].checkTreePrototype(obj.manager.altTreesMain.altTreesManagerData.patches[i].treesNoGroup[treesNoBillbSch].idPrototype, dataLinks.getAltTree(obj.manager.altTreesMain.altTreesManagerData.patches[i].treesNoGroup[treesNoBillbSch].idPrototype));
                            treesNoBillbSch++;
                        }
                    }

                    obj.manager.altTreesMain.altTreesManagerData.patches[i].EditDataFile(false, null, addTreesBillb.Count, null, -1);
                    obj.manager.altTreesMain.altTreesManagerData.patches[i].EditDataFile(true, null, addTreesNoBillb.Count, null, -1);

                    obj.manager.addAltTrees(obj.manager.altTreesMain.altTreesManagerData.patches[i]);

                }
            }

                if (!isStop)
                    obj.manager.offSelectionTrees();
        }


        void removePatch(AltTreesPatch patch)
        {
            for (int i = 0; i < obj.manager.altTreesMain.altTreesManagerData.patches.Length; i++)
            {
                if (obj.manager.altTreesMain.altTreesManagerData.patches[i].Equals(patch))
                {
                    int count = 0;
                    AltTreesPatch[] patchesTemp = obj.manager.altTreesMain.altTreesManagerData.patches;
                    obj.manager.altTreesMain.altTreesManagerData.patches = new AltTreesPatch[patchesTemp.Length - 1];
                    for (int j = 0; j < patchesTemp.Length; j++)
                    {
                        if (!patchesTemp[j].Equals(patch))
                        {
                            obj.manager.altTreesMain.altTreesManagerData.patches[count] = patchesTemp[j];
                            count++;
                        }
                    }

                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(patch.treesData));
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(patch.treesNoGroupData));
                    obj.manager.altTreesMain.altTreesManager.removeAltTrees(patch, false);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    return;
                }
            }
        }

        void OnEnable()
        {
            obj = (AltTreeInstance)target;

            if (obj == null)
                return;

            if (obj.isCrossFade)
                Selection.activeObject = null;
            else
            {
                Selection.activeObject = obj.gameObject;
                obj.manager.setSelectionTrees(new Transform[] { Selection.activeTransform });
            }
        }

        bool hideWireframe = false;
        bool hideWireframeStar = false;

        public override void OnInspectorGUI()
        {
            if (Selection.gameObjects.Length > 1)
            {
                Selection.activeTransform = Selection.gameObjects[0].transform;
                obj.manager.setSelectionTrees(new Transform[] { Selection.activeTransform });
            }

            DrawDefaultInspector();
            
            #if UNITY_5_5 || UNITY_5_6 || UNITY_5_7

            #else
                hideWireframe = EditorGUILayout.Toggle("HideWireframe", hideWireframe);
            #endif
            

            if (hideWireframe != hideWireframeStar)
            {
                #if UNITY_5_5 || UNITY_5_6 || UNITY_5_7

                #else
                    EditorUtility.SetSelectedWireframeHidden(obj.gameObject.GetComponent<MeshRenderer>(), hideWireframe);
                #endif
                hideWireframeStar = hideWireframe;
            }
        }

        void getDataLinks()
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