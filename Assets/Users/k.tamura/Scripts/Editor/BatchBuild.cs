using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;

namespace SFAI
{
    public enum SetBuildSettings
    {
        None = 0,
        Debug,
        Release,
        Master
    }
    public class BatchBuild
    {

        // Android ビルド
        [MenuItem("SFAI/Build/Android/None")]
        static void AndroidBuild()
        {
            SetSymbols(SetBuildSettings.None, BuildTargetGroup.Android);
            AndroidBuilder();
        }
        [MenuItem("SFAI/Build/Android/Debug")]
        static void AndroidBuildDebug()
        {
            SetSymbols(SetBuildSettings.Debug, BuildTargetGroup.Android);
            AndroidBuilder();
        }
        [MenuItem("SFAI/Build/Android/Release")]
        static void AndroidBuildRelease()
        {
            SetSymbols(SetBuildSettings.Release, BuildTargetGroup.Android);
            AndroidBuilder();
        }
        [MenuItem("SFAI/Build/Android/Master")]
        static void AndroidBuildMaster()
        {
            SetSymbols(SetBuildSettings.Master, BuildTargetGroup.Android);
            AndroidBuilder();
        }

        static void AndroidBuilder()
        {

            // 有効なシーン一覧のパスを取得
            var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();

            // 出力用のファイルを定義
            var outputFile = Application.dataPath + "/../androidBuild.apk";
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            var target = BuildTarget.Android;
            var options = BuildOptions.None;

            BuildPipeline.BuildPlayer(scenes, outputFile, target, options);
        }

        // iOSビルド
        [MenuItem("SFAI/Build/iOS/None")]
        static void iOSBuild()
        {
            SetSymbols(SetBuildSettings.None);
            iOSBuilder();
        }
        [MenuItem("SFAI/Build/iOS/Debug")]
        static void iOSBuildDebug()
        {
            SetSymbols(SetBuildSettings.Debug);
            iOSBuilder();
        }
        [MenuItem("SFAI/Build/iOS/Release")]
        static void iOSBuildRelease()
        {
            SetSymbols(SetBuildSettings.Release);
            iOSBuilder();
        }
        [MenuItem("SFAI/Build/iOS/Master")]
        static void iOSBuildMaster()
        {
            SetSymbols(SetBuildSettings.Master);
            iOSBuilder();
        }

        static void iOSBuilder()
        {

            // 有効なシーン一覧のパスを取得
            var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();

            // 出力用のフォルダを設定
            var outputFile = Application.dataPath + "/../XcodeProject";
            if (Directory.Exists(outputFile))
            {
                Directory.Delete(outputFile, true);
            }

            var target = BuildTarget.iOS;
            var options = BuildOptions.None;

            BuildPipeline.BuildPlayer(scenes, outputFile, target, options);
        }

        [MenuItem("SFAI/SetSymbols/None")]
        static void SetSymbol()
        {
            SetSymbols(SetBuildSettings.None);
        }
        [MenuItem("SFAI/SetSymbols/Debug")]
        static void SetSymbolDebug()
        {
            SetSymbols(SetBuildSettings.Debug);
        }
        [MenuItem("SFAI/SetSymbols/Release")]
        static void SetSymbolRelease()
        {
            SetSymbols(SetBuildSettings.Release);
        }
        [MenuItem("SFAI/SetSymbols/Master")]
        static void SetSymbolMaster()
        {
            SetSymbols(SetBuildSettings.Master);
        }


        static void SetSymbols(SetBuildSettings buildsettings = SetBuildSettings.Debug, BuildTargetGroup targetGroup = BuildTargetGroup.iOS, BuildTarget target = BuildTarget.iOS)
        {
            if (!EditorUserBuildSettings.activeBuildTarget.Equals(target))
                EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, target);
            StringBuilder builder = new StringBuilder();
            builder.Clear();
            switch (buildsettings)
            {
                case SetBuildSettings.Debug:
                    builder.Append("SFAI_DEBUG;SFAI_SOUND;UNITY_POST_PROCESSING_STACK_V2;");
                    break;
                case SetBuildSettings.Release:
                    builder.Append("SFAI_RELEASE;SFAI_L_SOUND;UNITY_POST_PROCESSING_STACK_V2;");
                    break;
                case SetBuildSettings.Master:
                    builder.Append("SFAI_MASTER;SFAI_L_SOUND;UNITY_POST_PROCESSING_STACK_V2;");
                    break;
                case SetBuildSettings.None:
                    builder.Append("UNITY_POST_PROCESSING_STACK_V2;");
                    break;
                default:
                    Debug.LogErrorFormat("Missing Settings : {0}", buildsettings);
                    return;
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, builder.ToString());
        }


    }
}