using UnityEngine;
using PlayerBomName;

namespace PlayerActionName{
    public class PlayerAction
    {
        protected Vector3 LastV3;

        protected Rigidbody rigidBody;
        protected Transform myTransform;
        protected Animator animator;
        protected Field cField;
        private float gridSize = 0.005f; // マス目のサイズ
        protected float moveSpeed;
        private int iViewID;
        private bool canMove = true;

        private Material cMaterial;

        private float elapsedTime = 0f;

        public bool pushBtnUp = false;
        public bool pushBtnDown = false;
        public bool pushBtnLeft = false;
        public bool pushBtnRight = false;
        public bool pushBtnEnter = false;


        public PlayerAction(ref Rigidbody rb, ref Transform tf, ref Animator ani, ref Field fi, int iViewID){
            rigidBody = rb;
            myTransform = tf;
            animator = ani;
            cField = fi;
            LastV3 = myTransform.position;
            moveSpeed = 1.0f;
        }

        public void SetMaterialType(string sParamMaterialType){
            MaterialManager cMaterialMng = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
            cMaterial = cMaterialMng.GetMaterialOfType(sParamMaterialType);
        }

        public void UpdateButton()
        {
            elapsedTime += Time.deltaTime;
            /*
            // 経過時間が0.5秒以上の場合
            if (elapsedTime >= 0.25f)
            {
                GameObject gPlayer = GameObject.Find("Player1(Clone)");
                Player cPlayer = gPlayer.GetComponent<Player>();
                BtnMoveClear(cPlayer);
                elapsedTime = 0f;
            }
            */

            //Debug.Log("UpdateButton");
            if (pushBtnUp)
            {
                BtnMoveUp();
            }
            else if (pushBtnDown)
            {
                BtnMoveDown();
            }
            else if (pushBtnRight)
            {
                BtnMoveRight();
            }
            else if (pushBtnLeft)
            {
                BtnMoveLeft();
            }
            else if(pushBtnEnter)
            {
                BtnDropBom();
            }
            else 
            {
                // No button pressed
            } 
        }



        public void UpdateMovement()
        {
            animator.SetBool ("Walking", false);
            UpdatePlayerMovement();

            Vector3 v3 = GetPos();
            Vector3 v3_ground = new Vector3(v3.x, v3.y-1, v3.z);
            
            canMove = cField.IsMatch(v3_ground, cMaterial);
            
            //Debug.Log("canMove : " + canMove + " cMaterial :" + cMaterial + " v3 :"  + v3 + " v3_ground :" + v3_ground);

            if(canMove){
                LastV3 = myTransform.position;
            }
            else{
                myTransform.position = LastV3;                
            }

        }

        public void BtnMoveClear(Player cPlayer)
        {
            PlayerAction cPlayerAction = cPlayer.GetPlayerAction();
            cPlayerAction.pushBtnUp = false;
            cPlayerAction.pushBtnDown = false;
            cPlayerAction.pushBtnRight = false;
            cPlayerAction.pushBtnLeft = false;
            cPlayerAction.pushBtnEnter = false;
        }


        public  void BtnMoveUp()
        {
            string name = cField.GetName();
            GameObject gPlayer = GameObject.Find(name);
            Player cPlayer = gPlayer.GetComponent<Player>();
            BtnMoveClear(cPlayer);
            PlayerAction cPlayerAction = cPlayer.GetPlayerAction();
            cPlayerAction.pushBtnUp = true;
            MovePlayer(gPlayer,Vector3.forward);
        }

        public  void BtnMoveDown()
        {
            string name = cField.GetName();
            GameObject gPlayer = GameObject.Find(name);
            Player cPlayer = gPlayer.GetComponent<Player>();
            BtnMoveClear(cPlayer);
            PlayerAction cPlayerAction = cPlayer.GetPlayerAction();
            cPlayerAction.pushBtnDown = true;
            MovePlayer(gPlayer,Vector3.back);
        }

        public  void BtnMoveRight()
        {
            string name = cField.GetName();
            GameObject gPlayer = GameObject.Find(name);
            Player cPlayer = gPlayer.GetComponent<Player>();
            BtnMoveClear(cPlayer);
            PlayerAction cPlayerAction = cPlayer.GetPlayerAction();
            cPlayerAction.pushBtnRight = true;
            MovePlayer(gPlayer,Vector3.right);
        }

        public  void BtnMoveLeft()
        {
            string name = cField.GetName();
            GameObject gPlayer = GameObject.Find(name);
            Player cPlayer = gPlayer.GetComponent<Player>();
            BtnMoveClear(cPlayer);
            PlayerAction cPlayerAction = cPlayer.GetPlayerAction();
            cPlayerAction.pushBtnLeft = true;
            MovePlayer(gPlayer, Vector3.left);
        }

        public  void BtnDropBom()
        {
            string name = cField.GetName();
            GameObject gPlayer = GameObject.Find(name);
            Player cPlayer = gPlayer.GetComponent<Player>();
            BtnMoveClear(cPlayer);
            PlayerAction cPlayerAction = cPlayer.GetPlayerAction();
            cPlayerAction.pushBtnEnter = true;
            cPlayer.DropBom();
        }

        private void MovePlayer(GameObject gPlayer, Vector3 direction)
        {
            //Debug.Log("MovePlayer: " + direction);
            /*
            Rigidbody cRigidbody = gPlayer.GetComponent<Rigidbody>();
            cRigidbody.velocity = gPlayer.transform.position + direction * gridSize;
            Rigidbody cRigidbody = gPlayer.GetComponent<Rigidbody>();
            cRigidbody.velocity = direction * gridSize;
            */
            /*
            Vector3 newPosition = gPlayer.transform.position + direction * gridSize;
            newPosition.x = Mathf.RoundToInt(newPosition.x / gridSize) * gridSize;
            newPosition.z = Mathf.RoundToInt(newPosition.z / gridSize) * gridSize;
            gPlayer.transform.position = newPosition;
            */
            Transform cTransform = gPlayer.GetComponent<Transform>();
            float moveDistance = moveSpeed * Time.deltaTime;
            cTransform.Translate(Vector3.forward * moveDistance);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            cTransform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

            Animator cAnimator = cTransform.Find("PlayerModel").GetComponent<Animator>();
            cAnimator.SetBool("Walking", true);
        }

        public void MoveUp()
        {
            Vector3 newPosition = rigidBody.position + Vector3.forward * gridSize * moveSpeed; // gridSizeに移動スピードを掛ける
            newPosition.x = Mathf.RoundToInt(newPosition.x / gridSize) * gridSize;
            newPosition.z = Mathf.RoundToInt(newPosition.z / gridSize) * gridSize;
            rigidBody.MovePosition(newPosition);
            myTransform.rotation = Quaternion.Euler(0, 0, 0);
            animator.SetBool("Walking", true);
        }

        public void MoveDown()
        {
            Vector3 newPosition = rigidBody.position - Vector3.forward * gridSize * moveSpeed;
            newPosition.x = Mathf.RoundToInt(newPosition.x / gridSize) * gridSize;
            newPosition.z = Mathf.RoundToInt(newPosition.z / gridSize) * gridSize;
            rigidBody.MovePosition(newPosition);
            myTransform.rotation = Quaternion.Euler(0, 180, 0);
            animator.SetBool("Walking", true);
        }

        public void MoveLeft()
        {
            Vector3 newPosition = rigidBody.position - Vector3.right * gridSize * moveSpeed;
            newPosition.x = Mathf.RoundToInt(newPosition.x / gridSize) * gridSize;
            newPosition.z = Mathf.RoundToInt(newPosition.z / gridSize) * gridSize;
            rigidBody.MovePosition(newPosition);
            myTransform.rotation = Quaternion.Euler(0, 270, 0);
            animator.SetBool("Walking", true);
        }

        public void MoveRight()
        {
            Vector3 newPosition = rigidBody.position + Vector3.right * gridSize * moveSpeed;
            newPosition.x = Mathf.RoundToInt(newPosition.x / gridSize) * gridSize;
            newPosition.z = Mathf.RoundToInt(newPosition.z / gridSize) * gridSize;
            rigidBody.MovePosition(newPosition);
            myTransform.rotation = Quaternion.Euler(0, 90, 0);
            animator.SetBool("Walking", true);
        }

        protected virtual void UpdatePlayerMovement()
        {
            // Keyboard input
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                BtnMoveUp();
            }
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                BtnMoveDown();
            }
            else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                BtnMoveLeft();
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                BtnMoveRight();
            }

            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W) ||
                Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D) ||
                Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A) ||
                Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
            {
                string name = cField.GetName();
                GameObject gPlayer = GameObject.Find(name);
                Player cPlayer = gPlayer.GetComponent<Player>();
                BtnMoveClear(cPlayer);
            }
                

        }



        private Vector3 GetPos(){
            float x = Mathf.Round(myTransform.position.x);
            /*
            if(x % 2 == 1){
                x += 1;
            }
            */
            float z = Mathf.Round(myTransform.position.z);
            /*
            if(z % 2 == 1){
                z += 1;
            }
            */
            float y = 1;
            return new Vector3(x,y,z);
        }

        public void SpeedUp(){
            moveSpeed += 0.5f;
        }
    }
}