using System.Collections.Generic;
using UnityEngine;

public class CanvasPowerGageManager
{
    /// <summary>
    /// CanvasPowerGageを上詰めで再配置する (SetPlayerCntを利用)
    /// </summary>
    public void RearrangeCanvases()
    {
        // "CanvasPowerGage" 名称を持つオブジェクトをすべて取得
        List<GameObject> canvasPowerGages = Library_Base.FindGameObjectsByPartialName("CanvasPowerGage(Clone)");
        if (canvasPowerGages.Count == 0)
        {
            Debug.LogWarning("CanvasPowerGage が見つかりませんでした。");
            return;
        }

        // ソート: Canvas をプレイヤー番号順に並べ替え
        canvasPowerGages.Sort((a, b) =>
        {
            int noA = a.GetComponent<PowerGage_Slider>().GetPlayerNo();
            int noB = b.GetComponent<PowerGage_Slider>().GetPlayerNo();
            return noA.CompareTo(noB);
        });

        // 再配置処理
        int newPlayerCnt = 1; // 新しいプレイヤーのカウント
        foreach (GameObject canvas in canvasPowerGages)
        {
            PowerGage_Slider powerGageSlider = canvas.GetComponent<PowerGage_Slider>();
            if (powerGageSlider != null)
            {
                powerGageSlider.SetPlayerCnt(newPlayerCnt); // 新しい位置を設定
                newPlayerCnt++; // 次の位置のためにカウントを増やす
            }
        }
    }
}
