using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class PowerGage_Slider : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
	protected Slider cSlider;
	private int iPlayerCnt;
	private int iPlayerNo;
	private int iPlayerNo_before;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        iPlayerCnt = (int)data[0];
        iPlayerNo = (int)data[1];
    }

	public void SetPlayerCnt(int iCnt){
		iPlayerNo_before = iPlayerCnt;
		iPlayerCnt = iCnt;
	}
	public void SetPlayerNo(int iNo){
		iPlayerNo = iNo;
	}
	public int GetPlayerNo(){
		return iPlayerNo;
	}


	void Awake(){
	}
	void Start(){
		init();
	}
	void Update() {
		if(iPlayerNo_before == iPlayerCnt){
			return;
		}
		iPlayerNo_before = iPlayerCnt;
		SetSliderPos(iPlayerCnt);
	}
	private void init()
	{
		cSlider = transform.Find("Slider").GetComponent<Slider>();
		SetSliderPos(iPlayerCnt); //プレイヤーの位置設定では、現在のプレイヤー人数の合計値を設定

		Color cColor = GetColor(iPlayerNo);
		SetSliderColor(cColor);
	}

	public virtual void SetSliderPos(int iPlayerPos){
		RectTransform sliderRectTransform = cSlider.GetComponent<RectTransform>(); // SliderのRectTransformを取得します。
		Vector3 newPosition = sliderRectTransform.anchoredPosition; // ローカル座標
		// 横の位置を計算 (X個ごとにX座標をずらす)
		int column = (iPlayerPos - 1) % 30; // X個ごとに新しい列を作成
		int row = (iPlayerPos - 1) / 30; // 1列にX個が並んだら、次の行に移動

		// Y座標: 同じ行の中で、X個目ごとにY座標を変えず、X座標を変更
		newPosition.y = 200 - column * 12; // X個まで縦に並べる
		// X座標: 5個ごとにずらす
		newPosition.x = row * 55 - 400; // 1行に並ぶ間隔、必要に応じて調整

		sliderRectTransform.anchoredPosition = newPosition; // 新しい座標を設定します。
	}

	private void SetSliderColor(Color sliderColor){
        // スライダーの色を変更
        Image fillImage = cSlider.fillRect.GetComponent<Image>(); // Fill部分のImageコンポーネントを取得します。
        if (fillImage != null)
        {
            fillImage.color = sliderColor; // Fill部分の色を設定します。
        }

        // 必要に応じてBackgroundの色も変更
        Image backgroundImage = cSlider.GetComponentInChildren<Image>(); // 子要素のImageを取得
        if (backgroundImage != null)
        {
            backgroundImage.color = Color.gray; // 背景色を設定します（例: グレー）。
        }
	}

	private Color GetColor(int value)
	{
		switch (value)
		{
			case 1:
				return Color.red; // 赤
			case 2:
				return Color.blue; // 青
			case 3:
				return Color.green; // 緑
			case 4:
				return Color.yellow; // 黄色
			default:
				return Color.white; // デフォルト値（白）
		}
	}

}


