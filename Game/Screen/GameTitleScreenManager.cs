using Photon.Pun;

public class GameTitleScreenManager : BaseScreenManager
{
    public override void InitializeScreen()
    {
        InitializeCanvas();
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

    void Start(){
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect(); // 切断処理を開始
        }
    }
}
