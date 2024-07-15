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
    public override void SpawnPlayerObjects(int playercnt)
    {
        //m_playerCount = GetArrayLength(GetIndex());;
		SetPlayerNum(playercnt-1);
        for (int i = 1; i <= playercnt; i++)
        {
            string canvasName = "Canvas" + i;
            GameObject gCanvas = LoadResource(canvasName);
            Vector3 v3PwrGage = new Vector3(0, 0, 0);
            gCanvas.transform.position = v3PwrGage;

            string PlayerName = "Player" + i;

            if (i != 1){
                Slider cSlider = gCanvas.GetComponentInChildren<Slider>(); // Canvasの子要素からSliderを取得します。
                SetPower(cSlider);
            }
            GameObject gPlayer = LoadResource(PlayerName);
            SetupPlayer(gPlayer, i, gCanvas);
        }
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