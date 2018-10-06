using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MD_GameController : MonoBehaviour {

    public float InstRandomRange = 9.0f;
    public float InstY = 4.0f;
    public float InstZ = 10.0f;

    public GameObject MissilePrefab;
    //private Missile Missile_Script;

    public GameObject City1_GO;
    public GameObject City2_GO;
    public GameObject City3_GO;

    private List<GameObject> cities;

    // Use this for initialization
    void Start () {

        this.cities = new List<GameObject>();

        cities.Add(City1_GO);
        cities.Add(City2_GO);
        cities.Add(City3_GO);

        //Missile_Script.set_target(City1_GO.transform);
        //Missile_Script.start_move();

        //Missile_Script.face(City1_GO.transform.position);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ToInstantiatetMissile()
    {
        GameObject Missile =
                (GameObject)Instantiate(MissilePrefab, RandomPosGenerate(), new Quaternion());
        Missile Missile_Script = Missile.GetComponent<Missile>();
        int city_number = Random.Range(0, 3);
        Debug.Log("city_number " + city_number);
        Missile_Script.set_target(cities[city_number].transform);
        //Missile_Script.start_move();
        Missile_Script.start_flag = true;
    }

    private Vector3 RandomPosGenerate()
    {
        return new Vector3(Random.Range(-InstRandomRange, InstRandomRange),
                            InstY, InstZ);
    }
}
