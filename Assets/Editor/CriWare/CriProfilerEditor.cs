/****************************************************************************
 *
 * Copyright (c) 2018 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

using System;
using System.Text;
using UnityEngine;
using UnityEditor;

public sealed class CriProfilerEditor : EditorWindow
{
	private static CriProfiler profiler = CriProfiler.GetSingleton();

	#region Unity Event Callbacks

	[MenuItem ("Window/CRIWARE/CRI Profiler")]
	static void InitWindow()
	{
		CriProfilerEditor window = (CriProfilerEditor)EditorWindow.GetWindow(typeof(CriProfilerEditor));
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_4_OR_NEWER
		window.titleContent = new GUIContent("CRI Profiler");
#else
		window.title = "CRI Profiler";
#endif
		window.Show();
	}
	
	private struct PlaymodeState {
		public readonly bool isPlaying;
		public readonly bool isPaused;
		public readonly bool isPlayingOrWillChangePlaymode;
		public PlaymodeState(bool isPlaying, bool isPaused, bool isPlayingOrWillChangePlaymode)
		{
			this.isPlaying = isPlaying;
			this.isPaused = isPaused;
			this.isPlayingOrWillChangePlaymode = isPlayingOrWillChangePlaymode;
		}
	}
	private PlaymodeState prevPlaymodeState = new PlaymodeState(false, false, false);
	private void OnStateChanged()
	{
		if(EditorApplication.isPlaying == EditorApplication.isPlayingOrWillChangePlaymode) {
			InitResources();
		}
		if (EditorApplication.isPlayingOrWillChangePlaymode == false) {
			profiler.StopProfiling();
		} else if (startWithUnityPlayer == true && EditorApplication.isPlaying == true && prevPlaymodeState.isPlaying == false) {
			this.remoteProfiling = false;
			profiler.ipAddressString = defaultIpAddr;
			profiler.StartProfiling();
		}
		if (this.pauseWithUnityPlayer == true) {
			this.isWindowPaused = EditorApplication.isPaused;
		}
		prevPlaymodeState = new PlaymodeState(EditorApplication.isPlaying, EditorApplication.isPaused, EditorApplication.isPlayingOrWillChangePlaymode);
	}

#if UNITY_2017_2_OR_NEWER
	private void OnPlayModeStateChanged(PlayModeStateChange state)
	{
		OnStateChanged();
	}
	private void OnPauseStateChanged(PauseState state)
	{
		OnStateChanged();
	}
#endif

#if UNITY_5_6_OR_NEWER
	private void OnSceneOpened(UnityEngine.SceneManagement.Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
	{
		InitResources();
	}
#endif

	private void OnEnable()
	{
#if UNITY_2017_2_OR_NEWER
		UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		UnityEditor.EditorApplication.pauseStateChanged += OnPauseStateChanged;
#else
		UnityEditor.EditorApplication.playmodeStateChanged += OnStateChanged;
#endif
#if UNITY_5_6_OR_NEWER
		UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += OnSceneOpened;
#endif

		InitResources();
	}

	private void OnDisable()
	{
		if (profiler != null) {
			profiler.StopProfiling();
		}
		DestroyResources();

#if UNITY_2017_2_OR_NEWER
		UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
		UnityEditor.EditorApplication.pauseStateChanged -= OnPauseStateChanged;
#else
		UnityEditor.EditorApplication.playmodeStateChanged -= OnStateChanged;
#endif
#if UNITY_5_6_OR_NEWER
		UnityEditor.SceneManagement.EditorSceneManager.sceneOpened -= OnSceneOpened;
#endif
	}

	#endregion Unity Event Callbacks

	#region Data Slice Definition

	private struct DataSlicePerformance {
		public readonly Single cpuLoad;
		public readonly Single[] cpuLoadHistory;
		public readonly UInt32 maxVoices;
		public readonly UInt32 usedVoices;
		public readonly UInt32[] voiceUsageHistory;
		public readonly UInt32 maxStandardVoices;
		public readonly int usedStandardVoices;
		public readonly UInt32 maxHcaMxVoices;
		public readonly int usedHcaMxVoices;
		public readonly UInt32 usedStreams;
		public readonly Single totalBps;
		public readonly string cuenameLastPlayed;
		public DataSlicePerformance(CriProfiler profiler)
		{
			this.cpuLoad = profiler.cpu_load;
			this.cpuLoadHistory = profiler.CpuLoadHistory;
			this.maxVoices =
				profiler.maxVoice_stdStreaming +
				profiler.maxVoice_hcamxStreaming +
				profiler.maxVoice_stdOnMemory +
				profiler.maxVoice_hcamxOnMemory;
			this.usedVoices = profiler.num_used_voices;
			this.voiceUsageHistory = profiler.VoiceUsageHistory;
			this.maxStandardVoices = profiler.maxVoice_stdOnMemory + profiler.maxVoice_stdStreaming;
			this.usedStandardVoices = profiler.GetVoicePoolUsage(CriProfiler.VoicePoolFormat.Standard);
			this.maxHcaMxVoices = profiler.maxVoice_hcamxOnMemory + profiler.maxVoice_hcamxStreaming;
			this.usedHcaMxVoices = profiler.GetVoicePoolUsage(CriProfiler.VoicePoolFormat.HcaMx);
			this.usedStreams = profiler.num_used_streams;
			this.totalBps = profiler.total_bps;
			this.cuenameLastPlayed = profiler.cuename_lastPlayed;
		}
	}

	private struct DataSlicePlayback {
		public readonly CriProfiler.PlaybackInfo[] playbackList;
		public readonly StringBuilder playbackListString;
		public DataSlicePlayback(CriProfiler profiler)
		{
			this.playbackList = profiler.PlaybackList;
			this.playbackListString = new StringBuilder();
			foreach (var elem in playbackList) {
				playbackListString.Append(elem.cueName);
				playbackListString.Append(' ');
			}
		}
	}
	private struct DataSliceLoudness {
		public readonly Single momentary;
		public readonly Single shortTerm;
		public readonly Single integrated;
		public DataSliceLoudness(CriProfiler profiler)
		{
			this.momentary = profiler.loudness_momentary;
			this.shortTerm = profiler.loudness_shortTerm;
			this.integrated = profiler.loudness_integrated;
		}
	}
	private struct DataSliceLevels {
		public readonly CriProfiler.LevelInfo[] levels;
		public readonly int outputCh;
		public DataSliceLevels(CriProfiler profiler)
		{
			this.levels = profiler.LevelsAllCh;
			this.outputCh = profiler.numChMasterOut;
		}
	}

	private struct DataSliceLocalRuntime {
		public readonly uint memUsageAtom;
		public readonly uint memUsageFs;
		public DataSliceLocalRuntime(bool isPlaying)
		{
			if (isPlaying == true) {
                memUsageAtom = CriAtomPlugin.IsLibraryInitialized() ? CriWare.GetAtomMemoryUsage() : 0;
                memUsageFs = CriFsPlugin.IsLibraryInitialized() ? CriWare.GetFsMemoryUsage() : 0;
			} else {
				memUsageAtom = 0;
				memUsageFs = 0;
			}
		}
	}

	#endregion Data Slice Definition

	#region Internal Fields

	/* configurations */
	private bool startWithUnityPlayer = true;
	private bool pauseWithUnityPlayer = true;
	private bool remoteProfiling = false;
	private bool isWindowPaused = false;
	private const Single DB_MIN = -96.0f;
	private const Single DB_MAX = 20.0f;
	private const Single DEFAULT_DB_FLOOR = -48.0f;
	private const Single DEFAULT_DB_CEILING = 0;
	private const Single DEFAULT_DB_TARGET = -24.0f;
	private const Single DB_RANGE_PLUSMINUS_MAX = 20.0f;
	private const Single DB_RANGE_PLUSMINUS_MIN = 0;
	private const Single DEFAULT_DB_RANGE_PLUSMINUS = 2.0f;
	private const Single DEFAULT_DB_WARNING = -6.0f;
	private Single currentDbFloor = DEFAULT_DB_FLOOR;
	private Single currentDbCeiling = DEFAULT_DB_CEILING;
	private Single currentDbTarget = DEFAULT_DB_TARGET;
	private Single currentDbRangePlusMinus = DEFAULT_DB_RANGE_PLUSMINUS;
	private Single currentDbWarning = DEFAULT_DB_WARNING;
	private const Single DEFAULT_LEVEL_FLOOR = -48.0f;
	private const Single DEFAULT_LEVEL_CEILING = 0;
	private const Single DEFAULT_LEVEL_WARNING = -6.0f;
	private const int DEFAULT_LEVEL_CAL_INTVL = 6;
	private const int DEFAULT_LEVEL_CAL_INTVL_MIN = 1;
	private const int DEFAULT_LEVEL_CAL_INTVL_MAX = 24;
	private const MultiChannelMeterType DEFAULT_LEVEL_TYPE = MultiChannelMeterType.Peak;
	private Single currentLevelFloor = DEFAULT_LEVEL_FLOOR;
	private Single currentLevelCeiling = DEFAULT_LEVEL_CEILING;
	private Single currentLevelWarning = DEFAULT_LEVEL_WARNING;
	private int currentLevelCalInterval = DEFAULT_LEVEL_CAL_INTVL;
	private MultiChannelMeterType currentLevelType = DEFAULT_LEVEL_TYPE;
	private const string defaultIpAddr = "127.0.0.1";

	/* GUI parameters */
	private const float LABEL_WIDTH = 148.0f;
	private const float CHART_INDENT_WIDTH = 15.0f;
	private const float GUI_INDENT_WIDTH = 19.0f;
	private const int TEXT_SIZE_CALIBRATOR = 10;
	private const int TEXT_SIZE_LABEL = 11;
	private const int TEXT_SIZE_CUE_LIST = 12;
	private const int TEXT_SIZE_GIANT = 20;
	private Vector2 textOffsetChartTitle = new Vector2(3.0f, 2.0f);
	private Vector2 textOffsetCalibrator = new Vector2(-4.0f, -6.0f);
	private UnityEngine.Color colorWarningHigh = new Color(224 / 255.0f, 44 / 255.0f, 44 / 255.0f);
	private UnityEngine.Color colorWarningHighTolerant = new Color(244 / 255.0f, 170 / 255.0f, 44 / 255.0f);
	private UnityEngine.Color colorMidRange = new Color(107 / 255.0f, 224 / 255.0f, 44 / 255.0f);
	private UnityEngine.Color colorWarningLow = new Color(44 / 255.0f, 107 / 255.0f, 224 / 255.0f);
	private UnityEngine.Color colorGrid = new Color(0.2f, 0.2f, 0.2f);

	/* GUI variables */
	private Vector2 scrollViewPosition;
	private Vector2 textareaScrollViewPostion;
	private bool foldTogglerConfig = true;
	private bool foldTogglerExperimental = false;
	private bool foldTogglerPerformance = true;
	private bool foldTogglerPlayingList = true;
	private bool foldTogglerLoudness = true;
	private bool foldTogglerLoudnessAppearance = false;
	private bool foldTogglerLevels = true;
	private bool foldTogglerLevelAppearance = false;
	private GUIStyle barBorderStyle;
	private GUIStyle barBackgroundStyle;
	private GUIStyle barStyle;
	private GUIStyle barHolderStyle;
	private GUIStyle barWarningHighStyle;
	private GUIStyle barWarningHighTolerantStyle;
	private GUIStyle barMidRangeStyle;
	private GUIStyle barWarningLowStyle;
	private GUIStyle barTextStyle;
	private GUIStyle barTextShadowStyle;
	private GUIStyle meterTextWarningHighStyle;
	private GUIStyle meterTextWarningHighTolerantStyle;
	private GUIStyle meterTextMidRangeStyle;
	private GUIStyle meterTextWarningLowStyle;
	private GUIStyle multiChMeterTextStyle;
	private GUIStyle cueListStyle;
	private GUIStyle scrollViewStyle;
	private GUIStyle chartLabelStyle;
	private Material mat;
	private StringBuilder gStrBuilder = new StringBuilder();
	private StringBuilder gStrBuilderSub = new StringBuilder();
	
	/* Data slices */
	private DataSlicePerformance perfData = new DataSlicePerformance(profiler);
	private DataSlicePlayback playbackData = new DataSlicePlayback(profiler);
	private DataSliceLoudness loudnessData = new DataSliceLoudness(profiler);
	private DataSliceLevels levelsData = new DataSliceLevels(profiler);
	private DataSliceLocalRuntime runtimeData = new DataSliceLocalRuntime(false);

	private void InitResources()
	{
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_4_OR_NEWER
		var shader = Shader.Find("Hidden/Internal-Colored");
#else
        var shader = Shader.Find("Sprites/Default");
#endif
		if(mat == null) {
			mat = new Material(shader);
		}
		if (gStrBuilder == null) {
			gStrBuilder = new StringBuilder(200); /* for labels only */
		}
		if (gStrBuilderSub == null) {
			gStrBuilderSub = new StringBuilder(200); /* for subroutine string makers only */
		}

		InitializeStyleWithBgColor(ref barBorderStyle, Color.gray);
		InitializeStyleWithBgColor(ref barBackgroundStyle, Color.black);
		InitializeStyleWithBgColor(ref barStyle, Color.gray);
		InitializeStyleWithBgColor(ref barHolderStyle, Color.yellow);
		InitializeStyleWithBgColor(ref barWarningHighStyle, colorWarningHigh);
		InitializeStyleWithBgColor(ref barWarningHighTolerantStyle, colorWarningHighTolerant);
		InitializeStyleWithBgColor(ref barMidRangeStyle, colorMidRange);
		InitializeStyleWithBgColor(ref barWarningLowStyle, colorWarningLow);

		InitializeTextStyle(ref barTextStyle, TEXT_SIZE_LABEL, Color.white, TextAnchor.UpperLeft);
		InitializeTextStyle(ref barTextShadowStyle, TEXT_SIZE_LABEL, Color.black, TextAnchor.UpperLeft);
		InitializeTextStyle(ref multiChMeterTextStyle, TEXT_SIZE_LABEL, Color.white, TextAnchor.MiddleCenter);
		InitializeTextStyle(ref chartLabelStyle, TEXT_SIZE_CALIBRATOR, Color.gray, TextAnchor.UpperRight);

		InitializeTextStyle(ref cueListStyle, TEXT_SIZE_CUE_LIST, Color.white, TextAnchor.UpperLeft);
		cueListStyle.fontStyle = FontStyle.Bold;
		cueListStyle.wordWrap = true;

		InitializeTextStyle(ref meterTextWarningHighStyle, TEXT_SIZE_GIANT, colorWarningHigh, TextAnchor.MiddleCenter);
		InitializeTextStyle(ref meterTextWarningHighTolerantStyle, TEXT_SIZE_GIANT, colorWarningHighTolerant, TextAnchor.MiddleCenter);
		InitializeTextStyle(ref meterTextMidRangeStyle, TEXT_SIZE_GIANT, colorMidRange, TextAnchor.MiddleCenter);
		InitializeTextStyle(ref meterTextWarningLowStyle, TEXT_SIZE_GIANT, colorWarningLow, TextAnchor.MiddleCenter);

		if (scrollViewStyle == null) {
			scrollViewStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Game).box);
		}
	}

	private void InitializeStyle(ref GUIStyle style)
	{
		if (style == null) {
			style = new GUIStyle();
		}
	}

	private void InitializeStyleWithBgColor(ref GUIStyle style, Color color)
	{
		InitializeStyle(ref style);
		if (style.normal.background == null) {
			style.normal.background = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		}
		style.normal.background.SetPixel(1, 1, color);
		style.normal.background.Apply();
	}

	private void InitializeTextStyle(ref GUIStyle style, int size, Color color, TextAnchor anchor)
	{
		InitializeStyle(ref style);
		style.normal.textColor = color;
		style.fontSize = size;
		style.alignment = anchor;
	}

	private void DestroyResources()
	{
		DestroyImmediate(mat);
	}

	#endregion Internal Fields

	#region Const strings on GUI
	/* common */
	private const string S_SPACE = " ";
	private const string S_DIVIDER = " / ";
	private const string S_NAN = "-";
	private const string S_BELOW_MIN = "---";
	private const string S_FLOAT2FORMAT = "{0:F2}";
	private const string S_FLOAT1FORMAT = "{0:F1}";
	private const string S_FLOAT0FORMAT = "{0:F0}";
	private const string S_FLOAT0 = "F0";
	private const string S_PERCENTAGE = "%";
	private const string S_LKFS = "LKFS";
	private const string S_DBFS = "dBFS";
	private const string S_MIN = "Min: ";
	private const string S_MAX = "Max: ";
	private const string S_MBYTE = " MB";
	private const string S_KBYTE = " KB";
	private const string S_BYTE = " Byte";
	/* specified */
	private const string S_START = "Start";
	private const string S_STOP = "Stop";
	private const string S_PAUSE = "Pause";
	private const string S_RESUME = "Resume";
	private const string S_FOLD_CONFIGURATION = "Configuration";
	private const string S_STARTWITHPLAYER = "Start with Player";
	private const string S_PAUSEWITHPLAYER = "Pause with Player";
	private const string S_EXPERIMENTAL = "Experimental";
	private const string S_REMOTEPROFILING = "Remote Profiling";
	private const string S_IPADDRESS = "IP Address";
	private const string S_FOLD_PERFORMANCE = "Performance";
	private const string S_CPUUSAGE = "CPU Usage: ";
	private const string S_NUMBEROFVOICES = "Number of Voices: ";
	private const string S_ATOMMEMUSAGE = "Atom Memory Usage";
	private const string S_FSMEMUSAGE = "FS Memory Usage";
	private const string S_STANDARDPOOLUSAGE = "Standard Pool Usage";
	private const string S_HCAMXPOOLUSAGE = "HCA-MX Pool Usage";
	private const string S_STREAMINGUSAGE = "Streaming Usage";
	private const string S_VOICES_BRACE = " voices (";
	private const string S_MBPS_BRACE = " Mbps)";
	private const string S_LASTPLAYED = "Last Played";
	private const string S_BRACE = "[ ";
	private const string S_COUNTERBRACE = " ]";
	private const string S_FOLD_PLAYING = "Playing";
	private const string S_FOLD_LOUDNESS = "Loudness";
	private const string S_MOMENTARY = "Momentary ";
	private const string S_SHORTTERM = "Short Term ";
	private const string S_INTEGRATED = "Integrated ";
	private const string S_FOLD_APPEARANCE = "Appearance";
	private GUIContent GC_LEVELRANGE = new GUIContent("Disp. Range (dB)");
	private const string S_TARGET = "Target (dB)";
	private const string S_RANGE = "Tgt. Range (+/-dB)";
	private const string S_WARNING = "Warning (dB)";
	private const string S_GRID_INTVL = "Grid Interval (dB)";
	private const string S_RESETDBRANGES = "Reset";
	private const string S_FOLD_LEVELS = "Peak / RMS";
	private const string S_CHANNEL = "CH";
	private const string S_PEAK = "Peak";
	private const string S_RMS = "RMS";
	private string[] S_MultiChMeterButton = { S_PEAK, S_RMS };
	#endregion Const strings on GUI

	#region OnGUI

	private void OnGUI()
	{
		GUILayoutOption buttonHeightSetting = GUILayout.Height(25);
		scrollViewPosition = GUILayout.BeginScrollView(scrollViewPosition);
		{
			GUILayout.BeginHorizontal();
			{
				GUI.enabled = (EditorApplication.isPlaying || remoteProfiling) && (profiler.IsProfiling == false);
				if (GUILayout.Button(S_START, buttonHeightSetting)) {
					profiler.StartProfiling();
					this.isWindowPaused = false;
				}
				GUI.enabled = (profiler.IsProfiling == true);
				if (GUILayout.Button(S_STOP, buttonHeightSetting)) {
					profiler.StopProfiling();
				}
				if (this.isWindowPaused == false) {
					if (GUILayout.Button(S_PAUSE, buttonHeightSetting)) {
						this.isWindowPaused = true;
					}
				} else {
					if (GUILayout.Button(S_RESUME, buttonHeightSetting)) {
						this.isWindowPaused = false;
					}
				}
				GUI.enabled = true;
			}
			GUILayout.EndHorizontal();

			DrawConfigurationInfo();
			DrawrPerformanceInfo();
			DrawrPlaybackInfo();
			DrawLoudnessInfo();
			DrawLevelsInfo();
			GUILayout.Space(20.0f);
		}
		GUILayout.EndScrollView();

		this.Repaint();
	}

	private void DrawConfigurationInfo()
	{
		foldTogglerConfig = CriFoldout(foldTogglerConfig, S_FOLD_CONFIGURATION);
		if (foldTogglerConfig) {
			EditorGUI.indentLevel++;
			startWithUnityPlayer = EditorGUILayout.Toggle(S_STARTWITHPLAYER, startWithUnityPlayer);
			pauseWithUnityPlayer = EditorGUILayout.Toggle(S_PAUSEWITHPLAYER, pauseWithUnityPlayer);
			foldTogglerExperimental = CriFoldout(foldTogglerExperimental, S_EXPERIMENTAL);
			if (foldTogglerExperimental) {
				EditorGUI.indentLevel++;
				remoteProfiling = EditorGUILayout.Toggle(S_REMOTEPROFILING, remoteProfiling);
				if (remoteProfiling == false) {
					profiler.ipAddressString = defaultIpAddr;
				}
				GUI.enabled = remoteProfiling;
				profiler.ipAddressString = EditorGUILayout.TextField(S_IPADDRESS, profiler.ipAddressString);
				GUI.enabled = true;
				EditorGUI.indentLevel--;
			}
			EditorGUI.indentLevel--;
		}
	}

	private void DrawrPerformanceInfo()
	{
		foldTogglerPerformance = CriFoldout(foldTogglerPerformance, S_FOLD_PERFORMANCE);
		if (foldTogglerPerformance) {
			if (isWindowPaused == false) {
				perfData = new DataSlicePerformance(profiler);
				if (this.remoteProfiling == false) {
					runtimeData = new DataSliceLocalRuntime(EditorApplication.isPlaying);
				}
			}
			EditorGUI.indentLevel++;
			Rect chartRect = EditorGUILayout.GetControlRect(GUILayout.Height(80));
			chartRect.xMin += CHART_INDENT_WIDTH;
			gStrBuilder.Length = 0;
			LineGraph(chartRect, gStrBuilder.Append(S_CPUUSAGE).AppendFormat(S_FLOAT2FORMAT, perfData.cpuLoad).Append(S_PERCENTAGE).ToString(), perfData.cpuLoadHistory, 100.0f, 5, 5.0f, true);
			chartRect = EditorGUILayout.GetControlRect(GUILayout.Height(80));
			chartRect.xMin += CHART_INDENT_WIDTH;
			gStrBuilder.Length = 0;
			LineGraph(chartRect, gStrBuilder.Append(S_NUMBEROFVOICES).Append(perfData.usedVoices).Append(S_DIVIDER).Append(perfData.maxVoices).ToString(), Array.ConvertAll(perfData.voiceUsageHistory, x => (float)x), perfData.maxVoices, 5, 4.0f, false);
			GUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField(S_STANDARDPOOLUSAGE, GUILayout.Width(LABEL_WIDTH));
				gStrBuilder.Length = 0;
				SimpleBarGraph(EditorGUILayout.GetControlRect(), perfData.usedStandardVoices, 0, perfData.maxStandardVoices,
					gStrBuilder.Append(perfData.usedStandardVoices >= 0 ? perfData.usedStandardVoices.ToString() : S_NAN)
					.Append(S_DIVIDER)
					.Append(perfData.maxStandardVoices > 0 ? perfData.maxStandardVoices.ToString() : S_NAN)
					.ToString());
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField(S_HCAMXPOOLUSAGE, GUILayout.Width(LABEL_WIDTH));
				gStrBuilder.Length = 0;
				SimpleBarGraph(EditorGUILayout.GetControlRect(), perfData.usedHcaMxVoices, 0, perfData.maxHcaMxVoices,
					gStrBuilder.Append(perfData.usedHcaMxVoices >= 0 ? perfData.usedHcaMxVoices.ToString() : S_NAN)
					.Append(S_DIVIDER)
					.Append(perfData.maxHcaMxVoices > 0 ? perfData.maxHcaMxVoices.ToString() : S_NAN)
					.ToString());
			}
			GUILayout.EndHorizontal();
			if (this.remoteProfiling == false) {
				EditorGUILayout.LabelField(S_ATOMMEMUSAGE, DataSize2String(runtimeData.memUsageAtom));
				EditorGUILayout.LabelField(S_FSMEMUSAGE, DataSize2String(runtimeData.memUsageFs));
			}
			gStrBuilder.Length = 0;
			EditorGUILayout.LabelField(S_STREAMINGUSAGE, gStrBuilder.Append(perfData.usedStreams).Append(S_VOICES_BRACE).AppendFormat(S_FLOAT2FORMAT, perfData.totalBps / 1000000).Append(S_MBPS_BRACE).ToString());
			gStrBuilder.Length = 0;
			EditorGUILayout.LabelField(S_LASTPLAYED, gStrBuilder.Append(S_BRACE).Append(perfData.cuenameLastPlayed).Append(S_COUNTERBRACE).ToString());
			EditorGUI.indentLevel--;
		}
	}

	private void DrawrPlaybackInfo()
	{
		foldTogglerPlayingList = CriFoldout(foldTogglerPlayingList, S_FOLD_PLAYING);
		if (foldTogglerPlayingList) {
			if (isWindowPaused == false) {
				playbackData = new DataSlicePlayback(profiler);
			}
			GUILayout.BeginHorizontal();
			{
				GUILayout.Space(GUI_INDENT_WIDTH);
				textareaScrollViewPostion = GUILayout.BeginScrollView(new Vector2(textareaScrollViewPostion.x, Mathf.Infinity), scrollViewStyle, GUILayout.Height(60));
				GUILayout.Label(playbackData.playbackListString.ToString(), cueListStyle, GUILayout.ExpandHeight(true));
				GUILayout.EndScrollView();
			}
			GUILayout.EndHorizontal();
		}
	}

	private void DrawLoudnessInfo()
	{
		foldTogglerLoudness = CriFoldout(foldTogglerLoudness, S_FOLD_LOUDNESS);
		if (foldTogglerLoudness) {
			if (isWindowPaused == false) {
				loudnessData = new DataSliceLoudness(profiler);
			}
			Rect chartRect;
			GUILayoutOption[] numberRectGUISettings = { GUILayout.Height(60), GUILayout.MinWidth(60) };
			GUILayoutOption barHeightSetting = GUILayout.Height(15);
			const float paddingOffset = 2.0f;
			GUILayout.BeginHorizontal();
			{
				chartRect = EditorGUILayout.GetControlRect(numberRectGUISettings);
				chartRect.x += CHART_INDENT_WIDTH;
				gStrBuilder.Length = 0;
				NumericMeter(chartRect, gStrBuilder.Append(S_MOMENTARY).Append(S_SPACE).Append(S_LKFS).ToString(), loudnessData.momentary);
				chartRect = EditorGUILayout.GetControlRect(numberRectGUISettings);
				chartRect.x += CHART_INDENT_WIDTH - paddingOffset;
				gStrBuilder.Length = 0;
				NumericMeter(chartRect, gStrBuilder.Append(S_SHORTTERM).Append(S_SPACE).Append(S_LKFS).ToString(), loudnessData.shortTerm);
				chartRect = EditorGUILayout.GetControlRect(numberRectGUISettings);
				chartRect.xMin += CHART_INDENT_WIDTH - paddingOffset * 2;
				gStrBuilder.Length = 0;
				NumericMeter(chartRect, gStrBuilder.Append(S_INTEGRATED).Append(S_SPACE).Append(S_LKFS).ToString(), loudnessData.integrated);
			}
			GUILayout.EndHorizontal();
			chartRect = EditorGUILayout.GetControlRect(barHeightSetting);
			chartRect.xMin += CHART_INDENT_WIDTH;
			LoudnessBarGraph(chartRect, loudnessData.momentary, currentDbFloor, currentDbCeiling, currentDbTarget, currentDbRangePlusMinus, currentDbWarning, S_MOMENTARY);
			chartRect = EditorGUILayout.GetControlRect(barHeightSetting);
			chartRect.xMin += CHART_INDENT_WIDTH;
			LoudnessBarGraph(chartRect, loudnessData.shortTerm, currentDbFloor, currentDbCeiling, currentDbTarget, currentDbRangePlusMinus, currentDbWarning, S_SHORTTERM);
			chartRect = EditorGUILayout.GetControlRect(barHeightSetting);
			chartRect.xMin += CHART_INDENT_WIDTH;
			LoudnessBarGraph(chartRect, loudnessData.integrated, currentDbFloor, currentDbCeiling, currentDbTarget, currentDbRangePlusMinus, currentDbWarning, S_INTEGRATED);
			EditorGUI.indentLevel++;
			foldTogglerLoudnessAppearance = CriFoldout(foldTogglerLoudnessAppearance, S_FOLD_APPEARANCE);
			if (foldTogglerLoudnessAppearance) {
				EditorGUI.indentLevel++;
				EditorGUILayout.MinMaxSlider(GC_LEVELRANGE, ref currentDbFloor, ref currentDbCeiling, DB_MIN, DB_MAX);
				gStrBuilder.Length = 0;
				EditorGUILayout.LabelField(S_SPACE, gStrBuilder.Append(S_MIN).AppendFormat(S_FLOAT2FORMAT, currentDbFloor).Append(S_DIVIDER).Append(S_MAX).AppendFormat(S_FLOAT2FORMAT, currentDbCeiling).ToString());
				currentDbTarget = EditorGUILayout.Slider(S_TARGET, currentDbTarget, DB_MIN, DB_MAX);
				currentDbRangePlusMinus = EditorGUILayout.Slider(S_RANGE, currentDbRangePlusMinus, DB_RANGE_PLUSMINUS_MIN, DB_RANGE_PLUSMINUS_MAX);
				currentDbWarning = EditorGUILayout.Slider(S_WARNING, currentDbWarning, DB_MIN, DB_MAX);
				GUILayout.BeginHorizontal();
				{
					EditorGUILayout.LabelField(S_SPACE, GUILayout.Width(LABEL_WIDTH));
					if (GUILayout.Button(S_RESETDBRANGES, GUILayout.ExpandWidth(false))) {
						currentDbFloor = DEFAULT_DB_FLOOR;
						currentDbCeiling = DEFAULT_DB_CEILING;
						currentDbTarget = DEFAULT_DB_TARGET;
						currentDbRangePlusMinus = DEFAULT_DB_RANGE_PLUSMINUS;
						currentDbWarning = DEFAULT_DB_WARNING;
					}
				}
				GUILayout.EndHorizontal();
				EditorGUI.indentLevel--;
			}
			EditorGUI.indentLevel--;
		}
	}

	private void DrawLevelsInfo()
	{
		foldTogglerLevels = CriFoldout(foldTogglerLevels, S_FOLD_LEVELS);
		if (foldTogglerLevels) {
			if (isWindowPaused == false) {
				levelsData = new DataSliceLevels(profiler);
			}
			GUILayout.BeginHorizontal();
			{
				GUILayout.Space(GUI_INDENT_WIDTH);
				currentLevelType = (MultiChannelMeterType)GUILayout.Toolbar((int)currentLevelType, S_MultiChMeterButton);
			}
			GUILayout.EndHorizontal();
			Rect chartRect = EditorGUILayout.GetControlRect(GUILayout.Height(200));
			chartRect.xMin += CHART_INDENT_WIDTH;
			MultiChannelMeter(chartRect, levelsData.levels, levelsData.outputCh, currentLevelType, currentLevelFloor, currentLevelCeiling, currentLevelWarning, S_MultiChMeterButton[(int)currentLevelType]);
			EditorGUI.indentLevel++;
			foldTogglerLevelAppearance = CriFoldout(foldTogglerLevelAppearance, S_FOLD_APPEARANCE);
			if (foldTogglerLevelAppearance) {
				EditorGUI.indentLevel++;
				EditorGUILayout.MinMaxSlider(GC_LEVELRANGE, ref currentLevelFloor, ref currentLevelCeiling, DB_MIN, DB_MAX);
				gStrBuilder.Length = 0;
				EditorGUILayout.LabelField(S_SPACE, gStrBuilder.Append(S_MIN).AppendFormat(S_FLOAT2FORMAT, currentLevelFloor).Append(S_DIVIDER).Append(S_MAX).AppendFormat(S_FLOAT2FORMAT, currentLevelCeiling).ToString());
				currentLevelWarning = EditorGUILayout.Slider(S_WARNING, currentLevelWarning, DB_MIN, DB_MAX);
				currentLevelCalInterval = EditorGUILayout.IntSlider(S_GRID_INTVL, currentLevelCalInterval, DEFAULT_LEVEL_CAL_INTVL_MIN, DEFAULT_LEVEL_CAL_INTVL_MAX);
				GUILayout.BeginHorizontal();
				{
					EditorGUILayout.LabelField(S_SPACE, GUILayout.Width(LABEL_WIDTH));
					if (GUILayout.Button(S_RESETDBRANGES, GUILayout.ExpandWidth(false))) {
						currentLevelFloor = DEFAULT_LEVEL_FLOOR;
						currentLevelCeiling = DEFAULT_LEVEL_CEILING;
						currentLevelWarning = DEFAULT_LEVEL_WARNING;
						currentLevelCalInterval = DEFAULT_LEVEL_CAL_INTVL;
					}
				}
				GUILayout.EndHorizontal();
				EditorGUI.indentLevel--;
			}
			EditorGUI.indentLevel--;
		}
	}

	private bool CriFoldout(bool foldout, string content)
	{
#if UNITY_5_5_OR_NEWER
		return EditorGUILayout.Foldout(foldout, content, true);
#else
        return EditorGUILayout.Foldout(foldout, content);
#endif
	}

	private Single Level2Db(Single level)
	{
		return Mathf.Log10(level) * 20.0f;
	}

	private Single Value2Ratio(Single val, Single min, Single max)
	{
		if (max - min == 0.0f) {
			return 0.0f;
		}
		if (val > max) {
			return 1.0f;
		}
		if (val < min) {
			return 0.0f;
		}
		return (val - min) / (max - min);
	}

	private string Dbfs2String(Single dbVal, string unit, string format)
	{
		if(dbVal < Single.MinValue) {
			return S_BELOW_MIN;
		} else {
			gStrBuilderSub.Length = 0;
			return gStrBuilderSub.AppendFormat(format, dbVal).Append(S_SPACE).Append(unit).ToString();
		}
	}

	private string DataSize2String(float sizeByte)
	{
		gStrBuilderSub.Length = 0;
		if (sizeByte > 524288.0f) {
			return gStrBuilderSub.AppendFormat(S_FLOAT2FORMAT, sizeByte / 1048576.0f).Append(S_MBYTE).ToString();
		} else if (sizeByte > 512.0f) {
			return gStrBuilderSub.AppendFormat(S_FLOAT2FORMAT, sizeByte / 1024.0f).Append(S_KBYTE).ToString();
		} else {
			return gStrBuilderSub.AppendFormat(S_FLOAT0FORMAT, sizeByte).Append(S_BYTE).ToString();
		}
	}

	#endregion OnGUI

	#region Drawing Functions

	private void SimpleBarGraph(UnityEngine.Rect rect, Single val, Single min, Single max, String text)
	{
		Single ratio = Value2Ratio(val, min, max);

		UnityEngine.GUI.Box(rect, "", barBorderStyle);
		rect.xMin++;
		rect.yMin++;
		rect.xMax--;
		rect.yMax--;
		UnityEngine.GUI.Box(rect, "", barBackgroundStyle);
		if (Event.current.type == EventType.Repaint) {
			if (ratio > 0.0f) {
				Rect barRect = rect;
				if (ratio > 0.999999f) {
					UnityEngine.GUI.Box(barRect, "", barWarningHighStyle);
				} else {
					barRect.width *= ratio;
					UnityEngine.GUI.Box(barRect, "", barStyle);
				}
			}
		}
		rect.xMin += 2;
		UnityEngine.GUI.Label(new Rect(rect.x + 1, rect.y + 1, rect.width, rect.height), text, barTextShadowStyle);
		UnityEngine.GUI.Label(rect, text, barTextStyle);
	}

	private void LoudnessBarGraph(
		UnityEngine.Rect rect, 
		Single val, 
		Single min, 
		Single max, 
		Single valTarget, 
		Single valRangePlusMinus, 
		Single valWarningHigh,
		String text)
	{
		Single ratio = Value2Ratio(val, min, max);
		Single ratioTarget = Value2Ratio(valTarget, min, max);
		Single ratioRange = Value2Ratio(min + valRangePlusMinus, min, max);
		Single ratioWarning = Value2Ratio(valWarningHigh, min, max);

		UnityEngine.GUI.Box(rect, "", barBorderStyle);
		rect.xMin++;
		rect.yMin++;
		rect.xMax--;
		rect.yMax--;
		UnityEngine.GUI.Box(rect, "", barBackgroundStyle);
		if (Event.current.type == EventType.Repaint) {
			if (ratio > 0.0f) {
				Rect barRect = rect;
				if (ratio > ratioWarning) {
					barRect.width = rect.width * ratio;
					UnityEngine.GUI.Box(barRect, "", barWarningHighStyle);
				}
				if (ratio > ratioTarget + ratioRange) {
					barRect.width = rect.width * Mathf.Min(ratio, ratioWarning);
					UnityEngine.GUI.Box(barRect, "", barWarningHighTolerantStyle);
				}
				if (ratio > ratioTarget - ratioRange) {
					barRect.width = rect.width * Mathf.Min(ratio, ratioTarget + ratioRange);
					UnityEngine.GUI.Box(barRect, "", barMidRangeStyle);
				}
				if (ratio > 0) {
					barRect.width = rect.width * Mathf.Max(0, Mathf.Min(ratio, ratioTarget - ratioRange));
					UnityEngine.GUI.Box(barRect, "", barWarningLowStyle);
				}
			}
		}
		rect.xMin += 2;
		UnityEngine.GUI.Label(new Rect(rect.x + 1, rect.y + 1, rect.width, rect.height), text, barTextShadowStyle);
		UnityEngine.GUI.Label(rect, text, barTextStyle);
	}

	/* Positive values only */
	private void LineGraph(Rect rect, string title, float[] data, float maxVal, int sections, float minInterVal, bool adaptiveScale)
	{
		const float titleHeight = 20;
		const float labelWidth = 30;
		const float labelHeight = 10.0f;
		float cellHeight = 0;
		int count = 0;
		Rect labelRect;

		float currentMaxVal = maxVal;
		if(adaptiveScale == true) {
			currentMaxVal = Mathf.Min(Mathf.Max(data) * 2.0f, maxVal);
		}
		float interVal = (int)(currentMaxVal / sections);
		if (interVal < minInterVal) {
			interVal = minInterVal;
		}

		GUI.Box(rect, "", barBorderStyle);
		rect.xMin++;
		rect.yMin++;
		rect.xMax--;
		rect.yMax--;
		GUI.Box(rect, "", barBackgroundStyle);
		labelRect = new Rect(rect.x + textOffsetChartTitle.x, rect.y + textOffsetChartTitle.y, rect.width, titleHeight);
		GUI.Label(labelRect, title, barTextStyle);
		rect.xMin += labelWidth;
		rect.yMin += titleHeight;
		GUI.Box(rect, "", barBorderStyle);
		rect.xMin++;
		rect.yMin++;
		GUI.Box(rect, "", barBackgroundStyle);

		if (mat != null && Event.current.type == EventType.Repaint) {
			GUI.BeginClip(rect);
			GL.PushMatrix();
			{
				mat.SetPass(0);

				/* grid */
				GL.Begin(GL.LINES);
				cellHeight = rect.height * interVal / currentMaxVal;
				count = (int)(currentMaxVal / interVal);
				if(currentMaxVal % interVal == 0) {
					count -= 1;
				}
				GL.Color(colorGrid);
				float lineYPos = 0;
				for (int i = 1; i <= count; i++) {
					lineYPos = rect.height - i * cellHeight;
					GL.Vertex3(0, lineYPos, 0);
					GL.Vertex3(rect.width, lineYPos, 0);
				}
				GL.End();

                /* data */
#if UNITY_5_6_OR_NEWER
                GL.Begin(GL.LINE_STRIP);
                for (int i = 0; i < data.Length; i++) {
					GL.Color(ColorSelector(data[i], maxVal));
					GL.Vertex3((rect.width / (data.Length - 1)) * i, rect.height * (1 - data[i] / currentMaxVal), 0);
				}
				GL.End();
#else
                GL.Begin(GL.LINES);
                for (int i = 1; i < data.Length; i++)
                {
                    GL.Color(ColorSelector(data[i - 1], maxVal));
                    GL.Vertex3((rect.width / (data.Length - 1)) * (i - 1), rect.height * (1 - data[i - 1] / currentMaxVal), 0);
                    GL.Color(ColorSelector(data[i], maxVal));
                    GL.Vertex3((rect.width / (data.Length - 1)) * i, rect.height * (1 - data[i] / currentMaxVal), 0);
                }
                GL.End();
#endif
			}
			GL.PopMatrix();
			GUI.EndClip();

			/* labels*/
			rect.xMin -= labelWidth;
			labelRect = new Rect(rect.x + textOffsetCalibrator.x, rect.y + textOffsetCalibrator.y, labelWidth, labelHeight);
			GUI.Label(labelRect, currentMaxVal.ToString(S_FLOAT0), chartLabelStyle);
			for (int i = 1; i <= count; i++) {
				labelRect.y = rect.y + rect.height - i * cellHeight + textOffsetCalibrator.y;
				if (labelRect.y > rect.y) {
					GUI.Label(labelRect, (interVal * i).ToString(S_FLOAT0), chartLabelStyle);
				}
			}
		} /* if Event == Repaint */
	}

	private void NumericMeter(UnityEngine.Rect rect, String meterLabel, Single meterVal)
	{
		const Single titleHeight = 20;
		Rect labelRect;
		Rect numRect;

		UnityEngine.GUI.Box(rect, "", barBorderStyle);
		rect.xMin++;
		rect.yMin++;
		rect.xMax--;
		rect.yMax--;
		UnityEngine.GUI.Box(rect, "", barBackgroundStyle);

		labelRect = new Rect(rect.x + textOffsetChartTitle.x, rect.y + textOffsetChartTitle.y, rect.width, titleHeight);
		GUI.Label(labelRect, meterLabel, barTextStyle);
		numRect = new Rect(rect.center.x, rect.center.y, 10, 10);
		if (meterVal > currentDbWarning) {
			GUI.Label(numRect, Dbfs2String(meterVal, "", S_FLOAT1FORMAT), meterTextWarningHighStyle);
		} else if (meterVal > currentDbTarget + currentDbRangePlusMinus) {
			GUI.Label(numRect, Dbfs2String(meterVal, "", S_FLOAT1FORMAT), meterTextWarningHighTolerantStyle);
		} else if (meterVal > currentDbTarget - currentDbRangePlusMinus) {
			GUI.Label(numRect, Dbfs2String(meterVal, "", S_FLOAT1FORMAT), meterTextMidRangeStyle);
		} else {
			GUI.Label(numRect, Dbfs2String(meterVal, "", S_FLOAT1FORMAT), meterTextWarningLowStyle);
		}
	}

	private enum MultiChannelMeterType {
		Peak,
		RMS
	}
	private void MultiChannelMeter(
		UnityEngine.Rect rect, 
		CriProfiler.LevelInfo[] info, 
		int numCh, 
		MultiChannelMeterType type,
		float min,
		float max,
		float warningThresh,
		string title)
	{
		float warningRatio = (warningThresh - min) / (max - min);
		
		const float barInterval = 20.0f;
		const float calWidth = 30.0f;
		const float calHeight = 10.0f;
		const float titleHeight = 20.0f;
		const float headHeight = 20.0f;
		const float footHeight = 20.0f;
		const float baseValue = 0.0f;
		Color barColor = Color.green;
		Color barWarningColor = Color.red;
		Color holderColor = Color.green;
		Color holderWarningColor = Color.red;
		Rect chartRect;

		GUI.Box(rect, "", barBorderStyle);
		rect.xMin++;
		rect.yMin++;
		rect.xMax--;
		rect.yMax--;
		GUI.Box(rect, "", barBackgroundStyle);

		chartRect = rect;
		chartRect.xMin += calWidth;
		chartRect.xMax++;
		chartRect.yMin += titleHeight + headHeight;
		chartRect.yMax -= footHeight;
		GUI.Box(chartRect, "", barBorderStyle);
		chartRect.xMin++;
		chartRect.yMin++;
		chartRect.xMax--;
		chartRect.yMax--;
		GUI.Box(chartRect, "", barBackgroundStyle);

		/* title */
		GUI.Label(new Rect(rect.x + textOffsetChartTitle.x, rect.y + textOffsetChartTitle.y, rect.width, rect.height), title, barTextStyle);
		rect.yMin += titleHeight;

		/* horizontal labels */
		float barWidth = (chartRect.width - barInterval * (numCh + 1)) / numCh;
		float labelXStart = calWidth + 1 + barInterval;
		if (info != null && info.Length >= numCh) {
			for (int i = 0; i < numCh; ++i) {
				switch (type) {
					case MultiChannelMeterType.Peak:
						GUI.Label(new Rect(rect.x + labelXStart, rect.y, barWidth, headHeight), Dbfs2String(Level2Db(info[i].levelPeak), "", S_FLOAT1FORMAT), multiChMeterTextStyle);
						break;
					case MultiChannelMeterType.RMS:
						GUI.Label(new Rect(rect.x + labelXStart, rect.y, barWidth, headHeight), Dbfs2String(Level2Db(info[i].levelRms), "", S_FLOAT1FORMAT), multiChMeterTextStyle);
						break;
					default:
						break;
				}
				GUI.Label(new Rect(rect.x + labelXStart, rect.y + rect.height - footHeight, barWidth * 0.5f, footHeight), S_CHANNEL, multiChMeterTextStyle);
				GUI.Label(new Rect(rect.x + labelXStart + barWidth * 0.5f, rect.y + rect.height - footHeight, barWidth * 0.5f, footHeight), (i + 1).ToString(), multiChMeterTextStyle);
				labelXStart += barInterval + barWidth;
			}
		}

		/* vertical labels */
		rect.yMin += headHeight + 1;
		rect.yMax -= footHeight + 1;
		float cellHeight = rect.height * currentLevelCalInterval / (currentLevelCeiling - currentLevelFloor);

		float firstCellInterval = ((currentLevelCeiling - baseValue) % currentLevelCalInterval + currentLevelCalInterval) % currentLevelCalInterval;
		if (firstCellInterval == 0) {
			firstCellInterval = currentLevelCalInterval;
		}
		float firstCellHeight = rect.height * firstCellInterval / (currentLevelCeiling - currentLevelFloor);

		float remainingInterval = currentLevelCeiling - firstCellInterval - currentLevelFloor;
		int cellCount = (int)(remainingInterval / currentLevelCalInterval);
		if (remainingInterval % currentLevelCalInterval == 0) {
			cellCount -= 1;
		}

		GUI.Label(new Rect(rect.x + textOffsetCalibrator.x, rect.y + textOffsetCalibrator.y, calWidth, calHeight), currentLevelCeiling.ToString(S_FLOAT0), chartLabelStyle);
		GUI.Label(new Rect(rect.x + textOffsetCalibrator.x, rect.y + rect.height + textOffsetCalibrator.y, calWidth, calHeight), currentLevelFloor.ToString(S_FLOAT0), chartLabelStyle);
		for (int i=0; i < cellCount + 1; ++i) {
			GUI.Label(new Rect(rect.x + textOffsetCalibrator.x, rect.y + firstCellHeight + cellHeight * i + textOffsetCalibrator.y, calWidth, calHeight), (currentLevelCeiling - firstCellInterval - currentLevelCalInterval * i).ToString(S_FLOAT0), chartLabelStyle);
		}

		/* GL drawing */
		if (mat != null && Event.current.type == EventType.Repaint) {
			GUI.BeginClip(chartRect);
			GL.PushMatrix();
			{
				mat.SetPass(0);

				/* grid */
				float lineYPos = 0;
				GL.Begin(GL.LINES);
				GL.Color(colorGrid);
				for (int i = 0; i < cellCount + 1; ++i) {
					lineYPos = firstCellHeight + i * cellHeight;
					GL.Vertex3(0, lineYPos, 0);
					GL.Vertex3(chartRect.width, lineYPos, 0);
				}
				GL.End();

				/* bars */
				if (info != null && info.Length >= numCh && barInterval * (numCh + 1) < chartRect.width) {
					float barHeight;
					float barWarningHeight;
					float barXStart;
					for (int i = 0; i < numCh; ++i) {
						float heightRatio = 0;
						switch (type) {
							case MultiChannelMeterType.Peak:
								heightRatio = ((Level2Db(info[i].levelPeak) - currentLevelFloor) / (currentLevelCeiling - currentLevelFloor));
								break;
							case MultiChannelMeterType.RMS:
								heightRatio = ((Level2Db(info[i].levelRms) - currentLevelFloor) / (currentLevelCeiling - currentLevelFloor));
								break;
							default:
								break;
						}
						if (heightRatio > 1.0f) {
							heightRatio = 1.0f;
						}
						if (heightRatio < 0) {
							heightRatio = 0;
						}
						barHeight = chartRect.height * heightRatio;
						barWarningHeight = chartRect.height * warningRatio;
						barXStart = (barInterval + barWidth) * i + barInterval;
						GL.Begin(GL.QUADS);
						if (heightRatio > warningRatio) {
							GL.Color(barWarningColor);
							GL.Vertex3(barXStart, chartRect.height, 0);
							GL.Vertex3(barXStart + barWidth, chartRect.height, 0);
							GL.Vertex3(barXStart + barWidth, chartRect.height - barHeight, 0);
							GL.Vertex3(barXStart, chartRect.height - barHeight, 0);
						}
						GL.Color(barColor);
						GL.Vertex3(barXStart, chartRect.height, 0);
						GL.Vertex3(barXStart + barWidth, chartRect.height, 0);
						GL.Vertex3(barXStart + barWidth, chartRect.height - Mathf.Max(0, Mathf.Min(barHeight, barWarningHeight)), 0);
						GL.Vertex3(barXStart, chartRect.height - Mathf.Max(0, Mathf.Min(barHeight, barWarningHeight)), 0);
						GL.End();

						/* holders */
						if (type == MultiChannelMeterType.Peak) {
							heightRatio = ((Level2Db(info[i].levelPeakHold) - currentLevelFloor) / (currentLevelCeiling - currentLevelFloor));
							if (heightRatio > 1.0f) {
								heightRatio = 1.0f;
							}
							if (heightRatio < 0) {
								heightRatio = 0;
							}
							barHeight = chartRect.height * heightRatio;
							GL.Begin(GL.LINES);
							if (heightRatio > warningRatio) {
								GL.Color(holderWarningColor);
							} else {
								GL.Color(holderColor);
							}
							GL.Vertex3(barXStart, chartRect.height - barHeight, 0);
							GL.Vertex3(barXStart + barWidth, chartRect.height - barHeight, 0);
							GL.End();
						}
					}
				}
			}
			GL.PopMatrix();
			GUI.EndClip();
		}
	}

	#endregion Drawing Functions

	private Color ColorSelector(float sample, float maxVal)
	{
		if (sample < maxVal / 3.0f) {
			return Color.green;
		}
		if (sample < maxVal * 2.0f / 3.0f) {
			return Color.yellow;
		}
		return Color.red;
	}

}
