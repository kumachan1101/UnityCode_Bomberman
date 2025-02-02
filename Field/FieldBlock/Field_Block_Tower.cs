using UnityEngine;

public class Field_Block_Tower : MonoBehaviour
{
    private int playercnt = 4;
    // GameManager.xmax と GameManager.zmax を使用して初期化
    protected Vector3[] v3TowerPos;
    public GameObject towerPrefab; // タワーのPrefab（Inspectorで設定）
    private MaterialManager materialManager;

    void Start()
    {
		materialManager = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();

        SetPositions();
        for (int i = 0; i < playercnt; i++)
        {
            SpawnTowerObjects(i);
        }
    }

    private void SetPositions()
    {
        int xmax = GameManager.xmax;
        int zmax = GameManager.zmax;

        v3TowerPos = new Vector3[]
        {
            new Vector3(2, 0.5f, 2),
            new Vector3(xmax - 3, 0.5f, zmax - 3),
            new Vector3(2, 0.5f, zmax - 3),
            new Vector3(xmax - 3, 0.5f, 2)
        };
    }

    public virtual int GetPower(){
        return 10;
    }

    private void SpawnTowerObjects(int index)
    {
        if (!IsValidTowerIndex(index))
        {
            Debug.LogError("Invalid index for tower position: " + index);
            return;
        }

        GameObject newTower = CreateTower(index);
        GameObject gCanvas = CreateCanvas(index);
        SetupTowerCanvasIntegration(newTower, gCanvas, index);
        ConfigureTowerMaterial(newTower);
    }

    private bool IsValidTowerIndex(int index)
    {
        return index >= 0 && index < v3TowerPos.Length;
    }

    private GameObject CreateTower(int index)
    {
        GameObject newTower = Instantiate(towerPrefab, v3TowerPos[index], Quaternion.identity);
        newTower.name = "Tower" + (index + 1);
        return newTower;
    }

    private GameObject CreateCanvas(int index)
    {
        GameObject playerPrefab = Resources.Load<GameObject>("CanvasPowerGageTower");
        GameObject gCanvas = Instantiate(playerPrefab);
        gCanvas.name = "CanvasPowerGageTower" + (index + 1);
        gCanvas.transform.position = new Vector3(0, 0, 0);

        PowerGage_Slider powerGage = gCanvas.GetComponent<PowerGage_Slider>();
        powerGage.SetPlayerCnt(index + 1);
        powerGage.SetPlayerNo(index + 1);

        return gCanvas;
    }

    private void SetupTowerCanvasIntegration(GameObject tower, GameObject canvas, int index)
    {
        PowerGageIF cPowerGageIF = tower.AddComponent<PowerGageIF_Tower>();
        cPowerGageIF.SetCanvasInsID(canvas.GetInstanceID());
    }

    private void ConfigureTowerMaterial(GameObject tower)
    {
        string materialType = materialManager.GetBomMaterialByPlayerName(tower.name);
        Tower cTower = tower.GetComponent<Tower>();
        cTower.SetMaterialType(materialType);
    }



}
