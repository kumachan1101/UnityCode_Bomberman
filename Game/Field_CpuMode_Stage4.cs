using UnityEngine;
using UnityEngine.UI;
public class Field_CpuMode_Stage4 : Field_CpuMode_Stage3 {


    protected Vector3[] v3PlayerPos4 = new Vector3[]
    {
        new Vector3(1, 1, 1),
        new Vector3(GameManager.xmax-2, 1, GameManager.zmax-2),
        new Vector3(GameManager.xmax/2, 1, GameManager.zmax-2),
        new Vector3(GameManager.xmax-2, 1, GameManager.zmax/2),
        new Vector3(1, 1, GameManager.zmax-2),
        new Vector3(1, 1, GameManager.zmax/2),
        new Vector3(GameManager.xmax-2, 1, 1),
        new Vector3(GameManager.xmax/2, 1, 1),
        new Vector3(1, 1, 1),
        new Vector3(GameManager.xmax-2, 1, GameManager.zmax-2),
        new Vector3(GameManager.xmax/2, 1, GameManager.zmax-2),
        new Vector3(GameManager.xmax-2, 1, GameManager.zmax/2),
        new Vector3(1, 1, GameManager.zmax-2),
        new Vector3(1, 1, GameManager.zmax/2),
        new Vector3(GameManager.xmax-2, 1, 1),
        new Vector3(GameManager.xmax/2, 1, 1)
    };

    protected override int GetIndex(){
        return 4;
    }

    protected override void GetCPUPlayerInfo(ref string canvasName, ref string playerName){
		canvasName = "Canvas4";
		playerName = "Player4";
    }

    protected override void SetPower(Slider cSlider){
        cSlider.maxValue = 20;
		cSlider.value = 20;
    }

    protected override string GetBomMaterial(Vector3 target, int index)
    {
        target.y += 1;

        // v3PlayerPosの各要素と比較
        for (int i = 0; i < GetArrayLength(index); i++)
        {
            if (GetPlayerPosition(index,i) == target)
            {
                // 一致する要素が見つかった場合、該当する文字列を返す
                if(0 != i){
                    return "BomMaterial4";
                }
                else{
                    return "BomMaterial1";
                }
            }
        }

        return "InvalidMaterial";
    }


}