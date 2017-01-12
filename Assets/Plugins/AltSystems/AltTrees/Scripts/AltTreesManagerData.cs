using UnityEngine;

namespace AltSystems.AltTrees
{
    public class AltTreesManagerData : ScriptableObject
    {
        public AltTreesPatch[] patches = new AltTreesPatch[0];

        public bool draw = true;
        public bool generateAllBillboardsOnStart = false;
        public bool enableColliders = true;

        public bool autoConfig = true;
        public int sizePatch = 1000;
        public int maxLOD = 4;
        public float distancePatchFactor = 1.5f;
        public float distanceTreesLODFactor = 1f;
        public float distanceObjectsLODFactor = 1f;
        public float checkTreesPercentPerFrame = 10f;
        public float crossFadeTimeBillboard = 0.4f;
        public float crossFadeTimeMesh = 0.4f;

        public int initObjsCountPool = 100;
        public int objsPerOneMaxPool = 150;

        public int initBillboardCountPool = 1500;
        public int billboardsMaxPool = 2000;

        public int initCollidersCountPool = 50;
        public int collidersPerOneMaxPool = 70;
        public int initColliderBillboardsCountPool = 40;
        public int colliderBillboardsPerOneMaxPool = 60;


        public bool drawDebugPutches = false;
        public bool drawDebugPutchesStar = false;
        public bool drawDebugBillboards = false;
        public bool drawDebugBillboardsStar = false;
        public bool debugLog = true;
        public bool debugLogInBilds = false;
        public bool hideGroupBillboards = false;
    }
}