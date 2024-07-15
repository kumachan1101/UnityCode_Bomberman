using System.Collections.Generic;
using UnityEngine;

public class BomManagement
{
    private List<GameObject> bomList = new List<GameObject>();

    public void AddBom(GameObject bom)
    {
        bomList.Add(bom);
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
