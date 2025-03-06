using UnityEngine;

public class PowerGageAddedRemoved : Field_Event
{
    private PowerGageAddedRemovedHandler powerGageHandler;

    protected override void Init()
    {
        powerGageHandler = new PowerGageAddedRemovedHandler();
    }

    // PowerGageの追加・削除イベントリスナーを登録
    protected override void RegisterListeners()
    {
        UnregisterListeners();
        PowerGageIF.onPowerGageAdded.AddListener(powerGageHandler.OnAdded);
        PowerGageIF.onPowerGageRemoved.AddListener(powerGageHandler.OnRemoved);
    }

    // PowerGageのリスナー解除
    protected override void UnregisterListeners()
    {
        PowerGageIF.onPowerGageAdded.RemoveListener(powerGageHandler.OnAdded);
        PowerGageIF.onPowerGageRemoved.RemoveListener(powerGageHandler.OnRemoved);
    }
}

public class PowerGageAddedRemovedHandler
{
    private CanvasPowerGageManager manager;

    public PowerGageAddedRemovedHandler()
    {
        manager = new CanvasPowerGageManager();
    }

    // PowerGage追加時の処理
    public void OnAdded(object obj)
    {
        if (obj is GameObject gameObject)
        {
            manager.RearrangeAddCanvases(gameObject);
        }
        else
        {
            Debug.Log($"objの型: {obj.GetType()}");
            Debug.LogWarning("渡されたオブジェクトはGameObjectではありません。");
        }
    }

    // PowerGage削除時の処理
    public void OnRemoved(object obj)
    {
        if (obj is GameObject gameObject)
        {
            manager.RearrangeRemoveCanvases(gameObject);
        }
        else
        {
            Debug.Log($"objの型: {obj.GetType()}");
            Debug.LogWarning("渡されたオブジェクトはGameObjectではありません。");
        }
    }
}
