using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RRTest : MonoBehaviour
{
    [SerializeField] private TextMesh tM1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Process_RR();
    }

    private void Process_RR()
    {
        tM1.text = (1.0f / Time.deltaTime).ToString("F4");
    }
}
