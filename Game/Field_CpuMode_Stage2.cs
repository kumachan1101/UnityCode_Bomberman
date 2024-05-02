using UnityEngine;
using UnityEngine.UI;
public class Field_CpuMode_Stage2 : Field_CpuMode {


    protected Vector3[] v3PlayerPos2 = new Vector3[]
    {
        new Vector3(1, 1, 1),
        new Vector3(GameManager.xmax-2, 1, GameManager.zmax-2),
        new Vector3(1, 1, GameManager.zmax-2),
        new Vector3(GameManager.xmax-2, 1, 1)
    };


    protected override void SpawnPlayerObjects(int playerCount)
    {
        m_playerCount = playerCount;
        for (int i = 1; i <= playerCount; i++)
        {
            string canvasName;
            string playerName;
            GameObject gPlayer;
            Player cPlayer;


            if (i == 1)
            {
                canvasName = "Canvas1";
                playerName = "Player1";
                gPlayer = LoadResource(playerName);
                cPlayer = gPlayer.GetComponent<Player>();
            }
            else
            {
                canvasName = "Canvas2";
                playerName = "Player2";
                gPlayer = LoadResource(playerName);
                cPlayer = gPlayer.GetComponent<Player_CpuMode>();
            }

            GameObject gCanvas = LoadResource(canvasName);
            Vector3 v3PwrGage = new Vector3(0, 0, 0);
            gCanvas.transform.position = v3PwrGage;

            if (i == 1)
            {
            }
            else
            {
                Slider cSlider = gCanvas.GetComponentInChildren<Slider>(); // Canvasの子要素からSliderを取得します。
                SetPower(cSlider);
            }

            SetupPlayer(gPlayer, i, gCanvas);
        }
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

    protected virtual void SetPower(Slider cSlider){
        cSlider.value = 10;
    }

    protected override int GetIndex(){
        return 2;
    }


}