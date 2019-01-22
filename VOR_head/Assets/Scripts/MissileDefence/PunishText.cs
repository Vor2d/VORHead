using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunishText : MonoBehaviour
{
    [SerializeField] float LifeTime = 1.0f;
    [SerializeField] Vector3 MoveDistance = Vector3.zero;

    private bool start_flag;

    private void Awake()
    {
        start_flag = false;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(start_flag)
        {
            transform.Translate(MoveDistance * Time.deltaTime * (1 / LifeTime));
        }
    }

    public void start_move()
    {
        start_flag = true;
        Destroy(gameObject, LifeTime);
    }
}
