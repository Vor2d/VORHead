using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetGroup : MonoBehaviour
{
    [SerializeField] private Transform Target_TRANS;
    [SerializeField] private Vector3 InitPosition;
    [SerializeField] private Vector3 EndPosition;

    private bool moving;
    private bool end_move;

    private void Awake()
    {
        this.moving = false;
        this.end_move = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        init_position();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void move_target(float speed)
    {
        if (!moving) { StartCoroutine(move_corotine(speed)); }
    }

    public void end_moving()
    {
        end_move = true;
    }

    private IEnumerator move_corotine(float speed)
    {
        moving = true;
        Target_TRANS.position = InitPosition;
        end_move = false;
        while (Target_TRANS.position.x < EndPosition.x && !end_move)
        {
            Target_TRANS.position += new Vector3(speed*Time.deltaTime, 0.0f, 0.0f);
            yield return null;
        }
        Target_TRANS.position = InitPosition;
        end_move = false;
        moving = false;
    }

    public void enlarge_target(float amount)
    {
        Target_TRANS.localScale += new Vector3(amount, amount, amount);
    }

    public void shrink_target(float amount)
    {
        if (Target_TRANS.localScale.x - amount > 0.0f) 
        {
            Target_TRANS.localScale -= new Vector3(amount, amount, amount);
        }
    }

    private void init_position()
    {
        Target_TRANS.position = InitPosition;
    }
}
