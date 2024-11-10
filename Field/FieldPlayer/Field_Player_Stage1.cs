using UnityEngine;
using UnityEngine.UI;
public class Field_Player_Stage1 : Field_Player_CpuMode {

    protected Vector3[] v3PlayerPos1;
    public override int GetIndex(){
        return 1;
    }

    public override void SetPlayerPositions()
    {
        // GameManager.xmax と GameManager.zmax の値がここで取得可能であると仮定
        int xmax = GameManager.xmax;
        int zmax = GameManager.zmax;

        v3PlayerPos1 = new Vector3[]
        {
            new Vector3(2, 0.5f, 2),
            new Vector3(xmax - 3, 0.5f, zmax - 3),
            new Vector3(2, 0.5f, zmax - 3),
            new Vector3(xmax - 3, 0.5f, 2)
        };
    }


    protected override void GetPlayerNames(int iPlayerNo, ref string canvasName, ref string playerName)
    {
        canvasName = "Canvas" + iPlayerNo;
        playerName = "Player" + iPlayerNo;
    }

    protected override void SetPower(Slider cSlider){
        cSlider.maxValue = 10;
		cSlider.value = 10;
    }


    public override string GetBomMaterial(Vector3 target, int index)
    {
        target.y += 1;

        // v3PlayerPosの各要素と比較
        for (int i = 0; i < GetArrayLength(index); i++)
        {
            if (GetPlayerPosition(index,i) == target)
            {
                // 一致する要素が見つかった場合、該当する文字列を返す
                return "BomMaterial" + (i + 1);
            }
        }

        return "InvalidMaterial";
    }




}