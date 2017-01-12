using UnityEngine;
using UnityEditor;
using System.IO;

namespace AltSystems.AltTrees.Editor
{
    public class AltTreesAssetModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        public static AssetDeleteResult OnWillDeleteAsset(string AssetPath, RemoveAssetOptions rao)
        {
            isUpdate = false;

            if (Directory.Exists(AssetPath))
            {
                if(AssetPath == "Assets/Plugins/AltSystems/AltTrees/DataBase/Trees")
                    findPrefabs(AssetPath, true);
                else
                    findPrefabs(AssetPath, false);
            }
            else
            {
                checkFile(AssetPath, false, true);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();


            if (isUpdate)
            {
                AltTrees[] objs = Transform.FindObjectsOfType(typeof(AltTrees)) as AltTrees[];
                foreach (AltTrees obj in objs)
                {
                    obj.reInitTimer = 3;
                }
            }

            return AssetDeleteResult.DidNotDelete;
        }

        static void findPrefabs(string folder, bool delete)
        {
            string[] files = Directory.GetFiles(folder);
            foreach (string str in files)
            {
                checkFile(str, true, delete);
            }
        }

        static bool isUpdate = false;

        static void checkFile(string filePath, bool isFolder, bool delete)
        {
            string[] strs = filePath.Split('.');
            if (strs[strs.Length - 1].Equals("prefab"))
            {

                GameObject go = (GameObject)AssetDatabase.LoadAssetAtPath(filePath, typeof(GameObject));
                if (go != null)
                {
                    AltTree tree = go.GetComponent<AltTree>();
                    if (tree != null)
                    {
                        if (delete)
                        {
                            if (!isFolder)
                            {
                                if (!tree.folderResources.Equals("") && Directory.Exists("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + tree.folderResources))
                                {
                                    Directory.Delete("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + tree.folderResources, true);
                                }
                            }
                        }

                        isUpdate = true;
                    }
                }
            }
        }
    }
}