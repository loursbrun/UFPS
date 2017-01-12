using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace AltSystems.AltTrees.Editor
{
    [CustomEditor(typeof(AltTree))]
    public class AltTree_Editor : UnityEditor.Editor
    {
        AltTree obj = null;
        AltTreesDataLinks dataLinks = null;

        SerializedProperty drawPlaneBillboard;
        SerializedProperty hueVariationLeaves;
        SerializedProperty hueVariationBark;
        SerializedProperty color;
        SerializedProperty isObject;
        SerializedProperty specularColor;
        

        SerializedProperty isMeshCrossFade;
        SerializedProperty isBillboardCrossFade;
    
        Color hueVariationLeavesNew;
        Color hueVariationBarkNew;
        Color colorNew;
        Color specularColorNew;
        bool isObjectNew;
        bool isMeshCrossFadeNew;
        bool isBillboardCrossFadeNew;
        bool drawPlaneBillboardNew;

        bool showMaterials = false;
        bool showBillboardSettings = false;
        bool showShaders = false;

        GUIStyle style;
        GUIStyle sty;
        GUIStyle sty2;
        GUIStyle sty3;
        GUIStyle sty4;
        GUIStyle sty5;
        GUIStyle styGreen;
        GUIStyle styRed;
        GUIStyle sty6;
        Texture2D textur;

        static List<Material> matsBark = new List<Material>();
        static List<Material> matsLeaves = new List<Material>();

        int popupWindParams = 0;

        public void OnEnable()
        {
            obj = (AltTree)target;

            obj.checkVersionTree(true);

            isPrefab = AssetDatabase.GetAssetPath(obj) != "";

            if(isPrefab)
            {
                getDataLinks();

                if (dataLinksCorrupted)
                    return;

                AltTree altT = dataLinks.getAltTree(obj.id);

                if(altT == null)
                {
                    string[] filesTemp = Directory.GetFiles("Assets", obj.name + ".spm", SearchOption.AllDirectories);
                    GameObject goT = null;
                    if (filesTemp.Length > 0 && filesTemp[0].Length > 0)
                        goT = (GameObject)AssetDatabase.LoadAssetAtPath(filesTemp[0], typeof(GameObject));

                    dataLinks.addTree(goT, obj, obj.id);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    EditorUtility.SetDirty(dataLinks);
                }
            }

            style = new GUIStyle();
            style.fontSize = 30;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;

            sty = new GUIStyle();
            sty.fontStyle = FontStyle.Bold;

            sty2 = new GUIStyle();
            sty2.alignment = TextAnchor.MiddleRight;
            sty2.margin.top = 3;

            sty3 = new GUIStyle();
            sty3.alignment = TextAnchor.MiddleCenter;
            sty3.clipping = TextClipping.Clip;
            sty3.padding.bottom = 12;
            sty3.fontSize = 13;

            sty4 = new GUIStyle();
            sty4.alignment = TextAnchor.MiddleCenter;
            sty4.clipping = TextClipping.Clip;
            sty4.padding.top = 14;
            sty4.fontSize = 9;

            sty5 = new GUIStyle();
            sty5.richText = true;

            styGreen = new GUIStyle();
            styGreen.normal.textColor = new Color32(46,143,0,255);

            styRed = new GUIStyle();
            styRed.normal.textColor = Color.red;

            sty6 = new GUIStyle();


            drawPlaneBillboard = serializedObject.FindProperty("drawPlaneBillboard");
            hueVariationLeaves = serializedObject.FindProperty("hueVariationLeaves");
            hueVariationBark = serializedObject.FindProperty("hueVariationBark");
            color = serializedObject.FindProperty("color");
            specularColor = serializedObject.FindProperty("specularColor");

            isMeshCrossFade = serializedObject.FindProperty("isMeshCrossFade");
            isBillboardCrossFade = serializedObject.FindProperty("isBillboardCrossFade");

            isObject = serializedObject.FindProperty("isObject");

            hueVariationLeavesNew = hueVariationLeaves.colorValue;
            hueVariationBarkNew = hueVariationBark.colorValue;
            colorNew = color.colorValue;
            specularColorNew = specularColor.colorValue;

            isMeshCrossFadeNew = isMeshCrossFade.boolValue;
            isBillboardCrossFadeNew = isBillboardCrossFade.boolValue;
            drawPlaneBillboardNew = drawPlaneBillboard.boolValue;

            isObjectNew = isObject.boolValue;


            textur = new Texture2D(1, 1);
            textur.SetPixel(0, 0, Color.white);
            textur.Apply();
            textur.hideFlags = HideFlags.HideAndDontSave;
        }

        public void OnDisable()
        {
            DestroyImmediate(textur);
        }

        int popup = 0;
        int newPopup = 0;
        bool isPrefab = false;

        public override void OnInspectorGUI()
        {
            if (dataLinksCorrupted)
                return;

            if (newPopup != popup)
                popup = newPopup;

            serializedObject.Update();

            obj = (AltTree)target;

            float widthWindow = EditorGUIUtility.currentViewWidth;

            EditorGUILayout.Space();
            EditorGUILayout.Space();

        

            EditorGUILayout.LabelField("ID Tree:    <b>" + obj.id + "</b>", sty5);

            string[] popupStrs;

            if (!obj.isObject)
            {
                if (obj.drawPlaneBillboard)
                {
                    popupStrs = new string[obj.distances.Length + 3];
                    popupStrs[0] = "-";
                    for (int i = 0; i < obj.distances.Length + 1; i++)
                    {
                        popupStrs[i + 1] = "LOD " + i;
                    }
                    popupStrs[popupStrs.Length - 1] = "Billboard";
                }
                else
                {
                    popupStrs = new string[obj.distances.Length + 2];
                    popupStrs[0] = "-";
                    for (int i = 0; i < obj.distances.Length + 1; i++)
                    {
                        popupStrs[i + 1] = "LOD " + i;
                    }
                }
            }
            else
            {
                if (obj.drawPlaneBillboard)
                {
                    popupStrs = new string[obj.distances.Length + 4];
                    popupStrs[0] = "-";
                    for (int i = 0; i < obj.distances.Length + 1; i++)
                    {
                        popupStrs[i + 1] = "LOD " + i;
                    }
                    popupStrs[popupStrs.Length - 2] = "Billboard";
                    popupStrs[popupStrs.Length - 1] = "Culling";
                }
                else
                {
                    popupStrs = new string[obj.distances.Length + 3];
                    popupStrs[0] = "-";
                    for (int i = 0; i < obj.distances.Length + 1; i++)
                    {
                        popupStrs[i + 1] = "LOD " + i;
                    }
                    popupStrs[popupStrs.Length - 1] = "Culling";
                }
            }
        
            GUILayout.BeginVertical(EditorStyles.helpBox);
            {

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("LOD Distances:", sty);
                    GUILayout.Label("Selected:   ", sty2);
                    popup = Mathf.Clamp(popup, 0, popupStrs.Length - 1);
                    popup = EditorGUILayout.Popup("", popup, popupStrs, GUILayout.Width(100f));
                    if (popup != newPopup)
                        newPopup = popup;
                }
                GUILayout.EndHorizontal();

                GUILayout.Box("", GUILayout.Height(30), GUILayout.Width(widthWindow - 55));
                Rect scale = GUILayoutUtility.GetLastRect();
                float widthScale = scale.width - 3f;

                scale.height -= 1f;
                scale.yMin += 2f;
                scale.xMin += 2f;
                Color colTemp = GUI.color;
                float tempFl = 0f;

                float dists = 0f;

                for (int i = 0; i < obj.distances.Length; i++)
                {
                    dists += obj.distances[i];
                }

                if (!obj.isObject)
                {
                    if (obj.drawPlaneBillboard)
                    {
                        dists += obj.distancePlaneBillboard;
                        float widthBilb = widthScale / 5f;
                        float t = 0f;

                        if (obj.distances.Length != 0)
                        {
                            widthScale -= widthScale / 5f;

                            for (int i = 0; i < obj.distances.Length; i++)
                            {
                                scale.xMin += t;
                                scale.width = (obj.distances[i] / dists) * widthScale;
                                t = (obj.distances[i] / dists) * widthScale;
                                GUI.color = getLodColor((510f / (obj.distances.Length + 1)) * i, i+1 == popup);
                                GUI.DrawTexture(scale, textur);
                                GUI.color = Color.black;
                                GUI.Label(scale, popupStrs[i + 1], sty3);
                                GUI.Label(scale, tempFl + "", sty4);
                                tempFl = obj.distances[i];

                                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && scale.Contains(Event.current.mousePosition))
                                {
                                    newPopup = i + 1;
                                    Repaint();
                                }
                            }

                            scale.xMin += t;
                            scale.width = (obj.distancePlaneBillboard / dists) * widthScale;
                            t = (obj.distancePlaneBillboard / dists) * widthScale;

                            GUI.color = getLodColor((510f / (obj.distances.Length + 1)) * obj.distances.Length, obj.distances.Length + 1 == popup);
                            GUI.DrawTexture(scale, textur);
                            GUI.color = Color.black;
                            GUI.Label(scale, popupStrs[obj.distances.Length + 1], sty3);
                            GUI.Label(scale, tempFl + "", sty4);
                            tempFl = obj.distancePlaneBillboard;

                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && scale.Contains(Event.current.mousePosition))
                            {
                                newPopup = obj.distances.Length + 1;
                                Repaint();
                            }


                            scale.xMin += t;
                            scale.width = widthBilb;

                            GUI.color = getLodColor((510f / (obj.distances.Length + 1)) * (obj.distances.Length + 1), obj.distances.Length + 1 + 1 == popup);
                            GUI.DrawTexture(scale, textur);
                            GUI.color = Color.black;
                            GUI.Label(scale, popupStrs[popupStrs.Length - 1], sty3);
                            GUI.Label(scale, tempFl + "", sty4);

                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && scale.Contains(Event.current.mousePosition))
                            {
                                newPopup = popupStrs.Length - 1;
                                Repaint();
                            }
                        }
                        else
                        {
                            scale.width = widthBilb * 4f;
                            t = widthBilb * 4f;
                            tempFl = 0;

                            GUI.color = getLodColor((510f / (obj.distances.Length + 1)) * (obj.distances.Length), (obj.distances.Length ) + 1 == popup);
                            GUI.DrawTexture(scale, textur);
                            GUI.color = Color.black;
                            GUI.Label(scale, popupStrs[obj.distances.Length + 1], sty3);
                            GUI.Label(scale, tempFl + "", sty4);
                            tempFl = obj.distancePlaneBillboard;

                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && scale.Contains(Event.current.mousePosition))
                            {
                                newPopup = obj.distances.Length + 1;
                                Repaint();
                            }


                            scale.xMin += t;
                            scale.width = widthBilb;
                            GUI.color = getLodColor((510f / (obj.distances.Length + 1)) * (obj.distances.Length + 1), (obj.distances.Length + 1) + 1 == popup);
                            GUI.DrawTexture(scale, textur);
                            GUI.color = Color.black;
                            GUI.Label(scale, popupStrs[popupStrs.Length - 1], sty3);
                            GUI.Label(scale, tempFl + "", sty4);

                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && scale.Contains(Event.current.mousePosition))
                            {
                                newPopup = popupStrs.Length - 1;
                                Repaint();
                            }
                        }
                    }
                    else
                    {
                        float t = 0f;
                        float widthBilb = widthScale / 5f;
                        tempFl = 0;

                        if (obj.distances.Length != 0)
                        {
                            widthScale -= widthScale / 5f;
                            for (int i = 0; i < obj.distances.Length; i++)
                            {
                                scale.xMin += t;
                                scale.width = (obj.distances[i] / dists) * widthScale;
                                t = (obj.distances[i] / dists) * widthScale;

                                GUI.color = getLodColor((510f / (obj.distances.Length)) * (i), (i) + 1 == popup);
                                GUI.DrawTexture(scale, textur);
                                GUI.color = Color.black;
                                GUI.Label(scale, popupStrs[i + 1], sty3);
                                GUI.Label(scale, tempFl + "", sty4);
                                tempFl = obj.distances[i];

                                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && scale.Contains(Event.current.mousePosition))
                                {
                                    newPopup = i + 1;
                                    Repaint();
                                }
                            }

                            scale.xMin += t;
                            scale.width = widthBilb;

                            GUI.color = getLodColor((510f / (obj.distances.Length)) * (obj.distances.Length), (obj.distances.Length) + 1 == popup);
                            GUI.DrawTexture(scale, textur);
                            GUI.color = Color.black;
                            GUI.Label(scale, popupStrs[obj.distances.Length + 1], sty3);
                            GUI.Label(scale, tempFl + "", sty4);

                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && scale.Contains(Event.current.mousePosition))
                            {
                                newPopup = obj.distances.Length + 1;
                                Repaint();
                            }
                        }
                        else
                        {
                            scale.width--;

                            GUI.color = getLodColor((510f / (obj.distances.Length + 1)) * (obj.distances.Length), (obj.distances.Length) + 1 == popup);
                            GUI.DrawTexture(scale, textur);
                            GUI.color = Color.black;
                            GUI.Label(scale, popupStrs[popupStrs.Length - 1], sty3);
                            GUI.Label(scale, tempFl + "", sty4);

                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && scale.Contains(Event.current.mousePosition))
                            {
                                newPopup = popupStrs.Length - 1;
                                Repaint();
                            }
                        }
                    }
                }
                else
                {
                    if (obj.drawPlaneBillboard)
                    {
                        dists += obj.distancePlaneBillboard;
                        dists += obj.distanceCulling;
                        float widthCull = widthScale / 5f;
                        float t = 0f;
                        tempFl = 0;

                        if (obj.distances.Length != 0)
                        {
                            widthScale -= widthScale / 5f;

                            for (int i = 0; i < obj.distances.Length; i++)
                            {
                                scale.xMin += t;
                                scale.width = (obj.distances[i] / dists) * widthScale;
                                t = (obj.distances[i] / dists) * widthScale;

                                GUI.color = getLodColor((510f / (obj.distances.Length + 2)) * (i), i + 1 == popup);
                                GUI.DrawTexture(scale, textur);
                                GUI.color = Color.black;
                                GUI.Label(scale, popupStrs[i + 1], sty3);
                                GUI.Label(scale, tempFl + "", sty4);
                                tempFl = obj.distances[i];

                                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && scale.Contains(Event.current.mousePosition))
                                {
                                    newPopup = i + 1;
                                    Repaint();
                                }
                            }

                            scale.xMin += t;
                            scale.width = (obj.distancePlaneBillboard / dists) * widthScale;
                            t = (obj.distancePlaneBillboard / dists) * widthScale;

                            GUI.color = getLodColor((510f / (obj.distances.Length + 2)) * (obj.distances.Length ), (obj.distances.Length ) + 1 == popup);
                            GUI.DrawTexture(scale, textur);
                            GUI.color = Color.black;
                            GUI.Label(scale, popupStrs[obj.distances.Length + 1], sty3);
                            GUI.Label(scale, tempFl + "", sty4);
                            tempFl = obj.distancePlaneBillboard;

                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && scale.Contains(Event.current.mousePosition))
                            {
                                newPopup = obj.distances.Length + 1;
                                Repaint();
                            }


                            scale.xMin += t;
                            scale.width = (obj.distanceCulling / dists) * widthScale;
                            t = (obj.distanceCulling / dists) * widthScale;

                            GUI.color = getLodColor((510f / (obj.distances.Length + 2)) * (obj.distances.Length + 1), (obj.distances.Length + 1) + 1 == popup);
                            GUI.DrawTexture(scale, textur);
                            GUI.color = Color.black;
                            GUI.Label(scale, popupStrs[popupStrs.Length - 2], sty3);
                            GUI.Label(scale, tempFl + "", sty4);
                            tempFl = obj.distanceCulling;

                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && scale.Contains(Event.current.mousePosition))
                            {
                                newPopup = popupStrs.Length - 2;
                                Repaint();
                            }

                            scale.xMin += t;
                            scale.width = widthCull;

                            GUI.color = getLodColor((510f / (obj.distances.Length + 2)) * (obj.distances.Length + 2), (obj.distances.Length + 2) + 1 == popup);
                            GUI.DrawTexture(scale, textur);
                            GUI.color = Color.black;
                            GUI.Label(scale, popupStrs[popupStrs.Length - 1], sty3);
                            GUI.Label(scale, tempFl + "", sty4);

                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && scale.Contains(Event.current.mousePosition))
                            {
                                newPopup = popupStrs.Length - 1;
                                Repaint();
                            }
                        }
                        else
                        {
                            widthScale -= widthScale / 5f;

                            scale.xMin += t;
                            scale.width = (obj.distancePlaneBillboard / dists) * widthScale;
                            t = (obj.distancePlaneBillboard / dists) * widthScale;

                            GUI.color = getLodColor((510f / (obj.distances.Length + 2)) * (obj.distances.Length ), (obj.distances.Length) + 1 == popup);
                            GUI.DrawTexture(scale, textur);
                            GUI.color = Color.black;
                            GUI.Label(scale, popupStrs[obj.distances.Length + 1], sty3);
                            GUI.Label(scale, tempFl + "", sty4);
                            tempFl = obj.distancePlaneBillboard;

                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && scale.Contains(Event.current.mousePosition))
                            {
                                newPopup = obj.distances.Length + 1;
                                Repaint();
                            }


                            scale.xMin += t;
                            scale.width = (obj.distanceCulling / dists) * widthScale;
                            t = (obj.distanceCulling / dists) * widthScale;

                            GUI.color = getLodColor((510f / (obj.distances.Length + 2)) * (obj.distances.Length + 1), (obj.distances.Length + 1) + 1 == popup);
                            GUI.DrawTexture(scale, textur);
                            GUI.color = Color.black;
                            GUI.Label(scale, popupStrs[popupStrs.Length - 2], sty3);
                            GUI.Label(scale, tempFl + "", sty4);
                            tempFl = obj.distanceCulling;

                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && scale.Contains(Event.current.mousePosition))
                            {
                                newPopup = popupStrs.Length - 2;
                                Repaint();
                            }

                            scale.xMin += t;
                            scale.width = widthCull;

                            GUI.color = getLodColor((510f / (obj.distances.Length + 2)) * (obj.distances.Length + 2), (obj.distances.Length + 2) + 1 == popup);
                            GUI.DrawTexture(scale, textur);
                            GUI.color = Color.black;
                            GUI.Label(scale, popupStrs[popupStrs.Length - 1], sty3);
                            GUI.Label(scale, tempFl + "", sty4);

                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && scale.Contains(Event.current.mousePosition))
                            {
                                newPopup = popupStrs.Length - 1;
                                Repaint();
                            }
                        }
                    }
                    else
                    {
                        dists += obj.distanceCulling;
                        float widthCull = widthScale / 5f;
                        float t = 0f;
                        tempFl = 0;

                        if (obj.distances.Length != 0)
                        {
                            widthScale -= widthScale / 5f;
                            for (int i = 0; i < obj.distances.Length; i++)
                            {
                                scale.xMin += t;
                                scale.width = (obj.distances[i] / dists) * widthScale;
                                t = (obj.distances[i] / dists) * widthScale;

                                GUI.color = getLodColor((510f / (obj.distances.Length + 1)) * (i), i + 1 == popup);
                                GUI.DrawTexture(scale, textur);
                                GUI.color = Color.black;
                                GUI.Label(scale, popupStrs[i + 1], sty3);
                                GUI.Label(scale, tempFl + "", sty4);
                                tempFl = obj.distances[i];

                                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && scale.Contains(Event.current.mousePosition))
                                {
                                    newPopup = i + 1;
                                    Repaint();
                                }
                            }

                            scale.xMin += t;
                            scale.width = (obj.distanceCulling / dists) * widthScale;
                            t = (obj.distanceCulling / dists) * widthScale;

                            GUI.color = getLodColor((510f / (obj.distances.Length + 1)) * (obj.distances.Length), (obj.distances.Length) + 1 == popup);
                            GUI.DrawTexture(scale, textur);
                            GUI.color = Color.black;
                            GUI.Label(scale, popupStrs[obj.distances.Length + 1], sty3);
                            GUI.Label(scale, tempFl + "", sty4);
                            tempFl = obj.distanceCulling;

                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && scale.Contains(Event.current.mousePosition))
                            {
                                newPopup = obj.distances.Length + 1;
                                Repaint();
                            }


                            scale.xMin += t;
                            scale.width = widthCull;

                            GUI.color = getLodColor((510f / (obj.distances.Length + 1)) * (obj.distances.Length + 1), (obj.distances.Length + 1) + 1 == popup);
                            GUI.DrawTexture(scale, textur);
                            GUI.color = Color.black;
                            GUI.Label(scale, popupStrs[popupStrs.Length - 1], sty3);
                            GUI.Label(scale, tempFl + "", sty4);

                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && scale.Contains(Event.current.mousePosition))
                            {
                                newPopup = popupStrs.Length - 1;
                                Repaint();
                            }
                        }
                        else
                        {
                            scale.width = widthCull * 4f;
                            t = widthCull * 4f;

                            GUI.color = getLodColor((510f / (obj.distances.Length + 1)) * (obj.distances.Length), (obj.distances.Length) + 1 == popup);
                            GUI.DrawTexture(scale, textur);
                            GUI.color = Color.black;
                            GUI.Label(scale, popupStrs[obj.distances.Length + 1], sty3);
                            GUI.Label(scale, tempFl + "", sty4);
                            tempFl = obj.distanceCulling;

                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && scale.Contains(Event.current.mousePosition))
                            {
                                newPopup = obj.distances.Length + 1;
                                Repaint();
                            }


                            scale.xMin += t;
                            scale.width = widthCull;

                            GUI.color = getLodColor((510f / (obj.distances.Length + 1)) * (obj.distances.Length + 1), (obj.distances.Length + 1) + 1 == popup);
                            GUI.DrawTexture(scale, textur);
                            GUI.color = Color.black;
                            GUI.Label(scale, popupStrs[popupStrs.Length - 1], sty3);
                            GUI.Label(scale, tempFl + "", sty4);

                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && scale.Contains(Event.current.mousePosition))
                            {
                                newPopup = popupStrs.Length - 1;
                                Repaint();
                            }
                        }
                    }
                }
                GUI.color = colTemp;

                GUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    if (popup != 0)
                    {
                        GUILayout.Label(popupStrs[popup], sty);
                        float min = 0f;
                        float max = 0f;

                        if (!obj.isObject)
                        {
                            if (obj.drawPlaneBillboard)
                            {
                                if (obj.distances.Length != 0)
                                {
                                    if (popup - 1 <= obj.distances.Length)
                                    {
                                        if (popup - 1 == 0)
                                        {
                                            EditorGUILayout.FloatField("Distance Start:", 0f);
                                            EditorGUILayout.Space();
                                            obj.lods[popup - 1] = (GameObject)EditorGUILayout.ObjectField(obj.lods[popup - 1], typeof(GameObject), false);
                                        }
                                        else if (popup - 1 == 1)
                                        {
                                            min = 0.01f;
                                            if (obj.distances.Length > 1)
                                                max = obj.distances[popup - 1] - 0.01f;
                                            else
                                                max = obj.distancePlaneBillboard - 0.01f;
                                            obj.distances[popup - 2] = (float)System.Math.Round(Mathf.Clamp(EditorGUILayout.FloatField("Distance Start:", obj.distances[popup - 2]), min, max), 2);
                                            obj.distancesSquares[popup - 2] = obj.distances[popup - 2] * obj.distances[popup - 2];
                                            EditorGUILayout.Space();
                                            obj.lods[popup - 1] = (GameObject)EditorGUILayout.ObjectField(obj.lods[popup - 1], typeof(GameObject), false);
                                        }
                                        else if (popup - 1 == obj.distances.Length)
                                        {
                                            if (obj.distances.Length > 1)
                                                min = obj.distances[popup - 3] + 0.01f;
                                            else
                                                min = 0.01f;
                                            max = obj.distancePlaneBillboard - 0.01f;
                                            obj.distances[popup - 2] = (float)System.Math.Round(Mathf.Clamp(EditorGUILayout.FloatField("Distance Start:", obj.distances[popup - 2]), min, max), 2);
                                            obj.distancesSquares[popup - 2] = obj.distances[popup - 2] * obj.distances[popup - 2];
                                            EditorGUILayout.Space();
                                            obj.lods[popup - 1] = (GameObject)EditorGUILayout.ObjectField(obj.lods[popup - 1], typeof(GameObject), false);
                                        }
                                        else
                                        {
                                            min = obj.distances[popup - 3] + 0.01f;
                                            max = obj.distances[popup - 1] - 0.01f;
                                            obj.distances[popup - 2] = (float)System.Math.Round(Mathf.Clamp(EditorGUILayout.FloatField("Distance Start:", obj.distances[popup - 2]), min, max), 2);
                                            obj.distancesSquares[popup - 2] = obj.distances[popup - 2] * obj.distances[popup - 2];
                                            EditorGUILayout.Space();
                                            obj.lods[popup - 1] = (GameObject)EditorGUILayout.ObjectField(obj.lods[popup - 1], typeof(GameObject), false);
                                        }
                                    }
                                    else
                                    {
                                        min = obj.distances[obj.distances.Length - 1] + 0.01f;
                                        max = 1000f;
                                        obj.distancePlaneBillboard = (float)System.Math.Round(Mathf.Clamp(EditorGUILayout.FloatField("Distance Start:", obj.distancePlaneBillboard), min, max), 2);
                                        obj.distancePlaneBillboardSquare = obj.distancePlaneBillboard * obj.distancePlaneBillboard;
                                    }
                                }
                                else
                                {
                                    if (popup - 1 == 0)
                                    {
                                        EditorGUILayout.FloatField("Distance Start:", 0f);
                                        EditorGUILayout.Space();
                                        obj.lods[popup - 1] = (GameObject)EditorGUILayout.ObjectField(obj.lods[popup - 1], typeof(GameObject), false);
                                    }
                                    else
                                    {
                                        min = 0.01f;
                                        max = 1000f;
                                        obj.distancePlaneBillboard = (float)System.Math.Round(Mathf.Clamp(EditorGUILayout.FloatField("Distance Start:", obj.distancePlaneBillboard), min, max), 2);
                                        obj.distancePlaneBillboardSquare = obj.distancePlaneBillboard * obj.distancePlaneBillboard;
                                    }
                                }
                            }
                            else
                            {
                                if (obj.distances.Length != 0)
                                {
                                    if (popup - 1 == 0)
                                    {
                                        EditorGUILayout.FloatField("Distance Start:", 0f);
                                        EditorGUILayout.Space();
                                        obj.lods[popup - 1] = (GameObject)EditorGUILayout.ObjectField(obj.lods[popup - 1], typeof(GameObject), false);
                                    }
                                    else if (popup - 1 == 1)
                                    {
                                        min = 0.01f;
                                        if (obj.distances.Length > 1)
                                            max = obj.distances[popup - 1] - 0.01f;
                                        else
                                            max = 1000f;
                                        obj.distances[popup - 2] = (float)System.Math.Round(Mathf.Clamp(EditorGUILayout.FloatField("Distance Start:", obj.distances[popup - 2]), min, max), 2);
                                        obj.distancesSquares[popup - 2] = obj.distances[popup - 2] * obj.distances[popup - 2];
                                        EditorGUILayout.Space();
                                        obj.lods[popup - 1] = (GameObject)EditorGUILayout.ObjectField(obj.lods[popup - 1], typeof(GameObject), false);
                                    }
                                    else if (popup - 1 == obj.distances.Length)
                                    {
                                        if (obj.distances.Length > 1)
                                            min = obj.distances[popup - 3] + 0.01f;
                                        else
                                            min = 0.01f;
                                        max = 1000f;
                                        obj.distances[popup - 2] = (float)System.Math.Round(Mathf.Clamp(EditorGUILayout.FloatField("Distance Start:", obj.distances[popup - 2]), min, max), 2);
                                        obj.distancesSquares[popup - 2] = obj.distances[popup - 2] * obj.distances[popup - 2];
                                        EditorGUILayout.Space();
                                        obj.lods[popup - 1] = (GameObject)EditorGUILayout.ObjectField(obj.lods[popup - 1], typeof(GameObject), false);
                                    }
                                    else
                                    {
                                        min = obj.distances[popup - 3] + 0.01f;
                                        max = obj.distances[popup - 1] - 0.01f;
                                        obj.distances[popup - 2] = (float)System.Math.Round(Mathf.Clamp(EditorGUILayout.FloatField("Distance Start:", obj.distances[popup - 2]), min, max), 2);
                                        obj.distancesSquares[popup - 2] = obj.distances[popup - 2] * obj.distances[popup - 2];
                                        EditorGUILayout.Space();
                                        obj.lods[popup - 1] = (GameObject)EditorGUILayout.ObjectField(obj.lods[popup - 1], typeof(GameObject), false);
                                    }
                                }
                                else
                                {
                                    if (popup - 1 == 0)
                                    {
                                        EditorGUILayout.FloatField("Distance Start:", 0f);
                                        EditorGUILayout.Space();
                                        obj.lods[popup - 1] = (GameObject)EditorGUILayout.ObjectField(obj.lods[popup - 1], typeof(GameObject), false);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (obj.drawPlaneBillboard)
                            {
                                if (obj.distances.Length != 0)
                                {
                                    if (popup - 1 <= obj.distances.Length)
                                    {
                                        if (popup - 1 == 0)
                                        {
                                            EditorGUILayout.FloatField("Distance Start:", 0f);
                                            EditorGUILayout.Space();
                                            obj.lods[popup - 1] = (GameObject)EditorGUILayout.ObjectField(obj.lods[popup - 1], typeof(GameObject), false);
                                        }
                                        else if (popup - 1 == 1)
                                        {
                                            min = 0.01f;
                                            if (obj.distances.Length > 1)
                                                max = obj.distances[popup - 1] - 0.01f;
                                            else
                                                max = obj.distancePlaneBillboard - 0.01f;
                                            obj.distances[popup - 2] = (float)System.Math.Round(Mathf.Clamp(EditorGUILayout.FloatField("Distance Start:", obj.distances[popup - 2]), min, max), 2);
                                            obj.distancesSquares[popup - 2] = obj.distances[popup - 2] * obj.distances[popup - 2];
                                            EditorGUILayout.Space();
                                            obj.lods[popup - 1] = (GameObject)EditorGUILayout.ObjectField(obj.lods[popup - 1], typeof(GameObject), false);
                                        }
                                        else if (popup - 1 == obj.distances.Length)
                                        {
                                            if (obj.distances.Length > 1)
                                                min = obj.distances[popup - 3] + 0.01f;
                                            else
                                                min = 0.01f;
                                            max = obj.distancePlaneBillboard - 0.01f;
                                            obj.distances[popup - 2] = (float)System.Math.Round(Mathf.Clamp(EditorGUILayout.FloatField("Distance Start:", obj.distances[popup - 2]), min, max), 2);
                                            obj.distancesSquares[popup - 2] = obj.distances[popup - 2] * obj.distances[popup - 2];
                                            EditorGUILayout.Space();
                                            obj.lods[popup - 1] = (GameObject)EditorGUILayout.ObjectField(obj.lods[popup - 1], typeof(GameObject), false);
                                        }
                                        else
                                        {
                                            min = obj.distances[popup - 3] + 0.01f;
                                            max = obj.distances[popup - 1] - 0.01f;
                                            obj.distances[popup - 2] = (float)System.Math.Round(Mathf.Clamp(EditorGUILayout.FloatField("Distance Start:", obj.distances[popup - 2]), min, max), 2);
                                            obj.distancesSquares[popup - 2] = obj.distances[popup - 2] * obj.distances[popup - 2];
                                            EditorGUILayout.Space();
                                            obj.lods[popup - 1] = (GameObject)EditorGUILayout.ObjectField(obj.lods[popup - 1], typeof(GameObject), false);
                                        }
                                    }
                                    else if (popup - 1 == obj.distances.Length + 1)
                                    {
                                        min = obj.distances[obj.distances.Length - 1] + 0.01f;
                                        max = obj.distanceCulling - 0.01f;
                                        obj.distancePlaneBillboard = (float)System.Math.Round(Mathf.Clamp(EditorGUILayout.FloatField("Distance Start:", obj.distancePlaneBillboard), min, max), 2);
                                        obj.distancePlaneBillboardSquare = obj.distancePlaneBillboard * obj.distancePlaneBillboard;
                                    }
                                    else if (popup - 1 == obj.distances.Length + 2)
                                    {
                                        min = obj.distancePlaneBillboard + 0.01f;
                                        max = 1000f;
                                        obj.distanceCulling = (float)System.Math.Round(Mathf.Clamp(EditorGUILayout.FloatField("Distance Start:", obj.distanceCulling), min, max), 2);
                                        obj.distanceCullingSquare = obj.distanceCulling * obj.distanceCulling;
                                    }
                                }
                                else if (popup - 1 == 1)
                                {
                                    min = 0.01f;
                                    max = obj.distanceCulling - 0.01f;
                                    obj.distancePlaneBillboard = (float)System.Math.Round(Mathf.Clamp(EditorGUILayout.FloatField("Distance Start:", obj.distancePlaneBillboard), min, max), 2);
                                    obj.distancePlaneBillboardSquare = obj.distancePlaneBillboard * obj.distancePlaneBillboard;
                                }
                                else if (popup - 1 == 2)
                                {
                                    min = obj.distancePlaneBillboard + 0.01f;
                                    max = 1000f;
                                    obj.distanceCulling = (float)System.Math.Round(Mathf.Clamp(EditorGUILayout.FloatField("Distance Start:", obj.distanceCulling), min, max), 2);
                                    obj.distanceCullingSquare = obj.distanceCulling * obj.distanceCulling;
                                }
                                else if (popup - 1 == 0)
                                {
                                    EditorGUILayout.FloatField("Distance Start:", 0f);
                                    EditorGUILayout.Space();
                                    obj.lods[popup - 1] = (GameObject)EditorGUILayout.ObjectField(obj.lods[popup - 1], typeof(GameObject), false);
                                }
                            }
                            else
                            {
                                if (obj.distances.Length != 0)
                                {
                                    if (popup - 1 <= obj.distances.Length)
                                    {
                                        if (popup - 1 == 0)
                                        {
                                            EditorGUILayout.FloatField("Distance Start:", 0f);
                                            EditorGUILayout.Space();
                                            obj.lods[popup - 1] = (GameObject)EditorGUILayout.ObjectField(obj.lods[popup - 1], typeof(GameObject), false);
                                        }
                                        else if (popup - 1 == 1)
                                        {
                                            min = 0.01f;
                                            if (obj.distances.Length > 1)
                                                max = obj.distances[popup - 1] - 0.01f;
                                            else
                                                max = obj.distanceCulling - 0.01f;
                                            obj.distances[popup - 2] = (float)System.Math.Round(Mathf.Clamp(EditorGUILayout.FloatField("Distance Start:", obj.distances[popup - 2]), min, max), 2);
                                            obj.distancesSquares[popup - 2] = obj.distances[popup - 2] * obj.distances[popup - 2];
                                            EditorGUILayout.Space();
                                            obj.lods[popup - 1] = (GameObject)EditorGUILayout.ObjectField(obj.lods[popup - 1], typeof(GameObject), false);
                                        }
                                        else if (popup - 1 == obj.distances.Length)
                                        {
                                            if (obj.distances.Length > 1)
                                                min = obj.distances[popup - 3] + 0.01f;
                                            else
                                                min = 0.01f;
                                            max = obj.distanceCulling - 0.01f;
                                            obj.distances[popup - 2] = (float)System.Math.Round(Mathf.Clamp(EditorGUILayout.FloatField("Distance Start:", obj.distances[popup - 2]), min, max), 2);
                                            obj.distancesSquares[popup - 2] = obj.distances[popup - 2] * obj.distances[popup - 2];
                                            EditorGUILayout.Space();
                                            obj.lods[popup - 1] = (GameObject)EditorGUILayout.ObjectField(obj.lods[popup - 1], typeof(GameObject), false);
                                        }
                                        else
                                        {
                                            min = obj.distances[popup - 3] + 0.01f;
                                            max = obj.distances[popup - 1] - 0.01f;
                                            obj.distances[popup - 2] = (float)System.Math.Round(Mathf.Clamp(EditorGUILayout.FloatField("Distance Start:", obj.distances[popup - 2]), min, max), 2);
                                            obj.distancesSquares[popup - 2] = obj.distances[popup - 2] * obj.distances[popup - 2];
                                            EditorGUILayout.Space();
                                            obj.lods[popup - 1] = (GameObject)EditorGUILayout.ObjectField(obj.lods[popup - 1], typeof(GameObject), false);
                                        }
                                    }
                                    else if (popup - 1 == obj.distances.Length + 1)
                                    {
                                        min = obj.distances[obj.distances.Length - 1] + 0.01f;
                                        max = 1000f;
                                        obj.distanceCulling = (float)System.Math.Round(Mathf.Clamp(EditorGUILayout.FloatField("Distance Start:", obj.distanceCulling), min, max), 2);
                                        obj.distanceCullingSquare = obj.distanceCulling * obj.distanceCulling;
                                    }
                                }
                                else if (popup - 1 == 1)
                                {
                                    min = 0.01f;
                                    max = 1000f;
                                    obj.distanceCulling = (float)System.Math.Round(Mathf.Clamp(EditorGUILayout.FloatField("Distance Start:", obj.distanceCulling), min, max), 2);
                                    obj.distanceCullingSquare = obj.distanceCulling * obj.distanceCulling;
                                }
                                else if (popup - 1 == 0)
                                {
                                    EditorGUILayout.FloatField("Distance Start:", 0f);
                                    EditorGUILayout.Space();
                                    obj.lods[popup - 1] = (GameObject)EditorGUILayout.ObjectField(obj.lods[popup - 1], typeof(GameObject), false);
                                }
                            }
                        }
                    }
                }
                GUILayout.EndVertical();

            }
            GUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (!isPrefab)
                return;


            GUILayout.BeginVertical(EditorStyles.helpBox);
            {

                isObjectNew = EditorGUILayout.Toggle("Is Object", isObjectNew);

                EditorGUILayout.Space();

                drawPlaneBillboardNew = EditorGUILayout.Toggle("Draw Plane Billboard", drawPlaneBillboardNew);
            
                isMeshCrossFadeNew = EditorGUILayout.Toggle("Mesh Cross-fade", isMeshCrossFadeNew);
                isBillboardCrossFadeNew = EditorGUILayout.Toggle("Billboard Cross-fade", isBillboardCrossFadeNew);

                EditorGUILayout.Space();

                colorNew = EditorGUILayout.ColorField("Main Color:", colorNew);
                specularColorNew = EditorGUILayout.ColorField("Specular Color:", specularColorNew);
                hueVariationLeavesNew = EditorGUILayout.ColorField("Hue Leaves Random:", hueVariationLeavesNew);
                hueVariationBarkNew = EditorGUILayout.ColorField("Hue Bark Random:", hueVariationBarkNew);


                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Apply & Refresh Scene", GUILayout.Height(18)))
                    {
                        obj.setSettings(hueVariationLeavesNew, hueVariationBarkNew, colorNew, specularColorNew, isMeshCrossFadeNew, isBillboardCrossFadeNew, isObjectNew, drawPlaneBillboardNew);

                        EditorUtility.SetDirty(obj);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    if (GUILayout.Button("Revert", GUILayout.Height(18)))
                    {
                        hueVariationLeavesNew = hueVariationLeaves.colorValue;
                        hueVariationBarkNew = hueVariationBark.colorValue;
                        colorNew = color.colorValue;
                        specularColorNew = specularColor.colorValue;
                        isMeshCrossFadeNew = isMeshCrossFade.boolValue;
                        isBillboardCrossFadeNew = isBillboardCrossFade.boolValue;
                        drawPlaneBillboardNew = drawPlaneBillboard.boolValue;
                        isObjectNew = isObject.boolValue;
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }
            GUILayout.EndVertical();

            EditorGUILayout.Space();


            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                obj.windMode = EditorGUILayout.Popup("Wind Mode:", obj.windMode, new string[] {"None", "SpeedTree", "TreeCreator" });

                if(obj.windMode == 1)
                {
                    obj.windIntensity_ST = EditorGUILayout.Slider("Intensity: ", obj.windIntensity_ST, 0f, 5f);

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    if (obj.loadedConfig)
                        EditorGUILayout.LabelField("Configuration: ", "loaded", styGreen);
                    else
                        EditorGUILayout.LabelField("Configuration: ", "not loaded", styRed);

                    EditorGUILayout.BeginHorizontal();
                    {
                        string[] _popupStrs;
                        if (Directory.Exists("AltSystems/AltTrees/SpeedTreeWindParameters"))
                        {
                            string[] filesTemp = Directory.GetFiles("AltSystems/AltTrees/SpeedTreeWindParameters", "*.altWSTParams", SearchOption.TopDirectoryOnly);
                            _popupStrs = new string[filesTemp.Length + 1];
                            _popupStrs[0] = "-";
                            for(int i = 0; i < filesTemp.Length; i++)
                            {
                                string[] strs = filesTemp[i].Replace(".altWSTParams", "").Split('\\');

                                _popupStrs[i + 1] = strs[strs.Length - 1];
                            }
                        }
                        else
                            _popupStrs = new string[] { "-" };

                        if (popupWindParams >= _popupStrs.Length)
                            popupWindParams = 0;

                        popupWindParams = EditorGUILayout.Popup("Load File Config:", popupWindParams, _popupStrs);
                        if (popupWindParams == 0)
                            GUI.enabled = false;
                        if (GUILayout.Button("Load"))
                        {
                            obj.windParams_ST = new float[704];
                            obj.windParamsUp_ST = new bool[704];

                            using (BinaryReader reader = new BinaryReader(File.Open("AltSystems/AltTrees/SpeedTreeWindParameters/" + _popupStrs[popupWindParams] + ".altWSTParams", FileMode.Open)))
                            {
                                for (int i = 0; i < 704; i++)
                                {
                                    obj.windParamsUp_ST[i] = reader.ReadBoolean();
                                    obj.windParams_ST[i] = reader.ReadSingle();
                                }
                            }
                            obj.loadedConfig = true;

                            EditorUtility.SetDirty(obj);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                        GUI.enabled = true;
                    }
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Label("<color=#0000ffff>[How create WindFileConfig for SpeedTrees]</color>", sty6);
                    Rect scaleRect = GUILayoutUtility.GetLastRect();

                    if (GUI.Button(scaleRect, "", sty))
                        Application.OpenURL("http://altsystems-unity.net/AltTrees/Documentation/windParamsSpeedTreePage.php");
                    EditorGUIUtility.AddCursorRect(scaleRect, MouseCursor.Link);
                }
                else if (obj.windMode == 2)
                {
                    obj.windIntensity_TC = EditorGUILayout.Slider("Intensity: ", obj.windIntensity_TC, 0f, 5f);
                    obj.windBendCoefficient_TC = EditorGUILayout.Slider("Bending Coefficient: ", obj.windBendCoefficient_TC, 0f, 10f);
                    obj.windTurbulenceCoefficient_TC = EditorGUILayout.Slider("Turbulence Coefficient: ", obj.windTurbulenceCoefficient_TC, 0f, 10f);
                    

                    //bending coefficient
                }
            }
            GUILayout.EndVertical();

            EditorGUILayout.Space();

            obj.colliders = (GameObject)EditorGUILayout.ObjectField("Colliders:", obj.colliders, typeof(GameObject), false);
            obj.billboardColliders = (GameObject)EditorGUILayout.ObjectField("Billboard Colliders:", obj.billboardColliders, typeof(GameObject), false);

            EditorGUILayout.Space();
            
            if (GUILayout.Button("Tree Resources Folder", GUILayout.Width(200)))
            {
                Selection.activeObject = AssetDatabase.LoadAssetAtPath("Assets/Plugins/AltSystems/AltTrees/DataBase/Trees/TreeResources/" + obj.folderResources, typeof(Object));
            }

            EditorGUILayout.Space();

            showMaterials = EditorGUILayout.Foldout(showMaterials, "Materials");
            if (showMaterials)
            {
                matsBark.Clear();
                matsLeaves.Clear();

                for (int i = 0; i < obj.barkMaterials.Length; i++)
                {
                    matsBark.Add(obj.barkMaterials[i]);
                }
                for (int i = 0; i < obj.leavesMaterials.Length; i++)
                {
                    matsLeaves.Add(obj.leavesMaterials[i]);
                }

                GUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    sty.fontSize = 13;
                    GUILayout.Label("Leaves: ", sty);
                    for (int i = 0; i < matsLeaves.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.ObjectField(matsLeaves[i], typeof(Material), false);
                            if (GUILayout.Button("to bark"))
                            {
                                matsBark.Add(matsLeaves[i]);
                                matsLeaves.RemoveAt(i);
                                i--;
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                
                    sty.fontSize = 13;
                    GUILayout.Label("Bark: ", sty);
                    for (int i = 0; i < matsBark.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.ObjectField(matsBark[i], typeof(Material), false);
                            if (GUILayout.Button("to leaves"))
                            {
                                matsLeaves.Add(matsBark[i]);
                                matsBark.RemoveAt(i);
                                i--;
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndVertical();

                if(matsBark.Count != obj.barkMaterials.Length || matsLeaves.Count != obj.leavesMaterials.Length)
                {
                    obj.barkMaterials = new Material[matsBark.Count];
                    obj.leavesMaterials = new Material[matsLeaves.Count];

                    for (int i = 0; i < matsBark.Count; i++)
                    {
                        obj.barkMaterials[i] = matsBark[i];
                    }
                    for (int i = 0; i < matsLeaves.Count; i++)
                    {
                        obj.leavesMaterials[i] = matsLeaves[i];
                    }
                    EditorUtility.SetDirty(obj);
                }
            }


            showShaders = EditorGUILayout.Foldout(showShaders, "Shaders");
            if (showShaders)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    obj.shaderBillboard = (Shader)EditorGUILayout.ObjectField("Shader Billboard:", obj.shaderBillboard, typeof(Shader), false);
                    obj.shaderBillboardGroup = (Shader)EditorGUILayout.ObjectField("Shader Billboards Group:", obj.shaderBillboardGroup, typeof(Shader), false);
                    obj.shaderAntialiasing = (Shader)EditorGUILayout.ObjectField("Shader Billboard Antialiasing:", obj.shaderAntialiasing, typeof(Shader), false);
                    obj.shaderNormalsToScreen = (Shader)EditorGUILayout.ObjectField("Shader Billboard NormalsToScreen:", obj.shaderNormalsToScreen, typeof(Shader), false);
                }
                GUILayout.EndVertical();
            }

            showBillboardSettings = EditorGUILayout.Foldout(showBillboardSettings, "Billboard Generation Settings");
            if (showBillboardSettings)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    obj.isAntialiasing = EditorGUILayout.Toggle("Antialiasing:", obj.isAntialiasing);
                    obj.isNormalmapBillboard = EditorGUILayout.Toggle("Normalmap:", obj.isNormalmapBillboard);

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        EditorGUILayout.LabelField("Texture:", sty);
                        obj.sizeTextureBillboard = EditorGUILayout.IntField("Size Texture:", obj.sizeTextureBillboard);
                        obj.ambientMode = (UnityEngine.Rendering.AmbientMode)EditorGUILayout.EnumPopup("Ambient Mode:", obj.ambientMode);
                        obj.ambientLight = EditorGUILayout.ColorField("Ambient Light:", obj.ambientLight);
                        obj.ambientIntensity = EditorGUILayout.FloatField("Ambient Intensity:", obj.ambientIntensity);

                        obj.textureImporterType = (TextureImporterType)EditorGUILayout.EnumPopup("Texture ImporterType:", obj.textureImporterType);
                        obj.filterMode = (FilterMode)EditorGUILayout.EnumPopup("FilterMode:", obj.filterMode);
                        obj.mipmapEnabled = EditorGUILayout.Toggle("MipMap:", obj.mipmapEnabled);
                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        EditorGUILayout.LabelField("Normalmap:", sty);
                        if (!obj.isNormalmapBillboard)
                            GUI.enabled = false;
                        obj.sizeNormalsBillboard = EditorGUILayout.IntField("Size Texture:", obj.sizeNormalsBillboard);
                        obj.textureImporterType_Normals = (TextureImporterType)EditorGUILayout.EnumPopup("Texture ImporterType:", obj.textureImporterType_Normals);
                        obj.filterMode_Normals = (FilterMode)EditorGUILayout.EnumPopup("FilterMode:", obj.filterMode_Normals);
                        obj.mipmapEnabled_Normals = EditorGUILayout.Toggle("MipMap:", obj.mipmapEnabled_Normals);

                        obj.anisoLevel_Normals = EditorGUILayout.IntField("AnisoLevel:", obj.anisoLevel_Normals);
                        obj.normalsScale_Normals = EditorGUILayout.Slider("Normal Scale:", obj.normalsScale_Normals, 0.01f, 1f);
                        GUI.enabled = true;
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
            }

            if (GUI.changed)
                EditorUtility.SetDirty(obj);


            EditorGUILayout.Space();

            if (GUILayout.Button("Refresh Scene", GUILayout.Height(18)))
            {
                EditorUtility.SetDirty(obj);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                AltTrees[] ats = FindObjectsOfType(typeof(AltTrees)) as AltTrees[];
                foreach (AltTrees at in ats)
                {
                    at.reInitTimer = 10;
                }
            }

            if (GUILayout.Button("Update Billboard Texture & Refresh Scene", GUILayout.Height(18)))
            {
                obj.setSettings(hueVariationLeavesNew, hueVariationBarkNew, colorNew, specularColorNew, isMeshCrossFadeNew, isBillboardCrossFadeNew, isObjectNew, drawPlaneBillboardNew, true);

                EditorUtility.SetDirty(obj);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        Color getLodColor(float colorZnach, bool activ)
        {
            if (colorZnach <= 255)
                return new Color32(255, (byte)colorZnach, 0, (byte)((activ) ? 255 : 70));
            else
                return new Color32((byte)(510 - colorZnach), 255, 0, (byte)((activ) ? 255 : 70));
        }

        public void OnSceneGUI()
        {
            if (dataLinksCorrupted)
                return;

            float dist = AltUtilities.fastDistanceSqrt(SceneView.lastActiveSceneView.camera.transform.position, obj.gameObject.transform.position);

            int lod = 0;
            float colorZnach;
            if (obj.distancesSquares.Length > 0)
            {
                if (dist >= obj.distancesSquares[obj.distancesSquares.Length - 1])
                {
                    if (obj.drawPlaneBillboard)
                    {
                        if (dist < obj.distancePlaneBillboardSquare)
                        {
                            lod = obj.lods.Length - 1;

                            colorZnach = (510 / obj.distancesSquares.Length) * (obj.distancesSquares.Length - 1);

                            if (colorZnach <= 255)
                                style.normal.textColor = new Color32(255, (byte)colorZnach, 0, 255);
                            else
                                style.normal.textColor = new Color32((byte)(510 - colorZnach), 255, 0, 255);
                        }
                        else
                        {
                            lod = -1;

                            style.normal.textColor = new Color32(0, 255, 0, 255);
                        }
                    }
                    else
                    {
                        style.normal.textColor = new Color32(0, 255, 0, 255);

                        lod = obj.lods.Length - 1;
                    }

                }
                else
                {
                    for (int i = 0; i < obj.distancesSquares.Length; i++)
                    {
                        if (dist < obj.distancesSquares[i])
                        {
                            lod = i;
                            if (obj.distancesSquares.Length > 1 || obj.drawPlaneBillboard)
                                colorZnach = (510 / (obj.drawPlaneBillboard ? obj.distancesSquares.Length + 1 : obj.distancesSquares.Length)) * i;
                            else
                                colorZnach = 510;

                            if (colorZnach <= 255)
                                style.normal.textColor = new Color32(255, (byte)colorZnach, 0, 255);
                            else
                                style.normal.textColor = new Color32((byte)(510 - colorZnach), 255, 0, 255);

                            break;
                        }

                    }
                }
            }
            else
            {
                if (obj.drawPlaneBillboard)
                {
                    if (dist < obj.distancePlaneBillboardSquare)
                    {
                        lod = obj.lods.Length - 1;

                        colorZnach = 0;

                        if (colorZnach <= 255)
                            style.normal.textColor = new Color32(255, (byte)colorZnach, 0, 255);
                        else
                            style.normal.textColor = new Color32((byte)(510 - colorZnach), 255, 0, 255);
                    }
                    else
                    {
                        lod = -1;

                        style.normal.textColor = new Color32(0, 255, 0, 255);
                    }
                }
                else
                {
                    style.normal.textColor = new Color32(255, 0, 0, 255);

                    lod = obj.lods.Length - 1;
                }
            }

            Vector3 oncam = Camera.current.WorldToScreenPoint(obj.gameObject.transform.position);

            if ((oncam.x >= -100) && (oncam.x <= Camera.current.pixelWidth) && (oncam.y >= 0) && (oncam.y <= Camera.current.pixelHeight) && (oncam.z > 0))
            {
                Handles.BeginGUI();

                Handles.Label(obj.gameObject.transform.position, "LOD: " + (lod != -1 ? lod.ToString() : "PlaneBillboard"), style);

                Handles.EndGUI();
            }
        }

        [System.NonSerialized]
        public bool dataLinksCorrupted = false;
        [System.NonSerialized]
        public bool dataLinksCorruptedLogged = false;
        
        public void dataLinksIsCorrupted()
        {
            dataLinksCorrupted = true;

            if (!dataLinksCorruptedLogged)
            {
                dataLinksCorruptedLogged = true;
                Debug.LogError("The AltTreesDataLinks file is corrupted. Select the file \"Assets/Plugins/AltSystems/AltTrees/DataBase/AltTreesDataLinks.asset\", and assign a script \"AltTreesDataLinks\" on the missing script.");
            }
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


                if (dataLinks == null)
                {
                    dataLinksIsCorrupted();
                }
            }
        }
    }
}