using UnityEngine;
using System.Collections.Generic;

public class RC : MonoBehaviour
{
    //Transforms;
    public List<Transform> Charts_TRANSs;
    //Prefabs;
    public GameObject DotsPS_Prefab;
    public GameObject Chart_Prefab;

    public static int DotReference;

    public static RC IS;

    private void Awake()
    {
        DotReference = 0;

        IS = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
