using UnityEngine;
public class ItemRainbow : Item{
    public override void Reflection(GameObject gObj){
        PlayerBom cPlayerBom = gObj.GetComponent<PlayerBom>();
        if(null != cPlayerBom){
            string sMaterialType = cPlayerBom.Get<string>(GetKind.MaterialType);
            MaterialManager cMaterialMng = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
            ExplosionManager cExplosionManager = GameObject.Find("ExplosionManager").GetComponent<ExplosionManager>();
            cExplosionManager.Rainbow_RPC(MaterialMapper.GetExplosionType(sMaterialType));
        }
    }
}
