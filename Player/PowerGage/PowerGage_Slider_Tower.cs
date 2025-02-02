using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class PowerGage_Slider_Tower : PowerGage_Slider
{

	public override void SetSliderPos(int iPlayerPos){
		RectTransform sliderRectTransform = cSlider.GetComponent<RectTransform>(); // SliderのRectTransformを取得します。
		Vector3 newPosition = sliderRectTransform.anchoredPosition; // ローカル座標
		newPosition.x = 400;
		newPosition.y = 200;
		newPosition.y = newPosition.y - (iPlayerPos - 1) * 12;
		sliderRectTransform.anchoredPosition = newPosition; // 新しい座標を設定します。
	}

}


