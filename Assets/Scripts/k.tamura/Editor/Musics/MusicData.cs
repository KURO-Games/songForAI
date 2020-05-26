using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class MusicData : EditorWindow
{
    public static MusicDataSettings msettings;
    string MName;
    string MBPM;
    string Composer;
    string NortsDesigner;
    string FilePath;
    [MenuItem("SingForAI-Tools/Music/DataCreate")]
    private static void Open()
    {
        // 生成
        EditorWindow.GetWindow<MusicData>("MusicDataCreate");
    }

    
    private void OnGUI()
    {
        EditorGUILayout.LabelField("曲のデータを作るところ");


        EditorGUI.BeginChangeCheck();
        if (msettings != null)
        {
            GUILayout.Label("ファイル名");
            GUILayout.Label(FilePath);
            GUILayout.Label("MusicName");
            MName= GUILayout.TextArea(msettings.MusicName);
            GUILayout.Label("MusicBPM");
            MBPM = GUILayout.TextArea(msettings.MusicBPM.ToString());
            GUILayout.Label("Composer");
            Composer = GUILayout.TextArea(msettings.Composer);
            GUILayout.Label("NortsDesignerName");
            NortsDesigner = GUILayout.TextArea(msettings.NortsDesigner);
        }

        if (EditorGUI.EndChangeCheck())// 変更を検知した場合、設定ファイルに戻す
        { 
            UnityEditor.Undo.RecordObject(msettings, "Edit ExampleEditorWindow"); 
            msettings.MusicName = MName;
            msettings.MusicBPM = float.Parse(MBPM);
            msettings.Composer = Composer;
            msettings.NortsDesigner = NortsDesigner;
            EditorUtility.SetDirty(msettings);
        }
        if (GUILayout.Button("Load"))
        {
            Debug.Log("Button!");
            Load();
        }
        if (GUILayout.Button("NewCreate"))
        {
            Debug.Log("Button!");
            Create();
        }
    }
    private void Load()
    {
        FilePath = EditorUtility.OpenFilePanel("曲データをロードする","","");
        string[] FilePaths = FilePath.Split('/');
        FilePath = "";
        int index = Array.IndexOf(FilePaths, "Assets");
        Debug.Log(index);
        for (; index < FilePaths.Length; index++)
        {

            FilePath += FilePaths[index] ;
            if (index<FilePaths.Length-1)
            {
                FilePath += "/";
            }
        }
        Debug.Log(FilePath);
        msettings = AssetDatabase.LoadAssetAtPath<MusicDataSettings>(FilePath);
        Repaint();
    }
    private void Save(string FPath)
    {
        if(FPath.Length != 0)
        {
            msettings.MusicName = MName;
            msettings.MusicBPM = float.Parse(MBPM);
            msettings.Composer = Composer;
            msettings.NortsDesigner = NortsDesigner;
            EditorUtility.SetDirty(msettings);
            Repaint();
            return;
        }
    }
    private void Create()
    {
        FilePath = EditorUtility.SaveFilePanelInProject("曲データをセーブする", "musicName", "asset", ".assetでセーブしてください。");
        if (FilePath.Length != 0)
        {
            AssetDatabase.CreateAsset(CreateInstance <MusicDataSettings>(), FilePath);
        }
    }
    private void Update()
    {
        Repaint();
    }
}
