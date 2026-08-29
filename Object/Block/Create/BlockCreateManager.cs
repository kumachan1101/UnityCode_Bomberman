
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using System.Collections;

public static class BlockManagerFactory
{
    public static T Create<T>(GameObject obj) where T : Component
    {
        return obj.AddComponent<T>();
    }
}

public abstract class BlockCreateManager : MonoBehaviourPunCallbacks
{
    protected GroundBlockManager groundBlockManager;
    protected FixedWallBlockManager fixedWallBlockManager;
    protected BrokenBlockManager brokenBlockManager;
    protected ObjMoveBlockManager objMoveBlockManager;
    protected virtual void InsObjMove_RPC(int x, int y, int z, Library_Base.Direction randomDirection) { }

    protected EventDispatcher eventDispatcher;
    private bool blockCreationStarted;
    private bool blockCreationCompleted;
    protected void InitEvent(){
         eventDispatcher = GameObject.Find("EventDispatcher").GetComponent<EventDispatcher>();        
    }

    void Awake()
    {
        InitEvent();
        CreateExplosionManager();
        CreateBlockManagers();
    }

    public void CompleteBlockCreate(){
        if (blockCreationCompleted)
        {
            return;
        }

        blockCreationCompleted = true;
        var vEvent = new CompleteBlockCreateEvent();
        eventDispatcher.DispatchEvent(vEvent);
    }

    /// <summary>
    /// 破壊可能ブロックの配置が実際に完了してからゲーム開始を通知する。
    /// </summary>
    public virtual void StartBlockCreation()
    {
        if (!TryBeginBlockCreation())
        {
            return;
        }

        AddBrokenBlock(5, CompleteBlockCreate);
    }

    protected bool TryBeginBlockCreation()
    {
        if (blockCreationStarted)
        {
            return false;
        }

        blockCreationStarted = true;
        return true;
    }

    // `ExplosionManager` を生成し、適切な `PoolerType` で初期化
    protected abstract void CreateExplosionManager();

    protected virtual void CreateBlockManagers()
    {
        CreateGroundBlockManager();
        CreateFixedWallBlockManager();
        CreateBrokenBlockManager();
        CreateObjMoveBlockManager();
    }

    protected virtual void CreateGroundBlockManager()
    {
        groundBlockManager = CreateBlockManager<GroundBlockManager>();
    }

    protected virtual void CreateFixedWallBlockManager()
    {
        fixedWallBlockManager = CreateBlockManager<FixedWallBlockManager>();
    }

    protected abstract void CreateBrokenBlockManager(); // 派生クラスで実装

    protected virtual void CreateObjMoveBlockManager()
    {
        objMoveBlockManager = CreateBlockManager<ObjMoveBlockManager>();
    }

    protected T CreateBlockManager<T>() where T : Component
    {
        return BlockManagerFactory.Create<T>(gameObject);
    }

    public void CreateBrokenBlock()
    {
        AddBrokenBlock(5);
    }

    public void AddBrokenBlock(int randomRangeMax)
    {
        AddBrokenBlock(randomRangeMax, null);
    }

    protected void AddBrokenBlock(int randomRangeMax, Action onCompleted)
    {
        brokenBlockManager.AddBrokenBlock(randomRangeMax, onCompleted);
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

    public bool IsBlockedForBomb(Vector3 targetPosition)
    {
        return IsAllWall(targetPosition) ||
               brokenBlockManager.IsBroken(targetPosition) ||
               IsMatchObjMove(targetPosition);
    }

}

public class GroundBlockManager : MonoBehaviour
{
    public GameObject GroundPrefab;
    public List<GameObject> GroundList;

	public void Awake() {
		Initialize();
	}

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

	void Awake() {
		Initialize();
	}
	private void Initialize()
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

	void Awake() {
		Initialize();
	}

	private void Initialize()
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

    public void AddBrokenBlock(int randomRangeMax, Action onCompleted = null)
    {
        StartCoroutine(AddBrokenBlockCoroutine(randomRangeMax, onCompleted));
    }

    private IEnumerator AddBrokenBlockCoroutine(int randomRangeMax, Action onCompleted)
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

        // 直前に生成したColliderを、完了イベントを受ける側から確実に参照できるようにする。
        Physics.SyncTransforms();
        onCompleted?.Invoke();
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

	void Awake() {
		Initialize();
	}

	private void Initialize()
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
