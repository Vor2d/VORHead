using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AcuityGroup : MonoBehaviour
{
    private const string path = "Sprites/Acuity/optotype bitmap/";

    public enum AcuityDirections { up,right,down,left};

    [SerializeField] private Transform AcuitySprite_TRANS;

    // Start is called before the first frame update
    void Start()
    {
        turn_off_acuity();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void init_acuity(int acuity_size)
    {
        AcuitySprite_TRANS.GetComponent<SpriteRenderer>().sprite = 
            Resources.Load<Sprite>(path + acuity_size.ToString());
    }

    public void turn_off_acuity()
    {
        gameObject.SetActive(false);
    }

    public void turn_on_acuity()
    {
        gameObject.SetActive(true);
    }

    public void turn_on_acuity(bool random_rotate)
    {
        if(random_rotate)
        {
            rotate();
        }
        gameObject.SetActive(true);
    }

    private void rotate()
    {
        int random_dir = UnityEngine.Random.Range(0, Enum.GetNames(typeof(AcuityDirections)).Length);
        AcuitySprite_TRANS.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, -(random_dir * 90.0f)));
    }
}
