using UnityEngine.UI;
using UnityEngine;

public class PowerGage_Tower : PowerGage
{

	void Awake() {
		cSlider = GetComponent<Slider>();
		int iPower = GameObject.Find("Field").GetComponent<Field_Block_Tower>().GetPower();
		cSlider.maxValue = iPower;
		cSlider.value = iPower;
		ChangeSliderLength(200f);

	}

}


