using UnityEngine;
using PlayerBomName;
using System;
using Unity.IO.LowLevel.Unsafe;

namespace PlayerActionName{
    public class PlayerAction
    {
        protected Vector3 LastV3;
        protected Rigidbody rigidBody;
        protected Transform myTransform;
        protected Animator animator;
        protected Field_Base cField;
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
        protected Library cLibrary;
        public PlayerAction(ref Rigidbody rb, ref Transform tf, ref Animator ani, ref Field_Base fi, int iViewID){
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

            if (pushBtnUp)
            {
                MoveUp();
            }
            else if (pushBtnDown)
            {
                MoveDown();
            }
            else if (pushBtnRight)
            {
                MoveRight();
            }
            else if (pushBtnLeft)
            {
                MoveLeft();
            }
            else if(pushBtnEnter)
            {
                DropBom();
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

            if(null == cLibrary){
                cLibrary = GameObject.Find("Library").GetComponent<Library>();
            }

            Vector3 v3 = cLibrary.GetPos(myTransform.position);
            Vector3 v3_ground = new Vector3(v3.x, v3.y-1, v3.z);
            
            canMove = cField.IsMatch(v3_ground, cMaterial);
            if(false == canMove){
                canMove = cField.IsMatchObjMove(v3_ground);
            }
            //Debug.Log("canMove : " + canMove + " cMaterial :" + cMaterial + " v3 :"  + v3 + " v3_ground :" + v3_ground);

            if(canMove){
                LastV3 = myTransform.position;
            }
            else{
                myTransform.position = LastV3;                
            }

        }

        public void MoveClear(Player_Base cPlayer)
        {
            PlayerAction cPlayerAction = cPlayer.GetPlayerAction();
            cPlayerAction.pushBtnUp = false;
            cPlayerAction.pushBtnDown = false;
            cPlayerAction.pushBtnRight = false;
            cPlayerAction.pushBtnLeft = false;
            cPlayerAction.pushBtnEnter = false;
        }

        private GameObject GetPlayerGameObject(){
            string name = cField.GetName();
            GameObject gPlayer = GameObject.Find(name);
            return gPlayer;
        }

        protected Player_Base GetPlayerComponent(GameObject gPlayer){
			Player_Base cPlayer = gPlayer.GetComponent<Player_Base>();
            return cPlayer;
        }


        public void PerformPlayerAction(Vector3 moveDirection, Action<PlayerAction> flagSetter)
        {
            GameObject gPlayer = GetPlayerGameObject();
            Player_Base cPlayer = GetPlayerComponent(gPlayer);
            MoveClear(cPlayer);

            PlayerAction cPlayerAction = cPlayer.GetPlayerAction();

            // フラグの設定を呼び出し元から受け取った関数で行う
            flagSetter(cPlayerAction);

            MovePlayer(gPlayer, moveDirection);
        }
        public virtual void MoveUp()
        {
            PerformPlayerAction(Vector3.forward, (cPlayerAction) => {
                cPlayerAction.pushBtnUp = true;
            });
        }

        public virtual void MoveDown()
        {
            PerformPlayerAction(Vector3.back, (cPlayerAction) => {
                cPlayerAction.pushBtnDown = true;
            });
        }

        public virtual void MoveRight()
        {
            PerformPlayerAction(Vector3.right, (cPlayerAction) => {
                cPlayerAction.pushBtnRight = true;
            });
        }

        public virtual void MoveLeft()
        {
            PerformPlayerAction(Vector3.left, (cPlayerAction) => {
                cPlayerAction.pushBtnLeft = true;
            });
        }

        public virtual void DropBom()
        {
            PerformPlayerAction(Vector3.zero, (cPlayerAction) => {
                cPlayerAction.pushBtnEnter = true;
            });
            
            GameObject gPlayer = GetPlayerGameObject();
            Player_Base cPlayer = GetPlayerComponent(gPlayer);
            cPlayer.DropBom();
        }

        private void MovePlayer(GameObject gPlayer, Vector3 direction)
        {
            
            if(direction == Vector3.zero){
                return;
            }
            Transform cTransform = gPlayer.GetComponent<Transform>();
            float moveDistance = moveSpeed * Time.deltaTime;
            cTransform.Translate(Vector3.forward * moveDistance);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            cTransform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

            Animator cAnimator = cTransform.Find("PlayerModel").GetComponent<Animator>();
            cAnimator.SetBool("Walking", true);
        }
        public void Move(Vector3 direction, float rotationY)
        {
            Vector3 newPosition = rigidBody.position + direction * gridSize * moveSpeed;
            newPosition.x = Mathf.RoundToInt(newPosition.x / gridSize) * gridSize;
            newPosition.z = Mathf.RoundToInt(newPosition.z / gridSize) * gridSize;
            rigidBody.MovePosition(newPosition);

            myTransform.rotation = Quaternion.Euler(0, rotationY, 0); // Y軸周りの回転を設定

            animator.SetBool("Walking", true);
        }

        protected virtual void UpdatePlayerMovement()
        {
            // Keyboard input
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                MoveUp();
            }
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                MoveDown();
            }
            else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                MoveLeft();
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                MoveRight();
            }

            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W) ||
                Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D) ||
                Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A) ||
                Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
            {

                GameObject gPlayer = GetPlayerGameObject();
                Player_Base cPlayer = GetPlayerComponent(gPlayer);
                MoveClear(cPlayer);
            }
        }

        public void SpeedUp(){
            if(moveSpeed >= 2){
                return;
            }
            moveSpeed += 0.5f;
        }
        
    }
}