using UnityEngine;
using UnityEngine.UI;
public class Field_CpuMode_Stage2 : Field_CpuMode_Stage1 {


    protected Vector3[] v3PlayerPos2 = new Vector3[]
    {
        new Vector3(1, 1, 1),
        new Vector3(GameManager.xmax-2, 1, GameManager.zmax-2),
        new Vector3(1, 1, GameManager.zmax-2),
        new Vector3(GameManager.xmax-2, 1, 1)
    };

    protected override int GetIndex(){
        return 2;
    }

	protected override void SpawnPlayerObjects(int playerCount)
	{
		m_playerCount = playerCount;
		for (int i = 1; i <= playerCount; i++)
		{
			string canvasName = "";
			string playerName = "";
			
			if (i == 1)
			{
				GetPlayerInfo(ref canvasName, ref playerName);
			}
			else
			{
				GetCPUPlayerInfo(ref canvasName, ref playerName);
			}

			GameObject gPlayer = LoadResource(playerName);
			GameObject gCanvas = LoadResource(canvasName);
			gCanvas.transform.position = new Vector3(0, 0, 0);

			if (i != 1)
			{
				Slider cSlider = gCanvas.GetComponentInChildren<Slider>(); // Canvasの子要素からSliderを取得します。
				SetPower(cSlider);
			}

			SetupPlayer(gPlayer, i, gCanvas);
		}
	}


	protected override void GetCPUPlayerInfo(ref string canvasName, ref string playerName){
		canvasName = "Canvas2";
		playerName = "Player2";
	}
   protected override void SetPower(Slider cSlider){
        cSlider.maxValue = 10;
		cSlider.value = 10;
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
                    return "BomMaterial2";
                }
                else{
                    return "BomMaterial1";
                }
            }
        }

        return "InvalidMaterial";
    }

 


}