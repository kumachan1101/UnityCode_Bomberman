using UnityEngine.UI;
using UnityEngine;

public class PowerGage : MonoBehaviour
{
	protected Slider cSlider;

	void Awake() {
		cSlider = transform.Find("Slider").GetComponent<Slider>();
		//cSlider = GetComponent<Slider>();
		int iPower = GameObject.Find("Field").GetComponent<PlayerPowerManager>().GetPower();
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


