using UnityEngine;
using UnityEngine.UI;
public class Field_CpuMode_Stage1 : Field_CpuMode {

    protected Vector3[] v3PlayerPos1 = new Vector3[]
    {
        new Vector3(1, 1, 1),
        new Vector3(GameManager.xmax-2, 1, GameManager.zmax-2),
        new Vector3(1, 1, GameManager.zmax-2),
        new Vector3(GameManager.xmax-2, 1, 1)
    };

    protected override int GetIndex(){
        return 1;
    }

    protected override void SpawnPlayerObjects(int playerCount)
    {
        m_playerCount = playerCount;
        for (int i = 1; i <= playerCount; i++)
        {
            string canvasName = "Canvas" + i;
            GameObject gCanvas = LoadResource(canvasName);
            Vector3 v3PwrGage = new Vector3(0, 0, 0);
            gCanvas.transform.position = v3PwrGage;

            if (i != 1)
            {
                Slider cSlider = gCanvas.GetComponentInChildren<Slider>(); // Canvasの子要素からSliderを取得します。
                SetPower(cSlider);
            }

            string PlayerName = "Player" + i;
            GameObject gPlayer = LoadResource(PlayerName);
            SetupPlayer(gPlayer, i, gCanvas);
        }
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
                return "BomMaterial" + (i + 1);
            }
        }

        return "InvalidMaterial";
    }






}