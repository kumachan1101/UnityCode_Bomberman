using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ButtonClickScript : MonoBehaviourPunCallbacks
{

    public void LoadGameScene()
    {
        //PhotonNetwork.LeaveRoom();

		DestroyAllPhotonViews();

        PhotonNetwork.Disconnect();
        //DestroyAllObjects();
        PhotonNetwork.LoadLevel("GameTitle");
        //SceneManager.LoadScene("GameTitle");
    }

    public void DestroyAllPhotonViews()
    {
        foreach (PhotonView view in FindObjectsOfType<PhotonView>())
        {
            if (view.IsMine)
            {
                PhotonNetwork.Destroy(view);
            }
        }
    }
/*
    private void DestroyAllObjects()
    {
        // ルートオブジェクトを取得し、それぞれを削除します。
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            Debug.Log(obj);
            if (obj != gameObject) // 自身のオブジェクトは削除しないようにします。
            {
                Destroy(obj);
            }
        }
    }
*/

}