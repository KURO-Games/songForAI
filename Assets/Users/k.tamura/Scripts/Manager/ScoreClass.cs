using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreClass
{
    public const string PlayerPrefsFormat = "{0}_{1}_{2}_{3}";
    public const string PlayerPrefsHighScore = "HighScore";
    public const string PlayerPrefsMaxCombo = "MaxCombo";
    public const string PlayerPrefsHighRank = "HighRank";

    /*
     * string.Formatを使い、PlayerPrefsFormatの形に変更する ({0}_{1}_{2}_{3})
     * string.Format(ScoreClass.PlayerPrefsFormat,
     *  MusicDatas.NotesDataName, (int) MusicDatas.gameType, MusicDatas.difficultNumber,ScoreClass.PlayerPrefsHighScore)
     *  であった場合 {0(曲名)}_{1(ゲームタイプ)}_{2(難易度の数値)}_{3(ハイスコア)} で保存される
     *  これにより、難易度とゲームタイプが別々でも個別の保存が行える
     */
}
