using System;
using System.Collections.Generic;
using UnityEngine;

public interface IEvent { }

// イベントのサンプル
public class CompleteBlockCreateEvent : IEvent { }

public class EventDispatcher : MonoBehaviour
{
    private static EventDispatcher instance;
    private Dictionary<Type, List<Delegate>> eventListeners = new Dictionary<Type, List<Delegate>>();

    public static EventDispatcher Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = new GameObject("EventDispatcher");
                instance = obj.AddComponent<EventDispatcher>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // 重複インスタンスを削除
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterListener<T>(Action<T> listener) where T : IEvent
    {
        var eventType = typeof(T);
        if (!eventListeners.ContainsKey(eventType))
        {
            eventListeners[eventType] = new List<Delegate>();
        }

        // リスナーを直接リストに登録
        eventListeners[eventType].Add(listener);
        //Debug.Log($"[RegisterListener] {eventType} が登録されました。登録数: {eventListeners[eventType].Count}");
    }

    public void UnregisterListener<T>(Action<T> listener) where T : IEvent
    {
        var eventType = typeof(T);
        if (eventListeners.ContainsKey(eventType))
        {
            // リスナーを削除
            eventListeners[eventType].Remove(listener);

            if (eventListeners[eventType].Count == 0)
            {
                eventListeners.Remove(eventType);
            }

            //Debug.Log($"[UnregisterListener] {eventType} の登録を解除しました。現在の登録数: {eventListeners.GetValueOrDefault(eventType)?.Count ?? 0}");
        }
    }

    public void DispatchEvent(IEvent eventToDispatch)
    {
        var eventType = eventToDispatch.GetType();
        //Debug.Log($"[DispatchEvent] Event Type: {eventType}");

        if (eventListeners.ContainsKey(eventType))
        {
            foreach (var listener in eventListeners[eventType])
            {
                //Debug.Log($"[DispatchEvent] Listener Type: {listener.GetType()}");

                // Action<T> 型のリスナーを処理するため、リスナーが IEvent 型に変換される場合
                if (listener is Action<IEvent> action)
                {
                    //Debug.Log("action " + eventToDispatch);
                    action(eventToDispatch);
                }
                // Action<T> 型のリスナー（ジェネリック）を処理
                else if (listener is Delegate typedAction && typedAction.GetType().IsGenericType)
                {
                    var eventTypeArgument = typedAction.GetType().GetGenericArguments()[0];
                    if (eventTypeArgument.IsAssignableFrom(eventType))
                    {
                        // Action<T> 型のリスナーを IEvent 型にキャストして呼び出す
                        //Debug.Log("invoke " + eventToDispatch);
                        typedAction.DynamicInvoke(eventToDispatch);
                    }
                }
                else
                {
                    Debug.Log("listener is not Action<IEvent>");
                }
            }
        }
        else
        {
            Debug.Log($"No listeners registered for event: {eventType}");
        }
    }





}
