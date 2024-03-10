using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
namespace PowerGageName
{
    public class PowerGage : MonoBehaviourPunCallbacks
    {
        public Slider cSlider;

        public void SetDamage(int iDamage){
            SetDamage_RPC(iDamage);
        }

        protected virtual void SetDamage_RPC(int iDamage){
            photonView.RPC(nameof(SyncSetDamage),RpcTarget.All, iDamage);
        }

        [PunRPC]
        public void SyncSetDamage(int iDamage){
            cSlider.value -= iDamage;
        }

        public void init(Color cColor, Vector3 v3){
            cSlider.GetComponent<RectTransform>().position = v3;
            GameObject gFill = cSlider.transform.Find("Fill Area/Fill").gameObject;
            gFill.GetComponent<Image>().color = cColor;
            //Debug.Log(grandChild);
        }

        public bool IsDead(){
            if(cSlider.value <= 0){
                return true;
            }
            return false;
        }
    }

}
