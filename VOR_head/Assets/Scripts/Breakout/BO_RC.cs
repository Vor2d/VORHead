using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_RC : MonoBehaviour
{
    [Header("Objects")]
    public Transform Brick_par_TRANS;
    [Header("Scripts")]
    public Controller_Input CI;
    [Header("Perfabs")]
    public GameObject Brick_prefab;

    public List<Transform> Bricks_pool { get; set; }

    public static BO_RC IS;

    private void Awake()
    {
        IS = this;

        this.Bricks_pool = new List<Transform>();
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
