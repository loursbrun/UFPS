using UnityEngine;
using UnityEditor;


namespace AltSystems.AltTrees.Editor
{
    [CustomEditor(typeof(AltWind))]
    public class AltWind_Editor : UnityEditor.Editor
    {
        AltWind obj = null;

        SerializedProperty power;
        SerializedProperty speedCurve;
        SerializedProperty speedFrequency;

        SerializedProperty gustEnabled;
        SerializedProperty gustCurve;
        SerializedProperty gustTime;
        SerializedProperty gustTimeFading;
        SerializedProperty gustFrequencyMin;
        SerializedProperty gustFrequencyMax;
        
        SerializedProperty heightFactorEnabled;
        SerializedProperty heightFactorCurve;

        GUIStyle sty;


        public void OnEnable()
        {
            obj = (AltWind)target;

            power = serializedObject.FindProperty("power");
            speedCurve = serializedObject.FindProperty("speedCurve");
            speedFrequency = serializedObject.FindProperty("speedFrequency");


            gustEnabled = serializedObject.FindProperty("gustEnabled");
            gustCurve = serializedObject.FindProperty("gustCurve");
            gustTime = serializedObject.FindProperty("gustTime");
            gustTimeFading = serializedObject.FindProperty("gustTimeFading");
            gustFrequencyMin = serializedObject.FindProperty("gustFrequencyMin");
            gustFrequencyMax = serializedObject.FindProperty("gustFrequencyMax");

            heightFactorEnabled = serializedObject.FindProperty("heightFactorEnabled");
            heightFactorCurve = serializedObject.FindProperty("heightFactorCurve");

            sty = new GUIStyle();
            sty.fontStyle = FontStyle.Bold;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            power.floatValue = EditorGUILayout.Slider("Power Wind:", power.floatValue, 0f, 1f);
            speedCurve.animationCurveValue = EditorGUILayout.CurveField("Speed Wind:", speedCurve.animationCurveValue);
            speedFrequency.floatValue = EditorGUILayout.Slider("Speed Change Time:", speedFrequency.floatValue, 0.01f, 1.0f);

            EditorGUILayout.Space();

            gustEnabled.boolValue = EditorGUILayout.Toggle("Gust Enabled: ", gustEnabled.boolValue);
            gustCurve.animationCurveValue = EditorGUILayout.CurveField("Gust Wind:", gustCurve.animationCurveValue);
            gustTime.floatValue = EditorGUILayout.Slider("Gust Change Time:", gustTime.floatValue, 0.1f, 10.0f);
            gustTimeFading.floatValue = EditorGUILayout.Slider("Gust Fading:", gustTimeFading.floatValue, 0.01f, 1);
            float min = gustFrequencyMin.floatValue;
            float max = gustFrequencyMax.floatValue;

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Gust Frequency: ");
                EditorGUILayout.MinMaxSlider(ref min, ref max, 0f, 50f);

                if (min != gustFrequencyMin.floatValue || gustFrequencyMax.floatValue != max)
                {
                    gustFrequencyMin.floatValue = min;
                    gustFrequencyMax.floatValue = max;
                }

                GUILayout.Label(min.ToString("0.0") + " - " + max.ToString("0.0"));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            heightFactorEnabled.boolValue = EditorGUILayout.Toggle("Height Factor Enabled: ", heightFactorEnabled.boolValue);
            heightFactorCurve.animationCurveValue = EditorGUILayout.CurveField("Height Factor:", heightFactorCurve.animationCurveValue);

            
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("Debug Info:", sty);
                EditorGUILayout.LabelField("Current Wind Speed: ", "" + obj.speed.ToString("0.00") + "     (1 - max)");
                EditorGUILayout.LabelField("Global Wind Speed: ", "" + AltWind.getSpeed().ToString("0.00") + "     (1 - max)");
                EditorGUILayout.LabelField("Current Gust: ", "" + obj.currentGust.ToString("0.00"));
            }
            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}