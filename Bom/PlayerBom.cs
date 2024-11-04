using UnityEngine;

public class PlayerBom
{
    private BomConfigurationBase bomConfiguration;
    private BomStatus bomStatus;
    private BomManagement bomManagement;

    public PlayerBom()
    {
        bomConfiguration = new DefaultBomConfiguration();
        bomStatus = new BomStatus_BomDefault();
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


	public BomStatus GetBomStatus(){
		return bomStatus;
	}

	public BomManagement GetBomManagement(){
		return bomManagement;
	}

    public BomStatus CreateBomStatus(BomStatusType statusType)
    {
        BomStatus cBomStatus = null;
        switch (statusType)
        {
            case BomStatusType.BomAttack:
                cBomStatus = new BomStatus_BomAttack();
                break;

            case BomStatusType.BomKick:
                cBomStatus = new BomStatus_BomKick();
                break;

            case BomStatusType.BomUp:
                cBomStatus = new BomStatus_BomUp();
                break;
                
            case BomStatusType.BomStatusInvalid:
                cBomStatus = new BomStatus_BomDefault();
                break;
        }

        return cBomStatus;
        
    }

    public BomConfigurationBase CreateBomConfiguration(BomConfigurationType configType)
    {
        BomConfigurationBase bomConfiguration;
        switch (configType)
        {
            case BomConfigurationType.ExplodeBom:
                bomConfiguration = new ExplodeBomConfiguration();
                break;

            case BomConfigurationType.BigBanBom:
                bomConfiguration = new BigBanBomConfiguration();
                break;

            case BomConfigurationType.FireUp:
                bomConfiguration = new FireUpConfiguration();
                break;

            default:
                bomConfiguration = new DefaultBomConfiguration(); // デフォルト設定
                break;
        }

        return bomConfiguration;
        
    }

/*
    public BomConfigurationBase GetBomConfiguration(BomConfigurationType configType)
    {
        // BomConfigurationType に基づいて派生クラスを生成
        switch (configType)
        {
            case BomConfigurationType.ExplodeBom:
                bomConfiguration = new ExplodeBomConfiguration(bomConfiguration);
                break;

            case BomConfigurationType.BigBanBom:
                bomConfiguration = new BigBanBomConfiguration(bomConfiguration);
                break;

            case BomConfigurationType.FireUp:
                bomConfiguration = new FireUpConfiguration(bomConfiguration);
                break;

            default:
                bomConfiguration = new DefaultBomConfiguration(); // デフォルト設定
                break;
        }

        return bomConfiguration;
    }
*/


    public BomConfigurationBase GetBomConfiguration()
    {
        return bomConfiguration;
    }

/*
    public void SetBomKind(BOM_KIND bomKind)
    {
        bomConfiguration.SetBomKind(bomKind);
    }
    public void IncreaseExplosion()
    {
        bomConfiguration.IncreaseExplosion();
    }
*/
    public BomParameters CreateBomParameters(Vector3 position, Vector3 direction)
    {
        return new BomParameters
        {
            position = position,
            bomKind = GetBomKind(),
            viewID = 0,  // 必要に応じて設定
            explosionNum = GetExplosionNum(),
            bomKick = CanKick(),
            materialType = GetMaterialType(),
            bomAttack = CanAttack(),
            direction = direction
        };
    }

}
