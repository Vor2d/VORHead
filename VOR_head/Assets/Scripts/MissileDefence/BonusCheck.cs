using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BonusCheck : MonoBehaviour
{
    
    [SerializeField] private GameObject[] BonusTexts_Prefabs;

    [SerializeField] private Vector3 TextOffSet = Vector3.zero;
    [SerializeField] private int BonusLowerOffset = 0;
    [SerializeField] private float LifeTime = 1.0f;

    private MD_GameController MDGC_script;

    private int Bonus_get;

    private void Awake()
    {
        init();
    }

    // Start is called before the first frame update
    void Start()
    {
        change_score();
        instantiate_text();
        Destroy(gameObject, LifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void init()
    {
        init(0);
    }

    public void init(int bonus)
    {
        if(MDGC_script == null)
        {
            MDGC_script = GameObject.Find(MD_StrDefiner.GameController_name).
                                            GetComponent<MD_GameController>();
        }
        this.Bonus_get = bonus;
    }

    private void change_score()
    {
        int new_bonus_get = check_new_bonus();
        if (new_bonus_get < 0)
        {
            return;
        }
        MDGC_script.add_bonus_score(MDGC_script.BonusScores[new_bonus_get]);
    }

    private void instantiate_text()
    {
        int new_bonus_get = check_new_bonus();
        if(new_bonus_get < 0)
        {
            return;
        }
        GameObject bonus_text_Prefab = BonusTexts_Prefabs[new_bonus_get];

        GameObject bonus_text_OBJ = Instantiate(bonus_text_Prefab, 
                                                transform.position + TextOffSet,
                                                Quaternion.identity, transform);
        bonus_text_OBJ.GetComponent<TextMeshPro>().text +=
                                "\n+ " + MDGC_script.BonusScores[new_bonus_get];


    }

    private int check_new_bonus()
    {
        int new_bonus_get = Bonus_get - (BonusLowerOffset + 1);
        if (new_bonus_get < 0)
        {
            return -1;
        }
        if (new_bonus_get < BonusTexts_Prefabs.Length)
        {
            return new_bonus_get;
        }
        else
        {
            return BonusTexts_Prefabs.Length - 1;
        }
    }
}
