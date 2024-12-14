using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class PowerGage_Slider : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
	private Slider cSlider;
	private int iPlayerCnt;
	private int iPlayerNo;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        iPlayerCnt = (int)data[0];
        iPlayerNo = (int)data[1];
    }

	public void SetPlayerCnt(int iCnt){
		iPlayerCnt = iCnt;
	}
	public void SetPlayerNo(int iNo){
		iPlayerNo = iNo;
	}


	void Awake(){
	}
	void Start(){
		init();
	}
	private void init()
	{
		cSlider = transform.Find("Slider").GetComponent<Slider>();
		SetSliderPos(iPlayerCnt); //プレイヤーの位置設定では、現在のプレイヤー人数の合計値を設定

		Color cColor = GetColor(iPlayerNo);
		SetSliderColor(cColor);
	}

	private void SetSliderPos(int iPlayerPos){
		RectTransform sliderRectTransform = cSlider.GetComponent<RectTransform>(); // SliderのRectTransformを取得します。
		Vector3 newPosition = sliderRectTransform.anchoredPosition; // ローカル座標
		newPosition.y = newPosition.y - (iPlayerPos - 1) * 20;
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


