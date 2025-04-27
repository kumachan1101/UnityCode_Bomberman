using UnityEngine;

public class BomBigBan_Online : Bom_Online
{
    BomBigBan_Common cBomBiBomBigBan;

    void Awake() {
        AwakeCommon();
        cBomBiBomBigBan = gameObject.AddComponent<BomBigBan_Common>();
    }

    protected override void HandleExplosion(Vector3 initialPosition)
    {
        cBomBiBomBigBan.SetInstanceManager(cInsManager);
        moveManager.Explosion();
        transform.position = initialPosition;
        cBomBiBomBigBan.BomBiBomBigBan_Explosion(transform.position);
    }

}
