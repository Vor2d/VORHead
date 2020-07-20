using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyGroup : MonoBehaviour
{
    [SerializeField] private bool AutoResetHeight; //Auto matically reset position to 0;
    [SerializeField] private Transform Camera_TRANS;
    [SerializeField] private float Adjust_delay;

    private static float refer_height = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (AutoResetHeight) 
        {
            if (refer_height == 0.0f) { StartCoroutine(adjust_height_coro()); }
            else { adjust_height_inst(); }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator adjust_height_coro()
    {
        yield return new WaitForSeconds(Adjust_delay);
        refer_height = -Camera_TRANS.position.y;
        transform.position = new Vector3(0.0f, refer_height, 0.0f);
    }

    private void adjust_height_inst()
    {
        transform.position = new Vector3(0.0f, refer_height, 0.0f);
    }
}
