using UnityEngine;
using System.Collections;

public class GameMain : MonoBehaviour {

	//ロゴシーケンスが終わったら有効化するゲームオブジェクト
	public GameObject stageData;

	void Awake()
	{
		if(stageData)
			stageData.SetActiveRecursively(false);
			//Unity 4.xの場合は下記をお使い下さい
			//stageData.SetActive(false);
	}

	//ロゴシーケンスのスクリプトからSendMessageされるメソッド
	public void OnLogoSequenceEnd()
	{
		//指定のゲームオブジェクトを有効化する
		if(stageData)
			stageData.SetActiveRecursively(true);
			//Unity 4.xの場合は下記をお使い下さい
			//stageData.SetActive(true);
	}
}
