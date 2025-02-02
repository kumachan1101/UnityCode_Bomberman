using UnityEngine;
using UnityEngine.UI;
public class Field_Player_Random : Field_Player_CpuMode {

    protected Vector3[] v3PlayerPos100;

	public void Start()
	{
		int playercnt = GetArrayLength(GetIndex());
		// SetPlayerCntで毎回人数を設定することでスライダーの座標位置を変えている。。
        for (int i = 1; i <= playercnt; i++)
        {
            if(i == 1){
                SpawnPlayerObjects(i);
                SetPlayerName("Player1");
            }
            else{
                SpawnPlayerObjects(Random.Range(2, 5));
            }
            SetPlayerCnt(i); 
		}
	}
    public override int GetIndex(){
        // Field100のGameObjectを読みだす。100というのは、なるべく大きめの重複しない値を指定しただけ
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


    public override void SetPower(Slider cSlider){
        cSlider.maxValue = 10;
		cSlider.value = 10;
    }
    public override int GetPower(){
        return 10;
    }

}