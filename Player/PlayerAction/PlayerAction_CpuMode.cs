using UnityEngine;

public class PlayerAction_CpuMode : PlayerAction
{
	private float timer; 

	private int randomDirection;
	private float changeDirectionInterval = 3f; // 向きを変える間隔

	public PlayerAction_CpuMode(ref Rigidbody rb, ref Transform tf) : base(ref rb, ref tf) {
		// サブクラス固有の初期化処理
	}


	protected override void CanMove()
	{
		if (cLibrary == null)
		{
			cLibrary = GameObject.Find("Library").GetComponent<Library_Base>();
		}

		Vector3 currentPos = Library_Base.GetPos(myTransform.position);
		Vector3 forwardDirection = myTransform.forward; // 進行方向を取得
		Vector3 nextPos = currentPos + forwardDirection; // 進もうとしている位置

		// 現在位置の地面に爆風があるかどうかではなく、進行先の地面に爆風があるかをチェック
		Vector3 nextGroundPos = new Vector3(nextPos.x, nextPos.y - 1, nextPos.z);
		canMove = cField.IsMatch(nextGroundPos, playerMaterial.GetMaterial());

		// 範囲外に出ようとしていないか確認
		if (Library_Base.IsPositionOutOfBounds(nextPos))
		{
			canMove = false;
		}

		// 移動可能なら進む、移動不可なら元の位置に戻す
		if (canMove)
		{
			LastV3 = myTransform.position;
		}
		else
		{
			myTransform.position = LastV3; // 元の位置に戻す
		}
	}

	protected override void UpdatePlayerMovement()
	{
		timer -= Time.deltaTime;
		if (timer <= 0f){
			timer = changeDirectionInterval;
			randomDirection = UnityEngine.Random.Range(0, 4); // 0: Up, 1: Down, 2: Left, 3: Right
		}

		switch (randomDirection)
		{
			case 0:
				MoveUp();
				break;
			case 1:
				MoveDown();
				break;
			case 2:
				MoveLeft();
				break;
			case 3:
				MoveRight();
				break;
			default:
				break;
		}

	}


	public override void MoveUp()
	{
		//playerMovement.Move(Vector3.forward, 0); // 上方向への移動と回転角度0度を指定
		playerMovement.Move(Vector3.forward); // 上方向への移動と回転角度0度を指定
	}

	public override void MoveDown()
	{
		//playerMovement.Move(-Vector3.forward, 180); // 下方向への移動と回転角度180度を指定
		playerMovement.Move(-Vector3.forward); // 下方向への移動と回転角度180度を指定
	}

	public override void MoveLeft()
	{
		//playerMovement.Move(-Vector3.right, 270); // 左方向への移動と回転角度270度を指定
		playerMovement.Move(-Vector3.right); // 左方向への移動と回転角度270度を指定
	}

	public override void MoveRight()
	{
		//playerMovement.Move(Vector3.right, 90); // 右方向への移動と回転角度90度を指定
		playerMovement.Move(Vector3.right); // 右方向への移動と回転角度90度を指定
	}

}

