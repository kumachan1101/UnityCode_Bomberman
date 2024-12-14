using Photon.Pun;
using UnityEngine.UI;
using UnityEngine;

public class PowerGage : MonoBehaviourPunCallbacks
{
	protected Slider cSlider;

	void Awake() {
		cSlider = GetComponent<Slider>();
		int iPower = GameObject.Find("Field").GetComponent<Field_Player_Base>().GetPower();
		cSlider.maxValue = iPower;
		cSlider.value = iPower;

	}
	public void SetDamage(int iDamage){
		SetDamage_RPC(iDamage);
	}

	protected virtual void SetDamage_RPC(int iDamage){}

	[PunRPC]
	public void SyncSetDamage(int iDamage){
		cSlider.value -= iDamage;
	}

	public void HeartUp(int iHeart){
		HeartUp_RPC(iHeart);
	}

	protected virtual void HeartUp_RPC(int iHeart){}

	[PunRPC]
	public void SyncHeartUp(int iHeart){
		cSlider.value += iHeart;
	}

	public bool IsDead(){
		if(cSlider.value <= 0){
			return true;
		}
		return false;
	}

	public void SetSliderPos(int iPlayerPos){
		RectTransform sliderRectTransform = cSlider.GetComponent<RectTransform>(); // SliderのRectTransformを取得します。
		Vector3 newPosition = sliderRectTransform.anchoredPosition; // ローカル座標
		newPosition.y = newPosition.y - (iPlayerPos - 1) * 20;
		sliderRectTransform.anchoredPosition = newPosition; // 新しい座標を設定します。
	}

	public void SetSliderColor(Color sliderColor){
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
}


