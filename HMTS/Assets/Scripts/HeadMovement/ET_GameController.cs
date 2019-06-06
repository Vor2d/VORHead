using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ET_GameController : MonoBehaviour {

    public Text game_speed_text;
    public Text FPS_text;

    private float fps;
    private float deltaTime;

	// Use this for initialization
	void Start () {
        this.fps = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		
        if(Input.GetKeyDown(KeyCode.W))
        {
            Time.timeScale *= 2.0f;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Time.timeScale /= 2.0f;
        }

        game_speed_text.text = "Speed: " + Time.timeScale;
        FPS_text.text = "FPS: " + fps;

        if (Input.GetKeyDown(KeyCode.X))
        {
            Time.timeScale = 0.0f;
        }

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;
    }
}
