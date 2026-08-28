using Photon.Pun;
using UnityEngine;

public class GameTitleScreenManager : BaseScreenManager
{
    public override void InitializeScreen()
    {
        InitializeCanvas();
    }

    protected override void InitializeCanvas()
    {
        if (currentCanvas != null)
        {
            Destroy(currentCanvas);
            currentCanvas = null;
        }

        GameObject titleCanvas = GameObject.Find("GameStartCanvas");
        if (titleCanvas == null) return;
        ResponsiveTitleUiController responsive =
            titleCanvas.GetComponent<ResponsiveTitleUiController>();
        if (responsive == null)
            responsive = titleCanvas.AddComponent<ResponsiveTitleUiController>();
        responsive.RefreshLayout();
    }

    void Start(){
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect(); // 切断処理を開始
        }
    }
}
