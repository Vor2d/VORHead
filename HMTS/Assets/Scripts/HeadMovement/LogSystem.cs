using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogSystem : MonoBehaviour
{
    public System.Diagnostics.Stopwatch stop_watch { get; private set; }

    private void Awake()
    {
        stop_watch = new System.Diagnostics.Stopwatch();
    }

    public void start_stopwatch()
    {
        stop_watch.Start();
    }

}
