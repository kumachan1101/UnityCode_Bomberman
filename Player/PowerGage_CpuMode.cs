using UnityEngine;
using UnityEngine.UI;
namespace PowerGageName
{
    public class PowerGage_CpuMode : PowerGage
    {
        protected override void SetDamage_RPC(int iDamage){
            SyncSetDamage(iDamage);
        }
    }

}
