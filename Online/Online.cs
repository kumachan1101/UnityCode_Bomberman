using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using PowerGageName;
public class Online : MonoBehaviourPunCallbacks
{
    private int playerCount;
    private Field cField;
    /*
    private GameObject g;
    public Texture txt1;
    public Texture txt2;
    public Texture txt3;
    public Texture txt4;

    public Texture[] textures = new Texture[4]; 

    private Color[] cColor = new Color[4];

    private Vector3[] v3GagePos = new Vector3[4];
    */

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        //PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }


    public override void OnJoinedRoom()
    {
        playerCount = PhotonNetwork.PlayerList.Length; //ルームにいる人数を確認

        GameObject gField = GameObject.Find("Field");
        cField = gField.GetComponent<Field>();
        cField.CreateBrokenBlock();

        GameObject gItemControl = GameObject.Find("ItemControl");

        if (playerCount == 1)
        {
            //GameObject gField = PhotonNetwork.Instantiate("Field", new Vector3(0,0,0), Quaternion.identity);
            //gField.name = "Field";

            //GameObject gBomControl = PhotonNetwork.Instantiate("BomControl", new Vector3(0,0,0), Quaternion.identity);
            //gBomControl.AddComponent<BomControl>();

            //GameObject gItemControl = PhotonNetwork.Instantiate("ItemControl", new Vector3(0,0,0), Quaternion.identity);
            //gItemControl.AddComponent<ItemControl>();
            
            gItemControl.GetComponent<ItemControl>().SetMaster();
                

        }


        SpawnPlayerObjects(playerCount);

/*
        string canvasName = "Canvas";
        Vector3 v3PwrGage = new Vector3(0, 0, 0);
        GameObject gCanvas = PhotonNetwork.Instantiate(canvasName + playerCount, v3PwrGage, Quaternion.identity);

        GameObject gPlayer = PhotonNetwork.Instantiate("Player"+playerCount, v3PlayerPos[playerCount-1], Quaternion.identity);
        Player cPlayer = gPlayer.GetComponent<Player>();
        cPlayer.SetViewID(gPlayer.GetComponent<PhotonView>().ViewID);
        cPlayer.SetSlider(gCanvas);
        PowerGageName.PowerGage cPowerGage = gCanvas.transform.Find("Slider").GetComponent<PowerGage>();
        //cPowerGage.init(cColor[playerCount-1], v3GagePos[playerCount-1]);

        Transform tra = cPlayer.transform.Find("PlayerModel");
        GameObject gobj = tra.GetChild(1).gameObject;
        gobj.GetComponent<SkinnedMeshRenderer>().material.mainTexture = textures[playerCount-1];
*/
    }

    // 引数playerCountで受け取り、キャンバスとプレイヤーオブジェクトを生成する関数
    private void SpawnPlayerObjects(int playerCount)
    {
        string canvasName = "CanvasOnline";
        Vector3 v3PwrGage = new Vector3(0, 0, 0);

        GameObject gCanvas = PhotonNetwork.Instantiate(canvasName + playerCount, v3PwrGage, Quaternion.identity);
        Vector3 v3PlayerPos = cField.GetPlayerPosition(1, playerCount-1);
        GameObject gPlayer = PhotonNetwork.Instantiate("PlayerOnline" + playerCount, v3PlayerPos, Quaternion.identity);
        cField.SetName("PlayerOnline" + playerCount + "(Clone)");
        //Player cPlayer = gPlayer.GetComponent<Player_Online>();
        //cPlayer.SetViewID(gPlayer.GetComponent<PhotonView>().ViewID);
        //cPlayer.SetSlider(gCanvas);
        //Debug.Log(gPlayer.GetComponent<PhotonView>().ViewID);
    }

/*
    //[PunRPC]
    private void SyncPlayer(int playerCount){
        GameObject gPlayer = PhotonNetwork.Instantiate("Player", v3PlayerPos[playerCount-1], Quaternion.identity);
        gPlayer.name = "Player"+playerCount;

        Player cPlayer = gPlayer.GetComponent<Player>();

        cPlayer.SetViewID(gPlayer.GetComponent<PhotonView>().ViewID);
        //photonView.RPC(nameof(CommonProcess), RpcTarget.All, cPlayer, playerCount, textures[playerCount-1], cColor[playerCount-1], v3GagePos[playerCount-1]);
        //CommonProcess(cPlayer, playerCount, textures[playerCount-1], cColor[playerCount-1], v3GagePos[playerCount-1]); 


        string canvasName = "Canvas";

        Vector3 v3PwrGage = new Vector3(0, 0, 0);
        GameObject gCanvas = PhotonNetwork.Instantiate(canvasName, v3PwrGage, Quaternion.identity);
        gCanvas.name = canvasName + playerCount;
        cPlayer.SetSlider(gCanvas);

        PowerGageName.PowerGage cPowerGage = gCanvas.transform.Find("Slider").GetComponent<PowerGage>();
        cPowerGage.init(cColor[playerCount-1], v3GagePos[playerCount-1]);
        //.color = new Color32 (242, 108, 216, 255);

        Transform tra = cPlayer.transform.Find("PlayerModel");
        GameObject gobj = tra.GetChild(1).gameObject;
        gobj.GetComponent<SkinnedMeshRenderer>().material.mainTexture = textures[playerCount-1];


    }
*/
/*
    public int GetPlayerCnt(){
        return playerCount;
    }


    private void CommonProcess(Player cPlayer, int playerNumber, Texture texture, Color cColor, Vector3 v3) {
        string canvasName = "Canvas";

        Vector3 v3PwrGage = new Vector3(0, 0, 0);
        GameObject gCanvas = PhotonNetwork.Instantiate(canvasName, v3PwrGage, Quaternion.identity);
        gCanvas.name = canvasName + playerNumber;
        cPlayer.SetSlider(gCanvas);

        PowerGageName.PowerGage cPowerGage = gCanvas.transform.Find("Slider").GetComponent<PowerGage>();
        cPowerGage.init(cColor, v3);
        //.color = new Color32 (242, 108, 216, 255);

        Transform tra = cPlayer.transform.Find("PlayerModel");
        GameObject gobj = tra.GetChild(1).gameObject;
        gobj.GetComponent<SkinnedMeshRenderer>().material.mainTexture = texture;
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {


        if (stream.IsWriting) {
            GameObject gPlayer = PhotonNetwork.Instantiate("Player", v3PlayerPos[playerCount-1], Quaternion.identity);
            Player cPlayer = gPlayer.GetComponent<Player>();
            cPlayer.SetViewID(gPlayer.GetComponent<PhotonView>().ViewID);

            stream.SendNext("Player"+playerCount);


            Vector3 v3PwrGage = new Vector3(0, 0, 0);
            GameObject gCanvas = PhotonNetwork.Instantiate(canvasName, v3PwrGage, Quaternion.identity);
            cPlayer.SetSlider(gCanvas);

            string canvasName = "Canvas";
            stream.SendNext(canvasName + playerCount);


            PowerGageName.PowerGage cPowerGage = gCanvas.transform.Find("Slider").GetComponent<PowerGage>();
            cPowerGage.init(cColor[playerCount-1], v3GagePos[playerCount-1]);

            Transform tra = cPlayer.transform.Find("PlayerModel");
            GameObject gobj = tra.GetChild(1).gameObject;
            gobj.GetComponent<SkinnedMeshRenderer>().material.mainTexture = textures[playerCount-1];


        } else {
            gPlayer.name = (String)stream.ReceiveNext();
            gCanvas.name = (String)stream.ReceiveNext();
            gobj.GetComponent<SkinnedMeshRenderer>().material.mainTexture
       }
    }
*/
}