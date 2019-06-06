using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSoundCoroutine : GeneralCoroutine
{
    [SerializeField] private AudioSource _AudioSource;

    protected override IEnumerator coro_function<T>(T parameter1)
    {
        _AudioSource.Play();
        yield return new WaitForSeconds(_AudioSource.clip.length);
        coroutine_flag = false;
    }
}
