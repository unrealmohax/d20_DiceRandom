using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EventMessage
{
    OnStartNewGame,
    OnRollButtonClick,
    OnRollDice,
    OnStopDice,
    OnRandomAndBonusPoint,
    OnRestartGame,
    OnFinishGame,
    OnSumWithBonus,
}

public class EventController : MonoBehaviour
{
    private static readonly Dictionary<EventMessage, Delegate> _events = new Dictionary<EventMessage, Delegate>();

    #region Remove

    public static void RemoveAll()
    {
        _events.Clear();
    }

    private static void Remove(EventMessage eventMessage, Delegate method)
    {
        if (!_events.TryGetValue(eventMessage, out var value)) return;

        try
        {
            var d = Delegate.Remove(value, method);

            if (d != null)
                _events[eventMessage] = d;
            else
                _events.Remove(eventMessage);
        }
        catch (Exception e) when (e is ArgumentException)
        {
            Debug.Log($"Remove! Signatures do not match: new - {method.Method} <> old - {value.Method}");
        }
    }

    public static void RemoveListener(EventMessage eventMessage, Action method)
    {
        Remove(eventMessage, method);
    }

    public static void RemoveListener<T>(EventMessage eventMessage, Action<T> method)
    {
        Remove(eventMessage, method);
    }

    public static void RemoveListener<T1, T2>(EventMessage eventMessage, Action<T1, T2> method)
    {
        Remove(eventMessage, method);
    }

    public static void RemoveListener<T1, T2, T3>(EventMessage eventMessage, Action<T1, T2, T3> method)
    {
        Remove(eventMessage, method);
    }

    public static void RemoveListener<T1, T2, T3, T4>(EventMessage eventMessage, Action<T1, T2, T3, T4> method)
    {
        Remove(eventMessage, method);
    }

    public static void RemoveListener<T1, T2, T3, T4, T5>(EventMessage eventMessage, Action<T1, T2, T3, T4, T5> method)
    {
        Remove(eventMessage, method);
    }
    #endregion

    #region Add

    private static void Add(EventMessage eventMessage, Delegate method)
    {
        if (!_events.TryGetValue(eventMessage, out var value))
        {
            _events[eventMessage] = method;
        }
        else
        {
            try
            {
                if (value.GetInvocationList().Contains(method))
                {
                    Debug.Log($"[EventController] Add - already added: {method.Target} - {method.Method.Name}");
                    return;
                }

                _events[eventMessage] = Delegate.Combine(value, method);
            }
            catch (Exception e) when (e is ArgumentException)
            {
                Debug.Log($"Subscribe! Signatures do not match: new - {method.Method} <> old - {value.Method}");
            }
        }
    }

    public static void AddListener(EventMessage eventMessage, Action method)
    {
        Add(eventMessage, method);
    }

    public static void AddListener<T>(EventMessage eventMessage, Action<T> method)
    {
        Add(eventMessage, method);
    }

    public static void AddListener<T1, T2>(EventMessage eventMessage, Action<T1, T2> method)
    {
        Add(eventMessage, method);
    }

    public static void AddListener<T1, T2, T3>(EventMessage eventMessage, Action<T1, T2, T3> method)
    {
        Add(eventMessage, method);
    }

    public static void AddListener<T1, T2, T3, T4>(EventMessage eventMessage, Action<T1, T2, T3, T4> method)
    {
        Add(eventMessage, method);
    }

    public static void AddListener<T1, T2, T3, T4, T5>(EventMessage eventMessage, Action<T1, T2, T3, T4, T5> method)
    {
        Add(eventMessage, method);
    }
    #endregion

    #region Invoke
    public static void Invoke(EventMessage eventMessage)
    {
        if (!_events.TryGetValue(eventMessage, out var value)) return;

        try
        {
            var action = (Action)value;
            action?.Invoke();
        }
        catch (Exception e) when (e is InvalidCastException)
        {
            Debug.Log($"Invoke! Signatures do not match: {value.Method}");
            Debug.Log(e);
        }
    }

    public static void Invoke<T>(EventMessage eventMessage, T arg)
    {
        if (!_events.TryGetValue(eventMessage, out var value)) return;

        try
        {
            var action = (Action<T>)value;
            action?.Invoke(arg);
        }
        catch (Exception e) when (e is InvalidCastException)
        {
            Debug.Log($"Invoke! Signatures do not match: new - {typeof(T)} <> old - {value.Method}");
            Debug.Log(e);
        }
    }

    public static void Invoke<T1, T2>(EventMessage eventMessage, T1 arg1, T2 arg2)
    {
        if (!_events.TryGetValue(eventMessage, out var value)) return;

        try
        {
            var action = (Action<T1, T2>)value;
            action?.Invoke(arg1, arg2);
        }
        catch (Exception e) when (e is InvalidCastException)
        {
            Debug.Log($"Invoke! Signatures do not match: new - {typeof(T1)} {typeof(T2)} <> old - {value.Method}");
            Debug.Log(e);
        }
    }

    public static void Invoke<T1, T2, T3>(EventMessage eventMessage, T1 arg1, T2 arg2, T3 arg3)
    {
        if (!_events.TryGetValue(eventMessage, out var value)) return;

        try
        {
            var action = (Action<T1, T2, T3>)value;
            action?.Invoke(arg1, arg2, arg3);
        }
        catch (Exception e) when (e is InvalidCastException)
        {
            Debug.Log($"Invoke! Signatures do not match: new - {typeof(T1)} {typeof(T2)} {typeof(T3)} <> old - {value.Method}");
            Debug.Log(e);
        }
    }

    public static void Invoke<T1, T2, T3, T4>(EventMessage eventMessage, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (!_events.TryGetValue(eventMessage, out var value)) return;

        try
        {
            var action = (Action<T1, T2, T3, T4>)value;
            action?.Invoke(arg1, arg2, arg3, arg4);
        }
        catch (Exception e) when (e is InvalidCastException)
        {
            Debug.Log($"Invoke! Signatures do not match: new - {typeof(T1)} {typeof(T2)} {typeof(T3)} {typeof(T4)} <> old - {value.Method}");
            Debug.Log(e);
        }
    }

    public static void Invoke<T1, T2, T3, T4, T5>(EventMessage eventMessage, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        if (!_events.TryGetValue(eventMessage, out var value)) return;

        try
        {
            var action = (Action<T1, T2, T3, T4, T5>)value;
            action?.Invoke(arg1, arg2, arg3, arg4, arg5);
        }
        catch (Exception e) when (e is InvalidCastException)
        {
            Debug.Log($"Invoke! Signatures do not match: new - {typeof(T1)} {typeof(T2)} {typeof(T3)} {typeof(T4)} {typeof(T5)} <> old - {value.Method}");
            Debug.Log(e);
        }
    }
    #endregion

    #region MonoBehaviour

    private static EventController mInstance;
    private static EventController Instance
    {
        get
        {
            if (mInstance == null)
                mInstance = new GameObject("EventController").AddComponent<EventController>();

            return mInstance;
        }
    }

    private void Awake()
    {
        if (mInstance != null && mInstance != this)
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void OnDestroy()
    {
        if (mInstance == this)
            mInstance = null;
    }

    #endregion

    public static void InvokeNextFrame(EventMessage eventMessage)
	{
        Instance.StartCoroutine(Instance.WaitOneFrameAndInvoke(eventMessage));
    }

    private IEnumerator WaitOneFrameAndInvoke(EventMessage eventMessage)
    {
        yield return null;
        EventController.Invoke(eventMessage);
    }
}
