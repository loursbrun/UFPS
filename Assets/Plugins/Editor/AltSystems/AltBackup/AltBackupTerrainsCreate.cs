using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Reflection;

namespace AltSystems.AltBackup.Editor
{
    public class AltBackupTerrainsCreate : EditorWindow
    {
        bool isHeightMap = true;
        bool isTextureMap = true;
        bool isGrass = true;
        bool isTrees = true;
        string dat = "";
        static string nameFile = "%time%";

        Terrain terr = null;
        AltBackupIdTerrains idTerrains = null;

        void OnGUI()
        {
            if (!System.IO.Directory.Exists("AltSystems/AltBackups"))
            {
                System.IO.Directory.CreateDirectory("AltSystems/AltBackups");
            }


            if (AltSystemsNewsCheck.newsCheckStatic == null)
            {
                GameObject goTemp = new GameObject("newsCheckStatic");
                goTemp.hideFlags = HideFlags.HideInHierarchy;
                AltSystemsNewsCheck.newsCheckStatic = goTemp.AddComponent<AltSystemsNewsCheck>();
            }


            GUIStyle sty = new GUIStyle();
            sty.alignment = TextAnchor.MiddleCenter;
            sty.fontSize = 16;

            if (EditorGUIUtility.isProSkin)
                sty.normal.textColor = Color.white;
            else
                sty.normal.textColor = Color.black;


            if (Selection.activeTransform != null)
                terr = Selection.activeTransform.GetComponent<Terrain>();
            else
                terr = null;

            if (terr != null)
            {
                if (terr.terrainData != null)
                {
                    getIdTerrains();


                    sty.alignment = TextAnchor.MiddleLeft;
                    sty.fontSize = 14;
                    GUI.Label(new Rect(60, 5, 200, 30), "Terrain for backup: <b>" + Selection.activeTransform.name + "</b>", sty);

                    sty.fontSize = 10;
                    GUI.Label(new Rect(10, 30, 200, 30), "TerrainBackup ID: <b>" + idTerrains.getIdTerrain(terr.terrainData) + "</b>", sty);


                    sty.fontSize = 14;
                    GUI.Label(new Rect(10, 90, 200, 30), "Settings:", sty);

                    isHeightMap = GUI.Toggle(new Rect(10, 120, 200, 17), isHeightMap, "Backup HeightMap");
                    isTextureMap = GUI.Toggle(new Rect(10, 135, 200, 17), isTextureMap, "Backup TextureMap");
                    isGrass = GUI.Toggle(new Rect(10, 150, 200, 17), isGrass, "Backup Grass");
                    isTrees = GUI.Toggle(new Rect(10, 165, 200, 17), isTrees, "Backup Trees");
                    GUI.Label(new Rect(10, 183, 150, 30), "Name file: backup_");
                    nameFile = GUI.TextField(new Rect(125, 183, 100, 17), nameFile);
                    GUI.Label(new Rect(229, 183, 80, 30), ".altBackup");
                    GUI.Label(new Rect(20, 204, 500, 30), "(keys: %time% - file creation time; %terrain% - terrain name;");
                    GUI.Label(new Rect(120, 219, 500, 30), "%scene% - scene name)");


                    if (!(isHeightMap || isTextureMap || isGrass || isTrees))
                        GUI.enabled = false;

                    if (GUI.Button(new Rect(50, 250, 300, 28), "Create Backup"))
                    {
                        createBackup();
                    }

                    GUI.enabled = true;

                }
                else
                    GUI.Label(new Rect(0, 0, 400, 300), "Error! TerrainData of terrain is null.", sty);

            }
            else
                GUI.Label(new Rect(0, 0, 400, 300), "Select terrain in Hierarchy for backup!", sty);
        }

        void createBackup()
        {
            string nameFile2 = "backup_" + nameFile;

            EditorUtility.DisplayProgressBar("Creating backup... ", "Starting", 0.0f);

            getIdTerrains();

            EditorUtility.DisplayProgressBar("Creating backup... ", "Creating file backup", 0.1f);

            BinaryWriter fl = null;
            try
            {
                if (!System.IO.Directory.Exists("AltSystems/AltBackups"))
                {
                    System.IO.Directory.CreateDirectory("AltSystems/AltBackups");
                }
                dat = System.DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss");

                string sceneName = "";

                #if UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_5_7
                {
                    sceneName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
                }
                #else
                {
                    sceneName = "";
                    string[] path = EditorApplication.currentScene.Split('/');
                    if (path.Length > 1)
                    {
                        path = path[path.Length - 1].Split('.');
                        sceneName = path[0];
                    }
                }
                #endif

                

                nameFile2 = nameFile2.Replace("%time%", dat);
                nameFile2 = nameFile2.Replace("%terrain%", terr.gameObject.name);
                nameFile2 = nameFile2.Replace("%scene%", sceneName);

                bool stop = false;
                int num = 0;
                while (!stop)
                {
                    if (!System.IO.File.Exists("AltSystems/AltBackups/" + nameFile2 + (num==0 ? "" : "_"+num) + ".altbackup"))
                    {
                        if (num > 0)
                            nameFile2 += "_" + num;

                        stop = true;
                    }
                    else
                        num++;
                }

                fl = new BinaryWriter(File.Open("AltSystems/AltBackups/" + nameFile2 + ".altBackup", FileMode.Create));
                fl.Write((int)1);	//	version
                fl.Write((int)1);	//	terrain Backup

                fl.Write(idTerrains.getIdTerrain(terr.terrainData));

                fl.Write((long)0);
                fl.Write((long)0);
                fl.Write((long)0);
                fl.Write((long)0);

                long posHeightMap = 0;
                long posTextureMap = 0;
                long posGrass = 0;
                long posTrees = 0;

                if (isHeightMap)
                {
                    EditorUtility.DisplayProgressBar("Creating backup... ", "Saving HeightMap", 0.2f);

                    posHeightMap = fl.BaseStream.Position;
                    fl.Write(terr.terrainData.heightmapResolution);
                    fl.Write(terr.terrainData.size.x);
                    fl.Write(terr.terrainData.size.y);
                    fl.Write(terr.terrainData.size.z);

                    float[,] heights = terr.terrainData.GetHeights(0, 0, terr.terrainData.heightmapResolution, terr.terrainData.heightmapResolution);
                    for (int i = 0; i < terr.terrainData.heightmapResolution; i++)
                    {
                        for (int j = 0; j < terr.terrainData.heightmapResolution; j++)
                        {
                            fl.Write(heights[i, j]);
                        }
                    }
                }

                if (isTextureMap)
                {
                    EditorUtility.DisplayProgressBar("Creating a backup... ", "Saving TextureMap", 0.4f);

                    posTextureMap = fl.BaseStream.Position;

                    fl.Write(terr.terrainData.alphamapResolution);
                    fl.Write(terr.terrainData.splatPrototypes.Length);

                    for (int i = 0; i < terr.terrainData.splatPrototypes.Length; i++)
                    {
                        fl.Write(terr.terrainData.splatPrototypes[i].metallic);
                        if (terr.terrainData.splatPrototypes[i].normalMap != null)
                            fl.Write(idTerrains.getIdTexture(terr.terrainData.splatPrototypes[i].normalMap));
                        else
                            fl.Write((int)0);
                        fl.Write(terr.terrainData.splatPrototypes[i].smoothness);
                        fl.Write(terr.terrainData.splatPrototypes[i].specular.r);
                        fl.Write(terr.terrainData.splatPrototypes[i].specular.g);
                        fl.Write(terr.terrainData.splatPrototypes[i].specular.b);
                        fl.Write(terr.terrainData.splatPrototypes[i].specular.a);
                        if (terr.terrainData.splatPrototypes[i].texture != null)
                            fl.Write(idTerrains.getIdTexture(terr.terrainData.splatPrototypes[i].texture));
                        else
                            fl.Write((int)0);
                        fl.Write(terr.terrainData.splatPrototypes[i].tileOffset.x);
                        fl.Write(terr.terrainData.splatPrototypes[i].tileOffset.y);
                        fl.Write(terr.terrainData.splatPrototypes[i].tileSize.x);
                        fl.Write(terr.terrainData.splatPrototypes[i].tileSize.y);
                    }

                    float[, ,] maps = terr.terrainData.GetAlphamaps(0, 0, terr.terrainData.alphamapResolution, terr.terrainData.alphamapResolution);

                    for (int y = 0; y < terr.terrainData.alphamapResolution; y++)
                    {
                        for (int x = 0; x < terr.terrainData.alphamapResolution; x++)
                        {
                            for (int z = 0; z < terr.terrainData.splatPrototypes.Length; z++)
                            {
                                fl.Write(maps[x, y, z]);
                            }
                        }
                    }

                }

                if (isGrass)
                {
                    EditorUtility.DisplayProgressBar("Creating a backup... ", "Saving Grass", 0.6f);

                    posGrass = fl.BaseStream.Position;

                    fl.Write(terr.terrainData.detailHeight);
                    fl.Write(terr.terrainData.detailWidth);
                    fl.Write(terr.terrainData.detailResolution);

                    fl.Write(terr.detailObjectDensity);
                    fl.Write(terr.detailObjectDistance);

                    BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                    PropertyInfo prop = terr.terrainData.GetType().GetProperty("detailResolutionPerPatch", flags);
                    int val = (int)prop.GetValue(terr.terrainData, null);

                    fl.Write(val);
                    fl.Write(terr.terrainData.detailPrototypes.Length);

                    for (int i = 0; i < terr.terrainData.detailPrototypes.Length; i++)
                    {
                        fl.Write(terr.terrainData.detailPrototypes[i].bendFactor);
                        fl.Write(terr.terrainData.detailPrototypes[i].dryColor.r);
                        fl.Write(terr.terrainData.detailPrototypes[i].dryColor.g);
                        fl.Write(terr.terrainData.detailPrototypes[i].dryColor.b);
                        fl.Write(terr.terrainData.detailPrototypes[i].dryColor.a);
                        fl.Write(terr.terrainData.detailPrototypes[i].healthyColor.r);
                        fl.Write(terr.terrainData.detailPrototypes[i].healthyColor.g);
                        fl.Write(terr.terrainData.detailPrototypes[i].healthyColor.b);
                        fl.Write(terr.terrainData.detailPrototypes[i].healthyColor.a);

                        fl.Write(terr.terrainData.detailPrototypes[i].maxHeight);
                        fl.Write(terr.terrainData.detailPrototypes[i].maxWidth);
                        fl.Write(terr.terrainData.detailPrototypes[i].minHeight);
                        fl.Write(terr.terrainData.detailPrototypes[i].minWidth);
                        fl.Write(terr.terrainData.detailPrototypes[i].noiseSpread);
                        if (terr.terrainData.detailPrototypes[i].prototype != null)
                            fl.Write(idTerrains.getIdTreesAndDetailMeshes(terr.terrainData.detailPrototypes[i].prototype));
                        else
                            fl.Write((int)0);
                        if (terr.terrainData.detailPrototypes[i].prototypeTexture != null)
                            fl.Write(idTerrains.getIdTexture(terr.terrainData.detailPrototypes[i].prototypeTexture));
                        else
                            fl.Write((int)0);
                        if (terr.terrainData.detailPrototypes[i].renderMode == DetailRenderMode.Grass)
                            fl.Write((int)1);
                        else if (terr.terrainData.detailPrototypes[i].renderMode == DetailRenderMode.GrassBillboard)
                            fl.Write((int)2);
                        else if (terr.terrainData.detailPrototypes[i].renderMode == DetailRenderMode.VertexLit)
                            fl.Write((int)3);
                        else
                            fl.Write((int)0);
                        fl.Write(terr.terrainData.detailPrototypes[i].usePrototypeMesh);
                    }

                    for (int i = 0; i < terr.terrainData.detailPrototypes.Length; i++)
                    {
                        int[,] map = terr.terrainData.GetDetailLayer(0, 0, terr.terrainData.detailWidth, terr.terrainData.detailHeight, i);

                        for (int y = 0; y < terr.terrainData.detailHeight; y++)
                        {
                            for (int x = 0; x < terr.terrainData.detailWidth; x++)
                            {
                                fl.Write(map[x, y]);
                            }
                        }
                    }

                }


                if (isTrees)
                {
                    EditorUtility.DisplayProgressBar("Creating a backup... ", "Saving Trees", 0.8f);

                    posTrees = fl.BaseStream.Position;

                    fl.Write(terr.bakeLightProbesForTrees);
                    fl.Write(terr.drawTreesAndFoliage);
                    fl.Write(terr.treeBillboardDistance);
                    fl.Write(terr.treeCrossFadeLength);
                    fl.Write(terr.treeDistance);
                    fl.Write(terr.treeMaximumFullLODCount);

                    fl.Write(terr.terrainData.treeInstanceCount);
                    fl.Write(terr.terrainData.treePrototypes.Length);

                    for (int i = 0; i < terr.terrainData.treePrototypes.Length; i++)
                    {
                        fl.Write(terr.terrainData.treePrototypes[i].bendFactor);
                        if (terr.terrainData.treePrototypes[i].prefab != null)
                            fl.Write(idTerrains.getIdTreesAndDetailMeshes(terr.terrainData.treePrototypes[i].prefab));
                        else
                            fl.Write((int)0);
                    }

                    for (int i = 0; i < terr.terrainData.treeInstances.Length; i++)
                    {
                        fl.Write(terr.terrainData.treeInstances[i].color.r);
                        fl.Write(terr.terrainData.treeInstances[i].color.g);
                        fl.Write(terr.terrainData.treeInstances[i].color.b);
                        fl.Write(terr.terrainData.treeInstances[i].color.a);

                        fl.Write(terr.terrainData.treeInstances[i].heightScale);

                        fl.Write(terr.terrainData.treeInstances[i].lightmapColor.r);
                        fl.Write(terr.terrainData.treeInstances[i].lightmapColor.g);
                        fl.Write(terr.terrainData.treeInstances[i].lightmapColor.b);
                        fl.Write(terr.terrainData.treeInstances[i].lightmapColor.a);

                        fl.Write(terr.terrainData.treeInstances[i].position.x);
                        fl.Write(terr.terrainData.treeInstances[i].position.y);
                        fl.Write(terr.terrainData.treeInstances[i].position.z);

                        fl.Write(terr.terrainData.treeInstances[i].prototypeIndex);
                        fl.Write(terr.terrainData.treeInstances[i].rotation);
                        fl.Write(terr.terrainData.treeInstances[i].widthScale);
                    }

                }

                EditorUtility.DisplayProgressBar("Creating a backup... ", "Saving file backup", 1.0f);


                if (isHeightMap)
                {
                    fl.BaseStream.Position = 12;
                    fl.Write(posHeightMap);
                }
                if (isTextureMap)
                {
                    fl.BaseStream.Position = 20;
                    fl.Write(posTextureMap);
                }
                if (isGrass)
                {
                    fl.BaseStream.Position = 28;
                    fl.Write(posGrass);
                }
                if (isTrees)
                {
                    fl.BaseStream.Position = 36;
                    fl.Write(posTrees);
                }

            }
            catch (IOException exc)
            {
                Debug.LogError(exc.Message);
                return;
            }
            finally
            {
                if (fl.BaseStream.Length < 1024)
                    Debug.Log("Backup saved to AltBackups/" + nameFile2 + ".altBackup. Size: " + fl.BaseStream.Length + " B.");
                else if (fl.BaseStream.Length < 1048576)
                    Debug.Log("Backup saved to AltBackups/" + nameFile2 + ".altBackup. Size: " + (float)System.Math.Round(((float)fl.BaseStream.Length / 1024f), 2) + " kB.");
                else
                    Debug.Log("Backup saved to AltBackups/" + nameFile2 + ".altBackup. Size: " + (float)System.Math.Round(((float)fl.BaseStream.Length / 1048576f), 2) + " MB.");

                if (fl != null)
                    fl.Close();

                EditorUtility.SetDirty(idTerrains);

                EditorUtility.ClearProgressBar();
            }
        }

        void getIdTerrains()
        {
            if (!System.IO.Directory.Exists("Assets/Plugins/AltSystems/AltBackup"))
            {
                System.IO.Directory.CreateDirectory("Assets/Plugins/AltSystems/AltBackup");
            }
            if (!System.IO.File.Exists("Assets/Plugins/AltSystems/AltBackup/IdsTerrainsBackups.asset"))
            {

                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<AltBackupIdTerrains>(), "Assets/Plugins/AltSystems/AltBackup/IdsTerrainsBackups.asset");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

            }
            idTerrains = (AltBackupIdTerrains)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltBackup/IdsTerrainsBackups.asset", typeof(AltBackupIdTerrains));

            if (idTerrains == null)
            {

                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<AltBackupIdTerrains>(), "Assets/Plugins/AltSystems/AltBackup/IdsTerrainsBackups.asset");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                idTerrains = (AltBackupIdTerrains)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltBackup/IdsTerrainsBackups.asset", typeof(AltBackupIdTerrains));
            }

        }

        void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}
