using System;
using UnityEngine;

/// <summary>
/// Parent game controller object;
/// </summary>
public class GeneralGameController : MonoBehaviour
{
    //Camera group to adjunst in VR;
    [SerializeField] private Transform CameraParent_TRANS;
    [SerializeField] private Transform Camera_TRANS;

    //Use GameTimeScale in the calculation if customized time scale is needed;
    public static float GameTimeScale = 1.0f;
    public static float UITimeScale = 1.0f;
    public static float GameDeltaTime = 0.0f;
    public static float UIDeltaTime = 0.0f;

    public Guid GGC_ID { get; private set; }

    private void Awake()
    {
        GGC_ID = Guid.NewGuid();
    }

    protected virtual void Update()
    {
        GameDeltaTime = Time.deltaTime * GameTimeScale;
        UIDeltaTime = Time.deltaTime * UITimeScale;
    }

    protected virtual void recenter_VR()
    {
        if (CameraParent_TRANS == null || Camera_TRANS == null) { GeneralMethods.recenter_VR(); }
        else
        { 
            float refer_height = GeneralMethods.recenter_VR(CameraParent_TRANS, Camera_TRANS); 
            if(CameraParent_TRANS.TryGetComponent<BodyParent>(out BodyParent BP))
            {
                BP.set_ref_hei(refer_height);
            }
        }
    }

    public virtual void restart()
    {
        GeneralMethods.restart_scene();
    }


}
