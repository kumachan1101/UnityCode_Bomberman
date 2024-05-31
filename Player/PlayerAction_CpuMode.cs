using UnityEngine;

namespace PlayerActionName{
    public class PlayerAction_CpuMode : PlayerAction
    {
        private float timer; 

        private int randomDirection;
        private float changeDirectionInterval = 3f; // 向きを変える間隔

        public PlayerAction_CpuMode(ref Rigidbody rb, ref Transform tf, ref Animator ani, ref Field_Base fi, int iViewID) : base(ref rb, ref tf, ref ani, ref fi, iViewID) {
            // サブクラス固有の初期化処理
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
            Move(Vector3.forward, 0); // 上方向への移動と回転角度0度を指定
        }

        public override void MoveDown()
        {
            Move(-Vector3.forward, 180); // 下方向への移動と回転角度180度を指定
        }

        public override void MoveLeft()
        {
            Move(-Vector3.right, 270); // 左方向への移動と回転角度270度を指定
        }

        public override void MoveRight()
        {
            Move(Vector3.right, 90); // 右方向への移動と回転角度90度を指定
        }

    }


}