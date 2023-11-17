using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class CoroutineController
{
    private static readonly CoroutineManager _coroutineManager;
    private static readonly Dictionary<string, CoroutineTask> _coroutines;
    private static readonly Dictionary<string, Queue<IEnumerator>> _queuedCoroutines;
    private static CoroutineTask _processQueuedRoutine;

    static CoroutineController()
    {
        _coroutines = new Dictionary<string, CoroutineTask>();
        _queuedCoroutines = new Dictionary<string, Queue<IEnumerator>>();

        var obj = Object.FindObjectOfType<CoroutineManager>();
        _coroutineManager = obj != null ? obj : new GameObject("CoroutineManager").AddComponent<CoroutineManager>();
    }

    public static CoroutineTask StartCoroutine(IEnumerator routine, string id = "", Action onComplete = null)
    {
        if (_coroutineManager == null) return null;
        if (routine == null)
        {
            Debug.Log("Coroutine is null");
            return null;
        }

        var coroutineController = new CoroutineTask(routine, id, Remove);
        if (!string.IsNullOrEmpty(id))
        {
            if (_coroutines.TryGetValue(id, out var result))
            {
                result?.Stop();
            }

            _coroutines[id] = coroutineController;
        }

        _coroutineManager.StartCoroutine(coroutineController.Start(onComplete));
        return coroutineController;
    }

    public static CoroutineTask DelayedInvoke(float delay, Action action, Action onComplete = null)
    {
        return StartCoroutine(WaitAndInvoke(delay, action), "", onComplete);
    }

    private static IEnumerator WaitAndInvoke(float delay, Action action)
    {
        yield return delay;
        action.Invoke();
    }

    public static void StopAllCoroutines()
    {
        if (_coroutineManager == null) return;

        StopAllQueuedCoroutines();

        _coroutines.Clear();
        _coroutineManager.StopAllCoroutines();

        Debug.Log("All coroutines are stopped");
    }

	public static bool Contains(string id)
	{
		if (_coroutines == null || string.IsNullOrEmpty(id))
			return false;
		
		return _coroutines.ContainsKey(id);
	}

	private static void Remove(string id)
    {
        if (!string.IsNullOrEmpty(id))
            _coroutines.Remove(id);
    }

    public static void AddCoroutineToQueue(IEnumerator routine, string group = "")
    {
        group = string.IsNullOrEmpty(group) ? routine.ToString() : group;

        if (_queuedCoroutines.TryGetValue(group, out var result))
            result.Enqueue(routine);
        else
        {
            _queuedCoroutines[group] = new Queue<IEnumerator>();
            _queuedCoroutines[group].Enqueue(routine);
        }

        if (_processQueuedRoutine == null || _processQueuedRoutine.State == CoroutineState.Finished)
            _processQueuedRoutine = StartCoroutine(ProcessQueuedRoutine(group));
    }

    public static void StopQueuedCoroutines(IEnumerator routine, string group = "")
    {
        if (_queuedCoroutines == null) return;

        if (!_queuedCoroutines.TryGetValue(string.IsNullOrEmpty(@group) ? routine.ToString() : @group, out var result)) return;

        Debug.Log($"Queued coroutines are stopped");
        result.Clear();
    }

    public static void StopAllQueuedCoroutines()
    {
        _queuedCoroutines.Clear();
    }

    public static void StopCoroutine(string id)
	{
        if (_coroutines.TryGetValue(id, out var task))
            StopCoroutine(task);
	}

    public static void StopCoroutine(CoroutineTask task)
    {
        task?.Stop();
    }
    

    private static IEnumerator ProcessQueuedRoutine(string key)
    {
        while (_queuedCoroutines[key].Count > 0)
        {
            if (_coroutineManager == null) yield break;

            yield return _coroutineManager.StartCoroutine(_queuedCoroutines[key].Dequeue());
        }

        yield return null;
    }
}