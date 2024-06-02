using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BomPosName;
using Photon.Pun;
using PlayerBomName;

//namespace BomName{
    public class Bom_Base : MonoBehaviourPunCallbacks
    {
        public GameObject ExplosionPrefab;

        //protected Material cMaterialType;
		protected string sMaterialKind;
        protected Field_Base cField;

        protected Library cLibrary;

		protected MaterialManager cMaterialMng;

		protected BomControl cBomControl;
        //private List<GameObject> ExplosionList = new List<GameObject>();
        // Start is called before the first frame update
        private Vector3 moveDirection = Vector3.zero;
		private Vector3 previousPosition;
        private float moveSpeed = 3f; // 動く速さ
        protected bool isMoving = false; // 移動中フラグ
        public int iExplosionNum;
        protected object lockObject = new object(); // ロックオブジェクト

        // Playerクラスから方向を受け取るメソッド
        public void SetMoveDirection(Vector3 direction)
        {
            moveDirection =direction;
            //moveDirection = direction.normalized; // 方向を正規化することで、速度を一定に保つ
        }
        
        public void SetMaterialKind(string sParamMaterial){
            sMaterialKind = sParamMaterial;
        }

        void Awake(){
            cField = GameObject.Find("Field").GetComponent<Field_Base>();
            cLibrary = GameObject.Find("Library").GetComponent<Library>();
			cMaterialMng = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
        }
        // Start is called before the first frame update
        void Start()
        {
			previousPosition = transform.position;
			// インスタンス生成直後にマテリアルを設定している事もあり、Awakeのタイミングではまだマテリアルが取得できない。
			// 初回描画のタイミングであれば取得可能であるため、ここでマテリアルを設定している
			GetComponent<Renderer>().material = cMaterialMng.GetMaterialOfType(sMaterialKind);
			string sExplosion = cMaterialMng.GetMaterialOfExplosion(sMaterialKind);
			ExplosionPrefab = Resources.Load<GameObject>(sExplosion);
            //DelayMethodを3秒後に呼び出す
            Invoke(nameof(Explosion), 3f);
        }

        public void CancelInvokeAndCallExplosion()
        {
			//Debug.Log("CancelInvokeAndCallExplosion");
            CancelInvoke(nameof(Explosion));
            Explosion();
        }

        // Update is called once per frame
        void Update()
        {
            if (isMoving)
            {
                transform.position += moveDirection * moveSpeed * Time.deltaTime * 2;
            }
			previousPosition = transform.position;
        }

        protected virtual bool IsWall(Vector3 v3Temp){
            bool bRet = cField.IsAllWall(v3Temp);
            return bRet;
        }

        protected virtual GameObject Instantiate_Explosion(Vector3 v3){
			return null;
        }


        protected virtual bool XorZ_Explosion(Vector3 v3Temp){
            bool bRet = IsWall(v3Temp);
            if(bRet){
                return true;
            }
            cLibrary.DeletePositionAndName(v3Temp, "Explosion");
            GameObject g = Instantiate_Explosion(v3Temp);

            bRet = cField.IsBroken(v3Temp);
            if(bRet){
                g.transform.position = v3Temp;
                return true;
            }
            g.transform.position = v3Temp;
            return false;
        }


        protected virtual bool X_Explosion(int i){
            Vector3 v3Temp = new Vector3(transform.position.x+i,transform.position.y,transform.position.z);
            bool bRet = XorZ_Explosion(v3Temp);
            return bRet;
        }

        protected virtual bool Z_Explosion(int i){
            Vector3 v3Temp = new Vector3(transform.position.x,transform.position.y,transform.position.z+i);
            bool bRet = XorZ_Explosion(v3Temp);
            return bRet;
        }

		protected virtual bool IsExplosion(){
			return false;
		}

        protected virtual void Explosion()
        {
			if(false == IsExplosion()){
				return;
			}

            lock (lockObject)
            {
                Vector3 v3 = cLibrary.GetPos(transform.position);
                transform.position = v3;
                cLibrary.DeletePositionAndName(v3, "Explosion");

                //中心の爆風は、アイテム効果で、地面に沈まないようにする→未実装
                GameObject g = Instantiate_Explosion(v3);
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
                DestroySync(this.gameObject);
            }
        }

		protected virtual void DestroySync(GameObject g){}

        private void OnDestroy()
        {
            // Destroy時に登録したInvokeをすべてキャンセル
            CancelInvoke();
        }

		private void OnCollisionEnter(Collision collision){
            switch (collision.transform.name)
            {
                case "Broken(Clone)":
                case "FixedWall(Clone)":
                case "Wall(Clone)":
				case "Bom(Clone)":
				case "Bombigban(Clone)":
				case "BomExplode(Clone)":
                    break;
                default:
                    return;
            }
            // 衝突を検知したら座標を補正して移動を止める
            transform.position = cLibrary.GetPos(previousPosition);
            isMoving = false; // 移動停止
		}

        void OnTriggerEnter(Collider other)
        {
            switch (other.transform.name)
            {
				case "Explosion1(Clone)":
				case "Explosion2(Clone)":
				case "Explosion3(Clone)":
				case "Explosion4(Clone)":
					CancelInvokeAndCallExplosion();
	                break;
                default:
                    return;
            }
        }


        void OnTriggerExit(Collider other)
        {
            //Debug.Log("すり抜けた！");
            GetComponent<SphereCollider>().isTrigger = false;
            
        }

        public void AbailableBomKick(){
            isMoving = true;
        }

        public void AbailableBomAttack(Vector3 direction){
            isMoving = true;
            SetMoveDirection(direction);
        }


    }
    
//}
