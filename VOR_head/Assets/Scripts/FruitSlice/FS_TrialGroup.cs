using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_TrialGroup : MonoBehaviour
{
    [SerializeField] private Transform[] DebugIndicators;
    [SerializeField] private Texture2D texture;
    [SerializeField] private Transform DummyMesh_TRANS;
    [SerializeField] private int StarsToUnlock;

    public Transform[] Indicators { get { return DebugIndicators; } }
    public Texture2D Texture { get { return texture; } }
    public int Stars_to_unlock { get { return StarsToUnlock; } }

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
