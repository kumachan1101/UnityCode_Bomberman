using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bom_BigBan : Bom_Explode
{

    private object lockObject = new object(); // ロックオブジェクト
    protected bool XZ_Explosion(int i, int j){
        GameObject g = Instantiate(ExplosionPrefab);
        g.GetComponent<Renderer>().material = cMaterialType;
        Vector3 v3Temp = new Vector3(transform.position.x+i,transform.position.y,transform.position.z+j);
        if(cField.CheckPositionAndName(v3Temp, "Explosion(Clone)")){
            Destroy(g);
            return false;
        }

        bool bRet = IsWall(v3Temp);
        if(bRet){
            Destroy(g);
            return true;
        }
        g.transform.position = v3Temp;
        return false;
    }

    protected override void Explosion()
    {
        lock (lockObject)
        {
            Vector3 v3 = GetPos(transform.position);
            transform.position = v3;
            if(!cField.CheckPositionAndName(v3, "Explosion(Clone)")){
                GameObject g = Instantiate(ExplosionPrefab);
                g.GetComponent<Renderer>().material = cMaterialType;
                g.transform.position = v3;
            }

            //GameObject gPlayer =  GameObject.Find(objname);
            //gPlayer.GetComponent<Player>().SetBomKind(BomKind.BOM_KIND.BOM_KIND_BIGBAN);
            //GameObject gBomControl = GameObject.Find("BomControl");
            //int iExplosionNum = gBomControl.GetComponent<BomControl>().GetExplosionNum();

            for (int j = 1; j <= iExplosionNum; j++) 
            {
                if(XZ_Explosion(0, j)){
                    break;
                }
            }
            for (int j = 1; j <= iExplosionNum; j++) 
            {
                if(XZ_Explosion(0, j*(-1))){
                    break;
                }
            }

            for (int i = 1; i <= iExplosionNum; i++) 
            {
                if(XZ_Explosion(i*(-1), 0)){
                    break;
                }
                for (int j = 1; j <= iExplosionNum; j++) 
                {
                    if(XZ_Explosion(i*(-1), j)){
                        break;
                    }
                }
                for (int j = 1; j <= iExplosionNum; j++) 
                {
                    if(XZ_Explosion(i*(-1), j*(-1))){
                        break;
                    }
                }
            }

            for (int i = 1; i <= iExplosionNum; i++) 
            {
                if(XZ_Explosion(i, 0)){
                    break;
                }
                for (int j = 1; j <= iExplosionNum; j++) 
                {
                    if(XZ_Explosion(i, j)){
                        break;
                    }
                }
                for (int j = 1; j <= iExplosionNum; j++) 
                {
                    if(XZ_Explosion(i, j*(-1))){
                        break;
                    }
                }
            }

            Destroy(this.gameObject);
        }
    }

}