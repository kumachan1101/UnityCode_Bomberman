using System.Collections.Generic;
using UnityEngine;

public class PlayerAction_CpuMode : PlayerAction
{
	private float timer; 
	private int randomDirection;
	private float changeDirectionInterval = 3f; // 向きを変える間隔
	private Dictionary<int, Vector3> randomMovementBindings;
    private Field_Block_Base cField;
    protected Material cMaterial;
    private float bombCooldown = 3f; // 爆弾を置くクールダウン時間
    private float bombTimer = 0f; // 次に爆弾を置けるまでの時間
	protected override void InitDiff() {
        cField = GameObject.Find("Field").GetComponent<Field_Block_Base>();
        MaterialManager materialManager = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
		string MaterialType = materialManager.GetBomMaterialByPlayerName(this.gameObject.name);
        cMaterial = materialManager.GetMaterialOfType(MaterialType);

	}

    protected override void InitializeBindings()
    {
        // ランダム方向の動作を設定
        randomMovementBindings = new Dictionary<int, Vector3>
        {
            { 0, Vector3.forward }, // Up
            { 1, Vector3.back },    // Down
            { 2, Vector3.left },    // Left
            { 3, Vector3.right }    // Right
        };
    }


	protected override void UpdatePlayerMovement()
	{
        // タイマーを減少させ、ランダムな方向を選択
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = changeDirectionInterval;
            randomDirection = Random.Range(0, randomMovementBindings.Count);
        }

        // ランダム方向に基づいて動作
        if (randomMovementBindings.TryGetValue(randomDirection, out var direction))
        {
            if(CanMove(direction)){
                PerformPlayerAction(direction);
            }
        }
        
	}


    protected override void UpdatePlayerActions()
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
        bombTimer = Random.Range(1f, bombCooldown);
    }

	private bool CanMove(Vector3 forwardDirection)
	{
		Vector3 currentPos = Library_Base.GetPos(playerMovement.GetCurrentPos());
		Vector3 nextPos = currentPos + forwardDirection; // 進もうとしている位置

		// 現在位置の地面に爆風があるかどうかではなく、進行先の地面に爆風があるかをチェック
		Vector3 nextGroundPos = new Vector3(nextPos.x, nextPos.y - 1, nextPos.z);
        bool canMove = cField.IsMatch(nextGroundPos, cMaterial);

		// 範囲外に出ようとしていないか確認
		if (Library_Base.IsPositionOutOfBounds(nextPos))
		{
			canMove = false;
		}
        return canMove;
	}
}

