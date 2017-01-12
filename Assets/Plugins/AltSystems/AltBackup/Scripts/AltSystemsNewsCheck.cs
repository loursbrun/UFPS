#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using AltSystems.AltBackup;

namespace AltSystems
{
    [ExecuteInEditMode]
    public class AltSystemsNewsCheck : MonoBehaviour
    {
        public static AltSystemsNewsCheck newsCheckStatic;
        public static string version = "0.9.6.1";

        double time = 10000000;

        void Start()
        {
            if (newsCheckStatic == null)
                newsCheckStatic = this;
            else
            {
                if (!newsCheckStatic.Equals(this))
                {
                    DestroyImmediate(this);
                }
            }

            check();
        }

        void Update()
        {
            if (newsCheckStatic == null)
                newsCheckStatic = this;
            else
            {
                if (!newsCheckStatic.Equals(this))
                {
                    DestroyImmediate(this);
                }
            }

            check();
        }

        void check()
        {
            if (EditorApplication.timeSinceStartup - time <= 0 || EditorApplication.timeSinceStartup - time > 600)
            {
                time = EditorApplication.timeSinceStartup;


                bool checkNews = false;

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
                            checkNews = (k == 1) ? true : false;
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
                else
                    checkNews = true;

                if (checkNews && !Application.isPlaying)
                    EditorCoroutines.Start(getLatestNews());
            }
        }

        IEnumerator getLatestNews()
        {
            WWW www = new WWW("http://altsystems-unity.net/getLatestNewsNumber.php?unity=" + Application.unityVersion + "&asset=AltTrees&ver=" + version);

            double time = EditorApplication.timeSinceStartup;

            while (!www.isDone && string.IsNullOrEmpty(www.error))
            {
                if (EditorApplication.timeSinceStartup - time >= 5)
                    yield break;

                yield return www;
            };

            if (www.isDone)
            {
                if (string.IsNullOrEmpty(www.error))
                {
                    int j;
                    int k;
                    if (int.TryParse(www.text, out j))
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

                                if (int.TryParse(strs[0], out k))
                                {
                                    if (k < j)
                                    {
                                        if (fl != null)
                                            fl.Close();

                                        BinaryWriter fl2 = null;
                                        try
                                        {
                                            string str2 = j + ";" + strs[1] + ";" + strs[2];
                                            fl2 = new BinaryWriter(File.Open("AltSystems/news.cfg", FileMode.Truncate));
                                            fl2.Write(str2);
                                        }
                                        catch (IOException exc)
                                        {
                                            Debug.LogError(exc.Message);
                                            yield break;
                                        }
                                        finally
                                        {
                                            if (fl2 != null)
                                                fl2.Close();
                                        }

                                        AltSystemsNewsWindow.Init(j - k);
                                    }
                                }
                            }
                            catch (IOException exc)
                            {
                                Debug.LogError(exc.Message);
                                yield break;
                            }
                            finally
                            {
                                if (fl != null)
                                    fl.Close();
                            }
                        }
                        else
                        {
                            BinaryWriter fl = null;
                            try
                            {
                                string str = j + ";1;1";
                                fl = new BinaryWriter(File.Open("AltSystems/news.cfg", FileMode.Create));
                                fl.Write(str);
                            }
                            catch (IOException exc)
                            {
                                Debug.LogError(exc.Message);
                                yield break;
                            }
                            finally
                            {
                                if (fl != null)
                                    fl.Close();
                            }
                        }
                    }
                }
                /*else
                {
                    Debug.LogError(www.error);
                }*/
            }
            /*else
                Debug.LogError("!www.isDone");*/
        }
    }
}
#endif