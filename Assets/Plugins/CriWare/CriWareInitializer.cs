/****************************************************************************
 *
 * Copyright (c) 2012 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;


/*JP
 * \brief CRI File System初期化パラメータ
 */
[System.Serializable]
public class CriFsConfig {
	/*JP デバイス性能読み込み速度のデフォルト値(bps) */
	public const int defaultAndroidDeviceReadBitrate = 50000000;

	/*JP ローダー数 */
	public int numberOfLoaders    = 16;
	/*JP バインダ数 */
	public int numberOfBinders    = 8;
	/*JP インストーラ数 */
	public int numberOfInstallers = 2;
	/*JP インストールバッファのサイズ */
	public int installBufferSize  = CriFsPlugin.defaultInstallBufferSize / 1024;
	/*JP パスの最大長 */
	public int maxPath            = 256;
	/*JP ユーザーエージェント文字列 */
	public string userAgentString = "";
	/*JP ファイルディスクリプタの節約モードフラグ */
	public bool minimizeFileDescriptorUsage = false;
	/*JP [Android] デバイス性能読み込み速度(bps) */
	public int androidDeviceReadBitrate = defaultAndroidDeviceReadBitrate;

}

/*JP
 * \brief CRI Atom初期化パラメータ
 */
[System.Serializable]
public class CriAtomConfig {
	/*JP ACFファイル名
	 *   \attention ACFファイルをStreamingAssetsフォルダに配置しておく必要あり。 */
	public string acfFileName = "";
	
	/*JP 標準ボイスプール作成パラメータ */
	[System.Serializable]
	public class StandardVoicePoolConfig {
		public int memoryVoices    = 16;
		public int streamingVoices = 8;
	}
	
	/*JP HCA-MXボイスプール作成パラメータ */
	[System.Serializable]
	public class HcaMxVoicePoolConfig {
		public int memoryVoices    = 0;
		public int streamingVoices = 0;
	}
	
	/*JP 最大バーチャルボイス数 */
	public int maxVirtualVoices = 32;
	/*JP 最大ボイスリミットグループ数 */
	public int maxVoiceLimitGroups = 32;
	/*JP 最大カテゴリ数 */
	public int maxCategories = 32;
	/*JP 1フレーム当たりの最大シーケンスイベント数 */
	public int maxSequenceEventsPerFrame = 2;
	/*JP 1フレーム当たりの最大ビート同期コールバック数 */
	public int maxBeatSyncCallbacksPerFrame = 1;
	/*JP 標準ボイスプール作成パラメータ */
	public StandardVoicePoolConfig standardVoicePoolConfig = new StandardVoicePoolConfig();
	/*JP HCA-MXボイスプール作成パラメータ */
	public HcaMxVoicePoolConfig hcaMxVoicePoolConfig = new HcaMxVoicePoolConfig();
	/*JP 出力サンプリングレート */
	public int outputSamplingRate = 0;
	/*JP インゲームプレビューを使用するかどうか */
	public bool usesInGamePreview = false;
	/*JP サーバ周波数 */
	public float serverFrequency  = 60.0f;
	/*JP ASR出力チャンネル数 */
	public int asrOutputChannels  = 0;
	/*JP 乱数種に時間（System.DateTime.Now.Ticks）を使用するかどうか */
	public bool useRandomSeedWithTime = false;
	/*JP 再生単位でのカテゴリ参照数 */
	public int categoriesPerPlayback = 4;
	/*JP 最大バス数 */
	public int maxBuses = 8;
	/*JP 最大パラメータブロック数 */
	public int maxParameterBlocks = 1024;
	/*JP VR サウンド出力モードを使用するか否か */
	public bool vrMode = false;

	/*JP [PC] 出力バッファリング時間 */
	public int pcBufferingTime = 0;
	
	/*JP [iOS] 出力バッファリング時間(ミリ秒)*/
	public int  iosBufferingTime     = 50;
	/*JP [iOS] iPodの再生を上書きするか？ */
	public bool iosOverrideIPodMusic = false;

	/*JP [Android] 出力バッファリング時間 */
	public int androidBufferingTime      = 133;
	/*JP [Android] 再生開始バッファリング時間 */
	public int androidStartBufferingTime = 100;

	/*JP [Android] 低遅延再生用ボイスプール作成パラメータ */
	[System.Serializable]
	public class AndroidLowLatencyStandardVoicePoolConfig {
		public int memoryVoices    = 0;
		public int streamingVoices = 0;
	}
	/*JP [Android] 低遅延再生用ボイスプール作成パラメータ */
	public AndroidLowLatencyStandardVoicePoolConfig androidLowLatencyStandardVoicePoolConfig = new AndroidLowLatencyStandardVoicePoolConfig();
	/*JP [Android] Android OS の Fast Mixer を使用して、音声再生時の発音遅延を短縮するかどうか。ASR/NSR の発音遅延や、遅延推測機能の結果に影響する */
	public bool androidUsesAndroidFastMixer = true;
}

/*JP
 * \brief CRI Mana初期化パラメータ
 */
[System.Serializable]
public class CriManaConfig {
	/*JP デコーダー数 */
	public int  numberOfDecoders   = 8;
	/*JP 連結再生エントリー数 */
	public int  numberOfMaxEntries = 4;
	/*JP GL.IssuePluginEventを用いたマルチスレッドでのテクスチャ描画処理を有効にするかどうか */
	public readonly bool graphicsMultiThreaded = true; // always true.
}

/// \addtogroup CRIWARE_UNITY_COMPONENT
/// @{

/*JP
 * \brief CRIWARE初期化コンポーネント
 * \par 説明:
 * CRIWAREライブラリの初期化を行うためのコンポーネントです。<br>
 */
[AddComponentMenu("CRIWARE/Library Initializer")]
public class CriWareInitializer : MonoBehaviour {
	
	/*JP CRI File Systemライブラリを初期化するかどうか */
	public bool initializesFileSystem = true;
	
	/*JP CRI File Systemライブラリ初期化設定 */
	public CriFsConfig fileSystemConfig = new CriFsConfig();
	
	/*JP CRI Atomライブラリを初期化するかどうか */
	public bool initializesAtom = true;
	
	/*JP CRI Atomライブラリ初期化設定 */
	public CriAtomConfig atomConfig = new CriAtomConfig();
	
	/*JP CRI Manaライブラリを初期化するかどうか */
	public bool initializesMana = true;
	
	/*JP CRI Manaライブラリ初期化設定 */
	public CriManaConfig manaConfig = new CriManaConfig();

	/*JP Awake時にライブラリを初期化するかどうか */
	public bool dontInitializeOnAwake = false;

	/*JP シーンチェンジ時にライブラリを終了するかどうか */
	public bool dontDestroyOnLoad = false;

	/* オブジェクト作成時の処理 */
	void Awake() {
		/* 現在のランタイムのバージョンが正しいかチェック */
		CriWare.CheckBinaryVersionCompatibility();

		if (dontInitializeOnAwake) {
			/* フラグが立っていた場合はAwakeでは初期化を行わない */
			return;
		}

		/* プラグインの初期化 */
		this.Initialize();
	}
	
	/* Execution Order の設定を確実に有効にするために OnEnable をオーバーライド */
	void OnEnable() {
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	/**
	 * <summary>プラグインの初期化（手動初期化用）</summary> 
	 * \par 説明:
	 * プラグインの初期化を行います。<br/>
	 * デフォルトでは本関数はAwake関数内で自動的に呼び出されるので、アプリケーションが直接呼び出す必要はありません。<br/>
	 * <br/>
	 * 初期化パラメタをスクリプトから動的に変更して初期化を行いたい場合、本関数を使用してください。<br/>
	 * \par 注意：
	 * 本関数を使用する場合は、 自動初期化を無効にするため、 ::CriWareInitializer::dontInitializeOnAwake プロパティをインスペクタ上でチェックしてください。<br/>
	 * また、本関数を呼び出すタイミングは全てのプラグインAPIよりも前に呼び出す必要があります。Script Execution Orderが高いスクリプト上で行ってください。
	 * 
	 */
	public void Initialize() {
		/* 初期化カウンタの更新 */
		initializationCount++;
		if (initializationCount != 1) {
			/* CriWareInitializer自身による多重初期化は許可しない */
			GameObject.Destroy(this);
			return;
		}

		/* 非実行時にライブラリ機能を使用していた場合は一度終了処理を行う */
		if ((CriFsPlugin.IsLibraryInitialized() == true && CriAtomPlugin.IsLibraryInitialized() == true && CriManaPlugin.IsLibraryInitialized() == true) ||
			(CriFsPlugin.IsLibraryInitialized() == true && CriAtomPlugin.IsLibraryInitialized() == true && CriManaPlugin.IsLibraryInitialized() == false) ||
			(CriFsPlugin.IsLibraryInitialized() == true && CriAtomPlugin.IsLibraryInitialized() == false && CriManaPlugin.IsLibraryInitialized() == false)) {
#if UNITY_EDITOR || (!UNITY_PS3)
			/* CRI Manaライブラリの終了 */
			if (initializesMana) {
				CriManaPlugin.FinalizeLibrary();
			}
#endif

			/* CRI Atomライブラリの終了 */
			if (initializesAtom) {
				/* EstimatorがStop状態になるまでFinalize */
				while (CriAtomExLatencyEstimator.GetCurrentInfo().status != CriAtomExLatencyEstimator.Status.Stop) {
					CriAtomExLatencyEstimator.FinalizeModule();
				}

				/* 終了処理の実行 */
				CriAtomPlugin.FinalizeLibrary();
			}

			/* CRI File Systemライブラリの終了 */
			if (initializesFileSystem) {
				CriFsPlugin.FinalizeLibrary();
			}
		}

		/* CRI File Systemライブラリの初期化 */
		if (initializesFileSystem) {
			InitializeFileSystem(fileSystemConfig);
		}

		/* CRI Atomライブラリの初期化 */
		if (initializesAtom) {
			InitializeAtom(atomConfig);
		}

		/* シーンチェンジ後もオブジェクトを維持するかどうかの設定 */
		if (dontDestroyOnLoad) {
			DontDestroyOnLoad(transform.gameObject);
			DontDestroyOnLoad(CriWare.managerObject);
		}
	}

	/**
	 * <summary>プラグインの終了（手動終了用）</summary> 
	 * \par 説明:
	 * プラグインを終了します。<br/>
	 * デフォルトでは本関数はOnDestroy関数内で自動的に呼び出されるので、アプリケーションが直接呼び出す必要はありません。
	 */
	public void Shutdown() {
		/* 初期化カウンタの更新 */
		initializationCount--;
		if (initializationCount != 0) {
			initializationCount = initializationCount < 0 ? 0 : initializationCount;
			return;
		}

		/* CRI Atomライブラリの終了 */
		if (initializesAtom) {
			/* EstimatorがStop状態になるまでFinalize */
			while (CriAtomExLatencyEstimator.GetCurrentInfo().status != CriAtomExLatencyEstimator.Status.Stop) {
				CriAtomExLatencyEstimator.FinalizeModule();
			}

			/* 終了処理の実行 */
			CriAtomPlugin.FinalizeLibrary();
		}

		/* CRI File Systemライブラリの終了 */
		if (initializesFileSystem) {
			CriFsPlugin.FinalizeLibrary();
		}
	}

	void OnDestroy() {
		Shutdown();
	}
	
	/* 初期化カウンタ */
	private static int initializationCount = 0;

	/* 初期化実行チェック関数 */
	public static bool IsInitialized() {
		if (initializationCount > 0) {
			return true;
		} else {
			/* 現在のランタイムのバージョンが正しいかチェック */
			CriWare.CheckBinaryVersionCompatibility();
			return false;
		}
	}


	public static bool InitializeFileSystem(CriFsConfig config)
	{
		/* CRI File Systemライブラリの初期化 */
		if (!CriFsPlugin.IsLibraryInitialized()) {
			CriFsPlugin.SetConfigParameters(
				config.numberOfLoaders,
				config.numberOfBinders,
				config.numberOfInstallers,
				(config.installBufferSize * 1024),
				config.maxPath,
				config.minimizeFileDescriptorUsage
				);
			{
				/* Ver.2.03.03 以前は 0 がデフォルト値だったことの互換性維持のための処理 */
				if (config.androidDeviceReadBitrate == 0) {
					config.androidDeviceReadBitrate = CriFsConfig.defaultAndroidDeviceReadBitrate;
				}
			}
			CriFsPlugin.SetConfigAdditionalParameters_ANDROID(config.androidDeviceReadBitrate);
			CriFsPlugin.InitializeLibrary();
			if (config.userAgentString.Length != 0) {
				CriFsUtility.SetUserAgentString(config.userAgentString);
			}
			return true;
		} else {
			return false;
		}
	}

	public static bool InitializeAtom(CriAtomConfig config)
	{
		/* CRI Atomライブラリの初期化 */
		if (CriAtomPlugin.IsLibraryInitialized() == false) {
			/* 初期化処理の実行 */
			CriAtomPlugin.SetConfigParameters(
				(int)Math.Max(config.maxVirtualVoices, CriAtomPlugin.GetRequiredMaxVirtualVoices(config)),
				config.maxVoiceLimitGroups,
				config.maxCategories,
				config.maxSequenceEventsPerFrame,
				config.maxBeatSyncCallbacksPerFrame,
				config.standardVoicePoolConfig.memoryVoices,
				config.standardVoicePoolConfig.streamingVoices,
				config.hcaMxVoicePoolConfig.memoryVoices,
				config.hcaMxVoicePoolConfig.streamingVoices,
				config.outputSamplingRate,
				config.asrOutputChannels,
				config.usesInGamePreview,
				config.serverFrequency,
				config.maxParameterBlocks,
				config.categoriesPerPlayback,
				config.maxBuses,
				config.vrMode);

			CriAtomPlugin.SetConfigAdditionalParameters_PC(
				config.pcBufferingTime
				);

			CriAtomPlugin.SetConfigAdditionalParameters_IOS(
				(uint)Math.Max(config.iosBufferingTime, 16),
				config.iosOverrideIPodMusic
				);
			/* Android 固有の初期化パラメータを登録 */
			{
				/* Ver.2.03.03 以前は 0 がデフォルト値だったことの互換性維持のための処理 */
				if (config.androidBufferingTime == 0) {
					config.androidBufferingTime = (int)(4 * 1000.0 / config.serverFrequency);
				}
				if (config.androidStartBufferingTime == 0) {
					config.androidStartBufferingTime = (int)(3 * 1000.0 / config.serverFrequency);
				}
				IntPtr android_context = IntPtr.Zero;
#if !UNITY_EDITOR && UNITY_ANDROID
				if (config.androidUsesAndroidFastMixer) {
					AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
					AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
					android_context = activity.GetRawObject();
				}
#endif
				CriAtomPlugin.SetConfigAdditionalParameters_ANDROID(
					config.androidLowLatencyStandardVoicePoolConfig.memoryVoices,
					config.androidLowLatencyStandardVoicePoolConfig.streamingVoices,
					config.androidBufferingTime,
					config.androidStartBufferingTime,
					android_context);
			}

			CriAtomPlugin.InitializeLibrary();

			if (config.useRandomSeedWithTime == true){
				/* 時刻を乱数種に設定 */
				CriAtomEx.SetRandomSeed((uint)System.DateTime.Now.Ticks);
			}

			/* ACFファイル指定時は登録 */
			if (config.acfFileName.Length != 0) {
				string acfPath = config.acfFileName;
				if (CriWare.IsStreamingAssetsPath(acfPath)) {
					acfPath = Path.Combine(CriWare.streamingAssetsPath, acfPath);
				}

				CriAtomEx.RegisterAcf(null, acfPath);
			}
			return true;
		} else {
			return false;
		}

	}

	public static bool InitializeMana(CriManaConfig config) {
		/* CRI Manaライブラリの初期化 */
		if (CriManaPlugin.IsLibraryInitialized() == false) {
			CriManaPlugin.SetConfigParameters(config.graphicsMultiThreaded, config.numberOfDecoders, config.numberOfMaxEntries);
			CriManaPlugin.InitializeLibrary();

			// set shader global keyword to inform cri mana shaders to output to correct colorspace
			if (QualitySettings.activeColorSpace == ColorSpace.Linear) {
				Shader.EnableKeyword("CRI_LINEAR_COLORSPACE");
			}
			return true;
		} else {
			return false;
		}
	}
} // end of class

/// @}

/* --- end of file --- */
