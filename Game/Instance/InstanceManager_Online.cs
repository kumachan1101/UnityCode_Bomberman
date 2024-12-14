using UnityEngine;
using Photon.Pun;
using System.Linq;

public class InstanceManager_Online : InstanceManager_Base
{
    private int iViewID;

    public override void SetPothonView(int viewid){
        iViewID = viewid;
    }

    public override void InstantiateInstancePool(Vector3 position)
	{
        PhotonView photonView = PhotonView.Find(iViewID);
		if(false == GetComponent<PhotonView>().IsMine){
			return;
		}
		photonView.RPC("InstantiateInstancePool_RPC",RpcTarget.All, position);
    }

    public override void DestroyInstancePool(GameObject instance)
    {
        instanceQueue.Enqueue(instance);
        PhotonView photonView = PhotonView.Find(iViewID);
		if(false == GetComponent<PhotonView>().IsMine){
			return;
		}
        photonView.RPC("DestroyInstancePool_RPC",RpcTarget.All);
    }

    override public void DestroyInstance(GameObject instance)
    {
        if (instance != null)
        {
            var bomComponent = instance.GetComponent<Bom_Base>(); // BomComponentはカスタムコンポーネント
            bomComponent.bDel = true;

            BomControl cBomControl = GameObject.Find("BomControl").GetComponent<BomControl>();
            cBomControl.instanceList.Add(instance);

            // ゲーム画面内で位置を移動 (0, -5, 0)
            instance.transform.position = new Vector3(0, -5, 0);

            // リストが10個を超えた場合、古いものを削除
            if (cBomControl.instanceList.Count > 10)
            {
                GameObject oldestInstance = cBomControl.instanceList[0]; // 最初の要素を取得
                cBomControl.instanceList.RemoveAt(0);                   // リストから削除

                if (oldestInstance != null)
                {
                    PhotonView pv = oldestInstance.GetComponent<PhotonView>();
                    if (pv != null && pv.IsMine) {
                        PhotonNetwork.Destroy(oldestInstance); // ネットワーク全体で削除
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("DestroyInstance: 渡されたGameObjectがnullです。");
        }        
    }
}