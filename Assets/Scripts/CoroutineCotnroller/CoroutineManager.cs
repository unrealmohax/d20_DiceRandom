using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    private void Awake()
    {
        var objects = FindObjectsOfType<CoroutineManager>();

        if (objects.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
    }
}