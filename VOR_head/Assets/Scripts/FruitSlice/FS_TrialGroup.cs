using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_TrialGroup : MonoBehaviour
{
    [SerializeField] private Transform[] DebugIndicators;
    [SerializeField] private Texture2D texture;
    [SerializeField] private Transform DummyMesh_TRANS;

    public Transform[] Indicators { get { return DebugIndicators; } }
    public Texture2D Texture { get { return texture; } }

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        clear_debug();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void clear_debug()
    {
        DummyMesh_TRANS.gameObject.SetActive(false);
    }


}
