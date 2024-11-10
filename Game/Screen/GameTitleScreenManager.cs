using Photon.Pun;

public class GameTitleScreenManager : BaseScreenManager
{
    public override void InitializeScreen()
    {
        InitializeCanvas();
        DestroyAllPhotonViews();
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
			if (view.IsMine)
			{
				PhotonNetwork.Destroy(view.gameObject);
			}
		}
	}

}
