using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Reflection;

namespace AltSystems.AltTrees.Editor
{
    [CustomEditor(typeof(AltTrees))]
    public class AltTrees_Editor : UnityEditor.Editor
    {
        static public string version = "0.9.6.1";
        static public int altTreesVersionUnity = 540;

        int currentUnityVer = 0;

        AltTreesDataLinks dataLinks = null;
        int sizePatchTemp = 0;
        int maxLODTemp = 0;
        AltTrees obj = null;



        int sizePatch;
        int maxLOD;
        float distancePatchFactor;
        float distanceTreesLODFactor;
        float distanceObjectsLODFactor;
        float checkTreesPercentPerFrame;
        float crossFadeTimeBillboard;
        float crossFadeTimeMesh;

        int initObjsCountPool;
        int objsPerOneMaxPool;

        int initBillboardCountPool;
        int billboardsMaxPool;

        int initCollidersCountPool;
        int collidersPerOneMaxPool;
        int initColliderBillboardsCountPool;
        int colliderBillboardsPerOneMaxPool;

        bool draw;
        bool autoConfig;
        bool autoConfigStar;
        bool generateAllBillboardsOnStart;
        bool enableColliders;
        bool drawDebugPutches;
        bool drawDebugBillboards;
        bool drawDebugBillboardsStar;
        bool debugLog;
        bool debugLogInBilds;
        bool hideGroupBillboards;
        bool hideGroupBillboardsStar;

        bool isRefresh = false;


        SerializedProperty menuId;
        int menuIdStar = 0;



        SerializedProperty idTreeSelected;
        int idTreeSelectedStar = -1;
        SerializedProperty brushSize;
        SerializedProperty treeCount;
        SerializedProperty speedPlace;
        SerializedProperty randomRotation;

        SerializedProperty height;
        SerializedProperty heightRandom;
        SerializedProperty isRandomHeight;

        SerializedProperty lockWidthToHeight;
        SerializedProperty width;
        SerializedProperty widthRandom;
        SerializedProperty isRandomWidth;

        SerializedProperty isRandomHueLeaves;
        SerializedProperty isRandomHueBark;
        Color hueColorLeaves;
        Color hueColorBark;

        SerializedProperty angleLimit;


        Transform projectorTransform = null;
        Projector projector = null;

        bool isPlacingShift = false;
        bool isPlacingCtrl = false;

        bool isImport = false;
        bool isExport = false;
        public Terrain terrainTempImport = null;
        Terrain terrainTempExport = null;
        Terrain terrainTempExportStar = null;
        Terrain terrainTempMassPlace = null;
        int countMassPlace = 10000;
        public bool isDeleteTreesFromTerrain = true;
        public bool isDeleteTreesFromAltTrees = true;
        GUIStyle sty;
        GUIStyle sty2;
        GUIStyle sty3;
        GUIStyle sty4;
        GUIStyle sty5;
        GUIStyle sty6;

        AltTree[] altTreesArrayExport;
        GameObject[] terrainTreesArrayExport;
        List<AltTreesTrees> attTemp;
        List<int> prototypesListTemp;
        AltTreesPatch[] listPatchesExport;

        Vector2 posScroll = Vector2.zero;

        AltTree treeTemp;

        int[] treeIdsTemp;

        Texture2D textureBackground;

        bool checkTreeVersionsStatus = false;

        public void OnEnable()
        {
            obj = (AltTrees)target;

            if (obj.altTreesManagerData == null)
                CreateAltTreesManagerData();

            getDataLinks();

            if (obj.dataLinksCorrupted)
                return;

            checkTreeVersionsStatus = dataLinks.checkTreeVersionsStatus();
            if (checkTreeVersionsStatus)
            {
                sty6 = new GUIStyle();
                sty6.fontStyle = FontStyle.Bold;
                sty6.normal.textColor = Color.red;
                sty6.wordWrap = true;
                altTT = dataLinks.checkTreeVersions();
                return;
            }

            sty = new GUIStyle();
            sty.wordWrap = true;
            sty2 = new GUIStyle();
            sty3 = new GUIStyle();
            sty3.alignment = TextAnchor.LowerCenter;
            sty4 = new GUIStyle();
            sty4.richText = true;
            sty5 = new GUIStyle();
            sty5.fontStyle = FontStyle.Bold;
            sty6 = new GUIStyle();
            sty6.fontStyle = FontStyle.Bold;
            sty6.normal.textColor = Color.red;
            sty6.wordWrap = true;

            Color32 cvet = new Color32(20, 97, 225, 255);
            textureBackground = new Texture2D(2, 2);
            textureBackground.SetPixels32(new Color32[] { cvet, cvet, cvet, cvet });
            textureBackground.hideFlags = HideFlags.HideAndDontSave;
            textureBackground.Apply();

            isRefresh = false;

            sizePatch = obj.altTreesManagerData.sizePatch;
            maxLOD = obj.altTreesManagerData.maxLOD;
            distancePatchFactor = obj.altTreesManagerData.distancePatchFactor;
            distanceTreesLODFactor = obj.altTreesManagerData.distanceTreesLODFactor;
            distanceObjectsLODFactor = obj.altTreesManagerData.distanceObjectsLODFactor;
            checkTreesPercentPerFrame = obj.altTreesManagerData.checkTreesPercentPerFrame;
            crossFadeTimeBillboard = obj.altTreesManagerData.crossFadeTimeBillboard;
            crossFadeTimeMesh = obj.altTreesManagerData.crossFadeTimeMesh;

            initObjsCountPool = obj.altTreesManagerData.initObjsCountPool;
            objsPerOneMaxPool = obj.altTreesManagerData.objsPerOneMaxPool;

            initBillboardCountPool = obj.altTreesManagerData.initBillboardCountPool;
            billboardsMaxPool = obj.altTreesManagerData.billboardsMaxPool;

            initCollidersCountPool = obj.altTreesManagerData.initCollidersCountPool;
            collidersPerOneMaxPool = obj.altTreesManagerData.collidersPerOneMaxPool;
            initColliderBillboardsCountPool = obj.altTreesManagerData.initColliderBillboardsCountPool;
            colliderBillboardsPerOneMaxPool = obj.altTreesManagerData.colliderBillboardsPerOneMaxPool;

            draw = obj.altTreesManagerData.draw;
            autoConfig = obj.altTreesManagerData.autoConfig;
            autoConfigStar = autoConfig;
            generateAllBillboardsOnStart = obj.altTreesManagerData.generateAllBillboardsOnStart;
            enableColliders = obj.altTreesManagerData.enableColliders;
            drawDebugPutches = obj.altTreesManagerData.drawDebugPutches;
            drawDebugBillboards = obj.altTreesManagerData.drawDebugBillboards;
            drawDebugBillboardsStar = obj.altTreesManagerData.drawDebugBillboardsStar;
            debugLog = obj.altTreesManagerData.debugLog;
            debugLogInBilds = obj.altTreesManagerData.debugLogInBilds;
            hideGroupBillboards = obj.altTreesManagerData.hideGroupBillboards;
            hideGroupBillboardsStar = hideGroupBillboards;


            menuId = serializedObject.FindProperty("menuId");
            menuIdStar = menuId.intValue;

            idTreeSelected = serializedObject.FindProperty("idTreeSelected");
            brushSize = serializedObject.FindProperty("brushSize");
            treeCount = serializedObject.FindProperty("treeCount");
            speedPlace = serializedObject.FindProperty("speedPlace");
            randomRotation = serializedObject.FindProperty("randomRotation");

            height = serializedObject.FindProperty("height");
            heightRandom = serializedObject.FindProperty("heightRandom");
            isRandomHeight = serializedObject.FindProperty("isRandomHeight");
            lockWidthToHeight = serializedObject.FindProperty("lockWidthToHeight");
            width = serializedObject.FindProperty("width");
            widthRandom = serializedObject.FindProperty("widthRandom");
            isRandomWidth = serializedObject.FindProperty("isRandomWidth");

            isRandomHueLeaves = serializedObject.FindProperty("isRandomHueLeaves");
            isRandomHueBark = serializedObject.FindProperty("isRandomHueBark");

            angleLimit = serializedObject.FindProperty("angleLimit");

            

            sizePatchTemp = sizePatch;
            maxLODTemp = maxLOD;


            projectorTransform = (Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Plugins/Editor/AltSystems/AltTrees/EditorResources/Projector/Projector.prefab", typeof(GameObject))) as GameObject).transform;
            projector = projectorTransform.GetComponent<Projector>();
            projectorTransform.gameObject.SetActive(false);
            projectorTransform.gameObject.hideFlags = HideFlags.HideAndDontSave;


            countTrees = 0;
            countTreesTemp = 0;

            if (dataLinks.altTrees != null)
            {
                for (int i = 0; i < dataLinks.altTrees.Length; i++)
                {
                    if (dataLinks.altTrees[i] != null)
                        countTrees++;
                }
            }

            trees = new AltTree[countTrees];
            treeIdsTemp = new int[countTrees];
            schs = new int[countTrees];
            if (idTreeSelected.intValue >= treeIdsTemp.Length)
                idTreeSelected.intValue = -1;
            if (dataLinks.altTrees != null)
            {
                for (int i = 0; i < dataLinks.altTrees.Length && countTreesTemp < countTrees; i++)
                {
                    if (dataLinks.altTrees[i] != null)
                    {
                        trees[countTreesTemp] = dataLinks.altTrees[i];
                        treeIdsTemp[countTreesTemp] = i;
                        countTreesTemp++;
                    }
                }
            }

            textures = new GUIContent[countTrees];
            for (int i = 0; i < countTrees; i++)
            {
                textures[i] = new GUIContent();
                if (trees[i] != null)
                {
                    textures[i].image = AssetPreview.GetAssetPreview(trees[i].gameObject);
                    textures[i].text = trees[i].name;
                }
            }


            if (menuId.intValue == 1 && idTreeSelected.intValue != -1)
            {
                projectorTransform.gameObject.SetActive(true);
                OffMouseSelect.enable();
                idTreeSelectedStar = idTreeSelected.intValue;


                if (idTreeSelected.intValue >= treeIdsTemp.Length)
                    idTreeSelected.intValue = -1;


                treeTemp = dataLinks.altTrees[treeIdsTemp[idTreeSelected.intValue]];
                if (treeTemp != null)
                {
                    hueColorLeaves = treeTemp.hueVariationLeaves;
                    hueColorBark = treeTemp.hueVariationBark;
                }
            }


            if (autoConfig)
            {
                Terrain terr = (Terrain)Transform.FindObjectOfType(typeof(Terrain));
                float size = 0f;
                int degree = 0;

                if (terr != null)
                    size = Mathf.Clamp(Mathf.Max(terr.terrainData.size.x, terr.terrainData.size.z), 100, 10000);
                else
                    size = 1000f;

                for (int d = 1; d < 16; d++)
                {
                    if (size / Mathf.Pow(2f, (float)d) <= 160)
                    {
                        degree = d;
                        break;
                    }
                }

                if (size != sizePatch)
                {
                    sizePatch = (int)size;
                    sizePatchTemp = sizePatch;

                    serializedObject.ApplyModifiedProperties();

                    resizePatches(sizePatch);
                }
                if (degree != obj.altTreesManagerData.maxLOD)
                {
                    obj.altTreesManagerData.maxLOD = degree;
                    maxLOD = degree;
                    maxLODTemp = maxLOD;

                    EditorUtility.SetDirty(obj.altTreesManagerData);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    serializedObject.ApplyModifiedProperties();

                    obj.ReInit(true);
                }
            }
            
            int.TryParse( (Application.unityVersion.Substring(0, 5)).Replace(".", "") , out currentUnityVer);

        }

        void OnDisable()
        {
            checkTreeVersionsStatus = false;
            textures = null;
            treeTemp = null;
            if (projectorTransform != null)
            {
                projector = null;
                DestroyImmediate(projectorTransform.gameObject);
            }
            OffMouseSelect.disable();

            if (isRefresh)
            {
                AssetDatabase.SaveAssets();
            }
            AssetDatabase.Refresh();
        }

        GUIContent[] textures;
        int[] schs;
        int countTrees = 0;
        int countTreesTemp = 0;
        AltTree[] trees;
        Type _type = null;
        FieldInfo _propInfo = null;
        static bool isUpdate = false;

        AltTree[] altTT;

        public override void OnInspectorGUI()
        {
            if (AltSystemsNewsCheck.newsCheckStatic == null)
            {
                GameObject goTemp = new GameObject("newsCheckStatic");
                goTemp.hideFlags = HideFlags.HideInHierarchy;
                AltSystemsNewsCheck.newsCheckStatic = goTemp.AddComponent<AltSystemsNewsCheck>();
            }

            if(checkTreeVersionsStatus)
            {
                GUILayout.Space(20);
                GUILayout.Label("Some trees require an upgrade:", sty6);
                GUILayout.Space(20);

                if (altTT != null)
                {
                    for (int i = 0; i < altTT.Length; i++)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.ObjectField(altTT[i], typeof(AltTree), false);
                            if(GUILayout.Button("Upgrade"))
                            {
                                Selection.activeGameObject = altTT[i].gameObject;
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }

                return;
            }

            serializedObject.Update();

            if (obj.altTreesManagerData == null)
                CreateAltTreesManagerData();

            getDataLinks();

            if (obj.dataLinksCorrupted)
                return;

            _type = System.Type.GetType("AltSystems.AltTrees.Editor.InstallerAltTrees");
            if (!isUpdate && _type != null)
            {
                if (_type != null)
                    _propInfo = _type.GetField("isInstallOk", BindingFlags.Static | BindingFlags.Public);

                if (_propInfo != null)
                {
                    isUpdate = true;
                    MethodInfo _method = _type.GetMethod("sendQuestion");
                    _method.Invoke(null, null);
                    Selection.activeGameObject = null;
                }
            }

            for (int i = 0; i < countTrees; i++)
            {
                if (textures[i].image == null && trees[i] != null)
                {
                    schs[i]++;
                    textures[i].image = AssetPreview.GetAssetPreview(trees[i].gameObject);

                    if (schs[i] == 5)
                    {
                        schs[i] = 0;
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(trees[i].gameObject), ImportAssetOptions.ForceUpdate);
                    }
                    Repaint();
                }
            }

            if (idTreeSelected.intValue >= treeIdsTemp.Length)
                idTreeSelected.intValue = -1;

            if (idTreeSelectedStar != idTreeSelected.intValue)
            {
                if (idTreeSelected.intValue != -1)
                {
                    treeTemp = dataLinks.altTrees[treeIdsTemp[idTreeSelected.intValue]];
                    if (treeTemp != null)
                    {
                        hueColorLeaves = treeTemp.hueVariationLeaves;
                        hueColorBark = treeTemp.hueVariationBark;
                    }
                }

                idTreeSelectedStar = idTreeSelected.intValue;
            }

            GUIStyle style = new GUIStyle();
            style.padding = new RectOffset(2, 2, 2, 2);
            style.imagePosition = ImagePosition.ImageAbove;

            style.clipping = TextClipping.Clip;
            style.alignment = TextAnchor.UpperCenter;
            style.fontSize = 9;

            style.onNormal.background = textureBackground;

            EditorGUILayout.BeginHorizontal();
            {
                if (menuId.intValue == 1)
                    GUI.enabled = false;
                if (GUILayout.Button("Place"))
                {
                    if (idTreeSelected.intValue != -1)
                    {
                        projectorTransform.gameObject.SetActive(true);
                        OffMouseSelect.enable();
                    }
                    menuId.intValue = 1;
                }
                GUI.enabled = true;
                if (menuId.intValue == 2)
                    GUI.enabled = false;
                if (GUILayout.Button("Patches"))
                {
                    projectorTransform.gameObject.SetActive(false);
                    OffMouseSelect.disable();

                    menuId.intValue = 2;
                }
                GUI.enabled = true;
                if (menuId.intValue == 3)
                    GUI.enabled = false;
                if (GUILayout.Button("Settings"))
                {
                    projectorTransform.gameObject.SetActive(false);
                    OffMouseSelect.disable();

                    menuId.intValue = 3;
                }
                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if(altTreesVersionUnity == 520 && currentUnityVer >= 540)
            {
                GUILayout.Label("Attention! You use the AltTrees for Unity 5.2.0+. Please, update AltTrees for Unity 5.4.0+", sty6);
            }

            EditorGUILayout.Space();

            if (menuId.intValue == 1)
            {

                if (idTreeSelected.intValue != -1 && !projectorTransform.gameObject.activeSelf)
                {
                    projectorTransform.gameObject.SetActive(true);
                    OffMouseSelect.enable();
                }

                GUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    GUILayout.Label("Place Trees", sty5);
                    GUILayout.Label("Hold down shift to erase trees.\nHold down ctrl to erase the selected tree type.", EditorStyles.wordWrappedMiniLabel);
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    idTreeSelected.intValue = GUI.SelectionGrid(GetBrushAspectRect(countTrees, 64, 12), idTreeSelected.intValue, textures, (int)Mathf.Ceil((float)((Screen.width - 20) / 64)), style);
                }
                GUILayout.EndVertical();

                if (idTreeSelected.intValue >= countTrees)
                    idTreeSelected.intValue = -1;

                if (idTreeSelected.intValue != -1)
                {
                    GUILayout.Label("Settings:", EditorStyles.boldLabel);

                    brushSize.intValue = EditorGUILayout.IntSlider("Brush Size: ", brushSize.intValue, 1, 300);
                    treeCount.intValue = EditorGUILayout.IntSlider("Tree Count: ", treeCount.intValue, 1, 1000);
                    speedPlace.intValue = EditorGUILayout.IntSlider("Speed Place: ", speedPlace.intValue, 1, 10);
                    randomRotation.boolValue = EditorGUILayout.Toggle("Random Y Rotation: ", randomRotation.boolValue);

                    GUILayout.Space(7);

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Tree Height:", GUILayout.Width(EditorGUIUtility.labelWidth - 6f));


                        GUILayout.Label("Random?");
                        isRandomHeight.boolValue = GUILayout.Toggle(isRandomHeight.boolValue, "");

                        if (isRandomHeight.boolValue)
                        {
                            float heightTemp = height.floatValue;
                            float heightRandomTemp = height.floatValue + heightRandom.floatValue;
                            EditorGUILayout.MinMaxSlider(ref heightTemp, ref heightRandomTemp, 0.1f, 2f);

                            if (heightTemp != height.floatValue || heightRandom.floatValue != heightRandomTemp)
                            {
                                height.floatValue = heightTemp;
                                heightRandom.floatValue = heightRandomTemp - heightTemp;
                            }

                            GUILayout.Label(heightTemp.ToString("0.0") + " - " + heightRandomTemp.ToString("0.0"));
                        }
                        else
                        {
                            height.floatValue = (float)System.Math.Round(EditorGUILayout.Slider(height.floatValue, 0.1f, 2f), 2);
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(7);

                    lockWidthToHeight.boolValue = EditorGUILayout.Toggle("Lock Width to Height: ", lockWidthToHeight.boolValue);

                    GUILayout.Space(7);

                    if (lockWidthToHeight.boolValue)
                        GUI.enabled = false;

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Tree Width:", GUILayout.Width(EditorGUIUtility.labelWidth - 6f));


                        GUILayout.Label("Random?");
                        isRandomWidth.boolValue = GUILayout.Toggle(isRandomWidth.boolValue, "");

                        if (isRandomWidth.boolValue)
                        {
                            float widthTemp = width.floatValue;
                            float widthRandomTemp = width.floatValue + widthRandom.floatValue;
                            EditorGUILayout.MinMaxSlider(ref widthTemp, ref widthRandomTemp, 0.1f, 2f);

                            if (widthTemp != width.floatValue || widthRandom.floatValue != widthRandomTemp)
                            {
                                width.floatValue = widthTemp;
                                widthRandom.floatValue = widthRandomTemp - widthTemp;
                            }

                            GUILayout.Label(widthTemp.ToString("0.0") + " - " + widthRandomTemp.ToString("0.0"));
                        }
                        else
                        {
                            width.floatValue = (float)System.Math.Round(EditorGUILayout.Slider(width.floatValue, 0.1f, 2f), 2);
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUI.enabled = true;


                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Hue Leaves:", GUILayout.Width(EditorGUIUtility.labelWidth - 6f));


                        GUILayout.Label("Random(from alpha color)?");
                        isRandomHueLeaves.boolValue = GUILayout.Toggle(isRandomHueLeaves.boolValue, "");

                        hueColorLeaves = EditorGUILayout.ColorField(hueColorLeaves);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Hue Bark:", GUILayout.Width(EditorGUIUtility.labelWidth - 6f));


                        GUILayout.Label("Random(from alpha color)?");
                        isRandomHueBark.boolValue = GUILayout.Toggle(isRandomHueBark.boolValue, "");

                        hueColorBark = EditorGUILayout.ColorField(hueColorBark);
                    }
                    GUILayout.EndHorizontal();

                    angleLimit.floatValue = (float)System.Math.Round(EditorGUILayout.Slider("Angle Limit:", angleLimit.floatValue, 0.0f, 90f), 1);

                    GUILayout.Space(10);



                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        GUILayout.Label("Mass Place:", EditorStyles.boldLabel);
                        terrainTempMassPlace = (Terrain)EditorGUILayout.ObjectField(terrainTempMassPlace, typeof(Terrain), true);
                        countMassPlace = EditorGUILayout.IntField("Count:", countMassPlace);
                        if (terrainTempMassPlace == null || countMassPlace <= 0)
                            GUI.enabled = false;
                        if (GUILayout.Button("Place"))
                        {
                            massPlace();
                        }
                        GUI.enabled = true;
                    }
                    GUILayout.EndVertical();
                }

            }
            else if (menuId.intValue == 2)
            {
                EditorGUILayout.LabelField("Patches:", EditorStyles.boldLabel);

                EditorGUILayout.Space();

                GUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    posScroll = GUILayout.BeginScrollView(posScroll);
                    {
                        sty2.fontSize = 9;
                        sty2.alignment = TextAnchor.MiddleLeft;
                        sty2.fontStyle = FontStyle.Bold;

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("   Patch:", sty2);
                            GUILayout.Label("Prototypes:", sty2);
                            GUILayout.Label("Trees:", sty2);
                            GUILayout.Label("Objects:", sty2);
                        }
                        GUILayout.EndHorizontal();

                        for (int i = 0; i < obj.altTreesManagerData.patches.Length; i++)
                        {
                            GUILayout.BeginHorizontal(EditorStyles.helpBox);
                            {
                                GUILayout.Label("Patch [" + obj.altTreesManagerData.patches[i].stepX + " x " + obj.altTreesManagerData.patches[i].stepY + "]");
                                GUILayout.Label(((obj.altTreesManagerData.patches[i].prototypes != null) ? obj.altTreesManagerData.patches[i].prototypes.Length : 0) + "");


                                GUILayout.Label((obj.altTreesManagerData.patches[i].treesCount - obj.altTreesManagerData.patches[i].treesEmptyCount) + " [" + obj.altTreesManagerData.patches[i].treesEmptyCount + "]");
                                GUILayout.Label((obj.altTreesManagerData.patches[i].treesNoGroupCount - obj.altTreesManagerData.patches[i].treesNoGroupEmptyCount) + " [" + obj.altTreesManagerData.patches[i].treesNoGroupEmptyCount + "]");
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                    GUILayout.EndScrollView();
                }
                GUILayout.EndVertical();

                EditorGUILayout.Space();
            }
            else if (menuId.intValue == 3)
            {
                EditorGUILayout.LabelField("ID Manager:    <b>" + obj.getIdManager() + "</b>", sty4);

                GUILayout.Space(5);
                bool starDraw = draw;

                draw = GUILayout.Toggle(draw, "Draw");
                enableColliders = GUILayout.Toggle(enableColliders, "Enable Tree Colliders");
                generateAllBillboardsOnStart = GUILayout.Toggle(generateAllBillboardsOnStart, "Generate All Billboards On Start");

                serializedObject.ApplyModifiedProperties();

                if (starDraw != draw)
                {
                    if (draw)
                    {
                        obj.Init(true);
                    }
                    else
                        obj.altTreesManager.destroy(true);
                }

                GUILayout.Space(20);

                GUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    GUILayout.Label("Patch Settings", sty5);

                    autoConfig = GUILayout.Toggle(autoConfig, "Auto Configuration");

                    if (autoConfigStar != autoConfig)
                    {
                        autoConfigStar = autoConfig;

                        if (autoConfig)
                        {
                            Terrain terr = (Terrain)Transform.FindObjectOfType(typeof(Terrain));
                            float size = 0f;
                            int degree = 0;

                            if (terr != null)
                                size = Mathf.Clamp(Mathf.Max(terr.terrainData.size.x, terr.terrainData.size.z), 100, 10000);
                            else
                                size = 1000f;

                            if (size != sizePatch)
                            {
                                sizePatch = (int)size;
                                sizePatchTemp = sizePatch;

                                serializedObject.ApplyModifiedProperties();

                                obj.altTreesManagerData.autoConfig = autoConfig;

                                resizePatches(sizePatch);

                                return;
                            }


                            for (int d = 1; d < 16; d++)
                            {
                                if (size / Mathf.Pow(2f, (float)d) <= 160)
                                {
                                    degree = d;
                                    break;
                                }
                            }

                            if (degree != obj.altTreesManagerData.maxLOD)
                            {
                                obj.altTreesManagerData.maxLOD = degree;
                                maxLOD = degree;
                                maxLODTemp = maxLOD;

                                EditorUtility.SetDirty(obj.altTreesManagerData);
                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();

                                serializedObject.ApplyModifiedProperties();

                                obj.ReInit(true);

                                return;
                            }
                        }
                    }

                    if (autoConfig)
                        GUI.enabled = false;
                    EditorGUILayout.BeginHorizontal();
                    {
                        sizePatchTemp = EditorGUILayout.IntField("Size Patch:", sizePatchTemp);
                        if (GUILayout.Button("Set", GUILayout.Width(60f)))
                        {
                            if (sizePatch != sizePatchTemp)
                            {
                                serializedObject.ApplyModifiedProperties();

                                resizePatches(sizePatchTemp);

                                sizePatch = sizePatchTemp;

                                return;
                            }
                        }
                        GUILayout.Space(100);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        maxLODTemp = EditorGUILayout.IntField("Max LOD Patch:", maxLODTemp);
                        if (GUILayout.Button("Set", GUILayout.Width(60f)))
                        {
                            if (maxLOD != maxLODTemp)
                            {
                                maxLOD = maxLODTemp;
                                obj.altTreesManagerData.maxLOD = maxLOD;
                                EditorUtility.SetDirty(obj.altTreesManagerData);
                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();

                                serializedObject.ApplyModifiedProperties();

                                obj.ReInit(true);
                            }
                        }
                        GUILayout.Space(100);
                    }
                    EditorGUILayout.EndHorizontal();

                    GUI.enabled = true;

                    distancePatchFactor = Mathf.Clamp(EditorGUILayout.FloatField("Distance Patch Factor:", distancePatchFactor), 0.1f, 20f);
                }
                EditorGUILayout.EndVertical();

                GUILayout.Space(10);


                GUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    GUILayout.Label("LOD Settings", sty5);

                    distanceTreesLODFactor = EditorGUILayout.FloatField("Distance Trees LOD Factor:", distanceTreesLODFactor);
                    distanceObjectsLODFactor = EditorGUILayout.FloatField("Distance Objects LOD Factor:", distanceObjectsLODFactor);
                    checkTreesPercentPerFrame = Mathf.Floor(Mathf.Clamp(EditorGUILayout.FloatField("Check Trees Per Frame, Percent:", checkTreesPercentPerFrame), 0, 100));

                    crossFadeTimeBillboard = EditorGUILayout.FloatField("Cross-fade Billboard Time:", crossFadeTimeBillboard);
                    crossFadeTimeMesh = EditorGUILayout.FloatField("Cross-fade Mesh Time:", crossFadeTimeMesh);

                }
                GUILayout.EndVertical();

                GUILayout.Space(10);


                GUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    GUILayout.Label("Pool Settings", sty5);

                    initObjsCountPool = EditorGUILayout.IntField("Initial Mesh Count:", initObjsCountPool);
                    objsPerOneMaxPool = EditorGUILayout.IntField("Max Mesh Count:", objsPerOneMaxPool);
                    EditorGUILayout.Space();
                    initBillboardCountPool = EditorGUILayout.IntField("Initial Billboards Count:", initBillboardCountPool);
                    billboardsMaxPool = EditorGUILayout.IntField("Max Billboards Count:", billboardsMaxPool);
                    EditorGUILayout.Space();
                    initCollidersCountPool = EditorGUILayout.IntField("Initial Collider Count:", initCollidersCountPool);
                    collidersPerOneMaxPool = EditorGUILayout.IntField("Max Colliders Count:", collidersPerOneMaxPool);
                    EditorGUILayout.Space();
                    initColliderBillboardsCountPool = EditorGUILayout.IntField("Initial Billboard Collider Count:", initColliderBillboardsCountPool);
                    colliderBillboardsPerOneMaxPool = EditorGUILayout.IntField("Max Billboard Colliders Count:", colliderBillboardsPerOneMaxPool);

                }
                GUILayout.EndVertical();

                EditorGUILayout.Space();


                if (!draw)
                    GUI.enabled = false;



                GUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    GUILayout.Label("Debug Settings", sty5);

                    drawDebugPutches = GUILayout.Toggle(drawDebugPutches, "Draw debug putches");
                    drawDebugBillboards = GUILayout.Toggle(drawDebugBillboards, "Debug billboards DrawCalls");


                    if (drawDebugBillboardsStar != drawDebugBillboards)
                    {
                        obj.altTreesManagerData.drawDebugBillboards = drawDebugBillboards;
                        EditorUtility.SetDirty(obj.altTreesManagerData);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        drawDebugBillboardsStar = drawDebugBillboards;
                        serializedObject.ApplyModifiedProperties();

                        obj.ReInit(true);
                    }

                    debugLog = GUILayout.Toggle(debugLog, "Debug Logs");
                    debugLogInBilds = GUILayout.Toggle(debugLogInBilds, "Debug Logs in Builds");
                    hideGroupBillboards = GUILayout.Toggle(hideGroupBillboards, "Hide GroupBillboards");

                }
                GUILayout.EndVertical();

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                GUI.enabled = true;


                GUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    GUILayout.Label("Import/Export Trees", sty5);


                    isImport = EditorGUILayout.Foldout(isImport, "Import trees from Terrain");
                    if (isImport)
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        {
                            GUILayout.Label("Select the terrain for import:");
                            EditorGUILayout.Space();

                            terrainTempImport = (Terrain)EditorGUILayout.ObjectField(terrainTempImport, typeof(Terrain), true);


                            if (terrainTempImport != null)
                            {
                                EditorGUILayout.Space();
                                EditorGUILayout.Space();

                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("Count of instance trees for import: ", sty, GUILayout.Width(190));

                                    sty.fontStyle = FontStyle.Bold;
                                    GUILayout.Label(terrainTempImport.terrainData.treeInstances.Length.ToString(), sty);
                                    sty.fontStyle = FontStyle.Normal;
                                }
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("Count of prototype trees: ", sty, GUILayout.Width(150));

                                    sty.fontStyle = FontStyle.Bold;
                                    GUILayout.Label(terrainTempImport.terrainData.treePrototypes.Length.ToString(), sty);
                                    sty.fontStyle = FontStyle.Normal;
                                }
                                GUILayout.EndHorizontal();

                                EditorGUILayout.Space();

                                isDeleteTreesFromTerrain = GUILayout.Toggle(isDeleteTreesFromTerrain, "Delete trees from Terrain");

                                EditorGUILayout.Space();

                                if (terrainTempImport.terrainData.treeInstances.Length == 0)
                                    GUI.enabled = false;

                                if (GUILayout.Button("Import"))
                                {
                                    serializedObject.ApplyModifiedProperties();

                                    Import();

                                    obj.altTreesManagerData.draw = true;
                                    EditorUtility.SetDirty(obj.altTreesManagerData);
                                    obj.ReInit(true);

                                    terrainTempImport = null;
                                    isImport = false;
                                }

                                GUI.enabled = true;
                                EditorGUILayout.Space();
                            }
                        }
                        GUILayout.EndVertical();
                    }

                    isExport = EditorGUILayout.Foldout(isExport, "Export trees to Terrain");
                    if (isExport)
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        {
                            GUILayout.Label("Select the terrain for export:");
                            EditorGUILayout.Space();

                            terrainTempExport = (Terrain)EditorGUILayout.ObjectField(terrainTempExport, typeof(Terrain), true);


                            if (terrainTempExport != null)
                            {
                                TerrainData terrainData = terrainTempExport.terrainData;


                                if (terrainTempExportStar == null || !terrainTempExportStar.Equals(terrainTempExport))
                                {
                                    attTemp = new List<AltTreesTrees>();
                                    prototypesListTemp = new List<int>();

                                    listPatchesExport = obj.getPatches(terrainTempExport.transform.position + obj.altTreesManager.jump * obj.altTreesManagerData.sizePatch, terrainData.size.x, terrainData.size.z);
                                    for (int i = 0; i < listPatchesExport.Length; i++)
                                    {
                                        AltTreesTrees[] attArrayTemp = listPatchesExport[i].getTreesForExport(new Vector2((terrainTempExport.transform.position + obj.altTreesManager.jump * obj.altTreesManagerData.sizePatch).x, (terrainTempExport.transform.position + obj.altTreesManager.jump * obj.altTreesManagerData.sizePatch).z), terrainData.size.x, terrainData.size.z);

                                        if (attArrayTemp != null && attArrayTemp.Length != 0)
                                        {
                                            for (int k = 0; k < attArrayTemp.Length; k++)
                                            {
                                                attTemp.Add(attArrayTemp[k]);
                                            }
                                        }
                                    }


                                    for (int i = 0; i < attTemp.Count; i++)
                                    {
                                        if (!prototypesListTemp.Contains(attTemp[i].idPrototype))
                                        {
                                            prototypesListTemp.Add(attTemp[i].idPrototype);
                                        }
                                    }



                                    altTreesArrayExport = new AltTree[prototypesListTemp.Count];
                                    terrainTreesArrayExport = new GameObject[prototypesListTemp.Count];

                                    for (int i = 0; i < prototypesListTemp.Count; i++)
                                    {
                                        altTreesArrayExport[i] = dataLinks.getAltTree(prototypesListTemp[i]);
                                        terrainTreesArrayExport[i] = dataLinks.getTree(prototypesListTemp[i]);
                                    }

                                    terrainTempExportStar = terrainTempExport;
                                }


                                EditorGUILayout.Space();
                                EditorGUILayout.Space();

                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("Count of instance trees for export: ", sty, GUILayout.Width(190));

                                    sty.fontStyle = FontStyle.Bold;
                                    GUILayout.Label(attTemp.Count.ToString() + " - " + terrainData.treeInstances.Length, sty);
                                    sty.fontStyle = FontStyle.Normal;
                                }
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("Count of prototype trees: ", sty, GUILayout.Width(150));

                                    sty.fontStyle = FontStyle.Bold;
                                    GUILayout.Label(prototypesListTemp.Count.ToString() + " - " + terrainData.treePrototypes.Length, sty);
                                    sty.fontStyle = FontStyle.Normal;
                                }
                                GUILayout.EndHorizontal();

                                EditorGUILayout.Space();

                                isDeleteTreesFromTerrain = GUILayout.Toggle(isDeleteTreesFromTerrain, "Delete trees from Terrain");
                                isDeleteTreesFromAltTrees = GUILayout.Toggle(isDeleteTreesFromAltTrees, "Delete trees from AltTrees");

                                EditorGUILayout.Space();



                                if (!(attTemp.Count == 0 || prototypesListTemp.Count == 0))
                                {
                                    sty.fontStyle = FontStyle.Bold;
                                    GUILayout.Label("Prototypes Dependency:", sty);


                                    GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(430));
                                    {
                                        GUILayout.BeginHorizontal();
                                        {
                                            GUILayout.BeginVertical(GUILayout.Width(20));
                                            {
                                                GUILayout.Label("", sty);
                                            }
                                            GUILayout.EndVertical();
                                            GUILayout.BeginVertical(sty3, GUILayout.MaxWidth(200));
                                            {
                                                GUILayout.Label("AltTrees:", sty);
                                            }
                                            GUILayout.EndVertical();
                                            GUILayout.BeginVertical(sty3, GUILayout.MaxWidth(200));
                                            {
                                                GUILayout.Label("Terrain Trees:", sty);
                                            }
                                            GUILayout.EndVertical();
                                        }
                                        GUILayout.EndHorizontal();


                                        for (int i = 0; i < prototypesListTemp.Count; i++)
                                        {
                                            GUILayout.BeginHorizontal();
                                            {
                                                GUILayout.BeginVertical(GUILayout.Width(20));
                                                {
                                                    GUILayout.Label((i + 1) + ".", sty);
                                                }
                                                GUILayout.EndVertical();
                                                GUILayout.BeginVertical();
                                                {
                                                    if (altTreesArrayExport[i] != null)
                                                    {
                                                        Texture2D tex = null;
                                                        tex = AssetPreview.GetAssetPreview(altTreesArrayExport[i].lods[0]);
                                                        if (tex != null)
                                                            GUILayout.Label(tex, GUILayout.Width(90), GUILayout.Height(90));
                                                        EditorGUILayout.ObjectField(altTreesArrayExport[i], typeof(AltTree), false, GUILayout.Width(150));
                                                    }

                                                }
                                                GUILayout.EndVertical();
                                                GUILayout.BeginVertical();
                                                {
                                                    if (altTreesArrayExport[i] != null)
                                                    {
                                                        Texture2D tex = null;
                                                        tex = AssetPreview.GetAssetPreview(terrainTreesArrayExport[i]);
                                                        if (tex != null)
                                                            GUILayout.Label(tex, GUILayout.Width(90), GUILayout.Height(90));
                                                        else
                                                            GUILayout.Label("", GUILayout.Width(90), GUILayout.Height(90));
                                                        terrainTreesArrayExport[i] = EditorGUILayout.ObjectField(terrainTreesArrayExport[i], typeof(GameObject), false, GUILayout.Width(150)) as GameObject;
                                                    }

                                                }
                                                GUILayout.EndVertical();

                                                this.Repaint();
                                            }
                                            GUILayout.EndHorizontal();
                                        }
                                    }
                                    GUILayout.EndVertical();






                                    sty.fontStyle = FontStyle.Normal;

                                    EditorGUILayout.Space();


                                    bool isStop = false;
                                    for (int i = 0; i < prototypesListTemp.Count; i++)
                                    {
                                        LODGroup lodGroup = terrainTreesArrayExport[i].GetComponent<LODGroup>();
                                        SerializedObject objj;
                                        SerializedProperty prop = null;
                                        GameObject goo = null;
                                        if (lodGroup != null)
                                        {
                                            objj = new SerializedObject(lodGroup);
                                            if (lodGroup.lodCount > 0)
                                            {
                                                prop = objj.FindProperty("m_LODs.Array.data[0].renderers");
                                                if (prop.arraySize > 0)
                                                {
                                                    goo = (prop.GetArrayElementAtIndex(0).FindPropertyRelative("renderer").objectReferenceValue as Renderer).gameObject;
                                                }
                                            }
                                        }


                                        if (terrainTreesArrayExport[i] == null || !(terrainTreesArrayExport[i].GetComponent<Tree>() != null || (goo != null && goo.GetComponent<Tree>() != null)))
                                            isStop = true;
                                    }


                                    if (isStop)
                                        GUI.enabled = false;

                                }
                                else
                                    GUI.enabled = false;

                                if (GUILayout.Button("Export"))
                                {
                                    Export();

                                    serializedObject.ApplyModifiedProperties();
                                    obj.ReInit(true);

                                    terrainTempExport = null;
                                    isExport = false;
                                }

                                GUI.enabled = true;
                                EditorGUILayout.Space();
                            }
                        }
                        GUILayout.EndVertical();
                    }
                }
                GUILayout.EndVertical();

                EditorGUILayout.Space();



                if (GUI.changed && menuIdStar == menuId.intValue && menuId.intValue == 3)
                {
                    obj.altTreesManagerData.sizePatch = sizePatch;
                    obj.altTreesManagerData.maxLOD = maxLOD;
                    obj.altTreesManagerData.distancePatchFactor = distancePatchFactor;
                    obj.altTreesManagerData.distanceTreesLODFactor = distanceTreesLODFactor;
                    obj.altTreesManagerData.distanceObjectsLODFactor = distanceObjectsLODFactor;
                    obj.altTreesManagerData.checkTreesPercentPerFrame = checkTreesPercentPerFrame;
                    obj.altTreesManagerData.crossFadeTimeBillboard = crossFadeTimeBillboard;
                    obj.altTreesManagerData.crossFadeTimeMesh = crossFadeTimeMesh;

                    obj.altTreesManagerData.initObjsCountPool = initObjsCountPool;
                    obj.altTreesManagerData.objsPerOneMaxPool = objsPerOneMaxPool;

                    obj.altTreesManagerData.initBillboardCountPool = initBillboardCountPool;
                    obj.altTreesManagerData.billboardsMaxPool = billboardsMaxPool;

                    obj.altTreesManagerData.initCollidersCountPool = initCollidersCountPool;
                    obj.altTreesManagerData.collidersPerOneMaxPool = collidersPerOneMaxPool;
                    obj.altTreesManagerData.initColliderBillboardsCountPool = initColliderBillboardsCountPool;
                    obj.altTreesManagerData.colliderBillboardsPerOneMaxPool = colliderBillboardsPerOneMaxPool;

                    obj.altTreesManagerData.draw = draw;
                    obj.altTreesManagerData.autoConfig = autoConfig;
                    obj.altTreesManagerData.generateAllBillboardsOnStart = generateAllBillboardsOnStart;
                    obj.altTreesManagerData.enableColliders = enableColliders;
                    obj.altTreesManagerData.drawDebugPutches = drawDebugPutches;
                    obj.altTreesManagerData.drawDebugBillboards = drawDebugBillboards;
                    obj.altTreesManagerData.drawDebugBillboardsStar = drawDebugBillboardsStar;
                    obj.altTreesManagerData.debugLog = debugLog;
                    obj.altTreesManagerData.debugLogInBilds = debugLogInBilds;
                    obj.altTreesManagerData.hideGroupBillboards = hideGroupBillboards;

                    EditorUtility.SetDirty(obj.altTreesManagerData);

                    isRefresh = true;
                }
                menuIdStar = menuId.intValue;
            }

            serializedObject.ApplyModifiedProperties();

            if (treeTemp != null)
            {
                if (!hueColorLeaves.Equals(treeTemp.hueVariationLeaves))
                {
                    treeTemp.hueVariationLeaves = hueColorLeaves;
                    EditorUtility.SetDirty(treeTemp);
                }
                if (!hueColorBark.Equals(treeTemp.hueVariationBark))
                {
                    treeTemp.hueVariationBark = hueColorBark;
                    EditorUtility.SetDirty(treeTemp);
                }
            }

            if (hideGroupBillboards != hideGroupBillboardsStar)
            {
                hideGroupBillboardsStar = hideGroupBillboards;
                obj.ReInit(true);
            }
        }

        public void OnSceneGUI()
        {
            Event current = Event.current;

            getDataLinks();

            if (obj.dataLinksCorrupted)
                return;
            
            if(checkTreeVersionsStatus)
                return;

            if (current.shift)
                isPlacingShift = true;
            else
                isPlacingShift = false;
            if (current.control)
                isPlacingCtrl = true;
            else
                isPlacingCtrl = false;


            if ((menuId.intValue == 1 && idTreeSelected.intValue != -1) && (current.type == EventType.MouseMove || current.type == EventType.MouseDown || current.type == EventType.MouseDrag))
            {
                projector.orthographicSize = brushSize.intValue;

                Ray worldRay = HandleUtility.GUIPointToWorldRay(current.mousePosition);
                RaycastHit hitInfo;


                if (Physics.Raycast(worldRay, out hitInfo))
                {
                    projectorTransform.position = hitInfo.point + Vector3.up * 100;

                    if ((current.type == EventType.MouseDown || current.type == EventType.MouseDrag) && current.button == 0)
                    {
                        Dictionary<AltTreesPatch, List<AddTreesStruct>> tempListPatches = new Dictionary<AltTreesPatch, List<AddTreesStruct>>();

                        if (isPlacingShift)
                        {
                            List<int> removedTrees = new List<int>();
                            List<int> removedTreesNoGroup = new List<int>();
                            AltTreesPatch[] listPatches = obj.getPatches(hitInfo.point + obj.altTreesManager.jump * obj.altTreesManagerData.sizePatch, 0.9f * brushSize.intValue);
                            for (int i = 0; i < listPatches.Length; i++)
                            {
                                removedTrees.Clear();
                                removedTreesNoGroup.Clear();
                                if (listPatches[i].removeTrees(new Vector2(hitInfo.point.x, hitInfo.point.z), 0.9f * brushSize.intValue, removedTrees, removedTreesNoGroup))
                                {
                                    if (listPatches[i].treesCount == listPatches[i].treesEmptyCount + removedTrees.Count && listPatches[i].treesNoGroupCount == listPatches[i].treesNoGroupEmptyCount + removedTreesNoGroup.Count)
                                        removePatch(listPatches[i]);
                                    else
                                    {
                                        if (removedTrees.Count > 0)
                                            listPatches[i].EditDataFile(false, null, 0, removedTrees);

                                        if (listPatches[i].treesCount == listPatches[i].treesEmptyCount)
                                        {
                                            if (listPatches[i].treesData != null)
                                            {
                                                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(listPatches[i].treesData));

                                                EditorUtility.SetDirty(obj.altTreesManagerData);
                                                AssetDatabase.SaveAssets();
                                                AssetDatabase.Refresh();
                                            }
                                        }
                                        if (removedTreesNoGroup.Count > 0)
                                            listPatches[i].EditDataFile(true, null, 0, removedTreesNoGroup);

                                        if (listPatches[i].treesNoGroupCount == listPatches[i].treesNoGroupEmptyCount)
                                        {
                                            if (listPatches[i].treesNoGroupData != null)
                                            {
                                                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(listPatches[i].treesNoGroupData));

                                                EditorUtility.SetDirty(obj.altTreesManagerData);
                                                AssetDatabase.SaveAssets();
                                                AssetDatabase.Refresh();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (isPlacingCtrl)
                        {
                            List<int> removedTrees = new List<int>();
                            List<int> removedTreesNoGroup = new List<int>();
                            AltTreesPatch[] listPatches = obj.getPatches(hitInfo.point + obj.altTreesManager.jump * obj.altTreesManagerData.sizePatch, 0.9f * brushSize.intValue);
                            for (int i = 0; i < listPatches.Length; i++)
                            {
                                removedTrees.Clear();
                                removedTreesNoGroup.Clear();
                                if (listPatches[i].removeTrees(new Vector2(hitInfo.point.x, hitInfo.point.z), 0.9f * brushSize.intValue, removedTrees, removedTreesNoGroup, dataLinks.altTrees[treeIdsTemp[idTreeSelected.intValue]]))
                                {
                                    if (listPatches[i].treesCount == listPatches[i].treesEmptyCount + removedTrees.Count && listPatches[i].treesNoGroupCount == listPatches[i].treesNoGroupEmptyCount + removedTreesNoGroup.Count)
                                        removePatch(listPatches[i]);
                                    else
                                    {
                                        if (removedTrees.Count > 0)
                                            listPatches[i].EditDataFile(false, null, 0, removedTrees);

                                        if (listPatches[i].treesCount == listPatches[i].treesEmptyCount)
                                        {
                                            if (listPatches[i].treesData != null)
                                            {
                                                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(listPatches[i].treesData));

                                                EditorUtility.SetDirty(obj.altTreesManagerData);
                                                AssetDatabase.SaveAssets();
                                                AssetDatabase.Refresh();
                                            }
                                        }
                                        if (removedTreesNoGroup.Count > 0)
                                            listPatches[i].EditDataFile(true, null, 0, removedTreesNoGroup);

                                        if (listPatches[i].treesNoGroupCount == listPatches[i].treesNoGroupEmptyCount)
                                        {
                                            if (listPatches[i].treesNoGroupData != null)
                                            {
                                                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(listPatches[i].treesNoGroupData));

                                                EditorUtility.SetDirty(obj.altTreesManagerData);
                                                AssetDatabase.SaveAssets();
                                                AssetDatabase.Refresh();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Vector2 randVector = new Vector2();
                            RaycastHit hitInfo2;
                            int countPlace = 0;

                            if (current.type == EventType.MouseDown)
                                countPlace = treeCount.intValue;
                            else
                                countPlace = Mathf.CeilToInt((((float)treeCount.intValue) / 10f) * ((float)speedPlace.intValue));

                            int sch = 0;
                            for (int i = 0; i < countPlace; i++)
                            {
                                randVector = UnityEngine.Random.insideUnitCircle * 0.9f * brushSize.intValue;

                                if (Physics.Raycast(hitInfo.point + new Vector3(randVector.x, 100f, randVector.y), Vector3.up * -1, out hitInfo2, 200f) && Vector3.Angle(hitInfo2.normal.normalized, Vector3.up) <= angleLimit.floatValue)
                                {
                                    sch = 0;
                                    AltTreesPatch tempPatch = getPatch(hitInfo2.point);

                                    if (tempListPatches.ContainsKey(tempPatch))
                                    {
                                        tempListPatches[tempPatch].Add(new AddTreesStruct(hitInfo2.point, dataLinks.altTrees[treeIdsTemp[idTreeSelected.intValue]]));
                                    }
                                    else
                                    {
                                        tempPatch.checkTreePrototype(dataLinks.altTrees[treeIdsTemp[idTreeSelected.intValue]].id, dataLinks.altTrees[treeIdsTemp[idTreeSelected.intValue]]);
                                        tempListPatches.Add(tempPatch, new List<AddTreesStruct>());
                                        tempListPatches[tempPatch].Add(new AddTreesStruct(hitInfo2.point, dataLinks.altTrees[treeIdsTemp[idTreeSelected.intValue]]));
                                    }
                                }
                                else
                                {
                                    sch++;

                                    if (sch < 100)
                                        i--;
                                    else
                                        sch = 0;
                                }
                            }

                            foreach (AltTreesPatch key in tempListPatches.Keys)
                            {
                                key.addTrees(tempListPatches[key].ToArray(), randomRotation.boolValue, isRandomHeight.boolValue, height.floatValue, heightRandom.floatValue,
                                             lockWidthToHeight.boolValue, isRandomWidth.boolValue, width.floatValue, widthRandom.floatValue, hueColorLeaves, hueColorBark, isRandomHueLeaves.boolValue, isRandomHueBark.boolValue);
                            }
                        }
                    }
                }
                else
                    projectorTransform.position = Vector3.up * -1000000;
            }
        }



        AltTreesPatch getPatch(Vector3 pos, int sizePatch = 0)
        {
            if (sizePatch == 0)
                sizePatch = obj.altTreesManagerData.sizePatch;
            AltTreesPatch altTreesPatchTemp = obj.getPatch(pos + obj.altTreesManager.jump * sizePatch, sizePatch);
            if (altTreesPatchTemp != null)
                return altTreesPatchTemp;
            else
                return addPatch(Mathf.FloorToInt(pos.x / ((float)sizePatch)) + (int)obj.altTreesManager.jump.x, Mathf.FloorToInt(pos.z / ((float)sizePatch)) + (int)obj.altTreesManager.jump.z);
        }

        AltTreesPatch addPatch(int _stepX, int _stepY)
        {
            AltTreesPatch atpTemp = new AltTreesPatch(_stepX, _stepY);

            atpTemp.prototypes = new AltTreePrototypes[0];
            atpTemp.trees = new AltTreesTrees[0];

            AltTreesPatch[] patchesTemp = obj.altTreesManagerData.patches;
            obj.altTreesManagerData.patches = new AltTreesPatch[patchesTemp.Length + 1];
            for (int i = 0; i < patchesTemp.Length; i++)
            {
                obj.altTreesManagerData.patches[i] = patchesTemp[i];
            }
            obj.altTreesManagerData.patches[patchesTemp.Length] = atpTemp;

            EditorUtility.SetDirty(obj.altTreesManagerData);
            atpTemp.Init(obj.altTreesManager, obj, obj.altTreesManagerData);
            obj.altTreesManager.addAltTrees(atpTemp);

            return atpTemp;
        }


        void removePatch(AltTreesPatch patch)
        {
            for (int i = 0; i < obj.altTreesManagerData.patches.Length; i++)
            {
                if (obj.altTreesManagerData.patches[i].Equals(patch))
                {
                    int count = 0;
                    AltTreesPatch[] patchesTemp = obj.altTreesManagerData.patches;
                    obj.altTreesManagerData.patches = new AltTreesPatch[patchesTemp.Length - 1];
                    for (int j = 0; j < patchesTemp.Length; j++)
                    {
                        if (!patchesTemp[j].Equals(patch))
                        {
                            obj.altTreesManagerData.patches[count] = patchesTemp[j];
                            count++;
                        }
                    }

                    if (patch.treesData != null)
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(patch.treesData));
                    if (patch.treesNoGroupData != null)
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(patch.treesNoGroupData));
                    obj.altTreesManager.removeAltTrees(patch, false);

                    EditorUtility.SetDirty(obj.altTreesManagerData);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    return;
                }
            }
        }

        Rect GetBrushAspectRect(int elementCount, int approxSize, int extraLineHeight)
        {
            int num1 = (int)Mathf.Ceil((float)((Screen.width - 20) / approxSize));
            int num2 = elementCount / num1;
            if (elementCount % num1 != 0)
                ++num2;
            Rect aspectRect = GUILayoutUtility.GetAspectRect((float)num1 / (float)num2);
            Rect rect = GUILayoutUtility.GetRect(10f, (float)(extraLineHeight * num2));
            aspectRect.height += rect.height;
            return aspectRect;
        }

        public void Import()
        {
            getDataLinks();

            if (obj.dataLinksCorrupted)
                return;

            TerrainData terrainData = terrainTempImport.terrainData;

            int countTemp = 0;
            int treePrototypesLength = terrainData.treePrototypes.Length;

            AltTree[] prototypesTemp = new AltTree[treePrototypesLength];



            for (int i = 0; i < treePrototypesLength; i++)
            {
                AltTree treeTemp2 = null;
                treeTemp2 = dataLinks.getAltTree(terrainData.treePrototypes[i].prefab);

                if (treeTemp2 == null)
                {
                    ImportTreeWindow.ConvertTree(terrainData.treePrototypes[i].prefab, this, terrainTempImport, isDeleteTreesFromTerrain, isDeleteTreesFromAltTrees);
                    return;
                }

                prototypesTemp[i] = dataLinks.getAltTree(terrainData.treePrototypes[i].prefab);
            }

            EditorUtility.DisplayProgressBar("Import trees from Terrain... ", "Starting... ", 0.0f);

            for (int i = 0; i < prototypesTemp.Length; i++)
            {
                if (prototypesTemp[i] == null)
                    Debug.Log("null pr");
            }

            int treeCounts = terrainData.treeInstances.Length;


            Dictionary<AltTreesPatch, List<ImportTreesStruct>> tempListPatches = new Dictionary<AltTreesPatch, List<ImportTreesStruct>>();

            Color colorLeavesTemp;
            Color colorBarkTemp;

            for (int j = 0; j < treeCounts; j++)
            {
                Vector3 posTemp = Vector3.Scale(terrainData.treeInstances[j].position, terrainData.size) + terrainTempImport.GetPosition();

                AltTreesPatch tempPatch = getPatch(posTemp);

                colorLeavesTemp = prototypesTemp[terrainData.treeInstances[j].prototypeIndex].hueVariationLeaves;
                colorLeavesTemp.a = UnityEngine.Random.value * colorLeavesTemp.a;
                colorBarkTemp = prototypesTemp[terrainData.treeInstances[j].prototypeIndex].hueVariationBark;
                colorBarkTemp.a = UnityEngine.Random.value * colorBarkTemp.a;

                if (tempListPatches.ContainsKey(tempPatch))
                {
                    tempPatch.checkTreePrototype(prototypesTemp[terrainData.treeInstances[j].prototypeIndex].id, prototypesTemp[terrainData.treeInstances[j].prototypeIndex]);
                    tempListPatches[tempPatch].Add(new ImportTreesStruct(posTemp, prototypesTemp[terrainData.treeInstances[j].prototypeIndex].id, colorLeavesTemp, colorBarkTemp, terrainData.treeInstances[j].lightmapColor, terrainData.treeInstances[j].rotation * 57.2958f, terrainData.treeInstances[j].heightScale, terrainData.treeInstances[j].widthScale, prototypesTemp[terrainData.treeInstances[j].prototypeIndex].isObject, prototypesTemp[terrainData.treeInstances[j].prototypeIndex]));
                }
                else
                {
                    tempPatch.checkTreePrototype(prototypesTemp[terrainData.treeInstances[j].prototypeIndex].id, prototypesTemp[terrainData.treeInstances[j].prototypeIndex]);
                    tempListPatches.Add(tempPatch, new List<ImportTreesStruct>());
                    tempListPatches[tempPatch].Add(new ImportTreesStruct(posTemp, prototypesTemp[terrainData.treeInstances[j].prototypeIndex].id, colorLeavesTemp, colorBarkTemp, terrainData.treeInstances[j].lightmapColor, terrainData.treeInstances[j].rotation * 57.2958f, terrainData.treeInstances[j].heightScale, terrainData.treeInstances[j].widthScale, prototypesTemp[terrainData.treeInstances[j].prototypeIndex].isObject, prototypesTemp[terrainData.treeInstances[j].prototypeIndex]));
                }


                countTemp++;
                if (countTemp > ((float)treeCounts) / 100f)
                {
                    EditorUtility.DisplayProgressBar("Import trees from Terrain... ", "Please wait... " + (int)(((float)j / (float)treeCounts) * 100f) + "%", (float)j / (float)treeCounts);
                    countTemp = 0;
                }
            }



            foreach (AltTreesPatch key in tempListPatches.Keys)
            {
                key.addTreesImport(tempListPatches[key].ToArray());
            }



            if (isDeleteTreesFromTerrain)
            {
                terrainData.treeInstances = new TreeInstance[0];
                terrainData.treePrototypes = new TreePrototype[0];

                terrainData.RefreshPrototypes();
                terrainTempImport.Flush();
            }
            else
                terrainTempImport.drawTreesAndFoliage = false;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();

            OnDisable();
            OnEnable();
        }


        void Export()
        {
            EditorUtility.DisplayProgressBar("Export trees to Terrain... ", "Starting... ", 0.0f);

            TerrainData terrainData = terrainTempExport.terrainData;

            int treePrototypesLength = terrainData.treePrototypes.Length;


            Dictionary<int, int> prototypesIndexArray = new Dictionary<int, int>();
            int treeInstancesStart = 0;
            TreeInstance[] tisTemp;

            if (!isDeleteTreesFromTerrain)
            {
                for (int i = 0; i < prototypesListTemp.Count; i++)
                {
                    bool isOk = false;
                    for (int k = 0; k < treePrototypesLength; k++)
                    {
                        if (terrainData.treePrototypes[k].prefab.Equals(terrainTreesArrayExport[i]))
                        {
                            prototypesIndexArray.Add(prototypesListTemp[i], k);
                            k = treePrototypesLength;
                            isOk = true;
                        }
                    }
                    if (!isOk)
                    {
                        TreePrototype[] tpsTemp = new TreePrototype[treePrototypesLength + 1];
                        TreePrototype tpTemp = new TreePrototype();
                        for (int k = 0; k < treePrototypesLength; k++)
                        {
                            tpsTemp[k] = terrainData.treePrototypes[k];
                        }
                        tpTemp.prefab = terrainTreesArrayExport[i];

                        tpsTemp[treePrototypesLength] = tpTemp;
                        terrainData.treePrototypes = tpsTemp;

                        prototypesIndexArray.Add(prototypesListTemp[i], treePrototypesLength);
                        treePrototypesLength++;
                    }
                }

                treeInstancesStart = terrainData.treeInstances.Length;
                tisTemp = new TreeInstance[terrainData.treeInstances.Length + attTemp.Count];

                for (int k = 0; k < terrainData.treeInstances.Length; k++)
                {
                    tisTemp[k] = terrainData.treeInstances[k];
                }

            }
            else
            {
                TreePrototype[] tpsTemp = new TreePrototype[prototypesListTemp.Count];
                for (int i = 0; i < prototypesListTemp.Count; i++)
                {
                    TreePrototype tpTemp = new TreePrototype();
                    tpTemp.prefab = terrainTreesArrayExport[i];
                    tpsTemp[i] = tpTemp;
                    prototypesIndexArray.Add(prototypesListTemp[i], i);
                }
                terrainData.treePrototypes = tpsTemp;
                treePrototypesLength = prototypesListTemp.Count;

                treeInstancesStart = 0;
                tisTemp = new TreeInstance[attTemp.Count];
            }


            for (int i = 0; i < attTemp.Count; i++)
            {
                Vector3 posTemp = (attTemp[i].getPosWorld() - terrainTempExport.GetPosition());
                posTemp.x = posTemp.x / terrainData.size.x;
                posTemp.y = posTemp.y / terrainData.size.y;
                posTemp.z = posTemp.z / terrainData.size.z;

                TreeInstance tiTemp = new TreeInstance();
                tiTemp.color = attTemp[i].color;
                tiTemp.heightScale = attTemp[i].heightScale;
                tiTemp.lightmapColor = attTemp[i].lightmapColor;
                tiTemp.position = posTemp;
                tiTemp.prototypeIndex = prototypesIndexArray[attTemp[i].idPrototype];
                tiTemp.rotation = attTemp[i].rotation / 57.2958f;
                tiTemp.widthScale = attTemp[i].widthScale;
                tisTemp[treeInstancesStart + i] = tiTemp;
            }

            terrainData.treeInstances = tisTemp;


            terrainData.RefreshPrototypes();
            terrainTempExport.Flush();


            if (isDeleteTreesFromAltTrees)
            {
                List<int> removedTrees = new List<int>();
                List<int> removedTreesNoGroup = new List<int>();
                for (int i = 0; i < listPatchesExport.Length; i++)
                {
                    removedTrees.Clear();
                    removedTreesNoGroup.Clear();
                    if (listPatchesExport[i].removeTrees(attTemp.ToArray(), new Vector2((terrainTempExport.transform.position + obj.altTreesManager.jump * obj.altTreesManagerData.sizePatch).x, (terrainTempExport.transform.position + obj.altTreesManager.jump * obj.altTreesManagerData.sizePatch).z), terrainData.size.x, terrainData.size.z, removedTrees, removedTreesNoGroup))
                    {
                        bool isStopDelete = false;
                        for (int j = 0; j < listPatchesExport[i].trees.Length; j++)
                        {
                            if (listPatchesExport[i].trees[j] != null && listPatchesExport[i].trees[j].noNull)
                            {
                                isStopDelete = true;
                                break;
                            }
                        }
                        if (!isStopDelete)
                        {
                            for (int j = 0; j < listPatchesExport[i].treesNoGroup.Length; j++)
                            {
                                if (listPatchesExport[i].treesNoGroup[j] != null && listPatchesExport[i].treesNoGroup[j].noNull)
                                {
                                    isStopDelete = true;
                                    break;
                                }
                            }
                        }

                        if (!isStopDelete)
                            removePatch(listPatchesExport[i]);
                        else
                        {
                            if (removedTrees.Count > 0)
                                listPatchesExport[i].EditDataFile(false, null, 0, removedTrees);
                            if (removedTreesNoGroup.Count > 0)
                                listPatchesExport[i].EditDataFile(true, null, 0, removedTreesNoGroup);

                            EditorUtility.SetDirty(listPatchesExport[i].altTreesManagerData);
                        }
                    }
                }
            }

            terrainTempExport.drawTreesAndFoliage = true;
            obj.altTreesManager.destroy(true);

            obj.altTreesManagerData.draw = false;
            EditorUtility.SetDirty(obj.altTreesManagerData);


            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }


        void massPlace()
        {
            Vector3 vect3Temp = new Vector3();
            float randX = 0f;
            float randZ = 0f;
            Dictionary<AltTreesPatch, List<AddTreesStruct>> tempListPatches = new Dictionary<AltTreesPatch, List<AddTreesStruct>>();

            for (int i = 0; i < countMassPlace; i++)
            {
                randX = UnityEngine.Random.value;
                randZ = UnityEngine.Random.value;
                vect3Temp.x = terrainTempMassPlace.GetPosition().x + terrainTempMassPlace.terrainData.size.x * randX;
                vect3Temp.z = terrainTempMassPlace.GetPosition().z + terrainTempMassPlace.terrainData.size.z * randZ;
                vect3Temp.y = terrainTempMassPlace.SampleHeight(vect3Temp);

                if (Vector3.Angle(terrainTempMassPlace.terrainData.GetInterpolatedNormal(randX, randZ).normalized, Vector3.up) <= angleLimit.floatValue)
                {
                    AltTreesPatch tempPatch = getPatch(vect3Temp);

                    if (tempListPatches.ContainsKey(tempPatch))
                    {
                        tempListPatches[tempPatch].Add(new AddTreesStruct(vect3Temp, dataLinks.altTrees[treeIdsTemp[idTreeSelected.intValue]]));
                    }
                    else
                    {
                        tempPatch.checkTreePrototype(dataLinks.altTrees[treeIdsTemp[idTreeSelected.intValue]].id, dataLinks.altTrees[treeIdsTemp[idTreeSelected.intValue]]);
                        tempListPatches.Add(tempPatch, new List<AddTreesStruct>());
                        tempListPatches[tempPatch].Add(new AddTreesStruct(vect3Temp, dataLinks.altTrees[treeIdsTemp[idTreeSelected.intValue]]));
                    }
                }
                else
                    i--;
            }

            foreach (AltTreesPatch key in tempListPatches.Keys)
            {
                key.addTrees(tempListPatches[key].ToArray(), randomRotation.boolValue, isRandomHeight.boolValue, height.floatValue, heightRandom.floatValue,
                             lockWidthToHeight.boolValue, isRandomWidth.boolValue, width.floatValue, widthRandom.floatValue, hueColorLeaves, hueColorBark, isRandomHueLeaves.boolValue, isRandomHueBark.boolValue);
                EditorUtility.SetDirty(key.altTreesManagerData);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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

                if(dataLinks == null)
                {
                    obj.dataLinksIsCorrupted();
                }
            }
        }

        void resizePatches(int sizePatchTemp)
        {
            AltTreesPatch[] patchesTemp = obj.altTreesManagerData.patches;

            for (int i = 0; i < patchesTemp.Length; i++)
            {
                if (patchesTemp[i].treesData != null)
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(patchesTemp[i].treesData));
                if (patchesTemp[i].treesNoGroupData != null)
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(patchesTemp[i].treesNoGroupData));
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            obj.altTreesManagerData.patches = new AltTreesPatch[0];

            Dictionary<AltTreesPatch, List<ImportTreesStruct>> tempListPatches = new Dictionary<AltTreesPatch, List<ImportTreesStruct>>();


            for (int i = 0; i < patchesTemp.Length; i++)
            {
                if (patchesTemp[i].trees != null)
                {
                    for (int h = 0; h < patchesTemp[i].trees.Length; h++)
                    {
                        AltTreesPatch tempPatch = getPatch(patchesTemp[i].trees[h].getPosWorld(), sizePatchTemp);

                        if (tempListPatches.ContainsKey(tempPatch))
                        {
                            tempPatch.checkTreePrototype(patchesTemp[i].trees[h].idPrototype, dataLinks.getAltTree(patchesTemp[i].trees[h].idPrototype));
                            tempListPatches[tempPatch].Add(new ImportTreesStruct(patchesTemp[i].trees[h].getPosWorld(), patchesTemp[i].trees[h].idPrototype, patchesTemp[i].trees[h].color, patchesTemp[i].trees[h].colorBark, patchesTemp[i].trees[h].lightmapColor, patchesTemp[i].trees[h].rotation, patchesTemp[i].trees[h].heightScale, patchesTemp[i].trees[h].widthScale, false, dataLinks.getAltTree(patchesTemp[i].trees[h].idPrototype)));
                        }
                        else
                        {
                            tempPatch.checkTreePrototype(patchesTemp[i].trees[h].idPrototype, dataLinks.getAltTree(patchesTemp[i].trees[h].idPrototype));
                            tempListPatches.Add(tempPatch, new List<ImportTreesStruct>());
                            tempListPatches[tempPatch].Add(new ImportTreesStruct(patchesTemp[i].trees[h].getPosWorld(), patchesTemp[i].trees[h].idPrototype, patchesTemp[i].trees[h].color, patchesTemp[i].trees[h].colorBark, patchesTemp[i].trees[h].lightmapColor, patchesTemp[i].trees[h].rotation, patchesTemp[i].trees[h].heightScale, patchesTemp[i].trees[h].widthScale, false, dataLinks.getAltTree(patchesTemp[i].trees[h].idPrototype)));
                        }
                    }
                }
                if (patchesTemp[i].treesNoGroup != null)
                {
                    for (int h = 0; h < patchesTemp[i].treesNoGroup.Length; h++)
                    {
                        AltTreesPatch tempPatch = getPatch(patchesTemp[i].treesNoGroup[h].getPosWorld(), sizePatchTemp);

                        if (tempListPatches.ContainsKey(tempPatch))
                        {
                            tempPatch.checkTreePrototype(patchesTemp[i].treesNoGroup[h].idPrototype, dataLinks.getAltTree(patchesTemp[i].treesNoGroup[h].idPrototype));
                            tempListPatches[tempPatch].Add(new ImportTreesStruct(patchesTemp[i].treesNoGroup[h].getPosWorld(), patchesTemp[i].treesNoGroup[h].idPrototype, patchesTemp[i].treesNoGroup[h].color, patchesTemp[i].treesNoGroup[h].colorBark, patchesTemp[i].treesNoGroup[h].lightmapColor, patchesTemp[i].treesNoGroup[h].rotation, patchesTemp[i].treesNoGroup[h].heightScale, patchesTemp[i].treesNoGroup[h].widthScale, true, dataLinks.getAltTree(patchesTemp[i].treesNoGroup[h].idPrototype)));
                        }
                        else
                        {
                            tempPatch.checkTreePrototype(patchesTemp[i].treesNoGroup[h].idPrototype, dataLinks.getAltTree(patchesTemp[i].treesNoGroup[h].idPrototype));
                            tempListPatches.Add(tempPatch, new List<ImportTreesStruct>());
                            tempListPatches[tempPatch].Add(new ImportTreesStruct(patchesTemp[i].treesNoGroup[h].getPosWorld(), patchesTemp[i].treesNoGroup[h].idPrototype, patchesTemp[i].treesNoGroup[h].color, patchesTemp[i].treesNoGroup[h].colorBark, patchesTemp[i].treesNoGroup[h].lightmapColor, patchesTemp[i].treesNoGroup[h].rotation, patchesTemp[i].treesNoGroup[h].heightScale, patchesTemp[i].treesNoGroup[h].widthScale, true, dataLinks.getAltTree(patchesTemp[i].treesNoGroup[h].idPrototype)));
                        }
                    }
                }
            }


            obj.altTreesManagerData.sizePatch = sizePatchTemp;
            EditorUtility.SetDirty(obj.altTreesManagerData);
            obj.ReInit(true);

            foreach (AltTreesPatch key in tempListPatches.Keys)
            {
                key.addTreesImport(tempListPatches[key].ToArray());
            }
            EditorUtility.SetDirty(obj.altTreesManagerData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void CreateAltTreesManagerData()
        {
            if (!System.IO.Directory.Exists("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesData/" + obj.getIdManager()))
            {
                System.IO.Directory.CreateDirectory("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesData/" + obj.getIdManager());
            }

            if (!System.IO.File.Exists("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesData/" + obj.getIdManager() + "/altTreesManagerData.asset"))
            {
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<AltTreesManagerData>(), "Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesData/" + obj.getIdManager() + "/altTreesManagerData.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            obj.altTreesManagerData = (AltTreesManagerData)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesData/" + obj.getIdManager() + "/altTreesManagerData.asset", typeof(AltTreesManagerData));
            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }
}