using UnityEngine;
public class Player_CpuMode : Player_Base
{

    private float bombCooldown = 3f; // 爆弾を置くクールダウン時間
    private float bombTimer = 0f; // 次に爆弾を置けるまでの時間

    public override void UpdateKey()
    {
        bombTimer -= Time.deltaTime; // タイマーを進める

        // タイマーが0以下になったら爆弾を置ける
        if (bombTimer <= 0f)
        {
            DropBom(); // 爆弾を設置
            ResetBombTimer(); // タイマーをリセット
        }
    }

    // 爆弾を置くタイミングをランダムにする
    private void ResetBombTimer()
    {
        // 次に爆弾を置けるまでの時間をランダムに設定（例：1秒〜5秒）
        bombTimer = UnityEngine.Random.Range(1f, bombCooldown);
    }
    protected override void CreatePlayerAction(){
        cPlayerAction = new PlayerAction_CpuMode(ref rigidBody, ref myTransform);
    }


    protected override bool IsAvairable(){
        if(iViewID == -1){
            return false;
        }
        return true;
    }

	protected override void DestroySync(GameObject g){
		Destroy(g);
	}

}
