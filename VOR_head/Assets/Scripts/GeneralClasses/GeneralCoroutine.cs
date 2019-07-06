using System.Collections;
using UnityEngine;

/// <summary>
/// Coroutine with easy lock;
/// </summary>
public class GeneralCoroutine : MonoBehaviour
{
    protected bool coroutine_flag;

    protected virtual void Start()
    {
        this.coroutine_flag = false;
    }

    public void start_coroutine()
    {
        if (!coroutine_flag)
        {
            coroutine_flag = true;
            StartCoroutine(coro_function());
        }
    }

    public void start_coroutine(float time)
    {
        if (!coroutine_flag)
        {
            coroutine_flag = true;
            StartCoroutine(coro_function(time));
        }
    }

    public void start_coroutine<T>(T parameter1)
    {
        if(!coroutine_flag)
        {
            coroutine_flag = true;
            StartCoroutine(coro_function<T>(parameter1));
        }
    }

    protected virtual IEnumerator coro_function()
    {
        yield return null;
        coroutine_flag = false;
    }

    protected virtual IEnumerator coro_function(float time)
    {
        yield return new WaitForSeconds(time);
        coroutine_flag = false;
    }

    protected virtual IEnumerator coro_function<T>(T parameter1)
    {
        yield return null;
        coroutine_flag = false;
    }
}
