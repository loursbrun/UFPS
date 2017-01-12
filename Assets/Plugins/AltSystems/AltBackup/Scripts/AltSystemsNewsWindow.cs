#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;
using AltSystems.AltBackup;

namespace AltSystems
{
    public class AltSystemsNewsWindow : EditorWindow
    {
        static AltSystemsNewsWindow window;

        static public void Init(int _idLast)
        {
            window = (AltSystemsNewsWindow)EditorWindow.GetWindow(typeof(AltSystemsNewsWindow), true, "AltSystems News:");
            window.minSize = new Vector2(600, 500);
            window.maxSize = new Vector2(600, 500);
            CenterOnMainEditorWindow_NoEditor.CenterOnMainWin(window);
            window.wantsMouseMove = true;

            isLoading = false;
            isLoadingStart = false;
            news = "";
            idLast = _idLast;

            if (!System.IO.Directory.Exists("AltSystems"))
                System.IO.Directory.CreateDirectory("AltSystems");

            if (System.IO.File.Exists("AltSystems/news.cfg"))
            {
                BinaryReader fl = null;
                try
                {
                    string str = "";
                    fl = new BinaryReader(File.Open("AltSystems/news.cfg", FileMode.Open));
                    str = fl.ReadString();

                    string[] strs = str.Split(';');
                    int k;

                    if (int.TryParse(strs[2], out k))
                    {
                        checkNews = (k == 1);
                        checkNewsStar = checkNews;
                    }
                }
                catch (IOException exc)
                {
                    Debug.LogError(exc.Message);
                }
                finally
                {
                    if (fl != null)
                        fl.Close();
                }
            }
        }

        static bool isLoading = false;
        static bool isLoadingStart = false;
        static string news = "";
        Vector2 scroll = new Vector2();
        Texture2D textur = null;
        static bool checkNews = true;
        static bool checkNewsStar = true;
        static int idLast;


        void OnGUI()
        {
            if (AltSystemsNewsCheck.newsCheckStatic == null)
            {
                GameObject goTemp = new GameObject("newsCheckStatic");
                goTemp.hideFlags = HideFlags.HideInHierarchy;
                AltSystemsNewsCheck.newsCheckStatic = goTemp.AddComponent<AltSystemsNewsCheck>();
            }


            GUIStyle sty = new GUIStyle();

            if (textur == null)
            {
                textur = new Texture2D(570, 1);
                for (int i = 0; i < 570; i++)
                {
                    textur.SetPixel(i, 0, Color.gray);
                }
                textur.wrapMode = TextureWrapMode.Clamp;
                textur.Apply();
            }

            if (isLoading)
            {
                sty.fontSize = 12;
                sty.wordWrap = true;
                sty.richText = true;


                string[] newsTemp = news.Split(']');

                scroll = GUILayout.BeginScrollView(scroll, GUILayout.MaxHeight(480));
                {
                    GUILayout.Space(30);

                    for (int i = 0; i < newsTemp.Length; i++)
                    {
                        string[] newsTemp2 = newsTemp[i].Split('[');

                        sty.fontStyle = FontStyle.Bold;
                        sty.fontSize = 15;
                        sty.padding.left = 40;

                        if (idLast == 0 || idLast <= i)
                            GUILayout.Label(newsTemp2[0], sty);
                        else
                            GUILayout.Label(newsTemp2[0] + "   <color=red>new!</color>", sty);

                        sty.fontSize = 11;
                        sty.fontStyle = FontStyle.Normal;
                        sty.normal.textColor = Color.gray;
                        sty.padding.left = 50;

                        GUILayout.Space(2);

                        GUILayout.Label(newsTemp2[1], sty);

                        GUILayout.Space(3);

                        sty.normal.textColor = Color.black;
                        sty.fontSize = 12;
                        sty.padding.left = 40;
                        GUILayout.Label(newsTemp2[2], sty);

                        GUILayout.Space(10);

                        if (i != newsTemp.Length - 1)
                        {
                            GUILayout.Label(textur);
                            GUILayout.Space(15);
                        }

                    }
                }
                GUILayout.EndScrollView();
            }
            else
            {
                sty.fontSize = 18;

                GUI.Label(new Rect(250, 30, 100, 30), "Loading ...", sty);

                if (!isLoadingStart)
                {
                    EditorCoroutines.Start(testRoutine());

                    isLoadingStart = true;
                }
            }

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(500);
                checkNews = GUILayout.Toggle(checkNews, "Check News");
            }
            GUILayout.EndHorizontal();


            if (checkNewsStar != checkNews)
            {
                if (!System.IO.Directory.Exists("AltSystems"))
                    System.IO.Directory.CreateDirectory("AltSystems");

                if (System.IO.File.Exists("AltSystems/news.cfg"))
                {
                    BinaryReader fl = null;
                    try
                    {
                        string str = "";
                        fl = new BinaryReader(File.Open("AltSystems/news.cfg", FileMode.Open));
                        str = fl.ReadString();

                        string[] strs = str.Split(';');

                        if (fl != null)
                            fl.Close();

                        BinaryWriter fl2 = null;
                        try
                        {
                            string str2 = strs[0] + ";" + strs[1] + ";" + ((checkNews) ? "1" : "0");
                            fl2 = new BinaryWriter(File.Open("AltSystems/news.cfg", FileMode.Truncate));
                            fl2.Write(str2);
                        }
                        catch (IOException exc)
                        {
                            Debug.LogError(exc.Message);
                        }
                        finally
                        {
                            if (fl2 != null)
                                fl2.Close();
                        }
                    }
                    catch (IOException exc)
                    {
                        Debug.LogError(exc.Message);
                    }
                    finally
                    {
                        if (fl != null)
                            fl.Close();
                    }
                }
                checkNewsStar = checkNews;
            }
        }



        IEnumerator testRoutine()
        {
            WWW www = new WWW("http://altsystems-unity.net/newsUnity.php?unity=" + Application.unityVersion + "&asset=AltBackup&ver=" + AltSystemsNewsCheck.version);

            double time = EditorApplication.timeSinceStartup;

            while (!www.isDone && string.IsNullOrEmpty(www.error))
            {
                if (EditorApplication.timeSinceStartup - time >= 10)
                    yield break;

                yield return www;
            };

            if (www.isDone)
            {
                if (string.IsNullOrEmpty(www.error))
                {
                    Repaint();

                    news = www.text.Replace("<br>", Environment.NewLine);
                    isLoading = true;

                    Repaint();
                }
                else
                {
                    Debug.LogError(www.error);
                }
            }
            else
                Debug.LogError("!www.isDone");
        }
    }
}
#endif