using UnityEngine;

/// <summary>
/// Parent game controller object;
/// </summary>
public class GeneralGameController : MonoBehaviour
{
    //Use GameTimeScale in the calculation if customized time scale is needed;
    public static float GameTimeScale = 1.0f;
    public static float UITimeScale = 1.0f;
    public static float GameDeltaTime = 0.0f;
    public static float UIDeltaTime = 0.0f;

    protected virtual void Update()
    {
        GameDeltaTime = Time.deltaTime * GameTimeScale;
        UIDeltaTime = Time.deltaTime * UITimeScale;
    }

    protected virtual void recenter_VR()
    {
        GeneralMethods.recenter_VR();
    }

    public virtual void restart()
    {
        GeneralMethods.restart_scene();
    }


}
