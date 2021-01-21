/// <summary>
/// CueSheet用SoundDefine
/// 基本変更禁止
/// </summary>
public static class SoundDefine
{
    /// <summary>
    /// ファイルパス
    /// </summary>
    public static readonly string filePath = UnityEngine.Application.streamingAssetsPath + "/Sound/{0}";


    #region 曲一覧(BGM枠)
    public static readonly string Title = "Title";//タイトル
    public static readonly string Home = "Home";//ホーム画面
    public static readonly string[] musics = new string[] //enum用配列と合わせてます
    {
        "music01", "music02", "music03" 
    };
    public static readonly string music01 = "music01";//シャイニングスター
    public static readonly string music02 = "music02";//君の笑顔
    public static readonly string music03 = "music03";//魔王城
    #endregion
    #region Se
    public static readonly string Se = "Se";//SE用
    #endregion
    #region シナリオ小節(Scenario枠)
    public static readonly string sc01 = "sc01";//シナリオ1
    public static readonly string sc02 = "sc02";//シナリオ2
    public static readonly string sc03 = "sc03";//シナリオ3
    /*
     * cueIDでボイス有りシナリオを0から加算していく
     * */
    #endregion
    #region 演奏ゲームボイス(Scenario枠を利用)
    public static readonly string flanz = "flanz";//フランツ用
    public static readonly string eric = "eric";//エリック用
    public static readonly string pare = "pare";//パレストリーナ・ジョバンニ用
    #endregion
    /*
     *　cueIDで管理
     *　0:開始時
     *　1:フルコンボ
     *　2:そこそこ
     *　3:ダメダメ
     */

    /*
     * SEとDemoは既存番号と同じにしているので記載無し
     */

}
