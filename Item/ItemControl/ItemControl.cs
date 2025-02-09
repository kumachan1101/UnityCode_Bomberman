using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CreateItem
{
    public string itemName;
    public GameObject itemPrefab;
}

public interface IItemPathProvider
{
    Dictionary<string, string> GetItemPaths();
}

public interface IItemCountProvider
{
    Dictionary<string, int> GetItemCounts();
}

public class ItemFactory
{
    private readonly IItemPathProvider pathProvider;

    public ItemFactory(IItemPathProvider pathProvider)
    {
        this.pathProvider = pathProvider;
    }

    public CreateItem Create(string itemName)
    {
        var itemPaths = pathProvider.GetItemPaths();
        if (itemPaths.TryGetValue(itemName, out string path))
        {
            GameObject prefab = Resources.Load<GameObject>(path);
            return new CreateItem { itemName = itemName, itemPrefab = prefab };
        }
        return null;
    }
}

abstract public class ItemControl: MonoBehaviourPunCallbacks
{

    public List<GameObject> ItemList = new List<GameObject>();
    protected List<CreateItem> itemList = new List<CreateItem>();

    void Awake(){
        CreateItem_AddList(CreateItemPathProvider());
    }

    protected abstract IItemPathProvider CreateItemPathProvider();

    protected void CreateItem_AddList(IItemPathProvider pathProvider)
    {
        var factory = new ItemFactory(pathProvider);
        var countProvider = pathProvider as IItemCountProvider;
        var itemCounts = countProvider?.GetItemCounts() ?? new Dictionary<string, int>();

        foreach (var itemName in pathProvider.GetItemPaths().Keys)
        {
            int count = itemCounts.TryGetValue(itemName, out int itemCount) ? itemCount : 1;
            for (int i = 0; i < count; i++)
            {
                var item = factory.Create(itemName);
                if (item != null)
                {
                    itemList.Add(item);
                }
            }
        }
    }

    abstract public void CreateItem_RPC(Vector3 v3);

    public bool IsItem(Vector3 v3){
        foreach (GameObject gItem in ItemList) {
            if(null != gItem){
                if(gItem.transform.position == v3){
                    return true;
                }
            }
        }
        return false;
    }

    // アイテムをランダムに生成する関数
    
    [PunRPC]
    public void CreateRandomItem(Vector3 position, int randomIndex)
    {
        CreateItem selectedItem = itemList[randomIndex];
        GameObject itemInstance = Instantiate(selectedItem.itemPrefab, position, Quaternion.identity);
    }

    abstract protected bool IsCreateItem();

}

