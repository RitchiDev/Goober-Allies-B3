using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEventManager
{
    private static Dictionary<GameEventType, System.Action> m_AllEventReactions = new Dictionary<GameEventType, System.Action>();

    public static void RaiseEvent(GameEventType gameEventType)
    {
        if (!m_AllEventReactions.ContainsKey(gameEventType))
        {
            Debug.LogWarning($"Event reactions does not contain: {gameEventType}");
            return;
        }

        m_AllEventReactions[gameEventType]?.Invoke();
    }

    public static void AddListener(GameEventType gameEventType, System.Action function)
    {
        if (!m_AllEventReactions.ContainsKey(gameEventType))
        {
            m_AllEventReactions.Add(gameEventType, null);
        }

        m_AllEventReactions[gameEventType] += function;
    }

    public static void RemoveListener(GameEventType gameEventType, System.Action function)
    {
        if (!m_AllEventReactions.ContainsKey(gameEventType))
        {
            Debug.LogWarning($"Event reactions does not contain: {gameEventType}");
            return;
        }

        if (m_AllEventReactions[gameEventType] == null)
        {
            Debug.LogWarning($"This game event type is null: {gameEventType}");
            return;
        }

        m_AllEventReactions[gameEventType] -= function;
    }
}

public static class GameEventManager<T>
{
    private static Dictionary<GameEventType, System.Action<T>> m_AllEventReactions = new Dictionary<GameEventType, System.Action<T>>();

    // Example: EventManager<Enemy>.RaiseEvent(gameEventType, this); < If it's located in the Enemy() class.
    public static void RaiseEvent(GameEventType gameEventType, T arg)
    {
        if (m_AllEventReactions.ContainsKey(gameEventType))
        {
            Debug.LogWarning($"Event reactions does not contain: {gameEventType}");
            return;
        }

        m_AllEventReactions[gameEventType]?.Invoke(arg);
    }

    public static void AddListener(GameEventType gameEventType, System.Action<T> function)
    {
        if (m_AllEventReactions.ContainsKey(gameEventType))
        {
            m_AllEventReactions.Add(gameEventType, null);
        }

        m_AllEventReactions[gameEventType] += function;
    }

    public static void RemoveListener(GameEventType gameEventType, System.Action<T> function)
    {
        if (m_AllEventReactions.ContainsKey(gameEventType))
        {
            Debug.LogWarning($"Event reactions does not contain: {gameEventType}");
            return;
        }

        if (m_AllEventReactions[gameEventType] == null)
        {
            Debug.LogWarning($"This game event type is null: {gameEventType}");
            return;
        }

        m_AllEventReactions[gameEventType] -= function;
    }
}