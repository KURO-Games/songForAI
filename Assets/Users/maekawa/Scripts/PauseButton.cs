using System.Collections;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
	//対象のオブジェクト（今回はPauseUI）をアタッチする必要あり
	//手順：スクリプトをアタッチ→インスペクターで対象をアタッチ

	[SerializeField] //エディタ拡張
	private GameObject PauseUI; //ポーズ時に表示されるUI

	//ButtonインスペクターのOnClick()にButtonを追加→Buttonをアタッチ→実行させたい関数を指定

	public void OnClick()
	{
		//ポーズUIのアクティブ、非アクティブを切り替え
		PauseUI.SetActive(!PauseUI.activeSelf);

		//　ポーズUIが表示されてる時は停止
		if (PauseUI.activeSelf)
		{
			Time.timeScale = 0f;
		}
		//　ポーズUIが表示されてなければ通常通り進行
		else
		{
			Time.timeScale = 1f;
		}
	}
}
