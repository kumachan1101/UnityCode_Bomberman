using System.Xml.Serialization;
using UnityEngine;
public enum BomStatusType
{
    BomAttack,
    BomKick,
    BomUp,
    BomStatusInvalid
}
public abstract class BomStatus
{
    private int bomNum;
    private bool canKick;
    private bool canAttack;
    private bool canBreakthrough;

    public BomStatus()
    {
        SetDefault();
    }

    public void SetDefault(){
        bomNum = 3;
        canKick = false;
        canAttack = false;
        canBreakthrough = false;
    }

    public abstract void Request(BomStatus cBomStatus);

    public int GetBomNum()
    {
        return bomNum;
    }

    public void IncreaseBom()
    {
        bomNum++;
    }

    public bool CanKick()
    {
        return canKick;
    }

    public void EnableKick()
    {
        canKick = true;
    }

    public bool CanAttack()
    {
        return canAttack;
    }

    public void EnableAttack()
    {
        canAttack = true;
    }

    public bool CanBreakthrough()
    {
        return canBreakthrough;
    }

    public void EnableBreakthrough()
    {
        canBreakthrough = true;
    }
}
