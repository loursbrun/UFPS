using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using System;
using System.IO;
using System.Linq;

namespace AltSystems.AltTrees.Editor
{
    public class InstallerAltTrees : EditorWindow
    {
        static string version = "0.9.6.1";
        static int altTreesVersionUnity = 540;

        static int currentUnityVer = 0;

        static string currentVersion = "-";
        static string versionServer = "";

        static InstallerAltTrees window;
        static bool isCheckUpdates = false;
        static bool isCheckStart = false;
        static bool isCheckUpdatesOk = false;
        static bool isInstall = false;
        static public bool isInstallOk = false;
        static public bool isQuestion = false;
        int errorId = 0;
        string dat = "";

        string[] listScripts;
        string[] listEditorScripts;

        static string starVer = "";

        [MenuItem("Window/AltSystems/AltTrees/Install or Update AltTrees")]
        static void OpenInstaller()
        {
            Type types = System.Type.GetType("AltSystems.AltTrees.Editor.AltTrees_Editor");
            if (types != null)
            {
                FieldInfo myPropInfo = types.GetField("version", BindingFlags.Static | BindingFlags.Public);

                if (myPropInfo != null)
                    starVer = (string)myPropInfo.GetValue(null);
            }


            int.TryParse((Application.unityVersion.Substring(0, 5)).Replace(".", ""), out currentUnityVer);

            window = (InstallerAltTrees)EditorWindow.GetWindow(typeof(InstallerAltTrees), true, "Install or Update AltTrees");
            window.minSize = new Vector2(300, 250);
            window.maxSize = new Vector2(300, 250);
            CenterOnMainEditorWindowInstaller.CenterOnMainWinInstaller(window);
            window.wantsMouseMove = true;

            isCheckStart = false;
            isCheckUpdates = false;
            isCheckUpdatesOk = false;
            isInstall = false;
            isInstallOk = false;
            isQuestion = false;
        }

        static public void sendQuestion()
        {
            OpenInstaller();
            isQuestion = true;
        }


        void OnGUI()
        {
            GUIStyle sty = new GUIStyle();
            sty.richText = true;

            GUIStyle sty2 = new GUIStyle();
            sty2.richText = true;
            sty2.normal.textColor = Color.red;

            if (!isCheckStart)
            {
                EditorCoroutinesInstaller.Start(checkUpdates());
                isCheckStart = true;
            }

            if (isQuestion)
            {
                sty.fontSize = 15;
                GUI.Label(new Rect(80, 70, 300, 30), "<b>Update AltTrees?</b>", sty);

                sty.fontSize = 11;
                GUI.Label(new Rect(5, 215, 300, 30), "Current version: <b>" + currentVersion + "</b>", sty);
                GUI.Label(new Rect(5, 230, 300, 30), "New version:      <b>" + version + "</b>", sty);

                sty.fontSize = 15;

                if (GUI.Button(new Rect(35, 150, 110, 30), "Update"))
                {
                    isQuestion = false;
                }
                if (GUI.Button(new Rect(155, 150, 110, 30), "Remove Installer"))
                {
                    System.IO.File.Delete("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/InstallerAltTrees.cs");
                    if (System.IO.File.Exists("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/InstallerAltTrees.cs" + ".meta"))
                        System.IO.File.Delete("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/InstallerAltTrees.cs" + ".meta");
                    System.IO.Directory.Delete("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees", true);
                    if (System.IO.File.Exists("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees" + ".meta"))
                        System.IO.File.Delete("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees" + ".meta");

                    AssetDatabase.Refresh();

                    this.Close();
                }

                return;
            }

            if (isCheckUpdates && isCheckUpdatesOk)
            {
                if (!isInstall)
                {
                    if (altTreesVersionUnity == 520 && currentUnityVer >= 540)
                    {
                        GUIStyle sty6;
                        sty6 = new GUIStyle();
                        sty6.fontSize = 10;
                        sty6.fontStyle = FontStyle.Bold;
                        sty6.normal.textColor = Color.red;
                        sty6.wordWrap = true;

                        GUI.Label(new Rect(5, 10, 300, 30), "Attention! You use the AltTrees for Unity 5.2.0+ Please, update AltTrees for Unity 5.4.0+", sty6);
                    }

                    if (starVer == "0.9.6")
                    {

                        sty.fontSize = 15;
                        sty2.fontSize = 15;
                        GUI.Label(new Rect(20, 50, 300, 30), "This is a beta version! Use of this", sty);
                        GUI.Label(new Rect(30, 70, 300, 30), "asset may crash your project.", sty);
                        GUI.Label(new Rect(5, 90, 300, 30), "<b>Before installation, make a backup!</b>", sty);

                        GUI.Label(new Rect(20, 110, 300, 30), "<b>Attention! Before installation,</b>", sty2);
                        GUI.Label(new Rect(10, 126, 300, 30), "<b>read the instructions for updating!</b>", sty2);


                        sty.fontSize = 11;
                        GUI.Label(new Rect(5, 215, 300, 30), "Current version: <b>" + currentVersion + "</b>", sty);
                        GUI.Label(new Rect(5, 230, 300, 30), "New version:      <b>" + version + "</b>", sty);

                        sty.fontSize = 15;

                        if (GUI.Button(new Rect(30, 150, 240, 30), "Read the instructions for updating"))
                        {
                            Application.OpenURL("https://forum.unity3d.com/threads/alttrees-system-wip-assetstore.431811/page-3#post-2900914");
                        }

                        if (GUI.Button(new Rect(60, 195, 180, 20), "Start Install/Update AltTrees"))
                        {
                            bool stop = false;
                            if (!System.IO.Directory.Exists("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees"))
                            {
                                Debug.LogError("Directory missing: Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees");
                                stop = true;
                            }
                            /*if (!System.IO.Directory.Exists("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/Files"))
                            {
                                Debug.LogError("Directory missing: Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/Files");
                                stop = true;
                            }
                            if (!System.IO.Directory.Exists("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/Files/Scripts"))
                            {
                                Debug.LogError("Directory missing: Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/Files/Scripts");
                                stop = true;
                            }
                            if (!System.IO.Directory.Exists("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/Files/Editor"))
                            {
                                Debug.LogError("Directory missing: Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/Files/Editor");
                                stop = true;
                            }*/

                            if (!stop)
                                EditorApplication.delayCall += install;
                        }
                    }
                    else
                    {
                        sty.fontSize = 15;
                        GUI.Label(new Rect(20, 50, 300, 30), "This is a beta version! Use of this", sty);
                        GUI.Label(new Rect(30, 70, 300, 30), "asset may crash your project.", sty);
                        GUI.Label(new Rect(5, 110, 300, 30), "<b>Before installation, make a backup!</b>", sty);

                        sty.fontSize = 11;
                        GUI.Label(new Rect(5, 215, 300, 30), "Current version: <b>" + currentVersion + "</b>", sty);
                        GUI.Label(new Rect(5, 230, 300, 30), "New version:      <b>" + version + "</b>", sty);

                        sty.fontSize = 15;

                        if (GUI.Button(new Rect(50, 150, 200, 30), "Start Install/Update AltTrees"))
                        {
                            bool stop = false;
                            if (!System.IO.Directory.Exists("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees"))
                            {
                                Debug.LogError("Directory missing: Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees");
                                stop = true;
                            }
                            /*if (!System.IO.Directory.Exists("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/Files"))
                            {
                                Debug.LogError("Directory missing: Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/Files");
                                stop = true;
                            }
                            if (!System.IO.Directory.Exists("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/Files/Scripts"))
                            {
                                Debug.LogError("Directory missing: Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/Files/Scripts");
                                stop = true;
                            }
                            if (!System.IO.Directory.Exists("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/Files/Editor"))
                            {
                                Debug.LogError("Directory missing: Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/Files/Editor");
                                stop = true;
                            }*/

                            if (!stop)
                                EditorApplication.delayCall += install;
                        }
                    }
                }
                else
                    if (isInstallOk)
                {
                    if (window == null)
                        window = this;

                    window.minSize = new Vector2(350, 250);
                    window.maxSize = new Vector2(350, 250);
                    CenterOnMainEditorWindowInstaller.CenterOnMainWinInstaller(window);


                    sty.fontSize = 15;
                    sty.fontStyle = FontStyle.Bold;
                    GUI.Label(new Rect(138, 10, 300, 30), "Excellent!", sty);
                    GUI.Label(new Rect(85, 30, 300, 30), "Installation successful!", sty);


                    sty.fontSize = 12;
                    sty.fontStyle = FontStyle.Normal;
                    if (System.IO.File.Exists("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/cfg.txt"))
                    {
                        GUI.Label(new Rect(55, 80, 300, 30), "Older scripts moved to the folder", sty);
                        GUI.Label(new Rect(2, 95, 300, 30), "\"project folder/AltSystems/AltTrees/OldScriptsTemp\"", sty);
                    }

                    GUI.Label(new Rect(15, 150, 300, 30), "Get started here: <b>Window/AltSystems/AltTrees</b>", sty);

                    GUI.Label(new Rect(50, 200, 300, 30), "<b>Thank you for choosing AltSystems! ;)</b>", sty);


                    if (GUI.Button(new Rect(100, 220, 150, 18), "Exit"))
                    {
                        exit();
                    }
                }
                else
                {
                    if (errorId == 0)
                    {
                        sty.fontSize = 14;
                        GUI.Label(new Rect(60, 30, 300, 30), "Installing ... Please wait ...", sty);
                    }
                    else
                    {
                        if (errorId == 1)
                        {
                            sty.fontSize = 16;
                            sty.fontStyle = FontStyle.Bold;
                            GUI.Label(new Rect(125, 30, 300, 30), "Error!", sty);

                            sty.fontSize = 12;
                            sty.fontStyle = FontStyle.Normal;
                            GUI.Label(new Rect(10, 80, 300, 30), "In the project is already installed <b>AltTerrain!</b>", sty);
                            sty.fontSize = 13;
                            GUI.Label(new Rect(65, 100, 300, 30), "AltTerrain already includes", sty);
                            GUI.Label(new Rect(80, 120, 300, 30), "a modified AltTrees.", sty);
                        }
                        /*else if (errorId == 2)
                        {
                            sty.fontSize = 16;
                            sty.fontStyle = FontStyle.Bold;
                            GUI.Label(new Rect(125, 30, 300, 30), "Error!", sty);

                            sty.fontSize = 12;
                            sty.fontStyle = FontStyle.Normal;
                            GUI.Label(new Rect(15, 80, 300, 30), "In the project is already installed <b>AltTrees!</b>", sty);
                            sty.fontSize = 13;
                            GUI.Label(new Rect(70, 100, 300, 30), "AltTrees already includes", sty);
                            GUI.Label(new Rect(80, 120, 300, 30), "a modified AltBackup.", sty);
                        }*/

                        if (GUI.Button(new Rect(75, 180, 150, 18), "Exit"))
                        {
                            this.Close();
                        }
                        if (GUI.Button(new Rect(55, 210, 190, 18), "Delete the installer"))
                        {

                            this.Close();
                        }
                    }
                }
            }
            else
            {
                if (isCheckUpdates)
                {
                    sty.fontSize = 16;
                    sty.fontStyle = FontStyle.Bold;
                    GUI.Label(new Rect(40, 30, 300, 30), "A new version is available!", sty);
                    sty.fontStyle = FontStyle.Normal;
                    sty.fontSize = 14;
                    GUI.Label(new Rect(60, 70, 100, 30), "A new version:", sty);
                    sty.fontStyle = FontStyle.Bold;
                    GUI.Label(new Rect(200, 70, 100, 30), versionServer.ToString(), sty);
                    sty.fontStyle = FontStyle.Normal;
                    GUI.Label(new Rect(60, 90, 100, 30), "Current version:", sty);
                    sty.fontStyle = FontStyle.Bold;
                    GUI.Label(new Rect(200, 90, 100, 30), version.ToString(), sty);
                    sty.fontStyle = FontStyle.Normal;


                    if (GUI.Button(new Rect(5, 150, 142, 25), "Download new version"))
                    {
                        Application.OpenURL("https://www.assetstore.unity3d.com/#!/content/76657");
                        this.Close();
                    }
                    if (GUI.Button(new Rect(152, 150, 142, 25), "Install current version"))
                    {
                        isCheckUpdatesOk = true;
                    }
                    if (GUI.Button(new Rect(100, 180, 100, 23), "Cancel"))
                    {
                        this.Close();
                    }
                }
                else
                {
                    sty.fontSize = 16;
                    GUI.Label(new Rect(50, 30, 300, 30), "Check the new version ...", sty);
                }
            }



        }

        void exit()
        {
            this.Close();
        }

        void install()
        {
            isInstall = true;

            Type types = System.Type.GetType("AltSystems.AltTerrain.Editor.AltTerrain_Editor");
            if (types != null)
            {
                errorId = 1;
                return;
            }
            else
            {
                string[] dirs = Directory.GetFiles("Assets/", "AltTerrain_Editor.cs", System.IO.SearchOption.AllDirectories);
                if (dirs.Length > 0 && dirs[0] != null)
                {
                    errorId = 1;
                    return;
                }
            }

            /*types = System.Type.GetType("AltSystems.AltTrees.Editor.AltTrees_Editor");
            if (types != null)
            {
                errorId = 2;
                return;
            }
            else
            {
                string[] dirs = Directory.GetFiles("Assets/", "AltTrees_Editor.cs", System.IO.SearchOption.AllDirectories);
                if (dirs.Length > 0 && dirs[0] != null)
                {
                    errorId = 2;
                    return;
                }
            }*/

            string ver = "";

            if (System.IO.File.Exists("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/cfg.txt"))
                System.IO.File.Delete("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/cfg.txt");
            if (System.IO.File.Exists("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/cfg.txt" + ".meta"))
                System.IO.File.Delete("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/cfg.txt" + ".meta");
            

            // AltTrees

            string[] dirs2;
            string[] dirs3;

            dirs2 = Directory.GetFiles("Assets/", "AltTrees_Editor.cs", System.IO.SearchOption.AllDirectories);
            types = System.Type.GetType("AltSystems.AltTrees.Editor.AltTrees_Editor");
            if (types != null || (dirs2.Length > 0 && dirs2[0] != null))
            {
                if (!System.IO.Directory.Exists("AltSystems/AltTrees/OldScriptsTemp"))
                {
                    System.IO.Directory.CreateDirectory("AltSystems/AltTrees/OldScriptsTemp");
                }
                dat = System.DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss");
                string dat2 = "";
                bool stop = false;
                int num = 0;
                while (!stop)
                {
                    if (num > 0)
                        dat2 = dat + "_" + num;
                    else
                        dat2 = dat;

                    if (!System.IO.File.Exists("AltSystems/AltTrees/OldScriptsTemp/" + dat2))
                    {
                        if (num > 0)
                            dat += "_" + num;

                        stop = true;
                    }
                    else
                        num++;
                }
                System.IO.Directory.CreateDirectory("AltSystems/AltTrees/OldScriptsTemp/" + dat);
                System.IO.Directory.CreateDirectory("AltSystems/AltTrees/OldScriptsTemp/" + dat + "/Scripts");
                System.IO.Directory.CreateDirectory("AltSystems/AltTrees/OldScriptsTemp/" + dat + "/Editor");



                if (!System.IO.Directory.Exists("AltSystems/AltBackups/OldScriptsTemp"))
                {
                    System.IO.Directory.CreateDirectory("AltSystems/AltBackups/OldScriptsTemp");
                }
                dat = System.DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss");
                dat2 = "";
                stop = false;
                num = 0;
                while (!stop)
                {
                    if (num > 0)
                        dat2 = dat + "_" + num;
                    else
                        dat2 = dat;

                    if (!System.IO.File.Exists("AltSystems/AltBackups/OldScriptsTemp/" + dat2))
                    {
                        if (num > 0)
                            dat += "_" + num;

                        stop = true;
                    }
                    else
                        num++;
                }
                System.IO.Directory.CreateDirectory("AltSystems/AltBackups/OldScriptsTemp/" + dat);
                System.IO.Directory.CreateDirectory("AltSystems/AltBackups/OldScriptsTemp/" + dat + "/Scripts");
                System.IO.Directory.CreateDirectory("AltSystems/AltBackups/OldScriptsTemp/" + dat + "/Editor");


                FieldInfo myPropInfo = null;
                if (types != null)
                    myPropInfo = types.GetField("version", BindingFlags.Static | BindingFlags.Public);

                string log = "";

                if (myPropInfo != null)
                    ver = (string)myPropInfo.GetValue(null);
                else
                {
                    if (dirs2.Length > 0 && dirs2[0] != null)
                    {
                        string textFileTemp = System.IO.File.ReadAllText(dirs2[0]);
                        if (textFileTemp.IndexOf("namespace AltSystems.AltTrees.Editor") != -1)
                        {
                            if (textFileTemp.IndexOf("public class AltTrees_Editor : UnityEditor.Editor") != -1)
                            {
                                if (textFileTemp.IndexOf("static public string version = ") != -1)
                                {
                                    string[] strsTemp = textFileTemp.Split('"');
                                    if (strsTemp.Length >= 3)
                                    {
                                        ver = strsTemp[1];
                                    }
                                }
                            }
                        }
                    }
                }

                if (ver.Equals("0.9.6"))
                {
                    listScripts = new string[] { "AltAntialiasing.cs", "AltTree.cs", "AltTreeInstance.cs", "AltTrees.cs", "AltTreesDataLinks.cs", "AltTreesManager.cs", "AltTreesManagerData.cs", "AltTreesPatch.cs", "AltUtilities.cs", "PostEffectsBase.cs", "AltWind.cs", "GenerateSpeedTreeWindParamsFile.cs" };
                    listEditorScripts = new string[] { "AboutAltTrees.cs", "AltTree_Editor.cs", "AltTreeInstance_Editor.cs", "AltTrees_Editor.cs", "AltTreesAssetModificationProcessor.cs", "AltTreesMenu.cs", "EditorCoroutines.cs", "ImportTreeWindow.cs", "OffMouseSelect.cs", "InstallerAltTrees.cs", "AltWind_Editor.cs" };



                    for (int i = 0; i < listScripts.Length; i++)
                    {
                        if (!System.IO.File.Exists("Assets/Plugins/AltSystems/AltTrees/Scripts/" + listScripts[i]))
                        {
                            try
                            {
                                string[] dirs = Directory.GetFiles("Assets/", listScripts[i], System.IO.SearchOption.AllDirectories);
                                if (dirs.Length > 0 && dirs[0] != null)
                                {
                                    string textFileTemp = System.IO.File.ReadAllText(dirs[0]);
                                    if (textFileTemp.IndexOf("namespace AltSystems.AltTrees") != -1)
                                    {
                                        if (textFileTemp.IndexOf("class " + listScripts[i].Split('.')[0]) != -1)
                                        {
                                            System.IO.File.Move(dirs[0], "AltSystems/AltTrees/OldScriptsTemp/" + dat + "/Scripts/" + listScripts[i]);
                                            if (System.IO.File.Exists(dirs[0] + ".meta"))
                                                System.IO.File.Delete(dirs[0] + ".meta");
                                            log += listScripts[i] + " moved from " + dirs[0] + Environment.NewLine;
                                        }
                                    }
                                }
                                else
                                    Debug.LogError("no find " + listScripts[i]);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError("Find file " + listScripts[i] + " failed. Error: " + e.ToString());
                            }
                        }
                        else
                        {
                            System.IO.File.Move("Assets/Plugins/AltSystems/AltTrees/Scripts/" + listScripts[i], "AltSystems/AltTrees/OldScriptsTemp/" + dat + "/Scripts/" + listScripts[i]);
                            if (System.IO.File.Exists("Assets/Plugins/AltSystems/AltTrees/Scripts/" + listScripts[i] + ".meta"))
                                System.IO.File.Delete("Assets/Plugins/AltSystems/AltTrees/Scripts/" + listScripts[i] + ".meta");
                            log += listScripts[i] + " moved from " + "Assets/Plugins/AltSystems/AltTrees/Scripts/" + listScripts[i] + Environment.NewLine;
                        }
                    }

                    for (int i = 0; i < listEditorScripts.Length; i++)
                    {
                        if (!System.IO.File.Exists("Assets/Plugins/Editor/AltSystems/AltTrees/" + listEditorScripts[i]))
                        {
                            if (!listEditorScripts[i].Equals("InstallerAltTrees.cs"))
                            {
                                try
                                {
                                    string[] dirs = Directory.GetFiles("Assets/", listEditorScripts[i], System.IO.SearchOption.AllDirectories);
                                    if (dirs.Length > 0 && dirs[0] != null)
                                    {
                                        string textFileTemp = System.IO.File.ReadAllText(dirs[0]);
                                        if (textFileTemp.IndexOf("namespace AltSystems.AltTrees.Editor") != -1)
                                        {
                                            if (textFileTemp.IndexOf("class " + listEditorScripts[i].Split('.')[0]) != -1)
                                            {
                                                System.IO.File.Move(dirs[0], "AltSystems/AltTrees/OldScriptsTemp/" + dat + "/Editor/" + listEditorScripts[i]);
                                                if (System.IO.File.Exists(dirs[0] + ".meta"))
                                                    System.IO.File.Delete(dirs[0] + ".meta");
                                                log += listEditorScripts[i] + " moved from " + dirs[0] + Environment.NewLine;
                                            }
                                        }
                                    }
                                    else
                                        Debug.LogError("no find " + listEditorScripts[i]);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogError("Find file " + listEditorScripts[i] + " failed. Error: " + e.ToString());
                                }
                            }
                        }
                        else
                        {
                            System.IO.File.Move("Assets/Plugins/Editor/AltSystems/AltTrees/" + listEditorScripts[i], "AltSystems/AltTrees/OldScriptsTemp/" + dat + "/Editor/" + listEditorScripts[i]);
                            if (System.IO.File.Exists("Assets/Plugins/Editor/AltSystems/AltTrees/" + listEditorScripts[i] + ".meta"))
                                System.IO.File.Delete("Assets/Plugins/Editor/AltSystems/AltTrees/" + listEditorScripts[i] + ".meta");
                            log += listEditorScripts[i] + " moved from " + "Assets/Plugins/Editor/AltSystems/AltTrees/" + listEditorScripts[i] + Environment.NewLine;
                        }
                    }



                    listScripts = new string[] { "AltBackupIdTerrains.cs", "AltBackupNewsCheck.cs", "AltSystemsNewsWindow.cs", "EditorCoroutines.cs", "CenterOnMainEditorWindow_NoEditor.cs" };
                    listEditorScripts = new string[] { "AboutAltBackup.cs", "AltBackupIdTerrains_Editor.cs", "AltBackupTerrains.cs", "AltBackupTerrainsCreate.cs", "AltBackupTerrainsRestore.cs", "CenterOnMainEditorWindow.cs" };

                    for (int i = 0; i < listScripts.Length; i++)
                    {
                        if (!System.IO.File.Exists("Assets/Plugins/AltSystems/AltBackup/Scripts/" + listScripts[i]))
                        {
                            try
                            {
                                string[] dirs = Directory.GetFiles("Assets/", listScripts[i], System.IO.SearchOption.AllDirectories);
                                if (dirs.Length > 0 && dirs[0] != null)
                                {
                                    string textFileTemp = System.IO.File.ReadAllText(dirs[0]);
                                    if (textFileTemp.IndexOf("namespace AltSystems.AltBackup") != -1)
                                    {
                                        if (textFileTemp.IndexOf("class " + listScripts[i].Split('.')[0]) != -1)
                                        {
                                            System.IO.File.Move(dirs[0], "AltSystems/AltBackups/OldScriptsTemp/" + dat + "/Scripts/" + listScripts[i]);
                                            if (System.IO.File.Exists(dirs[0] + ".meta"))
                                                System.IO.File.Delete(dirs[0] + ".meta");
                                            log += listScripts[i] + " moved from " + dirs[0] + Environment.NewLine;
                                        }
                                    }
                                }
                                else
                                    Debug.LogError("no find " + listScripts[i]);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError("Find file " + listScripts[i] + " failed. Error: " + e.ToString());
                            }
                        }
                        else
                        {
                            System.IO.File.Move("Assets/Plugins/AltSystems/AltBackup/Scripts/" + listScripts[i], "AltSystems/AltBackups/OldScriptsTemp/" + dat + "/Scripts/" + listScripts[i]);
                            if (System.IO.File.Exists("Assets/Plugins/AltSystems/AltBackup/Scripts/" + listScripts[i] + ".meta"))
                                System.IO.File.Delete("Assets/Plugins/AltSystems/AltBackup/Scripts/" + listScripts[i] + ".meta");
                            log += listScripts[i] + " moved from " + "Assets/Plugins/AltSystems/AltBackup/Scripts/" + listScripts[i] + Environment.NewLine;
                        }
                    }

                    for (int i = 0; i < listEditorScripts.Length; i++)
                    {
                        if (!System.IO.File.Exists("Assets/Plugins/Editor/AltSystems/AltBackup/" + listEditorScripts[i]))
                        {
                            if (!listEditorScripts[i].Equals("InstallerAltTrees.cs"))
                            {
                                try
                                {
                                    string[] dirs = Directory.GetFiles("Assets/", listEditorScripts[i], System.IO.SearchOption.AllDirectories);
                                    if (dirs.Length > 0 && dirs[0] != null)
                                    {
                                        string textFileTemp = System.IO.File.ReadAllText(dirs[0]);
                                        if (textFileTemp.IndexOf("namespace AltSystems.AltBackup.Editor") != -1)
                                        {
                                            if (textFileTemp.IndexOf("class " + listEditorScripts[i].Split('.')[0]) != -1)
                                            {
                                                System.IO.File.Move(dirs[0], "AltSystems/AltBackups/OldScriptsTemp/" + dat + "/Editor/" + listEditorScripts[i]);
                                                if (System.IO.File.Exists(dirs[0] + ".meta"))
                                                    System.IO.File.Delete(dirs[0] + ".meta");
                                                log += listEditorScripts[i] + " moved from " + dirs[0] + Environment.NewLine;
                                            }
                                        }
                                    }
                                    else
                                        Debug.LogError("no find " + listEditorScripts[i]);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogError("Find file " + listEditorScripts[i] + " failed. Error: " + e.ToString());
                                }
                            }
                        }
                        else
                        {
                            System.IO.File.Move("Assets/Plugins/Editor/AltSystems/AltBackup/" + listEditorScripts[i], "AltSystems/AltBackups/OldScriptsTemp/" + dat + "/Editor/" + listEditorScripts[i]);
                            if (System.IO.File.Exists("Assets/Plugins/Editor/AltSystems/AltBackup/" + listEditorScripts[i] + ".meta"))
                                System.IO.File.Delete("Assets/Plugins/Editor/AltSystems/AltBackup/" + listEditorScripts[i] + ".meta");
                            log += listEditorScripts[i] + " moved from " + "Assets/Plugins/Editor/AltSystems/AltBackup/" + listEditorScripts[i] + Environment.NewLine;
                        }
                    }
                }
                else if (ver.Equals("0.9.6.1"))
                {
                    listScripts = new string[] { "AltAntialiasing.cs", "AltTree.cs", "AltTreeInstance.cs", "AltTrees.cs", "AltTreesDataLinks.cs", "AltTreesManager.cs", "AltTreesManagerData.cs", "AltTreesPatch.cs", "AltUtilities.cs", "PostEffectsBase.cs", "AltWind.cs", "GenerateSpeedTreeWindParamsFile.cs" };
                    listEditorScripts = new string[] { "AboutAltTrees.cs", "AltTree_Editor.cs", "AltTreeInstance_Editor.cs", "AltTrees_Editor.cs", "AltTreesAssetModificationProcessor.cs", "AltTreesMenu.cs", "EditorCoroutines.cs", "ImportTreeWindow.cs", "OffMouseSelect.cs", "InstallerAltTrees.cs", "AltWind_Editor.cs" };



                    for (int i = 0; i < listScripts.Length; i++)
                    {
                        if (!System.IO.File.Exists("Assets/Plugins/AltSystems/AltTrees/Scripts/" + listScripts[i]))
                        {
                            try
                            {
                                string[] dirs = Directory.GetFiles("Assets/", listScripts[i], System.IO.SearchOption.AllDirectories);
                                if (dirs.Length > 0 && dirs[0] != null)
                                {
                                    string textFileTemp = System.IO.File.ReadAllText(dirs[0]);
                                    if (textFileTemp.IndexOf("namespace AltSystems.AltTrees") != -1)
                                    {
                                        if (textFileTemp.IndexOf("class " + listScripts[i].Split('.')[0]) != -1)
                                        {
                                            System.IO.File.Move(dirs[0], "AltSystems/AltTrees/OldScriptsTemp/" + dat + "/Scripts/" + listScripts[i]);
                                            if (System.IO.File.Exists(dirs[0] + ".meta"))
                                                System.IO.File.Delete(dirs[0] + ".meta");
                                            log += listScripts[i] + " moved from " + dirs[0] + Environment.NewLine;
                                        }
                                    }
                                }
                                else
                                    Debug.LogError("no find " + listScripts[i]);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError("Find file " + listScripts[i] + " failed. Error: " + e.ToString());
                            }
                        }
                        else
                        {
                            System.IO.File.Move("Assets/Plugins/AltSystems/AltTrees/Scripts/" + listScripts[i], "AltSystems/AltTrees/OldScriptsTemp/" + dat + "/Scripts/" + listScripts[i]);
                            if (System.IO.File.Exists("Assets/Plugins/AltSystems/AltTrees/Scripts/" + listScripts[i] + ".meta"))
                                System.IO.File.Delete("Assets/Plugins/AltSystems/AltTrees/Scripts/" + listScripts[i] + ".meta");
                            log += listScripts[i] + " moved from " + "Assets/Plugins/AltSystems/AltTrees/Scripts/" + listScripts[i] + Environment.NewLine;
                        }
                    }

                    for (int i = 0; i < listEditorScripts.Length; i++)
                    {
                        if (!System.IO.File.Exists("Assets/Plugins/Editor/AltSystems/AltTrees/" + listEditorScripts[i]))
                        {
                            if (!listEditorScripts[i].Equals("InstallerAltTrees.cs"))
                            {
                                try
                                {
                                    string[] dirs = Directory.GetFiles("Assets/", listEditorScripts[i], System.IO.SearchOption.AllDirectories);
                                    if (dirs.Length > 0 && dirs[0] != null)
                                    {
                                        string textFileTemp = System.IO.File.ReadAllText(dirs[0]);
                                        if (textFileTemp.IndexOf("namespace AltSystems.AltTrees.Editor") != -1)
                                        {
                                            if (textFileTemp.IndexOf("class " + listEditorScripts[i].Split('.')[0]) != -1)
                                            {
                                                System.IO.File.Move(dirs[0], "AltSystems/AltTrees/OldScriptsTemp/" + dat + "/Editor/" + listEditorScripts[i]);
                                                if (System.IO.File.Exists(dirs[0] + ".meta"))
                                                    System.IO.File.Delete(dirs[0] + ".meta");
                                                log += listEditorScripts[i] + " moved from " + dirs[0] + Environment.NewLine;
                                            }
                                        }
                                    }
                                    else
                                        Debug.LogError("no find " + listEditorScripts[i]);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogError("Find file " + listEditorScripts[i] + " failed. Error: " + e.ToString());
                                }
                            }
                        }
                        else
                        {
                            System.IO.File.Move("Assets/Plugins/Editor/AltSystems/AltTrees/" + listEditorScripts[i], "AltSystems/AltTrees/OldScriptsTemp/" + dat + "/Editor/" + listEditorScripts[i]);
                            if (System.IO.File.Exists("Assets/Plugins/Editor/AltSystems/AltTrees/" + listEditorScripts[i] + ".meta"))
                                System.IO.File.Delete("Assets/Plugins/Editor/AltSystems/AltTrees/" + listEditorScripts[i] + ".meta");
                            log += listEditorScripts[i] + " moved from " + "Assets/Plugins/Editor/AltSystems/AltTrees/" + listEditorScripts[i] + Environment.NewLine;
                        }
                    }



                    listScripts = new string[] { "AltBackupIdTerrains.cs", "AltSystemsNewsCheck.cs", "AltSystemsNewsWindow.cs", "EditorCoroutines.cs", "CenterOnMainEditorWindow_NoEditor.cs" };
                    listEditorScripts = new string[] { "AboutAltBackup.cs", "AltBackupIdTerrains_Editor.cs", "AltBackupTerrains.cs", "AltBackupTerrainsCreate.cs", "AltBackupTerrainsRestore.cs", "CenterOnMainEditorWindow.cs" };

                    for (int i = 0; i < listScripts.Length; i++)
                    {
                        if (!System.IO.File.Exists("Assets/Plugins/AltSystems/AltBackup/Scripts/" + listScripts[i]))
                        {
                            try
                            {
                                string[] dirs = Directory.GetFiles("Assets/", listScripts[i], System.IO.SearchOption.AllDirectories);
                                if (dirs.Length > 0 && dirs[0] != null)
                                {
                                    string textFileTemp = System.IO.File.ReadAllText(dirs[0]);
                                    if (textFileTemp.IndexOf("namespace AltSystems.AltBackup") != -1)
                                    {
                                        if (textFileTemp.IndexOf("class " + listScripts[i].Split('.')[0]) != -1)
                                        {
                                            System.IO.File.Move(dirs[0], "AltSystems/AltBackups/OldScriptsTemp/" + dat + "/Scripts/" + listScripts[i]);
                                            if (System.IO.File.Exists(dirs[0] + ".meta"))
                                                System.IO.File.Delete(dirs[0] + ".meta");
                                            log += listScripts[i] + " moved from " + dirs[0] + Environment.NewLine;
                                        }
                                    }
                                }
                                else
                                    Debug.LogError("no find " + listScripts[i]);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError("Find file " + listScripts[i] + " failed. Error: " + e.ToString());
                            }
                        }
                        else
                        {
                            System.IO.File.Move("Assets/Plugins/AltSystems/AltBackup/Scripts/" + listScripts[i], "AltSystems/AltBackups/OldScriptsTemp/" + dat + "/Scripts/" + listScripts[i]);
                            if (System.IO.File.Exists("Assets/Plugins/AltSystems/AltBackup/Scripts/" + listScripts[i] + ".meta"))
                                System.IO.File.Delete("Assets/Plugins/AltSystems/AltBackup/Scripts/" + listScripts[i] + ".meta");
                            log += listScripts[i] + " moved from " + "Assets/Plugins/AltSystems/AltBackup/Scripts/" + listScripts[i] + Environment.NewLine;
                        }
                    }

                    for (int i = 0; i < listEditorScripts.Length; i++)
                    {
                        if (!System.IO.File.Exists("Assets/Plugins/Editor/AltSystems/AltBackup/" + listEditorScripts[i]))
                        {
                            if (!listEditorScripts[i].Equals("InstallerAltTrees.cs"))
                            {
                                try
                                {
                                    string[] dirs = Directory.GetFiles("Assets/", listEditorScripts[i], System.IO.SearchOption.AllDirectories);
                                    if (dirs.Length > 0 && dirs[0] != null)
                                    {
                                        string textFileTemp = System.IO.File.ReadAllText(dirs[0]);
                                        if (textFileTemp.IndexOf("namespace AltSystems.AltBackup.Editor") != -1)
                                        {
                                            if (textFileTemp.IndexOf("class " + listEditorScripts[i].Split('.')[0]) != -1)
                                            {
                                                System.IO.File.Move(dirs[0], "AltSystems/AltBackups/OldScriptsTemp/" + dat + "/Editor/" + listEditorScripts[i]);
                                                if (System.IO.File.Exists(dirs[0] + ".meta"))
                                                    System.IO.File.Delete(dirs[0] + ".meta");
                                                log += listEditorScripts[i] + " moved from " + dirs[0] + Environment.NewLine;
                                            }
                                        }
                                    }
                                    else
                                        Debug.LogError("no find " + listEditorScripts[i]);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogError("Find file " + listEditorScripts[i] + " failed. Error: " + e.ToString());
                                }
                            }
                        }
                        else
                        {
                            System.IO.File.Move("Assets/Plugins/Editor/AltSystems/AltBackup/" + listEditorScripts[i], "AltSystems/AltBackups/OldScriptsTemp/" + dat + "/Editor/" + listEditorScripts[i]);
                            if (System.IO.File.Exists("Assets/Plugins/Editor/AltSystems/AltBackup/" + listEditorScripts[i] + ".meta"))
                                System.IO.File.Delete("Assets/Plugins/Editor/AltSystems/AltBackup/" + listEditorScripts[i] + ".meta");
                            log += listEditorScripts[i] + " moved from " + "Assets/Plugins/Editor/AltSystems/AltBackup/" + listEditorScripts[i] + Environment.NewLine;
                        }
                    }
                }

                System.IO.File.WriteAllText("AltSystems/AltTrees/OldScriptsTemp/" + dat + "/log.txt", log);
                System.IO.File.WriteAllText("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/cfg.txt", "1");
            }
            else
            {

                // AltBackup


                dirs2 = Directory.GetFiles("Assets/", "AltBackupTerrains.cs", System.IO.SearchOption.AllDirectories);
                dirs3 = Directory.GetFiles("Assets/", "AltBackup.dll", System.IO.SearchOption.AllDirectories);
                types = System.Type.GetType("AltSystems.AltBackup.Editor.AltBackupTerrains");
                if (types != null || (dirs2.Length > 0 && dirs2[0] != null) || (dirs3.Length > 0 && dirs3[0] != null))
                {
                    if (!System.IO.Directory.Exists("AltSystems/AltBackups/OldScriptsTemp"))
                    {
                        System.IO.Directory.CreateDirectory("AltSystems/AltBackups/OldScriptsTemp");
                    }
                    dat = System.DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss");
                    string dat2 = "";
                    bool stop = false;
                    int num = 0;
                    while (!stop)
                    {
                        if (num > 0)
                            dat2 = dat + "_" + num;
                        else
                            dat2 = dat;

                        if (!System.IO.File.Exists("AltSystems/AltBackups/OldScriptsTemp/" + dat2))
                        {
                            if (num > 0)
                                dat += "_" + num;

                            stop = true;
                        }
                        else
                            num++;
                    }
                    System.IO.Directory.CreateDirectory("AltSystems/AltBackups/OldScriptsTemp/" + dat);
                    System.IO.Directory.CreateDirectory("AltSystems/AltBackups/OldScriptsTemp/" + dat + "/Scripts");
                    System.IO.Directory.CreateDirectory("AltSystems/AltBackups/OldScriptsTemp/" + dat + "/Editor");


                    FieldInfo myPropInfo = null;
                    if (types != null)
                        myPropInfo = types.GetField("version", BindingFlags.Static | BindingFlags.Public);

                    string log = "";

                    if (myPropInfo != null)
                        ver = (string)myPropInfo.GetValue(null);
                    else
                    {
                        if (dirs2.Length > 0 && dirs2[0] != null)
                        {
                            string textFileTemp = System.IO.File.ReadAllText(dirs2[0]);
                            if (textFileTemp.IndexOf("namespace AltSystems.AltBackup.Editor") != -1)
                            {
                                if (textFileTemp.IndexOf("public class AltBackupTerrains : EditorWindow") != -1)
                                {
                                    if (textFileTemp.IndexOf("static public string version = ") != -1)
                                    {
                                        string[] strsTemp = textFileTemp.Split('"');
                                        if (strsTemp.Length >= 3)
                                        {
                                            ver = strsTemp[1];
                                        }
                                    }
                                }
                            }
                        }
                        else if (dirs3.Length > 0 && dirs3[0] != null)
                        {
                            ver = "";
                        }
                    }

                    if (!ver.Equals(""))
                    {
                        if (ver.Equals("1.1"))
                        {
                            listScripts = new string[] { "AltBackupIdTerrains.cs", "AltBackupNewsCheck.cs", "AltSystemsNewsWindow.cs", "EditorCoroutines.cs", "CenterOnMainEditorWindow_NoEditor.cs" };
                            listEditorScripts = new string[] { "AboutAltBackup.cs", "AltBackupIdTerrains_Editor.cs", "AltBackupTerrains.cs", "AltBackupTerrainsCreate.cs", "AltBackupTerrainsRestore.cs", "InstallerAltTrees.cs", "CenterOnMainEditorWindow.cs" };

                            for (int i = 0; i < listScripts.Length; i++)
                            {
                                if (!System.IO.File.Exists("Assets/Plugins/AltSystems/AltBackup/Scripts/" + listScripts[i]))
                                {
                                    try
                                    {
                                        string[] dirs = Directory.GetFiles("Assets/", listScripts[i], System.IO.SearchOption.AllDirectories);
                                        if (dirs.Length > 0 && dirs[0] != null)
                                        {
                                            string textFileTemp = System.IO.File.ReadAllText(dirs[0]);
                                            if (textFileTemp.IndexOf("namespace AltSystems.AltBackup") != -1)
                                            {
                                                if (textFileTemp.IndexOf("class " + listScripts[i].Split('.')[0]) != -1)
                                                {
                                                    System.IO.File.Move(dirs[0], "AltSystems/AltBackups/OldScriptsTemp/" + dat + "/Scripts/" + listScripts[i]);
                                                    if (System.IO.File.Exists(dirs[0] + ".meta"))
                                                        System.IO.File.Delete(dirs[0] + ".meta");
                                                    log += listScripts[i] + " moved from " + dirs[0] + Environment.NewLine;
                                                }
                                            }
                                        }
                                        else
                                            Debug.LogError("no find " + listScripts[i]);
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.LogError("Find file " + listScripts[i] + " failed. Error: " + e.ToString());
                                    }
                                }
                                else
                                {
                                    System.IO.File.Move("Assets/Plugins/AltSystems/AltBackup/Scripts/" + listScripts[i], "AltSystems/AltBackups/OldScriptsTemp/" + dat + "/Scripts/" + listScripts[i]);
                                    if (System.IO.File.Exists("Assets/Plugins/AltSystems/AltBackup/Scripts/" + listScripts[i] + ".meta"))
                                        System.IO.File.Delete("Assets/Plugins/AltSystems/AltBackup/Scripts/" + listScripts[i] + ".meta");
                                    log += listScripts[i] + " moved from " + "Assets/Plugins/AltSystems/AltBackup/Scripts/" + listScripts[i] + Environment.NewLine;
                                }
                            }

                            for (int i = 0; i < listEditorScripts.Length; i++)
                            {
                                if (!System.IO.File.Exists("Assets/Plugins/Editor/AltSystems/AltBackup/" + listEditorScripts[i]))
                                {
                                    if (!listEditorScripts[i].Equals("InstallerAltTrees.cs"))
                                    {
                                        try
                                        {
                                            string[] dirs = Directory.GetFiles("Assets/", listEditorScripts[i], System.IO.SearchOption.AllDirectories);
                                            if (dirs.Length > 0 && dirs[0] != null)
                                            {
                                                string textFileTemp = System.IO.File.ReadAllText(dirs[0]);
                                                if (textFileTemp.IndexOf("namespace AltSystems.AltBackup.Editor") != -1)
                                                {
                                                    if (textFileTemp.IndexOf("class " + listEditorScripts[i].Split('.')[0]) != -1)
                                                    {
                                                        System.IO.File.Move(dirs[0], "AltSystems/AltBackups/OldScriptsTemp/" + dat + "/Editor/" + listEditorScripts[i]);
                                                        if (System.IO.File.Exists(dirs[0] + ".meta"))
                                                            System.IO.File.Delete(dirs[0] + ".meta");
                                                        log += listEditorScripts[i] + " moved from " + dirs[0] + Environment.NewLine;
                                                    }
                                                }
                                            }
                                            else
                                                Debug.LogError("no find " + listEditorScripts[i]);
                                        }
                                        catch (Exception e)
                                        {
                                            Debug.LogError("Find file " + listEditorScripts[i] + " failed. Error: " + e.ToString());
                                        }
                                    }
                                }
                                else
                                {
                                    System.IO.File.Move("Assets/Plugins/Editor/AltSystems/AltBackup/" + listEditorScripts[i], "AltSystems/AltBackups/OldScriptsTemp/" + dat + "/Editor/" + listEditorScripts[i]);
                                    if (System.IO.File.Exists("Assets/Plugins/Editor/AltSystems/AltBackup/" + listEditorScripts[i] + ".meta"))
                                        System.IO.File.Delete("Assets/Plugins/Editor/AltSystems/AltBackup/" + listEditorScripts[i] + ".meta");
                                    log += listEditorScripts[i] + " moved from " + "Assets/Plugins/Editor/AltSystems/AltBackup/" + listEditorScripts[i] + Environment.NewLine;
                                }
                            }
                        }
                    }
                    else
                    {
                        listScripts = new string[] { "AltBackup.dll" };
                        listEditorScripts = new string[] { "AltBackup Editor.dll" };


                        if (Directory.Exists("AltBackups"))
                        {
                            CopyFolder("AltBackups", "AltSystems/AltBackups");

                            string[] filesTemp = Directory.GetFiles("AltBackups");
                            if (filesTemp.Length == 0)
                            {
                                Directory.Delete("AltBackups");
                            }
                        }



                        for (int i = 0; i < listScripts.Length; i++)
                        {
                            if (!System.IO.File.Exists("Assets/Plugins/AltSystems/AltBackup/Scripts/" + listScripts[i]))
                            {
                                try
                                {
                                    string[] dirs = Directory.GetFiles("Assets/", listScripts[i], System.IO.SearchOption.AllDirectories);
                                    if (dirs.Length > 0 && dirs[0] != null)
                                    {
                                        System.IO.File.Move(dirs[0], "AltSystems/AltBackups/OldScriptsTemp/" + dat + "/Scripts/" + listScripts[i]);
                                        if (System.IO.File.Exists(dirs[0] + ".meta"))
                                            System.IO.File.Delete(dirs[0] + ".meta");
                                        log += listScripts[i] + " moved from " + dirs[0] + Environment.NewLine;
                                    }
                                    else
                                        Debug.LogError("no find " + listScripts[i]);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogError("Find file " + listScripts[i] + " failed. Error: " + e.ToString());
                                }
                            }
                            else
                            {
                                System.IO.File.Move("Assets/Plugins/AltSystems/AltBackup/Scripts/" + listScripts[i], "AltSystems/AltBackups/OldScriptsTemp/" + dat + "/Scripts/" + listScripts[i]);
                                if (System.IO.File.Exists("Assets/Plugins/AltSystems/AltBackup/Scripts/" + listScripts[i] + ".meta"))
                                    System.IO.File.Delete("Assets/Plugins/AltSystems/AltBackup/Scripts/" + listScripts[i] + ".meta");
                                log += listScripts[i] + " moved from " + "Assets/Plugins/AltSystems/AltBackup/Scripts/" + listScripts[i] + Environment.NewLine;
                            }
                        }

                        for (int i = 0; i < listEditorScripts.Length; i++)
                        {
                            if (!System.IO.File.Exists("Assets/Plugins/Editor/AltBackup/" + listEditorScripts[i]))
                            {
                                try
                                {
                                    string[] dirs = Directory.GetFiles("Assets/", listEditorScripts[i], System.IO.SearchOption.AllDirectories);
                                    if (dirs.Length > 0 && dirs[0] != null)
                                    {
                                        System.IO.File.Move(dirs[0], "AltSystems/AltBackups/OldScriptsTemp/" + dat + "/Editor/" + listEditorScripts[i]);
                                        if (System.IO.File.Exists(dirs[0] + ".meta"))
                                            System.IO.File.Delete(dirs[0] + ".meta");
                                        log += listEditorScripts[i] + " moved from " + dirs[0] + Environment.NewLine;
                                    }
                                    else
                                        Debug.LogError("no find " + listEditorScripts[i]);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogError("Find file " + listEditorScripts[i] + " failed. Error: " + e.ToString());
                                }
                            }
                            else
                            {
                                System.IO.File.Move("Assets/Plugins/Editor/AltBackup/" + listEditorScripts[i], "AltSystems/AltBackups/OldScriptsTemp/" + dat + "/Editor/" + listEditorScripts[i]);
                                if (System.IO.File.Exists("Assets/Plugins/Editor/AltBackup/" + listEditorScripts[i] + ".meta"))
                                    System.IO.File.Delete("Assets/Plugins/Editor/AltBackup/" + listEditorScripts[i] + ".meta");
                                log += listEditorScripts[i] + " moved from " + "Assets/Plugins/Editor/AltBackup/" + listEditorScripts[i] + Environment.NewLine;
                            }
                        }

                        string[] filesTemp2 = Directory.GetFiles("Assets/Plugins/Editor/AltBackup");
                        if (filesTemp2.Length == 0)
                        {
                            Directory.Delete("Assets/Plugins/Editor/AltBackup");
                            if (System.IO.File.Exists("Assets/Plugins/Editor/AltBackup" + ".meta"))
                                System.IO.File.Delete("Assets/Plugins/Editor/AltBackup" + ".meta");
                        }
                    }

                    System.IO.File.WriteAllText("AltSystems/AltBackups/OldScriptsTemp/" + dat + "/log.txt", log);
                    System.IO.File.WriteAllText("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/cfg.txt", "1");
                }
            }

            AssetDatabase.Refresh();
            AssetDatabase.ImportPackage("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/package.unitypackage", false);
            AssetDatabase.Refresh();

            System.IO.File.Delete("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/package.unitypackage");
            if (System.IO.File.Exists("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/package.unitypackage" + ".meta"))
                System.IO.File.Delete("Assets/Plugins/Editor/AltSystems/AltTrees/InstallerAltTrees/package.unitypackage" + ".meta");

            AssetDatabase.Refresh();



            isInstallOk = true;
        }

        void CopyFolder(string sourceFolder, string destFolder)
        {

            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            string[] files = Directory.GetFiles(sourceFolder);

            foreach (string file in files)
                File.Move(file, Path.Combine(destFolder, Path.GetFileName(file)));

            string[] folders = Directory.GetDirectories(sourceFolder);

            foreach (string folder in folders)
                CopyFolder(folder, Path.Combine(destFolder, Path.GetFileName(folder)));
        }


        IEnumerator checkUpdates()
        {
            string ver = "-";
            Type types = System.Type.GetType("AltSystems.AltTerrain.Editor.AltTerrain_Editor");
            if (types != null)
            {
                ver = "terr";
            }
            else
            {
                string[] dirs = Directory.GetFiles("Assets/", "AltTerrain_Editor.cs", System.IO.SearchOption.AllDirectories);
                if (dirs.Length > 0 && dirs[0] != null)
                {
                    ver = "terr";
                }
                else
                {
                    types = System.Type.GetType("AltSystems.AltTrees.Editor.AltTrees_Editor");
                    if (types != null)
                    {
                        FieldInfo myPropInfo = types.GetField("version", BindingFlags.Static | BindingFlags.Public);

                        if (myPropInfo != null)
                        {
                            ver = "trees_" + (string)myPropInfo.GetValue(null);
                            currentVersion = (string)myPropInfo.GetValue(null);
                        }
                        else
                            ver = "trees";
                    }
                    else
                    {
                        dirs = Directory.GetFiles("Assets/", "AltTrees_Editor.cs", System.IO.SearchOption.AllDirectories);
                        if (dirs.Length > 0 && dirs[0] != null)
                        {
                            ver = "trees";
                        }
                        else
                        {
                            types = System.Type.GetType("AltSystems.AltBackup.Editor.AltBackupTerrains");
                            if (types != null)
                            {
                                FieldInfo myPropInfo = types.GetField("version", BindingFlags.Static | BindingFlags.Public);

                                if (myPropInfo != null)
                                    ver = "backup_" + (string)myPropInfo.GetValue(null);
                            }
                            else
                            {
                                string[] dirs3 = Directory.GetFiles("Assets/", "AltBackup.dll", System.IO.SearchOption.AllDirectories);

                                if (dirs3.Length > 0 && dirs3[0] != null)
                                {
                                    ver = "backup_1.0";
                                }
                            }
                        }
                    }
                }
            }



            WWW www = new WWW("http://altsystems-unity.net/getLatestVersionAltTrees.php?unity=" + Application.unityVersion + "&asset=AltTrees&ver=" + ver + "&newVer=" + version);

            double time = EditorApplication.timeSinceStartup;
            int dd = 0;
            bool isExit = false;

            while (!www.isDone && string.IsNullOrEmpty(www.error) && !isExit)
            {
                dd++;
                if (EditorApplication.timeSinceStartup - time >= 3 || dd > 500)
                {
                    isCheckUpdates = true;
                    isCheckUpdatesOk = true;
                    isExit = true;
                }
                yield return www;
            };


            Repaint();

            if (www.isDone)
            {
                if (string.IsNullOrEmpty(www.error))
                {
                    if (www.text.Equals(version))
                    {
                        isCheckUpdates = true;
                        isCheckUpdatesOk = true;
                    }
                    else
                    {
                        isCheckUpdates = true;
                        isCheckUpdatesOk = true;
                        versionServer = www.text;


                        float n = 0;
                        float m = 0;
                        string[] strs = version.Split('.');
                        string[] strsServ = versionServer.Split('.');

                        string versionStr = "";
                        string versionServerStr = "";


                        for (int i = 0; i < strs.Length; i++)
                        {
                            if (i == 0)
                                versionStr += strs[i] + ".";
                            else
                                versionStr += strs[i];
                        }
                        for (int i = 0; i < strsServ.Length; i++)
                        {
                            if (i == 0)
                                versionServerStr += strsServ[i] + ".";
                            else
                                versionServerStr += strsServ[i];
                        }

                        if (float.TryParse(versionStr, out n) && float.TryParse(versionServerStr, out m))
                        {
                            if (m > n)
                                isCheckUpdatesOk = false;
                        }
                    }
                }
                else
                {
                    Debug.LogWarning(www.error);
                    isCheckUpdates = true;
                    isCheckUpdatesOk = true;
                }
            }
            else
            {
                isCheckUpdates = true;
                isCheckUpdatesOk = true;
            }
            Repaint();
        }
    }

    public class EditorCoroutinesInstaller
    {
        IEnumerator coroutine;

        EditorCoroutinesInstaller(IEnumerator _coroutine)
        {
            coroutine = _coroutine;
        }

        public static void Start(IEnumerator _coroutine)
        {
            EditorCoroutinesInstaller coroutine = new EditorCoroutinesInstaller(_coroutine);
            coroutine._start();
        }

        void _start()
        {
            EditorApplication.update += _update;
        }

        void _update()
        {
            if (!coroutine.MoveNext())
            {
                EditorApplication.update -= _update;
            }
        }
    }

    static class CenterOnMainEditorWindowInstaller
    {
        static System.Type[] GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type aType)
        {
            var result = new List<System.Type>();
            var assemblies = aAppDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(aType))
                        result.Add(type);
                }
            }
            return result.ToArray();
        }

        static Rect GetEditorMainWindowPos()
        {
            var containerWinType = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(ScriptableObject)).Where(t => t.Name == "ContainerWindow").FirstOrDefault();
            if (containerWinType == null)
                throw new System.MissingMemberException("Can't find internal type ContainerWindow. Maybe something has changed inside Unity");
            var showModeField = containerWinType.GetField("m_ShowMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var positionProperty = containerWinType.GetProperty("position", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (showModeField == null || positionProperty == null)
                throw new System.MissingFieldException("Can't find internal fields 'm_ShowMode' or 'position'. Maybe something has changed inside Unity");
            var windows = Resources.FindObjectsOfTypeAll(containerWinType);
            foreach (var win in windows)
            {
                var showmode = (int)showModeField.GetValue(win);
                if (showmode == 4) // main window
                {
                    var pos = (Rect)positionProperty.GetValue(win, null);
                    return pos;
                }
            }
            throw new System.NotSupportedException("Can't find internal main window. Maybe something has changed inside Unity");
        }

        public static void CenterOnMainWinInstaller(this UnityEditor.EditorWindow aWin)
        {
            var main = GetEditorMainWindowPos();
            var pos = aWin.position;
            float w = (main.width - pos.width) * 0.5f;
            float h = (main.height - pos.height) * 0.5f;
            pos.x = main.x + w;
            pos.y = main.y + h / 2f;
            aWin.position = pos;
        }
    }
}