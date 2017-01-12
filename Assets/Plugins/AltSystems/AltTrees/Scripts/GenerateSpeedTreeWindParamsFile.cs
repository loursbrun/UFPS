#if UNITY_EDITOR

using System.IO;
using UnityEngine;

using UnityEditor;

namespace AltSystems.AltTrees.Tools
{
    public class GenerateSpeedTreeWindParamsFile : MonoBehaviour
    {
        Terrain terr;
        TerrainData terrData;
        WindZone wind;


        GameObject[] weirdTree;
        WindScanParamsStruct[] paramsWindZnach;
        WindScanParamsStruct[] paramsWindZnachStar;
        WindScanParamsStruct[] paramsWindApper;
        WindScanParamsStruct[] paramsWindApperStar;
        WindScanParamsStruct[] paramsWindSumm;
        WindScanParamsStruct[] paramsWind;
        WindScanParamsStruct[] paramsWind2;
        public GameObject windZone;

        float timStart;
        int idZamer = 0;
        int countOk = 0;
        int schWind = 0;
        float windMainZnach = 1f;
        bool stopped = false;
        bool isEnd = false;
        bool started = false;

        Texture2D blackTex;
        string[] filesTemp;
        Vector2 scroll = new Vector2();
        GUIStyle sty = new GUIStyle();

        void Awake()
        {
            blackTex = new Texture2D(2, 2);
            Color[] cols = new Color[4];
            cols[0] = Color.black;
            cols[1] = Color.black;
            cols[2] = Color.black;
            cols[3] = Color.black;
            blackTex.SetPixels(cols);
            blackTex.Apply();

            filesTemp = Directory.GetFiles("Assets", "*.spm", SearchOption.AllDirectories);

            sty.fontStyle = FontStyle.Bold;
            sty.normal.textColor = Color.white;
        }

        void OnGUI()
        {
            GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), blackTex);

            GUI.BeginGroup(new Rect((Screen.width - 600) / 2f, 0, 600, Screen.height));
            {

                GUILayout.BeginVertical(GUILayout.Width(600));
                {
                    GUILayout.Space(30);
                    if (started)
                    {
                        if (idZamer != 11)
                        {
                            if (GUILayout.Button("Progress:    " + Mathf.FloorToInt(100f/10.1f * idZamer) + "%", GUILayout.Height(35)))
                            {
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("Done", GUILayout.Height(35)))
                            {
                            }
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Start Scanning", GUILayout.Height(35)))
                        {
                            StartScan();
                        }
                    }
                    GUILayout.Space(20);

                    scroll = GUILayout.BeginScrollView(scroll);
                    {
                        GUILayout.Label("SpeedTree List for Scanning:", sty);

                        if (!started)
                        {
                            for (int i = 0; i < filesTemp.Length; i++)
                            {
                                GUILayout.Label(filesTemp[i].Replace(".spm", ""));
                            }
                        }
                        else
                        {
                            for (int i = 0; i < filesTemp.Length; i++)
                            {
                                string[] strs = filesTemp[i].Replace(".spm", "").Split('\\');
                                if (idZamer != 11)
                                    GUILayout.Label(strs[strs.Length - 1] + "    ...    Scanning...");
                                else
                                    GUILayout.Label(strs[strs.Length - 1] + "    ...    Done.");
                            }
                        }
                    }
                    GUILayout.EndScrollView();
                }
                GUILayout.EndVertical();
            }
            GUI.EndGroup();
        }

        void StartScan()
        {
            GameObject terrainObj = new GameObject("Terrain");
            GameObject windObj = new GameObject("Wind Zone");

            terrData = new TerrainData();

            terrData.size = new Vector3(10, 10, 10);
            terrData.heightmapResolution = 128;
            terrData.baseMapResolution = 128;
            terrData.SetDetailResolution(128, 16);

            terr = terrainObj.AddComponent<Terrain>();
            terr.hideFlags = HideFlags.HideAndDontSave;
            terrainObj.AddComponent<TerrainCollider>();
            terrainObj.GetComponent<TerrainCollider>().hideFlags = HideFlags.HideAndDontSave;

            wind = windObj.AddComponent<WindZone>();
            wind.hideFlags = HideFlags.HideAndDontSave;

            string[] files = Directory.GetFiles("Assets", "*.spm", SearchOption.AllDirectories);
            TreePrototype[] treePrototypes = new TreePrototype[files.Length];
            int i = 0;
            foreach (string str in files)
            {
                treePrototypes[i] = new TreePrototype();
                treePrototypes[i].prefab = AssetDatabase.LoadAssetAtPath(str, typeof(GameObject)) as GameObject;
                i++;
            }
            terrData.treePrototypes = treePrototypes;
            terr.terrainData = terrData;



            weirdTree = new GameObject[terrData.treePrototypes.Length];

            paramsWindZnach = new WindScanParamsStruct[terrData.treePrototypes.Length];
            paramsWindZnachStar = new WindScanParamsStruct[terrData.treePrototypes.Length];
            paramsWindApper = new WindScanParamsStruct[terrData.treePrototypes.Length];
            paramsWindApperStar = new WindScanParamsStruct[terrData.treePrototypes.Length];
            paramsWindSumm = new WindScanParamsStruct[terrData.treePrototypes.Length];
            paramsWind = new WindScanParamsStruct[terrData.treePrototypes.Length];
            paramsWind2 = new WindScanParamsStruct[terrData.treePrototypes.Length];


            Camera.main.transform.position = new Vector3(0,5,-15f);
            Camera.main.transform.rotation = Quaternion.identity;


            for (i = 0; i < terrData.treePrototypes.Length; i++)
            {
                LODGroup lodGroupTemp = terrData.treePrototypes[i].prefab.GetComponent<LODGroup>();
                if (lodGroupTemp != null)
                {

                    Renderer rend = lodGroupTemp.GetLODs()[0].renderers[0];


                    weirdTree[i] = Instantiate(rend.gameObject);
                    weirdTree[i].transform.position = new Vector3(0, 0, 0);
                    weirdTree[i].hideFlags = HideFlags.HideAndDontSave;

                    paramsWindZnach[i] = new WindScanParamsStruct();
                    paramsWindZnach[i].paramsAr = new float[704];
                    paramsWindZnachStar[i] = new WindScanParamsStruct();
                    paramsWindZnachStar[i].paramsAr = new float[704];
                    paramsWindApper[i] = new WindScanParamsStruct();
                    paramsWindApper[i].paramsAr = new float[704];
                    paramsWindApperStar[i] = new WindScanParamsStruct();
                    paramsWindApperStar[i].paramsAr = new float[704];
                    paramsWindSumm[i] = new WindScanParamsStruct();
                    paramsWindSumm[i].paramsAr = new float[704];
                    paramsWind[i] = new WindScanParamsStruct();
                    paramsWind[i].paramsAr = new float[704];
                    paramsWind[i].paramsAr_Up = new bool[64];
                    paramsWind2[i] = new WindScanParamsStruct();
                    paramsWind2[i].paramsAr = new float[704];
                }
            }

            timStart = Time.time;
            wind.windMain = windMainZnach;


            started = true;
        }

        void checkParamsWind()
        {
            if (schWind == 3 && idZamer < 11)
            {
                for (int i = 0; i < paramsWindZnach.Length; i++)
                {
                    for (int j = idZamer * 64; j < (idZamer + 1) * 64; j++)
                    {
                        paramsWindZnachStar[i].paramsAr[j] = paramsWindZnach[i].paramsAr[j];
                        paramsWindZnach[i].paramsAr[j] = paramsWindSumm[i].paramsAr[j] / 3f;
                    }
                }

                bool stop = false;

                for (int i = 0; i < paramsWindZnach.Length; i++)
                {
                    for (int j = idZamer * 64; j < (idZamer + 1) * 64; j++)
                    {
                        if (Mathf.Abs(Mathf.Abs(paramsWindZnach[i].paramsAr[j] - paramsWindZnachStar[i].paramsAr[j]) - paramsWind2[i].paramsAr[j]) > paramsWind2[i].paramsAr[j] * 0.05f)
                            stop = true;

                        if (stop)
                            break;
                    }

                    if (stop)
                        break;
                }

                for (int i = 0; i < paramsWindZnach.Length; i++)
                {
                    for (int j = idZamer * 64; j < (idZamer + 1) * 64; j++)
                    {
                        paramsWind2[i].paramsAr[j] = Mathf.Abs(paramsWindZnach[i].paramsAr[j] - paramsWindZnachStar[i].paramsAr[j]);
                        paramsWindSumm[i].paramsAr[j] = 0f;
                    }
                }

                if (!stop)
                    countOk++;
                else
                    countOk = 0;

                if (countOk == 2)
                {
                    for (int i = 0; i < paramsWindApperStar.Length; i++)
                    {
                        for (int j = idZamer * 64; j < (idZamer + 1) * 64; j++)
                        {
                            paramsWindApperStar[i].paramsAr[j] = paramsWindApper[i].paramsAr[j];
                        }
                    }
                }


                if (countOk == 3)
                {
                    for (int i = 0; i < paramsWindZnach.Length; i++)
                    {
                        for (int j = idZamer * 64; j < (idZamer + 1) * 64; j++)
                        {
                            if (Mathf.Abs(paramsWind2[i].paramsAr[j - (idZamer * 64)]) <= 0.1f)
                            {
                                if (idZamer == 0)
                                    paramsWind[i].paramsAr_Up[j] = false;
                                paramsWind[i].paramsAr[j] = paramsWindZnach[i].paramsAr[j];
                            }
                            else
                            {
                                if (idZamer == 0)
                                    paramsWind[i].paramsAr_Up[j] = true;
                                paramsWind[i].paramsAr[j] = (paramsWindApper[i].paramsAr[j] - paramsWindApperStar[i].paramsAr[j]) / 3f * 10f;
                            }
                        }
                    }
                    
                    idZamer++;


                    if (idZamer == 11)
                    {
                        stopped = true;
                    }
                    else
                    {
                        if (idZamer != 10)
                            windMainZnach = 1 - idZamer * 0.1f;
                        else
                            windMainZnach = 0.01f;
                        wind.windMain = windMainZnach;
                    }

                    countOk = 0;

                }

                schWind = 0;
            }
        }


        float tim = 0;


        void Update()
        {
            if (started && Time.time > tim && timStart + 4f <= Time.time && idZamer < 11)
            {
                for (int i = 0; i < terrData.treePrototypes.Length; i++)
                {
                    MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                    weirdTree[i].GetComponent<Renderer>().GetPropertyBlock(mpb);

                    paramsWindSumm[i].paramsAr[idZamer * 64 + 0] += mpb.GetVector("_ST_WindVector").x;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 1] += mpb.GetVector("_ST_WindVector").y;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 2] += mpb.GetVector("_ST_WindVector").z;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 3] += mpb.GetVector("_ST_WindVector").w;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 4] += mpb.GetVector("_ST_WindGlobal").x;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 5] += mpb.GetVector("_ST_WindGlobal").y;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 6] += mpb.GetVector("_ST_WindGlobal").z;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 7] += mpb.GetVector("_ST_WindGlobal").w;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 8] += mpb.GetVector("_ST_WindBranch").x;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 9] += mpb.GetVector("_ST_WindBranch").y;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 10] += mpb.GetVector("_ST_WindBranch").z;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 11] += mpb.GetVector("_ST_WindBranch").w;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 12] += mpb.GetVector("_ST_WindBranchTwitch").x;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 13] += mpb.GetVector("_ST_WindBranchTwitch").y;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 14] += mpb.GetVector("_ST_WindBranchTwitch").z;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 15] += mpb.GetVector("_ST_WindBranchTwitch").w;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 16] += mpb.GetVector("_ST_WindBranchWhip").x;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 17] += mpb.GetVector("_ST_WindBranchWhip").y;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 18] += mpb.GetVector("_ST_WindBranchWhip").z;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 19] += mpb.GetVector("_ST_WindBranchWhip").w;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 20] += mpb.GetVector("_ST_WindBranchAnchor").x;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 21] += mpb.GetVector("_ST_WindBranchAnchor").y;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 22] += mpb.GetVector("_ST_WindBranchAnchor").z;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 23] += mpb.GetVector("_ST_WindBranchAnchor").w;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 24] += mpb.GetVector("_ST_WindBranchAdherences").x;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 25] += mpb.GetVector("_ST_WindBranchAdherences").y;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 26] += mpb.GetVector("_ST_WindBranchAdherences").z;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 27] += mpb.GetVector("_ST_WindBranchAdherences").w;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 28] += mpb.GetVector("_ST_WindTurbulences").x;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 29] += mpb.GetVector("_ST_WindTurbulences").y;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 30] += mpb.GetVector("_ST_WindTurbulences").z;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 31] += mpb.GetVector("_ST_WindTurbulences").w;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 32] += mpb.GetVector("_ST_WindLeaf1Ripple").x;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 33] += mpb.GetVector("_ST_WindLeaf1Ripple").y;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 34] += mpb.GetVector("_ST_WindLeaf1Ripple").z;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 35] += mpb.GetVector("_ST_WindLeaf1Ripple").w;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 36] += mpb.GetVector("_ST_WindLeaf1Tumble").x;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 37] += mpb.GetVector("_ST_WindLeaf1Tumble").y;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 38] += mpb.GetVector("_ST_WindLeaf1Tumble").z;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 39] += mpb.GetVector("_ST_WindLeaf1Tumble").w;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 40] += mpb.GetVector("_ST_WindLeaf1Twitch").x;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 41] += mpb.GetVector("_ST_WindLeaf1Twitch").y;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 42] += mpb.GetVector("_ST_WindLeaf1Twitch").z;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 43] += mpb.GetVector("_ST_WindLeaf1Twitch").w;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 44] += mpb.GetVector("_ST_WindLeaf2Ripple").x;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 45] += mpb.GetVector("_ST_WindLeaf2Ripple").y;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 46] += mpb.GetVector("_ST_WindLeaf2Ripple").z;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 47] += mpb.GetVector("_ST_WindLeaf2Ripple").w;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 48] += mpb.GetVector("_ST_WindLeaf2Tumble").x;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 49] += mpb.GetVector("_ST_WindLeaf2Tumble").y;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 50] += mpb.GetVector("_ST_WindLeaf2Tumble").z;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 51] += mpb.GetVector("_ST_WindLeaf2Tumble").w;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 52] += mpb.GetVector("_ST_WindLeaf2Twitch").x;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 53] += mpb.GetVector("_ST_WindLeaf2Twitch").y;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 54] += mpb.GetVector("_ST_WindLeaf2Twitch").z;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 55] += mpb.GetVector("_ST_WindLeaf2Twitch").w;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 56] += mpb.GetVector("_ST_WindFrondRipple").x;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 57] += mpb.GetVector("_ST_WindFrondRipple").y;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 58] += mpb.GetVector("_ST_WindFrondRipple").z;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 59] += mpb.GetVector("_ST_WindFrondRipple").w;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 60] += mpb.GetVector("_ST_WindAnimation").x;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 61] += mpb.GetVector("_ST_WindAnimation").y;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 62] += mpb.GetVector("_ST_WindAnimation").z;
                    paramsWindSumm[i].paramsAr[idZamer * 64 + 63] += mpb.GetVector("_ST_WindAnimation").w;


                    paramsWindApper[i].paramsAr[idZamer * 64 + 0] = mpb.GetVector("_ST_WindVector").x;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 1] = mpb.GetVector("_ST_WindVector").y;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 2] = mpb.GetVector("_ST_WindVector").z;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 3] = mpb.GetVector("_ST_WindVector").w;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 4] = mpb.GetVector("_ST_WindGlobal").x;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 5] = mpb.GetVector("_ST_WindGlobal").y;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 6] = mpb.GetVector("_ST_WindGlobal").z;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 7] = mpb.GetVector("_ST_WindGlobal").w;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 8] = mpb.GetVector("_ST_WindBranch").x;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 9] = mpb.GetVector("_ST_WindBranch").y;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 10] = mpb.GetVector("_ST_WindBranch").z;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 11] = mpb.GetVector("_ST_WindBranch").w;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 12] = mpb.GetVector("_ST_WindBranchTwitch").x;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 13] = mpb.GetVector("_ST_WindBranchTwitch").y;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 14] = mpb.GetVector("_ST_WindBranchTwitch").z;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 15] = mpb.GetVector("_ST_WindBranchTwitch").w;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 16] = mpb.GetVector("_ST_WindBranchWhip").x;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 17] = mpb.GetVector("_ST_WindBranchWhip").y;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 18] = mpb.GetVector("_ST_WindBranchWhip").z;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 19] = mpb.GetVector("_ST_WindBranchWhip").w;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 20] = mpb.GetVector("_ST_WindBranchAnchor").x;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 21] = mpb.GetVector("_ST_WindBranchAnchor").y;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 22] = mpb.GetVector("_ST_WindBranchAnchor").z;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 23] = mpb.GetVector("_ST_WindBranchAnchor").w;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 24] = mpb.GetVector("_ST_WindBranchAdherences").x;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 25] = mpb.GetVector("_ST_WindBranchAdherences").y;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 26] = mpb.GetVector("_ST_WindBranchAdherences").z;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 27] = mpb.GetVector("_ST_WindBranchAdherences").w;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 28] = mpb.GetVector("_ST_WindTurbulences").x;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 29] = mpb.GetVector("_ST_WindTurbulences").y;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 30] = mpb.GetVector("_ST_WindTurbulences").z;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 31] = mpb.GetVector("_ST_WindTurbulences").w;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 32] = mpb.GetVector("_ST_WindLeaf1Ripple").x;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 33] = mpb.GetVector("_ST_WindLeaf1Ripple").y;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 34] = mpb.GetVector("_ST_WindLeaf1Ripple").z;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 35] = mpb.GetVector("_ST_WindLeaf1Ripple").w;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 36] = mpb.GetVector("_ST_WindLeaf1Tumble").x;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 37] = mpb.GetVector("_ST_WindLeaf1Tumble").y;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 38] = mpb.GetVector("_ST_WindLeaf1Tumble").z;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 39] = mpb.GetVector("_ST_WindLeaf1Tumble").w;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 40] = mpb.GetVector("_ST_WindLeaf1Twitch").x;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 41] = mpb.GetVector("_ST_WindLeaf1Twitch").y;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 42] = mpb.GetVector("_ST_WindLeaf1Twitch").z;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 43] = mpb.GetVector("_ST_WindLeaf1Twitch").w;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 44] = mpb.GetVector("_ST_WindLeaf2Ripple").x;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 45] = mpb.GetVector("_ST_WindLeaf2Ripple").y;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 46] = mpb.GetVector("_ST_WindLeaf2Ripple").z;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 47] = mpb.GetVector("_ST_WindLeaf2Ripple").w;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 48] = mpb.GetVector("_ST_WindLeaf2Tumble").x;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 49] = mpb.GetVector("_ST_WindLeaf2Tumble").y;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 50] = mpb.GetVector("_ST_WindLeaf2Tumble").z;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 51] = mpb.GetVector("_ST_WindLeaf2Tumble").w;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 52] = mpb.GetVector("_ST_WindLeaf2Twitch").x;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 53] = mpb.GetVector("_ST_WindLeaf2Twitch").y;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 54] = mpb.GetVector("_ST_WindLeaf2Twitch").z;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 55] = mpb.GetVector("_ST_WindLeaf2Twitch").w;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 56] = mpb.GetVector("_ST_WindFrondRipple").x;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 57] = mpb.GetVector("_ST_WindFrondRipple").y;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 58] = mpb.GetVector("_ST_WindFrondRipple").z;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 59] = mpb.GetVector("_ST_WindFrondRipple").w;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 60] = mpb.GetVector("_ST_WindAnimation").x;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 61] = mpb.GetVector("_ST_WindAnimation").y;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 62] = mpb.GetVector("_ST_WindAnimation").z;
                    paramsWindApper[i].paramsAr[idZamer * 64 + 63] = mpb.GetVector("_ST_WindAnimation").w;
                }
                
                tim = Time.time + 0.1f;

                schWind++;
                checkParamsWind();
            }
            
            if (stopped && !isEnd)
            {
                if (!Directory.Exists("AltSystems/AltTrees/SpeedTreeWindParameters"))
                    Directory.CreateDirectory("AltSystems/AltTrees/SpeedTreeWindParameters");

                for(int i = 0; i < paramsWind.Length; i++)
                {
                    using (BinaryWriter writer = new BinaryWriter(File.Open("AltSystems/AltTrees/SpeedTreeWindParameters/" + terrData.treePrototypes[i].prefab.name + ".altWSTParams", FileMode.Create)))
                    {
                        int jj = 0;
                        for (int j = 0; j < paramsWind[i].paramsAr.Length; j++)
                        {
                            if(j < 64)
                            {
                                if (float.IsNaN(paramsWind[i].paramsAr[j]))
                                    paramsWind[i].paramsAr_Up[j] = false;
                            }

                            writer.Write(paramsWind[i].paramsAr_Up[jj]);
                            jj++;
                            if (jj == 64)
                                jj = 0;


                            if (float.IsNaN(paramsWind[i].paramsAr[j]))
                                paramsWind[i].paramsAr[j] = 0;

                            writer.Write(paramsWind[i].paramsAr[j]);
                        }
                    }
                }


                isEnd = true;
            }
        }

        void OnDisable()
        {
            if(weirdTree != null)
                for (int i = 0; i < weirdTree.Length; i++)
                    Destroy(weirdTree[i]);
        }
    }


    struct WindScanParamsStruct
    {
        public Vector4 _ST_WindVector;
        public Vector4 _ST_WindGlobal;
        public Vector4 _ST_WindBranch;
        public Vector4 _ST_WindBranchTwitch;
        public Vector4 _ST_WindBranchWhip;
        public Vector4 _ST_WindBranchAnchor;
        public Vector4 _ST_WindBranchAdherences;
        public Vector4 _ST_WindTurbulences;
        public Vector4 _ST_WindLeaf1Ripple;
        public Vector4 _ST_WindLeaf1Tumble;
        public Vector4 _ST_WindLeaf1Twitch;
        public Vector4 _ST_WindLeaf2Ripple;
        public Vector4 _ST_WindLeaf2Tumble;
        public Vector4 _ST_WindLeaf2Twitch;
        public Vector4 _ST_WindFrondRipple;
        public Vector4 _ST_WindAnimation;

        public float _ST_WindVectorMagnitude;
        public float _ST_WindGlobalMagnitude;
        public float _ST_WindBranchMagnitude;
        public float _ST_WindBranchTwitchMagnitude;
        public float _ST_WindBranchWhipMagnitude;
        public float _ST_WindBranchAnchorMagnitude;
        public float _ST_WindBranchAdherencesMagnitude;
        public float _ST_WindTurbulencesMagnitude;
        public float _ST_WindLeaf1RippleMagnitude;
        public float _ST_WindLeaf1TumbleMagnitude;
        public float _ST_WindLeaf1TwitchMagnitude;
        public float _ST_WindLeaf2RippleMagnitude;
        public float _ST_WindLeaf2TumbleMagnitude;
        public float _ST_WindLeaf2TwitchMagnitude;
        public float _ST_WindFrondRippleMagnitude;
        public float _ST_WindAnimationMagnitude;

        public bool _ST_WindVector_Up;
        public bool _ST_WindGlobal_Up;
        public bool _ST_WindBranch_Up;
        public bool _ST_WindBranchTwitch_Up;
        public bool _ST_WindBranchWhip_Up;
        public bool _ST_WindBranchAnchor_Up;
        public bool _ST_WindBranchAdherences_Up;
        public bool _ST_WindTurbulences_Up;
        public bool _ST_WindLeaf1Ripple_Up;
        public bool _ST_WindLeaf1Tumble_Up;
        public bool _ST_WindLeaf1Twitch_Up;
        public bool _ST_WindLeaf2Ripple_Up;
        public bool _ST_WindLeaf2Tumble_Up;
        public bool _ST_WindLeaf2Twitch_Up;
        public bool _ST_WindFrondRipple_Up;
        public bool _ST_WindAnimation_Up;

        public float[] paramsAr;
        public bool[] paramsAr_Up;
    }
}
#endif