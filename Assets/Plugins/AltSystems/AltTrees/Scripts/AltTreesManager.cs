using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace AltSystems.AltTrees
{
    [ExecuteInEditMode]
    public class AltTreesManager : MonoBehaviour
    {
        public AltTrees altTreesMain;
        bool isInit = false;
        public Transform[] cameras = new Transform[0];
        public AtiTemp[] treesCameras = new AtiTemp[0];
        public AtiTemp[] treesCamerasTemp = new AtiTemp[0];
        Transform[] camerasTemp = new Transform[0];
        static public Camera camEditor;

        AltTreesQuad[] quads = new AltTreesQuad[0];
        public AltTreesPatch[] altTrees = new AltTreesPatch[0];

        int genPerFrame;

        GameObject goCubeDebug;

        MaterialPropertyBlock propBlock;
        Color colorTemp = new Color();
        MeshRenderer meshRendererTemp;
        MeshFilter meshFilterTemp;

        public Vector3 jump = new Vector3(0, 0, 0);

	
	    [System.NonSerialized]
        public AltTreesPool[] treesPoolArray = new AltTreesPool[0];
        public int[] treePrototypeIds = new int[0];
        GameObject goTemp;
        objBillboardPool objBillboardTemp;
	    Vector3 vectTemp;
        Bounds boundsTemp;

        bool isDestroyed = false;
        public Dictionary<GameObject, AltTreesTrees> treesList = new Dictionary<GameObject, AltTreesTrees>();
        int genPerFrameTemp = 0;


        [System.NonSerialized]
        List<objBillboardPool> objBillboardsPool = new List<objBillboardPool>();
        [System.NonSerialized]
        Dictionary<GameObject, objBillboardPool> objBillboardsUsedPool = new Dictionary<GameObject, objBillboardPool>();
    

        public List<AltTreesTrees> treesCrossFade = new List<AltTreesTrees>();
        List<GameObject> rendersToRemove = new List<GameObject>();
        int rendersToRemoveCounter = 0;

        static public int HueVariationLeave_PropertyID;
        static public int HueVariationBark_PropertyID;
        static public int Alpha_PropertyID;
        static public int Ind_PropertyID;
        static public int smoothValue_PropertyID;
        static public int Color_PropertyID;

        int getPrototypeIndex(int idPrototype)
        {
            for(int i = 0; i < treePrototypeIds.Length; i++)
            {
                if (treePrototypeIds[i] == idPrototype)
                    return i;
            }

            return -1;
        }
    
        public void Init(AltTrees _altTreesMain)
        {
            if (isInit)
                return;

            propBlock = new MaterialPropertyBlock();

            HueVariationLeave_PropertyID = Shader.PropertyToID("_HueVariationLeave");
            HueVariationBark_PropertyID = Shader.PropertyToID("_HueVariationBark");
            Alpha_PropertyID = Shader.PropertyToID("_Alpha");
            Ind_PropertyID = Shader.PropertyToID("_Ind");
            smoothValue_PropertyID = Shader.PropertyToID("_smoothValue");
            Color_PropertyID = Shader.PropertyToID("_Color");

            genPerFrame = 1;
            if (!Application.isPlaying)
                genPerFrame = 10;

            altTreesMain = _altTreesMain;

            vectTemp = new Vector3(1000000, 1000000, 1000000);


            if (Application.isPlaying)
            {
                Camera[] cams = Camera.allCameras;
                if (cams != null && cams.Length > 0)
                {
                    cameras = new Transform[1];
                    cameras[0] = cams[0].transform;
                    treesCameras = new AtiTemp[1];
                }
            }
            else
            {
                if (AltTreesManager.camEditor != null)
                {
                    cameras = new Transform[1];
                    cameras[0] = AltTreesManager.camEditor.transform;
                    treesCameras = new AtiTemp[1];
                }
            }

            if (Application.isEditor)
            {
                goCubeDebug = GameObject.CreatePrimitive(PrimitiveType.Cube);
                DestroyImmediate(goCubeDebug.GetComponent<BoxCollider>());
                goCubeDebug.hideFlags = HideFlags.HideAndDontSave;
                goCubeDebug.transform.position = vectTemp;

                matTemp = new Material(goCubeDebug.GetComponent<MeshRenderer>().sharedMaterial);
                matTemp.shaderKeywords = goCubeDebug.GetComponent<MeshRenderer>().sharedMaterial.shaderKeywords;
                matTemp.hideFlags = HideFlags.DontSave;
                Shader sh = Shader.Find("Hidden/AltTrees/DebugCube");
                if (sh == null)
                {
                    matTemp.SetFloat("_Mode", 2);
                    matTemp.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    matTemp.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    matTemp.SetInt("_ZWrite", 0);
                    matTemp.DisableKeyword("_ALPHATEST_ON");
                    matTemp.EnableKeyword("_ALPHABLEND_ON");
                    matTemp.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                }
                else
                {
                    matTemp.shader = sh;
                }
                goCubeDebug.GetComponent<MeshRenderer>().sharedMaterial = matTemp;
                matTemp = null;
            }

        

            if (Application.isPlaying)
            {
                for (int j = 0; j < altTreesMain.altTreesManagerData.initBillboardCountPool; j++)
                {
                    objBillboardsPool.Add(getPlaneBillboard());
                }
            }
        
            isInit = true;
        }

	    void Update()
	    {
            /*if (Input.GetKeyDown(KeyCode.Alpha1))
                Shader.globalMaximumLOD = 1000;
            if (Input.GetKeyDown(KeyCode.Alpha2))
                Shader.globalMaximumLOD = 1001;*/
        
            if (Application.isPlaying)
		        UpdateFunk();
	    }

        int idCheck = 0;

        int p1;
        int p2;
        float pp;
        bool starStatWind = false;

        public void UpdateFunk()
	    {
            if (isDestroyed)
            {
                if (this.gameObject!=null)
                    DestroyImmediate(this.gameObject);
                return;
            }

            if (cameras.Length == 0)
            {
                if (Application.isPlaying)
                {
                    Camera[] cams = Camera.allCameras;
                    if (cams != null && cams.Length > 0)
                    {
                        cameras = new Transform[1];
                        cameras[0] = cams[0].transform;
                        treesCameras = new AtiTemp[1];
                    }
                }
                else
                {
                    if (AltTreesManager.camEditor != null)
                    {
                        cameras = new Transform[1];
                        cameras[0] = AltTreesManager.camEditor.transform;
                        treesCameras = new AtiTemp[1];
                    }
                }

                if (cameras.Length == 0)
                    return;
            }
            else
            {
                for (int c = 0; c < cameras.Length; c++)
                {
                    if (cameras[c] == null)
                    {
                        Debug.LogError("cameras[c] == null");
                        return;
                    }
                }
            }

            if (isSelectionTree)
            {
                for (int i = 0; i < cameras.Length; i++)
                {
                    altTreeInstanceTemp = cameras[i].GetComponent<AltTreeInstance>();
                    if (altTreeInstanceTemp != null)
                    {
                        cameras[i].localRotation = Quaternion.Euler(0f, cameras[i].localRotation.eulerAngles.y, 0f);

                        if (!cameras[i].localScale.Equals(altTreeInstanceTemp.scaleTempStar))
                        {
                            altTreeInstanceTemp.scaleTempStar.y = cameras[i].localScale.y;

                            if (cameras[i].localScale.x != altTreeInstanceTemp.scaleTempStar.x)
                            {
                                altTreeInstanceTemp.scaleTempStar.x = cameras[i].localScale.x;
                                altTreeInstanceTemp.scaleTempStar.z = cameras[i].localScale.x;
                            }
                            else
                            {
                                altTreeInstanceTemp.scaleTempStar.x = cameras[i].localScale.z;
                                altTreeInstanceTemp.scaleTempStar.z = cameras[i].localScale.z;
                            }

                            cameras[i].localScale = altTreeInstanceTemp.scaleTempStar;
                        }

                        if (!altTreeInstanceTemp.hueLeave.Equals(altTreeInstanceTemp.hueLeaveStar) || !altTreeInstanceTemp.hueBark.Equals(altTreeInstanceTemp.hueBarkStar))
                        {
                            propBlock.Clear();
                            propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, altTreeInstanceTemp.hueLeave);
                            propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, altTreeInstanceTemp.hueBark);
                            propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, Mathf.Clamp(tempFloatNext / (float)altTreesMain.altTreesManagerData.crossFadeTimeBillboard, 0f, 1.0f));
                            propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                            propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 0.0f);
                            cameras[i].GetComponent<MeshRenderer>().SetPropertyBlock(propBlock);
                        
                            altTreeInstanceTemp.hueLeaveStar = altTreeInstanceTemp.hueLeave;
                            altTreeInstanceTemp.hueBarkStar = altTreeInstanceTemp.hueBark;
                        }
                    }
                }
                altTreeInstanceTemp = null;
            }

            if (rendersToRemove.Count != 0)
            {
                rendersToRemoveCounter++;
                if (rendersToRemoveCounter == 2 || Application.isPlaying)
                {
                    for (int i = 0; i < rendersToRemove.Count; i++)
                    {
                        if (Application.isPlaying)
                            Destroy(rendersToRemove[i]);
                        else
                            DestroyImmediate(rendersToRemove[i]);
                    }
                    rendersToRemove.Clear();
                    rendersToRemoveCounter = 0;
                }
            }
            
            _speedWind = AltWind.getSpeed();
            _directionWind = AltWind.getDirection();
            

            if (_speedWind >= 0.01f)
            {
                if (!starStatWind)
                {
                    for (int i = 0; i < treePrototypeIds.Length; i++)
                    {
                        if (treesPoolArray[i].tree.windMode != 0 && ((treesPoolArray[i].tree.windMode == 2 && treesPoolArray[i].tree.windIntensity_TC != 0f) || (treesPoolArray[i].tree.windMode == 1 && treesPoolArray[i].tree.loadedConfig && treesPoolArray[i].tree.windIntensity_ST != 0f)))
                        {
                            for (int j = 0; j < treesPoolArray[i].objsArray.Length; j++)
                            {
                                for (int k = 0; k < treesPoolArray[i].objsArray[j].materialsMesh.Length; k++)
                                {
                                    treesPoolArray[i].objsArray[j].materialsMesh[k].EnableKeyword("ENABLE_ALTWIND");
                                }
                                for (int k = 0; k < treesPoolArray[i].objsArray[j].materialsMeshCrossFade.Length; k++)
                                {
                                    treesPoolArray[i].objsArray[j].materialsMeshCrossFade[k].EnableKeyword("ENABLE_ALTWIND");
                                }
                            }
                        }
                    }

                    starStatWind = true;
                }
                
                for (int i = 0; i < treePrototypeIds.Length; i++)
                {
                    if (!treesPoolArray[i].tree.windStar && treesPoolArray[i].tree.windMode != 0 && ((treesPoolArray[i].tree.windMode == 2 && treesPoolArray[i].tree.windIntensity_TC != 0f) || (treesPoolArray[i].tree.windMode == 1 && treesPoolArray[i].tree.loadedConfig && treesPoolArray[i].tree.windIntensity_ST != 0f)))
                    {
                        for (int j = 0; j < treesPoolArray[i].objsArray.Length; j++)
                        {
                            for (int k = 0; k < treesPoolArray[i].objsArray[j].materialsMesh.Length; k++)
                            {
                                treesPoolArray[i].objsArray[j].materialsMesh[k].EnableKeyword("ENABLE_ALTWIND");
                            }
                            for (int k = 0; k < treesPoolArray[i].objsArray[j].materialsMeshCrossFade.Length; k++)
                            {
                                treesPoolArray[i].objsArray[j].materialsMeshCrossFade[k].EnableKeyword("ENABLE_ALTWIND");
                            }
                        }
                        treesPoolArray[i].tree.windStar = true;
                    }

                    if (treesPoolArray[i].tree.windMode == 1 && treesPoolArray[i].tree.loadedConfig)
                    {
                        if (treesPoolArray[i].tree.windIntensity_ST != 0f)
                        {
                            treesPoolArray[i].tree.calcWindParams_ST(ref _speedWind, ref _directionWind);

                            for (int j = 0; j < treesPoolArray[i].objsArray.Length; j++)
                            {
                                for (int k = 0; k < treesPoolArray[i].objsArray[j].materialsMesh.Length; k++)
                                {
                                    treesPoolArray[i].objsArray[j].materialsMesh[k].SetVector("_ST_WindVector", treesPoolArray[i].tree._ST_WindVector);
                                    treesPoolArray[i].objsArray[j].materialsMesh[k].SetVector("_ST_WindGlobal", treesPoolArray[i].tree._ST_WindGlobal);
                                    treesPoolArray[i].objsArray[j].materialsMesh[k].SetVector("_ST_WindBranch", treesPoolArray[i].tree._ST_WindBranch);
                                    treesPoolArray[i].objsArray[j].materialsMesh[k].SetVector("_ST_WindBranchTwitch", treesPoolArray[i].tree._ST_WindBranchTwitch);
                                    treesPoolArray[i].objsArray[j].materialsMesh[k].SetVector("_ST_WindBranchWhip", treesPoolArray[i].tree._ST_WindBranchWhip);
                                    treesPoolArray[i].objsArray[j].materialsMesh[k].SetVector("_ST_WindBranchAnchor", treesPoolArray[i].tree._ST_WindBranchAnchor);
                                    treesPoolArray[i].objsArray[j].materialsMesh[k].SetVector("_ST_WindBranchAdherences", treesPoolArray[i].tree._ST_WindBranchAdherences);
                                    treesPoolArray[i].objsArray[j].materialsMesh[k].SetVector("_ST_WindTurbulences", treesPoolArray[i].tree._ST_WindTurbulences);
                                    treesPoolArray[i].objsArray[j].materialsMesh[k].SetVector("_ST_WindLeaf1Ripple", treesPoolArray[i].tree._ST_WindLeaf1Ripple);
                                    treesPoolArray[i].objsArray[j].materialsMesh[k].SetVector("_ST_WindLeaf1Tumble", treesPoolArray[i].tree._ST_WindLeaf1Tumble);
                                    treesPoolArray[i].objsArray[j].materialsMesh[k].SetVector("_ST_WindLeaf1Twitch", treesPoolArray[i].tree._ST_WindLeaf1Twitch);
                                    treesPoolArray[i].objsArray[j].materialsMesh[k].SetVector("_ST_WindLeaf2Ripple", treesPoolArray[i].tree._ST_WindLeaf2Ripple);
                                    treesPoolArray[i].objsArray[j].materialsMesh[k].SetVector("_ST_WindLeaf2Tumble", treesPoolArray[i].tree._ST_WindLeaf2Tumble);
                                    treesPoolArray[i].objsArray[j].materialsMesh[k].SetVector("_ST_WindLeaf2Twitch", treesPoolArray[i].tree._ST_WindLeaf2Twitch);
                                    treesPoolArray[i].objsArray[j].materialsMesh[k].SetVector("_ST_WindFrondRipple", treesPoolArray[i].tree._ST_WindFrondRipple);
                                    treesPoolArray[i].objsArray[j].materialsMesh[k].SetVector("_ST_WindAnimation", treesPoolArray[i].tree._ST_WindAnimation);
                                }
                                for (int k = 0; k < treesPoolArray[i].objsArray[j].materialsMeshCrossFade.Length; k++)
                                {
                                    treesPoolArray[i].objsArray[j].materialsMeshCrossFade[k].SetVector("_ST_WindVector", treesPoolArray[i].tree._ST_WindVector);
                                    treesPoolArray[i].objsArray[j].materialsMeshCrossFade[k].SetVector("_ST_WindGlobal", treesPoolArray[i].tree._ST_WindGlobal);
                                    treesPoolArray[i].objsArray[j].materialsMeshCrossFade[k].SetVector("_ST_WindBranch", treesPoolArray[i].tree._ST_WindBranch);
                                    treesPoolArray[i].objsArray[j].materialsMeshCrossFade[k].SetVector("_ST_WindBranchTwitch", treesPoolArray[i].tree._ST_WindBranchTwitch);
                                    treesPoolArray[i].objsArray[j].materialsMeshCrossFade[k].SetVector("_ST_WindBranchWhip", treesPoolArray[i].tree._ST_WindBranchWhip);
                                    treesPoolArray[i].objsArray[j].materialsMeshCrossFade[k].SetVector("_ST_WindBranchAnchor", treesPoolArray[i].tree._ST_WindBranchAnchor);
                                    treesPoolArray[i].objsArray[j].materialsMeshCrossFade[k].SetVector("_ST_WindBranchAdherences", treesPoolArray[i].tree._ST_WindBranchAdherences);
                                    treesPoolArray[i].objsArray[j].materialsMeshCrossFade[k].SetVector("_ST_WindTurbulences", treesPoolArray[i].tree._ST_WindTurbulences);
                                    treesPoolArray[i].objsArray[j].materialsMeshCrossFade[k].SetVector("_ST_WindLeaf1Ripple", treesPoolArray[i].tree._ST_WindLeaf1Ripple);
                                    treesPoolArray[i].objsArray[j].materialsMeshCrossFade[k].SetVector("_ST_WindLeaf1Tumble", treesPoolArray[i].tree._ST_WindLeaf1Tumble);
                                    treesPoolArray[i].objsArray[j].materialsMeshCrossFade[k].SetVector("_ST_WindLeaf1Twitch", treesPoolArray[i].tree._ST_WindLeaf1Twitch);
                                    treesPoolArray[i].objsArray[j].materialsMeshCrossFade[k].SetVector("_ST_WindLeaf2Ripple", treesPoolArray[i].tree._ST_WindLeaf2Ripple);
                                    treesPoolArray[i].objsArray[j].materialsMeshCrossFade[k].SetVector("_ST_WindLeaf2Tumble", treesPoolArray[i].tree._ST_WindLeaf2Tumble);
                                    treesPoolArray[i].objsArray[j].materialsMeshCrossFade[k].SetVector("_ST_WindLeaf2Twitch", treesPoolArray[i].tree._ST_WindLeaf2Twitch);
                                    treesPoolArray[i].objsArray[j].materialsMeshCrossFade[k].SetVector("_ST_WindFrondRipple", treesPoolArray[i].tree._ST_WindFrondRipple);
                                    treesPoolArray[i].objsArray[j].materialsMeshCrossFade[k].SetVector("_ST_WindAnimation", treesPoolArray[i].tree._ST_WindAnimation);
                                }
                            }
                        }
                    }
                    else if (treesPoolArray[i].tree.windMode == 2)
                    {
                        _directionWind2 = _directionWind * _speedWind * treesPoolArray[i].tree.windBendCoefficient_TC * treesPoolArray[i].tree.windIntensity_TC;
                        for (int j = 0; j < treesPoolArray[i].objsArray.Length; j++)
                        {
                            for (int k = 0; k < treesPoolArray[i].objsArray[j].materialsMesh.Length; k++)
                            {
                                treesPoolArray[i].objsArray[j].materialsMesh[k].SetVector("_WindAltTree", new Vector4(_directionWind2.x, _directionWind2.y, _directionWind2.z , _speedWind * treesPoolArray[i].tree.windIntensity_TC * treesPoolArray[i].tree.windTurbulenceCoefficient_TC));
                            }
                            for (int k = 0; k < treesPoolArray[i].objsArray[j].materialsMeshCrossFade.Length; k++)
                            {
                                treesPoolArray[i].objsArray[j].materialsMeshCrossFade[k].SetVector("_WindAltTree", new Vector4(_directionWind2.x, _directionWind2.y, _directionWind2.z, _speedWind * treesPoolArray[i].tree.windIntensity_TC * treesPoolArray[i].tree.windTurbulenceCoefficient_TC));
                            }
                        }
                    }
                }
            }
            else
            {
                if (starStatWind)
                {
                    for (int i = 0; i < treePrototypeIds.Length; i++)
                    {
                        for (int j = 0; j < treesPoolArray[i].objsArray.Length; j++)
                        {
                            for (int k = 0; k < treesPoolArray[i].objsArray[j].materialsMesh.Length; k++)
                            {
                                treesPoolArray[i].objsArray[j].materialsMesh[k].DisableKeyword("ENABLE_ALTWIND");
                            }
                            for (int k = 0; k < treesPoolArray[i].objsArray[j].materialsMeshCrossFade.Length; k++)
                            {
                                treesPoolArray[i].objsArray[j].materialsMeshCrossFade[k].DisableKeyword("ENABLE_ALTWIND");
                            }
                        }
                        treesPoolArray[i].tree.windStar = false;
                    }

                    starStatWind = false;
                }
            }


            checkCrossFade();
        
            if (idCheck >= quads.Length)
                idCheck = 0;

            if (quads.Length != 0 && quads[idCheck] != null)
            {
                quads[idCheck].check(cameras, true, true);
                quads[idCheck].checkObjs(null, altTrees[idCheck], cameras);
            }
            idCheck++;


            if (altTreesMain.altTreesManagerData.drawDebugPutches != altTreesMain.altTreesManagerData.drawDebugPutchesStar)
            {
                for (int i = 0; i < altTrees.Length; i++)
                {
                    if (altTrees[i] != null)
                    {
                        if (quads[i] != null)
                            quads[i].checkDebugPutches(altTreesMain.altTreesManagerData.drawDebugPutches);
                    }
                }
                altTreesMain.altTreesManagerData.drawDebugPutchesStar = altTreesMain.altTreesManagerData.drawDebugPutches;
            }



            if (AltTreesQuad.objsToInit.Count > 0)
		    {

                genPerFrameTemp = 0;
                while (AltTreesQuad.objsToInit.Count > 0 && genPerFrame > genPerFrameTemp)
                {
                    genPerFrameTemp++;
                    if (altTrees != null && altTrees.Length > AltTreesQuad.objsToInit[0].altTreesID && altTrees[AltTreesQuad.objsToInit[0].altTreesID] != null && altTrees[AltTreesQuad.objsToInit[0].altTreesID].draw)
                    {
                        if (Application.isEditor && AltTreesQuad.objsToInit[0].rendersDebug == null)
                        {
                            GameObject go = Instantiate(goCubeDebug);
                            go.transform.position = new Vector3(AltTreesQuad.objsToInit[0].pos.x, 1, AltTreesQuad.objsToInit[0].pos.y);
                            go.transform.localScale = new Vector3(AltTreesQuad.objsToInit[0].size, 4, AltTreesQuad.objsToInit[0].size);
                            propBlock.Clear();
                            colorTemp.r = Random.value;
                            colorTemp.g = Random.value;
                            colorTemp.b = Random.value;
                            colorTemp.a = 0.3f;
                            propBlock.SetColor(AltTreesManager.Color_PropertyID, colorTemp);
                            go.transform.parent = this.transform;
                            go.name = "debug_" + AltTreesQuad.objsToInit[0].LOD;
                            go.hideFlags = HideFlags.DontSave;
                            go.GetComponent<MeshRenderer>().SetPropertyBlock(propBlock);
                            go.SetActive(false);
                            AltTreesQuad.objsToInit[0].rendersDebug = go;
                            if (altTrees[AltTreesQuad.objsToInit[0].altTreesID].rendersDebug==null)
                                altTrees[AltTreesQuad.objsToInit[0].altTreesID].rendersDebug = new List<GameObject>();
                            altTrees[AltTreesQuad.objsToInit[0].altTreesID].rendersDebug.Add(go);
                        }
                    
                        if (AltTreesQuad.objsToInit[0].treesCount != 0)
                        {
                            if (AltTreesQuad.objsToInit[0].LOD <= AltTreesQuad.objsToInit[0].startBillboardsLOD || AltTreesQuad.objsToInit[0].isGenerateAllBillboardsOnStart)
                            {
                                createMeshBillboards();
                            }
                            else
                            {
                                if (AltTreesQuad.objsToInit[0].renders.Count != 0)
                                {
                                    for (int i = 0; i < AltTreesQuad.objsToInit[0].renders.Count; i++)
                                    {
                                        if (Application.isPlaying)
                                            Destroy(AltTreesQuad.objsToInit[0].renders[i]);
                                        else
                                            DestroyImmediate(AltTreesQuad.objsToInit[0].renders[i]);
                                    }
                                    AltTreesQuad.objsToInit[0].renders.Clear();
                                }

                                AltTreesQuad.objsToInit[0].isInit = true;
                            }
                        }
                        else
                        {
                            if (AltTreesQuad.objsToInit[0].renders.Count != 0)
                            {
                                for (int i = 0; i < AltTreesQuad.objsToInit[0].renders.Count; i++)
                                {
                                    if (Application.isPlaying)
                                        Destroy(AltTreesQuad.objsToInit[0].renders[i]);
                                    else
                                        DestroyImmediate(AltTreesQuad.objsToInit[0].renders[i]);
                                }
                                AltTreesQuad.objsToInit[0].renders.Clear();
                            }

                            AltTreesQuad.objsToInit[0].isInit = true;
                        }
                    }

                    AltTreesQuad.objsToInit.RemoveAt(0);
                }
		    }
	    }

        float _speedWind = 0;
        Vector3 _directionWind = new Vector3();
        Vector3 _directionWind2 = new Vector3();

        float tempFloatNext = 0f;

        void checkCrossFade()
        {
            for (int i = treesCrossFade.Count - 1; i >= 0 ; i--)
            {
                if (Application.isPlaying)
                    tempFloatNext = treesCrossFade[i].crossFadeTime - Time.time;
                else
                {
                    #if UNITY_EDITOR
                    {
                        tempFloatNext = treesCrossFade[i].crossFadeTime - (float)EditorApplication.timeSinceStartup;
                    }
                    #endif
                }

                if (tempFloatNext <= 0f)
                {
                    if (treesCrossFade[i].currentCrossFadeId == 1)
                    {
                        treesCrossFade[i].crossFadeTreeMeshRenderer.sharedMaterials = treesPoolArray[treesCrossFade[i].idPrototypeIndex].objsArray[treesCrossFade[i].currentLOD].materialsMesh;
                        propBlock.Clear();
                        propBlock.SetColor(HueVariationLeave_PropertyID, treesCrossFade[i].color);
                        propBlock.SetColor(HueVariationBark_PropertyID, treesCrossFade[i].colorBark);
                        propBlock.SetFloat(Alpha_PropertyID, 1.0f);
                        propBlock.SetFloat(Ind_PropertyID, 0.0f); 
                        propBlock.SetFloat(smoothValue_PropertyID, 0.0f);
                        treesCrossFade[i].crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);

                        treesCrossFade[i].go.GetComponent<AltTreeInstance>().isCrossFade = false;
                        delObjBillboardPool(treesCrossFade[i].idPrototype, treesCrossFade[i].goCrossFade);
                        treesCrossFade[i].goCrossFade = null;
                    }
                    else if (treesCrossFade[i].currentCrossFadeId == 6)
                    {
                        treesCrossFade[i].crossFadeTreeMeshRenderer.sharedMaterials = treesPoolArray[treesCrossFade[i].idPrototypeIndex].objsArray[treesCrossFade[i].currentLOD].materialsMesh;
                        propBlock.Clear();
                        propBlock.SetColor(HueVariationLeave_PropertyID, treesCrossFade[i].color);
                        propBlock.SetColor(HueVariationBark_PropertyID, treesCrossFade[i].colorBark);
                        propBlock.SetFloat(Alpha_PropertyID, 1.0f);
                        propBlock.SetFloat(Ind_PropertyID, 0.0f);
                        propBlock.SetFloat(smoothValue_PropertyID, 0.0f);
                        treesCrossFade[i].crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);

                        treesCrossFade[i].go.GetComponent<AltTreeInstance>().isCrossFade = false;
                    }
                    else if (treesCrossFade[i].currentCrossFadeId == 7)
                    {
                        delObjBillboardPool(treesCrossFade[i].idPrototype, treesCrossFade[i].goCrossFade);
                        treesCrossFade[i].goCrossFade = null;
                    }
                    else if (treesCrossFade[i].currentCrossFadeId == 2)
                    {
                        treesCrossFade[i].crossFadeTreeMeshRenderer.sharedMaterials = treesPoolArray[treesCrossFade[i].idPrototypeIndex].objsArray[treesCrossFade[i].currentCrossFadeLOD].materialsMesh;
                        propBlock.Clear();
                        propBlock.SetColor(HueVariationLeave_PropertyID, treesCrossFade[i].color);
                        propBlock.SetColor(HueVariationBark_PropertyID, treesCrossFade[i].colorBark);
                        propBlock.SetFloat(Alpha_PropertyID, 1.0f);
                        propBlock.SetFloat(Ind_PropertyID, 0.0f);
                        propBlock.SetFloat(smoothValue_PropertyID, 0.0f);
                        treesCrossFade[i].crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);

                        treesCrossFade[i].goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = false;
                        delObjPool(treesCrossFade[i].idPrototype, treesCrossFade[i].currentCrossFadeLOD, treesCrossFade[i].goCrossFade);

                        treesCrossFade[i].crossFadeBillboardMeshRenderer.sharedMaterial = treesPoolArray[treesCrossFade[i].idPrototypeIndex].materialBillboard;
                        propBlock.Clear();
                        treesCrossFade[i].crossFadeBillboardMeshRenderer.GetPropertyBlock(propBlock);
                        propBlock.SetFloat(Alpha_PropertyID, 1.0f);
                        treesCrossFade[i].crossFadeBillboardMeshRenderer.SetPropertyBlock(propBlock);

                        treesCrossFade[i].goCrossFade = null;
                    }
                    else if (treesCrossFade[i].currentCrossFadeId == 5)
                    {
                        treesCrossFade[i].crossFadeTreeMeshRenderer.sharedMaterials = treesPoolArray[treesCrossFade[i].idPrototypeIndex].objsArray[treesCrossFade[i].currentCrossFadeLOD].materialsMesh;
                        propBlock.Clear();
                        propBlock.SetColor(HueVariationLeave_PropertyID, treesCrossFade[i].color);
                        propBlock.SetColor(HueVariationBark_PropertyID, treesCrossFade[i].colorBark);
                        propBlock.SetFloat(Alpha_PropertyID, 1.0f);
                        propBlock.SetFloat(Ind_PropertyID, 0.0f);
                        propBlock.SetFloat(smoothValue_PropertyID, 0.0f);
                        treesCrossFade[i].crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);

                        treesCrossFade[i].goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = false;
                        delObjPool(treesCrossFade[i].idPrototype, treesCrossFade[i].currentCrossFadeLOD, treesCrossFade[i].goCrossFade);

                        treesCrossFade[i].goCrossFade = null;
                    }
                    else if (treesCrossFade[i].currentCrossFadeId == 8)
                    {
                        treesCrossFade[i].crossFadeBillboardMeshRenderer.sharedMaterial = treesPoolArray[treesCrossFade[i].idPrototypeIndex].materialBillboard;
                        propBlock.Clear();
                        treesCrossFade[i].crossFadeBillboardMeshRenderer.GetPropertyBlock(propBlock);
                        propBlock.SetFloat(Alpha_PropertyID, 1.0f);
                        treesCrossFade[i].crossFadeBillboardMeshRenderer.SetPropertyBlock(propBlock);

                        treesCrossFade[i].goCrossFade = null;
                    }
                    else if (treesCrossFade[i].currentCrossFadeId == 3)
                    {

                        treesCrossFade[i].go.SetActive(true);
                        treesCrossFade[i].goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = false;
                        propBlock.Clear();
                        propBlock.SetColor(HueVariationLeave_PropertyID, treesCrossFade[i].color);
                        propBlock.SetColor(HueVariationBark_PropertyID, treesCrossFade[i].colorBark);
                        propBlock.SetFloat(Alpha_PropertyID, 1.0f);
                        propBlock.SetFloat(Ind_PropertyID, 0.0f);
                        propBlock.SetFloat(smoothValue_PropertyID, 0.0f);
                        treesCrossFade[i].crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);

                        treesCrossFade[i].goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = false;
                        delObjPool(treesCrossFade[i].idPrototype, treesCrossFade[i].currentCrossFadeLOD, treesCrossFade[i].goCrossFade);
                        treesCrossFade[i].goCrossFade = null;
                    }
                    else if (treesCrossFade[i].currentCrossFadeId == 4)
                    {
                        treesCrossFade[i].go = treesCrossFade[i].goCrossFade;
                        treesCrossFade[i].go.GetComponent<AltTreeInstance>().isCrossFade = false;
                        treesCrossFade[i].goCrossFade = null;
                    }

                    treesCrossFade[i].crossFadeBillboardMeshRenderer = null;
                    treesCrossFade[i].crossFadeTreeMeshRenderer = null;
                    treesCrossFade[i].currentCrossFadeId = -1;

                    treesCrossFade.RemoveAt(i);
                }
                else
                {
                    if (((treesCrossFade[i].currentCrossFadeId == 2 || treesCrossFade[i].currentCrossFadeId == 1) /*&& treesCrossFade[i].crossFadeBillboardMeshRenderer != null*/) || (treesCrossFade[i].currentCrossFadeId == 3 || treesCrossFade[i].currentCrossFadeId == 4) || (treesCrossFade[i].currentCrossFadeId == 5 || treesCrossFade[i].currentCrossFadeId == 7 || treesCrossFade[i].currentCrossFadeId == 6 || treesCrossFade[i].currentCrossFadeId == 8))
                    {
                        propBlock.Clear();
                        if (treesCrossFade[i].currentCrossFadeId == 2)
                        {
                            treesCrossFade[i].crossFadeBillboardMeshRenderer.GetPropertyBlock(propBlock);
                            propBlock.SetFloat(Alpha_PropertyID, Mathf.Clamp(1f - tempFloatNext / (float)altTreesMain.altTreesManagerData.crossFadeTimeBillboard, 0f, 1.0f));
                            treesCrossFade[i].crossFadeBillboardMeshRenderer.SetPropertyBlock(propBlock);

                            propBlock.Clear();
                            propBlock.SetColor(HueVariationLeave_PropertyID, treesCrossFade[i].color);
                            propBlock.SetColor(HueVariationBark_PropertyID, treesCrossFade[i].colorBark);
                            propBlock.SetFloat(Alpha_PropertyID, Mathf.Clamp(tempFloatNext / (float)altTreesMain.altTreesManagerData.crossFadeTimeBillboard, 0f, 1.0f));
                            propBlock.SetFloat(Ind_PropertyID, 0.0f);
                            propBlock.SetFloat(smoothValue_PropertyID, 0.0f);
                            treesCrossFade[i].crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);
                        }
                        else if (treesCrossFade[i].currentCrossFadeId == 5)
                        {
                            propBlock.SetColor(HueVariationLeave_PropertyID, treesCrossFade[i].color);
                            propBlock.SetColor(HueVariationBark_PropertyID, treesCrossFade[i].colorBark);
                            propBlock.SetFloat(Alpha_PropertyID, Mathf.Clamp(tempFloatNext / (float)altTreesMain.altTreesManagerData.crossFadeTimeBillboard, 0f, 1.0f));
                            propBlock.SetFloat(Ind_PropertyID, 0.0f);
                            propBlock.SetFloat(smoothValue_PropertyID, 0.0f);
                            treesCrossFade[i].crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);
                        }
                        else if (treesCrossFade[i].currentCrossFadeId == 8)
                        {
                            treesCrossFade[i].crossFadeBillboardMeshRenderer.GetPropertyBlock(propBlock);
                            propBlock.SetFloat(Alpha_PropertyID, Mathf.Clamp(1f - tempFloatNext / (float)altTreesMain.altTreesManagerData.crossFadeTimeBillboard, 0f, 1.0f));
                            treesCrossFade[i].crossFadeBillboardMeshRenderer.SetPropertyBlock(propBlock);
                        }
                        else if (treesCrossFade[i].currentCrossFadeId == 1)
                        {
                            treesCrossFade[i].crossFadeBillboardMeshRenderer.GetPropertyBlock(propBlock);
                            propBlock.SetFloat(Alpha_PropertyID, Mathf.Clamp(tempFloatNext / (float)altTreesMain.altTreesManagerData.crossFadeTimeBillboard, 0f, 1.0f));
                            treesCrossFade[i].crossFadeBillboardMeshRenderer.SetPropertyBlock(propBlock);

                            propBlock.Clear();
                            propBlock.SetColor(HueVariationLeave_PropertyID, treesCrossFade[i].color);
                            propBlock.SetColor(HueVariationBark_PropertyID, treesCrossFade[i].colorBark);
                            propBlock.SetFloat(Alpha_PropertyID, Mathf.Clamp(1f - tempFloatNext / (float)altTreesMain.altTreesManagerData.crossFadeTimeBillboard, 0f, 1.0f));
                            propBlock.SetFloat(Ind_PropertyID, 0.0f);
                            propBlock.SetFloat(smoothValue_PropertyID, 0.0f);
                            treesCrossFade[i].crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);        //err
                        }
                        else if (treesCrossFade[i].currentCrossFadeId == 6)
                        {
                            propBlock.SetColor(HueVariationLeave_PropertyID, treesCrossFade[i].color);
                            propBlock.SetColor(HueVariationBark_PropertyID, treesCrossFade[i].colorBark);
                            propBlock.SetFloat(Alpha_PropertyID, Mathf.Clamp(1f - tempFloatNext / (float)altTreesMain.altTreesManagerData.crossFadeTimeBillboard, 0f, 1.0f));
                            propBlock.SetFloat(Ind_PropertyID, 0.0f);
                            propBlock.SetFloat(smoothValue_PropertyID, 0.0f);
                            treesCrossFade[i].crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);
                        }
                        else if (treesCrossFade[i].currentCrossFadeId == 7)
                        {
                            treesCrossFade[i].crossFadeBillboardMeshRenderer.GetPropertyBlock(propBlock);
                            propBlock.SetFloat(Alpha_PropertyID, Mathf.Clamp(tempFloatNext / (float)altTreesMain.altTreesManagerData.crossFadeTimeBillboard, 0f, 1.0f));
                            treesCrossFade[i].crossFadeBillboardMeshRenderer.SetPropertyBlock(propBlock);
                        }
                        else if (treesCrossFade[i].currentCrossFadeId == 3)
                        {
                            propBlock.SetColor(HueVariationLeave_PropertyID, treesCrossFade[i].color);
                            propBlock.SetColor(HueVariationBark_PropertyID, treesCrossFade[i].colorBark);
                            propBlock.SetFloat(Alpha_PropertyID, 1.0f);
                            propBlock.SetFloat(Ind_PropertyID, 0.0f);
                            propBlock.SetFloat(smoothValue_PropertyID, Mathf.Clamp(1f - tempFloatNext / (float)altTreesMain.altTreesManagerData.crossFadeTimeMesh, 0f, 1.0f));
                            treesCrossFade[i].crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);
                        }
                        else if (treesCrossFade[i].currentCrossFadeId == 4)
                        {
                            propBlock.SetColor(HueVariationLeave_PropertyID, treesCrossFade[i].color);
                            propBlock.SetColor(HueVariationBark_PropertyID, treesCrossFade[i].colorBark);
                            propBlock.SetFloat(Alpha_PropertyID, 1.0f);
                            propBlock.SetFloat(Ind_PropertyID, 0.0f);
                            propBlock.SetFloat(smoothValue_PropertyID, Mathf.Clamp(tempFloatNext / (float)altTreesMain.altTreesManagerData.crossFadeTimeMesh, 0f, 1.0f));
                            treesCrossFade[i].crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);
                        }
                    }
                }



            }
        }

        public void addAltTrees(AltTreesPatch _altTrees, bool isNew = true)
        {
            if (_altTrees == null)
                return;

            bool isStop = false;
            int idNum = -1;
            if (isNew)
            {
                for (int i = 0; i < altTrees.Length; i++)
                {
                    if (altTrees[i] != null && altTrees[i].Equals(_altTrees))
                    {
                        isStop = true;
                        break;
                    }
                    else
                    {
                        if (altTrees[i] == null && idNum == -1)
                        {
                            idNum = i;
                        }
                    }
                }

                if (isStop)
                {
                    Debug.LogError("Already contains AltTrees.");
                    return;
                }

                if (idNum == -1)
                {
                    AltTreesPatch[] altTreesTemp = altTrees;
                    altTrees = new AltTreesPatch[altTrees.Length + 1];

                    for (int i = 0; i < altTreesTemp.Length; i++)
                    {
                        altTrees[i] = altTreesTemp[i];
                    }
                    altTrees[altTrees.Length - 1] = _altTrees;


                    AltTreesQuad[] quadsTemp = quads;
                    quads = new AltTreesQuad[quads.Length + 1];

                    for (int i = 0; i < quadsTemp.Length; i++)
                    {
                        quads[i] = quadsTemp[i];
                    }

                    idNum = altTrees.Length - 1;
                }
                else
                {
                    altTrees[idNum] = _altTrees;
                }
            }
            else
            {
                for (int i = 0; i < altTrees.Length; i++)
                {
                    if (altTrees[i] != null && altTrees[i].Equals(_altTrees))
                    {
                        idNum = i;
                        break;
                    }
                }

                if (idNum == -1)
                {
                    Debug.LogError("No contains AltTrees.");
                    return;
                }
            }

            _altTrees.altTreesId = idNum;

            createQuadTree(idNum);

            if (isNew)
            {
                if (_altTrees.prototypes != null)
                {
                    int isUpdate = 0;
                    for (int i = 0; i < _altTrees.prototypes.Length; i++)
                    {
                        if (_altTrees.prototypes[i].tree != null)
                        {
                            _altTrees.prototypes[i].isEnabled = true;
                            if (_altTrees.prototypes[i].tree.textureBillboard == null || _altTrees.prototypes[i].tree.materialBillboard == null || _altTrees.prototypes[i].tree.materialBillboardGroup == null)
                            {
                                #if UNITY_EDITOR
                                {
                                    if ((Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + _altTrees.prototypes[i].tree.folderResources + "/Billboard/" + _altTrees.prototypes[i].tree.id + ".png", typeof(Texture2D)) == null || (Material)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + _altTrees.prototypes[i].tree.folderResources + "/Billboard/" + _altTrees.prototypes[i].tree.id + ".mat", typeof(Material)) == null || (Material)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + _altTrees.prototypes[i].tree.folderResources + "/Billboard/" + _altTrees.prototypes[i].tree.id + "_group.mat", typeof(Material)) == null)
                                    {
                                        _altTrees.prototypes[i].tree.getTextureBillboard(false);
                                    }
                                    else
                                    {
                                        _altTrees.prototypes[i].tree.textureBillboard = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + _altTrees.prototypes[i].tree.folderResources + "/Billboard/" + _altTrees.prototypes[i].tree.id + ".png", typeof(Texture2D));
                                        _altTrees.prototypes[i].tree.materialBillboard = (Material)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + _altTrees.prototypes[i].tree.folderResources + "/Billboard/" + _altTrees.prototypes[i].tree.id + ".mat", typeof(Material));
                                        _altTrees.prototypes[i].tree.materialBillboardGroup = (Material)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + _altTrees.prototypes[i].tree.folderResources + "/Billboard/" + _altTrees.prototypes[i].tree.id + "_group.mat", typeof(Material));
                                    }

                                    EditorUtility.SetDirty(_altTrees.prototypes[i].tree);

                                    //if (!_altTrees.prototypes[i].tree.generateTextureBillboardRuntime)
                                    //Debug.LogError("No Billboard for " + _altTrees.prototypes[i].tree.name + ". Press Update Billboard button on tree!");
                                }
                                #else
                                {
                                    _altTrees.prototypes[i].tree.getTextureBillboard(true);
                                }
                                #endif
                            }


                            if (_altTrees.prototypes[i].tree.lods.Length != _altTrees.prototypes[i].tree.distancesSquares.Length + 1)
                                Debug.LogError("distances.Length+1 != lods.Length. id" + _altTrees.prototypes[i].tree.id);

                            addInitObjPool(_altTrees.prototypes[i].tree);
                        }
                        else
                        {
                            _altTrees.prototypes[i].isEnabled = false;
                            isUpdate++;
                        }
                    }
                    if(isUpdate > 0)
                    {
                        AltTreePrototypes[] prototypesTemp = _altTrees.prototypes;
                        _altTrees.prototypes = new AltTreePrototypes[prototypesTemp.Length - isUpdate];
                        int indx = 0;
                        for(int i = 0; i < prototypesTemp.Length; i++)
                        {
                            if(prototypesTemp[i].isEnabled)
                            {
                                _altTrees.prototypes[indx] = prototypesTemp[i];
                                _altTrees.prototypes[indx].isEnabled = true;
                                indx++;
                            }
                        }
                        #if UNITY_EDITOR
                            EditorUtility.SetDirty(_altTrees.altTreesManagerData);
                        #endif
                    }
                }
            }

            List<int> treesForDelete = new List<int>();
            List<int> objectsForDelete = new List<int>();

            if (_altTrees.trees != null)
            {
                for (int i = 0; i < _altTrees.trees.Length; i++)
                {
                    if (_altTrees.trees[i] != null && _altTrees.trees[i].noNull)
                    {
                        _altTrees.trees[i].idPrototypeIndex = getPrototypeIndex(_altTrees.trees[i].idPrototype);

                        if (_altTrees.trees[i].idPrototypeIndex != -1)
                        {
                            _altTrees.trees[i].currentLOD = -1;
                            _altTrees.trees[i].currentCrossFadeId = -1;
                            quads[idNum].checkTreesAdd(_altTrees.trees[i].getPosWorld().x, _altTrees.trees[i].getPosWorld().z, _altTrees.trees[i]);
                        }
                        else
                        {
                            _altTrees.trees[i].noNull = false;
                            treesForDelete.Add(i);
                        }
                    }
                }
                if(treesForDelete.Count > 0)
                {
                    _altTrees.EditDataFile(false, null, 0, treesForDelete);
                }
            }
            if (_altTrees.treesNoGroup != null)
            {
                for (int i = 0; i < _altTrees.treesNoGroup.Length; i++)
                {
                    if (_altTrees.treesNoGroup[i] != null && _altTrees.treesNoGroup[i].noNull)
                    {
                        _altTrees.treesNoGroup[i].idPrototypeIndex = getPrototypeIndex(_altTrees.treesNoGroup[i].idPrototype);

                        if (_altTrees.treesNoGroup[i].idPrototypeIndex != -1)
                        {
                            _altTrees.treesNoGroup[i].currentLOD = -1;
                            _altTrees.treesNoGroup[i].currentCrossFadeId = -1;
                            quads[idNum].checkTreesAdd(_altTrees.treesNoGroup[i].getPosWorld().x, _altTrees.treesNoGroup[i].getPosWorld().z, _altTrees.treesNoGroup[i], false);
                        }
                        else
                        {
                            _altTrees.treesNoGroup[i].noNull = false;
                            objectsForDelete.Add(i);
                        }
                    }
                }
                if (objectsForDelete.Count > 0)
                {
                    _altTrees.EditDataFile(true, null, 0, objectsForDelete);
                }
            }
        }



        void createQuadTree(int id)
        {
            quads[id] = new AltTreesQuad((altTrees[id].stepX - jump.x) * altTreesMain.altTreesManagerData.sizePatch + (float)altTreesMain.altTreesManagerData.sizePatch / 2f, (altTrees[id].stepY - jump.z) * altTreesMain.altTreesManagerData.sizePatch + (float)altTreesMain.altTreesManagerData.sizePatch / 2f, altTreesMain.altTreesManagerData.sizePatch, id, 0, altTreesMain.altTreesManagerData.maxLOD, altTreesMain.altTreesManagerData.maxLOD - 1, this);
        }

        AltTreeInstance altTreeInstanceTemp = null;
        Renderer rendererTemp;
        Material[] matsTemp;
        Material matTemp;

        public void addInitObjPool(AltTree go)
        {
            int indexTemp = getPrototypeIndex(go.id);
            if (indexTemp == -1)
            {
                int[] treePrototypeIdsTemp = treePrototypeIds;
                treePrototypeIds = new int[treePrototypeIdsTemp.Length + 1];
                for (int i = 0; i < treePrototypeIdsTemp.Length; i++)
                    treePrototypeIds[i] = treePrototypeIdsTemp[i];
                treePrototypeIds[treePrototypeIds.Length - 1] = go.id;
                indexTemp = treePrototypeIds.Length - 1;
                

                AltTreesPool[] treesPoolArrayTemp = treesPoolArray;
                treesPoolArray = new AltTreesPool[treesPoolArray.Length + 1];
                for (int i = 0; i < treesPoolArrayTemp.Length; i++)
                    treesPoolArray[i] = treesPoolArrayTemp[i];
                treesPoolArray[indexTemp] = new AltTreesPool();

                treesPoolArray[indexTemp].tree = go;

                if (Application.isPlaying && altTreesMain.altTreesManagerData.enableColliders)
                {
                    if (go.colliders != null)
                    {
                        go.isColliders = true;

                        if (go.colliders.Equals(go.billboardColliders))
                            go.isCollidersEqual = true;
                    }

                    if (go.billboardColliders != null)
                        go.isBillboardColliders = true;


                    if (go.isColliders)
                    {
                        for (int j = 0; j < (go.isCollidersEqual ? altTreesMain.altTreesManagerData.initCollidersCountPool + altTreesMain.altTreesManagerData.initBillboardCountPool : altTreesMain.altTreesManagerData.initCollidersCountPool); j++)
                        {
                            treesPoolArray[indexTemp].collidersArray.Add(Instantiate(go.colliders, vectTemp, Quaternion.identity) as GameObject);
                            treesPoolArray[indexTemp].collidersArray[treesPoolArray[indexTemp].collidersArray.Count - 1].SetActive(false);
                            treesPoolArray[indexTemp].collidersArray[treesPoolArray[indexTemp].collidersArray.Count - 1].transform.parent = transform;
                            treesPoolArray[indexTemp].collidersArray[treesPoolArray[indexTemp].collidersArray.Count - 1].hideFlags = HideFlags.DontSave;
                        }
                    }
                    if (go.isBillboardColliders && !go.isCollidersEqual)
                    {
                        for (int j = 0; j < altTreesMain.altTreesManagerData.initColliderBillboardsCountPool; j++)
                        {
                            treesPoolArray[indexTemp].colliderBillboardsArray.Add(Instantiate(go.billboardColliders, vectTemp, Quaternion.identity) as GameObject);
                            treesPoolArray[indexTemp].colliderBillboardsArray[treesPoolArray[indexTemp].colliderBillboardsArray.Count - 1].SetActive(false);
                            treesPoolArray[indexTemp].colliderBillboardsArray[treesPoolArray[indexTemp].colliderBillboardsArray.Count - 1].transform.parent = transform;
                            treesPoolArray[indexTemp].colliderBillboardsArray[treesPoolArray[indexTemp].colliderBillboardsArray.Count - 1].hideFlags = HideFlags.DontSave;
                        }
                    }
                }

                treesPoolArray[indexTemp].objsArray = new objsArr[go.lods.Length];

                for (int i = 0; i < go.lods.Length; i++)
                {
                    treesPoolArray[indexTemp].objsArray[i] = new objsArr();
                    rendererTemp = go.lods[i].GetComponent<MeshRenderer>();

                    treesPoolArray[indexTemp].objsArray[i].materialsMesh = new Material[rendererTemp.sharedMaterials.Length];
                    for (int j = 0; j < rendererTemp.sharedMaterials.Length; j++)
                    {
                        treesPoolArray[indexTemp].objsArray[i].materialsMesh[j] = new Material(rendererTemp.sharedMaterials[j]);
                        treesPoolArray[indexTemp].objsArray[i].materialsMesh[j].shaderKeywords = rendererTemp.sharedMaterials[j].shaderKeywords;
                        treesPoolArray[indexTemp].objsArray[i].materialsMesh[j].hideFlags = HideFlags.HideAndDontSave;
                        if (treesPoolArray[indexTemp].objsArray[i].materialsMesh[j].HasProperty("_Cutoff"))
                            treesPoolArray[indexTemp].objsArray[i].materialsMesh[j].SetFloat("_Cutoff", treesPoolArray[indexTemp].objsArray[i].materialsMesh[j].GetFloat("_Cutoff"));

                        if (starStatWind && treesPoolArray[indexTemp].tree.windMode != 0 && ((treesPoolArray[indexTemp].tree.windMode == 2 && treesPoolArray[indexTemp].tree.windIntensity_TC != 0f) || (treesPoolArray[indexTemp].tree.windMode == 1 && treesPoolArray[indexTemp].tree.loadedConfig && treesPoolArray[indexTemp].tree.windIntensity_ST != 0f)))
                        {
                            treesPoolArray[indexTemp].objsArray[i].materialsMesh[j].EnableKeyword("ENABLE_ALTWIND");
                            treesPoolArray[indexTemp].tree.windStar = true;
                        }
                        else
                            treesPoolArray[indexTemp].objsArray[i].materialsMesh[j].DisableKeyword("ENABLE_ALTWIND");


                        if (treesPoolArray[indexTemp].tree.leavesMaterials != null)
                        {
                            for (int f = 0; f < treesPoolArray[indexTemp].tree.leavesMaterials.Length; f++)
                            {
                                if (treesPoolArray[indexTemp].tree.leavesMaterials[f].Equals(rendererTemp.sharedMaterials[j]))
                                {
                                    treesPoolArray[indexTemp].objsArray[i].materialsMesh[j].SetInt("_Type", 0);
                                    f = treesPoolArray[indexTemp].tree.leavesMaterials.Length;
                                }
                            }
                        }
                        if (treesPoolArray[indexTemp].tree.barkMaterials != null)
                        {
                            for (int f = 0; f < treesPoolArray[indexTemp].tree.barkMaterials.Length; f++)
                            {
                                if (treesPoolArray[indexTemp].tree.barkMaterials[f].Equals(rendererTemp.sharedMaterials[j]))
                                {
                                    treesPoolArray[indexTemp].objsArray[i].materialsMesh[j].SetInt("_Type", 1);
                                    f = treesPoolArray[indexTemp].tree.barkMaterials.Length;
                                }
                            }
                        }
                    }

                    treesPoolArray[indexTemp].objsArray[i].materialsMeshCrossFade = new Material[rendererTemp.sharedMaterials.Length];
                    for (int j = 0; j < rendererTemp.sharedMaterials.Length; j++)
                    {
                        treesPoolArray[indexTemp].objsArray[i].materialsMeshCrossFade[j] = new Material(treesPoolArray[indexTemp].objsArray[i].materialsMesh[j]);
                        treesPoolArray[indexTemp].objsArray[i].materialsMeshCrossFade[j].shaderKeywords = treesPoolArray[indexTemp].objsArray[i].materialsMesh[j].shaderKeywords;
                        treesPoolArray[indexTemp].objsArray[i].materialsMeshCrossFade[j].hideFlags = HideFlags.HideAndDontSave;
                        treesPoolArray[indexTemp].objsArray[i].materialsMeshCrossFade[j].EnableKeyword("CROSSFADE");
                        if (starStatWind && treesPoolArray[indexTemp].tree.windMode != 0 && ((treesPoolArray[indexTemp].tree.windMode == 2 && treesPoolArray[indexTemp].tree.windIntensity_TC != 0f) || (treesPoolArray[indexTemp].tree.windMode == 1 && treesPoolArray[indexTemp].tree.loadedConfig && treesPoolArray[indexTemp].tree.windIntensity_ST != 0f)))
                        {
                            treesPoolArray[indexTemp].objsArray[i].materialsMeshCrossFade[j].EnableKeyword("ENABLE_ALTWIND");
                            treesPoolArray[indexTemp].tree.windStar = true;
                        }
                        else
                            treesPoolArray[indexTemp].objsArray[i].materialsMeshCrossFade[j].DisableKeyword("ENABLE_ALTWIND");
                    }
                    rendererTemp = null;

                    if (Application.isPlaying)
                    {
                        for (int j = 0; j < altTreesMain.altTreesManagerData.initObjsCountPool; j++)
                        {
                            treesPoolArray[indexTemp].objsArray[i].objs.Add(Instantiate(go.lods[i], vectTemp, Quaternion.identity) as GameObject);
                            treesPoolArray[indexTemp].objsArray[i].objs[treesPoolArray[indexTemp].objsArray[i].objs.Count - 1].SetActive(false);
                            treesPoolArray[indexTemp].objsArray[i].objs[treesPoolArray[indexTemp].objsArray[i].objs.Count - 1].transform.parent = transform;
                            altTreeInstanceTemp = treesPoolArray[indexTemp].objsArray[i].objs[treesPoolArray[indexTemp].objsArray[i].objs.Count - 1].AddComponent<AltTreeInstance>();
                            altTreeInstanceTemp.manager = this;
                            altTreeInstanceTemp.isObject = go.isObject;

                            rendererTemp = treesPoolArray[indexTemp].objsArray[i].objs[treesPoolArray[indexTemp].objsArray[i].objs.Count - 1].GetComponent<MeshRenderer>();
                            rendererTemp.sharedMaterials = treesPoolArray[indexTemp].objsArray[i].materialsMesh;

                            treesPoolArray[indexTemp].objsArray[i].objs[treesPoolArray[indexTemp].objsArray[i].objs.Count - 1].hideFlags = HideFlags.DontSave;
                        }
                    }
                }

                if (treesPoolArray[indexTemp].tree.materialBillboard != null)
                {
                    treesPoolArray[indexTemp].materialBillboard = new Material(treesPoolArray[indexTemp].tree.materialBillboard);
                    treesPoolArray[indexTemp].materialBillboard.shaderKeywords = treesPoolArray[indexTemp].tree.materialBillboard.shaderKeywords;
                    treesPoolArray[indexTemp].materialBillboard.hideFlags = HideFlags.DontSave;
                }
                else
                {
                    #if UNITY_EDITOR
                    {
                        treesPoolArray[indexTemp].materialBillboard = new Material((Material)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + treesPoolArray[indexTemp].tree.folderResources + "/Billboard/" + treesPoolArray[indexTemp].tree.id + ".mat", typeof(Material)));
                        treesPoolArray[indexTemp].materialBillboard.shaderKeywords = ((Material)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + treesPoolArray[indexTemp].tree.folderResources + "/Billboard/" + treesPoolArray[indexTemp].tree.id + ".mat", typeof(Material))).shaderKeywords;
                        treesPoolArray[indexTemp].materialBillboard.hideFlags = HideFlags.DontSave;
                    }
                    #endif
                }

                if (altTreesMain.altTreesManagerData.drawDebugBillboards)
                    treesPoolArray[indexTemp].materialBillboard.EnableKeyword("DEBUG_ON");

                treesPoolArray[indexTemp].materialBillboardCrossFade = new Material(treesPoolArray[indexTemp].materialBillboard);
                treesPoolArray[indexTemp].materialBillboardCrossFade.shaderKeywords = treesPoolArray[indexTemp].materialBillboard.shaderKeywords;
                treesPoolArray[indexTemp].materialBillboardCrossFade.EnableKeyword("CROSSFADE");
                treesPoolArray[indexTemp].materialBillboardCrossFade.hideFlags = HideFlags.DontSave;



                if (treesPoolArray[indexTemp].tree.materialBillboardGroup != null)
                {
                    treesPoolArray[indexTemp].materialBillboardGroup = new Material(treesPoolArray[indexTemp].tree.materialBillboardGroup);
                    treesPoolArray[indexTemp].materialBillboardGroup.shaderKeywords = treesPoolArray[indexTemp].tree.materialBillboardGroup.shaderKeywords;
                    treesPoolArray[indexTemp].materialBillboardGroup.hideFlags = HideFlags.DontSave;
                }
                else
                {
                    #if UNITY_EDITOR
                    {
                        if ((Material)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + treesPoolArray[indexTemp].tree.folderResources + "/Billboard/" + treesPoolArray[indexTemp].tree.id + "_group.mat", typeof(Material)) == null)
                            treesPoolArray[indexTemp].tree.getTextureBillboard(false);

                        treesPoolArray[indexTemp].materialBillboardGroup = new Material((Material)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + treesPoolArray[indexTemp].tree.folderResources + "/Billboard/" + treesPoolArray[indexTemp].tree.id + "_group.mat", typeof(Material)));
                        treesPoolArray[indexTemp].materialBillboardGroup.shaderKeywords = ((Material)AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + treesPoolArray[indexTemp].tree.folderResources + "/Billboard/" + treesPoolArray[indexTemp].tree.id + "_group.mat", typeof(Material))).shaderKeywords;
                        treesPoolArray[indexTemp].materialBillboardGroup.hideFlags = HideFlags.DontSave;
                    }
                    #endif
                }
                if (altTreesMain.altTreesManagerData.drawDebugBillboards)
                    treesPoolArray[indexTemp].materialBillboardGroup.EnableKeyword("DEBUG_ON");

                matTemp = null;
                System.GC.Collect();
            }
        }

        public GameObject getObjPool(int idPrefab, int lod, int altTreesID, int idTree, Color _colorLeave, Color _colorBark)
        {
            int indexTemp = getPrototypeIndex(idPrefab);

            if (Application.isPlaying && treesPoolArray[indexTemp].objsArray[lod].objs.Count > 0)
            {
                goTemp = treesPoolArray[indexTemp].objsArray[lod].objs[0];
                treesPoolArray[indexTemp].objsArray[lod].objs.RemoveAt(0);
                if (goTemp != null)
                {
                    altTreeInstanceTemp = goTemp.GetComponent<AltTreeInstance>();
                    altTreeInstanceTemp.idTree = idTree;
                    altTreeInstanceTemp.altTreesId = altTreesID;
                    altTreeInstanceTemp.hueLeave = _colorLeave;
                    altTreeInstanceTemp.hueLeaveStar = _colorLeave;
                    altTreeInstanceTemp.hueBark = _colorBark;
                    altTreeInstanceTemp.hueBarkStar = _colorBark;

                    rendererTemp = goTemp.GetComponent<MeshRenderer>();
                    rendererTemp.sharedMaterials = treesPoolArray[indexTemp].objsArray[lod].materialsMesh;

                    propBlock.Clear();
                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1.0f);
                    propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                    propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 0.0f);
                    propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, _colorLeave);
                    propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, _colorBark);
                    rendererTemp.SetPropertyBlock(propBlock);

                    goTemp.SetActive(true);
                    altTreeInstanceTemp = null;
                    rendererTemp = null;

                    return goTemp;
                }
                else
                {
                    altTreesMain.LogWarning("goTemp==null. Instantiate object pool. [" + treesPoolArray[indexTemp].tree.name + "]");
                    goTemp = null;
                    goTemp = Instantiate(treesPoolArray[indexTemp].tree.lods[lod], vectTemp, Quaternion.identity) as GameObject;
                    altTreeInstanceTemp = goTemp.AddComponent<AltTreeInstance>();
                    altTreeInstanceTemp.isObject = treesPoolArray[indexTemp].tree.isObject;
                    altTreeInstanceTemp.manager = this;
                    altTreeInstanceTemp.idTree = idTree;
                    altTreeInstanceTemp.altTreesId = altTreesID;
                    altTreeInstanceTemp.hueLeave = _colorLeave;
                    altTreeInstanceTemp.hueLeaveStar = _colorLeave;
                    altTreeInstanceTemp.hueBark = _colorBark;
                    altTreeInstanceTemp.hueBarkStar = _colorBark;

                    rendererTemp = goTemp.GetComponent<MeshRenderer>();
                    rendererTemp.sharedMaterials = treesPoolArray[indexTemp].objsArray[lod].materialsMesh;

                    propBlock.Clear();
                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1.0f);
                    propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                    propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 0.0f);
                    propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, _colorLeave);
                    propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, _colorBark);
                    rendererTemp.SetPropertyBlock(propBlock);

                    goTemp.transform.parent = transform;
                    goTemp.hideFlags = HideFlags.DontSave;
                    altTreeInstanceTemp = null;
                    goTemp.SetActive(true);
                    rendererTemp = null;

                    return goTemp;
                }
            }
            else
            {
                if (Application.isPlaying)
                    altTreesMain.LogWarning("Instantiate object pool. [" + treesPoolArray[indexTemp].tree.name + "]");
                goTemp = null;

                goTemp = Instantiate(treesPoolArray[indexTemp].tree.lods[lod], vectTemp, Quaternion.identity) as GameObject;
                altTreeInstanceTemp = goTemp.AddComponent<AltTreeInstance>();
                altTreeInstanceTemp.isObject = treesPoolArray[indexTemp].tree.isObject;
                altTreeInstanceTemp.manager = this;
                altTreeInstanceTemp.idTree = idTree;
                altTreeInstanceTemp.altTreesId = altTreesID;
                altTreeInstanceTemp.hueLeave = _colorLeave;
                altTreeInstanceTemp.hueLeaveStar = _colorLeave;
                altTreeInstanceTemp.hueBark = _colorBark;
                altTreeInstanceTemp.hueBarkStar = _colorBark;

                rendererTemp = goTemp.GetComponent<MeshRenderer>();
                rendererTemp.sharedMaterials = treesPoolArray[indexTemp].objsArray[lod].materialsMesh;

                propBlock.Clear();
                propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1.0f);
                propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 0.0f);
                propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, _colorLeave);
                propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, _colorBark);
                rendererTemp.SetPropertyBlock(propBlock);

                goTemp.transform.parent = transform;
                goTemp.hideFlags = HideFlags.DontSave;
                altTreeInstanceTemp = null;
                goTemp.SetActive(true);
                rendererTemp = null;

                return goTemp;
            }
        }

        public GameObject getColliderPool(int idPrefab, bool isBillboardCollider)
        {
            int indexTemp = getPrototypeIndex(idPrefab);

            if (!isBillboardCollider || treesPoolArray[indexTemp].tree.isCollidersEqual)
            {
                if (treesPoolArray[indexTemp].collidersArray.Count > 0)
                {
                    goTemp = treesPoolArray[indexTemp].collidersArray[0];
                    treesPoolArray[indexTemp].collidersArray.RemoveAt(0);
                    if (goTemp != null)
                    {
                        goTemp.SetActive(true);

                        return goTemp;
                    }
                    else
                    {
                        altTreesMain.LogWarning("goTemp==null. Instantiate Collider pool. [" + treesPoolArray[indexTemp].tree.name + "]");
                        goTemp = null;
                        goTemp = Instantiate(treesPoolArray[indexTemp].tree.colliders, vectTemp, Quaternion.identity) as GameObject;
                        goTemp.transform.parent = transform;
                        goTemp.hideFlags = HideFlags.DontSave;

                        return goTemp;
                    }
                }
                else
                {
                    altTreesMain.LogWarning("Instantiate Collider pool. [" + treesPoolArray[indexTemp].tree.name + "]");

                    goTemp = null;
                    goTemp = Instantiate(treesPoolArray[indexTemp].tree.colliders, vectTemp, Quaternion.identity) as GameObject;
                    goTemp.transform.parent = transform;
                    goTemp.hideFlags = HideFlags.DontSave;

                    return goTemp;
                }
            }
            else
            {
                if (treesPoolArray[indexTemp].colliderBillboardsArray.Count > 0)
                {
                    goTemp = treesPoolArray[indexTemp].colliderBillboardsArray[0];
                    treesPoolArray[indexTemp].colliderBillboardsArray.RemoveAt(0);
                    if (goTemp != null)
                    {
                        goTemp.SetActive(true);

                        return goTemp;
                    }
                    else
                    {
                        altTreesMain.LogWarning("goTemp==null. Instantiate BillboardCollider pool. [" + treesPoolArray[indexTemp].tree.name + "]");
                        goTemp = null;
                        goTemp = Instantiate(treesPoolArray[indexTemp].tree.billboardColliders, vectTemp, Quaternion.identity) as GameObject;
                        goTemp.transform.parent = transform;
                        goTemp.hideFlags = HideFlags.DontSave;

                        return goTemp;
                    }
                }
                else
                {
                    altTreesMain.LogWarning("Instantiate BillboardCollider pool. [" + treesPoolArray[indexTemp].tree.name + "]");

                    goTemp = null;
                    goTemp = Instantiate(treesPoolArray[indexTemp].tree.billboardColliders, vectTemp, Quaternion.identity) as GameObject;
                    goTemp.transform.parent = transform;
                    goTemp.hideFlags = HideFlags.DontSave;

                    return goTemp;
                }
            }
        }


        public void delColliderPool(int idPrefab, GameObject go, bool isBillboardCollider)
        {
            int indexTemp = getPrototypeIndex(idPrefab);

            if (!isBillboardCollider || treesPoolArray[indexTemp].tree.isCollidersEqual)
            {
                if (treesPoolArray[indexTemp].collidersArray.Count > (treesPoolArray[indexTemp].tree.isCollidersEqual ? altTreesMain.altTreesManagerData.collidersPerOneMaxPool + altTreesMain.altTreesManagerData.colliderBillboardsPerOneMaxPool : altTreesMain.altTreesManagerData.collidersPerOneMaxPool))
                {
                    altTreesMain.LogWarning("Destroy Collider pool. [" + treesPoolArray[indexTemp].tree.name + "]");

                    go.transform.position = vectTemp;
                    Destroy(go);
                }
                else
                {
                    go.transform.position = vectTemp;   //11
                    go.SetActive(false);
                    treesPoolArray[indexTemp].collidersArray.Add(go);
                }
            }
            else
            {
                if (treesPoolArray[indexTemp].colliderBillboardsArray.Count > altTreesMain.altTreesManagerData.colliderBillboardsPerOneMaxPool)
                {
                    altTreesMain.LogWarning("Destroy BillboardCollider pool. [" + treesPoolArray[indexTemp].tree.name + "]");

                    go.transform.position = vectTemp;
                    Destroy(go);
                }
                else
                {
                    go.transform.position = vectTemp;
                    go.SetActive(false);
                    treesPoolArray[indexTemp].colliderBillboardsArray.Add(go);
                }
            }
        }

        public GameObject getObjBillboardPool(int idPrefab, float widthScale, float heightScale, float rotation, Color _color)
        {
            int indexTemp = getPrototypeIndex(idPrefab);

            if (objBillboardsPool.Count > 0)
            {
                objBillboardTemp = objBillboardsPool[0];
                objBillboardsPool.RemoveAt(0);

                if (objBillboardTemp.go != null)
                {
                    objBillboardsUsedPool.Add(objBillboardTemp.go, objBillboardTemp);

                    objBillboardTemp.mr.sharedMaterial = treesPoolArray[indexTemp].materialBillboard;

                    propBlock.Clear();
                    if (!altTreesMain.altTreesManagerData.drawDebugBillboards)
                    {
                        propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1f);
                        propBlock.SetFloat("_Width", treesPoolArray[indexTemp].tree.size * widthScale / 2f);
                        propBlock.SetFloat("_Height", treesPoolArray[indexTemp].tree.size * heightScale / 2f);
                        propBlock.SetFloat("_Rotation", rotation);
                        propBlock.SetColor("_HueVariation", _color);
                    }
                    else
                    {
                        colorTemp.r = Random.value;
                        colorTemp.g = Random.value;
                        colorTemp.b = Random.value;
                        colorTemp.a = 1f;
                        propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1f);
                        propBlock.SetFloat("_Width", treesPoolArray[indexTemp].tree.size * widthScale / 2f);
                        propBlock.SetFloat("_Height", treesPoolArray[indexTemp].tree.size * heightScale / 2f);
                        propBlock.SetFloat("_Rotation", rotation);
                        propBlock.SetColor("_HueVariation", colorTemp);
                    }
                    objBillboardTemp.mr.SetPropertyBlock(propBlock);

                    boundsTemp = objBillboardTemp.ms.bounds;
                    boundsTemp.max += new Vector3(treesPoolArray[indexTemp].tree.size / 2f + treesPoolArray[indexTemp].tree.up, treesPoolArray[indexTemp].tree.size / 2f + treesPoolArray[indexTemp].tree.up, treesPoolArray[indexTemp].tree.size / 2f + treesPoolArray[indexTemp].tree.up);
                    boundsTemp.min -= new Vector3(treesPoolArray[indexTemp].tree.size / 2f - treesPoolArray[indexTemp].tree.up, treesPoolArray[indexTemp].tree.size / 2f - treesPoolArray[indexTemp].tree.up, treesPoolArray[indexTemp].tree.size / 2f - treesPoolArray[indexTemp].tree.up);
                    boundsTemp.max += new Vector3(treesPoolArray[indexTemp].tree.size * widthScale / 2f, treesPoolArray[indexTemp].tree.size * heightScale / 2f, treesPoolArray[indexTemp].tree.size * widthScale / 2f);
                    boundsTemp.min -= new Vector3(treesPoolArray[indexTemp].tree.size * widthScale / 2f, treesPoolArray[indexTemp].tree.size * heightScale / 2f, treesPoolArray[indexTemp].tree.size * widthScale / 2f);
                    objBillboardTemp.ms.bounds = boundsTemp;

                    objBillboardTemp.go.SetActive(true);
                    return objBillboardTemp.go;
                }
                else
                {
                    altTreesMain.LogError("objBillboardTemp.go == null");

                    if (Application.isPlaying)
                        altTreesMain.LogWarning("Instantiate billboard object pool. [" + treesPoolArray[indexTemp].tree.name + "]");

                    objBillboardTemp = getPlaneBillboard();

                    objBillboardsUsedPool.Add(objBillboardTemp.go, objBillboardTemp);

                    objBillboardTemp.mr.sharedMaterial = treesPoolArray[indexTemp].materialBillboard;
                    propBlock.Clear();
                    if (!altTreesMain.altTreesManagerData.drawDebugBillboards)
                    {
                        propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1f);
                        propBlock.SetFloat("_Width", treesPoolArray[indexTemp].tree.size * widthScale / 2f);
                        propBlock.SetFloat("_Height", treesPoolArray[indexTemp].tree.size * heightScale / 2f);
                        propBlock.SetFloat("_Rotation", rotation);
                        propBlock.SetColor("_HueVariation", _color);
                    }
                    else
                    {
                        colorTemp.r = Random.value;
                        colorTemp.g = Random.value;
                        colorTemp.b = Random.value;
                        colorTemp.a = 1f;
                        propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1f);
                        propBlock.SetFloat("_Width", treesPoolArray[indexTemp].tree.size * widthScale / 2f);
                        propBlock.SetFloat("_Height", treesPoolArray[indexTemp].tree.size * heightScale / 2f);
                        propBlock.SetFloat("_Rotation", rotation);
                        propBlock.SetColor("_HueVariation", colorTemp);
                    }
                    objBillboardTemp.mr.SetPropertyBlock(propBlock);

                    boundsTemp = objBillboardTemp.ms.bounds;
                    boundsTemp.max += new Vector3(treesPoolArray[indexTemp].tree.size / 2f + treesPoolArray[indexTemp].tree.up, treesPoolArray[indexTemp].tree.size / 2f + treesPoolArray[indexTemp].tree.up, treesPoolArray[indexTemp].tree.size / 2f + treesPoolArray[indexTemp].tree.up);
                    boundsTemp.min -= new Vector3(treesPoolArray[indexTemp].tree.size / 2f - treesPoolArray[indexTemp].tree.up, treesPoolArray[indexTemp].tree.size / 2f - treesPoolArray[indexTemp].tree.up, treesPoolArray[indexTemp].tree.size / 2f - treesPoolArray[indexTemp].tree.up);
                    boundsTemp.max += new Vector3(treesPoolArray[indexTemp].tree.size * widthScale / 2f, treesPoolArray[indexTemp].tree.size * heightScale / 2f, treesPoolArray[indexTemp].tree.size * widthScale / 2f);
                    boundsTemp.min -= new Vector3(treesPoolArray[indexTemp].tree.size * widthScale / 2f, treesPoolArray[indexTemp].tree.size * heightScale / 2f, treesPoolArray[indexTemp].tree.size * widthScale / 2f);
                    objBillboardTemp.ms.bounds = boundsTemp;


                    objBillboardTemp.go.hideFlags = HideFlags.DontSave;
                    return objBillboardTemp.go;
                }
            }
            else
            {
                if (Application.isPlaying)
                    altTreesMain.LogWarning("Instantiate billboard object pool. [" + treesPoolArray[indexTemp].tree.name + "]");

                objBillboardTemp = getPlaneBillboard();

                objBillboardsUsedPool.Add(objBillboardTemp.go, objBillboardTemp);

                objBillboardTemp.mr.sharedMaterial = treesPoolArray[indexTemp].materialBillboard;
                propBlock.Clear();
                if (!altTreesMain.altTreesManagerData.drawDebugBillboards)
                {
                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1f);
                    propBlock.SetFloat("_Width", treesPoolArray[indexTemp].tree.size * widthScale / 2f);
                    propBlock.SetFloat("_Height", treesPoolArray[indexTemp].tree.size * heightScale / 2f);
                    propBlock.SetFloat("_Rotation", rotation);
                    propBlock.SetColor("_HueVariation", _color);
                }
                else
                {
                    colorTemp.r = Random.value;
                    colorTemp.g = Random.value;
                    colorTemp.b = Random.value;
                    colorTemp.a = 1f;
                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1f);
                    propBlock.SetFloat("_Width", treesPoolArray[indexTemp].tree.size * widthScale / 2f);
                    propBlock.SetFloat("_Height", treesPoolArray[indexTemp].tree.size * heightScale / 2f);
                    propBlock.SetFloat("_Rotation", rotation);
                    propBlock.SetColor("_HueVariation", colorTemp);
                }
                objBillboardTemp.mr.SetPropertyBlock(propBlock);

                boundsTemp = objBillboardTemp.ms.bounds;
                boundsTemp.max += new Vector3(treesPoolArray[indexTemp].tree.size / 2f + treesPoolArray[indexTemp].tree.up, treesPoolArray[indexTemp].tree.size / 2f + treesPoolArray[indexTemp].tree.up, treesPoolArray[indexTemp].tree.size / 2f + treesPoolArray[indexTemp].tree.up);
                boundsTemp.min -= new Vector3(treesPoolArray[indexTemp].tree.size / 2f - treesPoolArray[indexTemp].tree.up, treesPoolArray[indexTemp].tree.size / 2f - treesPoolArray[indexTemp].tree.up, treesPoolArray[indexTemp].tree.size / 2f - treesPoolArray[indexTemp].tree.up);
                boundsTemp.max += new Vector3(treesPoolArray[indexTemp].tree.size * widthScale / 2f, treesPoolArray[indexTemp].tree.size * heightScale / 2f, treesPoolArray[indexTemp].tree.size * widthScale / 2f);
                boundsTemp.min -= new Vector3(treesPoolArray[indexTemp].tree.size * widthScale / 2f, treesPoolArray[indexTemp].tree.size * heightScale / 2f, treesPoolArray[indexTemp].tree.size * widthScale / 2f);
                objBillboardTemp.ms.bounds = boundsTemp;


                objBillboardTemp.go.hideFlags = HideFlags.DontSave;
                return objBillboardTemp.go;
            }
        }


        public void delObjPool(int idPrefab, int lod, GameObject go)
        {
            int indexTemp = getPrototypeIndex(idPrefab);

            treesList.Remove(go);
            if (!Application.isPlaying || treesPoolArray[indexTemp].objsArray[lod].objs.Count > altTreesMain.altTreesManagerData.objsPerOneMaxPool)
            {
                if (Application.isPlaying)
                    altTreesMain.LogWarning("Destroy object pool. [" + treesPoolArray[indexTemp].tree.name + "]");
            
                go.transform.position = vectTemp;
                if (Application.isPlaying)
                    Destroy(go);
                else
                    DestroyImmediate(go);
            }
            else
            {
                go.transform.position = vectTemp;
                altTreeInstanceTemp = go.GetComponent<AltTreeInstance>();
                go.SetActive(false);
                treesPoolArray[indexTemp].objsArray[lod].objs.Add(go);
            }
        }

        public void delObjBillboardPool(int idPrefab, GameObject go)
        {
            treesList.Remove(go);

            if (objBillboardsPool.Count > altTreesMain.altTreesManagerData.billboardsMaxPool || !Application.isPlaying)
            {
                if (Application.isPlaying)
                    altTreesMain.LogWarning("Destroy billboard object pool.");
                go.transform.position = vectTemp;
                if (Application.isPlaying)
                {
                    if (objBillboardsUsedPool.ContainsKey(go))
                    {
                        Destroy(objBillboardsUsedPool[go].ms);
                        objBillboardsUsedPool.Remove(go);
                        Destroy(go);
                    }
                    else
                    {
                        altTreesMain.LogError("!objBillboardsUsedPool.ContainsKey(go)");
                        Destroy(go.GetComponent<MeshFilter>().mesh);
                        Destroy(go);
                    }
                }
                else
                {
                    if (objBillboardsUsedPool.ContainsKey(go))
                    {
                        DestroyImmediate(objBillboardsUsedPool[go].ms);
                        objBillboardsUsedPool.Remove(go);
                    }
                    else
                    {
                        altTreesMain.LogError("!objBillboardsUsedPool.ContainsKey(go)");
                        DestroyImmediate(go.GetComponent<MeshFilter>().mesh);
                        DestroyImmediate(go);
                    }
                }

            }
            else
            {
                go.transform.position = vectTemp;
                go.SetActive(false);
                objBillboardsPool.Add(objBillboardsUsedPool[go]);
                objBillboardsUsedPool.Remove(go);
            }
        }

        objBillboardPool getPlaneBillboard()
        {
            Mesh ms = new Mesh();

            Vector3[] verts = new Vector3[4];
            Vector2[] uvs = new Vector2[4];
            Vector2[] uvs2 = new Vector2[4];
            Vector2[] uvs3 = new Vector2[4];
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


            uvs2_0.x = -1;
            uvs2_0.y = -1;
            uvs2[0] = uvs2_0;

            uvs2_1.x = 1;
            uvs2_1.y = -1;
            uvs2[1] = uvs2_1;

            uvs2_2.x = 1;
            uvs2_2.y = 1;
            uvs2[2] = uvs2_2;

            uvs2_3.x = -1;
            uvs2_3.y = 1;
            uvs2[3] = uvs2_3;

            uvs2_0.x = -1;
            uvs2_0.y = -1;
            uvs2[0] = uvs2_0;

            uvs2_1.x = 1;
            uvs2_1.y = -1;
            uvs2[1] = uvs2_1;

            uvs2_2.x = 1;
            uvs2_2.y = 1;
            uvs2[2] = uvs2_2;

            uvs2_3.x = -1;
            uvs2_3.y = 1;
            uvs2[3] = uvs2_3;


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

            ms.SetIndices(indices, MeshTopology.Triangles, 0);
            ms.hideFlags = HideFlags.HideAndDontSave;


            goTemp = new GameObject("planeBillboard", typeof(MeshFilter), typeof(MeshRenderer));
            goTemp.transform.parent = transform;
            goTemp.hideFlags = HideFlags.DontSave;
            goTemp.GetComponent<MeshFilter>().sharedMesh = ms;

            objBillboardTemp = new objBillboardPool();
            objBillboardTemp.go = goTemp;
            objBillboardTemp.ms = ms;
            objBillboardTemp.mr = goTemp.GetComponent<MeshRenderer>();


            return objBillboardTemp;
        }

        Vector2 uvs_0 = new Vector2(0, 0);
        Vector2 uvs_1 = new Vector2(1f / 3f, 0);
        Vector2 uvs_2 = new Vector2(1f / 3f, 1f / 3f);
        Vector2 uvs_3 = new Vector2(0, 1f / 3f);

        Vector2 uvs2_0 = new Vector2(-1, -1);
        Vector2 uvs2_1 = new Vector2(1, -1);
        Vector2 uvs2_2 = new Vector2(1, 1);
        Vector2 uvs2_3 = new Vector2(-1, 1);

        Vector2 uvs3Vect = new Vector2(0, 0);

        AltTreesTrees att = null;
        AltTree at = null;

        void createMeshBillboards()
        {
            List<GameObject> rendersTemp = new List<GameObject>();

            if (AltTreesQuad.objsToInit[0].isRender)
            {
                if (AltTreesQuad.objsToInit[0].LOD <= AltTreesQuad.objsToInit[0].startBillboardsLOD)
                    AltTreesQuad.objsToInit[0].isRender = false;
            }

            for (int j = 0; j < altTrees[AltTreesQuad.objsToInit[0].altTreesID].prototypes.Length; j++)
            {
                if (AltTreesQuad.objsToInit[0].treePrefabsCount.ContainsKey(altTrees[AltTreesQuad.objsToInit[0].altTreesID].prototypes[j].tree.id))
                {
                    int indexTemp = getPrototypeIndex(altTrees[AltTreesQuad.objsToInit[0].altTreesID].prototypes[j].tree.id);
                    int countTreesTemp = AltTreesQuad.objsToInit[0].treePrefabsCount[altTrees[AltTreesQuad.objsToInit[0].altTreesID].prototypes[j].tree.id];

                    if (countTreesTemp != 0)
                    {
                        GameObject go = new GameObject("mesh_" + j + "_" + AltTreesQuad.objsToInit[0].LOD, typeof(MeshRenderer), typeof(MeshFilter));
                        go.transform.position = new Vector3(0, 0, 0);
                        go.hideFlags = HideFlags.DontSave;
                        Mesh ms = new Mesh();

                        int countTemp = Mathf.Clamp(countTreesTemp, 0, 16250);

                        Vector3[] verts = new Vector3[countTemp * 4];
                        Vector2[] uvs = new Vector2[countTemp * 4];
                        Vector2[] uvs2 = new Vector2[countTemp * 4];
                        Vector2[] uvs3 = new Vector2[countTemp * 4];
                        Color[] cols = new Color[countTemp * 4];
                        int[] indices = new int[countTemp * 6];

                        int iTemp = 0;
                        int countT = 0;


                        if (altTreesMain.altTreesManagerData.drawDebugBillboards)
                        {
                            colorTemp.r = Random.value;
                            colorTemp.g = Random.value;
                            colorTemp.b = Random.value;
                            colorTemp.a = 1f;
                        }

                        for (int i2 = 0; i2 < AltTreesQuad.objsToInit[0].treesCount; i2++)
                        {
                            att = altTrees[AltTreesQuad.objsToInit[0].altTreesID].trees[AltTreesQuad.objsToInit[0].trees[i2].idTree];   //! 
                            at = altTrees[AltTreesQuad.objsToInit[0].altTreesID].prototypes[j].tree;

                            if (att != null)
                            {
                                if (att.idPrototype == altTrees[AltTreesQuad.objsToInit[0].altTreesID].prototypes[j].tree.id)
                                {
                                    verts[iTemp * 4 + 0] = att.getPosWorldBillboard();
                                    verts[iTemp * 4 + 0].y += at.size * att.heightScale / 2f + at.up * att.heightScale;
                                    verts[iTemp * 4 + 1] = verts[iTemp * 4 + 0];
                                    verts[iTemp * 4 + 2] = verts[iTemp * 4 + 0];
                                    verts[iTemp * 4 + 3] = verts[iTemp * 4 + 0];


                                    uvs[iTemp * 4 + 0] = uvs_0;
                                    uvs[iTemp * 4 + 1] = uvs_1;
                                    uvs[iTemp * 4 + 2] = uvs_2;
                                    uvs[iTemp * 4 + 3] = uvs_3;


                                    if (!altTreesMain.altTreesManagerData.drawDebugBillboards)
                                    {
                                        cols[iTemp * 4 + 0] = att.color;
                                        cols[iTemp * 4 + 1] = att.color;
                                        cols[iTemp * 4 + 2] = att.color;
                                        cols[iTemp * 4 + 3] = att.color;
                                    }
                                    else
                                    {
                                        cols[iTemp * 4 + 0] = colorTemp;
                                        cols[iTemp * 4 + 1] = colorTemp;
                                        cols[iTemp * 4 + 2] = colorTemp;
                                        cols[iTemp * 4 + 3] = colorTemp;
                                    }


                                    uvs2_0.x = -at.size * att.widthScale / 2f;
                                    uvs2_0.y = -at.size * att.heightScale / 2f;
                                    uvs2[iTemp * 4 + 0] = uvs2_0;

                                    uvs2_1.x = at.size * att.widthScale / 2f;
                                    uvs2_1.y = -at.size * att.heightScale / 2f;
                                    uvs2[iTemp * 4 + 1] = uvs2_1;

                                    uvs2_2.x = at.size * att.widthScale / 2f;
                                    uvs2_2.y = at.size * att.heightScale / 2f;
                                    uvs2[iTemp * 4 + 2] = uvs2_2;

                                    uvs2_3.x = -at.size * att.widthScale / 2f;
                                    uvs2_3.y = at.size * att.heightScale / 2f;
                                    uvs2[iTemp * 4 + 3] = uvs2_3;

                                    uvs3Vect.x = att.rotation;

                                    uvs3[iTemp * 4 + 0] = uvs3Vect;
                                    uvs3[iTemp * 4 + 1] = uvs3Vect;
                                    uvs3[iTemp * 4 + 2] = uvs3Vect;
                                    uvs3[iTemp * 4 + 3] = uvs3Vect;


                                    indices[iTemp * 6 + 0] = iTemp * 4 + 3;
                                    indices[iTemp * 6 + 1] = iTemp * 4 + 2;
                                    indices[iTemp * 6 + 2] = iTemp * 4 + 0;
                                    indices[iTemp * 6 + 3] = iTemp * 4 + 2;
                                    indices[iTemp * 6 + 4] = iTemp * 4 + 1;
                                    indices[iTemp * 6 + 5] = iTemp * 4 + 0;

                                    iTemp++;
                                }
                            }
                            else
                                Debug.LogError("att==null");


                            if (iTemp == countTemp)
                            {
                                ms.vertices = verts;

                                ms.uv = uvs;
                                ms.uv2 = uvs2;
                                ms.uv3 = uvs3;
                                ms.colors = cols;

                                ms.SetIndices(indices, MeshTopology.Triangles, 0);
                                ms.RecalculateBounds();
                                Bounds bn2 = ms.bounds;
                                bn2.max += new Vector3(at.size * att.widthScale / 2f, at.size * att.heightScale / 2f + at.up * att.heightScale, at.size * att.widthScale / 2f);
                                bn2.min -= new Vector3(at.size * att.widthScale / 2f, at.size * att.heightScale / 2f + at.up * att.heightScale, at.size * att.widthScale / 2f);
                                ms.bounds = bn2;
                                ms.hideFlags = HideFlags.HideAndDontSave;

                                go.GetComponent<MeshFilter>().sharedMesh = ms;

                                go.GetComponent<MeshRenderer>().sharedMaterial = treesPoolArray[indexTemp].materialBillboardGroup;

                                go.transform.parent = this.transform;
                                go.transform.localPosition = (altTrees[AltTreesQuad.objsToInit[0].altTreesID].step - jump) * altTreesMain.altTreesManagerData.sizePatch;
                                go.SetActive(false);
                                rendersTemp.Add(go);




                                go = new GameObject("mesh_" + j + "_" + AltTreesQuad.objsToInit[0].LOD, typeof(MeshRenderer), typeof(MeshFilter));
                                go.transform.position = new Vector3(0, 0, 0);
                                go.hideFlags = HideFlags.DontSave;
                                ms = new Mesh();

                                int countTemp2 = Mathf.Clamp(countTreesTemp - countT * countTemp + countTemp, 0, 16250);

                                verts = new Vector3[countTemp2 * 4];
                                uvs = new Vector2[countTemp2 * 4];
                                uvs2 = new Vector2[countTemp2 * 4];
                                uvs3 = new Vector2[countTemp2 * 4];
                                cols = new Color[countTemp2 * 4];
                                indices = new int[countTemp2 * 6];

                                iTemp = 0;

                                if (altTreesMain.altTreesManagerData.drawDebugBillboards)
                                {
                                    colorTemp.r = Random.value;
                                    colorTemp.g = Random.value;
                                    colorTemp.b = Random.value;
                                    colorTemp.a = 1f;
                                }

                                countT++;
                            }
                        }

                        ms.vertices = verts;

                        ms.uv = uvs;
                        ms.uv2 = uvs2;
                        ms.uv3 = uvs3;
                        ms.colors = cols;

                        ms.SetIndices(indices, MeshTopology.Triangles, 0);
                        ms.RecalculateBounds();
                        Bounds bn = ms.bounds;
                        bn.max += new Vector3(at.size * att.widthScale / 2f, at.size * att.heightScale / 2f + at.up * att.heightScale, at.size * att.widthScale / 2f);
                        bn.min -= new Vector3(at.size * att.widthScale / 2f, at.size * att.heightScale / 2f + at.up * att.heightScale, at.size * att.widthScale / 2f);
                        ms.bounds = bn;
                        ms.hideFlags = HideFlags.HideAndDontSave;

                        go.GetComponent<MeshFilter>().sharedMesh = ms;

                        go.GetComponent<MeshRenderer>().sharedMaterial = treesPoolArray[indexTemp].materialBillboardGroup;



                        go.transform.parent = this.transform;
                        go.transform.localPosition = (altTrees[AltTreesQuad.objsToInit[0].altTreesID].step - jump) * altTreesMain.altTreesManagerData.sizePatch;
                        go.SetActive(false);
                        rendersTemp.Add(go);
                    }
                }
            }

            if (AltTreesQuad.objsToInit[0].renders.Count != 0)
            {
                rendersToRemove.AddRange(AltTreesQuad.objsToInit[0].renders);
                rendersToRemoveCounter = 0;
            }
        
            AltTreesQuad.objsToInit[0].renders.Clear();

            for (int i = 0; i < rendersTemp.Count; i++)
            {
                AltTreesQuad.objsToInit[0].renders.Add(rendersTemp[i]);
            }

            rendersTemp.Clear();

            AltTreesQuad.objsToInit[0].isInit = true;
            AltTreesQuad.objsToInit[0].isGenerateAllBillboardsOnStart = false;

        }
    

        public void addCamera(Transform cam)
        {
            addCamera(cam, null);
        }

        public void addCamera(Transform cam, AtiTemp att)
        {
            if (cam != null)
            {
                bool isError = false;
                for (int c = 0; c < cameras.Length; c++)
                {
                    if (cameras[c].Equals(cam))
                    {
                        isError = true;
                        break;
                    }
                }

                if (!isError)
                {
                    camerasTemp = cameras;
                    cameras = new Transform[cameras.Length + 1];
                    for (int c = 0; c < camerasTemp.Length; c++)
                    {
                        cameras[c] = camerasTemp[c];
                    }
                    cameras[camerasTemp.Length] = cam;

                    treesCamerasTemp = treesCameras;
                    treesCameras = new AtiTemp[treesCameras.Length + 1];
                    for (int c = 0; c < treesCamerasTemp.Length; c++)
                    {
                        treesCameras[c] = treesCamerasTemp[c];
                    }
                    treesCameras[treesCamerasTemp.Length] = att;
                }
            }
        }

        public void removeCamera(Transform cam)
        {
            if (cam != null && cameras.Length > 0)
            {
                bool isOk = false;
                for (int c = 0; c < cameras.Length; c++)
                {
                    if (cameras[c].Equals(cam))
                    {
                        isOk = true;
                        break;
                    }
                }

                if (isOk)
                {
                    int schet = 0;
                    camerasTemp = cameras;
                    cameras = new Transform[cameras.Length - 1];
                    treesCamerasTemp = treesCameras;
                    treesCameras = new AtiTemp[treesCameras.Length - 1];
                    for (int c = 0; c < camerasTemp.Length; c++)
                    {
                        if (!camerasTemp[c].Equals(cam))
                        {
                            cameras[schet] = camerasTemp[c];
                            treesCameras[schet] = treesCamerasTemp[c];
                        }
                        schet++;
                    }
                }
            }
        }

        public bool isSelectionTree = false;

        public void setSelectionTrees(Transform[] trs)
        {
            isSelectionTree = true;

            for (int i = 0; i < trs.Length; i++)
            {
                altTreeInstanceTemp = trs[i].GetComponent<AltTreeInstance>();
                if (altTreeInstanceTemp != null)
                {
                    if (!altTreeInstanceTemp.isObject)
                    {
                        if (altTrees[altTreeInstanceTemp.altTreesId].trees[altTreeInstanceTemp.idTree] != null)
                        {
                            if (!altTrees[altTreeInstanceTemp.altTreesId].trees[altTreeInstanceTemp.idTree].go.Equals(trs[i].gameObject))
                            {
                                altTreeInstanceTemp.idTree = copyTree(altTreeInstanceTemp.altTreesId, altTreeInstanceTemp.idTree, trs[i].gameObject, true).idTree;
                                quads[altTreeInstanceTemp.altTreesId].checkTreesAdd(altTrees[altTreeInstanceTemp.altTreesId].trees[altTreeInstanceTemp.idTree].getPosWorld().x, altTrees[altTreeInstanceTemp.altTreesId].trees[altTreeInstanceTemp.idTree].getPosWorld().z, altTrees[altTreeInstanceTemp.altTreesId].trees[altTreeInstanceTemp.idTree]);
                            }


                            addCamera(trs[i], new AtiTemp(altTrees[altTreeInstanceTemp.altTreesId], altTreeInstanceTemp.altTreesId, altTreeInstanceTemp.idTree, altTreeInstanceTemp.isObject));

                            quads[altTreeInstanceTemp.altTreesId].lockQuads(altTrees[altTreeInstanceTemp.altTreesId].trees[altTreeInstanceTemp.idTree].getPosWorld());
                        }
                        else
                        {
                            if (Application.isPlaying)
                                Destroy(trs[i].gameObject);
                            else
                            {

                                #if UNITY_EDITOR
                                    Object[] objs = Selection.objects;
                                    Object[] objsTemp = new Object[objs.Length - 1];
                                    int h = 0;
                                    for (int g = 0; g < objs.Length; g++)
                                    {
                                        if (!((GameObject)objs[g]).Equals(trs[i].gameObject))
                                        {
                                            objsTemp[h] = objs[g];
                                            h++;
                                        }
                                    }

                                    Selection.objects = objsTemp;
                                #endif

                                DestroyImmediate(trs[i].gameObject);
                            }
                        }
                    }
                    else
                    {
                        if (altTrees[altTreeInstanceTemp.altTreesId].treesNoGroup[altTreeInstanceTemp.idTree] != null)
                        {
                            if (!altTrees[altTreeInstanceTemp.altTreesId].treesNoGroup[altTreeInstanceTemp.idTree].go.Equals(trs[i].gameObject))
                            {
                                altTreeInstanceTemp.idTree = copyTree(altTreeInstanceTemp.altTreesId, altTreeInstanceTemp.idTree, trs[i].gameObject, false).idTree;
                                quads[altTreeInstanceTemp.altTreesId].checkTreesAdd(altTrees[altTreeInstanceTemp.altTreesId].treesNoGroup[altTreeInstanceTemp.idTree].getPosWorld().x, altTrees[altTreeInstanceTemp.altTreesId].treesNoGroup[altTreeInstanceTemp.idTree].getPosWorld().z, altTrees[altTreeInstanceTemp.altTreesId].treesNoGroup[altTreeInstanceTemp.idTree]);
                            }


                            addCamera(trs[i], new AtiTemp(altTrees[altTreeInstanceTemp.altTreesId], altTreeInstanceTemp.altTreesId, altTreeInstanceTemp.idTree, altTreeInstanceTemp.isObject));

                            quads[altTreeInstanceTemp.altTreesId].lockQuads(altTrees[altTreeInstanceTemp.altTreesId].treesNoGroup[altTreeInstanceTemp.idTree].getPosWorld());
                        }
                        else
                        {
                            if (Application.isPlaying)
                                Destroy(trs[i].gameObject);
                            else
                            {
                                #if UNITY_EDITOR
                                    Object[] objs = Selection.objects;
                                    Object[] objsTemp = new Object[objs.Length - 1];
                                    int h = 0;
                                    for (int g = 0; g < objs.Length; g++)
                                    {
                                        if (!((GameObject)objs[g]).Equals(trs[i].gameObject))
                                        {
                                            objsTemp[h] = objs[g];
                                            h++;
                                        }
                                    }

                                    Selection.objects = objsTemp;

                                #endif
                                DestroyImmediate(trs[i].gameObject);
                            }
                        }
                    }
                }
            }
            altTreeInstanceTemp = null;
        }

        public void offSelectionTrees()
        {
            isSelectionTree = false;

            for (int i = 0; i < quads.Length; i++)
            {
                if (quads[i] != null)
                    quads[i].unlockQuads();
            }
        }

        public AltTreesPatch[] saveTrees()
        {
            AltTreeInstance ati = null;
            bool isRecalculateBound = false;
            List<AltTreesPatch> altTreesList = new List<AltTreesPatch>();
            List<AltTreesPatch> altTreesListReturn = new List<AltTreesPatch>();


            for (int i = 0; i < cameras.Length; i++)
            {
                isRecalculateBound = false;

                if (cameras[i] != null)
                {
                    ati = cameras[i].GetComponent<AltTreeInstance>();

                    if (ati != null)
                    {
                        if (!ati.isObject)
                        {
                            att = ati.manager.altTrees[ati.altTreesId].trees[ati.idTree];

                            if (AltUtilities.fastDistanceSqrt(cameras[i].position, att.getPosWorld()) > 0.0001f || !att.rotation.Equals(cameras[i].localRotation.eulerAngles.y) || (att.widthScale != cameras[i].localScale.x || att.heightScale != cameras[i].localScale.y || att.color != ati.hueLeave || att.colorBark != ati.hueBark))
                            {
                                if (!quads[ati.altTreesId].bound.inBounds(cameras[i].position.x, cameras[i].position.z))
                                {
                                    isRecalculateBound = true;
                                    quads[ati.altTreesId].removeTree(att);

                                    AltTreesPatch patchTemp = getPatch(cameras[i].position + jump * altTreesMain.altTreesManagerData.sizePatch);
                                    AltTreesPatch starPatch;

                                    if (patchTemp == null)
                                    {
                                        patchTemp = addPatch(Mathf.FloorToInt((cameras[i].position.x + jump.x * altTreesMain.altTreesManagerData.sizePatch) / ((float)altTreesMain.altTreesManagerData.sizePatch)), Mathf.FloorToInt((cameras[i].position.z + jump.z * altTreesMain.altTreesManagerData.sizePatch) / ((float)altTreesMain.altTreesManagerData.sizePatch)));

                                        ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].pos = patchTemp.getTreePosLocal(cameras[i].position, jump, altTreesMain.altTreesManagerData.sizePatch);
                                        ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].pos2D = new Vector2(ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].pos.x, ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].pos.z);
                                        ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].rotation = cameras[i].localRotation.eulerAngles.y;
                                        ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].widthScale = cameras[i].localScale.x;
                                        ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].heightScale = cameras[i].localScale.y;
                                        ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].color = ati.hueLeave;
                                        ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].colorBark = ati.hueBark;
                                        starPatch = ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].altTreesPatch;
                                        ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].altTreesPatch = patchTemp;


                                        patchTemp.tempTrees.Add(ati.manager.altTrees[ati.altTreesId].trees[ati.idTree]);
                                    }
                                    else
                                    {

                                        ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].pos = patchTemp.getTreePosLocal(cameras[i].position, jump, altTreesMain.altTreesManagerData.sizePatch);
                                        ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].pos2D = new Vector2(ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].pos.x, ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].pos.z);
                                        ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].rotation = cameras[i].localRotation.eulerAngles.y;
                                        ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].widthScale = cameras[i].localScale.x;
                                        ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].heightScale = cameras[i].localScale.y;
                                        ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].color = ati.hueLeave;
                                        ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].colorBark = ati.hueBark;
                                        starPatch = ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].altTreesPatch;
                                        ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].altTreesPatch = patchTemp;

                                        patchTemp.checkTreePrototype(ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].idPrototype, ati.manager.altTrees[ati.altTreesId].getAltTreePrototype(ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].idPrototype));
                                        addTrees(new AddTreesStruct[1] { new AddTreesStruct(cameras[i].position, ati.manager.altTrees[ati.altTreesId].getAltTreePrototype(ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].idPrototype)) }, patchTemp.altTreesId, ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].rotation, ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].heightScale, ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].widthScale, ati.hueLeave, ati.hueBark);

                                        if (!altTreesListReturn.Contains(patchTemp))
                                            altTreesListReturn.Add(patchTemp);
                                    }
                                    List<int> del = new List<int>();
                                    del.Add(ati.idTree);
                                    starPatch.EditDataFile(false, null, 0, del, -1);
                                    ati.manager.altTrees[ati.altTreesId].trees[ati.idTree] = null;
                                }
                                else
                                {
                                    quads[ati.altTreesId].reInitBillboards(att, cameras[i].position);
                                    att.pos = ati.manager.altTrees[ati.altTreesId].getTreePosLocal(cameras[i].position, jump, altTreesMain.altTreesManagerData.sizePatch);
                                    att.pos2D = new Vector2(att.pos.x, att.pos.z);
                                    att.rotation = cameras[i].localRotation.eulerAngles.y;
                                    ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].altTreesPatch = ati.manager.altTrees[ati.altTreesId];
                                    ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].widthScale = cameras[i].localScale.x;
                                    ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].heightScale = cameras[i].localScale.y;
                                    ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].color = ati.hueLeave;
                                    ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].colorBark = ati.hueBark;

                                    ati.manager.altTrees[ati.altTreesId].trees[ati.idTree].altTreesPatch.EditDataFile(false, null, 0, null, ati.idTree);
                                }

                                ati.manager.altTrees[ati.altTreesId].recalculateBound();

                                if (!altTreesListReturn.Contains(ati.manager.altTrees[ati.altTreesId]))
                                    altTreesListReturn.Add(ati.manager.altTrees[ati.altTreesId]);

                                if (isRecalculateBound)
                                    DestroyImmediate(ati.gameObject);
                            }
                        }
                        else
                        {
                            att = ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree];

                            if (AltUtilities.fastDistanceSqrt(cameras[i].position, att.getPosWorld()) > 0.0001f || !att.rotation.Equals(cameras[i].localRotation.eulerAngles.y) || (att.widthScale != cameras[i].localScale.x || att.heightScale != cameras[i].localScale.y || att.color != ati.hueLeave || att.colorBark != ati.hueBark))
                            {
                                if (!quads[ati.altTreesId].bound.inBounds(cameras[i].position.x, cameras[i].position.z))
                                {
                                    isRecalculateBound = true;
                                    quads[ati.altTreesId].removeTree(att, ati.isObject);

                                    AltTreesPatch patchTemp = getPatch(cameras[i].position + jump * altTreesMain.altTreesManagerData.sizePatch);
                                    AltTreesPatch starPatch;

                                    if (patchTemp == null)
                                    {
                                        patchTemp = addPatch(Mathf.FloorToInt((cameras[i].position.x + jump.x * altTreesMain.altTreesManagerData.sizePatch) / ((float)altTreesMain.altTreesManagerData.sizePatch)), Mathf.FloorToInt((cameras[i].position.z + jump.z * altTreesMain.altTreesManagerData.sizePatch) / ((float)altTreesMain.altTreesManagerData.sizePatch)));

                                        ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].pos = patchTemp.getTreePosLocal(cameras[i].position, jump, altTreesMain.altTreesManagerData.sizePatch);
                                        ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].pos2D = new Vector2(ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].pos.x, ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].pos.z);
                                        ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].rotation = cameras[i].localRotation.eulerAngles.y;
                                        ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].widthScale = cameras[i].localScale.x;
                                        ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].heightScale = cameras[i].localScale.y;
                                        ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].color = ati.hueLeave;
                                        ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].colorBark = ati.hueBark;
                                        starPatch = ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].altTreesPatch;
                                        ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].altTreesPatch = patchTemp;

                                        patchTemp.tempTrees.Add(ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree]);
                                    }
                                    else
                                    {

                                        ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].pos = patchTemp.getTreePosLocal(cameras[i].position, jump, altTreesMain.altTreesManagerData.sizePatch);
                                        ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].pos2D = new Vector2(ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].pos.x, ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].pos.z);
                                        ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].rotation = cameras[i].localRotation.eulerAngles.y;
                                        ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].widthScale = cameras[i].localScale.x;
                                        ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].heightScale = cameras[i].localScale.y;
                                        ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].color = ati.hueLeave;
                                        ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].colorBark = ati.hueBark;
                                        starPatch = ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].altTreesPatch;
                                        ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].altTreesPatch = patchTemp;

                                        ati.manager.altTrees[ati.altTreesId].checkTreePrototype(patchTemp.treesNoGroup[ati.idTree].idPrototype, ati.manager.altTrees[ati.altTreesId].getAltTreePrototype(ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].idPrototype));
                                        addTrees(new AddTreesStruct[1] { new AddTreesStruct(cameras[i].position, ati.manager.altTrees[ati.altTreesId].getAltTreePrototype(ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].idPrototype)) }, patchTemp.altTreesId, ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].rotation, ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].heightScale, ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].widthScale, ati.hueLeave, ati.hueBark);

                                        if (!altTreesListReturn.Contains(patchTemp))
                                            altTreesListReturn.Add(patchTemp);
                                    }
                                    List<int> del = new List<int>();
                                    del.Add(ati.idTree);
                                    starPatch.EditDataFile(true, null, 0, del, -1);
                                    ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree] = null;
                                }
                                else
                                {
                                    quads[ati.altTreesId].reInitBillboards(att, cameras[i].position, false);
                                    att.pos = ati.manager.altTrees[ati.altTreesId].getTreePosLocal(cameras[i].position, jump, altTreesMain.altTreesManagerData.sizePatch);
                                    att.pos2D = new Vector2(att.pos.x, att.pos.z);
                                    att.rotation = cameras[i].localRotation.eulerAngles.y;
                                    ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].altTreesPatch = ati.manager.altTrees[ati.altTreesId];
                                    ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].widthScale = cameras[i].localScale.x;
                                    ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].heightScale = cameras[i].localScale.y;
                                    ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].color = ati.hueLeave;
                                    ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].colorBark = ati.hueBark;

                                    ati.manager.altTrees[ati.altTreesId].treesNoGroup[ati.idTree].altTreesPatch.EditDataFile(true, null, 0, null, ati.idTree);
                                }

                                ati.manager.altTrees[ati.altTreesId].recalculateBound();

                                if (!altTreesListReturn.Contains(ati.manager.altTrees[ati.altTreesId]))
                                    altTreesListReturn.Add(ati.manager.altTrees[ati.altTreesId]);

                                if (isRecalculateBound)
                                    DestroyImmediate(ati.gameObject);
                            }
                        }
                    }
                }
                else
                {
                    if (treesCameras[i] != null)
                    {
                        if (!treesCameras[i].isObject)
                        {
                            att = treesCameras[i].altTrees.trees[treesCameras[i].idTree];
                            quads[treesCameras[i].altTreesId].removeTree(att, false);

                            List<int> del = new List<int>();
                            del.Add(treesCameras[i].idTree);
                            treesCameras[i].altTrees.EditDataFile(false, null, 0, del, -1);
                            treesCameras[i].altTrees.trees[treesCameras[i].idTree] = null;

                            treesCameras[i].altTrees.recalculateBound();

                            if (!altTreesListReturn.Contains(treesCameras[i].altTrees))
                                altTreesListReturn.Add(treesCameras[i].altTrees);
                        }
                        else
                        {
                            att = treesCameras[i].altTrees.treesNoGroup[treesCameras[i].idTree];
                            quads[treesCameras[i].altTreesId].removeTree(att, true);

                            List<int> del = new List<int>();
                            del.Add(treesCameras[i].idTree);
                            treesCameras[i].altTrees.EditDataFile(true, null, 0, del, -1);
                            treesCameras[i].altTrees.treesNoGroup[treesCameras[i].idTree] = null;

                            treesCameras[i].altTrees.recalculateBound();

                            if (!altTreesListReturn.Contains(treesCameras[i].altTrees))
                                altTreesListReturn.Add(treesCameras[i].altTrees);
                        }
                    }
                }
            }
            isRecalculateBound = false;

            if (!isRecalculateBound)
            {
                for (int i = 0; i < quads.Length; i++)
                {
                    if (quads[i] != null)
                        quads[i].goUpdateTrees();
                }
            }
            else
            {
                for (int i = 0; i < altTreesList.Count; i++)
                {
                    removeAltTrees(altTreesList[i], false);
                    addAltTrees(altTreesList[i], false);
                }
            }

            clearCameras();
            return altTreesListReturn.ToArray();
        }

        public bool removeTrees(Vector2 pos, float radius, AltTreesPatch at, List<int> removedTrees, List<int> removedTreesNoGroup, AltTree _tree = null)
        {
            bool result = false;
            for (int i = 0; i < quads.Length; i++)
            {
                if (altTrees[i] != null)
                {
                    if (altTrees[i].Equals(at))
                    {
                        if (quads[i].removeTrees(pos, radius, at, removedTrees, removedTreesNoGroup, _tree))
                        {
                            quads[i].goUpdateTrees();
                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        public bool removeTrees(AltTreesTrees[] att, Vector2 pos, float sizeX, float sizeZ, AltTreesPatch at, List<int> removedTrees, List<int> removedTreesNoGroup)
        {
            bool result = false;
            for (int i = 0; i < quads.Length; i++)
            {
                if (altTrees[i] != null)
                {
                    if (altTrees[i].Equals(at))
                    {
                        if (quads[i].removeTrees(att, pos, sizeX, sizeZ, at, removedTrees, removedTreesNoGroup))
                        {
                            quads[i].goUpdateTrees();
                            result = true;
                        }
                    }
                }
            }

            return result;
        }
    
        public AltTreesTrees[] getTreesForExport(Vector2 pos, float sizeX, float sizeZ, AltTreesPatch at)
        {
            List<AltTreesTrees> attTemp = new List<AltTreesTrees>();

            for (int i = 0; i < quads.Length; i++)
            {
                if (altTrees[i] != null)
                {
                    if (altTrees[i].Equals(at))
                    {
                        List<AltTreesTrees> attTemp2 = new List<AltTreesTrees>();
                        quads[i].getTreesForExport(pos, sizeX, sizeZ, at, attTemp2);
                        attTemp.AddRange(attTemp2);
                    }
                }
            }

            return attTemp.ToArray();
        }


        public void clearCameras()
        {
            if (Application.isPlaying)
            {
                Camera[] cams = Camera.allCameras;
                if (cams != null && cams.Length > 0)
                {
                    cameras = new Transform[1];
                    cameras[0] = cams[0].transform;
                    treesCameras = new AtiTemp[1];
                }
            }
            else
            {
                if (AltTreesManager.camEditor != null)
                {
                    cameras = new Transform[1];
                    cameras[0] = AltTreesManager.camEditor.transform;
                    treesCameras = new AtiTemp[1];
                }
            }
        }

        AltTreesPatch getPatch(Vector3 pos)
        {
            return altTreesMain.getPatch(pos, altTreesMain.altTreesManagerData.sizePatch);
        }

        AltTreesPatch addPatch(int _stepX, int _stepY)
        {
            AltTreesPatch atpTemp = new AltTreesPatch(_stepX, _stepY);

            AltTreesPatch[] patchesTemp = altTreesMain.altTreesManagerData.patches;
            altTreesMain.altTreesManagerData.patches = new AltTreesPatch[patchesTemp.Length + 1];
            for (int i = 0; i < patchesTemp.Length; i++)
            {
                altTreesMain.altTreesManagerData.patches[i] = patchesTemp[i];
            }
            altTreesMain.altTreesManagerData.patches[patchesTemp.Length] = atpTemp;
        
            return atpTemp;
        }


        void OnDestroy()
        {
            destroy(false);
        }

        void OnDisable()
        {
            destroy(true);
        }

        void OnApplicationQuit()
        {
            destroy(true);
        }

        /*void OnLevelWasLoaded(int level)
        {
            destroy(true);
        }*/

        public void destroy(bool destroyThis)
        {
            if (!isDestroyed)
            {
                for (int i = 0; i < altTrees.Length; i++)
                {
                    if (altTrees[i] != null)
                    {
                        if (altTrees[i].trees != null)
                        {
                            for (int j = 0; j < altTrees[i].trees.Length; j++)
                            {
                                if (altTrees[i].trees[j] != null && altTrees[i].trees[j].go != null && altTrees[i].trees[j].currentLOD != -2)
                                {
                                    if (Application.isPlaying)
                                        Destroy(altTrees[i].trees[j].go);
                                    else
                                        DestroyImmediate(altTrees[i].trees[j].go);
                                }
                                if (altTrees[i].trees[j] != null && altTrees[i].trees[j].goCrossFade != null && altTrees[i].trees[j].currentCrossFadeLOD != -2)
                                {
                                    if (Application.isPlaying)
                                        Destroy(altTrees[i].trees[j].goCrossFade);
                                    else
                                        DestroyImmediate(altTrees[i].trees[j].goCrossFade);
                                }
                            }
                        }
                        altTrees[i].trees = null;
                        if (altTrees[i].treesNoGroup != null)
                        {
                            for (int j = 0; j < altTrees[i].treesNoGroup.Length; j++)
                            {
                                if (altTrees[i].treesNoGroup[j] != null && altTrees[i].treesNoGroup[j].go != null && altTrees[i].treesNoGroup[j].currentLOD != -2)
                                {
                                    if (Application.isPlaying)
                                        Destroy(altTrees[i].treesNoGroup[j].go);
                                    else
                                        DestroyImmediate(altTrees[i].treesNoGroup[j].go);
                                }
                                if (altTrees[i].treesNoGroup[j] != null && altTrees[i].treesNoGroup[j].goCrossFade != null && altTrees[i].treesNoGroup[j].currentCrossFadeLOD != -2)
                                {
                                    if (Application.isPlaying)
                                        Destroy(altTrees[i].treesNoGroup[j].goCrossFade);
                                    else
                                        DestroyImmediate(altTrees[i].treesNoGroup[j].goCrossFade);
                                }
                            }
                        }
                        altTrees[i].treesNoGroup = null;
                        if (altTrees[i].rendersDebug != null)
                        {
                            for (int j = 0; j < altTrees[i].rendersDebug.Count; j++)
                            {
                                DestroyImmediate(altTrees[i].rendersDebug[j]);
                            }
                            altTrees[i].rendersDebug.Clear();
                        }

                        if (quads != null)
                        {
                            if (quads.Length > i && quads[i] != null)
                            {
                                quads[i].removeRenders();
                                altTrees[i] = null;
                                quads[i] = null;
                            }
                        }
                    }
                }

                for (int key = 0; key < treesPoolArray.Length; key++)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(treesPoolArray[key].materialBillboard);
                        Destroy(treesPoolArray[key].materialBillboardGroup);
                    }
                    else
                    {
                        DestroyImmediate(treesPoolArray[key].materialBillboard);
                        DestroyImmediate(treesPoolArray[key].materialBillboardGroup);
                    }

                    if (Application.isPlaying)
                    {
                        Destroy(treesPoolArray[key].materialBillboardCrossFade);
                    }
                    else
                    {
                        DestroyImmediate(treesPoolArray[key].materialBillboardCrossFade);
                    }

                    for (int j = 0; j < treesPoolArray[key].objsArray.Length; j++)
                    {
                        for (int k = 0; k < treesPoolArray[key].objsArray[j].materialsMesh.Length; k++)
                        {
                            if (Application.isPlaying)
                            {
                                Destroy(treesPoolArray[key].objsArray[j].materialsMesh[k]);
                                Destroy(treesPoolArray[key].objsArray[j].materialsMeshCrossFade[k]);
                            }
                            else
                            {
                                DestroyImmediate(treesPoolArray[key].objsArray[j].materialsMesh[k]);
                                DestroyImmediate(treesPoolArray[key].objsArray[j].materialsMeshCrossFade[k]);
                            }
                        }
                    }
                }


                if (Application.isPlaying)
                {
                    for (int key = 0; key < treesPoolArray.Length; key++)
                    {
                        for (int j = 0; j < treesPoolArray[key].objsArray.Length; j++)
                        {
                            for (int k = 0; k < treesPoolArray[key].objsArray[j].objs.Count; k++)
                            {
                                if (treesPoolArray[key].objsArray[j].objs[k] != null)
                                    DestroyImmediate(treesPoolArray[key].objsArray[j].objs[k]);
                            }
                        }
                        for (int j = 0; j < treesPoolArray[key].colliderBillboardsArray.Count; j++)
                        {
                            if (treesPoolArray[key].colliderBillboardsArray[j] != null)
                                DestroyImmediate(treesPoolArray[key].colliderBillboardsArray[j]);
                        }
                        for (int j = 0; j < treesPoolArray[key].collidersArray.Count; j++)
                        {
                            if (treesPoolArray[key].collidersArray[j] != null)
                                DestroyImmediate(treesPoolArray[key].collidersArray[j]);
                        }
                    }

                    for (int h = 0; h < objBillboardsPool.Count; h++)
                    {
                        objBillboardsPool[h].mr = null;
                        Destroy(objBillboardsPool[h].ms);
                        Destroy(objBillboardsPool[h].go);
                    }
                    foreach (GameObject key in objBillboardsUsedPool.Keys)
                    {
                        if (objBillboardsUsedPool[key].go != null)
                            Destroy(objBillboardsUsedPool[key].go);
                        if (objBillboardsUsedPool[key].ms != null)
                            Destroy(objBillboardsUsedPool[key].ms);
                    }

                    Destroy(goCubeDebug.GetComponent<MeshRenderer>().sharedMaterial);
                    Destroy(goCubeDebug);
                }
                else
                {
                    DestroyImmediate(goCubeDebug.GetComponent<MeshRenderer>().sharedMaterial);
                    DestroyImmediate(goCubeDebug);
                }


                AltTreesQuad.objsToInit.Clear();
                isDestroyed = true;

                if (destroyThis)
                    DestroyImmediate(this.gameObject);

            }
        }

        public void removeAltTrees(AltTreesPatch altT, bool delPrototypes = true)
        {
            int idAltT = -1;
            for (int i = 0; i < altTrees.Length; i++)
            {
                if (altTrees[i] != null)
                {
                    if (altTrees[i].Equals(altT))
                    {
                        idAltT = i;
                        break;
                    }
                }
            }
            if (idAltT != -1)
            {
                if (altTrees[idAltT].trees != null)
                {
                    for (int j = 0; j < altTrees[idAltT].trees.Length; j++)
                    {
                        if (altTrees[idAltT].trees[j] != null && altTrees[idAltT].trees[j].go != null)
                            DestroyImmediate(altTrees[idAltT].trees[j].go);
                    }
                }

                if (altTrees[idAltT].rendersDebug != null)
                {
                    for (int j = 0; j < altTrees[idAltT].rendersDebug.Count; j++)
                    {
                        DestroyImmediate(altTrees[idAltT].rendersDebug[j]);
                    }
                    altTrees[idAltT].rendersDebug.Clear();
                }

                if (Application.isPlaying)
                {
                    for (int key = 0; key < treesPoolArray.Length; key++)
                    {
                        for (int j = 0; j < treesPoolArray[key].objsArray.Length; j++)
                        {
                            for (int k = 0; k < treesPoolArray[key].objsArray[j].objs.Count; k++)
                            {
                                if (treesPoolArray[key].objsArray[j].objs[k] != null)
                                    DestroyImmediate(treesPoolArray[key].objsArray[j].objs[k]);
                            }
                        }
                    }
                }

                if (quads[idAltT] != null)
                {
                    quads[idAltT].removeRenders();
                    altTrees[idAltT] = null;
                    quads[idAltT] = null;
                }

            }
        }

        public void addTrees(AddTreesStruct[] positions, int idAltTree, float rotation, float height, float width, Color hueLeaves, Color hueBark)
        {
            addTrees(positions, idAltTree, false, false, height, rotation, false, false, width, 0f, hueLeaves, hueBark, false, false, true);
        }

        public void addTrees(AddTreesStruct[] positions, int idAltTree, bool randomRotation, bool isRandomHeight, float height, float heightRandom,
                            bool lockWidthToHeight, bool isRandomWidth, float width, float widthRandom, Color hueLeaves, Color hueBark, bool isRandomHueLeaves, bool isRandomHueBark, bool isTranslate = false)
        {
            if (altTrees.Length > idAltTree && altTrees[idAltTree] != null)
            {
                if (positions.Length > 0)
                {
                    float rotationTemp = 0f;
                    float heightTemp = 0f;
                    float widthTemp = 0f;

                    float hueLeavesA = hueLeaves.a;
                    float hueBarkA = hueBark.a;

                    int treesBillb = 0;
                    int treesNoBillb = 0;
                    int treesBillbSch = 0;
                    int treesNoBillbSch = 0;

                    for (int i = 0; i < positions.Length; i++)
                    {
                        if (!positions[i].altTree.isObject)
                            treesBillb++;
                        else
                            treesNoBillb++;
                    }

                    AddTreesStruct[] treesBillbArr = new AddTreesStruct[treesBillb];
                    AddTreesStruct[] treesNoBillbArr = new AddTreesStruct[treesNoBillb];

                    for (int i = 0; i < positions.Length; i++)
                    {
                        if (!positions[i].altTree.isObject)
                        {
                            treesBillbArr[treesBillbSch] = positions[i];
                            treesBillbSch++;
                        }
                        else
                        {
                            treesNoBillbArr[treesNoBillbSch] = positions[i];
                            treesNoBillbSch++;
                        }
                    }
                    treesBillbSch = 0;
                    treesNoBillbSch = 0;


                    int countTemp = 0;


                    if (altTrees[idAltTree].trees == null)
                        altTrees[idAltTree].trees = new AltTreesTrees[0];
                    if (altTrees[idAltTree].treesNoGroup == null)
                        altTrees[idAltTree].treesNoGroup = new AltTreesTrees[0];

                    int countEmptyRemove = 0;
                    List<int> changedTrees = new List<int>();
                    List<int> changedTreesNoGroup = new List<int>();
                    int addedTreesCount = 0;
                    int addedTreesNoGroupCount = 0;

                    if (treesBillb > 0)
                    {
                        countEmptyRemove = 0;

                        for (int j = 0; j < altTrees[idAltTree].treesEmptyCount; j++)
                        {
                            if (!isTranslate)
                            {
                                rotationTemp = randomRotation ? Random.value * 360f : 0f;
                                heightTemp = isRandomHeight ? height + Random.value * heightRandom : height;

                                if (lockWidthToHeight)
                                    widthTemp = heightTemp;
                                else
                                    widthTemp = isRandomWidth ? width + Random.value * widthRandom : width;
                            }
                            else
                            {
                                rotationTemp = heightRandom;
                                heightTemp = height;
                                widthTemp = width;
                            }

                            if (isRandomHueLeaves)
                                hueLeaves.a = Random.value * hueLeavesA;
                            if (isRandomHueBark)
                                hueBark.a = Random.value * hueBarkA;

                            altTrees[idAltTree].trees[altTrees[idAltTree].treesEmpty[j]] = new AltTreesTrees(altTrees[idAltTree].getTreePosLocal(treesBillbArr[treesBillbSch].pos, jump, altTreesMain.altTreesManagerData.sizePatch), altTrees[idAltTree].treesEmpty[j], treesBillbArr[treesBillbSch].altTree.id, hueLeaves, hueBark, Color.white, rotationTemp, heightTemp, widthTemp, altTrees[idAltTree]);
                            altTrees[idAltTree].trees[altTrees[idAltTree].treesEmpty[j]].idPrototypeIndex = getPrototypeIndex(altTrees[idAltTree].trees[altTrees[idAltTree].treesEmpty[j]].idPrototype);
                            quads[idAltTree].checkTreesAdd(altTrees[idAltTree].trees[altTrees[idAltTree].treesEmpty[j]].getPosWorld().x, altTrees[idAltTree].trees[altTrees[idAltTree].treesEmpty[j]].getPosWorld().z, altTrees[idAltTree].trees[altTrees[idAltTree].treesEmpty[j]]);
                            countTemp++;
                            treesBillbSch++;
                            countEmptyRemove++;
                            changedTrees.Add(altTrees[idAltTree].treesEmpty[j]);

                            if (treesBillbSch >= treesBillb)
                                break;
                        }
                        int[] treesEmptyTemp = altTrees[idAltTree].treesEmpty;
                        altTrees[idAltTree].treesEmptyCount -= countEmptyRemove;
                        altTrees[idAltTree].treesEmpty = new int[altTrees[idAltTree].treesEmptyCount];
                        for(int i = countEmptyRemove; i < treesEmptyTemp.Length; i++)
                        {
                            altTrees[idAltTree].treesEmpty[i - countEmptyRemove] = treesEmptyTemp[i];
                        }
                    }
                    if (treesNoBillb > 0)
                    {
                        countEmptyRemove = 0;

                        for (int j = 0; j < altTrees[idAltTree].treesNoGroupEmptyCount; j++)
                        {
                            if (!isTranslate)
                            {
                                rotationTemp = randomRotation ? Random.value * 360f : 0f;
                                heightTemp = isRandomHeight ? height + Random.value * heightRandom : height;

                                if (lockWidthToHeight)
                                    widthTemp = heightTemp;
                                else
                                    widthTemp = isRandomWidth ? width + Random.value * widthRandom : width;
                            }
                            else
                            {
                                rotationTemp = heightRandom;
                                heightTemp = height;
                                widthTemp = width;
                            }

                            if (isRandomHueLeaves)
                                hueLeaves.a = Random.value * hueLeavesA;
                            if (isRandomHueBark)
                                hueBark.a = Random.value * hueBarkA;

                            altTrees[idAltTree].treesNoGroup[altTrees[idAltTree].treesNoGroupEmpty[j]] = new AltTreesTrees(altTrees[idAltTree].getTreePosLocal(treesNoBillbArr[treesNoBillbSch].pos, jump, altTreesMain.altTreesManagerData.sizePatch), altTrees[idAltTree].treesNoGroupEmpty[j], treesNoBillbArr[treesNoBillbSch].altTree.id, hueLeaves, hueBark, Color.white, rotationTemp, heightTemp, widthTemp, altTrees[idAltTree]);
                            altTrees[idAltTree].treesNoGroup[altTrees[idAltTree].treesNoGroupEmpty[j]].idPrototypeIndex = getPrototypeIndex(altTrees[idAltTree].treesNoGroup[altTrees[idAltTree].treesNoGroupEmpty[j]].idPrototype);
                            quads[idAltTree].checkTreesAdd(altTrees[idAltTree].treesNoGroup[altTrees[idAltTree].treesNoGroupEmpty[j]].getPosWorld().x, altTrees[idAltTree].treesNoGroup[altTrees[idAltTree].treesNoGroupEmpty[j]].getPosWorld().z, altTrees[idAltTree].treesNoGroup[altTrees[idAltTree].treesNoGroupEmpty[j]], false);
                            countTemp++;
                            treesNoBillbSch++;
                            countEmptyRemove++;
                            changedTreesNoGroup.Add(altTrees[idAltTree].treesNoGroupEmpty[j]);

                            if (treesNoBillbSch >= treesNoBillb)
                                break;
                        }
                        int[] treesEmptyTemp = altTrees[idAltTree].treesNoGroupEmpty;
                        altTrees[idAltTree].treesNoGroupEmptyCount -= countEmptyRemove;
                        altTrees[idAltTree].treesNoGroupEmpty = new int[altTrees[idAltTree].treesNoGroupEmptyCount];
                        for (int i = countEmptyRemove; i < treesEmptyTemp.Length; i++)
                        {
                            altTrees[idAltTree].treesNoGroupEmpty[i - countEmptyRemove] = treesEmptyTemp[i];
                        }
                    }

                    if (treesBillbSch < treesBillb)
                    {
                        AltTreesTrees[] treesTemp = altTrees[idAltTree].trees;
                        altTrees[idAltTree].trees = new AltTreesTrees[treesTemp.Length + (treesBillb - treesBillbSch)];
                        addedTreesCount = treesBillb - treesBillbSch;
                        altTrees[idAltTree].treesCount = altTrees[idAltTree].trees.Length;

                        for (int i = 0; i < treesTemp.Length; i++)
                        {
                            altTrees[idAltTree].trees[i] = treesTemp[i];
                        }

                        int treesBillbSch2 = treesBillbSch;

                        for (int i = 0; i < (treesBillb - treesBillbSch); i++)
                        {
                            if (!isTranslate)
                            {
                                rotationTemp = randomRotation ? Random.value * 360f : 0f;
                                heightTemp = isRandomHeight ? height + Random.value * heightRandom : height;

                                if (lockWidthToHeight)
                                    widthTemp = heightTemp;
                                else
                                    widthTemp = isRandomWidth ? width + Random.value * widthRandom : width;
                            }
                            else
                            {
                                rotationTemp = heightRandom;
                                heightTemp = height;
                                widthTemp = width;
                            }

                            if (isRandomHueLeaves)
                                hueLeaves.a = Random.value * hueLeavesA;
                            if (isRandomHueBark)
                                hueBark.a = Random.value * hueBarkA;

                            altTrees[idAltTree].trees[treesTemp.Length + i] = new AltTreesTrees(altTrees[idAltTree].getTreePosLocal(treesBillbArr[treesBillbSch2].pos, jump, altTreesMain.altTreesManagerData.sizePatch), treesTemp.Length + i, treesBillbArr[treesBillbSch2].altTree.id, hueLeaves, hueBark, Color.white, rotationTemp, heightTemp, widthTemp, altTrees[idAltTree]);
                            altTrees[idAltTree].trees[treesTemp.Length + i].idPrototypeIndex = getPrototypeIndex(altTrees[idAltTree].trees[treesTemp.Length + i].idPrototype);
                            quads[idAltTree].checkTreesAdd(altTrees[idAltTree].trees[treesTemp.Length + i].getPosWorld().x, altTrees[idAltTree].trees[treesTemp.Length + i].getPosWorld().z, altTrees[idAltTree].trees[treesTemp.Length + i]);

                            treesBillbSch2++;
                        }
                    }

                    if (treesNoBillbSch < treesNoBillb)
                    {
                        AltTreesTrees[] treesTemp = altTrees[idAltTree].treesNoGroup;
                        altTrees[idAltTree].treesNoGroup = new AltTreesTrees[treesTemp.Length + (treesNoBillb - treesNoBillbSch)];
                        addedTreesNoGroupCount = treesNoBillb - treesNoBillbSch;
                        altTrees[idAltTree].treesNoGroupCount = altTrees[idAltTree].treesNoGroup.Length;

                        for (int i = 0; i < treesTemp.Length; i++)
                        {
                            altTrees[idAltTree].treesNoGroup[i] = treesTemp[i];
                        }

                        int treesNoBillbSch2 = treesNoBillbSch;

                        for (int i = 0; i < (treesNoBillb - treesNoBillbSch); i++)
                        {
                            if (!isTranslate)
                            {
                                rotationTemp = randomRotation ? Random.value * 360f : 0f;
                                heightTemp = isRandomHeight ? height + Random.value * heightRandom : height;

                                if (lockWidthToHeight)
                                    widthTemp = heightTemp;
                                else
                                    widthTemp = isRandomWidth ? width + Random.value * widthRandom : width;
                            }
                            else
                            {
                                rotationTemp = heightRandom;
                                heightTemp = height;
                                widthTemp = width;
                            }

                            if (isRandomHueLeaves)
                                hueLeaves.a = Random.value * hueLeavesA;
                            if (isRandomHueBark)
                                hueBark.a = Random.value * hueBarkA;

                            altTrees[idAltTree].treesNoGroup[treesTemp.Length + i] = new AltTreesTrees(altTrees[idAltTree].getTreePosLocal(treesNoBillbArr[treesNoBillbSch2].pos, jump, altTreesMain.altTreesManagerData.sizePatch), treesTemp.Length + i, treesNoBillbArr[treesNoBillbSch2].altTree.id, hueLeaves, hueBark, Color.white, rotationTemp, heightTemp, widthTemp, altTrees[idAltTree]);
                            altTrees[idAltTree].treesNoGroup[treesTemp.Length + i].idPrototypeIndex = getPrototypeIndex(altTrees[idAltTree].treesNoGroup[treesTemp.Length + i].idPrototype);
                            quads[idAltTree].checkTreesAdd(altTrees[idAltTree].treesNoGroup[treesTemp.Length + i].getPosWorld().x, altTrees[idAltTree].treesNoGroup[treesTemp.Length + i].getPosWorld().z, altTrees[idAltTree].treesNoGroup[treesTemp.Length + i], false);

                            treesNoBillbSch2++;
                        }
                    }

                    if(treesBillb > 0)
                        altTrees[idAltTree].EditDataFile(false, changedTrees, addedTreesCount, null);
                    if (treesNoBillb > 0)
                        altTrees[idAltTree].EditDataFile(true, changedTreesNoGroup, addedTreesNoGroupCount, null);

                    altTrees[idAltTree].recalculateBound();
                }
                else
                    Debug.LogError("positions.Length == 0");
            }
            else
                Debug.LogError("altTrees.Length<=idAltTree || altTrees[idAltTree] == null");
        }

        public AltTreesTrees copyTree(int idAltTree, int idTree, GameObject go, bool isTrees)
        {
            if (isTrees)
            {
                AltTreesTrees[] treesTemp = altTrees[idAltTree].trees;
                altTrees[idAltTree].trees = new AltTreesTrees[treesTemp.Length + 1];
                altTrees[idAltTree].treesCount = altTrees[idAltTree].trees.Length;


                for (int i = 0; i < treesTemp.Length; i++)
                {
                    altTrees[idAltTree].trees[i] = treesTemp[i];
                }

                altTrees[idAltTree].trees[altTrees[idAltTree].trees.Length - 1] = new AltTreesTrees(altTrees[idAltTree].trees[idTree], altTrees[idAltTree].trees.Length - 1);
                altTrees[idAltTree].trees[altTrees[idAltTree].trees.Length - 1].go = go;
                altTrees[idAltTree].trees[altTrees[idAltTree].trees.Length - 1].idPrototypeIndex = getPrototypeIndex(altTrees[idAltTree].trees[altTrees[idAltTree].trees.Length - 1].idPrototype);

                altTrees[idAltTree].EditDataFile(false, null, 1, null);

                return altTrees[idAltTree].trees[altTrees[idAltTree].trees.Length - 1];
            }
            else
            {
                AltTreesTrees[] treesTemp = altTrees[idAltTree].treesNoGroup;
                altTrees[idAltTree].treesNoGroup = new AltTreesTrees[treesTemp.Length + 1];
                altTrees[idAltTree].treesNoGroupCount = altTrees[idAltTree].treesNoGroup.Length;

                for (int i = 0; i < treesTemp.Length; i++)
                {
                    altTrees[idAltTree].treesNoGroup[i] = treesTemp[i];
                }

                altTrees[idAltTree].treesNoGroup[altTrees[idAltTree].treesNoGroup.Length - 1] = new AltTreesTrees(altTrees[idAltTree].treesNoGroup[idTree], altTrees[idAltTree].treesNoGroup.Length - 1);
                altTrees[idAltTree].treesNoGroup[altTrees[idAltTree].treesNoGroup.Length - 1].go = go;
                altTrees[idAltTree].treesNoGroup[altTrees[idAltTree].treesNoGroup.Length - 1].idPrototypeIndex = getPrototypeIndex(altTrees[idAltTree].treesNoGroup[altTrees[idAltTree].treesNoGroup.Length - 1].idPrototype);

                altTrees[idAltTree].EditDataFile(true, null, 1, null);

                return altTrees[idAltTree].treesNoGroup[altTrees[idAltTree].treesNoGroup.Length - 1];
            }
        }

        public void addTreesImport(ImportTreesStruct[] trees, int idAltTree)
        {
            if (altTrees.Length > idAltTree && altTrees[idAltTree] != null)
            {
                if (trees.Length > 0)
                {

                    int treesBillb = 0;
                    int treesNoBillb = 0;
                    int treesBillbSch = 0;
                    int treesNoBillbSch = 0;

                    for (int i = 0; i < trees.Length; i++)
                    {
                        if (!trees[i].isObject)
                            treesBillb++;
                        else
                            treesNoBillb++;
                    }

                    ImportTreesStruct[] treesBillbArr = new ImportTreesStruct[treesBillb];
                    ImportTreesStruct[] treesNoBillbArr = new ImportTreesStruct[treesNoBillb];

                    for (int i = 0; i < trees.Length; i++)
                    {
                        if (!trees[i].isObject)
                        {
                            treesBillbArr[treesBillbSch] = trees[i];
                            treesBillbSch++;
                        }
                        else
                        {
                            treesNoBillbArr[treesNoBillbSch] = trees[i];
                            treesNoBillbSch++;
                        }
                    }
                    treesBillbSch = 0;
                    treesNoBillbSch = 0;


                    int countTemp = 0;


                    if (altTrees[idAltTree].trees == null)
                        altTrees[idAltTree].trees = new AltTreesTrees[0];
                    if (altTrees[idAltTree].treesNoGroup == null)
                        altTrees[idAltTree].treesNoGroup = new AltTreesTrees[0];


                    int countEmptyRemove = 0;
                    List<int> changedTrees = new List<int>();
                    List<int> changedTreesNoGroup = new List<int>();
                    int addedTreesCount = 0;
                    int addedTreesNoGroupCount = 0;



                    if (treesBillb > 0)
                    {
                        for (int j = 0; j < altTrees[idAltTree].treesEmptyCount; j++)
                        {
                            altTrees[idAltTree].trees[altTrees[idAltTree].treesEmpty[j]] = new AltTreesTrees(altTrees[idAltTree].getTreePosLocal(treesBillbArr[treesBillbSch].pos, jump, altTreesMain.altTreesManagerData.sizePatch), altTrees[idAltTree].treesEmpty[j], treesBillbArr[treesBillbSch].altTree.id, treesBillbArr[treesBillbSch].color, treesBillbArr[treesBillbSch].colorBark, treesBillbArr[treesBillbSch].lightmapColor, treesBillbArr[treesBillbSch].rotation, treesBillbArr[treesBillbSch].heightScale, treesBillbArr[treesBillbSch].widthScale, altTrees[idAltTree]);
                            altTrees[idAltTree].trees[altTrees[idAltTree].treesEmpty[j]].idPrototypeIndex = getPrototypeIndex(altTrees[idAltTree].trees[altTrees[idAltTree].treesEmpty[j]].idPrototype);
                            quads[idAltTree].checkTreesAdd(altTrees[idAltTree].trees[altTrees[idAltTree].treesEmpty[j]].getPosWorld().x, altTrees[idAltTree].trees[altTrees[idAltTree].treesEmpty[j]].getPosWorld().z, altTrees[idAltTree].trees[altTrees[idAltTree].treesEmpty[j]]);
                            countTemp++;
                            treesBillbSch++;
                            countEmptyRemove++;
                            changedTrees.Add(altTrees[idAltTree].treesEmpty[j]);

                            if (treesBillbSch >= treesBillb)
                                break;
                        }
                        int[] treesEmptyTemp = altTrees[idAltTree].treesEmpty;
                        altTrees[idAltTree].treesEmptyCount -= countEmptyRemove;
                        altTrees[idAltTree].treesEmpty = new int[altTrees[idAltTree].treesEmptyCount];
                        for (int i = countEmptyRemove; i < treesEmptyTemp.Length; i++)
                        {
                            altTrees[idAltTree].treesEmpty[i - countEmptyRemove] = treesEmptyTemp[i];
                        }
                    }
                    if (treesNoBillb > 0)
                    {
                        countEmptyRemove = 0;
                        for (int j = 0; j < altTrees[idAltTree].treesNoGroupEmptyCount; j++)
                        {
                            altTrees[idAltTree].treesNoGroup[altTrees[idAltTree].treesNoGroupEmpty[j]] = new AltTreesTrees(altTrees[idAltTree].getTreePosLocal(treesNoBillbArr[treesNoBillbSch].pos, jump, altTreesMain.altTreesManagerData.sizePatch), altTrees[idAltTree].treesNoGroupEmpty[j], treesNoBillbArr[treesNoBillbSch].altTree.id, treesNoBillbArr[treesNoBillbSch].color, treesNoBillbArr[treesNoBillbSch].colorBark, treesNoBillbArr[treesNoBillbSch].lightmapColor, treesNoBillbArr[treesNoBillbSch].rotation, treesNoBillbArr[treesNoBillbSch].heightScale, treesNoBillbArr[treesNoBillbSch].widthScale, altTrees[idAltTree]);
                            altTrees[idAltTree].treesNoGroup[altTrees[idAltTree].treesNoGroupEmpty[j]].idPrototypeIndex = getPrototypeIndex(altTrees[idAltTree].treesNoGroup[altTrees[idAltTree].treesNoGroupEmpty[j]].idPrototype);
                            quads[idAltTree].checkTreesAdd(altTrees[idAltTree].treesNoGroup[altTrees[idAltTree].treesNoGroupEmpty[j]].getPosWorld().x, altTrees[idAltTree].treesNoGroup[altTrees[idAltTree].treesNoGroupEmpty[j]].getPosWorld().z, altTrees[idAltTree].treesNoGroup[altTrees[idAltTree].treesNoGroupEmpty[j]], false);
                            countTemp++;
                            treesNoBillbSch++;
                            countEmptyRemove++;
                            changedTreesNoGroup.Add(altTrees[idAltTree].treesNoGroupEmpty[j]);

                            if (treesNoBillbSch >= treesNoBillb)
                                break;
                        }
                        int[] treesEmptyTemp = altTrees[idAltTree].treesNoGroupEmpty;
                        altTrees[idAltTree].treesNoGroupEmptyCount -= countEmptyRemove;
                        altTrees[idAltTree].treesNoGroupEmpty = new int[altTrees[idAltTree].treesNoGroupEmptyCount];
                        for (int i = countEmptyRemove; i < treesEmptyTemp.Length; i++)
                        {
                            altTrees[idAltTree].treesNoGroupEmpty[i - countEmptyRemove] = treesEmptyTemp[i];
                        }
                    }

                    if (treesBillbSch < treesBillb)
                    {
                        AltTreesTrees[] treesTemp = altTrees[idAltTree].trees;
                        altTrees[idAltTree].trees = new AltTreesTrees[treesTemp.Length + (treesBillb - treesBillbSch)];
                        addedTreesCount = treesBillb - treesBillbSch;
                        altTrees[idAltTree].treesCount = altTrees[idAltTree].trees.Length;

                        for (int i = 0; i < treesTemp.Length; i++)
                        {
                            altTrees[idAltTree].trees[i] = treesTemp[i];
                        }

                        int treesBillbSch2 = treesBillbSch;

                        for (int i = 0; i < (treesBillb - treesBillbSch); i++)
                        {
                            altTrees[idAltTree].trees[treesTemp.Length + i] = new AltTreesTrees(altTrees[idAltTree].getTreePosLocal(treesBillbArr[treesBillbSch2].pos, jump, altTreesMain.altTreesManagerData.sizePatch), treesTemp.Length + i, treesBillbArr[treesBillbSch2].altTree.id, treesBillbArr[treesBillbSch2].color, treesBillbArr[treesBillbSch2].colorBark, treesBillbArr[treesBillbSch2].lightmapColor, treesBillbArr[treesBillbSch2].rotation, treesBillbArr[treesBillbSch2].heightScale, treesBillbArr[treesBillbSch2].widthScale, altTrees[idAltTree]);
                            altTrees[idAltTree].trees[treesTemp.Length + i].idPrototypeIndex = getPrototypeIndex(altTrees[idAltTree].trees[treesTemp.Length + i].idPrototype);
                            quads[idAltTree].checkTreesAdd(altTrees[idAltTree].trees[treesTemp.Length + i].getPosWorld().x, altTrees[idAltTree].trees[treesTemp.Length + i].getPosWorld().z, altTrees[idAltTree].trees[treesTemp.Length + i]);

                            treesBillbSch2++;
                        }
                    }

                    if (treesNoBillbSch < treesNoBillb)
                    {
                        AltTreesTrees[] treesTemp = altTrees[idAltTree].treesNoGroup;
                        altTrees[idAltTree].treesNoGroup = new AltTreesTrees[treesTemp.Length + (treesNoBillb - treesNoBillbSch)];
                        addedTreesNoGroupCount = treesNoBillb - treesNoBillbSch;
                        altTrees[idAltTree].treesNoGroupCount = altTrees[idAltTree].treesNoGroup.Length;

                        for (int i = 0; i < treesTemp.Length; i++)
                        {
                            altTrees[idAltTree].treesNoGroup[i] = treesTemp[i];
                        }

                        int treesNoBillbSch2 = treesNoBillbSch;

                        for (int i = 0; i < (treesNoBillb - treesNoBillbSch); i++)
                        {
                            altTrees[idAltTree].treesNoGroup[treesTemp.Length + i] = new AltTreesTrees(altTrees[idAltTree].getTreePosLocal(treesNoBillbArr[treesNoBillbSch2].pos, jump, altTreesMain.altTreesManagerData.sizePatch), treesTemp.Length + i, treesNoBillbArr[treesNoBillbSch2].altTree.id, treesNoBillbArr[treesNoBillbSch2].color, treesNoBillbArr[treesNoBillbSch2].colorBark, treesNoBillbArr[treesNoBillbSch2].lightmapColor, treesNoBillbArr[treesNoBillbSch2].rotation, treesNoBillbArr[treesNoBillbSch2].heightScale, treesNoBillbArr[treesNoBillbSch2].widthScale, altTrees[idAltTree]);
                            altTrees[idAltTree].treesNoGroup[treesTemp.Length + i].idPrototypeIndex = getPrototypeIndex(altTrees[idAltTree].treesNoGroup[treesTemp.Length + i].idPrototype);
                            quads[idAltTree].checkTreesAdd(altTrees[idAltTree].treesNoGroup[treesTemp.Length + i].getPosWorld().x, altTrees[idAltTree].treesNoGroup[treesTemp.Length + i].getPosWorld().z, altTrees[idAltTree].treesNoGroup[treesTemp.Length + i], false);

                            treesNoBillbSch2++;
                        }
                    }

                    if (treesBillb > 0)
                        altTrees[idAltTree].EditDataFile(false, changedTrees, addedTreesCount, null);
                    if (treesNoBillb > 0)
                        altTrees[idAltTree].EditDataFile(true, changedTreesNoGroup, addedTreesNoGroupCount, null);


                    altTrees[idAltTree].recalculateBound();
                }
                else
                    Debug.LogError("positions.Length == 0");
            }
            else
                Debug.LogError("altTrees.Length<=idAltTree || altTrees[idAltTree] == null");
        }
    }


    public class AltTreesQuad
    {
        AltTreesManager manager;
        public Vector2 pos;
        public int altTreesID = -1;
        public int LOD = -1;
        public int maxLOD = -1;
        public int startBillboardsLOD = -1;
        public float size = -1;
        public float sizeSQR = -1;
        public bool isActiv = false;
        public bool isRender = false;
        public List<GameObject> renders = new List<GameObject>();
        public GameObject rendersDebug = null;
        public List<AltTreesQuad> objs = new List<AltTreesQuad>();
        public bool isInit = false;
        static public List<AltTreesQuad> objsToInit = new List<AltTreesQuad>();
        public List<AltTreesTrees> trees = new List<AltTreesTrees>();
        public List<AltTreesTrees> treesNoBillb = new List<AltTreesTrees>();
        public Dictionary<int, int> treePrefabsCount = new Dictionary<int, int>();
        public int treesCount = 0;
        public int treesNoBillbCount = 0;
        public Bounds2D bound;

        public bool isLock = false;
        public bool isUpdate = false;
        public bool isGenerateAllBillboardsOnStart = false;

        public AltTreesQuad[] quads = new AltTreesQuad[4];
        public bool isChildQuads = false;
        int quadId = 0;

        public AltTreesQuad(float x, float z, float _size, int _altTreesID, int _LOD, int _maxLOD, int _startBillboardsLOD, AltTreesManager _manager, int _quadId = 0)
        {
            manager = _manager;
            pos = new Vector2(x, z);
            altTreesID = _altTreesID;
            LOD = _LOD;
            size = _size;
            sizeSQR = size * size;
            maxLOD = _maxLOD;
            startBillboardsLOD = _startBillboardsLOD;
            quadId = _quadId;

            bound = new Bounds2D(pos, size);

            if (manager.altTreesMain.altTreesManagerData.generateAllBillboardsOnStart && Application.isPlaying)
            {
                if (!objsToInit.Contains(this))
                {
                    objsToInit.Add(this);
                }
                isGenerateAllBillboardsOnStart = true;
            }

            if (LOD < maxLOD)
            {
                isChildQuads = true;
                quads[0] = new AltTreesQuad(x - size / 4f, z + size / 4f, size / 2f, altTreesID, LOD + 1, maxLOD, startBillboardsLOD, _manager, 1);
                quads[1] = new AltTreesQuad(x + size / 4f, z + size / 4f, size / 2f, altTreesID, LOD + 1, maxLOD, startBillboardsLOD, _manager, 2);
                quads[2] = new AltTreesQuad(x - size / 4f, z - size / 4f, size / 2f, altTreesID, LOD + 1, maxLOD, startBillboardsLOD, _manager, 3);
                quads[3] = new AltTreesQuad(x + size / 4f, z - size / 4f, size / 2f, altTreesID, LOD + 1, maxLOD, startBillboardsLOD, _manager, 4);
            }
        }
    

        bool _isNext = false;
        int idCheck = 0;

        public void check(Transform[] _cameras, bool _isOk, bool isFirst = false)
        {
            if (_isOk)
            {
                for (int c = 0; c < _cameras.Length; c++)
                {
                    _isNext = (AltUtilities.fastDistanceSqrt2D(ref pos, _cameras[c].position) <= (sizeSQR + sizeSQR) * manager.altTreesMain.altTreesManagerData.distancePatchFactor /* / ((float)maxLOD - (float)LOD))*/) ? true : false;

                    if (_isNext)
                        break;
                }

                if (isLock)
                {
                    _isNext = true;
                }

                if (_isNext && LOD < maxLOD)
                    isActiv = false;
                else
                    isActiv = true;
            }
            else
            {
                _isNext = false;
                isActiv = false;
            }

            if (isChildQuads)
            {
                if (isFirst)
                {
                    quads[idCheck].check(_cameras, _isNext);

                    idCheck++;
                    if (idCheck == 4)
                        idCheck = 0;
                }
                else
                {
                    quads[0].check(_cameras, _isNext);
                    quads[1].check(_cameras, _isNext);
                    quads[2].check(_cameras, _isNext);
                    quads[3].check(_cameras, _isNext);
                }
            }
        }

        public void lockQuads(Vector3 vect)
        {
            if (bound.inBounds(vect.x, vect.z, quadId))
            {
                isLock = true;

                if (isChildQuads)
                {
                    quads[0].lockQuads(vect);
                    quads[1].lockQuads(vect);
                    quads[2].lockQuads(vect);
                    quads[3].lockQuads(vect);
                }
            }
        }

        public void reInitBillboards(AltTreesTrees att, Vector3 vect, bool drawGroupBillboards = true)
        {

            if (bound.inBounds(att.getPosWorld().x, att.getPosWorld().z, quadId) && bound.inBounds(vect.x, vect.z, quadId))
            {
                if (drawGroupBillboards)
                    isUpdate = true;

                if (isChildQuads)
                {
                    quads[0].reInitBillboards(att, vect, drawGroupBillboards);
                    quads[1].reInitBillboards(att, vect, drawGroupBillboards);
                    quads[2].reInitBillboards(att, vect, drawGroupBillboards);
                    quads[3].reInitBillboards(att, vect, drawGroupBillboards);
                }
            }
            else if (bound.inBounds(att.getPosWorld().x, att.getPosWorld().z, quadId))
            {
                if (drawGroupBillboards)
                {
                    isUpdate = true;
                    if (!treePrefabsCount.ContainsKey(att.idPrototype))
                        Debug.Log("- " + att.idTree + ". " + att.idPrototype + ", " + trees.Count + ", " + treesCount + " = " + att.pos.ToString() + " = " + pos.ToString());
                    else if (treePrefabsCount[att.idPrototype] <= 0)
                        Debug.Log("+ " + att.idTree + ". " + treePrefabsCount[att.idPrototype] + ", " + trees.Count + ", " + treesCount + " = " + att.pos.ToString() + " = " + pos.ToString());
                    treePrefabsCount[att.idPrototype]--;
                    trees.Remove(att);
                    treesCount--;
                }
                else
                {
                    treesNoBillb.Remove(att);
                    treesNoBillbCount--;
                }

                if (isChildQuads)
                {
                    quads[0].reInitBillboards(att, vect, drawGroupBillboards);
                    quads[1].reInitBillboards(att, vect, drawGroupBillboards);
                    quads[2].reInitBillboards(att, vect, drawGroupBillboards);
                    quads[3].reInitBillboards(att, vect, drawGroupBillboards);
                }
            }
            else if (bound.inBounds(vect.x, vect.z, quadId))
            {
                if (drawGroupBillboards)
                {
                    isUpdate = true;
                    if (!treePrefabsCount.ContainsKey(att.idPrototype))
                        treePrefabsCount.Add(att.idPrototype, 0);
                    treePrefabsCount[att.idPrototype]++;
                    trees.Add(att);
                    treesCount++;
                }
                else
                {
                    treesNoBillb.Add(att);
                    treesNoBillbCount++;
                }

                if (isChildQuads)
                {
                    quads[0].reInitBillboards(att, vect, drawGroupBillboards);
                    quads[1].reInitBillboards(att, vect, drawGroupBillboards);
                    quads[2].reInitBillboards(att, vect, drawGroupBillboards);
                    quads[3].reInitBillboards(att, vect, drawGroupBillboards);
                }
            }


        }

        public void removeTree(AltTreesTrees att, bool isObject = true)
        {
            if (bound.inBounds(att.getPosWorld().x, att.getPosWorld().z, quadId))
            {
                if (!isObject)
                {
                    isUpdate = true;
                    treePrefabsCount[att.idPrototype]--;
                    trees.Remove(att);
                    treesCount--;
                }
                else
                {
                    treesNoBillb.Remove(att);
                    treesNoBillbCount--;
                }
            }


            if (isChildQuads)
            {
                quads[0].removeTree(att, isObject);
                quads[1].removeTree(att, isObject);
                quads[2].removeTree(att, isObject);
                quads[3].removeTree(att, isObject);
            }
        }
        
        public bool removeTrees(Vector2 _pos, float _radius, AltTreesPatch _at, List<int> removedTrees, List<int> removedTreesNoGroup, AltTree _tree = null)
        {
            bool result = false;
            if (AltUtilities.fastDistance2D(ref pos, ref _pos) <= _radius + size * 1.42f)
            {
                if (_tree != null)
                {
                    if (!_tree.isObject)
                    {
                        for (int i = treesCount - 1; i >= 0; i--)
                        {
                            if (trees[i].idPrototype == _tree.id)
                            {
                                if (AltUtilities.fastDistance2D(ref _pos, trees[i].get2DPosWorld()) <= _radius)
                                {
                                    if(!removedTrees.Contains(trees[i].idTree))
                                        removedTrees.Add(trees[i].idTree);
                                    _at.trees[trees[i].idTree] = null;
                                    isUpdate = true;
                                    treePrefabsCount[trees[i].idPrototype]--;

                                    deleteTreeCheckCrossFade(trees[i]);
                                    trees.Remove(trees[i]);
                                    treesCount--;
                                    result = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = treesNoBillbCount - 1; i >= 0; i--)
                        {
                            if (treesNoBillb[i].idPrototype == _tree.id)
                            {
                                if (AltUtilities.fastDistance2D(ref _pos, treesNoBillb[i].get2DPosWorld()) <= _radius)
                                {
                                    if (!removedTreesNoGroup.Contains(treesNoBillb[i].idTree))
                                        removedTreesNoGroup.Add(treesNoBillb[i].idTree);
                                    _at.treesNoGroup[treesNoBillb[i].idTree] = null;

                                    deleteTreeCheckCrossFade(treesNoBillb[i]);
                                    treesNoBillb.Remove(treesNoBillb[i]);
                                    treesNoBillbCount--;
                                    result = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int i = treesCount - 1; i >= 0; i--)
                    {
                        if (AltUtilities.fastDistance2D(ref _pos, trees[i].get2DPosWorld()) <= _radius)
                        {
                            if (!removedTrees.Contains(trees[i].idTree))
                                removedTrees.Add(trees[i].idTree);
                            _at.trees[trees[i].idTree] = null;
                            isUpdate = true;
                            treePrefabsCount[trees[i].idPrototype]--;

                            deleteTreeCheckCrossFade(trees[i]);
                            trees.Remove(trees[i]);
                            treesCount--;
                            result = true;
                        }
                    }
                    for (int i = treesNoBillbCount - 1; i >= 0; i--)
                    {
                        if (AltUtilities.fastDistance2D(ref _pos, treesNoBillb[i].get2DPosWorld()) <= _radius)
                        {
                            if (!removedTreesNoGroup.Contains(treesNoBillb[i].idTree))
                                removedTreesNoGroup.Add(treesNoBillb[i].idTree);
                            _at.treesNoGroup[treesNoBillb[i].idTree] = null;

                            deleteTreeCheckCrossFade(treesNoBillb[i]);
                            treesNoBillb.Remove(treesNoBillb[i]);
                            treesNoBillbCount--;
                            result = true;
                        }
                    }
                }


                if (isChildQuads)
                {
                    if (quads[0].removeTrees(_pos, _radius, _at, removedTrees, removedTreesNoGroup, _tree))
                        result = true;
                    if (quads[1].removeTrees(_pos, _radius, _at, removedTrees, removedTreesNoGroup, _tree))
                        result = true;
                    if (quads[2].removeTrees(_pos, _radius, _at, removedTrees, removedTreesNoGroup, _tree))
                        result = true;
                    if (quads[3].removeTrees(_pos, _radius, _at, removedTrees, removedTreesNoGroup, _tree))
                        result = true;
                }
            }
            return result;
        }

        public bool removeTrees(AltTreesTrees[] att, Vector2 _pos, float sizeX, float sizeZ, AltTreesPatch _at, List<int> removedTrees, List<int> removedTreesNoGroup)
        {
            Bounds2D boundTemp = new Bounds2D(_pos.x, _pos.x + sizeX, _pos.y + sizeZ, _pos.y);
            bool result = false;
            if (bound.isIntersection(boundTemp))
            {
                for (int k = 0; k < att.Length; k++)
                {
                    for (int i = treesCount - 1; i >= 0; i--)
                    {
                        if (att[k].Equals(_at.trees[trees[i].idTree]))
                        {
                            if (!removedTrees.Contains(trees[i].idTree))
                                removedTrees.Add(trees[i].idTree);
                            _at.trees[trees[i].idTree] = null;
                            isUpdate = true;
                            treePrefabsCount[trees[i].idPrototype]--;
                            if (trees[i].go != null)
                                Object.DestroyImmediate(trees[i].go);
                            trees.Remove(trees[i]);
                            treesCount--;
                            result = true;
                            i = -1;
                        }
                    }
                    for (int i = treesNoBillbCount - 1; i >= 0; i--)
                    {
                        if (att[k].Equals(_at.treesNoGroup[treesNoBillb[i].idTree]))
                        {
                            if (!removedTreesNoGroup.Contains(treesNoBillb[i].idTree))
                                removedTreesNoGroup.Add(treesNoBillb[i].idTree);
                            _at.treesNoGroup[treesNoBillb[i].idTree] = null;
                            isUpdate = true;
                            if (treesNoBillb[i].go != null)
                                Object.DestroyImmediate(treesNoBillb[i].go);
                            treesNoBillb.Remove(treesNoBillb[i]);
                            treesNoBillbCount--;
                            result = true;
                            i = -1;
                        }
                    }
                }

                if (isChildQuads)
                {
                    if (quads[0].removeTrees(att, _pos, sizeX, sizeZ, _at, removedTrees, removedTreesNoGroup))
                        result = true;
                    if (quads[1].removeTrees(att, _pos, sizeX, sizeZ, _at, removedTrees, removedTreesNoGroup))
                        result = true;
                    if (quads[2].removeTrees(att, _pos, sizeX, sizeZ, _at, removedTrees, removedTreesNoGroup))
                        result = true;
                    if (quads[3].removeTrees(att, _pos, sizeX, sizeZ, _at, removedTrees, removedTreesNoGroup))
                        result = true;
                }
            }
            return result;
        }

        public void getTreesForExport(Vector2 _pos, float sizeX, float sizeZ, AltTreesPatch _at, List<AltTreesTrees> attTemp2)
        {
            if (LOD == maxLOD)
            {
                Bounds2D boundTemp = new Bounds2D(_pos.x, _pos.x + sizeX, _pos.y + sizeZ, _pos.y);
                List<AltTreesTrees> attTemp = new List<AltTreesTrees>();

                if (bound.isIntersection(boundTemp))
                {
                    for (int i = treesCount - 1; i >= 0; i--)
                    {
                        if (boundTemp.inBounds(trees[i].get2DPosWorld()))
                        {
                            attTemp.Add(trees[i]);
                            attTemp2.Add(trees[i]);
                        }
                    }
                    for (int i = treesNoBillbCount - 1; i >= 0; i--)
                    {
                        if (boundTemp.inBounds(treesNoBillb[i].get2DPosWorld()))
                        {
                            attTemp.Add(treesNoBillb[i]);
                            attTemp2.Add(treesNoBillb[i]);
                        }
                    }
                }
            }
            else
            {
                if (isChildQuads)
                {
                    quads[0].getTreesForExport(_pos, sizeX, sizeZ, _at, attTemp2);
                    quads[1].getTreesForExport(_pos, sizeX, sizeZ, _at, attTemp2);
                    quads[2].getTreesForExport(_pos, sizeX, sizeZ, _at, attTemp2);
                    quads[3].getTreesForExport(_pos, sizeX, sizeZ, _at, attTemp2);
                }
            }
        }

        public void unlockQuads()
        {
            isLock = false;

            if (isChildQuads)
            {
                quads[0].unlockQuads();
                quads[1].unlockQuads();
                quads[2].unlockQuads();
                quads[3].unlockQuads();
            }
        }

        public void goUpdateTrees()
        {
            if (isUpdate)
            {
                isInit = false;

                if (!objsToInit.Contains(this))
                {
                    objsToInit.Add(this);
                }
            }

            isUpdate = false;

            if (isChildQuads)
            {
                quads[0].goUpdateTrees();
                quads[1].goUpdateTrees();
                quads[2].goUpdateTrees();
                quads[3].goUpdateTrees();
            }
        }

        public void checkObjs(AltTreesQuad _obj, AltTreesPatch _altTrees, Transform[] _cameras)
        {
            objs.Clear();

            if (_obj == null)
            {
                if (isActiv)
                {
                    if (isRender)
                    {
                        if (LOD > startBillboardsLOD)
                            checkTrees(_cameras);
                        return;
                    }
                    else
                    {
                        if (isInit)
                        {
                            if (isChildQuads)
                            {
                                quads[0].checkObjs(this, _altTrees, _cameras);
                                quads[1].checkObjs(this, _altTrees, _cameras);
                                quads[2].checkObjs(this, _altTrees, _cameras);
                                quads[3].checkObjs(this, _altTrees, _cameras);
                            }
                        }
                        else
                        {
                            if (!objsToInit.Contains(this))
                            {
                                objsToInit.Add(this);
                            }
                            return;
                        }
                    }
                }
                else
                {
                    if (isRender)
                    {
                        if (isChildQuads)
                        {
                            quads[0].checkObjs(this, _altTrees, _cameras);
                            quads[1].checkObjs(this, _altTrees, _cameras);
                            quads[2].checkObjs(this, _altTrees, _cameras);
                            quads[3].checkObjs(this, _altTrees, _cameras);
                        }
                    }
                    else
                    {
                        if (isChildQuads)
                        {
                            quads[0].checkObjs(null, _altTrees, _cameras);
                            quads[1].checkObjs(null, _altTrees, _cameras);
                            quads[2].checkObjs(null, _altTrees, _cameras);
                            quads[3].checkObjs(null, _altTrees, _cameras);
                        }

                        return;
                    }
                }

                if (isActiv && isInit && !isRender)
                {
                    if (LOD <= startBillboardsLOD)
                    {
                        for (int i = 0; i < renders.Count; i++)
                        {
                            if (!manager.altTreesMain.altTreesManagerData.hideGroupBillboards)
                                renders[i].SetActive(true);
                        }
                    }
                    else
                        checkTrees(_cameras);

                    if (manager.altTreesMain.altTreesManagerData.drawDebugPutches)
                    {
                        if (!manager.altTreesMain.altTreesManagerData.hideGroupBillboards)
                            rendersDebug.SetActive(true);
                    }
                    for (int i = 0; i < objs.Count; i++)
                    {
                        for (int j = 0; j < objs[i].renders.Count; j++)
                        {
                            objs[i].renders[j].SetActive(false);
                        }
                        if (LOD <= startBillboardsLOD && objs[i].LOD > startBillboardsLOD)
                            objs[i].deleteTrees();
                        if (manager.altTreesMain.altTreesManagerData.drawDebugPutches)
                        {
                            objs[i].rendersDebug.SetActive(false);
                        }
                        objs[i].isRender = false;
                    }
                    isRender = true;
                }
                else if (!isActiv && isRender)
                {
                    _isNext = true;
                    for (int i = 0; i < objs.Count; i++)
                    {
                        if (!objs[i].isInit)
                        {
                            _isNext = false;
                            break;
                        }
                    }

                    if (_isNext)
                    {
                        for (int i = 0; i < objs.Count; i++)
                        {
                            if (objs[i].LOD > startBillboardsLOD)
                            {
                                objs[i].checkTrees(_cameras, true);
                            }
                            else
                            {
                                for (int j = 0; j < objs[i].renders.Count; j++)
                                {
                                    if (!manager.altTreesMain.altTreesManagerData.hideGroupBillboards)
                                        objs[i].renders[j].SetActive(true);
                                }
                            }


                            if (manager.altTreesMain.altTreesManagerData.drawDebugPutches)
                            {
                                if (!manager.altTreesMain.altTreesManagerData.hideGroupBillboards)
                                    objs[i].rendersDebug.SetActive(true);
                            }
                            objs[i].isRender = true;
                        }
                        for (int i = 0; i < renders.Count; i++)
                        {
                            renders[i].SetActive(false);
                        }
                        if (manager.altTreesMain.altTreesManagerData.drawDebugPutches)
                        {
                            rendersDebug.SetActive(false);
                        }
                        isRender = false;
                    }
                }
            }
            else
            {
                if (_obj.isActiv)
                {
                    if (isRender)
                    {
                        _obj.objs.Add(this);
                    }
                    else
                    {
                        if (isChildQuads)
                        {
                            quads[0].checkObjs(_obj, _altTrees, _cameras);
                            quads[1].checkObjs(_obj, _altTrees, _cameras);
                            quads[2].checkObjs(_obj, _altTrees, _cameras);
                            quads[3].checkObjs(_obj, _altTrees, _cameras);
                        }
                    }
                }
                else
                {
                    if (isActiv)
                    {
                        _obj.objs.Add(this);

                        if (!isInit)
                        {
                            if (!objsToInit.Contains(this))
                            {
                                objsToInit.Add(this);
                            }
                        }
                    }
                    else
                    {
                        if (isChildQuads)
                        {
                            quads[0].checkObjs(_obj, _altTrees, _cameras);
                            quads[1].checkObjs(_obj, _altTrees, _cameras);
                            quads[2].checkObjs(_obj, _altTrees, _cameras);
                            quads[3].checkObjs(_obj, _altTrees, _cameras);
                        }
                    }
                }
            }
        }

        public void checkDebugPutches(bool drawDebugPutches)
        {
            if (isActiv && rendersDebug != null)
            {
                rendersDebug.SetActive(drawDebugPutches);
            }

            if (isChildQuads)
            {
                quads[0].checkDebugPutches(drawDebugPutches);
                quads[1].checkDebugPutches(drawDebugPutches);
                quads[2].checkDebugPutches(drawDebugPutches);
                quads[3].checkDebugPutches(drawDebugPutches);
            }
        }

        float distTemp = 0;
        float distTemp2 = 0;
        int newLOD = 0;

        Vector3 scaleTemp = new Vector3();
        Vector3 posWorldTemp;
        float sizePatchSquare = 0f;

        public int countPercentes = 0;

        public Vector3 getPosLocal(Vector3 _pos)
        {
            Vector3 _temp;
            _temp = manager.jump - manager.altTrees[altTreesID].step + _pos / manager.altTrees[altTreesID].altTrees.altTreesManagerData.sizePatch;

            return _temp;
        }


        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();

        void checkTrees(Transform[] _cameras, bool forceTrees = false)
        {
            if (treesCount + treesNoBillbCount == 0)
                return;

            bool isStop = false;
            AltTreesTrees treeTemp = null;

            Vector3[] posCamsLocalTemp = new Vector3[_cameras.Length];
            sizePatchSquare = manager.altTrees[altTreesID].altTrees.altTreesManagerData.sizePatch * manager.altTrees[altTreesID].altTrees.altTreesManagerData.sizePatch;

            for (int c = 0; c < _cameras.Length; c++)
            {
                posCamsLocalTemp[c] = getPosLocal(_cameras[c].position);
            }

            int ot = Mathf.FloorToInt(countPercentes * (((treesCount + treesNoBillbCount) / 100f) * manager.altTreesMain.altTreesManagerData.checkTreesPercentPerFrame));
            countPercentes++;
            int otDo = Mathf.FloorToInt(countPercentes * (((treesCount + treesNoBillbCount) / 100f) * manager.altTreesMain.altTreesManagerData.checkTreesPercentPerFrame));
            if (otDo >= treesCount + treesNoBillbCount)
            {
                otDo = treesCount + treesNoBillbCount;
                countPercentes = 0;
            }

            if(forceTrees)
            {
                ot = 0;
                otDo = treesCount;
            }

            for (int i = ot; i < otDo; i++)
            {
                if (i < treesCount)
                    treeTemp = trees[i];
                else
                    treeTemp = treesNoBillb[i - treesCount];

                isStop = false;

                #if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        if (manager.cameras.Length > 1 && treeTemp.go != null)
                        {
                            for (int c = 0; c < manager.cameras.Length; c++)
                            {
                                if (manager.cameras[c].Equals(treeTemp.go.transform))
                                {
                                    isStop = true;
                                    break;
                                }
                            }
                        }
                    }
                #endif

                if (!isStop)
                {
                    distTemp = 100000000;

                    for (int c = 0; c < _cameras.Length; c++)
                    {
                        distTemp2 = AltUtilities.fastDistanceSqrt(treeTemp.pos, posCamsLocalTemp[c]);
                        if (distTemp2 < distTemp)
                            distTemp = distTemp2;
                    }
                    distTemp *= sizePatchSquare;
                    distTemp /= ((i < treesCount) ? manager.altTreesMain.altTreesManagerData.distanceTreesLODFactor : manager.altTreesMain.altTreesManagerData.distanceObjectsLODFactor);
                

                    newLOD = 0;
                    for (int j = 0; j < manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.distancesSquares.Length; j++)
                    {
                        if (distTemp > manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.distancesSquares[j] * treeTemp.maxScaleSquare)
                            newLOD = j + 1;
                    }
                    if (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.drawPlaneBillboard && newLOD == manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.distancesSquares.Length && distTemp > manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.distancePlaneBillboardSquare * treeTemp.maxScaleSquare)
                        newLOD = -2;
                    if (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.isObject && newLOD == -2 && distTemp > manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.distanceCullingSquare * treeTemp.maxScaleSquare)
                        newLOD = -3;
                

                    if (treeTemp.currentLOD == -1)
                    {
                        if (newLOD != -2 && newLOD != -3)
                        {
                            treeTemp.go = manager.getObjPool(treeTemp.idPrototype, newLOD, altTreesID, treeTemp.idTree, treeTemp.color, treeTemp.colorBark);
                            manager.treesList.Add(treeTemp.go, treeTemp);
                        }
                        else if (newLOD != -3)
                        {
                            treeTemp.go = manager.getObjBillboardPool(treeTemp.idPrototype, treeTemp.widthScale, treeTemp.heightScale, treeTemp.rotation, treeTemp.color);
                            manager.treesList.Add(treeTemp.go, treeTemp);
                        }



                        if (newLOD != -2 && newLOD != -3)
                        {
                            scaleTemp.x = treeTemp.widthScale;
                            scaleTemp.y = treeTemp.heightScale;
                            scaleTemp.z = treeTemp.widthScale;
                            treeTemp.go.transform.localScale = scaleTemp;
                            treeTemp.go.transform.position = treeTemp.getPosWorld();
                            treeTemp.go.transform.rotation = Quaternion.AngleAxis(treeTemp.rotation, Vector3.up);
                            treeTemp.go.SetActive(true);

                            if (Application.isPlaying && manager.altTreesMain.altTreesManagerData.enableColliders)
                            {
                                if (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.isColliders)
                                {
                                    treeTemp.collider = manager.getColliderPool(treeTemp.idPrototype, false);
                                    treeTemp.collider.transform.localScale = treeTemp.go.transform.localScale;
                                    treeTemp.collider.transform.position = treeTemp.go.transform.position;
                                    treeTemp.collider.transform.rotation = treeTemp.go.transform.rotation;
                                }
                            }
                        }
                        else if (newLOD != -3)
                        {
                            treeTemp.go.transform.localScale = Vector3.one;
                            treeTemp.go.transform.position = treeTemp.getPosWorld() + new Vector3(0f, manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale / 2f /*- (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale - manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale / manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.space) / 2f*/ + manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.up * treeTemp.heightScale, 0f);
                            treeTemp.go.transform.rotation = Quaternion.identity;
                            treeTemp.go.SetActive(true);

                            if (Application.isPlaying && manager.altTreesMain.altTreesManagerData.enableColliders)
                            {
                                if (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.isBillboardColliders)
                                {
                                    treeTemp.collider = manager.getColliderPool(treeTemp.idPrototype, true);
                                    scaleTemp.x = treeTemp.widthScale;
                                    scaleTemp.y = treeTemp.heightScale;
                                    scaleTemp.z = treeTemp.widthScale;
                                    treeTemp.collider.transform.localScale = scaleTemp;
                                    treeTemp.collider.transform.position = treeTemp.getPosWorld();
                                    treeTemp.collider.transform.rotation = Quaternion.AngleAxis(treeTemp.rotation, Vector3.up);
                                }
                            }
                        }

                        treeTemp.currentLOD = newLOD;
                    }
                    else
                    {
                        #if UNITY_EDITOR
                            if (!Application.isPlaying)
                            {
                                if (manager.cameras.Length > 1 && manager.isSelectionTree && treeTemp.go != null)
                                {
                                    for (int j = 0; j < manager.cameras.Length; j++)
                                    {
                                        if (manager.cameras[j].Equals(treeTemp.go.transform))
                                        {
                                            Debug.Log("1");
                                            isStop = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        #endif

                        if (!isStop)
                        {
                            if (treeTemp.currentLOD != newLOD)
                            {
                                bool isNoBillboardCrossFade = (treeTemp.currentCrossFadeId == 3 || treeTemp.currentCrossFadeId == 4);
                                if (treeTemp.currentLOD != -2 && treeTemp.currentLOD != -3)      //  currentLOD is mesh
                                {
                                    if (newLOD == -2)               //  newLOD is billboard
                                    {
                                        if (Application.isPlaying && manager.altTreesMain.altTreesManagerData.enableColliders && !manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.isCollidersEqual)
                                        {
                                            if (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.isColliders)
                                            {
                                                manager.delColliderPool(treeTemp.idPrototype, treeTemp.collider, false);
                                            }
                                            if (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.isBillboardColliders)
                                            {
                                                treeTemp.collider = manager.getColliderPool(treeTemp.idPrototype, true);
                                                scaleTemp.x = treeTemp.widthScale;
                                                scaleTemp.y = treeTemp.heightScale;
                                                scaleTemp.z = treeTemp.widthScale;
                                                treeTemp.collider.transform.localScale = scaleTemp;
                                                treeTemp.collider.transform.position = treeTemp.getPosWorld();
                                                treeTemp.collider.transform.rotation = Quaternion.AngleAxis(treeTemp.rotation, Vector3.up);
                                            }
                                        }

                                        if (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.isBillboardCrossFade && !manager.isSelectionTree)
                                        {
                                            if (treeTemp.currentCrossFadeId == -1)
                                            {
                                                manager.treesCrossFade.Add(treeTemp);
                                                treeTemp.currentCrossFadeId = 2;
                                                treeTemp.currentCrossFadeLOD = treeTemp.currentLOD;
                                                treeTemp.goCrossFade = treeTemp.go;
                                                treeTemp.goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = true;
                                                treeTemp.crossFadeTreeMeshRenderer = treeTemp.goCrossFade.GetComponent<MeshRenderer>();
                                                treeTemp.crossFadeTreeMeshRenderer.sharedMaterials = manager.treesPoolArray[treeTemp.idPrototypeIndex].objsArray[treeTemp.currentLOD].materialsMeshCrossFade;

                                                treeTemp.go = manager.getObjBillboardPool(treeTemp.idPrototype, treeTemp.widthScale, treeTemp.heightScale, treeTemp.rotation, treeTemp.color);
                                                treeTemp.crossFadeBillboardMeshRenderer = treeTemp.go.GetComponent<MeshRenderer>();

                                                treeTemp.crossFadeBillboardMeshRenderer.sharedMaterial = manager.treesPoolArray[treeTemp.idPrototypeIndex].materialBillboardCrossFade;
                                                propBlock.Clear();
                                                treeTemp.crossFadeBillboardMeshRenderer.GetPropertyBlock(propBlock);
                                                propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 0.0f);
                                                treeTemp.crossFadeBillboardMeshRenderer.SetPropertyBlock(propBlock);

                                                if (Application.isPlaying)
                                                    treeTemp.crossFadeTime = Time.time + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                else
                                                {
    #if UNITY_EDITOR
                                                    {
                                                        treeTemp.crossFadeTime = (float)EditorApplication.timeSinceStartup + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                    }
    #endif
                                                }
                                                treeTemp.go.transform.localScale = Vector3.one;
                                                treeTemp.go.transform.position = treeTemp.getPosWorld() + new Vector3(0f, manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale / 2f /*- (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale - manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale / manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.space) / 2f*/ + manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.up * treeTemp.heightScale, 0f);
                                                treeTemp.go.transform.rotation = Quaternion.identity;
                                                treeTemp.go.SetActive(true);
                                                manager.treesList.Add(treeTemp.go, treeTemp);
                                            }
                                            else
                                            {
                                                if (treeTemp.currentCrossFadeId == 1)
                                                {
                                                    treeTemp.currentCrossFadeId = 2;
                                                    treeTemp.currentCrossFadeLOD = treeTemp.currentLOD;
                                                    manager.delObjBillboardPool(treeTemp.idPrototype, treeTemp.goCrossFade);
                                                    treeTemp.goCrossFade = treeTemp.go;
                                                    treeTemp.goCrossFade.SetActive(true);
                                                    treeTemp.goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = true;
                                                    treeTemp.crossFadeTreeMeshRenderer = treeTemp.goCrossFade.GetComponent<MeshRenderer>();
                                                    treeTemp.crossFadeTreeMeshRenderer.sharedMaterials = manager.treesPoolArray[treeTemp.idPrototypeIndex].objsArray[treeTemp.currentLOD].materialsMeshCrossFade;

                                                    treeTemp.go = manager.getObjBillboardPool(treeTemp.idPrototype, treeTemp.widthScale, treeTemp.heightScale, treeTemp.rotation, treeTemp.color);
                                                    treeTemp.crossFadeBillboardMeshRenderer = treeTemp.go.GetComponent<MeshRenderer>();

                                                    treeTemp.crossFadeBillboardMeshRenderer.sharedMaterial = manager.treesPoolArray[treeTemp.idPrototypeIndex].materialBillboardCrossFade;
                                                    propBlock.Clear();
                                                    treeTemp.crossFadeBillboardMeshRenderer.GetPropertyBlock(propBlock);
                                                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1.0f);
                                                    treeTemp.crossFadeBillboardMeshRenderer.SetPropertyBlock(propBlock);

                                                    if (Application.isPlaying)
                                                        treeTemp.crossFadeTime = Time.time + (manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard - (treeTemp.crossFadeTime - Time.time));
                                                    else
                                                    {
    #if UNITY_EDITOR
                                                        {
                                                            treeTemp.crossFadeTime = (float)EditorApplication.timeSinceStartup + (manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard - (treeTemp.crossFadeTime - (float)EditorApplication.timeSinceStartup));
                                                        }
    #endif
                                                    }
                                                    treeTemp.go.transform.localScale = Vector3.one;
                                                    treeTemp.go.transform.position = treeTemp.getPosWorld() + new Vector3(0f, manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale / 2f /*- (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale - manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale / manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.space) / 2f*/ + manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.up * treeTemp.heightScale, 0f);
                                                    treeTemp.go.transform.rotation = Quaternion.identity;
                                                    treeTemp.go.SetActive(true);
                                                    manager.treesList.Add(treeTemp.go, treeTemp);
                                                }
                                                else if (treeTemp.currentCrossFadeId == 2)
                                                {
                                                    Debug.Log("Error. treeTemp.currentCrossFadeId == 2");
                                                }
                                                else if (treeTemp.currentCrossFadeId == 3)
                                                {
                                                    propBlock.Clear();
                                                    propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, treeTemp.color);
                                                    propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, treeTemp.colorBark);
                                                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1.0f);
                                                    propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                                                    propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 0.0f);
                                                    treeTemp.crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);


                                                    treeTemp.currentCrossFadeId = 2;
                                                    treeTemp.currentCrossFadeLOD = treeTemp.currentLOD;
                                                    treeTemp.go.SetActive(true);
                                                    treeTemp.goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = false;
                                                    manager.delObjPool(treeTemp.idPrototype, treeTemp.currentCrossFadeLOD, treeTemp.goCrossFade);
                                                    treeTemp.goCrossFade = treeTemp.go;
                                                    treeTemp.goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = true;
                                                    treeTemp.crossFadeTreeMeshRenderer = treeTemp.goCrossFade.GetComponent<MeshRenderer>();
                                                    treeTemp.crossFadeTreeMeshRenderer.sharedMaterials = manager.treesPoolArray[treeTemp.idPrototypeIndex].objsArray[treeTemp.currentLOD].materialsMeshCrossFade;

                                                    treeTemp.go = manager.getObjBillboardPool(treeTemp.idPrototype, treeTemp.widthScale, treeTemp.heightScale, treeTemp.rotation, treeTemp.color);
                                                    treeTemp.crossFadeBillboardMeshRenderer = treeTemp.go.GetComponent<MeshRenderer>();

                                                    treeTemp.crossFadeBillboardMeshRenderer.sharedMaterial = manager.treesPoolArray[treeTemp.idPrototypeIndex].materialBillboardCrossFade;
                                                    propBlock.Clear();
                                                    treeTemp.crossFadeBillboardMeshRenderer.GetPropertyBlock(propBlock);
                                                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 0.0f);
                                                    treeTemp.crossFadeBillboardMeshRenderer.SetPropertyBlock(propBlock);

                                                    if (Application.isPlaying)
                                                        treeTemp.crossFadeTime = Time.time + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                    else
                                                    {
    #if UNITY_EDITOR
                                                        {
                                                            treeTemp.crossFadeTime = (float)EditorApplication.timeSinceStartup + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                        }
    #endif
                                                    }
                                                    treeTemp.go.transform.localScale = Vector3.one;
                                                    treeTemp.go.transform.position = treeTemp.getPosWorld() + new Vector3(0f, manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale / 2f /*- (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale - manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale / manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.space) / 2f*/ + manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.up * treeTemp.heightScale, 0f);
                                                    treeTemp.go.transform.rotation = Quaternion.identity;
                                                    treeTemp.go.SetActive(true);
                                                    manager.treesList.Add(treeTemp.go, treeTemp);
                                                }
                                                else if (treeTemp.currentCrossFadeId == 4)
                                                {
                                                    propBlock.Clear();
                                                    propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, treeTemp.color);
                                                    propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, treeTemp.colorBark);
                                                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1.0f);
                                                    propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                                                    propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 1.0f);
                                                    treeTemp.crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);

                                                    treeTemp.currentCrossFadeId = 2;
                                                    treeTemp.currentCrossFadeLOD = treeTemp.currentLOD;
                                                    treeTemp.go = manager.getObjBillboardPool(treeTemp.idPrototype, treeTemp.widthScale, treeTemp.heightScale, treeTemp.rotation, treeTemp.color);
                                                    treeTemp.crossFadeBillboardMeshRenderer = treeTemp.go.GetComponent<MeshRenderer>();
                                                    treeTemp.crossFadeBillboardMeshRenderer.sharedMaterial = manager.treesPoolArray[treeTemp.idPrototypeIndex].materialBillboardCrossFade;

                                                    if (Application.isPlaying)
                                                        treeTemp.crossFadeTime = Time.time + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                    else
                                                    {
    #if UNITY_EDITOR
                                                        {
                                                            treeTemp.crossFadeTime = (float)EditorApplication.timeSinceStartup + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                        }
    #endif
                                                    }

                                                    treeTemp.go.transform.localScale = Vector3.one;
                                                    treeTemp.go.transform.position = treeTemp.getPosWorld() + new Vector3(0f, manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale / 2f /*- (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale - manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale / manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.space) / 2f*/ + manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.up * treeTemp.heightScale, 0f);
                                                    treeTemp.go.transform.rotation = Quaternion.identity;
                                                    treeTemp.go.SetActive(true);
                                                    manager.treesList.Add(treeTemp.go, treeTemp);
                                                }
                                                else if (treeTemp.currentCrossFadeId == 6)
                                                {
                                                    propBlock.Clear();
                                                    propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, treeTemp.color);
                                                    propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, treeTemp.colorBark);
                                                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1.0f);
                                                    propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                                                    propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 0.0f);
                                                    treeTemp.crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);
                                                
                                                    treeTemp.goCrossFade = treeTemp.go;
                                                    treeTemp.currentCrossFadeId = 2;
                                                    treeTemp.currentCrossFadeLOD = treeTemp.currentLOD;
                                                    treeTemp.go = manager.getObjBillboardPool(treeTemp.idPrototype, treeTemp.widthScale, treeTemp.heightScale, treeTemp.rotation, treeTemp.color);
                                                    treeTemp.crossFadeBillboardMeshRenderer = treeTemp.go.GetComponent<MeshRenderer>();
                                                    treeTemp.crossFadeBillboardMeshRenderer.sharedMaterial = manager.treesPoolArray[treeTemp.idPrototypeIndex].materialBillboardCrossFade;

                                                    if (Application.isPlaying)
                                                        treeTemp.crossFadeTime = Time.time + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                    else
                                                    {
    #if UNITY_EDITOR
                                                        {
                                                            treeTemp.crossFadeTime = (float)EditorApplication.timeSinceStartup + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                        }
    #endif
                                                    }

                                                    treeTemp.go.transform.localScale = Vector3.one;
                                                    treeTemp.go.transform.position = treeTemp.getPosWorld() + new Vector3(0f, manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale / 2f /*- (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale - manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale / manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.space) / 2f*/ + manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.up * treeTemp.heightScale, 0f);
                                                    treeTemp.go.transform.rotation = Quaternion.identity;
                                                    treeTemp.go.SetActive(true);
                                                    manager.treesList.Add(treeTemp.go, treeTemp);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            manager.treesList.Remove(treeTemp.go);
                                            manager.delObjPool(treeTemp.idPrototype, treeTemp.currentLOD, treeTemp.go);

                                            treeTemp.go = manager.getObjBillboardPool(treeTemp.idPrototype, treeTemp.widthScale, treeTemp.heightScale, treeTemp.rotation, treeTemp.color);

                                            treeTemp.go.transform.localScale = Vector3.one;
                                            treeTemp.go.transform.position = treeTemp.getPosWorld() + new Vector3(0f, manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale / 2f /*- (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale - manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale / manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.space) / 2f*/ + manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.up * treeTemp.heightScale, 0f);
                                            treeTemp.go.transform.rotation = Quaternion.identity;
                                            treeTemp.go.SetActive(true);
                                            manager.treesList.Add(treeTemp.go, treeTemp);
                                        }
                                    }
                                    else if (newLOD == -3)               //  newLOD is culled
                                    {
                                        if (Application.isPlaying && manager.altTreesMain.altTreesManagerData.enableColliders)
                                        {
                                            if (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.isColliders)
                                            {
                                                manager.delColliderPool(treeTemp.idPrototype, treeTemp.collider, false);
                                            }
                                        }

                                        if (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.isBillboardCrossFade && !manager.isSelectionTree)
                                        {
                                            if (treeTemp.currentCrossFadeId == -1)
                                            {
                                                manager.treesCrossFade.Add(treeTemp);
                                                treeTemp.currentCrossFadeId = 5;
                                                treeTemp.currentCrossFadeLOD = treeTemp.currentLOD;
                                                treeTemp.goCrossFade = treeTemp.go;
                                                treeTemp.goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = true;
                                                treeTemp.crossFadeTreeMeshRenderer = treeTemp.goCrossFade.GetComponent<MeshRenderer>();
                                                treeTemp.crossFadeTreeMeshRenderer.sharedMaterials = manager.treesPoolArray[treeTemp.idPrototypeIndex].objsArray[treeTemp.currentLOD].materialsMeshCrossFade;

                                                if (Application.isPlaying)
                                                    treeTemp.crossFadeTime = Time.time + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                else
                                                {
    #if UNITY_EDITOR
                                                    {
                                                        treeTemp.crossFadeTime = (float)EditorApplication.timeSinceStartup + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                    }
    #endif
                                                }
                                            }
                                            else
                                            {
                                                if (treeTemp.currentCrossFadeId == 6)
                                                {
                                                    treeTemp.currentCrossFadeId = 5;
                                                    treeTemp.currentCrossFadeLOD = treeTemp.currentLOD;
                                                    treeTemp.goCrossFade = treeTemp.go;
                                                    treeTemp.goCrossFade.SetActive(true);
                                                    treeTemp.goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = true;
                                                    treeTemp.crossFadeTreeMeshRenderer = treeTemp.goCrossFade.GetComponent<MeshRenderer>();
                                                    treeTemp.crossFadeTreeMeshRenderer.sharedMaterials = manager.treesPoolArray[treeTemp.idPrototypeIndex].objsArray[treeTemp.currentLOD].materialsMeshCrossFade;

                                                    if (Application.isPlaying)
                                                        treeTemp.crossFadeTime = Time.time + (manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard - (treeTemp.crossFadeTime - Time.time));
                                                    else
                                                    {
    #if UNITY_EDITOR
                                                        {
                                                            treeTemp.crossFadeTime = (float)EditorApplication.timeSinceStartup + (manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard - (treeTemp.crossFadeTime - (float)EditorApplication.timeSinceStartup));
                                                        }
    #endif
                                                    }
                                                }
                                                else if (treeTemp.currentCrossFadeId == 1)
                                                {
                                                    manager.delObjBillboardPool(treeTemp.idPrototype, treeTemp.goCrossFade);
                                                    treeTemp.goCrossFade = treeTemp.go;
                                                    treeTemp.currentCrossFadeId = 5;
                                                    treeTemp.currentCrossFadeLOD = treeTemp.currentLOD;
                                                }
                                                else if (treeTemp.currentCrossFadeId == 5)
                                                {
                                                    Debug.Log("Error. treeTemp.currentCrossFadeId == 5");
                                                }
                                                else if (treeTemp.currentCrossFadeId == 3)
                                                {
                                                    treeTemp.crossFadeTreeMeshRenderer.sharedMaterials = manager.treesPoolArray[treeTemp.idPrototypeIndex].objsArray[treeTemp.currentCrossFadeLOD].materialsMesh;
                                                    propBlock.Clear();
                                                    propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, treeTemp.color);
                                                    propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, treeTemp.colorBark);
                                                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1.0f);
                                                    propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                                                    propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 0.0f);
                                                    treeTemp.crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);


                                                    treeTemp.currentCrossFadeId = 5;
                                                    treeTemp.currentCrossFadeLOD = treeTemp.currentLOD;
                                                    treeTemp.go.SetActive(true);
                                                    treeTemp.goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = false;
                                                    manager.delObjPool(treeTemp.idPrototype, treeTemp.currentCrossFadeLOD, treeTemp.goCrossFade);
                                                    treeTemp.goCrossFade = treeTemp.go;
                                                    treeTemp.goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = true;
                                                    treeTemp.crossFadeTreeMeshRenderer = treeTemp.goCrossFade.GetComponent<MeshRenderer>();
                                                    treeTemp.crossFadeTreeMeshRenderer.sharedMaterials = manager.treesPoolArray[treeTemp.idPrototypeIndex].objsArray[treeTemp.currentLOD].materialsMeshCrossFade;

                                                    if (Application.isPlaying)
                                                        treeTemp.crossFadeTime = Time.time + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                    else
                                                    {
    #if UNITY_EDITOR
                                                        {
                                                            treeTemp.crossFadeTime = (float)EditorApplication.timeSinceStartup + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                        }
    #endif
                                                    }
                                                }
                                                else if (treeTemp.currentCrossFadeId == 4)
                                                {
                                                    propBlock.Clear();
                                                    propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, treeTemp.color);
                                                    propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, treeTemp.colorBark);
                                                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1.0f);
                                                    propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                                                    propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 0.0f);
                                                    treeTemp.crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);
                                                    treeTemp.currentCrossFadeId = 5;
                                                    treeTemp.currentCrossFadeLOD = treeTemp.currentLOD;

                                                    if (Application.isPlaying)
                                                        treeTemp.crossFadeTime = Time.time + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                    else
                                                    {
    #if UNITY_EDITOR
                                                        {
                                                            treeTemp.crossFadeTime = (float)EditorApplication.timeSinceStartup + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                        }
    #endif
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            manager.treesList.Remove(treeTemp.go);
                                            manager.delObjPool(treeTemp.idPrototype, treeTemp.currentLOD, treeTemp.go);
                                        }
                                    }
                                    else                            //  newLOD is mesh
                                    {
                                        if (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.isMeshCrossFade && !manager.isSelectionTree)
                                        {
                                            if (treeTemp.currentCrossFadeId == -1)
                                            {
                                                manager.treesCrossFade.Add(treeTemp);

                                                if (newLOD > treeTemp.currentLOD)
                                                {
                                                    treeTemp.currentCrossFadeId = 3;
                                                    treeTemp.goCrossFade = treeTemp.go;
                                                    treeTemp.goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = true;
                                                    treeTemp.crossFadeTreeMeshRenderer = treeTemp.goCrossFade.GetComponent<MeshRenderer>();

                                                    treeTemp.go = manager.getObjPool(treeTemp.idPrototype, newLOD, altTreesID, treeTemp.idTree, treeTemp.color, treeTemp.colorBark);
                                                    treeTemp.go.SetActive(false);

                                                    scaleTemp.x = treeTemp.widthScale;
                                                    scaleTemp.y = treeTemp.heightScale;
                                                    scaleTemp.z = treeTemp.widthScale;
                                                    treeTemp.go.transform.localScale = scaleTemp;
                                                    treeTemp.go.transform.position = treeTemp.getPosWorld();
                                                    treeTemp.go.transform.rotation = Quaternion.AngleAxis(treeTemp.rotation, Vector3.up);
                                                    manager.treesList.Add(treeTemp.go, treeTemp);
                                                }
                                                else
                                                {
                                                    treeTemp.currentCrossFadeId = 4;
                                                    manager.delObjPool(treeTemp.idPrototype, treeTemp.currentLOD, treeTemp.go);
                                                    treeTemp.goCrossFade = manager.getObjPool(treeTemp.idPrototype, newLOD, altTreesID, treeTemp.idTree, treeTemp.color, treeTemp.colorBark);
                                                    treeTemp.goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = true;
                                                    treeTemp.crossFadeTreeMeshRenderer = treeTemp.goCrossFade.GetComponent<MeshRenderer>();
                                                    propBlock.Clear();
                                                    propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, treeTemp.color);
                                                    propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, treeTemp.colorBark);
                                                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1.0f);
                                                    propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                                                    propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 1.0f);
                                                    treeTemp.crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);

                                                    scaleTemp.x = treeTemp.widthScale;
                                                    scaleTemp.y = treeTemp.heightScale;
                                                    scaleTemp.z = treeTemp.widthScale;
                                                    treeTemp.goCrossFade.transform.localScale = scaleTemp;
                                                    treeTemp.goCrossFade.transform.position = treeTemp.getPosWorld();
                                                    treeTemp.goCrossFade.transform.rotation = Quaternion.AngleAxis(treeTemp.rotation, Vector3.up);
                                                    treeTemp.goCrossFade.SetActive(true);
                                                    manager.treesList.Add(treeTemp.goCrossFade, treeTemp);
                                                }


                                                treeTemp.currentCrossFadeLOD = treeTemp.currentLOD;
                                                if (Application.isPlaying)
                                                    treeTemp.crossFadeTime = Time.time + manager.altTreesMain.altTreesManagerData.crossFadeTimeMesh;
                                                else
                                                {
    #if UNITY_EDITOR
                                                    {
                                                        treeTemp.crossFadeTime = (float)EditorApplication.timeSinceStartup + manager.altTreesMain.altTreesManagerData.crossFadeTimeMesh;
                                                    }
    #endif
                                                }
                                            }
                                            else
                                            {
                                                if (treeTemp.currentCrossFadeId == 1 || treeTemp.currentCrossFadeId == 6)
                                                {
                                                    treeTemp.crossFadeTreeMeshRenderer.sharedMaterials = manager.treesPoolArray[treeTemp.idPrototypeIndex].objsArray[newLOD].materialsMesh;
                                                    propBlock.Clear();
                                                    propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, treeTemp.color);
                                                    propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, treeTemp.colorBark);
                                                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1.0f);
                                                    propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                                                    propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 0.0f);
                                                    treeTemp.crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);

                                                    treeTemp.go.GetComponent<AltTreeInstance>().isCrossFade = false;
                                                    manager.delObjPool(treeTemp.idPrototype, treeTemp.currentLOD, treeTemp.go);
                                                    treeTemp.go = manager.getObjPool(treeTemp.idPrototype, newLOD, altTreesID, treeTemp.idTree, treeTemp.color, treeTemp.colorBark);
                                                    treeTemp.go.GetComponent<AltTreeInstance>().isCrossFade = true;
                                                    treeTemp.crossFadeTreeMeshRenderer = treeTemp.go.GetComponent<MeshRenderer>();
                                                    treeTemp.crossFadeTreeMeshRenderer.sharedMaterials = manager.treesPoolArray[treeTemp.idPrototypeIndex].objsArray[newLOD].materialsMeshCrossFade;

                                                    float tempFloatNext = 0f;

                                                    if (Application.isPlaying)
                                                        tempFloatNext = treeTemp.crossFadeTime - Time.time;
                                                    else
                                                    {
    #if UNITY_EDITOR
                                                        {
                                                            tempFloatNext = treeTemp.crossFadeTime - (float)EditorApplication.timeSinceStartup;
                                                        }
    #endif
                                                    }
                                                    propBlock.Clear();
                                                    propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, treeTemp.color);
                                                    propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, treeTemp.colorBark);
                                                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, Mathf.Clamp(1f - tempFloatNext / (float)manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard, 0f, 1.0f));
                                                    propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                                                    propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 0.0f);
                                                    treeTemp.crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);

                                                    scaleTemp.x = treeTemp.widthScale;
                                                    scaleTemp.y = treeTemp.heightScale;
                                                    scaleTemp.z = treeTemp.widthScale;
                                                    treeTemp.go.transform.localScale = scaleTemp;
                                                    treeTemp.go.transform.position = treeTemp.getPosWorld();
                                                    treeTemp.go.transform.rotation = Quaternion.AngleAxis(treeTemp.rotation, Vector3.up);
                                                    treeTemp.go.SetActive(true);
                                                    manager.treesList.Add(treeTemp.go, treeTemp);
                                                }
                                                else if (treeTemp.currentCrossFadeId == 2)
                                                {
                                                    Debug.Log("Error. treeTemp.currentCrossFadeId == 2");
                                                }
                                                else if (isNoBillboardCrossFade)
                                                {
                                                    manager.treesCrossFade.Remove(treeTemp);

                                                    if (treeTemp.currentCrossFadeId == 3)
                                                    {
                                                        treeTemp.goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = false;
                                                        propBlock.Clear();
                                                        propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, treeTemp.color);
                                                        propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, treeTemp.colorBark);
                                                        propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1.0f);
                                                        propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                                                        propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 0.0f);
                                                        treeTemp.crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);

                                                        manager.delObjPool(treeTemp.idPrototype, treeTemp.currentCrossFadeLOD, treeTemp.goCrossFade);
                                                        manager.delObjPool(treeTemp.idPrototype, treeTemp.currentLOD, treeTemp.go);
                                                        treeTemp.crossFadeTreeMeshRenderer = null;
                                                    }
                                                    else if (treeTemp.currentCrossFadeId == 4)
                                                    {
                                                        treeTemp.goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = false;
                                                        propBlock.Clear();
                                                        propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, treeTemp.color);
                                                        propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, treeTemp.colorBark);
                                                        propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1.0f);
                                                        propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                                                        propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 0.0f);
                                                        treeTemp.crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);

                                                        manager.delObjPool(treeTemp.idPrototype, treeTemp.currentCrossFadeLOD, treeTemp.goCrossFade);
                                                        treeTemp.crossFadeTreeMeshRenderer = null;
                                                    }

                                                    treeTemp.go = manager.getObjPool(treeTemp.idPrototype, newLOD, altTreesID, treeTemp.idTree, treeTemp.color, treeTemp.colorBark);

                                                    scaleTemp.x = treeTemp.widthScale;
                                                    scaleTemp.y = treeTemp.heightScale;
                                                    scaleTemp.z = treeTemp.widthScale;
                                                    treeTemp.go.transform.localScale = scaleTemp;
                                                    treeTemp.go.transform.position = treeTemp.getPosWorld();
                                                    treeTemp.go.transform.rotation = Quaternion.AngleAxis(treeTemp.rotation, Vector3.up);
                                                    treeTemp.go.SetActive(true);
                                                    manager.treesList.Add(treeTemp.go, treeTemp);

                                                    treeTemp.currentCrossFadeId = -1;
                                                    treeTemp.currentCrossFadeLOD = -1;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            manager.treesList.Remove(treeTemp.go);
                                            manager.delObjPool(treeTemp.idPrototype, treeTemp.currentLOD, treeTemp.go);

                                            treeTemp.go = manager.getObjPool(treeTemp.idPrototype, newLOD, altTreesID, treeTemp.idTree, treeTemp.color, treeTemp.colorBark);
                                            scaleTemp.x = treeTemp.widthScale;
                                            scaleTemp.y = treeTemp.heightScale;
                                            scaleTemp.z = treeTemp.widthScale;
                                            treeTemp.go.transform.localScale = scaleTemp;
                                            treeTemp.go.transform.position = treeTemp.getPosWorld();
                                            treeTemp.go.transform.rotation = Quaternion.AngleAxis(treeTemp.rotation, Vector3.up);
                                            manager.treesList.Add(treeTemp.go, treeTemp);
                                        }
                                    }
                                }
                                else if (treeTemp.currentLOD == -2)               //  currentLOD is billboard
                                {
                                    if (newLOD != -3)               //  newLOD is mesh
                                    {
                                        if (Application.isPlaying && manager.altTreesMain.altTreesManagerData.enableColliders && !manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.isCollidersEqual)
                                        {
                                            if (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.isBillboardColliders)
                                            {
                                                manager.delColliderPool(treeTemp.idPrototype, treeTemp.collider, true);
                                            }
                                            if (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.isColliders)
                                            {
                                                treeTemp.collider = manager.getColliderPool(treeTemp.idPrototype, false);
                                                scaleTemp.x = treeTemp.widthScale;
                                                scaleTemp.y = treeTemp.heightScale;
                                                scaleTemp.z = treeTemp.widthScale;
                                                treeTemp.collider.transform.localScale = scaleTemp;
                                                treeTemp.collider.transform.position = treeTemp.getPosWorld();
                                                treeTemp.collider.transform.rotation = Quaternion.AngleAxis(treeTemp.rotation, Vector3.up);
                                            }
                                        }

                                        if (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.isBillboardCrossFade && !manager.isSelectionTree)
                                        {
                                            if (treeTemp.currentCrossFadeId == -1)
                                            {
                                                manager.treesCrossFade.Add(treeTemp);
                                                treeTemp.currentCrossFadeId = 1;
                                                treeTemp.currentCrossFadeLOD = -1;
                                                treeTemp.goCrossFade = treeTemp.go;
                                                treeTemp.crossFadeBillboardMeshRenderer = treeTemp.goCrossFade.GetComponent<MeshRenderer>();

                                                treeTemp.crossFadeBillboardMeshRenderer.sharedMaterial = manager.treesPoolArray[treeTemp.idPrototypeIndex].materialBillboardCrossFade;
                                                propBlock.Clear();
                                                treeTemp.crossFadeBillboardMeshRenderer.GetPropertyBlock(propBlock);
                                                propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1.0f);
                                                treeTemp.crossFadeBillboardMeshRenderer.SetPropertyBlock(propBlock);

                                                treeTemp.go = manager.getObjPool(treeTemp.idPrototype, newLOD, altTreesID, treeTemp.idTree, treeTemp.color, treeTemp.colorBark);
                                                treeTemp.go.GetComponent<AltTreeInstance>().isCrossFade = true;
                                                treeTemp.crossFadeTreeMeshRenderer = treeTemp.go.GetComponent<MeshRenderer>();
                                                treeTemp.crossFadeTreeMeshRenderer.sharedMaterials = manager.treesPoolArray[treeTemp.idPrototypeIndex].objsArray[newLOD].materialsMeshCrossFade;

                                                propBlock.Clear();
                                                propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, treeTemp.color);
                                                propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, treeTemp.colorBark);
                                                propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 0.0f);
                                                propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                                                propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 0.0f);
                                                treeTemp.crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);



                                                if (Application.isPlaying)
                                                    treeTemp.crossFadeTime = Time.time + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                else
                                                {
    #if UNITY_EDITOR
                                                    {
                                                        treeTemp.crossFadeTime = (float)EditorApplication.timeSinceStartup + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                    }
    #endif
                                                }

                                                scaleTemp.x = treeTemp.widthScale;
                                                scaleTemp.y = treeTemp.heightScale;
                                                scaleTemp.z = treeTemp.widthScale;
                                                treeTemp.go.transform.localScale = scaleTemp;
                                                treeTemp.go.transform.position = treeTemp.getPosWorld();
                                                treeTemp.go.transform.rotation = Quaternion.AngleAxis(treeTemp.rotation, Vector3.up);
                                                treeTemp.go.SetActive(true);
                                                manager.treesList.Add(treeTemp.go, treeTemp);
                                            }
                                            else
                                            {
                                                if (treeTemp.currentCrossFadeId == 1)
                                                {
                                                    Debug.Log("Error. treeTemp.currentCrossFadeId == 1");
                                                }
                                                else if (treeTemp.currentCrossFadeId == 2)
                                                {
                                                    treeTemp.crossFadeTreeMeshRenderer.sharedMaterials = manager.treesPoolArray[treeTemp.idPrototypeIndex].objsArray[newLOD].materialsMesh;
                                                    propBlock.Clear();
                                                    propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, treeTemp.color);
                                                    propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, treeTemp.colorBark);
                                                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1.0f);
                                                    propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                                                    propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 0.0f);
                                                    treeTemp.crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);

                                                    treeTemp.goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = false;
                                                    manager.delObjPool(treeTemp.idPrototype, treeTemp.currentCrossFadeLOD, treeTemp.goCrossFade);
                                                    treeTemp.goCrossFade = treeTemp.go;
                                                    treeTemp.go = manager.getObjPool(treeTemp.idPrototype, newLOD, altTreesID, treeTemp.idTree, treeTemp.color, treeTemp.colorBark);

                                                    treeTemp.go.GetComponent<AltTreeInstance>().isCrossFade = true;
                                                    treeTemp.crossFadeTreeMeshRenderer = treeTemp.go.GetComponent<MeshRenderer>();
                                                    treeTemp.crossFadeTreeMeshRenderer.sharedMaterials = manager.treesPoolArray[treeTemp.idPrototypeIndex].objsArray[newLOD].materialsMeshCrossFade;
                                                    propBlock.Clear();
                                                    propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, treeTemp.color);
                                                    propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, treeTemp.colorBark);
                                                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 0.0f);
                                                    propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                                                    propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 0.0f);
                                                    treeTemp.crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);

                                                    if (Application.isPlaying)
                                                        treeTemp.crossFadeTime = Time.time + (manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard - (treeTemp.crossFadeTime - Time.time));
                                                    else
                                                    {
    #if UNITY_EDITOR
                                                        {
                                                            treeTemp.crossFadeTime = (float)EditorApplication.timeSinceStartup + (manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard - (treeTemp.crossFadeTime - (float)EditorApplication.timeSinceStartup));
                                                        }
    #endif
                                                    }
                                                    treeTemp.currentCrossFadeId = 1;
                                                    treeTemp.currentCrossFadeLOD = -1;

                                                    scaleTemp.x = treeTemp.widthScale;
                                                    scaleTemp.y = treeTemp.heightScale;
                                                    scaleTemp.z = treeTemp.widthScale;
                                                    treeTemp.go.transform.localScale = scaleTemp;
                                                    treeTemp.go.transform.position = treeTemp.getPosWorld();
                                                    treeTemp.go.transform.rotation = Quaternion.AngleAxis(treeTemp.rotation, Vector3.up);
                                                    treeTemp.go.SetActive(true);
                                                    manager.treesList.Add(treeTemp.go, treeTemp);
                                                }
                                                else if (treeTemp.currentCrossFadeId == 8)
                                                {
                                                    treeTemp.goCrossFade = treeTemp.go;
                                                    treeTemp.go = manager.getObjPool(treeTemp.idPrototype, newLOD, altTreesID, treeTemp.idTree, treeTemp.color, treeTemp.colorBark);

                                                    treeTemp.go.GetComponent<AltTreeInstance>().isCrossFade = true;
                                                    treeTemp.crossFadeTreeMeshRenderer = treeTemp.go.GetComponent<MeshRenderer>();
                                                    treeTemp.crossFadeTreeMeshRenderer.sharedMaterials = manager.treesPoolArray[treeTemp.idPrototypeIndex].objsArray[newLOD].materialsMeshCrossFade;
                                                    propBlock.Clear();
                                                    propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, treeTemp.color);
                                                    propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, treeTemp.colorBark);
                                                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 0.0f);
                                                    propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                                                    propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 0.0f);
                                                    treeTemp.crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);

                                                    if (Application.isPlaying)
                                                        treeTemp.crossFadeTime = Time.time + (manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard - (treeTemp.crossFadeTime - Time.time));
                                                    else
                                                    {
    #if UNITY_EDITOR
                                                        {
                                                            treeTemp.crossFadeTime = (float)EditorApplication.timeSinceStartup + (manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard - (treeTemp.crossFadeTime - (float)EditorApplication.timeSinceStartup));
                                                        }
    #endif
                                                    }
                                                    treeTemp.currentCrossFadeId = 1;
                                                    treeTemp.currentCrossFadeLOD = -1;

                                                    scaleTemp.x = treeTemp.widthScale;
                                                    scaleTemp.y = treeTemp.heightScale;
                                                    scaleTemp.z = treeTemp.widthScale;
                                                    treeTemp.go.transform.localScale = scaleTemp;
                                                    treeTemp.go.transform.position = treeTemp.getPosWorld();
                                                    treeTemp.go.transform.rotation = Quaternion.AngleAxis(treeTemp.rotation, Vector3.up);
                                                    treeTemp.go.SetActive(true);
                                                    manager.treesList.Add(treeTemp.go, treeTemp);
                                                }
                                                else if (isNoBillboardCrossFade)
                                                {
                                                    Debug.Log("Error. treeTemp.currentCrossFadeId == " + treeTemp.currentCrossFadeId);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            manager.treesList.Remove(treeTemp.go);
                                            manager.delObjBillboardPool(treeTemp.idPrototype, treeTemp.go);

                                            treeTemp.go = manager.getObjPool(treeTemp.idPrototype, newLOD, altTreesID, treeTemp.idTree, treeTemp.color, treeTemp.colorBark);
                                            scaleTemp.x = treeTemp.widthScale;
                                            scaleTemp.y = treeTemp.heightScale;
                                            scaleTemp.z = treeTemp.widthScale;
                                            treeTemp.go.transform.localScale = scaleTemp;
                                            treeTemp.go.transform.position = treeTemp.getPosWorld();
                                            treeTemp.go.transform.rotation = Quaternion.AngleAxis(treeTemp.rotation, Vector3.up);
                                            manager.treesList.Add(treeTemp.go, treeTemp);
                                        }
                                    }
                                    else if (newLOD == -3)               //  newLOD is culled
                                    {
                                        if (Application.isPlaying && manager.altTreesMain.altTreesManagerData.enableColliders)
                                        {
                                            if (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.isBillboardColliders)
                                            {
                                                manager.delColliderPool(treeTemp.idPrototype, treeTemp.collider, true);
                                            }
                                        }

                                        if (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.isBillboardCrossFade && !manager.isSelectionTree)
                                        {
                                            if (treeTemp.currentCrossFadeId == -1)
                                            {
                                                manager.treesCrossFade.Add(treeTemp);
                                                treeTemp.currentCrossFadeId = 7;
                                                treeTemp.currentCrossFadeLOD = -1;
                                                treeTemp.goCrossFade = treeTemp.go;
                                                treeTemp.crossFadeBillboardMeshRenderer = treeTemp.goCrossFade.GetComponent<MeshRenderer>();

                                                treeTemp.crossFadeBillboardMeshRenderer.sharedMaterial = manager.treesPoolArray[treeTemp.idPrototypeIndex].materialBillboardCrossFade;
                                                propBlock.Clear();
                                                treeTemp.crossFadeBillboardMeshRenderer.GetPropertyBlock(propBlock);
                                                propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1.0f);
                                                treeTemp.crossFadeBillboardMeshRenderer.SetPropertyBlock(propBlock);

                                                if (Application.isPlaying)
                                                    treeTemp.crossFadeTime = Time.time + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                else
                                                {
    #if UNITY_EDITOR
                                                    {
                                                        treeTemp.crossFadeTime = (float)EditorApplication.timeSinceStartup + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                    }
    #endif
                                                }
                                            }
                                            else
                                            {
                                                if (treeTemp.currentCrossFadeId == 7)
                                                {
                                                    Debug.Log("Error. treeTemp.currentCrossFadeId == 7");
                                                }
                                                else if (treeTemp.currentCrossFadeId == 2)
                                                {
                                                    treeTemp.crossFadeTreeMeshRenderer.sharedMaterials = manager.treesPoolArray[treeTemp.idPrototypeIndex].objsArray[treeTemp.currentCrossFadeLOD].materialsMesh;
                                                    propBlock.Clear();
                                                    propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, treeTemp.color);
                                                    propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, treeTemp.colorBark);
                                                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1.0f);
                                                    propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                                                    propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 0.0f);
                                                    treeTemp.crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);

                                                    treeTemp.goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = false;
                                                    manager.delObjPool(treeTemp.idPrototype, treeTemp.currentCrossFadeLOD, treeTemp.goCrossFade);
                                                    treeTemp.goCrossFade = treeTemp.go;

                                                    if (Application.isPlaying)
                                                        treeTemp.crossFadeTime = Time.time + (manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard - (treeTemp.crossFadeTime - Time.time));
                                                    else
                                                    {
    #if UNITY_EDITOR
                                                        {
                                                            treeTemp.crossFadeTime = (float)EditorApplication.timeSinceStartup + (manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard - (treeTemp.crossFadeTime - (float)EditorApplication.timeSinceStartup));
                                                        }
    #endif
                                                    }
                                                    treeTemp.currentCrossFadeId = 7;
                                                    treeTemp.currentCrossFadeLOD = -1;
                                                }
                                                else if (treeTemp.currentCrossFadeId == 8)
                                                {
                                                    treeTemp.goCrossFade = treeTemp.go;

                                                    if (Application.isPlaying)
                                                        treeTemp.crossFadeTime = Time.time + (manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard - (treeTemp.crossFadeTime - Time.time));
                                                    else
                                                    {
    #if UNITY_EDITOR
                                                        {
                                                            treeTemp.crossFadeTime = (float)EditorApplication.timeSinceStartup + (manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard - (treeTemp.crossFadeTime - (float)EditorApplication.timeSinceStartup));
                                                        }
    #endif
                                                    }
                                                    treeTemp.currentCrossFadeId = 7;
                                                    treeTemp.currentCrossFadeLOD = -1;
                                                }
                                                else if (isNoBillboardCrossFade)
                                                {
                                                    Debug.Log("Error. treeTemp.currentCrossFadeId == " + treeTemp.currentCrossFadeId);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            manager.treesList.Remove(treeTemp.go);
                                            manager.delObjBillboardPool(treeTemp.idPrototype, treeTemp.go);
                                        }
                                    }
                                }
                                else if (treeTemp.currentLOD == -3)               //  currentLOD is culled
                                {
                                    if (newLOD != -2)               //  newLOD is mesh
                                    {
                                        if (Application.isPlaying && manager.altTreesMain.altTreesManagerData.enableColliders)
                                        {
                                            if (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.isColliders)
                                            {
                                                treeTemp.collider = manager.getColliderPool(treeTemp.idPrototype, false);
                                                scaleTemp.x = treeTemp.widthScale;
                                                scaleTemp.y = treeTemp.heightScale;
                                                scaleTemp.z = treeTemp.widthScale;
                                                treeTemp.collider.transform.localScale = scaleTemp;
                                                treeTemp.collider.transform.position = treeTemp.getPosWorld();
                                                treeTemp.collider.transform.rotation = Quaternion.AngleAxis(treeTemp.rotation, Vector3.up);
                                            }
                                        }

                                        if (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.isBillboardCrossFade && !manager.isSelectionTree)
                                        {
                                            if (treeTemp.currentCrossFadeId == -1)
                                            {
                                                manager.treesCrossFade.Add(treeTemp);
                                                treeTemp.currentCrossFadeId = 6;
                                                treeTemp.currentCrossFadeLOD = -1;

                                                treeTemp.go = manager.getObjPool(treeTemp.idPrototype, newLOD, altTreesID, treeTemp.idTree, treeTemp.color, treeTemp.colorBark);
                                                treeTemp.go.GetComponent<AltTreeInstance>().isCrossFade = true;
                                                treeTemp.crossFadeTreeMeshRenderer = treeTemp.go.GetComponent<MeshRenderer>();
                                                treeTemp.crossFadeTreeMeshRenderer.sharedMaterials = manager.treesPoolArray[treeTemp.idPrototypeIndex].objsArray[newLOD].materialsMeshCrossFade;

                                                propBlock.Clear();
                                                propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, treeTemp.color);
                                                propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, treeTemp.colorBark);
                                                propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 0.0f);
                                                propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                                                propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 0.0f);
                                                treeTemp.crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);


                                                if (Application.isPlaying)
                                                    treeTemp.crossFadeTime = Time.time + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                else
                                                {
    #if UNITY_EDITOR
                                                    {
                                                        treeTemp.crossFadeTime = (float)EditorApplication.timeSinceStartup + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                    }
    #endif
                                                }

                                                scaleTemp.x = treeTemp.widthScale;
                                                scaleTemp.y = treeTemp.heightScale;
                                                scaleTemp.z = treeTemp.widthScale;
                                                treeTemp.go.transform.localScale = scaleTemp;
                                                treeTemp.go.transform.position = treeTemp.getPosWorld();
                                                treeTemp.go.transform.rotation = Quaternion.AngleAxis(treeTemp.rotation, Vector3.up);
                                                treeTemp.go.SetActive(true);
                                                manager.treesList.Add(treeTemp.go, treeTemp);
                                            }
                                            else
                                            {
                                                if (treeTemp.currentCrossFadeId == 6)
                                                {
                                                    Debug.Log("Error. treeTemp.currentCrossFadeId == 6");
                                                }
                                                else if (treeTemp.currentCrossFadeId == 5)
                                                {
                                                    treeTemp.crossFadeTreeMeshRenderer.sharedMaterials = manager.treesPoolArray[treeTemp.idPrototypeIndex].objsArray[newLOD].materialsMesh;
                                                    propBlock.Clear();
                                                    propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, treeTemp.color);
                                                    propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, treeTemp.colorBark);
                                                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 1.0f);
                                                    propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                                                    propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 0.0f);
                                                    treeTemp.crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);

                                                    treeTemp.goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = false;
                                                    manager.delObjPool(treeTemp.idPrototype, treeTemp.currentCrossFadeLOD, treeTemp.goCrossFade);
                                                    treeTemp.go = manager.getObjPool(treeTemp.idPrototype, newLOD, altTreesID, treeTemp.idTree, treeTemp.color, treeTemp.colorBark);

                                                    treeTemp.go.GetComponent<AltTreeInstance>().isCrossFade = true;
                                                    treeTemp.crossFadeTreeMeshRenderer = treeTemp.go.GetComponent<MeshRenderer>();
                                                    treeTemp.crossFadeTreeMeshRenderer.sharedMaterials = manager.treesPoolArray[treeTemp.idPrototypeIndex].objsArray[newLOD].materialsMeshCrossFade;

                                                    propBlock.Clear();
                                                    propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, treeTemp.color);
                                                    propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, treeTemp.colorBark);
                                                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 0.0f);
                                                    propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                                                    propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 0.0f);
                                                    treeTemp.crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);

                                                    if (Application.isPlaying)
                                                        treeTemp.crossFadeTime = Time.time + (manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard - (treeTemp.crossFadeTime - Time.time));
                                                    else
                                                    {
    #if UNITY_EDITOR
                                                        {
                                                            treeTemp.crossFadeTime = (float)EditorApplication.timeSinceStartup + (manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard - (treeTemp.crossFadeTime - (float)EditorApplication.timeSinceStartup));
                                                        }
    #endif
                                                    }
                                                    treeTemp.currentCrossFadeId = 6;
                                                    treeTemp.currentCrossFadeLOD = -1;

                                                    scaleTemp.x = treeTemp.widthScale;
                                                    scaleTemp.y = treeTemp.heightScale;
                                                    scaleTemp.z = treeTemp.widthScale;
                                                    treeTemp.go.transform.localScale = scaleTemp;
                                                    treeTemp.go.transform.position = treeTemp.getPosWorld();
                                                    treeTemp.go.transform.rotation = Quaternion.AngleAxis(treeTemp.rotation, Vector3.up);
                                                    treeTemp.go.SetActive(true);
                                                    manager.treesList.Add(treeTemp.go, treeTemp);
                                                }
                                                else if (treeTemp.currentCrossFadeId == 7)
                                                {
                                                    treeTemp.go = manager.getObjPool(treeTemp.idPrototype, newLOD, altTreesID, treeTemp.idTree, treeTemp.color, treeTemp.colorBark);

                                                    treeTemp.go.GetComponent<AltTreeInstance>().isCrossFade = true;
                                                    treeTemp.crossFadeTreeMeshRenderer = treeTemp.go.GetComponent<MeshRenderer>();
                                                    treeTemp.crossFadeTreeMeshRenderer.sharedMaterials = manager.treesPoolArray[treeTemp.idPrototypeIndex].objsArray[newLOD].materialsMeshCrossFade;

                                                    propBlock.Clear();
                                                    propBlock.SetColor(AltTreesManager.HueVariationLeave_PropertyID, treeTemp.color);
                                                    propBlock.SetColor(AltTreesManager.HueVariationBark_PropertyID, treeTemp.colorBark);
                                                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 0.0f);
                                                    propBlock.SetFloat(AltTreesManager.Ind_PropertyID, 0.0f);
                                                    propBlock.SetFloat(AltTreesManager.smoothValue_PropertyID, 0.0f);
                                                    treeTemp.crossFadeTreeMeshRenderer.SetPropertyBlock(propBlock);

                                                    treeTemp.currentCrossFadeId = 1;
                                                    treeTemp.currentCrossFadeLOD = -1;

                                                    scaleTemp.x = treeTemp.widthScale;
                                                    scaleTemp.y = treeTemp.heightScale;
                                                    scaleTemp.z = treeTemp.widthScale;
                                                    treeTemp.go.transform.localScale = scaleTemp;
                                                    treeTemp.go.transform.position = treeTemp.getPosWorld();
                                                    treeTemp.go.transform.rotation = Quaternion.AngleAxis(treeTemp.rotation, Vector3.up);
                                                    treeTemp.go.SetActive(true);
                                                    manager.treesList.Add(treeTemp.go, treeTemp);
                                                }
                                                else if (isNoBillboardCrossFade)
                                                {
                                                    Debug.Log("Error. treeTemp.currentCrossFadeId == " + treeTemp.currentCrossFadeId);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            treeTemp.go = manager.getObjPool(treeTemp.idPrototype, newLOD, altTreesID, treeTemp.idTree, treeTemp.color, treeTemp.colorBark);
                                            scaleTemp.x = treeTemp.widthScale;
                                            scaleTemp.y = treeTemp.heightScale;
                                            scaleTemp.z = treeTemp.widthScale;
                                            treeTemp.go.transform.localScale = scaleTemp;
                                            treeTemp.go.transform.position = treeTemp.getPosWorld();
                                            treeTemp.go.transform.rotation = Quaternion.AngleAxis(treeTemp.rotation, Vector3.up);
                                            manager.treesList.Add(treeTemp.go, treeTemp);
                                        }
                                    }
                                    else if (newLOD == -2)               //  newLOD is billboard
                                    {
                                        if (Application.isPlaying && manager.altTreesMain.altTreesManagerData.enableColliders)
                                        {
                                            if (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.isBillboardColliders)
                                            {
                                                treeTemp.collider = manager.getColliderPool(treeTemp.idPrototype, true);
                                                scaleTemp.x = treeTemp.widthScale;
                                                scaleTemp.y = treeTemp.heightScale;
                                                scaleTemp.z = treeTemp.widthScale;
                                                treeTemp.collider.transform.localScale = scaleTemp;
                                                treeTemp.collider.transform.position = treeTemp.getPosWorld();
                                                treeTemp.collider.transform.rotation = Quaternion.AngleAxis(treeTemp.rotation, Vector3.up);
                                            }
                                        }

                                        if (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.isBillboardCrossFade && !manager.isSelectionTree)
                                        {
                                            if (treeTemp.currentCrossFadeId == -1)
                                            {
                                                manager.treesCrossFade.Add(treeTemp);
                                                treeTemp.currentCrossFadeId = 8;
                                                treeTemp.currentCrossFadeLOD = -1;
                                                treeTemp.go = manager.getObjBillboardPool(treeTemp.idPrototype, treeTemp.widthScale, treeTemp.heightScale, treeTemp.rotation, treeTemp.color);
                                                treeTemp.crossFadeBillboardMeshRenderer = treeTemp.go.GetComponent<MeshRenderer>();

                                                treeTemp.crossFadeBillboardMeshRenderer.sharedMaterial = manager.treesPoolArray[treeTemp.idPrototypeIndex].materialBillboardCrossFade;
                                                propBlock.Clear();
                                                treeTemp.crossFadeBillboardMeshRenderer.GetPropertyBlock(propBlock);
                                                propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 0.0f);
                                                treeTemp.crossFadeBillboardMeshRenderer.SetPropertyBlock(propBlock);

                                                if (Application.isPlaying)
                                                    treeTemp.crossFadeTime = Time.time + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                else
                                                {
    #if UNITY_EDITOR
                                                    {
                                                        treeTemp.crossFadeTime = (float)EditorApplication.timeSinceStartup + manager.altTreesMain.altTreesManagerData.crossFadeTimeBillboard;
                                                    }
    #endif
                                                }
                                                treeTemp.go.transform.localScale = Vector3.one;
                                                treeTemp.go.transform.position = treeTemp.getPosWorld() + new Vector3(0f, manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale / 2f /*- (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale - manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale / manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.space) / 2f*/ + manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.up * treeTemp.heightScale, 0f);
                                                treeTemp.go.transform.rotation = Quaternion.identity;
                                                treeTemp.go.SetActive(true);
                                                manager.treesList.Add(treeTemp.go, treeTemp);
                                            }
                                            else
                                            {
                                                if (treeTemp.currentCrossFadeId == 5)
                                                {
                                                    treeTemp.currentCrossFadeId = 2;

                                                    treeTemp.go = manager.getObjBillboardPool(treeTemp.idPrototype, treeTemp.widthScale, treeTemp.heightScale, treeTemp.rotation, treeTemp.color);
                                                    treeTemp.crossFadeBillboardMeshRenderer = treeTemp.go.GetComponent<MeshRenderer>();
                                                
                                                    treeTemp.crossFadeBillboardMeshRenderer.sharedMaterial = manager.treesPoolArray[treeTemp.idPrototypeIndex].materialBillboardCrossFade;
                                                    propBlock.Clear();
                                                    treeTemp.crossFadeBillboardMeshRenderer.GetPropertyBlock(propBlock);
                                                    propBlock.SetFloat(AltTreesManager.Alpha_PropertyID, 0.0f);
                                                    treeTemp.crossFadeBillboardMeshRenderer.SetPropertyBlock(propBlock);


                                                    treeTemp.go.transform.localScale = Vector3.one;
                                                    treeTemp.go.transform.position = treeTemp.getPosWorld() + new Vector3(0f, manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale / 2f /*- (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale - manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale / manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.space) / 2f*/ + manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.up * treeTemp.heightScale, 0f);
                                                    treeTemp.go.transform.rotation = Quaternion.identity;
                                                    treeTemp.go.SetActive(true);
                                                    manager.treesList.Add(treeTemp.go, treeTemp);
                                                }
                                                else if (treeTemp.currentCrossFadeId == 7)
                                                {
                                                    treeTemp.currentCrossFadeId = 8;
                                                    treeTemp.currentCrossFadeLOD = -1;
                                                    treeTemp.go = treeTemp.goCrossFade;
                                                }
                                                else if (treeTemp.currentCrossFadeId == 8)
                                                {
                                                    Debug.Log("Error. treeTemp.currentCrossFadeId == 8");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            treeTemp.go = manager.getObjBillboardPool(treeTemp.idPrototype, treeTemp.widthScale, treeTemp.heightScale, treeTemp.rotation, treeTemp.color);

                                            treeTemp.go.transform.localScale = Vector3.one;
                                            treeTemp.go.transform.position = treeTemp.getPosWorld() + new Vector3(0f, manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale / 2f /*- (manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale - manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.size * treeTemp.heightScale / manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.space) / 2f*/ + manager.treesPoolArray[treeTemp.idPrototypeIndex].tree.up * treeTemp.heightScale, 0f);
                                            treeTemp.go.transform.rotation = Quaternion.identity;
                                            treeTemp.go.SetActive(true);
                                            manager.treesList.Add(treeTemp.go, treeTemp);
                                        }
                                    }
                                }

                                treeTemp.currentLOD = newLOD;
                            }
                        }
                    }
                }
            }
        }

        void deleteTrees()
        {
            bool isStop = false;

            for (int i = 0; i < treesCount; i++)
            {
                isStop = false;

                if (!Application.isPlaying && trees[i].currentLOD != -1)
                {
                    if (manager.isSelectionTree)
                    {
                        for (int j = 0; j < manager.cameras.Length; j++)
                        {
                            if (manager.cameras[j].Equals(trees[i].go.transform))
                            {
                                isStop = true;
                                break;
                            }
                        }
                    }
                }

                if (!isStop)
                {
                    deleteTreeCheckCrossFade(trees[i]);
                }
            }

            for (int i = 0; i < treesNoBillbCount; i++)
            {
                isStop = false;

                if (!Application.isPlaying && treesNoBillb[i].currentLOD != -1)
                {
                    if (manager.isSelectionTree)
                    {
                        for (int j = 0; j < manager.cameras.Length; j++)
                        {
                            if (manager.cameras[j].Equals(treesNoBillb[i].go.transform))    //!
                            {
                                isStop = true;
                                break;
                            }
                        }
                    }
                }

                if (!isStop)
                {
                    deleteTreeCheckCrossFade(treesNoBillb[i]);
                }
            }
        }


        void deleteTreeCheckCrossFade(AltTreesTrees att)
        {
            if (Application.isPlaying && manager.altTreesMain.altTreesManagerData.enableColliders)
            {
                if (att.currentLOD != -1)
                {
                    if (att.currentLOD == -2)
                    {
                        if (manager.treesPoolArray[att.idPrototypeIndex].tree.isBillboardColliders)
                            manager.delColliderPool(att.idPrototype, att.collider, true);
                    }
                    else if (att.currentLOD != -3)
                    {
                        if (manager.treesPoolArray[att.idPrototypeIndex].tree.isColliders)
                            manager.delColliderPool(att.idPrototype, att.collider, false);  //11
                    }
                }
            }

            if (att.currentLOD != -1)
            {

                if (att.currentCrossFadeId == -1 && att.currentLOD != -3)
                {
                    manager.treesList.Remove(att.go);

                    if (att.currentLOD != -2)
                        manager.delObjPool(att.idPrototype, att.currentLOD, att.go);
                    else
                        manager.delObjBillboardPool(att.idPrototype, att.go);
                }
                else if (att.currentCrossFadeId == 1)
                {
                    att.go.GetComponent<AltTreeInstance>().isCrossFade = false;
                    manager.delObjBillboardPool(att.idPrototype, att.goCrossFade);
                    manager.delObjPool(att.idPrototype, att.currentLOD, att.go);
                    att.goCrossFade = null;
                    att.go = null;

                    att.crossFadeBillboardMeshRenderer = null;
                    att.crossFadeTreeMeshRenderer = null;
                    att.currentCrossFadeId = -1;
                    manager.treesCrossFade.Remove(att);
                }
                else if (att.currentCrossFadeId == 6)
                {
                    att.go.GetComponent<AltTreeInstance>().isCrossFade = false;
                    manager.delObjPool(att.idPrototype, att.currentLOD, att.go);
                    att.goCrossFade = null;
                    att.go = null;

                    att.crossFadeBillboardMeshRenderer = null;
                    att.crossFadeTreeMeshRenderer = null;
                    att.currentCrossFadeId = -1;
                    manager.treesCrossFade.Remove(att);
                }
                else if (att.currentCrossFadeId == 7)
                {
                    manager.delObjBillboardPool(att.idPrototype, att.goCrossFade);
                    att.goCrossFade = null;
                    att.go = null;

                    att.crossFadeBillboardMeshRenderer = null;
                    att.crossFadeTreeMeshRenderer = null;
                    att.currentCrossFadeId = -1;
                    manager.treesCrossFade.Remove(att);
                }
                else if (att.currentCrossFadeId == 2)
                {
                    att.goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = false;
                    manager.delObjPool(att.idPrototype, att.currentCrossFadeLOD, att.goCrossFade);
                    manager.delObjBillboardPool(att.idPrototype, att.go);
                    att.go = null;
                    att.goCrossFade = null;

                    att.crossFadeBillboardMeshRenderer = null;
                    att.crossFadeTreeMeshRenderer = null;
                    att.currentCrossFadeId = -1;
                    manager.treesCrossFade.Remove(att);
                }
                else if (att.currentCrossFadeId == 5)
                {
                    att.goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = false;
                    manager.delObjPool(att.idPrototype, att.currentCrossFadeLOD, att.goCrossFade);
                    att.go = null;
                    att.goCrossFade = null;

                    att.crossFadeBillboardMeshRenderer = null;
                    att.crossFadeTreeMeshRenderer = null;
                    att.currentCrossFadeId = -1;
                    manager.treesCrossFade.Remove(att);
                }
                else if (att.currentCrossFadeId == 8)
                {
                    manager.delObjBillboardPool(att.idPrototype, att.go);
                    att.go = null;
                    att.goCrossFade = null;

                    att.crossFadeBillboardMeshRenderer = null;
                    att.crossFadeTreeMeshRenderer = null;
                    att.currentCrossFadeId = -1;
                    manager.treesCrossFade.Remove(att);
                }
                else if (att.currentCrossFadeId == 3)
                {
                    att.go.SetActive(true);
                    att.goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = false;
                    att.goCrossFade.GetComponent<AltTreeInstance>().isCrossFade = false;
                    manager.delObjPool(att.idPrototype, att.currentCrossFadeLOD, att.goCrossFade);
                    manager.delObjPool(att.idPrototype, att.currentLOD, att.go);
                    att.goCrossFade = null;
                    att.go = null;

                    att.crossFadeTreeMeshRenderer = null;
                    att.currentCrossFadeId = -1;
                    manager.treesCrossFade.Remove(att);
                }
                else if (att.currentCrossFadeId == 4)
                {
                    att.go = att.goCrossFade;
                    att.go.GetComponent<AltTreeInstance>().isCrossFade = false;
                    manager.delObjPool(att.idPrototype, att.currentLOD, att.go);
                    att.goCrossFade = null;
                    att.go = null;

                    att.crossFadeTreeMeshRenderer = null;
                    att.currentCrossFadeId = -1;
                    manager.treesCrossFade.Remove(att);
                }
            
                att.countCheckLODs = 0;
                att.currentLOD = -1;
            }
        }


        public void checkTreesAdd(float _posX, float _posZ, AltTreesTrees _tree, bool groupBillb = true)
	    {
            if (bound.inBounds(_posX, _posZ, quadId))
		    {
                if (groupBillb)
                {
                    if (!treePrefabsCount.ContainsKey(_tree.idPrototype))
                        treePrefabsCount.Add(_tree.idPrototype, 0);
                    treePrefabsCount[_tree.idPrototype]++;
                    treesCount++;
                    trees.Add(_tree);
                    isInit = false;
                    isRender = false;
                }
                else if(LOD == maxLOD)
                {
                    treesNoBillbCount++;
                    treesNoBillb.Add(_tree);
                }

                if (isChildQuads)
                {
                    quads[0].checkTreesAdd(_posX, _posZ, _tree, groupBillb);
                    quads[1].checkTreesAdd(_posX, _posZ, _tree, groupBillb);
                    quads[2].checkTreesAdd(_posX, _posZ, _tree, groupBillb);
                    quads[3].checkTreesAdd(_posX, _posZ, _tree, groupBillb);
                }
		    }
	    }
    
        public void removeRenders()
        {
            for (int i = 0; i < renders.Count; i++)
            {
                if (Application.isPlaying)
                {
                    if(renders[i].GetComponent<MeshFilter>()!=null && renders[i].GetComponent<MeshFilter>().sharedMesh != null)
                        Object.Destroy(renders[i].GetComponent<MeshFilter>().sharedMesh);
                    Object.Destroy(renders[i]);
                }
                else
                {
                    if (renders[i].GetComponent<MeshFilter>() != null && renders[i].GetComponent<MeshFilter>().sharedMesh != null)
                        Object.DestroyImmediate(renders[i].GetComponent<MeshFilter>().sharedMesh);
                    Object.DestroyImmediate(renders[i]);
                }
            }

            if (isChildQuads)
            {
                quads[0].removeRenders();
                quads[1].removeRenders();
                quads[2].removeRenders();
                quads[3].removeRenders();
            }
        }
	
	
	
    }

    public class Bounds2D
    {
	    float left;
	    float right;
	    float up;
	    float down;
	
	    public Bounds2D(Vector2 pos, float size)
	    {
		    left = pos.x - size/2f;
		    right = pos.x + size/2f;
		
		    up = pos.y + size/2f;
		    down = pos.y - size/2f;
	    }

        public Bounds2D(float _left, float _right, float _up, float _down)
        {
            left = _left;
            right = _right;
            up = _up;
            down = _down;
        }
	
	    public bool inBounds(float _posX, float _posY, int quadId = 0)
	    {
            if ((_posX > left && _posX <= right) && (_posY > down && _posY <= up))
                return true;
            else
                return false;
	    }

        public bool inBounds(Vector2 _pos)
        {
            if ((_pos.x > left && _pos.x <= right) && (_pos.y > down && _pos.y <= up))
                return true;
            else
                return false;
        }

        public bool isIntersection(Bounds2D bound)
        {
            if (left > bound.right || right < bound.left || up < bound.down || down > bound.up)
                return false;
            else
                return true;
        }
    }



    public class AltTreesPool
    {
	    public AltTree tree;
        public objsArr[] objsArray = new objsArr[0];
        public List<GameObject> collidersArray = new List<GameObject>();
        public List<GameObject> colliderBillboardsArray = new List<GameObject>();
        public Material materialBillboard;
        public Material materialBillboardCrossFade;
        public Material materialBillboardGroup;
    }

    public class objsArr
    {
	    public List<GameObject> objs = new List<GameObject>();
        public Material[] materialsMesh;
        public Material[] materialsMeshCrossFade;
    }

    public class objBillboardPool
    {
        public Mesh ms;
        public GameObject go;
        public MeshRenderer mr;
    }
}




