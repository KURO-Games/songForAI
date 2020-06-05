using System.Collections;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
	[SerializeField]
	private GameObject PauseUI;

	// Start is called before the first frame update
	public void OnClick()
	{
		PauseUI.SetActive(!PauseUI.activeSelf);

		//　ポーズUIが表示されてる時は停止
		if (PauseUI.activeSelf)
		{
			Time.timeScale = 0f;
			//　ポーズUIが表示されてなければ通常通り進行
		}
		else
		{
			Time.timeScale = 1f;
		}
	}
}
