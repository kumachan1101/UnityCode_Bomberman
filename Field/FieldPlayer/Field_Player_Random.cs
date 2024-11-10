using UnityEngine;
using UnityEngine.UI;
public class Field_Player_Random : Field_Player_CpuMode {

    protected Vector3[] v3PlayerPos100;

    public override int GetIndex(){
        return 100;
    }


    public override void SetPlayerPositions()
    {
        int xmax = GameManager.xmax;
        int zmax = GameManager.zmax;

        // プレイヤー数を4以上のランダムな値に設定（例: 4〜10人）
        int playerCount = Random.Range(4, 20);

        // プレイヤー位置のリストを初期化
        v3PlayerPos100 = new Vector3[playerCount];

        for (int i = 0; i < playerCount; i++)
        {
            Vector3 randomPosition;

            // 他のプレイヤーと被らないようにランダムな位置を選定
            do
            {
                randomPosition = new Vector3(
                    Random.Range(3, xmax - 3), // フィールドの端を避ける
                    0.5f,
                    Random.Range(3, zmax - 3)
                );
            } while (IsPositionOccupied(randomPosition));

            v3PlayerPos100[i] = randomPosition;
        }
    }


	// 他のプレイヤー位置と重複しないかチェックする関数
	private bool IsPositionOccupied(Vector3 position)
	{
	    foreach (Vector3 existingPosition in v3PlayerPos100)
	    {
	        if (existingPosition == position)
	        {
	            return true;
	        }
	    }
	    return false;
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

}