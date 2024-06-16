using System;
using System.Collections;
using UnityEngine;

public class InvokeHelper : MonoBehaviour
{
    public static InvokeHelper Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Invoke(Action action, float delay)
    {
        StartCoroutine(InvokeCoroutine(action, delay));
    }

    private IEnumerator InvokeCoroutine(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action();
    }
}