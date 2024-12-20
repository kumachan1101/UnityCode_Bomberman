using UnityEngine;
using UnityEngine.UI;
public class Field_Player_Stage4 : Field_Player_Stage2 {


    protected Vector3[] v3PlayerPos4;

    public override int GetIndex(){
        return 4;
    }

    public override void SetPlayerPositions()
    {
        // GameManager.xmax と GameManager.zmax の値がここで取得可能であると仮定
        int xmax = GameManager.xmax;
        int zmax = GameManager.zmax;

        v3PlayerPos4 = new Vector3[]
        {
			new Vector3(2, 0.5f, 2),
			new Vector3(GameManager.xmax-3, 0.5f, GameManager.zmax-3),
			new Vector3(GameManager.xmax/2, 0.5f, GameManager.zmax-3),
			new Vector3(GameManager.xmax-3, 0.5f, GameManager.zmax/2),
			new Vector3(2, 0.5f, GameManager.zmax-3),
			new Vector3(2, 0.5f, GameManager.zmax/2),
			new Vector3(GameManager.xmax-3, 0.5f, 2),
			new Vector3(GameManager.xmax/2, 0.5f, 2),
			new Vector3(2, 0.5f, 2),
			new Vector3(GameManager.xmax-3, 0.5f, GameManager.zmax-3),
			new Vector3(GameManager.xmax/2, 0.5f, GameManager.zmax-3),
			new Vector3(GameManager.xmax-3, 0.5f, GameManager.zmax/2),
			new Vector3(2, 0.5f, GameManager.zmax-3),
			new Vector3(2, 0.5f, GameManager.zmax/2),
			new Vector3(GameManager.xmax-3, 0.5f, 2),
			new Vector3(GameManager.xmax/2, 0.5f, 2)
        };
    }


    protected override void GetCPUPlayerInfo(ref string canvasName, ref string playerName){
		canvasName = "Canvas4";
		playerName = "Player4";
    }

    public override void SetPower(Slider cSlider){
        cSlider.maxValue = 16;
		cSlider.value = 16;
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