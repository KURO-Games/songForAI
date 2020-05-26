using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MusicData : EditorWindow
{
    public static MusicDataSettings msettings;
    string MName;
    string MBPM;
    string Composer;
    string NortsDesigner;
    string FilePath;
    string[] FileFilter = {"asset","*"}; 
    [MenuItem("SingForAI-Tools/Music/DataCreate")]
    private static void Open()
    {
        // 生成
        EditorWindow.GetWindow<MusicData>("MusicDataCreate");
    }

    
    private void OnGUI()
    {
        EditorGUILayout.LabelField("曲のデータを作るところ");
        GUILayout.Label("MusicName");
        MName = GUILayout.TextArea(MName);
        //msettings.MusicName = MName;
        GUILayout.Label("MusicBPM");
        MBPM = GUILayout.TextArea(MBPM);
        //msettings.MusicBPM = float.Parse(MBPM);
        GUILayout.Label("Composer");
        Composer = GUILayout.TextArea(Composer);
        //msettings.Composer = Composer;
        GUILayout.Label("NortsDesignerName");
        NortsDesigner = GUILayout.TextArea(NortsDesigner);

        //msettings.NortsDesigner = NortsDesigner;

        if (GUILayout.Button("Load"))
        {
            Debug.Log("Button!");
            Load();
        }
        if (GUILayout.Button("Save"))
        {
            Debug.Log("Button!");
            Save(FilePath);

        }
        if (GUILayout.Button("Create"))
        {
            Debug.Log("Button!");
            Create();

        }
    }
    private void Load()
    {
        FilePath = EditorUtility.OpenFilePanelWithFilters("曲データをロードする","", FileFilter);
        msettings = AssetDatabase.LoadAssetAtPath<MusicDataSettings>(FilePath);
    }
    private void Save(string FPath)
    {
        if(FPath.Length != 0)
        {

            return;
        }
        FilePath = EditorUtility.SaveFilePanelInProject("曲データをセーブする", MName, "asset", ".assetでセーブしてください。");
        if (FilePath.Length != 0)
        {
            AssetDatabase.CreateAsset(msettings, FilePath);
        }
    }
    private void Create()
    {
        FilePath = EditorUtility.SaveFilePanelInProject("曲データをセーブする", MName, "asset", ".assetでセーブしてください。");
        if (FilePath.Length != 0)
        {
            AssetDatabase.CreateAsset(msettings, FilePath);
        }
    }
}
