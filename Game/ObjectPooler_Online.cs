using Photon.Pun; // Photon Unity Networkingの名前空間をインポート
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ObjectPooler_Online : ObjectPooler_Base
{
    private PhotonView photonView;

    protected override void Start()
    {
        photonView = GetComponent<PhotonView>();
        base.Start();
    }

    [PunRPC]
    public void SetObjectActive(int viewID, bool isActive)
    {
        // viewIDからPhotonViewを取得
        PhotonView targetPhotonView = PhotonView.Find(viewID);

        if (targetPhotonView != null)
        {
            // PhotonViewからGameObjectを取得し、アクティブ/非アクティブ状態を設定
            targetPhotonView.gameObject.SetActive(isActive);
        }
    }

    // オブジェクトのアクティブ/非アクティブ状態を変更するメソッド
    public override void SetObjectActive_RPC(GameObject targetObject, bool isActive)
    {
        // 自分のオブジェクトのみ状態を変更する
        PhotonView targetPhotonView = targetObject.GetComponent<PhotonView>();

        if (targetPhotonView != null && targetPhotonView.IsMine)
        {
            photonView.RPC("SetObjectActive", RpcTarget.All, targetPhotonView.ViewID, isActive);
        }
    }

    protected override GameObject CreateObject(GameObject prefab)
    {
        return PhotonNetwork.Instantiate(prefab.name, new Vector3(0, -2, 0), Quaternion.identity);
    }

    public override void InitializePool()
    {
        objectPoolQueue = new Queue<GameObject>();
        prefabByTag = new Dictionary<string, GameObject>();

        foreach (Pool pool in pools)
        {
            prefabByTag[pool.tag] = pool.prefab;

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = CreateObject(pool.prefab);
                objectPoolQueue.Enqueue(obj);
            }
        }
    }

    public override void EnqueueObject(string tag, GameObject obj)
    {
        if (!prefabByTag.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag \"" + tag + "\" doesn't exist.");
            return;
        }

        if (obj != null)
        {
			//Rigidbody rb = obj.GetComponent<Rigidbody>();
		    //rb.position = new Vector3(0, -2, 0); // オブジェクトの位置を変更
			obj.GetComponent<Explosion_Base>().SetPosition_RPC(new Vector3(0, -2, 0));

            //obj.transform.position = new Vector3(0, -2, 0); // オブジェクトの位置を変更
            objectPoolQueue.Enqueue(obj); // キューにエンキュー
            //Debug.Log("Enqueue: " + objectPoolQueue.Count);
        }
        else
        {
            Debug.LogWarning("Trying to enqueue a null or inactive object.");
        }
    }

	public override GameObject DequeueObject(string tag)
	{
		GameObject obj = null;
		List<GameObject> objectList = new List<GameObject>(objectPoolQueue);

		for (int i = 0; i < objectList.Count; i++)
		{
			if (objectList[i].name.Contains(tag))
			{
				obj = objectList[i];
				objectList.RemoveAt(i);
				break;
			}
		}

		// 新しいキューをリストから作成
		objectPoolQueue = new Queue<GameObject>(objectList);

		int count = objectPoolQueue.Count;
/*
		if (count <= 150)
		{
			InitializePool();
		}
*/
		Debug.Log(count);

		return obj;
	}

}
