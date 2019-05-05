using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcuityGroup : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        turn_off_acuity();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void turn_off_acuity()
    {
        gameObject.SetActive(false);
    }

    public void turn_on_acuity()
    {
        gameObject.SetActive(true);
    }
}
