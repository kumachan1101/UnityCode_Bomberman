
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Collections;
using System;

public static class BlockManagerFactory
{
    public static T Create<T>(GameObject obj) where T : Component
    {
        return obj.AddComponent<T>();
    }
}

public abstract class Field_Block_Base : MonoBehaviourPunCallbacks
{
    private bool bSetUp;
    protected Library_Base cLibrary;
    protected ExplosionManager explosionManager;
    protected GroundBlockManager groundBlockManager;
    protected FixedWallBlockManager fixedWallBlockManager;
    protected BrokenBlockManager brokenBlockManager;
    protected ObjMoveBlockManager objMoveBlockManager;
    protected virtual void InsObjMove_RPC(int x, int y, int z, Library_Base.Direction randomDirection) { }
    //public virtual void Rainbow_RPC(string sMaterialType) { }
    void Awake()
    {
        explosionManager = CreateExplosionManager();
        cLibrary = GameObject.Find("Library").GetComponent<Library_Base>();

        CreateBlockManagers();
        InitializeBlockManagers();

        bSetUp = false;
    }

    // `ExplosionManager` を生成し、適切な `PoolerType` で初期化
    protected abstract ExplosionManager CreateExplosionManager();

    protected virtual void CreateBlockManagers()
    {
        CreateAndInitializeGroundBlockManager();
        CreateAndInitializeFixedWallBlockManager();
        CreateAndInitializeBrokenBlockManager();
        CreateAndInitializeObjMoveBlockManager();
    }

    protected virtual void InitializeBlockManagers()
    {
        groundBlockManager?.Initialize();
        fixedWallBlockManager?.Initialize();
        brokenBlockManager?.Initialize();
        objMoveBlockManager?.Initialize();
    }

    protected virtual void CreateAndInitializeGroundBlockManager()
    {
        groundBlockManager = CreateBlockManager<GroundBlockManager>();
        groundBlockManager?.Initialize();
    }

    protected virtual void CreateAndInitializeFixedWallBlockManager()
    {
        fixedWallBlockManager = CreateBlockManager<FixedWallBlockManager>();
        fixedWallBlockManager?.Initialize();
    }

    protected abstract void CreateAndInitializeBrokenBlockManager(); // 派生クラスで実装

    protected virtual void CreateAndInitializeObjMoveBlockManager()
    {
        objMoveBlockManager = CreateBlockManager<ObjMoveBlockManager>();
        objMoveBlockManager?.Initialize();
    }

    protected T CreateBlockManager<T>() where T : Component
    {
        return BlockManagerFactory.Create<T>(gameObject);
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

    public void AddBrokenBlock(int randomRangeMax)
    {
        brokenBlockManager.AddBrokenBlock(randomRangeMax);
    }

    public void SetupStage()
    {
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
        return fixedWallBlockManager.HasFixedWallAt(v3) || fixedWallBlockManager.HasWallAt(v3);
    }

    public bool IsMatchObjMove(Vector3 targetPosition)
    {
        return objMoveBlockManager.IsMatchObjMove(targetPosition);
    }
    //public abstract void UpdateGroundExplosion(GameObject gObj);

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
    private List<GameObject> FixedWallList;
    private List<GameObject> WallList;

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
                if (x == 0 || z == 0 || x == GameManager.xmax - 1 || z == GameManager.zmax - 1 ||
                    x == 1 || z == 1 || x == GameManager.xmax - 2 || z == GameManager.zmax - 2)
                {
                    GameObject g2 = Instantiate(FixedWallPrefab);
                    g2.transform.position = new Vector3(x, y2, z);
                    FixedWallList.Add(g2);
                }
            }
        }
    }

    public bool HasFixedWallAt(Vector3 position)
    {
        return Library_Base.IsObjectAtPosition(FixedWallList, position);
    }

    public bool HasWallAt(Vector3 position)
    {
        return Library_Base.IsObjectAtPosition(WallList, position);
    }
}

public class BrokenBlockManager : MonoBehaviourPunCallbacks
{
    public GameObject BrokenPrefab;
    public List<GameObject> BrokenList;

    public void Initialize()
    {
        this.BrokenPrefab = Resources.Load<GameObject>("Broken");
        this.BrokenList = new List<GameObject>();
    }

    [PunRPC]
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

    protected virtual void InsBrokenBlock_RPC(int x, int y, int z){}

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
                    int iRand = UnityEngine.Random.Range(0, randomRangeMax);
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

    public bool IsBroken(Vector3 v3)
    {
        return Library_Base.IsObjectAtPosition(BrokenList, v3);
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
