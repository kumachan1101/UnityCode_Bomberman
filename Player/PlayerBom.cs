using BomKind;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BomPosName;
namespace PlayerBomName
{
    public class PlayerBom{
        private int iBomNum;
        private int iUsedBomNum;
        private int iExplosionNum;
        private BomKind.BOM_KIND eBomKind;

        private int iViewID;

        private bool bBomKick;
        private bool bBomAttack;
        private bool bWall;
        private List<GameObject> BomList = new List<GameObject>();

        private string sMaterialType;

        //private List<BomPos> BomList = new List<BomPos>();

        public PlayerBom(){
            iBomNum = 3;
            iExplosionNum = 3;
            eBomKind = BomKind.BOM_KIND.BOM_KIND_NOTHING;
            bBomKick = false;
            bWall = false;
        }

        public string GetMaterialType(){
            return sMaterialType;
        }
        public void SetMaterialType(string sParamMaterialType){
            sMaterialType = sParamMaterialType;
        }

        public void SetViewID(int iParamViewID){
            iViewID = iParamViewID;
        }

        public int GetExplosionNum(){
            return iExplosionNum;
        }

        public void ExplosionUp(){
            iExplosionNum++;
        }

        public void BomUp(){
            iBomNum++;
        }

        public void BomKick(){
            bBomKick = true;
        }

        public bool GetBomKick(){
            return bBomKick;
        }

        public void BomAttack(){
            bBomAttack = true;
        }


        public bool GetBomAttack(){
            return bBomAttack;
        }

        public void Wall(){
            bWall = true;
        }

        public bool GetBrokenthrough(){
            return bWall;
        }

        public void SetBomKind(BomKind.BOM_KIND ePramBomKind){
            eBomKind = ePramBomKind;
        }

        public BomKind.BOM_KIND GetBomKind(){
            return eBomKind;
        }

        private int GetBomNum(){
            int iBomNum = GetBomViewIDNum(iViewID);
            return iBomNum;
        }

        public bool isAbalableBom(Vector3 v3){
            if(iBomNum <= GetBomNum()){
                return false;
            }

            if(IsBom(v3)){
               return false;
            }

            return true;
        }


        private bool IsBom(Vector3 v3){
            foreach (GameObject gBom in BomList) {
                if(null != gBom){
                    if(gBom.transform.position == v3){
                        return true;
                    }
                }
            }
            return false;
        }

        public int GetBomViewIDNum(int iViewID){
            int iLen = 0;

            foreach (GameObject gBom in BomList) {
                if(null != gBom /*&& iViewID == gBom.GetComponent<Bom>().GetViewID()*/){
                    iLen++;
                }
            }
            return iLen;
        }

        public void AddBom(GameObject gBom){
            BomList.Add(gBom);
            //Debug.Log(BomList.Count);
        }

    }

}
