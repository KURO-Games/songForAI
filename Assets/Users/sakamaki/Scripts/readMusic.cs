using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class readMusic : MonoBehaviour
{
    // 保管する用のListClass
    private List<string[]> musicList = new List<string[]>();
    // csvファイルの中身をutf形式でencodingする
    private Encoding encoding;

    void Start()
    {
        // utf-8形式習得
        encoding = Encoding.GetEncoding("utf-8");
        // csvFileを習得する
        var csvFile = Application.streamingAssetsPath + ("Musics.csv");
        // Debug.log確認
        Debug.Log(csvFile);
        // Musics.csvの中身をutf-8に変更
        var reader = File.ReadAllText();

        // whileで一行ずつ習得
        // 習得できなかった場合Peek値が-1になるので-1以上の時に習得する
        while (reader.Peek() > -1)
        {
            // 1行ずつ読み込み
            var lineRead = reader.ReadLine();
            var Sep = lineRead.Split(',');
            // 末尾まで繰り返す
            musicList.Add(Sep);
        }

        //読み込んだデータを表示
        foreach (var data in musicList)
        {
            Debug.Log("曲名:" + data[0] + "作曲:" + data[1]
                      + "ノーツ作成者:" + data[2] + "カバーアルバム名" + data[3] + "ノーツデータ名:" + data[4]
                      + "Easy:" + data[5] + "Normal:" + data[6] + "Hard:" + data[7] + "Pro:" + data[8]);
        }
    }

    // Resources.Loadで実装していたため治すのに手こずり..作業途中です。
}
