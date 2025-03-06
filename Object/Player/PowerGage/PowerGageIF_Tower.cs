using UnityEngine;
using Photon.Pun;

public class PowerGageIF_Tower : PowerGageIF_CpuMode
{

    protected override PowerGage CreatePowerGage(GameObject sliderObject)
    {
        return sliderObject.AddComponent<PowerGage_Tower>();
    }

	protected override Component GetDestroyTarget() => GetComponent<Tower>();


}


