using System.Collections;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
	[SerializeField] 
	private GameObject PauseUI; //ポーズ時に表示されるUI

	public void OnClick()
	{
		//ポーズUIのアクティブ、非アクティブを切り替え
		PauseUI.SetActive(!PauseUI.activeSelf);

		//　ポーズUIが表示されてる時は停止
		if (PauseUI.activeSelf)
		{
			//Cursor.lockState = CursorLockMode.Locked;
			//{
			//	OnClick();
			//}

			Time.timeScale = 0f;
		}
		//　ポーズUIが表示されてなければ通常通り進行
		else
		{
			Time.timeScale = 1f;
		}
	}
}
