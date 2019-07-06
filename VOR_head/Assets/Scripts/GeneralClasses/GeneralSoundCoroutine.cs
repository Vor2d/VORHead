using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSoundCoroutine : GeneralCoroutine
{
    [SerializeField] private AudioSource _AudioSource;

    protected override IEnumerator coro_function()
    {
        _AudioSource.Play();
        yield return new WaitForSeconds(_AudioSource.clip.length);
        coroutine_flag = false;
    }

    protected override IEnumerator coro_function(float time)
    {
        _AudioSource.Play();
        yield return new WaitForSeconds(time);
        _AudioSource.Stop();
        coroutine_flag = false;
    }

    protected override IEnumerator coro_function<T>(T parameter1)
    {
        _AudioSource.Play();
        yield return new WaitForSeconds(_AudioSource.clip.length);
        coroutine_flag = false;
    }
}
