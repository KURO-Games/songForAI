using UnityEngine;
using System.Collections;

public class LogoFadeInOutSequence : MonoBehaviour {

	//あなたのチームのロゴ画像
	public Texture2D yourLogoTexture;
	//ADX2 LEのロゴ画像
	public Texture2D ADX2LE_LogoTexture;

	//背景テクスチャ(白または黒一色)
	private Texture2D bgTexture;
	//表示するテクスチャ
	private Texture2D logoTexture;

	//背景画像の白・黒切り替え
	enum BgColorType{
		white,
		black
	}
	//背景画像をは白一色か、黒一色か
	[SerializeField]
	private BgColorType bgColorType = BgColorType.white;

	//ロゴ画像の倍率
	public float logoSize = 1f;

	//フェードイン・フェードアウトに何秒かけるか
	public float fadeTime = 1.0f;
	//ロゴを静止何秒表示するか
	public float logoKeepTime = 2.0f;
	//ロゴフェードアウト後、背景画像をフェードアウトするか即終了するか
	public bool gameSceneFadeIn = false;
	//クリックやキー入力でロゴをスキップするか
	public bool isSkipEnable = true;

	//アルファ値
	private float alpha = 0f;
	//背景画像のアルファ値
	private float bgAlpha = 1f;

	//ロゴ画像のサイズ
	[SerializeField]
	private Vector2 logoTextureSize;

	//フェード終了シーケンスを受け取るGameObject
	public GameObject sequenceEndReceiveObject;

	//フェードイン・アウトの状態遷移管理
	enum Type {
		stop = -1,
		start,//処理開始
		fadeIn,
		still,//画像静止
		fadeOut,
		bgFadeOut//背景画像のフェードアウト
	}

	//フェード処理の遷移状態、自動スタートしない場合はType.stopにセット
	[SerializeField]
	private Type type = Type.start;

	// Use this for initialization
	void Start () {

		//１ピクセルのテクスチャを作る
		bgTexture = new Texture2D(1,1,TextureFormat.RGB24,false);

		//白か黒に塗る
		switch (bgColorType)
		{
			//白
			case BgColorType.white:
				bgTexture.SetPixel(0,0,Color.white);
				break;
			//黒
			case BgColorType.black:
				bgTexture.SetPixel(0,0,Color.black);
				break;
		}

		//テクスチャに適用
		bgTexture.Apply();

		//ロゴ画像の有無を確認
		if(!yourLogoTexture){
	    	Debug.LogError("Not Found Logo Texture");
		}

		if(!ADX2LE_LogoTexture){
	    	Debug.LogError("Not Found ADX2LE Logo Texture");
		}
	}
	
	// Update is called once per frame
	void Update () {

		//クリックorボタン入力orタッチで即終了
		if ((isSkipEnable)&&(Input.anyKey)){
        	//完了報告、ゲームオブジェクトの破棄
			OnSequenceEnd();
        }

		//Update内の処理分岐
		switch(type)
		{
			//何もしない(非表示、待機)
			case Type.stop:
				break;
			//タイマーを起動
			case Type.start:
				//bgTexture.SetPixel(0, 0, new Color(1f, 1f, 1f, alpha));
        		//bgTexture.Apply();
				StartCoroutine("Timer");
				break;
			//フェードイン中
			case Type.fadeIn:
				FadeIn();
				break;
			//静止中
			case Type.still:
				alpha = 1f;
				break;
			//フェードアウト中
			case Type.fadeOut:
				FadeOut();
				break;
			case Type.bgFadeOut:
				BgFadeOut();
				break;
		}

		
	}

	void OnGUI () {
		//停止状態以外は画像を表示する
		if (type != Type.stop){
			//黒テクスチャを画面サイズに引き伸ばして表示
			GUI.color = new Color(1,1,1,bgAlpha);
	    	GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), bgTexture);
	    	//その上からロゴ画像を表示

	    	if(logoTexture != null){
	    		GUI.color = new Color(1,1,1,alpha);
	    		GUI.DrawTexture(new Rect(
	    			Screen.width/2-(logoTextureSize.x*logoSize/2), 
	    			Screen.height/2-(logoTextureSize.y*logoSize/2),
	    			 logoTextureSize.x*logoSize,
	    			 logoTextureSize.y*logoSize
	    			 ),
	    		logoTexture);
	    	}
		}
	}

	//Update内の処理を切り替えるタイマー
	IEnumerator Timer()
	{
		//あなたのチームのロゴを表示
		logoTexture = yourLogoTexture;
		//あなたのチームのロゴサイズを取得
		logoTextureSize = new Vector2 (yourLogoTexture.width,yourLogoTexture.height);
		yield return new WaitForSeconds(1f);
		type = Type.fadeIn;
		yield return new WaitForSeconds(fadeTime);
		type = Type.still;
		yield return new WaitForSeconds(logoKeepTime);
		type = Type.fadeOut;
		yield return new WaitForSeconds(fadeTime);		

		//ADX2 LEのロゴを表示
		logoTexture = ADX2LE_LogoTexture;
		//ADX2 LEののロゴサイズを取得
		logoTextureSize = new Vector2 (ADX2LE_LogoTexture.width,ADX2LE_LogoTexture.height);
		//アルファ値をリセット
		alpha = 0f;
		yield return new WaitForSeconds(1f);
		type = Type.fadeIn;
		yield return new WaitForSeconds(fadeTime);
		type = Type.still;
		yield return new WaitForSeconds(logoKeepTime);
		type = Type.fadeOut;
		yield return new WaitForSeconds(fadeTime);

		yield return new WaitForSeconds(1f);

		//背景画像のフェードをする場合は実行
		if (gameSceneFadeIn){
			type = Type.bgFadeOut;
			yield return new WaitForSeconds(fadeTime);
		}

		//完了報告、ゲームオブジェクトの破棄
		OnSequenceEnd();

	}

	void FadeIn()
	{
		alpha += Time.deltaTime * (1/fadeTime);
	}

	void FadeOut()
	{
		alpha -= Time.deltaTime * (1/fadeTime);
	}

	//背景画像のフェードアウト
	void BgFadeOut()
	{
		bgAlpha -= Time.deltaTime * (1/fadeTime);
	}

	//外から呼ばれる用
	public void StartLogoSequence()
	{
		//停止状態のときのみ処理をスタートさせる
		if(type == Type.stop)
			type = Type.start;
	}

	void OnSequenceEnd()
	{
		//外から呼ばれたときの完了通知
		if(sequenceEndReceiveObject != null)
			sequenceEndReceiveObject.SendMessage("OnLogoSequenceEnd");
		SceneLoadManager.LoadScene("Title");

		//自分を破棄する
		Destroy(this.gameObject);

	}

}
