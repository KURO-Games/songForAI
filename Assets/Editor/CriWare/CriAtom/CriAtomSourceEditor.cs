/****************************************************************************
 *
 * Copyright (c) 2014 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

#define CRI_UNITY_EDITOR_PREVIEW

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

[CustomEditor(typeof(CriAtomSource))]
public class CriAtomSourceEditor : Editor
{
	#region Variables
	private CriAtomSource source = null;
	private bool showAndroidConfig;
	private GUIStyle style;

#if CRI_UNITY_EDITOR_PREVIEW
	private bool isPreviewReady = false;
	private CriAtomExAcb previewAcb;
	private CriAtomExPlayer previewPlayer;
	public CriAtom atomComponent;
	private string strPreviewAcb = null;
	private string strPreviewAwb = null;
	private string lastCuesheet = "";
#endif
	#endregion

	#region Functions
	private void OnEnable()	{
		this.source = (CriAtomSource)base.target;
		this.style = new GUIStyle();

#if CRI_UNITY_EDITOR_PREVIEW
		/* シーンからCriAtomコンポーネントを見つけ出す */
		atomComponent = (CriAtom)FindObjectOfType(typeof(CriAtom));
#endif
	}

	private void OnDisable() {
#if CRI_UNITY_EDITOR_PREVIEW
		if (previewAcb != null) {
			previewAcb.Dispose();
			previewAcb = null;
		}
		if (previewPlayer != null) {
			previewPlayer.Dispose();
			previewPlayer = null;
		}
#endif
	}

#if CRI_UNITY_EDITOR_PREVIEW
	/* プレビュ用：初期化関数 */
	private void PreparePreview() {
		if (CriAtomPlugin.IsLibraryInitialized() == false) {
			CriWareInitializer.InitializeAtom(new CriAtomConfig());
		}
		if (CriAtomPlugin.IsLibraryInitialized() == false) {
			return;
		}
		
		previewPlayer = new CriAtomExPlayer();
		if (previewPlayer == null) {
			return;
		}

		if (atomComponent != null) {
			CriAtomEx.RegisterAcf(null, Path.Combine(CriWare.streamingAssetsPath, atomComponent.acfFile));
		} else {
			Debug.LogWarning("[CRIWARE] CriAtom component not found in this scene");
			return;
		}

		isPreviewReady = true;
	}

	/* プレビュ用：音声データ設定・再生関数 */
	private void StartPreviewPlayer() {
		if (isPreviewReady == false) {
			PreparePreview();
		}
		if (isPreviewReady == true) {
			if (previewAcb == null || lastCuesheet != this.source.cueSheet) {
				if (previewAcb != null) {
					previewAcb.Dispose();
					previewAcb = null;
				}
				foreach (var cuesheet in atomComponent.cueSheets) {
					if (cuesheet.name == this.source.cueSheet) {
						strPreviewAcb = Path.Combine(CriWare.streamingAssetsPath, cuesheet.acbFile);
						strPreviewAwb = (cuesheet.awbFile == null) ? null : Path.Combine(CriWare.streamingAssetsPath, cuesheet.awbFile);
						previewAcb = CriAtomExAcb.LoadAcbFile(null, strPreviewAcb, strPreviewAwb);
						lastCuesheet = cuesheet.name;
					}
				}
			}
			if (previewAcb != null) {
				if (previewPlayer != null) {
					previewPlayer.SetCue(previewAcb, this.source.cueName);
					previewPlayer.SetVolume(this.source.volume);
					previewPlayer.SetPitch(this.source.pitch);
					previewPlayer.Loop(this.source.loop);
					previewPlayer.Start();
				} else {
					Debug.LogWarning("[CRIWARE] Player is not ready. Please try reloading the inspector");
				}
			} else {
				Debug.LogWarning("[CRIWARE] Specified cue sheet could not be found");
			}
		}
	}

	/* プレビュ用：再生停止関数 */
	private void StopPreviewPlayer() {
		if (previewPlayer != null) {
			previewPlayer.Stop();
		}
	}
#endif

	public override void OnInspectorGUI() {
		if (this.source == null) {
			return;
		}

		Undo.RecordObject(target, null);

		GUI.changed = false;
		{
			EditorGUI.indentLevel++;
			this.source.cueSheet = EditorGUILayout.TextField("Cue Sheet", this.source.cueSheet);
			this.source.cueName = EditorGUILayout.TextField("Cue Name", this.source.cueName);
#if CRI_UNITY_EDITOR_PREVIEW
			GUI.enabled = false;
			atomComponent = (CriAtom)EditorGUILayout.ObjectField("CriAtom Object", atomComponent, typeof(CriAtom), true);
			GUI.enabled = (atomComponent != null);
			GUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("Preview", GUILayout.MaxWidth(EditorGUIUtility.labelWidth - 5));
				if (GUILayout.Button("Play", GUILayout.MaxWidth(60))) {
					StartPreviewPlayer();
				}
				if (GUILayout.Button("Stop", GUILayout.MaxWidth(60))) {
					StopPreviewPlayer();
				}
			}
			GUILayout.EndHorizontal();
			GUI.enabled = true;
#endif
			this.source.playOnStart = EditorGUILayout.Toggle("Play On Start", this.source.playOnStart);
			EditorGUILayout.Space();
			this.source.volume = EditorGUILayout.Slider("Volume", this.source.volume, 0.0f, 1.0f);
			this.source.pitch = EditorGUILayout.Slider("Pitch", this.source.pitch, -1200f, 1200);
			this.source.loop = EditorGUILayout.Toggle("Loop", this.source.loop);
			this.source.use3dPositioning = EditorGUILayout.Toggle("3D Positioning", this.source.use3dPositioning);

			this.showAndroidConfig = EditorGUILayout.Foldout(this.showAndroidConfig, "Android Config");
			if (this.showAndroidConfig) {
				EditorGUI.indentLevel++;
				EditorGUILayout.BeginHorizontal();
				style.stretchWidth = true;
				this.source.androidUseLowLatencyVoicePool = EditorGUILayout.Toggle("Low Latency Playback", this.source.androidUseLowLatencyVoicePool);
				EditorGUILayout.EndHorizontal();
				EditorGUI.indentLevel--;
			}
		}
		if (GUI.changed) {
			EditorUtility.SetDirty(this.source);
		}
	}
	#endregion
} // end of class

/* end of file */
