using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif


namespace AltSystems.AltTrees
{
    [ExecuteInEditMode]
    public class AltWind : MonoBehaviour
    {
        public float power = 0.3f;
        public AnimationCurve speedCurve = new AnimationCurve(new Keyframe(0, 0.5f), new Keyframe(1, 1));
        public float speedFrequency = 0.1f;

        public bool gustEnabled = true;
        public AnimationCurve gustCurve = new AnimationCurve(new Keyframe(0, 0.3f), new Keyframe(1, 1.0f));
        public float gustTime = 0.7f;
        public float gustTimeFading = 0.15f;
        public float gustFrequencyMin = 10.0f;
        public float gustFrequencyMax = 20.0f;

        public bool heightFactorEnabled = false;
        public AnimationCurve heightFactorCurve = new AnimationCurve(new Keyframe(0, 1f), new Keyframe(500, 3f)); 

        void OnEnable()
        {
            addWind(this);
        }
        void OnDisable()
        {
            removeWind(this);

            currentGust = 0f;
            targetGust = 0f;
            currentSpeed = 0f;
            targetSpeed = 0f;
            isPlus = false;
            direction = Vector3.zero;
            speed = 0;
        }

        void Update()
        {
            if (Application.isPlaying)
                UpdateFunk();
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                UpdateFunk();
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color32(0, 213, 255, 255);
            Gizmos.DrawLine(transform.TransformPoint(1f, 0, -2f), transform.TransformPoint(1f, 0, 1));
            Gizmos.DrawLine(transform.TransformPoint(-1f, 0, -2f), transform.TransformPoint(-1f, 0, 1));
        
            Gizmos.DrawLine(transform.TransformPoint(1f, 0, -2f), transform.TransformPoint(-1f, 0, -2f));

            Gizmos.DrawLine(transform.TransformPoint(1f, 0, 1f), transform.TransformPoint(2f, 0, 1f));
            Gizmos.DrawLine(transform.TransformPoint(-1f, 0, 1f), transform.TransformPoint(-2f, 0, 1f));

            Gizmos.DrawLine(transform.TransformPoint(-2f, 0, 1f), transform.TransformPoint(0f, 0, 3f));
            Gizmos.DrawLine(transform.TransformPoint(2f, 0, 1f), transform.TransformPoint(0f, 0, 3f));


            Gizmos.DrawLine(transform.TransformPoint(0f, 0, -2f), transform.TransformPoint(0f, 0, 3f));


            Gizmos.DrawLine(transform.TransformPoint(0, 1f, -2f), transform.TransformPoint(0, 1f, 1));
            Gizmos.DrawLine(transform.TransformPoint(0, -1f, -2f), transform.TransformPoint(0, -1f, 1));

            Gizmos.DrawLine(transform.TransformPoint(0f, 1f, -2f), transform.TransformPoint(0f, -1f, -2f));

            Gizmos.DrawLine(transform.TransformPoint(0, 1f, 1f), transform.TransformPoint(0, 2f, 1f));
            Gizmos.DrawLine(transform.TransformPoint(0, -1f, 1f), transform.TransformPoint(0, -2f, 1f));

            Gizmos.DrawLine(transform.TransformPoint(0, -2f, 1f), transform.TransformPoint(0f, 0, 3f));
            Gizmos.DrawLine(transform.TransformPoint(0, 2f, 1f), transform.TransformPoint(0f, 0, 3f));
        }

        static List<AltWind> windList = new List<AltWind>();

        static void addWind(AltWind _wind)
        {
            if (!windList.Contains(_wind))
                windList.Add(_wind);

            lastUpdate = 0f;
            UpdateFunkStatic();
        }

        static void removeWind(AltWind _wind)
        {
            if (windList.Contains(_wind))
                windList.Remove(_wind);

            lastUpdate = 0f;
            UpdateFunkStatic();
        }

        static public float getSpeed()
        {
            return (speedGlobal > 1f) ? 1f : speedGlobal;
        }

        static public Vector3 getDirection()
        {
            return directionGlobal;
        }

        static float lastUpdate = 0f;
        static float speedGlobal = 0f;
        static Vector3 directionGlobal = new Vector3();
        static float tempTime = 0;

        static Matrix4x4 m1;
        static Matrix4x4 m2;
        static Matrix4x4 tt;
        static Vector3 vectTemp = new Vector3();


        float currentSpeed = 0f;
        float targetSpeed = 0f;
        bool isPlus = false;

        public float currentGust = 0f;
        float targetGust = 0f;
        float timeGust = 0;
        bool isOffGust = false;

        float heightFactor = 1f;
        Camera cameraTemp;

        public float speed = 0f;
        public Vector3 direction = new Vector3();

        float tempDeltaTime = 0f;
        #if UNITY_EDITOR
            double tempDeltaTimeStar = 0;
        #endif


        void UpdateFunk()
        {
            if (!this.enabled)
                return;
            #if UNITY_EDITOR
            {
                if (!Application.isPlaying)
                {
                    tempDeltaTime = (float)(EditorApplication.timeSinceStartup - tempDeltaTimeStar);
                    tempDeltaTimeStar = EditorApplication.timeSinceStartup;
                    tempTime = (float)EditorApplication.timeSinceStartup;

                    if (tempDeltaTime > 0.3f)
                        tempDeltaTime = 0.3f;

                    cameraTemp = SceneView.lastActiveSceneView.camera; ;
                }
            }
            #endif
            if (Application.isPlaying)
            {
                tempDeltaTime = Time.deltaTime;
                tempTime = Time.time;

                cameraTemp = Camera.main;
            }


            if ((isPlus && currentSpeed < targetSpeed) || (!isPlus && currentSpeed > targetSpeed))
            {
                if (isPlus)
                    currentSpeed += tempDeltaTime * speedFrequency;
                else
                    currentSpeed -= tempDeltaTime * speedFrequency;
            }
            else
            {
                targetSpeed = speedCurve.Evaluate(Random.value * (speedCurve.keys[speedCurve.length - 1].time - speedCurve.keys[0].time));

                if (targetSpeed > currentSpeed)
                    isPlus = true;
                else
                    isPlus = false;
            }

            if (gustEnabled)
            {
                if (!isOffGust)
                {
                    if (currentGust < targetGust)
                    {
                        currentGust += tempDeltaTime * gustTime;
                    }
                    else
                        isOffGust = true;
                }
                else
                {
                    if (currentGust > 0f)
                        currentGust -= tempDeltaTime * gustTime * gustTimeFading;

                    if (currentGust < 0f)
                        currentGust = 0f;
                }
                
                if (timeGust <= tempTime)
                {
                    targetGust = gustCurve.Evaluate(Random.value * (gustCurve.keys[gustCurve.length - 1].time - gustCurve.keys[0].time));

                    isOffGust = false;
                    timeGust = tempTime + gustFrequencyMin + Random.value * (gustFrequencyMax - gustFrequencyMin);
                }
            }
            else
            {
                currentGust = 0f;
                targetGust = 0f;
                isOffGust = true;
            }

            if (heightFactorEnabled && cameraTemp != null)
                heightFactor = heightFactorCurve.Evaluate(Mathf.Clamp(cameraTemp.transform.position.y, heightFactorCurve.keys[0].time, heightFactorCurve.keys[heightFactorCurve.length - 1].time));
            else
                heightFactor = 1;

            speed = Mathf.Clamp01((currentSpeed + currentGust) * power * heightFactor);
            

            m1 = Matrix4x4.TRS(Vector3.zero, transform.rotation, Vector3.one);
            m2 = Matrix4x4.TRS(Vector3.forward, Quaternion.identity, Vector3.zero);
            tt = m1 * m2;

            vectTemp.x = tt.m03;
            vectTemp.y = tt.m13;
            vectTemp.z = tt.m23;

            direction = vectTemp.normalized;


            UpdateFunkStatic();
        }
        
        static void UpdateFunkStatic()
        {
            if (lastUpdate + 0.01f > tempTime)
                return;

            if (windList.Count > 0)
            {
                vectTemp = Vector3.zero;

                for (int i = 0; i < windList.Count; i++)
                {
                    vectTemp += windList[i].direction * windList[i].speed;
                }

                speedGlobal = vectTemp.magnitude;
                directionGlobal = vectTemp.normalized;
            }
            else
            {
                speedGlobal = 0f;

                directionGlobal.x = 0f;
                directionGlobal.y = 0f;
                directionGlobal.z = 1f;
            }

            lastUpdate = tempTime;
        }
    }
}