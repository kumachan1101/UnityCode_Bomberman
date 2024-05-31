using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BomName;
public class Bom_Explode : Bom
{
    
    protected override bool X_Explosion(int i){
        GameObject g = Instantiate(ExplosionPrefab);
        g.GetComponent<Renderer>().material = cMaterialType;
        Vector3 v3Temp = new Vector3(transform.position.x+i,transform.position.y,transform.position.z);
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

    protected override bool Z_Explosion(int i){
        GameObject g = Instantiate(ExplosionPrefab);
        g.GetComponent<Renderer>().material = cMaterialType;
        Vector3 v3Temp = new Vector3(transform.position.x,transform.position.y,transform.position.z+i);
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


}