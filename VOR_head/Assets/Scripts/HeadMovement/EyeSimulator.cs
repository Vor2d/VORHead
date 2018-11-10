using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeSimulator : MonoBehaviour {

    public ET_FileReader ETFR_script;

    public float TimeInterval = 0.01f;
    public int column;

    private int counter;
    private float timer;

	// Use this for initialization
	void Start () {
        this.counter = 0;
        this.timer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {

        timer += Time.deltaTime;
        counter = (int)(timer / TimeInterval);

        if(counter <= ETFR_script.data.Count)
        {
            if(column == 1)
            {
                float eye_h = 
                    ETFR_script.excute_linear_left(ETFR_script.data[counter][1]);
                transform.eulerAngles = new Vector3(0.0f, eye_h, 0.0f);
            }
            if (column == 4)
            {
                float eye_h =
                    ETFR_script.excute_linear_right(ETFR_script.data[counter][4]);
                transform.eulerAngles = new Vector3(0.0f, eye_h, 0.0f);
            }
        }
        counter++;

    }

    private void FixedUpdate()
    {
        //float left_eye_h = ETFR_script.excute_linear(ETFR_script.data[counter][1]);
        //transform.eulerAngles = new Vector3(0.0f, left_eye_h, 0.0f);
        //counter++;
    }


}
