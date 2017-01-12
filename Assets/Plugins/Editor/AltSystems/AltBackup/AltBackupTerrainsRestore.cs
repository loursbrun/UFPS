using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;

namespace AltSystems.AltBackup.Editor
{
    public class AltBackupTerrainsRestore : EditorWindow
    {
        bool isHeightMap = true;
        bool isTextureMap = true;
        bool isGrass = true;
        bool isTrees = true;
        Vector2 posScroll = Vector2.zero;
        bool onlyThis = true;
        int idBackup = 0;
        string fileString = "";
        bool checkResourcesFalse = false;
        Vector2 scrollPos = new Vector2();

        System.Collections.Generic.List<int> listTexturesSplat = new System.Collections.Generic.List<int>();
        System.Collections.Generic.List<int> listTexturesNormalSplat = new System.Collections.Generic.List<int>();
        System.Collections.Generic.List<int> listTexturesGrass = new System.Collections.Generic.List<int>();
        System.Collections.Generic.List<int> listTrees = new System.Collections.Generic.List<int>();
        System.Collections.Generic.List<int> listDetailMesh = new System.Collections.Generic.List<int>();

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
            sty.clipping = TextClipping.Clip;

            if (EditorGUIUtility.isProSkin)
                sty.normal.textColor = Color.white;
            else
                sty.normal.textColor = Color.black;


            if (!checkResourcesFalse)
            {
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
                        GUI.Label(new Rect(60, 5, 200, 30), "Terrain for restore: <b>" + Selection.activeTransform.name + "</b>", sty);

                        sty.fontSize = 10;
                        GUI.Label(new Rect(10, 30, 200, 30), "TerrainBackup ID: <b>" + idTerrains.getIdTerrain(terr.terrainData) + "</b>", sty);


                        sty.fontSize = 14;
                        GUI.Label(new Rect(10, 77, 200, 30), "<b>Select backup for restore:</b>", sty);
                        GUI.Box(new Rect(10, 100, 600, 220), "");

                        onlyThis = GUI.Toggle(new Rect(428, 83, 200, 30), onlyThis, "Only backups for this terrain");

                        string[] dirs = Directory.GetFiles("AltSystems/AltBackups", "*.altBackup");

                        if (onlyThis)
                        {
                            System.Collections.Generic.List<string> tempList = new System.Collections.Generic.List<string>();

                            for (int i = 0; i < dirs.Length; i++)
                            {
                                BinaryReader fl = null;

                                try
                                {
                                    fl = new BinaryReader(File.Open(dirs[i], FileMode.Open));

                                    if (fl.ReadInt32() == 1 && fl.ReadInt32() == 1)
                                    {
                                        if (fl.ReadInt32() == idTerrains.getIdTerrain(terr.terrainData))
                                        {
                                            tempList.Add(dirs[i]);
                                        }
                                    }
                                    fl.Close();
                                }
                                catch (IOException exc)
                                {
                                    Debug.LogError(exc.Message);
                                    return;
                                }
                                finally
                                {
                                    if (fl != null)
                                        fl.Close();
                                }
                            }
                            dirs = new string[tempList.Count];
                            tempList.CopyTo(dirs);
                        }

                        IComparer revComparer = new ReverseComparer();
                        Array.Sort(dirs, revComparer);

                        posScroll = GUI.BeginScrollView(new Rect(10, 100, 600, 220), posScroll, new Rect(0, 0, 585, 205));
                        {
                            sty.fontSize = 9;
                            sty.alignment = TextAnchor.MiddleLeft;
                            sty.fontStyle = FontStyle.Bold;

                            GUI.Label(new Rect(70, 3, 200, 19), "backup:", sty);
                            GUI.Label(new Rect(182, 3, 200, 19), "TerrainBackup ID:", sty);
                            GUI.Label(new Rect(310, 3, 200, 19), "HeightMap:", sty);
                            GUI.Label(new Rect(375, 3, 200, 19), "TextureMap:", sty);
                            GUI.Label(new Rect(450, 3, 200, 19), "Grass:", sty);
                            GUI.Label(new Rect(497, 3, 200, 19), "Trees:", sty);
                            GUI.Label(new Rect(545, 3, 200, 19), "Delete:", sty);

                            sty.fontSize = 10;
                            sty.alignment = TextAnchor.MiddleLeft;
                            sty.fontStyle = FontStyle.Normal;

                            for (int i = 0; i < dirs.Length; i++)
                            {
                                string[] strTemp = dirs[i].Split('\\');
                                strTemp = strTemp[1].Split('.');
                                strTemp[0] = strTemp[0].Replace("backup_", "");

                                GUI.BeginGroup(new Rect(5, 25 + 20 * i, 580, 20));
                                {
                                    sty.alignment = TextAnchor.MiddleLeft;

                                    GUI.Box(new Rect(0, 0, 580, 20), "");

                                    idBackup = (GUI.Toggle(new Rect(2, 2, 530, 15), idBackup == i ? true : false, "")) ? i : idBackup;

                                    GUI.Label(new Rect(20, 0, 150, 19), "" + strTemp[0], sty);

                                    BinaryReader fl = null;
                                    try
                                    {
                                        fl = new BinaryReader(File.Open(dirs[i], FileMode.Open));

                                        if (fl.ReadInt32() == 1 && fl.ReadInt32() == 1)
                                        {
                                            sty.alignment = TextAnchor.MiddleCenter;

                                            GUI.Label(new Rect(185, 0, 66, 19), "" + fl.ReadInt32(), sty);

                                            if (fl.ReadInt64() > 0)
                                                GUI.Label(new Rect(320, 0, 20, 19), "yes", sty);
                                            else
                                                GUI.Label(new Rect(321, 0, 20, 19), "no", sty);

                                            if (fl.ReadInt64() > 0)
                                                GUI.Label(new Rect(390, 0, 20, 19), "yes", sty);
                                            else
                                                GUI.Label(new Rect(391, 0, 20, 19), "no", sty);

                                            if (fl.ReadInt64() > 0)
                                                GUI.Label(new Rect(450, 0, 20, 19), "yes", sty);
                                            else
                                                GUI.Label(new Rect(451, 0, 20, 19), "no", sty);

                                            if (fl.ReadInt64() > 0)
                                                GUI.Label(new Rect(495, 0, 20, 19), "yes", sty);
                                            else
                                                GUI.Label(new Rect(496, 0, 20, 19), "no", sty);
                                        }
                                        fl.Close();
                                    }
                                    catch (IOException exc)
                                    {
                                        Debug.LogError(exc.Message);
                                        return;
                                    }
                                    finally
                                    {
                                        if (fl != null)
                                            fl.Close();
                                    }

                                    if (GUI.Button(new Rect(548, 2, 20, 16), "X"))
                                    {
                                        try
                                        {
                                            if (!System.IO.Directory.Exists("AltSystems/AltBackups/Deleted"))
                                            {
                                                System.IO.Directory.CreateDirectory("AltSystems/AltBackups/Deleted");
                                            }

                                            System.IO.File.Move("AltSystems/AltBackups/backup_" + strTemp[0] + ".altBackup", "AltSystems/AltBackups/Deleted/backup_" + strTemp[0] + ".altBackup");
                                        }
                                        catch (IOException exc)
                                        {
                                            Debug.LogError(exc.Message);
                                            return;
                                        }
                                        finally
                                        {
                                            Debug.Log("Backup moved from AltSystems/AltBackups to AltSystems/AltBackups/Deleted");
                                            if (i == idBackup)
                                                idBackup = 100000;
                                        }
                                    }

                                }
                                GUI.EndGroup();


                            }


                        }
                        GUI.EndScrollView();


                        sty.fontSize = 14;
                        GUI.Label(new Rect(10, 335, 100, 30), "Settings:", sty);

                        if (idBackup >= dirs.Length)
                            GUI.enabled = false;
                        else
                        {

                            BinaryReader fl = null;
                            try
                            {
                                fl = new BinaryReader(File.Open(dirs[idBackup], FileMode.Open));

                                if (fl.ReadInt32() == 1 && fl.ReadInt32() == 1)
                                {
                                    fl.ReadInt32();

                                    if (fl.ReadInt64() == 0)
                                    {
                                        GUI.enabled = false;
                                    }
                                    isHeightMap = GUI.Toggle(new Rect(10, 365, 200, 17), isHeightMap, "Restore HeightMap");
                                    GUI.enabled = true;

                                    if (fl.ReadInt64() == 0)
                                    {
                                        GUI.enabled = false;
                                    }
                                    isTextureMap = GUI.Toggle(new Rect(10, 380, 200, 17), isTextureMap, "Restore TextureMap");
                                    GUI.enabled = true;

                                    if (fl.ReadInt64() == 0)
                                    {
                                        GUI.enabled = false;
                                    }
                                    isGrass = GUI.Toggle(new Rect(10, 395, 200, 17), isGrass, "Restore Grass");
                                    GUI.enabled = true;

                                    if (fl.ReadInt64() == 0)
                                    {
                                        GUI.enabled = false;
                                    }
                                    isTrees = GUI.Toggle(new Rect(10, 410, 200, 17), isTrees, "Restore Trees");
                                    GUI.enabled = true;
                                }
                                fl.Close();
                            }
                            catch (IOException exc)
                            {
                                Debug.LogError(exc.Message);
                                return;
                            }
                            finally
                            {
                                if (fl != null)
                                    fl.Close();
                            }

                        }

                        if (!(isHeightMap || isTextureMap || isGrass || isTrees))
                            GUI.enabled = false;

                        if (GUI.Button(new Rect(50, 450, 500, 28), "Restore Terrain"))
                        {
                            fileString = dirs[idBackup];
                            restoreTerrain();
                        }

                        GUI.enabled = true;

                    }
                    else
                        GUI.Label(new Rect(0, 0, 500, 300), "Error! TerrainData of terrain is null.", sty);

                }
                else
                    GUI.Label(new Rect(0, 0, 620, 300), "Select terrain in Hierarchy for restore!", sty);
            }
            else
            {
                sty.alignment = TextAnchor.MiddleLeft;
                sty.fontSize = 14;
                sty.normal.textColor = Color.red;

                GUI.Label(new Rect(20, 10, 300, 30), "<b>Error!</b>", sty);

                if (EditorGUIUtility.isProSkin)
                    sty.normal.textColor = Color.white;
                else
                    sty.normal.textColor = Color.black;

                GUI.Label(new Rect(20, 30, 300, 30), "Links to some of the resources are missing!", sty);
                GUI.Label(new Rect(20, 46, 300, 30), "Please assign the required objects and click Next:", sty);

                int x = 0;
                int y = 0;

                if (listTexturesSplat.Count > 0)
                {
                    for (int i = 0; i < listTexturesSplat.Count; i++)
                    {
                        if (x == 500)
                        {
                            x = 0;
                            y += 100;
                        }
                        x += 100;
                    }
                    x = 0;
                    y += 115;
                }
                if (listTexturesNormalSplat.Count > 0)
                {
                    for (int i = 0; i < listTexturesNormalSplat.Count; i++)
                    {
                        if (x == 500)
                        {
                            x = 0;
                            y += 100;
                        }
                        x += 100;
                    }
                    x = 0;
                    y += 115;
                }
                if (listTexturesGrass.Count > 0)
                {
                    for (int i = 0; i < listTexturesGrass.Count; i++)
                    {
                        if (x == 500)
                        {
                            x = 0;
                            y += 100;
                        }
                        x += 100;
                    }
                    x = 0;
                    y += 115;
                }
                if (listTrees.Count > 0)
                {
                    for (int i = 0; i < listTrees.Count; i++)
                    {
                        if (x == 500)
                        {
                            x = 0;
                            y += 100;
                        }
                        x += 100;
                    }
                    x = 0;
                    y += 115;
                }
                if (listDetailMesh.Count > 0)
                {
                    for (int i = 0; i < listDetailMesh.Count; i++)
                    {
                        if (x == 500)
                        {
                            x = 0;
                            y += 100;
                        }
                        x += 100;
                    }
                    y += 115;
                }

                scrollPos = GUI.BeginScrollView(new Rect(20, 73, 550, 400), scrollPos, new Rect(0, 0, 520, y));
                {
                    x = 0;
                    y = 0;
                    if (listTexturesSplat.Count > 0)
                    {
                        GUI.Label(new Rect(20, 0, 300, 30), "<b>TextureMap Textures:</b>", sty);
                        for (int i = 0; i < listTexturesSplat.Count; i++)
                        {
                            if (x == 500)
                            {
                                x = 0;
                                y += 100;
                            }

                            idTerrains.setTexture(listTexturesSplat[i], EditorGUI.ObjectField(new Rect(x + 20, y + 27, 70, 70), idTerrains.getTexture(listTexturesSplat[i]), typeof(Texture2D), false) as Texture2D);

                            sty.alignment = TextAnchor.MiddleCenter;
                            sty.fontSize = 10;
                            GUI.Label(new Rect(x + 20, y + 95, 70, 20), "id: <b>" + listTexturesSplat[i] + "</b>", sty);
                            sty.alignment = TextAnchor.MiddleLeft;
                            sty.fontSize = 14;

                            x += 100;
                        }
                        x = 0;
                        y += 115;
                    }
                    if (listTexturesNormalSplat.Count > 0)
                    {
                        GUI.Label(new Rect(20, y, 300, 30), "<b>TextureMap Normals:</b>", sty);
                        for (int i = 0; i < listTexturesNormalSplat.Count; i++)
                        {
                            if (x == 500)
                            {
                                x = 0;
                                y += 100;
                            }

                            idTerrains.setTexture(listTexturesNormalSplat[i], EditorGUI.ObjectField(new Rect(x + 20, y + 27, 70, 70), idTerrains.getTexture(listTexturesNormalSplat[i]), typeof(Texture2D), false) as Texture2D);

                            sty.alignment = TextAnchor.MiddleCenter;
                            sty.fontSize = 10;
                            GUI.Label(new Rect(x + 20, y + 95, 70, 20), "id: <b>" + listTexturesNormalSplat[i] + "</b>", sty);
                            sty.alignment = TextAnchor.MiddleLeft;
                            sty.fontSize = 14;

                            x += 100;
                        }
                        x = 0;
                        y += 115;
                    }
                    if (listTexturesGrass.Count > 0)
                    {
                        GUI.Label(new Rect(20, y, 300, 30), "<b>Grass Textures:</b>", sty);
                        for (int i = 0; i < listTexturesGrass.Count; i++)
                        {
                            if (x == 500)
                            {
                                x = 0;
                                y += 100;
                            }

                            idTerrains.setTexture(listTexturesGrass[i], EditorGUI.ObjectField(new Rect(x + 20, y + 27, 70, 70), idTerrains.getTexture(listTexturesGrass[i]), typeof(Texture2D), false) as Texture2D);

                            sty.alignment = TextAnchor.MiddleCenter;
                            sty.fontSize = 10;
                            GUI.Label(new Rect(x + 20, y + 95, 70, 20), "id: <b>" + listTexturesGrass[i] + "</b>", sty);
                            sty.alignment = TextAnchor.MiddleLeft;
                            sty.fontSize = 14;

                            x += 100;
                        }
                        x = 0;
                        y += 115;
                    }
                    if (listTrees.Count > 0)
                    {
                        GUI.Label(new Rect(20, y, 300, 30), "<b>Trees:</b>", sty);
                        for (int i = 0; i < listTrees.Count; i++)
                        {
                            if (x == 500)
                            {
                                x = 0;
                                y += 100;
                            }

                            if (idTerrains.getTreesAndDetailMeshes(listTrees[i]) != null)
                            {
                                Texture2D tex = null;
                                tex = AssetPreview.GetAssetPreview(idTerrains.getTreesAndDetailMeshes(listTrees[i]));
                                if (tex != null)
                                    EditorGUI.DrawPreviewTexture(new Rect(x + 20, y + 27, 70, 70), tex);
                            }

                            idTerrains.setTreesAndDetailMeshes(listTrees[i], EditorGUI.ObjectField(new Rect(x + 20, y + 82, 70, 15), idTerrains.getTreesAndDetailMeshes(listTrees[i]), typeof(GameObject), false) as GameObject);


                            sty.alignment = TextAnchor.MiddleCenter;
                            sty.fontSize = 10;
                            GUI.Label(new Rect(x + 20, y + 95, 70, 20), "id: <b>" + listTrees[i] + "</b>", sty);
                            sty.alignment = TextAnchor.MiddleLeft;
                            sty.fontSize = 14;

                            x += 100;
                        }
                        x = 0;
                        y += 115;
                    }
                    if (listDetailMesh.Count > 0)
                    {
                        GUI.Label(new Rect(20, y, 300, 30), "<b>Detail Meshes:</b>", sty);
                        for (int i = 0; i < listDetailMesh.Count; i++)
                        {
                            if (x == 500)
                            {
                                x = 0;
                                y += 100;
                            }

                            if (idTerrains.getTreesAndDetailMeshes(listDetailMesh[i]) != null)
                            {
                                Texture2D tex = null;
                                tex = AssetPreview.GetAssetPreview(idTerrains.getTreesAndDetailMeshes(listDetailMesh[i]));
                                if (tex != null)
                                    EditorGUI.DrawPreviewTexture(new Rect(x + 20, y + 27, 70, 70), tex);
                            }

                            idTerrains.setTreesAndDetailMeshes(listDetailMesh[i], EditorGUI.ObjectField(new Rect(x + 20, y + 82, 70, 15), idTerrains.getTreesAndDetailMeshes(listDetailMesh[i]), typeof(GameObject), false) as GameObject);

                            sty.alignment = TextAnchor.MiddleCenter;
                            sty.fontSize = 10;
                            GUI.Label(new Rect(x + 20, y + 95, 70, 20), "id: <b>" + listDetailMesh[i] + "</b>", sty);
                            sty.alignment = TextAnchor.MiddleLeft;
                            sty.fontSize = 14;

                            x += 100;
                        }
                    }
                }
                GUI.EndScrollView();

                if (GUI.Button(new Rect(160, 467, 200, 26), "Next"))
                {
                    checkResourcesFalse = false;
                    EditorUtility.SetDirty(idTerrains);
                    restoreTerrain();
                }

                if (GUI.Button(new Rect(520, 470, 80, 20), "Cancel"))
                {
                    checkResourcesFalse = false;
                }
            }
        }

        void restoreTerrain()
        {
            EditorUtility.DisplayProgressBar("Restoring backup... ", "Starting", 0.0f);

            listTexturesSplat.Clear();
            listTexturesNormalSplat.Clear();
            listTexturesGrass.Clear();
            listTrees.Clear();
            listDetailMesh.Clear();

            getIdTerrains();

            EditorUtility.DisplayProgressBar("Restoring backup... ", "Checking Resources", 0.05f);

            if (!checkResources())
            {
                scrollPos = Vector2.zero;

                EditorUtility.ClearProgressBar();
                checkResourcesFalse = true;
                return;
            }

            EditorUtility.DisplayProgressBar("Restoring backup... ", "Reading backup file", 0.1f);

            BinaryReader fl = null;
            try
            {
                fl = new BinaryReader(File.Open(fileString, FileMode.Open));

                if (fl.ReadInt32() == 1 && fl.ReadInt32() == 1)
                {
                    fl.ReadInt32();

                    long posHeightMap = fl.ReadInt64();
                    long posTextureMap = fl.ReadInt64();
                    long posGrass = fl.ReadInt64();
                    long posTrees = fl.ReadInt64();

                    if (posHeightMap > 0 && isHeightMap)
                    {
                        EditorUtility.DisplayProgressBar("Restoring backup... ", "Restoring HeightMap", 0.2f);

                        fl.BaseStream.Position = posHeightMap;

                        int heightmapResolution = fl.ReadInt32();
                        Vector3 scale = new Vector3(fl.ReadSingle(), fl.ReadSingle(), fl.ReadSingle());
                        float[,] heights = new float[heightmapResolution, heightmapResolution];

                        for (int i = 0; i < heightmapResolution; i++)
                        {
                            for (int j = 0; j < heightmapResolution; j++)
                            {
                                heights[i, j] = fl.ReadSingle();
                            }
                        }

                        terr.terrainData.heightmapResolution = heightmapResolution;
                        terr.terrainData.size = scale;
                        terr.terrainData.SetHeights(0, 0, heights);
                    }
                    if (posTextureMap > 0 && isTextureMap)
                    {
                        EditorUtility.DisplayProgressBar("Restoring backup... ", "Restoring TextureMap", 0.4f);

                        fl.BaseStream.Position = posTextureMap;


                        int alphamapResolution = fl.ReadInt32();
                        int splatPrototypesLength = fl.ReadInt32();
                        SplatPrototype[] sp = new SplatPrototype[splatPrototypesLength];
                        int tempid = 0;

                        for (int i = 0; i < splatPrototypesLength; i++)
                        {
                            sp[i] = new SplatPrototype();
                            sp[i].metallic = fl.ReadSingle();
                            tempid = fl.ReadInt32();
                            if (tempid != 0)
                                sp[i].normalMap = idTerrains.getTexture(tempid);
                            sp[i].smoothness = fl.ReadSingle();
                            sp[i].specular = new Color(fl.ReadSingle(), fl.ReadSingle(), fl.ReadSingle(), fl.ReadSingle());
                            tempid = fl.ReadInt32();
                            if (tempid != 0)
                                sp[i].texture = idTerrains.getTexture(tempid);
                            else
                                Debug.LogError("tempid==0");
                            sp[i].tileOffset = new Vector2(fl.ReadSingle(), fl.ReadSingle());
                            sp[i].tileSize = new Vector2(fl.ReadSingle(), fl.ReadSingle());
                        }

                        terr.terrainData.splatPrototypes = sp;
                        terr.terrainData.alphamapResolution = alphamapResolution;

                        float[, ,] maps = new float[alphamapResolution, alphamapResolution, splatPrototypesLength];

                        for (int y = 0; y < alphamapResolution; y++)
                        {
                            for (int x = 0; x < alphamapResolution; x++)
                            {
                                for (int z = 0; z < splatPrototypesLength; z++)
                                {
                                    maps[x, y, z] = fl.ReadSingle();
                                }
                            }
                        }
                        terr.terrainData.SetAlphamaps(0, 0, maps);

                    }
                    if (posGrass > 0 && isGrass)
                    {
                        EditorUtility.DisplayProgressBar("Restoring backup... ", "Restoring Grass", 0.6f);

                        fl.BaseStream.Position = posGrass;

                        int detailHeight = fl.ReadInt32();
                        int detailWidth = fl.ReadInt32();
                        int detailResolution = fl.ReadInt32();

                        terr.detailObjectDensity = fl.ReadSingle();
                        terr.detailObjectDistance = fl.ReadSingle();

                        int detailResolutionPerPatch = fl.ReadInt32();
                        int detailPrototypesLength = fl.ReadInt32();
                        int tempid = 0;

                        DetailPrototype[] dp = new DetailPrototype[detailPrototypesLength];

                        for (int i = 0; i < detailPrototypesLength; i++)
                        {
                            dp[i] = new DetailPrototype();

                            dp[i].bendFactor = fl.ReadSingle();
                            dp[i].dryColor = new Color(fl.ReadSingle(), fl.ReadSingle(), fl.ReadSingle(), fl.ReadSingle());
                            dp[i].healthyColor = new Color(fl.ReadSingle(), fl.ReadSingle(), fl.ReadSingle(), fl.ReadSingle());
                            dp[i].maxHeight = fl.ReadSingle();
                            dp[i].maxWidth = fl.ReadSingle();
                            dp[i].minHeight = fl.ReadSingle();
                            dp[i].minWidth = fl.ReadSingle();
                            dp[i].noiseSpread = fl.ReadSingle();
                            tempid = fl.ReadInt32();
                            if (tempid != 0)
                                dp[i].prototype = idTerrains.getTreesAndDetailMeshes(tempid);
                            tempid = fl.ReadInt32();
                            if (tempid != 0)
                                dp[i].prototypeTexture = idTerrains.getTexture(tempid);
                            tempid = fl.ReadInt32();
                            if (tempid == 1)
                                dp[i].renderMode = DetailRenderMode.Grass;
                            else if (tempid == 2)
                                dp[i].renderMode = DetailRenderMode.GrassBillboard;
                            else if (tempid == 3)
                                dp[i].renderMode = DetailRenderMode.VertexLit;
                            else
                                Debug.LogError("tempid==0");
                            dp[i].usePrototypeMesh = fl.ReadBoolean();
                        }
                        terr.terrainData.detailPrototypes = dp;
                        terr.terrainData.SetDetailResolution(detailResolution, detailResolutionPerPatch);

                        for (int i = 0; i < detailPrototypesLength; i++)
                        {
                            int[,] map = new int[detailWidth, detailHeight];

                            for (int y = 0; y < detailHeight; y++)
                            {
                                for (int x = 0; x < detailWidth; x++)
                                {
                                    map[x, y] = fl.ReadInt32();
                                }
                            }

                            terr.terrainData.SetDetailLayer(0, 0, i, map);
                        }

                    }
                    if (posTrees > 0 && isTrees)
                    {
                        EditorUtility.DisplayProgressBar("Restoring backup... ", "Restoring Trees", 0.8f);

                        fl.BaseStream.Position = posTrees;

                        terr.bakeLightProbesForTrees = fl.ReadBoolean();
                        terr.drawTreesAndFoliage = fl.ReadBoolean();
                        terr.treeBillboardDistance = fl.ReadSingle();
                        terr.treeCrossFadeLength = fl.ReadSingle();
                        terr.treeDistance = fl.ReadSingle();
                        terr.treeMaximumFullLODCount = fl.ReadInt32();

                        int treeInstanceCount = fl.ReadInt32();
                        int treePrototypesLength = fl.ReadInt32();
                        int tempid = 0;

                        TreePrototype[] tpt = new TreePrototype[treePrototypesLength];

                        for (int i = 0; i < treePrototypesLength; i++)
                        {
                            tpt[i] = new TreePrototype();
                            tpt[i].bendFactor = fl.ReadSingle();

                            tempid = fl.ReadInt32();
                            if (tempid != 0)
                                tpt[i].prefab = idTerrains.getTreesAndDetailMeshes(tempid);
                        }
                        terr.terrainData.treePrototypes = tpt;

                        TreeInstance[] tinsts = new TreeInstance[treeInstanceCount];

                        for (int i = 0; i < treeInstanceCount; i++)
                        {
                            tinsts[i] = new TreeInstance();
                            tinsts[i].color = new Color32(fl.ReadByte(), fl.ReadByte(), fl.ReadByte(), fl.ReadByte());
                            tinsts[i].heightScale = fl.ReadSingle();
                            tinsts[i].lightmapColor = new Color32(fl.ReadByte(), fl.ReadByte(), fl.ReadByte(), fl.ReadByte());
                            tinsts[i].position = new Vector3(fl.ReadSingle(), fl.ReadSingle(), fl.ReadSingle());
                            tinsts[i].prototypeIndex = fl.ReadInt32();
                            tinsts[i].rotation = fl.ReadSingle();
                            tinsts[i].widthScale = fl.ReadSingle();
                        }
                        terr.terrainData.treeInstances = tinsts;

                        terr.terrainData.RefreshPrototypes();
                    }

                    EditorUtility.DisplayProgressBar("Restoring backup... ", "Applying terrain", 1.0f);

                    terr.Flush();
                }
            }
            catch (IOException exc)
            {
                Debug.LogError(exc.Message);
                return;
            }
            finally
            {
                Debug.Log("Terrain restored from " + fileString);
                if (fl != null)
                    fl.Close();
                EditorUtility.ClearProgressBar();
            }
        }

        bool checkResources()
        {
            bool isError = false;

            BinaryReader fl = null;
            try
            {
                fl = new BinaryReader(File.Open(fileString, FileMode.Open));

                if (fl.ReadInt32() == 1 && fl.ReadInt32() == 1)
                {
                    fl.ReadInt32();

                    fl.ReadInt64();
                    long posTextureMap = fl.ReadInt64();
                    long posGrass = fl.ReadInt64();
                    long posTrees = fl.ReadInt64();

                    if (posTextureMap > 0 && isTextureMap)
                    {
                        fl.BaseStream.Position = posTextureMap;

                        fl.ReadInt32();
                        int splatPrototypesLength = fl.ReadInt32();
                        int tempid = 0;

                        for (int i = 0; i < splatPrototypesLength; i++)
                        {
                            fl.ReadSingle();

                            tempid = fl.ReadInt32();
                            if (tempid != 0)
                            {
                                if (idTerrains.getTexture(tempid) == null)
                                {
                                    isError = true;
                                    listTexturesNormalSplat.Add(tempid);
                                }
                            }
                            fl.ReadSingle();

                            fl.ReadSingle();
                            fl.ReadSingle();
                            fl.ReadSingle();
                            fl.ReadSingle();

                            tempid = fl.ReadInt32();
                            if (tempid != 0)
                            {
                                if (idTerrains.getTexture(tempid) == null)
                                {
                                    isError = true;
                                    listTexturesSplat.Add(tempid);
                                }
                            }
                            else
                                Debug.LogError("tempid==0");

                            fl.ReadSingle();
                            fl.ReadSingle();

                            fl.ReadSingle();
                            fl.ReadSingle();
                        }

                    }
                    if (posGrass > 0 && isGrass)
                    {
                        fl.BaseStream.Position = posGrass;

                        fl.ReadInt32();
                        fl.ReadInt32();
                        fl.ReadInt32();

                        fl.ReadSingle();
                        fl.ReadSingle();

                        fl.ReadInt32();
                        int detailPrototypesLength = fl.ReadInt32();
                        int tempid = 0;

                        for (int i = 0; i < detailPrototypesLength; i++)
                        {
                            fl.ReadSingle();

                            fl.ReadSingle();
                            fl.ReadSingle();
                            fl.ReadSingle();
                            fl.ReadSingle();

                            fl.ReadSingle();
                            fl.ReadSingle();
                            fl.ReadSingle();
                            fl.ReadSingle();

                            fl.ReadSingle();
                            fl.ReadSingle();
                            fl.ReadSingle();
                            fl.ReadSingle();
                            fl.ReadSingle();

                            tempid = fl.ReadInt32();
                            if (tempid != 0)
                            {
                                if (idTerrains.getTreesAndDetailMeshes(tempid) == null)
                                {
                                    isError = true;
                                    listDetailMesh.Add(tempid);
                                }
                            }

                            tempid = fl.ReadInt32();
                            if (tempid != 0)
                            {
                                if (idTerrains.getTexture(tempid) == null)
                                {
                                    isError = true;
                                    listTexturesGrass.Add(tempid);
                                }
                            }

                            fl.ReadInt32();
                            fl.ReadBoolean();
                        }
                    }
                    if (posTrees > 0 && isTrees)
                    {
                        fl.BaseStream.Position = posTrees;

                        fl.ReadBoolean();
                        fl.ReadBoolean();
                        fl.ReadSingle();
                        fl.ReadSingle();
                        fl.ReadSingle();
                        fl.ReadInt32();

                        fl.ReadInt32();
                        int treePrototypesLength = fl.ReadInt32();
                        int tempid = 0;

                        for (int i = 0; i < treePrototypesLength; i++)
                        {
                            fl.ReadSingle();

                            tempid = fl.ReadInt32();
                            if (tempid != 0)
                            {
                                if (idTerrains.getTreesAndDetailMeshes(tempid) == null)
                                {
                                    isError = true;
                                    listTrees.Add(tempid);
                                }
                            }
                        }
                    }
                }
            }
            catch (IOException exc)
            {
                Debug.LogError(exc.Message);
                return false;
            }
            finally
            {
                if (fl != null)
                    fl.Close();
            }
            if (isError)
                return false;
            else
                return true;
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

    public class ReverseComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return (new CaseInsensitiveComparer()).Compare(y, x);
        }
    }
}
