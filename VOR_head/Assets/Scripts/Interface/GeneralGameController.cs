using UnityEngine;

public class GeneralGameController : MonoBehaviour
{
    public static float GameTimeScale = 1.0f;
    public static float UITimeScale = 1.0f;
    public static float GameDeltaTime = 0.0f;
    public static float UIDeltaTime = 0.0f;

    public bool UsingHeadForMenu = false;

    protected virtual void Update()
    {
        GameDeltaTime = Time.deltaTime * GameTimeScale;
        UIDeltaTime = Time.deltaTime * UITimeScale;
    }


}
