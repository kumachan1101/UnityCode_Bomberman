using System.Collections.Generic;
using UnityEngine;

public class CanvasPowerGageManager
{
    /// <summary>
    /// CanvasPowerGageを上詰めで再配置する (SetPlayerCntを利用)
    /// </summary>
    private void RearrangeCanvases(List<GameObject> canvasPowerGages)
    {
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

    /// <summary>
    /// CanvasPowerGageを追加した際の処理
    /// </summary>
    public void RearrangeAddCanvases(GameObject obj)
    {
        // "CanvasPowerGage" 名称を持つオブジェクトをすべて取得
        List<GameObject> canvasPowerGages = Library_Base.FindGameObjectsIgnoringNumbers(obj.name);
        //canvasPowerGages.Add(obj); // 新たに追加されたオブジェクトをリストに追加
        RearrangeCanvases(canvasPowerGages);
    }

    /// <summary>
    /// CanvasPowerGageを削除した際の処理
    /// </summary>
    public void RearrangeRemoveCanvases(GameObject obj)
    {
        //Debug.Log("RearrangeRemoveCanvases:"+obj);
        // "CanvasPowerGage" 名称を持つオブジェクトをすべて取得
        List<GameObject> canvasPowerGages = Library_Base.FindGameObjectsIgnoringNumbers(obj.name);

        // リストから削除対象のオブジェクトを削除
        if (canvasPowerGages.Contains(obj))
        {
            canvasPowerGages.Remove(obj);
        }
        else
        {
            Debug.LogWarning(obj + "リストに削除対象のオブジェクトが見つかりません。");
        }
        RearrangeCanvases(canvasPowerGages);
    }
}
