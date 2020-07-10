/****************************************************************************
 *
 * Copyright (c) 2018 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.IO;
#if (UNITY_5 || UNITY_2017_1_OR_NEWER) && (UNITY_IOS || UNITY_TVOS)
using UnityEditor.iOS.Xcode;
#endif

public class CriWareBuildPostprocessor : ScriptableObject
{
	private static string prefsFilePath = "Assets/Editor/CriWare/CriWareBuildPostprocessorPrefs.asset";
	public bool iosAddDependencyFrameworks	= true;
	public bool iosReorderLibraryLinkingsForVp9 = true;

	[MenuItem("GameObject/CRIWARE/Create CriWareBuildPostprocessorPrefs.asset")]
	public static void Create()
	{
		CriWareBuildPostprocessor instance = (CriWareBuildPostprocessor)AssetDatabase.LoadAssetAtPath(prefsFilePath, typeof(CriWareBuildPostprocessor));
		if (instance) {
			Debug.LogError("[CRIWARE] Preferences file of CriWareBuildPostprocessor already exists. '(" + prefsFilePath + ")");
			Selection.activeObject = instance;
			return;
		}
		
		var scobj = ScriptableObject.CreateInstance<CriWareBuildPostprocessor>();
		if (scobj == null) {
			Debug.Log("[CRIWARE] Failed to create CriWareBuildPostprocessor");
			return;
		}
		
		Directory.CreateDirectory(Path.GetDirectoryName(prefsFilePath));
		AssetDatabase.CreateAsset(scobj, prefsFilePath);
		
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		Debug.Log("[CRIWARE] Created the preferences file of CriWareBuildPostprocessor. (" + prefsFilePath + ")");

		Selection.activeObject = scobj;
	}
	
	
	[PostProcessScene]
	public static void OnPostProcessScene() {
		CheckGraphicsApiProcess ();
	}
	
	
	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget build_target, string path)
	{
		CriWareBuildPostprocessor instance = (CriWareBuildPostprocessor)AssetDatabase.LoadAssetAtPath(prefsFilePath, typeof(CriWareBuildPostprocessor));
		if (instance == null) {
			instance = new CriWareBuildPostprocessor();
			Debug.Log(
				"[CRIWARE] Run CriWareBuildPostprocessor.OnPostprocessBuild with default preferences.\n"
				+ "If you want to change the preferences, please create the preferences file by 'CRI/Create CriWareBuildPostprocessorPrefs.asset' menu."
				);
		} else {
			Debug.Log(
				"[CRIWARE] Run CriWareBuildPostprocessor.OnPostprocessBuild with default preferences.\n"
				+ "If you want to change the preferences, please edit the preferences file (" + prefsFilePath + ")"
				);
		}

		instance.IosAddDependencyFrameworksProcess(build_target, path,
		                                           instance.iosAddDependencyFrameworks,
		                                           instance.iosReorderLibraryLinkingsForVp9);
	}


	private void IosAddDependencyFrameworksProcess(BuildTarget build_target, string path, bool add_frameworks, bool reorder_linkings)
	{
#if UNITY_IOS || UNITY_TVOS
#if UNITY_5 || UNITY_2017_1_OR_NEWER
		if ((build_target != BuildTarget.iOS && build_target != BuildTarget.tvOS) ||
		    (!add_frameworks && !reorder_linkings)) {
			return;
		}
		string project_path = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
		PBXProject project = new PBXProject();
		project.ReadFromString(File.ReadAllText(project_path));
		string target = project.TargetGuidByName("UnityFramework");
		if (string.IsNullOrEmpty(target)) {
			target = project.TargetGuidByName("Unity-iPhone");
		}

		if (add_frameworks) {
			Debug.Log("[CRIWARE][iOS] Add dependency frameworks (VideoToolbox.framework, Metal.framework)");
			if (build_target == BuildTarget.iOS) {
				project.AddFrameworkToProject(target, "VideoToolbox.framework", false);
			}
			project.AddFrameworkToProject(target, "Metal.framework", false);
		}
		if (reorder_linkings) {
			bool isVp9Enabled = project.ContainsFileByRealPath("Libraries/Plugins/iOS/libcri_mana_vpx.a");
 			if (isVp9Enabled) {
				string guid = project.FindFileGuidByRealPath("Libraries/libiPhone-lib.a", PBXSourceTree.Source);
				if (!string.IsNullOrEmpty(guid)) {
					project.RemoveFileFromBuild(target, guid);
					project.AddFileToBuild(target, guid);
					Debug.Log("[CRIWARE][iOS] Reordered linking of libiPhone-lib.a for VP9 support.");
				} else {
					Debug.LogWarning("[CRIWARE][iOS] Failed to reorder linkings of libraries for VP9 support.");
				}
			}
		}

		File.WriteAllText(project_path, project.WriteToString());
#else
		if (build_target != BuildTarget.iPhone) {
			return;
		}
		Debug.LogWarning("[CRIWARE][iOS] Please add dependency frameworks (VideoToolbox.framework, Metal.framework) on Xcode.");
#endif
#endif
	}
	
	
	private static bool CheckGraphicsApiProcess() {
		return true;
	}
}

