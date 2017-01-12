using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace AltSystems.AltTrees
{
    [ExecuteInEditMode]
    public class AltTree : MonoBehaviour
    {
        [HideInInspector]
        public int id = -1;
        [HideInInspector]
        public GameObject[] lods;
        [HideInInspector]
        public GameObject colliders;
        [HideInInspector]
        public GameObject billboardColliders;

        [System.NonSerialized]
        GameObject planeBillboard;
        [HideInInspector]
        public float[] distances;
        [HideInInspector]
        public float[] distancesSquares;
        [HideInInspector]
        public bool drawPlaneBillboard = true;
        [HideInInspector]
        public float distancePlaneBillboard = 7000f;
        [HideInInspector]
        public float distancePlaneBillboardSquare = 7000f;
        [HideInInspector]
        public bool isObject = false;
        [HideInInspector]
        public float distanceCulling = 15000f;
        [HideInInspector]
        public float distanceCullingSquare = 15000f;
        [HideInInspector]
        public Color color = new Color32(255, 255, 255, 255);
        [HideInInspector]
        public Color specularColor = new Color32(128, 128, 128, 0);
        [HideInInspector]
        public Color hueVariationLeaves = new Color32(255,0,0,80);
        [HideInInspector]
        public Color hueVariationBark = new Color32(0, 0, 0, 100);
        [HideInInspector]
        public Texture2D textureBillboard;
        [HideInInspector]
        public Material materialBillboard;
        [HideInInspector]
        public Material materialBillboardGroup;
        [HideInInspector]
        public Shader shaderAntialiasing;
        [HideInInspector]
        public Shader shaderBillboard;
        [HideInInspector]
        public Shader shaderBillboardGroup;
        [HideInInspector]
        public Shader shaderNormalsToScreen;
        [HideInInspector]
        public bool isMeshCrossFade = true;
        [HideInInspector]
        public bool isBillboardCrossFade = true;
        [HideInInspector]
        public float width = 1f;
        [HideInInspector]
        public float height = 1f;
        [HideInInspector]
        public float up = 1f;
        [HideInInspector]
        public float size = 1f;
        [HideInInspector]
        public float space = 0f;

        [HideInInspector]
        public Material[] leavesMaterials;
        [HideInInspector]
        public Material[] barkMaterials;

        [System.NonSerialized]
        public bool isColliders = false;
        [System.NonSerialized]
        public bool isBillboardColliders = false;
        [System.NonSerialized]
        public bool isCollidersEqual = false;

    
        public bool isAntialiasing = true;
        public bool isNormalmapBillboard = true;
        public int sizeTextureBillboard = 2048;
        public int sizeNormalsBillboard = 512;

        public UnityEngine.Rendering.AmbientMode ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        public Color ambientLight = Color.white;
        public float ambientIntensity = 1;
    
        #if UNITY_EDITOR
            #if UNITY_5_5 || UNITY_5_6 || UNITY_5_7
                public TextureImporterType textureImporterType = TextureImporterType.Default;
            #else
                public TextureImporterType textureImporterType = TextureImporterType.Advanced;
            #endif
        #endif
        public FilterMode filterMode = FilterMode.Point;
        public bool mipmapEnabled = false;

        #if UNITY_EDITOR
            #if UNITY_5_5 || UNITY_5_6 || UNITY_5_7
                public TextureImporterType textureImporterType_Normals = TextureImporterType.Default;
            #else
                public TextureImporterType textureImporterType_Normals = TextureImporterType.Bump;
            #endif
        #endif
        public FilterMode filterMode_Normals = FilterMode.Bilinear;
        public bool mipmapEnabled_Normals = false;
        public int anisoLevel_Normals = 16;
        public float normalsScale_Normals = 0.8f;

        [HideInInspector]
        public string folderResources = "";


        public int windMode = 0;
        [HideInInspector]
        public bool loadedConfig = false;

        public float windIntensity_TC = 1f;
        public float windBendCoefficient_TC = 1f;
        public float windTurbulenceCoefficient_TC = 1f;


        public float windIntensity_ST = 1f;
        public bool[] windParamsUp_ST;
        public float[] windParams_ST;
        float[] paramsUpper = new float[64];



        public Vector4 _ST_WindVector = new Vector4();
        public Vector4 _ST_WindGlobal = new Vector4();
        public Vector4 _ST_WindBranch = new Vector4();
        public Vector4 _ST_WindBranchTwitch = new Vector4();
        public Vector4 _ST_WindBranchWhip = new Vector4();
        public Vector4 _ST_WindBranchAnchor = new Vector4();
        public Vector4 _ST_WindBranchAdherences = new Vector4();
        public Vector4 _ST_WindTurbulences = new Vector4();
        public Vector4 _ST_WindLeaf1Ripple = new Vector4();
        public Vector4 _ST_WindLeaf1Tumble = new Vector4();
        public Vector4 _ST_WindLeaf1Twitch = new Vector4();
        public Vector4 _ST_WindLeaf2Ripple = new Vector4();
        public Vector4 _ST_WindLeaf2Tumble = new Vector4();
        public Vector4 _ST_WindLeaf2Twitch = new Vector4();
        public Vector4 _ST_WindFrondRipple = new Vector4();
        public Vector4 _ST_WindAnimation = new Vector4();
        

        float tempTime = 0;
        #if UNITY_EDITOR
            double tempTimeStar = 0;
        #endif

        int p1 = 0;
        int p2 = 0;
        float pp = 0;
        float _speedWind2 = 0;
        [System.NonSerialized]
        public bool windStar = false;

        public int version = 0;

        public void calcWindParams_ST(ref float _speedWind, ref Vector3 _directionWind)
        {
            _speedWind2 = _speedWind * windIntensity_ST;
            if (_speedWind2 < 0.01f)
                _speedWind2 = 0.01f;
            if (_speedWind2 > 1f)
                _speedWind2 = 1f;

            p1 = 10 - Mathf.CeilToInt(_speedWind2 / 0.1f);
            p2 = p1 + 1;
            pp = (1f - p1 * 0.1f - _speedWind2) * 10f;


            #if UNITY_EDITOR
            {
                if (!Application.isPlaying)
                {
                    tempTime = (float)(EditorApplication.timeSinceStartup - tempTimeStar);
                    tempTimeStar = EditorApplication.timeSinceStartup;
                }
            }
            #endif

            if (Application.isPlaying)
                tempTime = Time.deltaTime;

            _ST_WindVector.x = _directionWind.x;
            _ST_WindVector.y = _directionWind.y;
            _ST_WindVector.z = _directionWind.z;
            _ST_WindVector.w = Mathf.Lerp(windParams_ST[p1 * 64 + 3], windParams_ST[p2 * 64 + 3], pp);


            if (!windParamsUp_ST[4])
                _ST_WindGlobal.x = Mathf.Lerp(windParams_ST[p1 * 64 + 4], windParams_ST[p2 * 64 + 4], pp);
            else
                _ST_WindGlobal.x = paramsUpper[4] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 4], windParams_ST[p2 * 64 + 4], pp);
            if (!windParamsUp_ST[5])
                _ST_WindGlobal.y = Mathf.Lerp(windParams_ST[p1 * 64 + 5], windParams_ST[p2 * 64 + 5], pp);
            else
                _ST_WindGlobal.y = paramsUpper[5] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 5], windParams_ST[p2 * 64 + 5], pp);
            if (!windParamsUp_ST[6])
                _ST_WindGlobal.z = Mathf.Lerp(windParams_ST[p1 * 64 + 6], windParams_ST[p2 * 64 + 6], pp);
            else
                _ST_WindGlobal.z = paramsUpper[6] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 6], windParams_ST[p2 * 64 + 6], pp);
            if (!windParamsUp_ST[7])
                _ST_WindGlobal.w = Mathf.Lerp(windParams_ST[p1 * 64 + 7], windParams_ST[p2 * 64 + 7], pp);
            else
                _ST_WindGlobal.w = paramsUpper[7] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 7], windParams_ST[p2 * 64 + 7], pp);


            if (!windParamsUp_ST[8])
                _ST_WindBranch.x = Mathf.Lerp(windParams_ST[p1 * 64 + 8], windParams_ST[p2 * 64 + 8], pp);
            else
                _ST_WindBranch.x = paramsUpper[8] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 8], windParams_ST[p2 * 64 + 8], pp);
            if (!windParamsUp_ST[9])
                _ST_WindBranch.y = Mathf.Lerp(windParams_ST[p1 * 64 + 9], windParams_ST[p2 * 64 + 9], pp);
            else
                _ST_WindBranch.y = paramsUpper[9] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 9], windParams_ST[p2 * 64 + 9], pp);
            if (!windParamsUp_ST[10])
                _ST_WindBranch.z = Mathf.Lerp(windParams_ST[p1 * 64 + 10], windParams_ST[p2 * 64 + 10], pp);
            else
                _ST_WindBranch.z = paramsUpper[10] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 10], windParams_ST[p2 * 64 + 10], pp);
            if (!windParamsUp_ST[11])
                _ST_WindBranch.w = Mathf.Lerp(windParams_ST[p1 * 64 + 11], windParams_ST[p2 * 64 + 11], pp);
            else
                _ST_WindBranch.w = paramsUpper[11] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 11], windParams_ST[p2 * 64 + 11], pp);


            if (!windParamsUp_ST[12])
                _ST_WindBranchTwitch.x = Mathf.Lerp(windParams_ST[p1 * 64 + 12], windParams_ST[p2 * 64 + 12], pp);
            else
                _ST_WindBranchTwitch.x = paramsUpper[12] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 12], windParams_ST[p2 * 64 + 12], pp);
            if (!windParamsUp_ST[13])
                _ST_WindBranchTwitch.y = Mathf.Lerp(windParams_ST[p1 * 64 + 13], windParams_ST[p2 * 64 + 13], pp);
            else
                _ST_WindBranchTwitch.y = paramsUpper[13] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 13], windParams_ST[p2 * 64 + 13], pp);
            if (!windParamsUp_ST[14])
                _ST_WindBranchTwitch.z = Mathf.Lerp(windParams_ST[p1 * 64 + 14], windParams_ST[p2 * 64 + 14], pp);
            else
                _ST_WindBranchTwitch.z = paramsUpper[14] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 14], windParams_ST[p2 * 64 + 14], pp);
            if (!windParamsUp_ST[15])
                _ST_WindBranchTwitch.w = Mathf.Lerp(windParams_ST[p1 * 64 + 15], windParams_ST[p2 * 64 + 15], pp);
            else
                _ST_WindBranchTwitch.w = paramsUpper[15] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 15], windParams_ST[p2 * 64 + 15], pp);


            if (!windParamsUp_ST[16])
                _ST_WindBranchWhip.x = Mathf.Lerp(windParams_ST[p1 * 64 + 16], windParams_ST[p2 * 64 + 16], pp);
            else
                _ST_WindBranchWhip.x = paramsUpper[16] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 16], windParams_ST[p2 * 64 + 16], pp);
            if (!windParamsUp_ST[17])
                _ST_WindBranchWhip.y = Mathf.Lerp(windParams_ST[p1 * 64 + 17], windParams_ST[p2 * 64 + 17], pp);
            else
                _ST_WindBranchWhip.y = paramsUpper[17] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 17], windParams_ST[p2 * 64 + 17], pp);
            if (!windParamsUp_ST[18])
                _ST_WindBranchWhip.z = Mathf.Lerp(windParams_ST[p1 * 64 + 18], windParams_ST[p2 * 64 + 18], pp);
            else
                _ST_WindBranchWhip.z = paramsUpper[18] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 18], windParams_ST[p2 * 64 + 18], pp);
            if (!windParamsUp_ST[19])
                _ST_WindBranchWhip.w = Mathf.Lerp(windParams_ST[p1 * 64 + 19], windParams_ST[p2 * 64 + 19], pp);
            else
                _ST_WindBranchWhip.w = paramsUpper[19] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 19], windParams_ST[p2 * 64 + 19], pp);


            if (!windParamsUp_ST[20])
                _ST_WindBranchAnchor.x = Mathf.Lerp(windParams_ST[p1 * 64 + 20], windParams_ST[p2 * 64 + 20], pp);
            else
                _ST_WindBranchAnchor.x = paramsUpper[20] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 20], windParams_ST[p2 * 64 + 20], pp);
            if (!windParamsUp_ST[21])
                _ST_WindBranchAnchor.y = Mathf.Lerp(windParams_ST[p1 * 64 + 21], windParams_ST[p2 * 64 + 21], pp);
            else
                _ST_WindBranchAnchor.y = paramsUpper[21] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 21], windParams_ST[p2 * 64 + 21], pp);
            if (!windParamsUp_ST[22])
                _ST_WindBranchAnchor.z = Mathf.Lerp(windParams_ST[p1 * 64 + 22], windParams_ST[p2 * 64 + 22], pp);
            else
                _ST_WindBranchAnchor.z = paramsUpper[22] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 22], windParams_ST[p2 * 64 + 22], pp);
            if (!windParamsUp_ST[23])
                _ST_WindBranchAnchor.w = Mathf.Lerp(windParams_ST[p1 * 64 + 23], windParams_ST[p2 * 64 + 23], pp);
            else
                _ST_WindBranchAnchor.w = paramsUpper[23] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 23], windParams_ST[p2 * 64 + 23], pp);


            if (!windParamsUp_ST[24])
                _ST_WindBranchAdherences.x = Mathf.Lerp(windParams_ST[p1 * 64 + 24], windParams_ST[p2 * 64 + 24], pp);
            else
                _ST_WindBranchAdherences.x = paramsUpper[24] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 24], windParams_ST[p2 * 64 + 24], pp);
            if (!windParamsUp_ST[25])
                _ST_WindBranchAdherences.y = Mathf.Lerp(windParams_ST[p1 * 64 + 25], windParams_ST[p2 * 64 + 25], pp);
            else
                _ST_WindBranchAdherences.y = paramsUpper[25] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 25], windParams_ST[p2 * 64 + 25], pp);
            if (!windParamsUp_ST[26])
                _ST_WindBranchAdherences.z = Mathf.Lerp(windParams_ST[p1 * 64 + 26], windParams_ST[p2 * 64 + 26], pp);
            else
                _ST_WindBranchAdherences.z = paramsUpper[26] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 26], windParams_ST[p2 * 64 + 26], pp);
            if (!windParamsUp_ST[27])
                _ST_WindBranchAdherences.w = Mathf.Lerp(windParams_ST[p1 * 64 + 27], windParams_ST[p2 * 64 + 27], pp);
            else
                _ST_WindBranchAdherences.w = paramsUpper[27] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 27], windParams_ST[p2 * 64 + 27], pp);


            if (!windParamsUp_ST[28])
                _ST_WindTurbulences.x = Mathf.Lerp(windParams_ST[p1 * 64 + 28], windParams_ST[p2 * 64 + 28], pp);
            else
                _ST_WindTurbulences.x = paramsUpper[28] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 28], windParams_ST[p2 * 64 + 28], pp);
            if (!windParamsUp_ST[29])
                _ST_WindTurbulences.y = Mathf.Lerp(windParams_ST[p1 * 64 + 29], windParams_ST[p2 * 64 + 29], pp);
            else
                _ST_WindTurbulences.y = paramsUpper[29] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 29], windParams_ST[p2 * 64 + 29], pp);
            if (!windParamsUp_ST[30])
                _ST_WindTurbulences.z = Mathf.Lerp(windParams_ST[p1 * 64 + 30], windParams_ST[p2 * 64 + 30], pp);
            else
                _ST_WindTurbulences.z = paramsUpper[30] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 30], windParams_ST[p2 * 64 + 30], pp);
            if (!windParamsUp_ST[31])
                _ST_WindTurbulences.w = Mathf.Lerp(windParams_ST[p1 * 64 + 31], windParams_ST[p2 * 64 + 31], pp);
            else
                _ST_WindTurbulences.w = paramsUpper[31] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 31], windParams_ST[p2 * 64 + 31], pp);


            if (!windParamsUp_ST[32])
                _ST_WindLeaf1Ripple.x = Mathf.Lerp(windParams_ST[p1 * 64 + 32], windParams_ST[p2 * 64 + 32], pp);
            else
                _ST_WindLeaf1Ripple.x = paramsUpper[32] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 32], windParams_ST[p2 * 64 + 32], pp);
            if (!windParamsUp_ST[33])
                _ST_WindLeaf1Ripple.y = Mathf.Lerp(windParams_ST[p1 * 64 + 33], windParams_ST[p2 * 64 + 33], pp);
            else
                _ST_WindLeaf1Ripple.y = paramsUpper[33] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 33], windParams_ST[p2 * 64 + 33], pp);
            if (!windParamsUp_ST[34])
                _ST_WindLeaf1Ripple.z = Mathf.Lerp(windParams_ST[p1 * 64 + 34], windParams_ST[p2 * 64 + 34], pp);
            else
                _ST_WindLeaf1Ripple.z = paramsUpper[34] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 34], windParams_ST[p2 * 64 + 34], pp);
            if (!windParamsUp_ST[35])
                _ST_WindLeaf1Ripple.w = Mathf.Lerp(windParams_ST[p1 * 64 + 35], windParams_ST[p2 * 64 + 35], pp);
            else
                _ST_WindLeaf1Ripple.w = paramsUpper[35] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 35], windParams_ST[p2 * 64 + 35], pp);


            if (!windParamsUp_ST[36])
                _ST_WindLeaf1Tumble.x = Mathf.Lerp(windParams_ST[p1 * 64 + 36], windParams_ST[p2 * 64 + 36], pp);
            else
                _ST_WindLeaf1Tumble.x = paramsUpper[36] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 36], windParams_ST[p2 * 64 + 36], pp);
            if (!windParamsUp_ST[37])
                _ST_WindLeaf1Tumble.y = Mathf.Lerp(windParams_ST[p1 * 64 + 37], windParams_ST[p2 * 64 + 37], pp);
            else
                _ST_WindLeaf1Tumble.y = paramsUpper[37] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 37], windParams_ST[p2 * 64 + 37], pp);
            if (!windParamsUp_ST[38])
                _ST_WindLeaf1Tumble.z = Mathf.Lerp(windParams_ST[p1 * 64 + 38], windParams_ST[p2 * 64 + 38], pp);
            else
                _ST_WindLeaf1Tumble.z = paramsUpper[38] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 38], windParams_ST[p2 * 64 + 38], pp);
            if (!windParamsUp_ST[39])
                _ST_WindLeaf1Tumble.w = Mathf.Lerp(windParams_ST[p1 * 64 + 39], windParams_ST[p2 * 64 + 39], pp);
            else
                _ST_WindLeaf1Tumble.w = paramsUpper[39] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 39], windParams_ST[p2 * 64 + 39], pp);


            if (!windParamsUp_ST[40])
                _ST_WindLeaf1Twitch.x = Mathf.Lerp(windParams_ST[p1 * 64 + 40], windParams_ST[p2 * 64 + 40], pp);
            else
                _ST_WindLeaf1Twitch.x = paramsUpper[40] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 40], windParams_ST[p2 * 64 + 40], pp);
            if (!windParamsUp_ST[41])
                _ST_WindLeaf1Twitch.y = Mathf.Lerp(windParams_ST[p1 * 64 + 41], windParams_ST[p2 * 64 + 41], pp);
            else
                _ST_WindLeaf1Twitch.y = paramsUpper[41] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 41], windParams_ST[p2 * 64 + 41], pp);
            if (!windParamsUp_ST[42])
                _ST_WindLeaf1Twitch.z = Mathf.Lerp(windParams_ST[p1 * 64 + 42], windParams_ST[p2 * 64 + 42], pp);
            else
                _ST_WindLeaf1Twitch.z = paramsUpper[42] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 42], windParams_ST[p2 * 64 + 42], pp);
            if (!windParamsUp_ST[43])
                _ST_WindLeaf1Twitch.w = Mathf.Lerp(windParams_ST[p1 * 64 + 43], windParams_ST[p2 * 64 + 43], pp);
            else
                _ST_WindLeaf1Twitch.w = paramsUpper[43] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 43], windParams_ST[p2 * 64 + 43], pp);


            if (!windParamsUp_ST[44])
                _ST_WindLeaf2Ripple.x = Mathf.Lerp(windParams_ST[p1 * 64 + 44], windParams_ST[p2 * 64 + 44], pp);
            else
                _ST_WindLeaf2Ripple.x = paramsUpper[44] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 44], windParams_ST[p2 * 64 + 44], pp);
            if (!windParamsUp_ST[45])
                _ST_WindLeaf2Ripple.y = Mathf.Lerp(windParams_ST[p1 * 64 + 45], windParams_ST[p2 * 64 + 45], pp);
            else
                _ST_WindLeaf2Ripple.y = paramsUpper[45] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 45], windParams_ST[p2 * 64 + 45], pp);
            if (!windParamsUp_ST[46])
                _ST_WindLeaf2Ripple.z = Mathf.Lerp(windParams_ST[p1 * 64 + 46], windParams_ST[p2 * 64 + 46], pp);
            else
                _ST_WindLeaf2Ripple.z = paramsUpper[46] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 46], windParams_ST[p2 * 64 + 46], pp);
            if (!windParamsUp_ST[47])
                _ST_WindLeaf2Ripple.w = Mathf.Lerp(windParams_ST[p1 * 64 + 47], windParams_ST[p2 * 64 + 47], pp);
            else
                _ST_WindLeaf2Ripple.w = paramsUpper[47] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 47], windParams_ST[p2 * 64 + 47], pp);


            if (!windParamsUp_ST[48])
                _ST_WindLeaf2Tumble.x = Mathf.Lerp(windParams_ST[p1 * 64 + 48], windParams_ST[p2 * 64 + 48], pp);
            else
                _ST_WindLeaf2Tumble.x = paramsUpper[48] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 48], windParams_ST[p2 * 64 + 48], pp);
            if (!windParamsUp_ST[49])
                _ST_WindLeaf2Tumble.y = Mathf.Lerp(windParams_ST[p1 * 64 + 49], windParams_ST[p2 * 64 + 49], pp);
            else
                _ST_WindLeaf2Tumble.y = paramsUpper[49] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 49], windParams_ST[p2 * 64 + 49], pp);
            if (!windParamsUp_ST[50])
                _ST_WindLeaf2Tumble.z = Mathf.Lerp(windParams_ST[p1 * 64 + 50], windParams_ST[p2 * 64 + 50], pp);
            else
                _ST_WindLeaf2Tumble.z = paramsUpper[50] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 50], windParams_ST[p2 * 64 + 50], pp);
            if (!windParamsUp_ST[51])
                _ST_WindLeaf2Tumble.w = Mathf.Lerp(windParams_ST[p1 * 64 + 51], windParams_ST[p2 * 64 + 51], pp);
            else
                _ST_WindLeaf2Tumble.w = paramsUpper[51] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 51], windParams_ST[p2 * 64 + 51], pp);


            if (!windParamsUp_ST[52])
                _ST_WindLeaf2Twitch.x = Mathf.Lerp(windParams_ST[p1 * 64 + 52], windParams_ST[p2 * 64 + 52], pp);
            else
                _ST_WindLeaf2Twitch.x = paramsUpper[52] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 52], windParams_ST[p2 * 64 + 52], pp);
            if (!windParamsUp_ST[53])
                _ST_WindLeaf2Twitch.y = Mathf.Lerp(windParams_ST[p1 * 64 + 53], windParams_ST[p2 * 64 + 53], pp);
            else
                _ST_WindLeaf2Twitch.y = paramsUpper[53] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 53], windParams_ST[p2 * 64 + 53], pp);
            if (!windParamsUp_ST[54])
                _ST_WindLeaf2Twitch.z = Mathf.Lerp(windParams_ST[p1 * 64 + 54], windParams_ST[p2 * 64 + 54], pp);
            else
                _ST_WindLeaf2Twitch.z = paramsUpper[54] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 54], windParams_ST[p2 * 64 + 54], pp);
            if (!windParamsUp_ST[55])
                _ST_WindLeaf2Twitch.w = Mathf.Lerp(windParams_ST[p1 * 64 + 55], windParams_ST[p2 * 64 + 55], pp);
            else
                _ST_WindLeaf2Twitch.w = paramsUpper[55] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 55], windParams_ST[p2 * 64 + 55], pp);


            if (!windParamsUp_ST[56])
                _ST_WindFrondRipple.x = Mathf.Lerp(windParams_ST[p1 * 64 + 56], windParams_ST[p2 * 64 + 56], pp);
            else
                _ST_WindFrondRipple.x = paramsUpper[56] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 56], windParams_ST[p2 * 64 + 56], pp);
            if (!windParamsUp_ST[57])
                _ST_WindFrondRipple.y = Mathf.Lerp(windParams_ST[p1 * 64 + 57], windParams_ST[p2 * 64 + 57], pp);
            else
                _ST_WindFrondRipple.y = paramsUpper[57] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 57], windParams_ST[p2 * 64 + 57], pp);
            if (!windParamsUp_ST[58])
                _ST_WindFrondRipple.z = Mathf.Lerp(windParams_ST[p1 * 64 + 58], windParams_ST[p2 * 64 + 58], pp);
            else
                _ST_WindFrondRipple.z = paramsUpper[58] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 58], windParams_ST[p2 * 64 + 58], pp);
            if (!windParamsUp_ST[59])
                _ST_WindFrondRipple.w = Mathf.Lerp(windParams_ST[p1 * 64 + 59], windParams_ST[p2 * 64 + 59], pp);
            else
                _ST_WindFrondRipple.w = paramsUpper[59] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 59], windParams_ST[p2 * 64 + 59], pp);


            if (!windParamsUp_ST[60])
                _ST_WindAnimation.x = Mathf.Lerp(windParams_ST[p1 * 64 + 60], windParams_ST[p2 * 64 + 60], pp);
            else
                _ST_WindAnimation.x = paramsUpper[60] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 60], windParams_ST[p2 * 64 + 60], pp);
            if (!windParamsUp_ST[61])
                _ST_WindAnimation.y = Mathf.Lerp(windParams_ST[p1 * 64 + 61], windParams_ST[p2 * 64 + 61], pp);
            else
                _ST_WindAnimation.y = paramsUpper[61] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 61], windParams_ST[p2 * 64 + 61], pp);
            if (!windParamsUp_ST[62])
                _ST_WindAnimation.z = Mathf.Lerp(windParams_ST[p1 * 64 + 62], windParams_ST[p2 * 64 + 62], pp);
            else
                _ST_WindAnimation.z = paramsUpper[62] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 62], windParams_ST[p2 * 64 + 62], pp);
            if (!windParamsUp_ST[63])
                _ST_WindAnimation.w = Mathf.Lerp(windParams_ST[p1 * 64 + 63], windParams_ST[p2 * 64 + 63], pp);
            else
                _ST_WindAnimation.w = paramsUpper[63] += tempTime * Mathf.Lerp(windParams_ST[p1 * 64 + 63], windParams_ST[p2 * 64 + 63], pp);
        }

        void OnDrawGizmosSelected()
        {
            float colorZnach = 0f;
            if (lods.Length > 1)
            {
                for (int i = 0; i < distancesSquares.Length; i++)
                {
                    if (distancesSquares.Length > 1 || drawPlaneBillboard)
                        colorZnach = (510 / (drawPlaneBillboard ? distancesSquares.Length + 1 : distancesSquares.Length)) * i;
                    else
                        colorZnach = (510 / distancesSquares.Length) * i;

                    if (colorZnach <= 255)
                        Gizmos.color = new Color32(255, (byte)colorZnach, 0, 255);
                    else
                        Gizmos.color = new Color32((byte)(510 - colorZnach), 255, 0, 255);

                    Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(distancesSquares[i]));
                }
            }
            if (drawPlaneBillboard)
            {
                if(distancesSquares.Length>0)
                    colorZnach = (510 / distancesSquares.Length) * (distancesSquares.Length-1);
                else
                    colorZnach = 0;

                if (colorZnach <= 255)
                    Gizmos.color = new Color32(255, (byte)colorZnach, 0, 255);
                else
                    Gizmos.color = new Color32((byte)(510 - colorZnach), 255, 0, 255);

                Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(distancePlaneBillboardSquare));
            }
        }

        Vector3 scaleStar;

        void OnDrawGizmos()
        {
            #if UNITY_EDITOR
            {
                float dist = AltUtilities.fastDistanceSqrt(SceneView.lastActiveSceneView.camera.transform.position, gameObject.transform.position);

                if (gameObject.transform.localScale.x != gameObject.transform.localScale.z)
                    gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.x);

                if (!scaleStar.Equals(gameObject.transform.localScale))
                {
                    if (planeBillboard != null)
                    {
                        uvs2_0.x = -size * gameObject.transform.localScale.x / 2f;
                        uvs2_0.y = -size * gameObject.transform.localScale.y / 2f;
                        uvs2[0] = uvs2_0;

                        uvs2_1.x = size * gameObject.transform.localScale.x / 2f;
                        uvs2_1.y = -size * gameObject.transform.localScale.y / 2f;
                        uvs2[1] = uvs2_1;

                        uvs2_2.x = size * gameObject.transform.localScale.x / 2f;
                        uvs2_2.y = size * gameObject.transform.localScale.y / 2f;
                        uvs2[2] = uvs2_2;

                        uvs2_3.x = -size * gameObject.transform.localScale.x / 2f;
                        uvs2_3.y = size * gameObject.transform.localScale.y / 2f;
                        uvs2[3] = uvs2_3;

                        ms.uv2 = uvs2;

                        ms.RecalculateBounds();
                        Bounds bn = ms.bounds;
                        bn.max += new Vector3(size / 2f + up, size / 2f + up, size / 2f + up);
                        bn.min -= new Vector3(size / 2f - up, size / 2f - up, size / 2f - up);
                        ms.bounds = bn;
                        ms.RecalculateNormals();
                    }

                    scaleStar = gameObject.transform.localScale;
                }

                setDistance(dist);

                if (planeBillboard != null)
                {
                    planeBillboard.transform.localScale = Vector3.one;
                    planeBillboard.transform.position = gameObject.transform.position + new Vector3(0f, size * transform.localScale.y / 2f + up * transform.localScale.y, 0f);

                    if (planeBillboard.transform.rotation != gameObject.transform.rotation)
                    {
                        uvs3Vect.x = transform.rotation.eulerAngles.y;

                        uvs3[0] = uvs3Vect;
                        uvs3[1] = uvs3Vect;
                        uvs3[2] = uvs3Vect;
                        uvs3[3] = uvs3Vect;

                        ms.uv3 = uvs3;
                    }

                    planeBillboard.transform.rotation = gameObject.transform.rotation;
                    planeBillboard.transform.localScale = Vector3.one;
                }
            }
            #endif
        }

        void OnDestroy()
        {
            if (ms != null)
                DestroyImmediate(ms);
            if (planeBillboard != null)
                DestroyImmediate(planeBillboard);
        }

        void OnDisable()
        {
            if (ms != null)
                DestroyImmediate(ms);
            if (planeBillboard != null)
                DestroyImmediate(planeBillboard);
        }


        public void setDistance(float dist)
        {
            int lod = 0;
            if (distancesSquares.Length > 0)
            {
                if (dist >= distancesSquares[distancesSquares.Length - 1])
                {
                    if (drawPlaneBillboard)
                    {
                        if (dist < distancePlaneBillboardSquare)
                            lod = lods.Length - 1;
                        else
                            lod = -1;
                    }
                    else
                        lod = lods.Length - 1;
                }
                else
                {
                    for (int i = 0; i < distancesSquares.Length; i++)
                    {
                        if (dist < distancesSquares[i] || i == distancesSquares.Length - 1)
                        {
                            lod = i;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (drawPlaneBillboard)
                {
                    if (dist < distancePlaneBillboardSquare)
                        lod = lods.Length - 1;
                    else
                        lod = -1;
                }
                else
                    lod = lods.Length - 1;
            }


            for (int i = 0; i < lods.Length; i++)
            {
                if (lods[i] != null)
                {
                    if (i != lod)
                        lods[i].SetActive(false);
                    else
                        lods[i].SetActive(true);
                }
            }

            if (lod == -1)
            {
                if (planeBillboard == null)
                {
                    //Debug.Log("getPlaneBillboard", this.gameObject);
                    getPlaneBillboard();
                }
                planeBillboard.SetActive(true);
            }
            else
            {
                if (planeBillboard != null)
                    planeBillboard.SetActive(false);
            }
        }


        [System.NonSerialized]
        Mesh ms;

        Vector2[] uvs2 = new Vector2[4];
        Vector2 uvs2_0 = new Vector2(-1, -1);
        Vector2 uvs2_1 = new Vector2(1, -1);
        Vector2 uvs2_2 = new Vector2(1, 1);
        Vector2 uvs2_3 = new Vector2(-1, 1);

        Vector2 uvs3Vect = new Vector2(0, 0);
        Vector2[] uvs3 = new Vector2[4];

        void getPlaneBillboard()
        {
            if (ms != null)
                DestroyImmediate(ms);
            ms = new Mesh();
            if (planeBillboard != null)
                DestroyImmediate(planeBillboard);
        
            planeBillboard = new GameObject("planeBillboard", typeof(MeshFilter), typeof(MeshRenderer));
            planeBillboard.transform.position = gameObject.transform.position + new Vector3(0f, size * transform.localScale.y / 2f + up * transform.localScale.y, 0f);
            planeBillboard.transform.rotation = gameObject.transform.rotation;
            planeBillboard.transform.localScale = gameObject.transform.localScale;
            planeBillboard.SetActive(false);
            planeBillboard.hideFlags = HideFlags.HideAndDontSave;


            Vector3[] verts = new Vector3[4];
            Vector2[] uvs = new Vector2[4];
            Color[] cols = new Color[4];
            int[] indices = new int[6];

            Vector2 uvs_0 = new Vector2(0, 0);
            Vector2 uvs_1 = new Vector2(1f / 3f, 0);
            Vector2 uvs_2 = new Vector2(1f / 3f, 1f / 3f);
            Vector2 uvs_3 = new Vector2(0, 1f / 3f);

            verts[0] = Vector3.zero;
            verts[1] = Vector3.zero;
            verts[2] = Vector3.zero;
            verts[3] = Vector3.zero;


            uvs[0] = uvs_0;
            uvs[1] = uvs_1;
            uvs[2] = uvs_2;
            uvs[3] = uvs_3;

            cols[0] = hueVariationLeaves;
            cols[1] = hueVariationLeaves;
            cols[2] = hueVariationLeaves;
            cols[3] = hueVariationLeaves;


            uvs2_0.x = -size * gameObject.transform.localScale.x / 2f;
            uvs2_0.y = -size * gameObject.transform.localScale.y / 2f;
            uvs2[0] = uvs2_0;

            uvs2_1.x = size * gameObject.transform.localScale.x / 2f;
            uvs2_1.y = -size * gameObject.transform.localScale.y / 2f;
            uvs2[1] = uvs2_1;

            uvs2_2.x = size * gameObject.transform.localScale.x / 2f;
            uvs2_2.y = size * gameObject.transform.localScale.y / 2f;
            uvs2[2] = uvs2_2;

            uvs2_3.x = -size * gameObject.transform.localScale.x / 2f;
            uvs2_3.y = size * gameObject.transform.localScale.y / 2f;
            uvs2[3] = uvs2_3;


            uvs3Vect.x = transform.rotation.eulerAngles.y;

            uvs3[0] = uvs3Vect;
            uvs3[1] = uvs3Vect;
            uvs3[2] = uvs3Vect;
            uvs3[3] = uvs3Vect;


            indices[0] = 3;
            indices[1] = 2;
            indices[2] = 0;
            indices[3] = 2;
            indices[4] = 1;
            indices[5] = 0;



            ms.vertices = verts;
            ms.uv = uvs;
            ms.uv2 = uvs2;
            ms.uv3 = uvs3;
            ms.colors = cols;

            ms.SetIndices(indices, MeshTopology.Triangles, 0);
            ms.RecalculateBounds();
            Bounds bn = ms.bounds;
            bn.max += new Vector3(size * gameObject.transform.localScale.x / 2f, size * gameObject.transform.localScale.y / 2f, size * gameObject.transform.localScale.x / 2f);
            bn.min -= new Vector3(size * gameObject.transform.localScale.x / 2f, size * gameObject.transform.localScale.y / 2f, size * gameObject.transform.localScale.x / 2f);
            ms.bounds = bn;
            ms.RecalculateNormals();
            ms.hideFlags = HideFlags.HideAndDontSave;

            planeBillboard.GetComponent<MeshFilter>().sharedMesh = ms;

            if (materialBillboard != null)
            {
                planeBillboard.GetComponent<MeshRenderer>().sharedMaterial = new Material(materialBillboard);
                planeBillboard.GetComponent<MeshRenderer>().sharedMaterial.shaderKeywords = materialBillboard.shaderKeywords;
                planeBillboard.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Alpha", 1.0f);
                planeBillboard.GetComponent<MeshRenderer>().sharedMaterial.EnableKeyword("ONEPLANE");
            }
            else
            {
                #if UNITY_EDITOR
                {
                    planeBillboard.GetComponent<MeshRenderer>().sharedMaterial = new Material((Material)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + ".mat", typeof(Material)));
                    planeBillboard.GetComponent<MeshRenderer>().sharedMaterial.shaderKeywords = ((Material)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + ".mat", typeof(Material))).shaderKeywords;
                    planeBillboard.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Alpha", 1.0f);
                    planeBillboard.GetComponent<MeshRenderer>().sharedMaterial.EnableKeyword("ONEPLANE");
                }
                #endif
            }
        }

        public void getTextureBillboard(bool isTemp, string textProgressBar = "")
        {
            //Debug.Log("getTextureBillboard");
            
            #if UNITY_EDITOR
            {
                EditorUtility.DisplayProgressBar(textProgressBar == "" ? "Generating Texture Billboard... " : textProgressBar, "Generating Texture Billboard... ", 0.0f);
            }
            #endif

            Vector3 sdvigVector = new Vector3(-500,-500,-500);

            GameObject camTempGO = new GameObject("camTemp");
            Camera camTemp = camTempGO.AddComponent<Camera>();
            AltAntialiasing altA = null;
            if (isAntialiasing)
            {
                altA = camTempGO.AddComponent<AltAntialiasing>();
                altA.shaderFXAAIII = shaderAntialiasing;
            }

            camTempGO.transform.position = Vector3.zero + sdvigVector;
            camTempGO.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
            camTemp.orthographic = true;
            camTemp.cullingMask = 4;
            camTemp.clearFlags = CameraClearFlags.Color;
            camTemp.backgroundColor = new Color32(0, 0, 0, 0);

            UnityEngine.Rendering.AmbientMode ambientModeStar = RenderSettings.ambientMode;
            Color ambientLightStar = RenderSettings.ambientLight;
            float ambientIntensityStar = RenderSettings.ambientIntensity;



            RenderSettings.ambientMode = ambientMode;
            RenderSettings.ambientLight = ambientLight;
            RenderSettings.ambientIntensity = ambientIntensity;

            System.Collections.Generic.List<Light> lightsList = new System.Collections.Generic.List<Light>();

            Light[] lights = GameObject.FindObjectsOfType(typeof(Light)) as Light[];
            foreach (Light light in lights)
            {
                if (light.enabled)
                {
                    light.enabled = false;
                    lightsList.Add(light);
                }
            }


            transform.position = Vector3.zero;
            Renderer goTreeRenderer = lods[lods.Length - 1].GetComponent<Renderer>();
            float stepX = Mathf.Sqrt(Mathf.Pow(goTreeRenderer.bounds.size.x, 2) + Mathf.Pow(goTreeRenderer.bounds.size.z, 2));

            float sizeBounds2 = Mathf.Max(goTreeRenderer.bounds.size.y, stepX) + 0.1f;



            GameObject goTreeTempp = (GameObject)Instantiate(lods[lods.Length - 1], Vector3.zero, Quaternion.identity);
            goTreeTempp.SetActive(true);
            goTreeTempp.layer = 2;
            goTreeTempp.hideFlags = HideFlags.DontSave;
            goTreeTempp.transform.position = Vector3.zero + sdvigVector;

            Material[] mats = goTreeTempp.GetComponent<MeshRenderer>().sharedMaterials;

            for (int g = 0; g < mats.Length; g++)
            {
                string[] strs = mats[g].shaderKeywords;
                mats[g] = new Material(mats[g]);
                mats[g].shaderKeywords = strs;
                mats[g].hideFlags = HideFlags.HideAndDontSave;
                mats[g].SetColor("_HueVariation", new Color32(255, 255, 255, 0));
            }

            goTreeTempp.GetComponent<MeshRenderer>().sharedMaterials = mats;

            camTemp.orthographicSize = sizeBounds2/* / 2f*/;
            camTempGO.transform.position = new Vector3(0, -goTreeRenderer.bounds.center.y/* - goTreeRenderer.bounds.size.y / 2f*/, 100) + sdvigVector;
            camTemp.aspect = 1.0f;

            RenderTexture tempRT2;
            tempRT2 = new RenderTexture(sizeTextureBillboard, sizeTextureBillboard, 24);
            if (isAntialiasing)
                altA.enabled = false;

            int maxX = sizeTextureBillboard;
            int maxY = sizeTextureBillboard;

            for (int k = 0; k < 9; k++)
            {
                if(k<=6)
                    goTreeTempp.transform.rotation = (Quaternion.Euler(0f, 240f - k*40f, 0f));
                else
                    if (k == 7)
                        goTreeTempp.transform.rotation = Quaternion.Euler(0f, 320f, 0f);
                    else
                        if (k == 8)
                            goTreeTempp.transform.rotation = Quaternion.Euler(0f, 280f, 0f);


                goTreeTempp.transform.Rotate(new Vector3(0,-180,0));

                camTemp.targetTexture = tempRT2;


                camTemp.Render();

                RenderTexture.active = tempRT2;

                Texture2D virtualPhoto3 = new Texture2D(sizeTextureBillboard, sizeTextureBillboard, TextureFormat.ARGB32, false);
                virtualPhoto3.ReadPixels(new Rect(0, 0, sizeTextureBillboard, sizeTextureBillboard), 0, 0);
                virtualPhoto3.hideFlags = HideFlags.DontSave;

                /*byte[] bytes22;
                bytes22 = virtualPhoto3.EncodeToPNG();
                System.IO.File.WriteAllBytes(Application.dataPath + "/Plugins/AltSystems/AltTrees/DataBase/Billboards/" + folderBillboard + "/" + id + "_"+k+".png", bytes22);*/
                
                Color32[] cols3 = virtualPhoto3.GetPixels32();

                for (int i = 0; i < sizeTextureBillboard; i++)
                {
                    for (int j = 0; j < sizeTextureBillboard; j++)
                    {
                        if (!cols3[j * sizeTextureBillboard + i].Equals(new Color32(0, 0, 0, 0)))
                        {
                            if (maxX > i)
                                maxX = i;

                            i = sizeTextureBillboard;
                            break;
                        }
                    }
                }

                for (int i = sizeTextureBillboard-1; i >= 0; i--)
                {
                    for (int j = 0; j < sizeTextureBillboard; j++)
                    {
                        if (!cols3[j * sizeTextureBillboard + i].Equals(new Color32(0, 0, 0, 0)))
                        {
                            if (maxX > sizeTextureBillboard - i)
                                maxX = sizeTextureBillboard - i;
                            i = 0;
                            break;
                        }
                    }
                }

                for (int i = 0; i < sizeTextureBillboard; i++)
                {
                    for (int j = 0; j < sizeTextureBillboard; j++)
                    {
                        if (!cols3[i * sizeTextureBillboard + j].Equals(new Color32(0, 0, 0, 0)))
                        {
                            if (maxY > i)
                            {
                                maxY = i;
                            }

                            i = sizeTextureBillboard;
                            break;
                        }
                    }
                }

                for (int i = sizeTextureBillboard - 1; i >= 0; i--)
                {
                    for (int j = 0; j < sizeTextureBillboard; j++)
                    {
                        if (!cols3[i * sizeTextureBillboard + j].Equals(new Color32(0, 0, 0, 0)))
                        {
                            if (maxY > sizeTextureBillboard - i)
                                maxY = sizeTextureBillboard - i;
                            i = 0;
                            break;
                        }
                    }
                }
            }

            float sizeX = (sizeBounds2 * 2f / (float)sizeTextureBillboard) * ((float)(sizeTextureBillboard - maxX * 2));
            //float sizeY = (sizeBounds2 * 2f / (float)sizeTextureBillboard) * ((float)(sizeTextureBillboard - maxY * 2));

            float otstX = (sizeBounds2 / 2f / (float)sizeTextureBillboard) * (float)maxX * 2f;
            float otstYnizKorni = /*(sizeBounds2 - goTreeRenderer.bounds.size.y) / 2f - otstYniz*/ goTreeRenderer.bounds.size.y / 2f - goTreeRenderer.bounds.center.y;
            
            RenderTexture.active = null;
            camTemp.targetTexture = null;
            DestroyImmediate(tempRT2);





        
            height = goTreeRenderer.bounds.size.y;
            width = sizeBounds2 - otstX;

            sizeBounds2 = Mathf.Max(sizeX, goTreeRenderer.bounds.size.y + height*0.05f);
            space = (sizeBounds2 / (float)sizeTextureBillboard) * 16f;
            sizeBounds2 += space;
            float sizeBoundsWithSpace = sizeBounds2;

            otstYnizKorni += height * 0.05f;
            up = -otstYnizKorni;

            size = sizeBounds2;


            GameObject[] goTreeTemp = new GameObject[9];
            for (int i = 0; i < 9; i++)
            {
                goTreeTemp[i] = (GameObject)Instantiate(goTreeTempp, Vector3.zero, Quaternion.identity);
                goTreeTemp[i].SetActive(true);
                goTreeTemp[i].layer = 2;
                goTreeTemp[i].hideFlags = HideFlags.DontSave;
            }

            DestroyImmediate(goTreeTempp);

            goTreeTemp[0].transform.position = new Vector3(+sizeBoundsWithSpace, sizeBoundsWithSpace / 2f + otstYnizKorni, 0) + sdvigVector;
            goTreeTemp[0].transform.rotation = Quaternion.Euler(0f, 240f, 0f);
            goTreeTemp[1].transform.position = new Vector3(0, sizeBoundsWithSpace / 2f + otstYnizKorni, 0) + sdvigVector;
            goTreeTemp[1].transform.rotation = Quaternion.Euler(0f, 200f, 0f);
            goTreeTemp[2].transform.position = new Vector3(-sizeBoundsWithSpace, sizeBoundsWithSpace / 2f + otstYnizKorni, 0) + sdvigVector;
            goTreeTemp[2].transform.rotation = Quaternion.Euler(0f, 160f, 0f);

            goTreeTemp[3].transform.position = new Vector3(+sizeBoundsWithSpace, -sizeBoundsWithSpace / 2f + otstYnizKorni, 0) + sdvigVector;
            goTreeTemp[3].transform.rotation = Quaternion.Euler(0f, 120f, 0f);
            goTreeTemp[4].transform.position = new Vector3(0, -sizeBoundsWithSpace / 2f + otstYnizKorni, 0) + sdvigVector;
            goTreeTemp[4].transform.rotation = Quaternion.Euler(0f, 80f, 0f);
            goTreeTemp[5].transform.position = new Vector3(-sizeBoundsWithSpace, -sizeBoundsWithSpace / 2f + otstYnizKorni, 0) + sdvigVector;
            goTreeTemp[5].transform.rotation = Quaternion.Euler(0f, 40f, 0f);

            goTreeTemp[6].transform.position = new Vector3(+sizeBoundsWithSpace, -sizeBoundsWithSpace * 1.5f + otstYnizKorni, 0) + sdvigVector;
            goTreeTemp[6].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            goTreeTemp[7].transform.position = new Vector3(0, -sizeBoundsWithSpace * 1.5f + otstYnizKorni, 0) + sdvigVector;
            goTreeTemp[7].transform.rotation = Quaternion.Euler(0f, 320f, 0f);
            goTreeTemp[8].transform.position = new Vector3(-sizeBoundsWithSpace, -sizeBoundsWithSpace * 1.5f + otstYnizKorni, 0) + sdvigVector;
            goTreeTemp[8].transform.rotation = Quaternion.Euler(0f, 280f, 0f);

            for (int i = 0; i < 9; i++)
                goTreeTemp[i].transform.Rotate(new Vector3(0, -180, 0));



            camTemp.orthographicSize = ((sizeBoundsWithSpace) * 3f) / 2f;
            camTempGO.transform.position = new Vector3(0, 0, 100) + sdvigVector;

            camTemp.aspect = 1.0f;


            RenderTexture tempRT = new RenderTexture(sizeTextureBillboard, sizeTextureBillboard, 24);

            camTemp.targetTexture = tempRT;


            camTemp.Render();
            if (isAntialiasing)
                altA.enabled = false;

            RenderTexture.active = tempRT;
            Texture2D virtualPhoto = new Texture2D(sizeTextureBillboard, sizeTextureBillboard, TextureFormat.ARGB32, false);
            virtualPhoto.hideFlags = HideFlags.DontSave;
            virtualPhoto.ReadPixels(new Rect(0, 0, sizeTextureBillboard, sizeTextureBillboard), 0, 0);



        


            Color32[] cols = virtualPhoto.GetPixels32();
            /*Color32 colTemp = Color.white;
            bool isCol = false;

            for (int i = 0; i < sizeTextureBillboard; i++)
            {
                isCol = false;
                for (int j = 0; j < sizeTextureBillboard; j++)
                {
                    if (isCol)
                    {
                        if (cols[i * sizeTextureBillboard + j].Equals(new Color32(0, 0, 0, 0)))
                            cols[i * sizeTextureBillboard + j] = colTemp;
                        else
                        {
                            colTemp = cols[i * sizeTextureBillboard + j];
                            colTemp.a = 0;
                            isCol = true;
                        }
                    }
                    else
                    {
                        if (!cols[i * sizeTextureBillboard + j].Equals(new Color32(0, 0, 0, 0)))
                        {
                            colTemp = cols[i * sizeTextureBillboard + j];
                            colTemp.a = 0;
                            isCol = true;
                        }
                    }
                }
                isCol = false;
                for (int j = sizeTextureBillboard - 1; j >= 0; j--)
                {
                    if (isCol)
                    {
                        if (cols[i * sizeTextureBillboard + j].Equals(new Color32(0, 0, 0, 0)))
                            cols[i * sizeTextureBillboard + j] = colTemp;
                        else
                        {
                            colTemp = cols[i * sizeTextureBillboard + j];
                            colTemp.a = 0;
                            isCol = true;
                        }
                    }
                    else
                    {
                        if (!cols[i * sizeTextureBillboard + j].Equals(new Color32(0, 0, 0, 0)))
                        {
                            colTemp = cols[i * sizeTextureBillboard + j];
                            colTemp.a = 0;
                            isCol = true;
                        }
                    }
                }
            }

            isCol = false;

            for (int i = 0; i < sizeTextureBillboard; i++)
            {
                isCol = false;
                for (int j = 0; j < sizeTextureBillboard; j++)
                {
                    if (isCol)
                    {
                        if (cols[j * sizeTextureBillboard + i].Equals(new Color32(0, 0, 0, 0)))
                            cols[j * sizeTextureBillboard + i] = colTemp;
                        else
                        {
                            colTemp = cols[j * sizeTextureBillboard + i];
                            colTemp.a = 0;
                            isCol = true;
                        }
                    }
                    else
                    {
                        if (!cols[j * sizeTextureBillboard + i].Equals(new Color32(0, 0, 0, 0)))
                        {
                            colTemp = cols[j * sizeTextureBillboard + i];
                            colTemp.a = 0;
                            isCol = true;
                        }
                    }
                }
                isCol = false;
                for (int j = sizeTextureBillboard - 1; j >= 0; j--)
                {
                    if (isCol)
                    {
                        if (cols[j * sizeTextureBillboard + i].Equals(new Color32(0, 0, 0, 0)))
                            cols[j * sizeTextureBillboard + i] = colTemp;
                        else
                        {
                            colTemp = cols[j * sizeTextureBillboard + i];
                            colTemp.a = 0;
                            isCol = true;
                        }
                    }
                    else
                    {
                        if (!cols[j * sizeTextureBillboard + i].Equals(new Color32(0, 0, 0, 0)))
                        {
                            colTemp = cols[j * sizeTextureBillboard + i];
                            colTemp.a = 0;
                            isCol = true;
                        }
                    }
                }
            }*/


            #if UNITY_EDITOR
                for (int i = 0; i < 9; i++)
                {
                    MeshRenderer mr = goTreeTemp[i].GetComponent<MeshRenderer>();
                    Material[] matsTemp = new Material[mr.sharedMaterials.Length];

                    for (int j = 0; j < mr.sharedMaterials.Length; j++)
                    {
                        matsTemp[j] = new Material(mr.sharedMaterials[j]);
                        matsTemp[j].shaderKeywords = mr.sharedMaterials[j].shaderKeywords;
                        matsTemp[j].hideFlags = HideFlags.DontSave;
                        if (mr.sharedMaterials[j].shader.name == "AltTrees/SpeedTree")
                        {
                            matsTemp[j].shader = (Shader)AssetDatabase.LoadAssetAtPath("Assets/Plugins/Editor/AltSystems/AltTrees/EditorResources/SpeedTreeNormalsAltTree.shader", typeof(Shader));
                        }
                        else if (mr.sharedMaterials[j].shader.name == "AltTrees/TreeCreatorLeaves")
                        {
                            matsTemp[j].shader = (Shader)AssetDatabase.LoadAssetAtPath("Assets/Plugins/Editor/AltSystems/AltTrees/EditorResources/TreeCreatorLeavesNormalsAltTree.shader", typeof(Shader));
                        }
                        else if (mr.sharedMaterials[j].shader.name == "AltTrees/TreeCreatorBark")
                        {
                            matsTemp[j].shader = (Shader)AssetDatabase.LoadAssetAtPath("Assets/Plugins/Editor/AltSystems/AltTrees/EditorResources/TreeCreatorBarkNormalsAltTree.shader", typeof(Shader));
                        }
                        else if (mr.sharedMaterials[j].shader.name == "AltTrees/Bark")
                        {
                            matsTemp[j].shader = (Shader)AssetDatabase.LoadAssetAtPath("Assets/Plugins/Editor/AltSystems/AltTrees/EditorResources/BarkNormalsAltTree.shader", typeof(Shader));
                        }
                        else if (mr.sharedMaterials[j].shader.name == "AltTrees/Leaves")
                        {
                            matsTemp[j].shader = (Shader)AssetDatabase.LoadAssetAtPath("Assets/Plugins/Editor/AltSystems/AltTrees/EditorResources/LeavesNormalsAltTree.shader", typeof(Shader));
                        }
                        else if (mr.sharedMaterials[j].shader.name == "AltTrees/Bark Bumped")
                        {
                            matsTemp[j].shader = (Shader)AssetDatabase.LoadAssetAtPath("Assets/Plugins/Editor/AltSystems/AltTrees/EditorResources/BarkBumpNormalsAltTree.shader", typeof(Shader));
                        }
                        else if (mr.sharedMaterials[j].shader.name == "AltTrees/Leaves Bumped")
                        {
                            matsTemp[j].shader = (Shader)AssetDatabase.LoadAssetAtPath("Assets/Plugins/Editor/AltSystems/AltTrees/EditorResources/LeavesBumpNormalsAltTree.shader", typeof(Shader));
                        }
                    
                        matsTemp[j].SetFloat("_BumpScale", normalsScale_Normals);
                    }

                    mr.materials = matsTemp;
                }
            #endif

            camTemp.backgroundColor = new Color32(127, 128, 255, 255);

            if (isAntialiasing)
                altA.enabled = false;

            RenderTexture.active = null;
            camTemp.targetTexture = null;
            DestroyImmediate(tempRT);
            tempRT = new RenderTexture(sizeNormalsBillboard, sizeNormalsBillboard, 24);

            camTemp.targetTexture = tempRT;
            camTemp.Render();

            RenderTexture.active = tempRT;

            Texture2D virtualPhoto2 = new Texture2D(sizeNormalsBillboard, sizeNormalsBillboard, TextureFormat.ARGB32, false);
            virtualPhoto2.hideFlags = HideFlags.DontSave;
            virtualPhoto2.ReadPixels(new Rect(0, 0, sizeNormalsBillboard, sizeNormalsBillboard), 0, 0);











            virtualPhoto.SetPixels32(cols);
            virtualPhoto.Apply();

            textureBillboard = virtualPhoto;




            if (isTemp)
            {
                textureBillboard.hideFlags = HideFlags.HideAndDontSave;
                materialBillboard = new Material(shaderBillboard);
                materialBillboard.SetTexture("_MainTex", textureBillboard);
                materialBillboard.hideFlags = HideFlags.HideAndDontSave;
            }
            else
            {
                #if UNITY_EDITOR
                {
                    if (!System.IO.Directory.Exists("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard"))
                    {
                        System.IO.Directory.CreateDirectory("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard");
                    }

                    if(System.IO.File.Exists(Application.dataPath + "/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + ".png"))
                        System.IO.File.Delete(Application.dataPath + "/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + ".png");
                    if (isNormalmapBillboard)
                    {
                        if (System.IO.File.Exists(Application.dataPath + "/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + "_normal.png"))
                            System.IO.File.Delete(Application.dataPath + "/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + "_normal.png");
                    }
                    if(System.IO.File.Exists(Application.dataPath + "/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + ".mat"))
                        System.IO.File.Delete(Application.dataPath + "/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + ".mat");


                    byte[] bytes;
                    bytes = textureBillboard.EncodeToPNG();
                    System.IO.File.WriteAllBytes(Application.dataPath + "/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + ".png", bytes);
                    if (isNormalmapBillboard)
                    {
                        bytes = virtualPhoto2.EncodeToPNG();
                        System.IO.File.WriteAllBytes(Application.dataPath + "/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + "_normal.png", bytes);
                    }

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();


                    TextureImporter textureImporter = AssetImporter.GetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + ".png") as TextureImporter;
                    if (textureImporter != null)
                    {
                        textureImporter.textureType = textureImporterType;
                        textureImporter.filterMode = filterMode;
                        textureImporter.mipmapEnabled = mipmapEnabled;
                        textureImporter.alphaIsTransparency = true;
                        AssetDatabase.ImportAsset("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + ".png");
                    }
                    else
                        Debug.Log("TextureImporter billboard is null");

                    if (isNormalmapBillboard)
                    {
                        textureImporter = AssetImporter.GetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + "_normal.png") as TextureImporter;
                        if (textureImporter != null)
                        {
                            textureImporter.textureType = textureImporterType_Normals;
                            textureImporter.filterMode = filterMode_Normals;
                            textureImporter.convertToNormalmap = false;
                            textureImporter.anisoLevel = anisoLevel_Normals;
                            textureImporter.mipmapEnabled = mipmapEnabled_Normals;
                            AssetDatabase.ImportAsset("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + "_normal.png");
                        }
                        else
                            Debug.Log("TextureImporter billboard normal is null");
                    }

                    materialBillboard = new Material(shaderBillboard);
                    materialBillboard.SetTexture("_MainTex", (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + ".png", typeof(Texture2D)));
                    if (isNormalmapBillboard)
                        materialBillboard.SetTexture("_BumpMap", (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + "_normal.png", typeof(Texture2D)));

                    AssetDatabase.CreateAsset(materialBillboard, "Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + ".mat");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    materialBillboardGroup = new Material(shaderBillboardGroup);
                    materialBillboardGroup.SetTexture("_MainTex", (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + ".png", typeof(Texture2D)));
                    if (isNormalmapBillboard)
                        materialBillboardGroup.SetTexture("_BumpMap", (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + "_normal.png", typeof(Texture2D)));

                    AssetDatabase.CreateAsset(materialBillboardGroup, "Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + "_group.mat");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();


                    textureBillboard = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + ".png", typeof(Texture2D));
                    materialBillboard = (Material)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + ".mat", typeof(Material));
                    materialBillboardGroup = (Material)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + folderResources + "/Billboard/" + id + "_group.mat", typeof(Material));

                }
                #endif
            
            }






            Vector3 vectTemp = new Vector3(10000, -10000, 10000);

            for (int i = 0; i < goTreeTemp.Length; i++)
            {
                goTreeTemp[i].transform.position = vectTemp;
                DestroyImmediate(goTreeTemp[i]);
            }


            RenderSettings.ambientMode = ambientModeStar;
            RenderSettings.ambientLight = ambientLightStar;
            RenderSettings.ambientIntensity = ambientIntensityStar;

            for (int i = 0; i < lightsList.Count; i++)
                lightsList[i].enabled = true;




            RenderTexture.active = null;
            camTemp.targetTexture = null;
            DestroyImmediate(tempRT);
            DestroyImmediate(virtualPhoto2);
            DestroyImmediate(camTempGO);

            #if UNITY_EDITOR
            {
                EditorUtility.ClearProgressBar();
            }
            #endif
        }


        public void setSettings(Color _hueVariationLeaves, Color _hueVariationBark, Color _color, Color _specularColor, bool _isMeshCrossFade, bool _isBillboardCrossFade, bool _isObject, bool _drawPlaneBillboard, bool forceUpdateTexture = false)
        {
            drawPlaneBillboard = _drawPlaneBillboard;
            isMeshCrossFade = _isMeshCrossFade;
            isBillboardCrossFade = _isBillboardCrossFade;
            isObject = _isObject;


            if (!hueVariationLeaves.Equals(_hueVariationLeaves) || !hueVariationBark.Equals(_hueVariationBark) || !color.Equals(_color) || !specularColor.Equals(_specularColor) || forceUpdateTexture)
            {
                hueVariationLeaves = _hueVariationLeaves;
                hueVariationBark = _hueVariationBark;
                color = _color;
                specularColor = _specularColor;

                if (barkMaterials != null)
                {
                    for (int i = 0; i < barkMaterials.Length; i++)
                    {
                        barkMaterials[i].SetColor("_Color", color);
                        barkMaterials[i].SetColor("_HueVariation", hueVariationBark);
                        barkMaterials[i].SetColor("_SpecColor", specularColor);
                    }
                }
                if (leavesMaterials != null)
                {
                    for (int i = 0; i < leavesMaterials.Length; i++)
                    {
                        leavesMaterials[i].SetColor("_Color", color);
                        leavesMaterials[i].SetColor("_HueVariation", hueVariationLeaves);
                        leavesMaterials[i].SetColor("_SpecColor", specularColor);
                    }
                }

                getTextureBillboard(false);
            }


            #if UNITY_EDITOR
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            #endif

            AltTrees[] ats = FindObjectsOfType(typeof(AltTrees)) as AltTrees[];
            foreach (AltTrees at in ats)
            {
                at.reInitTimer = 10;
            }
        }

        public bool checkVersionTreeStatus()
        {
            #if UNITY_EDITOR
            {
                if(version == 0)
                {
                    return true;
                }
            }
            #endif
            return false;
        }

        public bool checkVersionTree(bool updateAltTrees = false, string textProgressBar = "", bool saveObj = true)
        {
            #if UNITY_EDITOR
            {
                if(version == 0)
                {
                    getTextureBillboard(false, textProgressBar);
                    version = 1;

                    if (saveObj)
                    {
                        EditorUtility.SetDirty(this);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                    if(updateAltTrees)
                    {
                        AltTrees[] ats = FindObjectsOfType(typeof(AltTrees)) as AltTrees[];
                        foreach (AltTrees at in ats)
                        {
                            at.reInitTimer = 10;
                        }
                    }

                    return true;
                }
            }
            #endif

            return false;
        }
    }
}





