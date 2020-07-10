/****************************************************************************
 *
 * Copyright (c) 2011 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public sealed class CriAtomWindow : EditorWindow, ISerializationCallbackReceiver
{
	#region Variables
	private int selectedCueId = 0;
	private int selectedCueInfoIndex = 0;
	private int selectedCueSheetId = 0;
	private bool isCueSheetListInitiated = false;
	private int lastPreviewCueSheetId = -1;
	private Vector2 scrollPos;
	private Vector2 scrollPos_Window;
	private Rect windowRect = new Rect(10, 10, 100, 100);
	private bool scaling = true;
	private bool useCopyAssetsFromCriAtomCraft = false;
	// Public
	public string searchPath = "";
	public string acfPath = "";
	public string dspBusSetting = "";
	public CriAtomAcfInfo.AcfInfo acfInfoData;

	private CriAtomWindowPrefs criAtomWindowPrefs = null;
	const string pathToJson = "Assets/Editor/CriWare/CriAtom/saveAcfData.json";
	#endregion

	#region Functions
	public void OnAfterDeserialize()
	{
	#if UNITY_5_3_OR_NEWER
		string tmp_json;
		if (System.IO.File.Exists(pathToJson)) {
			tmp_json = File.ReadAllText(pathToJson);
			acfInfoData = JsonUtility.FromJson<CriAtomAcfInfo.AcfInfo>(tmp_json);
		} else {
			tmp_json = null;
			acfInfoData = null;
		}
	#endif
	}

	public void OnBeforeSerialize()
	{
	#if UNITY_5_3_OR_NEWER
		string tmp_json;
		tmp_json = JsonUtility.ToJson(acfInfoData);
		File.WriteAllText(pathToJson, tmp_json);
	#endif
	}

	private static string tempJsonPath()
	{
		string filePath = "Assets/Editor/CriWare/CriAtom/saveAcfData";
		filePath += ".json";
		return filePath;
	}

	public void OnDestroy()
	{
		if (System.IO.File.Exists(pathToJson)) {
			System.IO.File.Delete(tempJsonPath());
			System.IO.File.Delete(tempJsonPath() + ".meta");
		}
		acfInfoData = null;
	}

	[MenuItem("Window/CRIWARE/Open CRI Atom Window", false, 100)]
	static void OpenWindow()
	{
		EditorWindow.GetWindow<CriAtomWindow>(false, "CRI Atom");
	}

	void OnSelectionChange()
	{
		Repaint();
	}

	void OnFocus()
	{
	}

	private void OnEnable()
	{
		searchPath = Application.streamingAssetsPath;
		criAtomWindowPrefs = CriAtomWindowPrefs.Load();
	}

	private void OnDisable()
	{
	}

	private void GUISearchAndRelaod()
	{
		EditorGUILayout.BeginHorizontal();
		{
			this.searchPath = EditorGUILayout.TextField("Search File Path", this.searchPath, EditorStyles.label);

			if (EditorApplication.isPlaying) {
				GUI.color = Color.white;
			} else {
				GUI.color = Color.gray;
			}
			if (GUILayout.Button(new GUIContent("Reload Info", "Note that this button is available while the Unity Editor is playing a scene that has a CriWareIntializer component."))) {
				ReloadAcbInfo();
			}
			GUI.color = Color.white;
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();
	}

	private void GUIACFSettings()
	{
		this.acfPath = EditorGUILayout.TextField("ACF File Path", this.acfPath, EditorStyles.label);
		//this.dspBusSetting = EditorGUILayout.TextField("DSP Bus Setting", this.dspBusSetting, EditorStyles.label);
		EditorGUILayout.Space();
	}

	private void ReloadAcbInfo()
	{
		if (CriAtomAcfInfo.GetCueInfo(ref acfInfoData, true, searchPath)) {
			this.acfPath = acfInfoData.acfFilePath;
			acfInfoData.GetAcbInfoList(true, searchPath);
		} else {
			if (EditorApplication.isPlaying) {
				Debug.LogError("Not found search file. (created in CRI Atom Craft [acf,acb,awb]) \"" + searchPath + "\"", this);
			} else {
				Debug.LogError("Please push \"play\" button.(Make sure the Scene has \"CriWareLibrayInitializer\".)", this);
			}
		}
	}

	private void GUICueList()
	{
		#region CueSheet
		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.PrefixLabel("Cue Sheet");

			List<string> popupCueSheetNameList = new List<string>();
			var acbInfoList = acfInfoData.GetAcbInfoList(false, searchPath);
			foreach (CriAtomAcfInfo.AcbInfo cueSheetInfo in acbInfoList) {
				popupCueSheetNameList.Add(cueSheetInfo.name);
			}
			if (isCueSheetListInitiated == true) {
				lastPreviewCueSheetId = this.selectedCueSheetId;
			} else {
				isCueSheetListInitiated = true;
			}
			this.selectedCueSheetId = EditorGUILayout.Popup(this.selectedCueSheetId, popupCueSheetNameList.ToArray());
			if (this.selectedCueSheetId != lastPreviewCueSheetId) {
				this.selectedCueInfoIndex = 0;
				this.selectedCueId = acbInfoList[this.selectedCueSheetId].cueInfoList[0].id;
			}
		}
		EditorGUILayout.EndHorizontal();
		#endregion

		GUILayout.BeginHorizontal();
		{
			GUIStyle style = new GUIStyle(EditorStyles.miniButtonMid);
			style.alignment = TextAnchor.LowerLeft;
			if (GUILayout.Button("Cue Name", style)) {
				this.SortCueList(1);
			}
			if (GUILayout.Button("Cue ID", style, GUILayout.Width(70))) {
				this.SortCueList(0);
			}
		}
		GUILayout.EndHorizontal();

		{
			var acbInfoList = acfInfoData.GetAcbInfoList(false, searchPath);
			if (acbInfoList.Length > this.selectedCueSheetId) {
				var acbInfo = acbInfoList[this.selectedCueSheetId];

				if (acbInfo.cueInfoList.Count > 0) {
					float height = this.position.height - 354.0f;
					if (height < 100.0f) height = 100.0f;
					scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(height));
					for(int i = 0; i < acbInfo.cueInfoList.Count; ++i) {
						EditorGUILayout.BeginHorizontal();
						if (this.selectedCueInfoIndex == i) {
							GUI.color = Color.yellow;
						} else {
							GUI.color = Color.white;
						}

						if (GUILayout.Button(acbInfo.cueInfoList[i].name, EditorStyles.radioButton)) {
							this.selectedCueInfoIndex = i;
							this.selectedCueId = acbInfo.cueInfoList[i].id;
						}
						GUILayout.Label(acbInfo.cueInfoList[i].id.ToString(), GUILayout.Width(40));
						EditorGUILayout.EndHorizontal();
					}
					GUI.color = Color.white;
					EditorGUILayout.EndScrollView();
				} else {
					EditorGUILayout.HelpBox("Can not found(CueSheetID:" + this.selectedCueSheetId.ToString() + ")\nDo update display, Please push the play button in Unity Editor.\n and push \"Reload Info\" button.", MessageType.Error);
				}
			}
		}
	}

	private void GUICueInfo()
	{
		#region CueInfo.
		EditorGUILayout.BeginHorizontal("Toolbar");
		EditorGUILayout.LabelField("Selected Cue");
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.LabelField("Cue ID", this.selectedCueId.ToString());
		{
			var acbInfoList = acfInfoData.GetAcbInfoList(false, searchPath);
			if (acbInfoList.Length > this.selectedCueSheetId) {
				var acbInfo = acbInfoList[selectedCueSheetId];
				bool found = false;
				foreach (var key in acbInfoList)
				{
					if (key.id == acbInfo.id) {
						found = true;
						break;
					}
				}
				if (found == true) {
					foreach (var cue in acbInfo.cueInfoList) {
						if (cue.id == this.selectedCueId) {
							EditorGUILayout.LabelField("Cue Name", cue.name);
							EditorGUILayout.LabelField("User Data", cue.comment, EditorStyles.wordWrappedLabel, GUILayout.Height(28));
							EditorGUILayout.Space();
							break;
						}
					}
				} else {
					EditorGUILayout.HelpBox("Can not Get Cue Info!!!(CueID:" + this.selectedCueId.ToString() + ")", MessageType.Error);
				}
			}
		}
		#endregion
	}

	private void GUIEdit()
	{
		//if (this.selectedCueSheetId < acfInfoData.acbInfoList.Count && acfInfoData.acbInfoList[this.selectedCueSheetId].cueInfoList.ContainsKey(this.selectedCueId))
		{
			EditorGUILayout.BeginHorizontal();
			{
				if (GUILayout.Button("Create GameObject", EditorStyles.miniButtonLeft, GUILayout.Height(22))) {
					this.CreateAtomSourceGameObject(true);
				}

				GameObject targetObject = null;
				CriAtomSource atomSource = null;
				if (Selection.gameObjects.Length > 0) {
					targetObject = Selection.gameObjects[0];
					atomSource = targetObject.GetComponent<CriAtomSource>();
				}

				if (targetObject == null) {
					GUI.backgroundColor = Color.gray;
				}
				if (GUILayout.Button("Add Component", EditorStyles.miniButtonMid, GUILayout.Height(22))) {
					if (targetObject != null) {
						if (atomSource != null) {
							if (EditorUtility.DisplayDialog("There are already \"Cri Atom Souce\".", "Are you sure you want to add more?", "Add", "No")) {
								this.CreateAtomSourceGameObject(false);
							}
						} else {
							this.CreateAtomSourceGameObject(false);
						}
					}
				}

				if (atomSource == null) {
					GUI.backgroundColor = Color.gray;
				}
				if (GUILayout.Button("Update Cue Info", EditorStyles.miniButtonRight, GUILayout.Height(22))) {
					if (atomSource != null) {
						var acbInfoList = acfInfoData.GetAcbInfoList(false, searchPath);
						if (acbInfoList.Length > this.selectedCueSheetId) {
							var acbInfo = acbInfoList[this.selectedCueSheetId];
							var cueInfo = acbInfo.cueInfoList[this.selectedCueInfoIndex];
							if (atomSource.cueSheet == acbInfo.name && atomSource.cueName == cueInfo.name) {
								EditorUtility.DisplayDialog("Information", "Is the same configuration.", "OK");
							} else {
								atomSource.cueSheet = acbInfo.name;
								atomSource.cueName = cueInfo.name;
							}
							Selection.activeGameObject = targetObject;
						}
					}
				}
				GUI.backgroundColor = Color.white;

			}
			EditorGUILayout.EndHorizontal();
		}
	}

	private void SortCueList(int type)
	{
		List<CriAtomAcfInfo.CueInfo> cueList = new List<CriAtomAcfInfo.CueInfo>();

		var acbInfoList = acfInfoData.GetAcbInfoList(false, searchPath);
		if (acbInfoList.Length > this.selectedCueSheetId) {
			foreach (CriAtomAcfInfo.CueInfo inf in acbInfoList[selectedCueSheetId].cueInfoList) {
				cueList.Add(inf);
			}
			switch (type) {
			case 0:
				cueList.Sort(delegate (CriAtomAcfInfo.CueInfo x, CriAtomAcfInfo.CueInfo y) {
					return x.id - y.id;
				});
				break;
			default:
				cueList.Sort(delegate (CriAtomAcfInfo.CueInfo x, CriAtomAcfInfo.CueInfo y) {
					return string.Compare(x.name, y.name);
				});
				break;
			} // end of switch
		}
	}

	private void ScalingWindow(int windowID)
	{
		GUILayout.Box("", GUILayout.Width(20), GUILayout.Height(20));
		if (Event.current.type == EventType.MouseUp)
			this.scaling = false;
		else if (Event.current.type == EventType.MouseDown && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
			this.scaling = true;

		if (this.scaling)
			this.windowRect = new Rect(windowRect.x, windowRect.y, windowRect.width + Event.current.delta.x, windowRect.height + Event.current.delta.y);

	}

	private void OnGUI()
	{
		this.windowRect = GUILayout.Window(0, windowRect, ScalingWindow, "resizeable", GUILayout.MinHeight(80), GUILayout.MaxHeight(250));
		if (acfInfoData == null) {
			EditorGUILayout.HelpBox("Do update display,\n1. Please push the \"play\" button in Unity Editor.(Make sure the Scene has \"CriWareLibrayInitializer\".)\n2. And push \"Reload Info\" button.", MessageType.Info);
			GUISearchAndRelaod();
			GUIImportAssetsFromAtomCraft();
			return;
		}
		this.scrollPos_Window = GUILayout.BeginScrollView(this.scrollPos_Window);
		{
			GUISearchAndRelaod();
			GUIACFSettings();
			GUICueList();
			GUICueInfo();
			GUIEdit();
			GUIImportAssetsFromAtomCraft();
		}
		GUILayout.EndScrollView();
	}

	private void GUIImportAssetsFromAtomCraft()
	{
		useCopyAssetsFromCriAtomCraft = GUILayout.Toggle(useCopyAssetsFromCriAtomCraft, "Use Copy Assets Folder (Created in CRI Atom Craft)");
		if (useCopyAssetsFromCriAtomCraft) {
			if (criAtomWindowPrefs == null) {
				criAtomWindowPrefs = CriAtomWindowPrefs.Load();
			}
			GUILayout.Space(12);
			EditorGUILayout.HelpBox("Copy \"Assets\" folder (created in CRI Atom Craft) to the \"Streaming Assets\" folder.\n1. Push \"Select Assets Root\" to select \"CRI Atom Craft\" output Assets Folder.\n2. Push \"Update Assets of \"CRI Atom Craft\"\"."
									, MessageType.Info);
			GUILayout.BeginHorizontal();
			GUILayout.Label("\"CRI Atom Craft\" Assets Path:");

			if (GUILayout.Button("Select Assets Root")) {
				string tmpStr = SelectFolder();
				if (tmpStr != String.Empty) {
					//	Check Assets Root
					bool isUnityAsesetsRoot = false;
					{
						string[] dirs = System.IO.Directory.GetDirectories(tmpStr);
						foreach (string dir in dirs) {
							if (Path.GetFileName(dir) == "Editor") {
								isUnityAsesetsRoot = true;
								break;
							}
						}
					}
					if (isUnityAsesetsRoot == false) {
						criAtomWindowPrefs.outputAssetsRoot = tmpStr;
						criAtomWindowPrefs.Save();
					} else {
						Debug.LogError(String.Format("Choose \"CRI Atom Craft\" output Assets Path"));
					}
				}
			}
			GUILayout.EndHorizontal();

			if (criAtomWindowPrefs != null) {
				criAtomWindowPrefs.outputAssetsRoot = GUILayout.TextArea(criAtomWindowPrefs.outputAssetsRoot);
			}
			//GUILayout.Label(Application.dataPath);

			GUI.color = Color.green;
			if (GUILayout.Button("Update Assets of \"CRI Atom Craft\"")) {
				if (CheckPathIsAtomCraftAssetRoot(criAtomWindowPrefs.outputAssetsRoot) == false) {
					return;
				}
				try{
					CopyDirectory(criAtomWindowPrefs.outputAssetsRoot, Application.dataPath);
					Debug.Log("Complete Update Assets of \"CRI Atom Craft\" " + criAtomWindowPrefs.outputAssetsRoot);
				}
				catch (Exception ex) {
					Debug.LogError(ex.Message);
					Debug.LogError("Failed to update Assets of \"CRI Atom Craft\" " + criAtomWindowPrefs.outputAssetsRoot);
				}
				AssetDatabase.Refresh();

				ReloadAcbInfo();
			}
		}
	}
	private string SelectFolder()
	{
		string outString = String.Empty;

		outString = EditorUtility.OpenFolderPanel("Select \"CRI Atom Craft\" output Assets Folder", outString, criAtomWindowPrefs.outputAssetsRoot);
		if (CheckPathIsAtomCraftAssetRoot(outString) == false) {
			outString = String.Empty;
		}
		return outString;
	}

	private bool CheckPathIsAtomCraftAssetRoot(string outString)
	{
		if (outString != String.Empty && (System.IO.Path.GetFileName(outString) == "Assets")) {
			Debug.Log(String.Format("Change \"CRI Atom Craft\" output Assets \"{0}\"", outString));
			return true;
		} else {
			Debug.LogError(String.Format("Can not Change \"CRI Atom Craft\" output Assets \"{0}\".Please select a \"Assets\" Folder.", outString));
			return false;
		}
	}

	private static void CopyDirectory(string sourceDirName, string destDirName)
	{
		if (!System.IO.Directory.Exists(destDirName)) {
			System.IO.Directory.CreateDirectory(destDirName);
			System.IO.File.SetAttributes(destDirName,
				System.IO.File.GetAttributes(sourceDirName));
		}
		if (destDirName[destDirName.Length - 1] != System.IO.Path.DirectorySeparatorChar)
			destDirName = destDirName + System.IO.Path.DirectorySeparatorChar;

		string[] files = System.IO.Directory.GetFiles(sourceDirName);
		foreach (string file in files) {
			if (System.IO.Path.GetExtension(file.Replace("\\", "/")) == ".acf" ||
				System.IO.Path.GetExtension(file.Replace("\\", "/")) == ".acb" ||
				System.IO.Path.GetExtension(file.Replace("\\", "/")) == ".awb"
                ) {
				Debug.Log(String.Format("Copy \"{0}\" to \"{1}\"", file, destDirName + System.IO.Path.GetFileName(file)));
				System.IO.File.Copy(file,
					destDirName + System.IO.Path.GetFileName(file), true);
			}
		}
		string[] dirs = System.IO.Directory.GetDirectories(sourceDirName);
		foreach (string dir in dirs)
			CopyDirectory(dir, destDirName + System.IO.Path.GetFileName(dir));
	}

	static string ToBase64(string s)
	{
		return System.Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(s));
	}

	static string FromBase64(string s)
	{
		return UTF8Encoding.UTF8.GetString(System.Convert.FromBase64String(s));
	}

	private void CreateAtomSourceGameObject(bool createGameObjectFlag)
	{
		if (Selection.gameObjects.Length == 0) {
			createGameObjectFlag = true;
		}
		var acbInfoList = acfInfoData.GetAcbInfoList(false, searchPath);
		if (acbInfoList.Length > this.selectedCueSheetId) {
			GameObject go = null;
			if (createGameObjectFlag) {
				go = new GameObject(acbInfoList[this.selectedCueSheetId].cueInfoList[this.selectedCueInfoIndex].name + "(CriAtomSource)");
				if (Selection.gameObjects.Length > 0) {
					go.transform.parent = Selection.gameObjects[0].transform;
				}
			} else {
				go = Selection.gameObjects[0];
			}
			var acbInfo = acbInfoList[this.selectedCueSheetId];
			CriAtom atom = GameObject.FindObjectOfType(typeof(CriAtom)) as CriAtom;
			if (atom == null) {
				atom = CriWare.managerObject.AddComponent<CriAtom>();
				atom.acfFile = acfInfoData.acfPath;
			}
			CriAtomCueSheet cueSheet = atom.GetCueSheetInternal(acbInfo.name);
			if (cueSheet == null) {
				cueSheet = atom.AddCueSheetInternal(null, acbInfo.acbPath, acbInfo.awbPath, null);
			}
			CriAtomSource newCriAtomSource = go.AddComponent<CriAtomSource>();
			newCriAtomSource.cueSheet = cueSheet.name;
			newCriAtomSource.cueName = acbInfo.cueInfoList[this.selectedCueInfoIndex].name;
			Selection.activeObject = go;
			//Debug.Log("Add \"CRI Atom Souce\" \"" + newCriAtomSource.AcbName + "/" + newCriAtomSource.CueName + "\"");
		}
	}
	#endregion
} // end of class

/* end of file */
