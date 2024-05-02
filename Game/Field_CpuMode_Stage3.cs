using UnityEngine;
using UnityEngine.UI;
public class Field_CpuMode_Stage3 : Field_CpuMode_Stage2 {


    protected Vector3[] v3PlayerPos3 = new Vector3[]
    {
        new Vector3(1, 1, 1),
        new Vector3(GameManager.xmax-2, 1, GameManager.zmax-2),
        new Vector3(GameManager.xmax/2, 1, GameManager.zmax-2),
        new Vector3(GameManager.xmax-2, 1, GameManager.zmax/2),
        new Vector3(1, 1, GameManager.zmax-2),
        new Vector3(1, 1, GameManager.zmax/2),
        new Vector3(GameManager.xmax-2, 1, 1),
        new Vector3(GameManager.xmax/2, 1, 1)
    };


    protected override void SetPower(Slider cSlider){
        cSlider.value = 15;
    }
    protected override int GetIndex(){
        return 3;
    }


}