using UnityEngine;
public class PowerGageIF_CpuMode : PowerGageIF
{
	protected override void SetDamage_RPC(int iDamage){
		SyncSetDamage(iDamage);
	}
	protected override void HeartUp_RPC(int iHeart){
		SyncHeartUp(iHeart);
	}

}


