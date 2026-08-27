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
		//iPlayerNo_before = iPlayerCnt;
		iPlayerCnt = iCnt;
	}
	public void SetPlayerNo(int iNo){
		iPlayerNo = iNo;
	}
	public int GetPlayerNo(){
		return iPlayerNo;
	}
	public int GetPlayerPosition(){
		return iPlayerCnt;
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
		iPlayerCnt = iPlayerPos;
		ResponsiveGameUiController.LayoutPowerGauge(this);
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

