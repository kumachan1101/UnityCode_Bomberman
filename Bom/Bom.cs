using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BomPosName;
using Photon.Pun;

namespace BomName{
    public class Bom : MonoBehaviourPunCallbacks
    {
        public GameObject ExplosionPrefab;

        protected Material cMaterialType;
        protected Field cField;
        private int iViewID;
        //private List<GameObject> ExplosionList = new List<GameObject>();
        // Start is called before the first frame update
        private Vector3 moveDirection = Vector3.zero;
        private float moveSpeed = 3f; // 動く速さ
        private bool isMoving = false; // 移動中フラグ
        public int iExplosionNum;
        private object lockObject = new object(); // ロックオブジェクト

        // Playerクラスから方向を受け取るメソッド
        public void SetMoveDirection(Vector3 direction)
        {
            moveDirection =direction;
            //moveDirection = direction.normalized; // 方向を正規化することで、速度を一定に保つ
        }
        
        public void SetMaterialType(Material cParamMaterial){
            cMaterialType = cParamMaterial;
        }


        void Awake(){
            cField = GameObject.Find("Field").GetComponent<Field>();
        }
        // Start is called before the first frame update
        void Start()
        {
            //DelayMethodを3秒後に呼び出す
            Invoke(nameof(Explosion), 3f);
        }

        public void CancelInvokeAndCallExplosion()
        {
            CancelInvoke(nameof(Explosion));
            Explosion();
        }

        // Update is called once per frame
        void Update()
        {
            if (isMoving)
            {
                //Debug.Log(moveDirection);
                //Debug.Log("moveDirection" + moveDirection);
                transform.position += moveDirection * moveSpeed * Time.deltaTime * 2;
            }

        }

        public void SetViewID(int iParamViewID){
            iViewID = iParamViewID;
        }

        public int GetViewID(){
            return iViewID;
        }


        protected virtual bool IsWall(Vector3 v3Temp){
            bool bRet = cField.IsAllWall(v3Temp);
            return bRet;
        }

        protected virtual bool X_Explosion(int i){
            GameObject g = Instantiate(ExplosionPrefab);
            g.GetComponent<Renderer>().material = cMaterialType;
            Vector3 v3Temp = new Vector3(transform.position.x+i,transform.position.y,transform.position.z);
            cField.DeletePositionAndName(v3Temp, "Explosion(Clone)");

            bool bRet = IsWall(v3Temp);
            if(bRet){
                Destroy(g);
                return true;
            }
            bRet = cField.IsBroken(v3Temp);
            if(bRet){
                g.transform.position = v3Temp;
                return true;
            }
            g.transform.position = v3Temp;
            return false;
        }

        protected virtual bool Z_Explosion(int i){
            GameObject g = Instantiate(ExplosionPrefab);
            g.GetComponent<Renderer>().material = cMaterialType;
            Vector3 v3Temp = new Vector3(transform.position.x,transform.position.y,transform.position.z+i);
            cField.DeletePositionAndName(v3Temp, "Explosion(Clone)");
            bool bRet = IsWall(v3Temp);
            if(bRet){
                Destroy(g);
                return true;
            }
            bRet = cField.IsBroken(v3Temp);
            if(bRet){
                g.transform.position = v3Temp;
                return true;
            }

            g.transform.position = v3Temp;
            return false;
        }

        protected virtual void Explosion()
        {
            lock (lockObject)
            {
                Vector3 v3 = GetPos(transform.position);
                transform.position = v3;
                cField.DeletePositionAndName(v3, "Explosion(Clone)");

                //中心の爆風は、アイテム効果で、地面に沈まないようにする
                GameObject g = Instantiate(ExplosionPrefab);
                g.GetComponent<Renderer>().material = cMaterialType;
                g.transform.position = v3;

                //float x = g.transform.position.x;
                for (int i = 1; i <= iExplosionNum; i++) 
                {
                    if(X_Explosion(i*(-1))){
                        break;
                    }
                }

                for (int i = 1; i <= iExplosionNum; i++) 
                {
                    if(X_Explosion(i)){
                        break;
                    }
                }

                //float z = g.transform.position.z;
                for (int i = 1; i <= iExplosionNum; i++) 
                {
                    if(Z_Explosion(i*(-1))){
                        break;
                    }
                }

                for (int i = 1; i <= iExplosionNum; i++) 
                {
                    if(Z_Explosion(i)){
                        break;
                    }
                }
    /*
                BomPos cBom = new BomPos();
                cBom.SetPos(transform.position);
                GameObject gBomControl = GameObject.Find("BomControl");
                gBomControl.GetComponent<BomControl>().DelBom(cBom);
    */
                Destroy(this.gameObject);
            }
        }

        private void OnDestroy()
        {
            // Destroy時に登録したInvokeをすべてキャンセル
            CancelInvoke();
        }

        void OnCollisionEnter(Collision collision)
        {
            ProcessCollision(collision.transform.name);
        }

        void OnTriggerEnter(Collider other)
        {
            ProcessCollision(other.transform.name);
        }

        void ProcessCollision(string collisionName)
        {
            switch (collisionName)
            {
                case "Bom(Clone)":
                case "Bombigban(Clone)":
                case "BomExplode(Clone)":
                case "Broken(Clone)":
                case "FixedWall(Clone)":
                case "Wall(Clone)":
                    break;
                default:
                    return;
            }
            // 衝突を検知したら座標を補正して移動を止める
            transform.position = GetPos(transform.position);
            isMoving = false; // 移動停止
        }

        void OnTriggerExit(Collider other)
        {
            //Debug.Log("すり抜けた！");
            GetComponent<SphereCollider>().isTrigger = false;
            
        }

        protected Vector3 GetPos(Vector3 position)
        {
            float x = Mathf.Round(position.x);
            /*
            if (x % 2 == 1)
            {
                x += 1;
            }
            */
            float y = 1;
            float z = Mathf.Round(position.z);
            /*
            if (z % 2 == 1)
            {
                z += 1;
            }
            */
            return new Vector3(x, y, z);
        }

        public void AbailableBomKick(){
            isMoving = true;
        }

        public void AbailableBomAttack(Vector3 direction){
            isMoving = true;
            SetMoveDirection(direction);
        }


    }
    
}
