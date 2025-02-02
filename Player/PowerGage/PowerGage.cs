using UnityEngine.UI;
using UnityEngine;

public class PowerGage : MonoBehaviour
{
	protected Slider cSlider;

	void Awake() {
		cSlider = GetComponent<Slider>();
		int iPower = GameObject.Find("Field").GetComponent<Field_Player_Base>().GetPower();
		cSlider.maxValue = iPower;
		cSlider.value = iPower;
		ChangeSliderLength(50f);

	}

	public void SetDamage(int iDamage){
		cSlider.value -= iDamage;
	}

	public void HeartUp(int iHeart){
		cSlider.value += iHeart;
	}
	public bool IsHeartUp(){
		if(cSlider.value == cSlider.maxValue){
			return false;
		}
		return true;
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

    // スライダーの長さを変更するメソッド
    public void ChangeSliderLength(float newLength)
    {
        if (cSlider != null)
        {
            RectTransform rectTransform = cSlider.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(newLength, rectTransform.sizeDelta.y);
            }
            else
            {
                Debug.LogError("RectTransformが見つかりません。スライダーが正しく設定されているか確認してください。");
            }
        }
        else
        {
            Debug.LogError("スライダーが割り当てられていません。");
        }
    }

}


