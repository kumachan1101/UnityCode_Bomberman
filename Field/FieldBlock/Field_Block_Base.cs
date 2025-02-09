
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Collections;

public class Field_Block_Base : MonoBehaviourPunCallbacks
{
    private bool bSetUp;

    protected Library_Base cLibrary;
    protected ExplosionManager explosionManager;

    private GroundBlockManager groundBlockManager;
    private FixedWallBlockManager fixedWallBlockManager;
    private BrokenBlockManager brokenBlockManager;
    private ObjMoveBlockManager objMoveBlockManager;

    protected virtual void ClearBrokenList_RPC() { }
    protected virtual void InsBrokenBlock_RPC(int x, int y, int z) { }
    protected virtual void InsObjMove_RPC(int x, int y, int z, Library_Base.Direction randomDirection) { }
    public virtual void Rainbow_RPC(string sMaterialType) { }

    void Awake()
    {
        explosionManager = new GameObject("ExplosionManager").AddComponent<ExplosionManager>();
        explosionManager.Initialize();
        //explosionManager.Initialize(GetComponent<ObjectPooler_Base>());
        cLibrary = GameObject.Find("Library").GetComponent<Library_Base>();

        // Field_Block_BaseのGameObjectに管理クラスを追加
        groundBlockManager = gameObject.AddComponent<GroundBlockManager>();
        fixedWallBlockManager = gameObject.AddComponent<FixedWallBlockManager>();
        brokenBlockManager = gameObject.AddComponent<BrokenBlockManager>();
        objMoveBlockManager = gameObject.AddComponent<ObjMoveBlockManager>();

        // 初期化処理
        groundBlockManager.Initialize();
        fixedWallBlockManager.Initialize();
        brokenBlockManager.Initialize();
        objMoveBlockManager.Initialize();

        bSetUp = false;
    }

    void Start()
    {
        CreateFixedBlock();
        CreateField();
        SetupStage();
    }

    public void CreateField()
    {
        AddBrokenBlock(5);
    }
    public void SetupStage()
    {
        //explosionManager.SetupStage();
        bSetUp = true;
    }
    public bool GetSetUp()
    {
        return bSetUp;
    }

    [PunRPC]
    public void ClearBrokenList()
    {
        brokenBlockManager.ClearBrokenList();
    }

    [PunRPC]
    public void InsBrokenBlock(int x, int y, int z)
    {
        brokenBlockManager.InsBrokenBlock(x, y, z);
    }

    public void AddBrokenBlock(int randomRangeMax)
    {
        StartCoroutine(AddBrokenBlockCoroutine(randomRangeMax));
    }

    private IEnumerator AddBrokenBlockCoroutine(int randomRangeMax)
    {
        int y = 1;
        for (int x = 0; x < GameManager.xmax; x++)
        {
            for (int z = 0; z < GameManager.zmax; z++)
            {
                Vector3 v3 = new Vector3(x, y, z);
                if (false == Library_Base.IsGameObjectAtPosition(v3))
                {
                    int iRand = Random.Range(0, randomRangeMax);
                    if (0 == iRand)
                    {
                        InsBrokenBlock_RPC(x, y, z);
                    }
                }
                if (z % 10 == 0)
                {
                    yield return null;
                }
            }
            if (x % 10 == 0)
            {
                yield return null;
            }
        }
    }

    [PunRPC]
    public void InsObjMove(int x, int y, int z, Library_Base.Direction randomDirection)
    {
        objMoveBlockManager.InsObjMove(x, y, z, randomDirection);
    }

    protected virtual void SetFieldRange()
    {
        GameManager.SetFieldRange(20, 20);
    }

    protected void CreateFixedBlock()
    {
        SetFieldRange();
        GetComponent<Field_Player_Base>().SetPlayerPositions();

        fixedWallBlockManager.CreateFixedWall();
        groundBlockManager.CreateGroundBlock();
    }

    public bool IsAllWall(Vector3 v3)
    {
        return cLibrary.IsObjectAtPosition(fixedWallBlockManager.FixedWallList, v3) || cLibrary.IsObjectAtPosition(fixedWallBlockManager.WallList, v3);
    }

    public bool IsBroken(Vector3 v3)
    {
        return cLibrary.IsObjectAtPosition(brokenBlockManager.BrokenList, v3);
    }

    public bool IsMatchObjMove(Vector3 targetPosition)
    {
        return objMoveBlockManager.IsMatchObjMove(targetPosition);
    }

    public bool IsMatch(Vector3 targetPosition, Material targetMaterial)
    {
        return explosionManager.IsMatch(targetPosition, targetMaterial);
    }

    public virtual void UpdateGroundExplosion(GameObject gObj) { }
    public virtual GameObject DequeueObject(string tag) { return null; }
    public virtual void EnqueueObject(GameObject obj) { }
}

public class GroundBlockManager : MonoBehaviour
{
    public GameObject GroundPrefab;
    public List<GameObject> GroundList;

    public void Initialize()
    {
        this.GroundPrefab = Resources.Load<GameObject>("Ground");
        this.GroundList = new List<GameObject>();
    }

    public void CreateGroundBlock()
    {
        int y1 = 0;
        for (int x = 0; x < GameManager.xmax; x++)
        {
            for (int z = 0; z < GameManager.zmax; z++)
            {
                GameObject g1 = Instantiate(GroundPrefab);
                g1.transform.position = new Vector3(x, y1, z);
                GroundList.Add(g1);
            }
        }
    }
}

public class FixedWallBlockManager : MonoBehaviour
{
    public GameObject FixedWallPrefab;
    public List<GameObject> FixedWallList;
    public List<GameObject> WallList;

    public void Initialize()
    {
        this.FixedWallPrefab = Resources.Load<GameObject>("FixedWall");
        this.WallList = new List<GameObject>();
        this.FixedWallList = new List<GameObject>();
    }

    public void CreateFixedWall()
    {
        int y2 = 1;
        for (int x = 0; x < GameManager.xmax; x++)
        {
            for (int z = 0; z < GameManager.zmax; z++)
            {
                if ((x == 0 || z == 0 || x == GameManager.xmax - 1 || z == GameManager.zmax - 1) ||
                    (x == 1 || z == 1 || x == GameManager.xmax - 2 || z == GameManager.zmax - 2))
                {
                    GameObject g2 = Instantiate(FixedWallPrefab);
                    g2.transform.position = new Vector3(x, y2, z);
                    FixedWallList.Add(g2);
                }
            }
        }
    }
}

public class BrokenBlockManager : MonoBehaviour
{
    public GameObject BrokenPrefab;
    public List<GameObject> BrokenList;

    public void Initialize()
    {
        this.BrokenPrefab = Resources.Load<GameObject>("Broken");
        this.BrokenList = new List<GameObject>();
    }

    public void InsBrokenBlock(int x, int y, int z)
    {
        GameObject g = Instantiate(BrokenPrefab);
        g.transform.position = new Vector3(x, y, z);
        BrokenList.Add(g);
    }

    public void ClearBrokenList()
    {
        foreach (var g in BrokenList)
        {
            if (g != null)
            {
                Destroy(g);
            }
        }
        BrokenList.Clear();
    }
}

public class ObjMoveBlockManager : MonoBehaviour
{
    public GameObject ObjMovePrefab;
    public List<GameObject> ObjMoveList;

    public void Initialize()
    {
        this.ObjMovePrefab = Resources.Load<GameObject>("ObjMove");
        this.ObjMoveList = new List<GameObject>();
    }

    public void InsObjMove(int x, int y, int z, Library_Base.Direction randomDirection)
    {
        GameObject g = Instantiate(ObjMovePrefab);
        g.transform.position = new Vector3(x, y, z);
        Library_Base.SetDirection(g, randomDirection);
        ObjMoveList.Add(g);
    }

    public bool IsMatchObjMove(Vector3 targetPosition)
    {
        foreach (GameObject obj in ObjMoveList)
        {
            if (obj != null && obj.transform.position == targetPosition)
            {
                return true;
            }
        }
        return false;
    }
}
