using Photon.Pun;

public class GameTitleScreenManager : BaseScreenManager
{
    public override void InitializeScreen()
    {
        InitializeCanvas();
        //DestroyAllPhotonViews();
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect(); // 切断処理を開始
        }

    }

    protected override void InitializeCanvas()
    {
        // タイトル画面にはCanvasを表示しないので何もしない
        if (currentCanvas != null)
        {
            Destroy(currentCanvas);
            currentCanvas = null;
        }
    }

	public void DestroyAllPhotonViews()
	{
		foreach (PhotonView view in FindObjectsOfType<PhotonView>())
		{
            view.TransferOwnership(PhotonNetwork.MasterClient);
			PhotonNetwork.Destroy(view.gameObject);
		}
	}

}
