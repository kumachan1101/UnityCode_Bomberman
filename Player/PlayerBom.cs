using UnityEngine;

public class PlayerBom
{
    private BomConfiguration bomConfiguration;
    private BomStatus bomStatus;
    private BomManagement bomManagement;

    public PlayerBom()
    {
        bomConfiguration = new BomConfiguration();
        bomStatus = new BomStatus();
        bomManagement = new BomManagement();
    }

    public string GetMaterialType()
    {
        return bomConfiguration.GetMaterialType();
    }

    public void SetMaterialType(string materialType)
    {
        bomConfiguration.SetMaterialType(materialType);
    }

    public void IncreaseExplosion()
    {
        bomConfiguration.IncreaseExplosion();
    }

    public void IncreaseBom()
    {
        bomStatus.IncreaseBom();
    }

    public void EnableKick()
    {
        bomStatus.EnableKick();
    }

    public void EnableAttack()
    {
        bomStatus.EnableAttack();
    }

    public void EnableBreakthrough()
    {
        bomStatus.EnableBreakthrough();
    }

    public int GetExplosionNum()
    {
        return bomConfiguration.GetExplosionNum();
    }

    public bool CanKick()
    {
        return bomStatus.CanKick();
    }

    public bool CanAttack()
    {
        return bomStatus.CanAttack();
    }

    public bool CanBreakthrough()
    {
        return bomStatus.CanBreakthrough();
    }

    public void AddBom(GameObject bom)
    {
        bomManagement.AddBom(bom);
    }

    public bool IsBomAvailable(Vector3 position)
    {
        return bomManagement.IsBomAvailable(position, bomStatus.GetBomNum());
    }

    public BOM_KIND GetBomKind()
    {
        return bomConfiguration.GetBomKind();
    }

    public void SetBomKind(BOM_KIND bomKind)
    {
        bomConfiguration.SetBomKind(bomKind);
    }

	public BomConfiguration GetBomConfiguration(){
		return bomConfiguration;
	}

	public BomStatus GetBomStatus(){
		return bomStatus;
	}

	public BomManagement GetBomManagement(){
		return bomManagement;
	}
}
