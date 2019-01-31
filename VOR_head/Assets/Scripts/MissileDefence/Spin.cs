using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour {

    [SerializeField] private bool RandomSpeed = false;
    [SerializeField] private Vector3 RandomRange = new Vector3(10.0f, 10.0f, 10.0f);
    [SerializeField] private Vector3 SpinSpeed = new Vector3(0.0f, 10.0f, 0.0f);
    //[SerializeField] private SpinMode ObjectSpinMode;

	// Use this for initialization
	void Start () {
		if(RandomSpeed)
        {
            SpinSpeed = new Vector3(Random.Range(-RandomRange.x, RandomRange.x),
                                    Random.Range(-RandomRange.y, RandomRange.y),
                                    Random.Range(-RandomRange.z, RandomRange.z));
        }
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(SpinSpeed * GeneralGameController.GameDeltaTime);

    }
}

//public enum SpinMode { A_forWard, all_axes };
