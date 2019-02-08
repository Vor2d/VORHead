using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class City : MonoBehaviour {

    public TextMesh BIndicatorText;
    [SerializeField] private GameObject HittedText_Prefab;
    [SerializeField] private GameObject DestroiedText_Prefab;
    [SerializeField] private Transform Shield_TRANS;

    [SerializeField] private int Health;
    [SerializeField] private bool UsingShield = true;
    [SerializeField] private int CityLifeOffset = 1;
    [SerializeField] private Vector3 TextOffSet = Vector3.zero;

    private MD_GameController MD_GC_Script;

    public bool already_destroied { get; private set; }
    public int curr_health { get; private set; }

    // Use this for initialization
    void Start () {
        if(MD_GC_Script == null)
        {
            MD_GC_Script = GameObject.Find(MD_StrDefiner.GameController_name).
                                                GetComponent<MD_GameController>();
        }

        this.already_destroied = false;
        this.curr_health = Health + CityLifeOffset;

    }
	
	// Update is called once per frame
	void Update () {
        BIndicatorText.text = curr_health.ToString();
	}

    public void get_hit()
    {
        curr_health--;
        if(check_shield_broke())
        {
            disable_shield();
        }
        
        if (check_destroied())
        {
            if(!already_destroied)
            {
                already_destroied = true;
                MD_GC_Script.City_destroied = true;
                if (MD_GC_Script.UsingPunishSystem)
                {
                    instantiate_punish_text(true);
                }
                fake_destroy();
            }
        }
        else
        {
            MD_GC_Script.City_hitted();
            if (MD_GC_Script.UsingPunishSystem)
            {
                instantiate_punish_text(false);
            }
        }

        GetComponent<AudioSource>().Play();

    }

    private void fake_destroy()
    {
        gameObject.SetActive(false);
    }

    public void resummon()
    {
        gameObject.SetActive(true);
        curr_health = Health + CityLifeOffset;
        Shield_TRANS.GetComponent<MeshRenderer>().enabled = true;
        already_destroied = false;
    }

    private bool check_shield_broke()
    {
        if(UsingShield && curr_health == CityLifeOffset)
        {
            return true;
        }
        return false;
    }

    private void disable_shield()
    {
        Shield_TRANS.GetComponent<MeshRenderer>().enabled = false;
    }

    private bool check_destroied()
    {
        return curr_health <= 0;
    }

    private void instantiate_punish_text(bool destroied)
    {
        GameObject punish_text;
        if (!destroied)
        {
            punish_text = Instantiate(HittedText_Prefab,
                                        transform.position + TextOffSet, Quaternion.identity);
            punish_text.GetComponent<TextMeshPro>().text +=
                                    "\n- " + MD_GC_Script.GetHitPunish;
        }
        else
        {
            punish_text = Instantiate(DestroiedText_Prefab,
                            transform.position + TextOffSet, Quaternion.identity);
            punish_text.GetComponent<TextMeshPro>().text +=
                                    "\n- " + MD_GC_Script.GetDestroyPunish;
        }
        punish_text.GetComponent<PunishText>().start_move();
    }
}
