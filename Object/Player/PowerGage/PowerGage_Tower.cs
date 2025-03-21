using UnityEngine.UI;
using UnityEngine;

public class PowerGage_Tower : PowerGage
{

	void Awake() {
		//cSlider = GetComponent<Slider>();
		cSlider = transform.Find("Slider").GetComponent<Slider>();
		int iPower = GameObject.Find("Field").GetComponent<TowerSpawnManager>().GetPower();
		cSlider.maxValue = iPower;
		cSlider.value = iPower;
		ChangeSliderLength(200f);

	}

}


