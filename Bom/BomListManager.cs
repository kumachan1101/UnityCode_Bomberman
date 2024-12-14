using System.Collections.Generic;
using UnityEngine;

public class BomListManager
{
    private List<GameObject> bomList = new List<GameObject>();

    public void Add(GameObject bom)
    {
        bomList.Add(bom);
    }

    // リストから指定されたGameObjectを削除
    public void Remove(GameObject bom)
    {
        if (bomList.Contains(bom))
        {
            bomList.Remove(bom);
            Debug.Log($"GameObject {bom.name} をリストから削除しました。");
        }
        else
        {
            Debug.LogWarning($"GameObject {bom.name} はリストに存在しません。");
        }
    }

    public bool IsBomAvailable(Vector3 position, int maxBomNum)
    {
        if (maxBomNum <= GetCurrentBomNum())
        {
            return false;
        }

        if (IsBomAtPosition(position))
        {
            return false;
        }

        return true;
    }

    private int GetCurrentBomNum()
    {
        int count = 0;

        foreach (GameObject bom in bomList)
        {
            if (bom != null)
            {
                // bDelがtrueの場合、カウントをスキップ
                var bomComponent = bom.GetComponent<Bom_Base>(); // BomComponentはカスタムコンポーネント
                if (bomComponent != null && bomComponent.bDel)
                {
                    continue;
                }

                count++;
            }
        }

        return count;
    }


    private bool IsBomAtPosition(Vector3 position)
    {
        foreach (GameObject bom in bomList)
        {
            if (bom != null && bom.transform.position == position)
            {
                return true;
            }
        }

        return false;
    }
}
