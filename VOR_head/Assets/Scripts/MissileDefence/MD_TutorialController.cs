﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
public class MD_TutorialController : MonoBehaviour
{
    [SerializeField] private Transform TutorialTMP_TRANS;
    [SerializeField] private Transform[] Position_Indicator_TRANSs;
    [SerializeField] private Transform LineRender_TRANS;
    [SerializeField] private Transform HeadCursor_TRANS;
    [SerializeField] private GameObject Asteroid_Prefab;
    [SerializeField] private AmmoSystem AS_script;

    [SerializeField] private float InterTime;
    [TextArea]
    [SerializeField] private string[] TutorialStrings;

    private Animator MDTCAnimator;
    private int text_index;
    private float inter_timer;
    private bool inter_timer_flag;

    // Start is called before the first frame update
    void Start()
    {
        this.MDTCAnimator = GetComponent<Animator>();

        this.text_index = 0;
        this.inter_timer = InterTime;
        this.inter_timer_flag = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(inter_timer_flag)
        {
            inter_timer -= Time.deltaTime;
        }
    }

    public void ToInit()
    {
        TutorialTMP_TRANS.GetComponent<MeshRenderer>().enabled = false;
        AS_script.set_ammo(1);
    }

    public void ToWelcome()
    {
        show_next_T_TMP(0);
        inter_timer_flag = true;
    }

    private void show_next_T_TMP(int pos_index)
    {
        TutorialTMP_TRANS.position = Position_Indicator_TRANSs[pos_index].position;
        TutorialTMP_TRANS.GetComponent<TextMeshPro>().text = TutorialStrings[text_index];
        text_index++;
        TutorialTMP_TRANS.GetComponent<MeshRenderer>().enabled = true;
    }

    private void turn_off_T_TMP()
    {
        TutorialTMP_TRANS.GetComponent<MeshRenderer>().enabled = false;
    }

    public void Welcome()
    {
        fade_out();
        check_timer(ref inter_timer,ref inter_timer_flag, InterTime);
    }

    private void fade_out()
    {
        TutorialTMP_TRANS.GetComponent<TextMeshPro>().faceColor = 
                                new Color(1.0f,1.0f,1.0f,(1.0f - inter_timer/InterTime));
    }

    public void ExitWelcome()
    {
        TutorialTMP_TRANS.GetComponent<TextMeshPro>().faceColor =
                                                        new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    private void check_timer(ref float timer,ref bool timer_flag, float Time)
    {
        if (timer < 0.0f)
        {
            timer = Time;
            timer_flag = false;
            MDTCAnimator.SetTrigger(MD_StrDefiner.AnimatorNextStepTrigger_str);
            turn_off_line();
            turn_off_T_TMP();
        }
    }

    public void ToCursor()
    {
        inter_timer_flag = true;
        show_next_T_TMP(0);
    }

    public void Cursor()
    {
        show_line(Position_Indicator_TRANSs[6].position, HeadCursor_TRANS.position);
        check_timer(ref inter_timer,ref inter_timer_flag, InterTime);
    }

    private void show_line(Vector3 start_pos, Vector3 end_pos)
    {
        Vector3[] positions = new Vector3[2] { start_pos, end_pos };
        LineRender_TRANS.GetComponent<LineRenderer>().SetPositions(positions);
        LineRender_TRANS.GetComponent<LineRenderer>().enabled = true;
    }

    private void turn_off_line()
    {
        LineRender_TRANS.GetComponent<LineRenderer>().enabled = false;
    }

    public void ToEnemy()
    {
        inter_timer_flag = true;
        show_next_T_TMP(0);
        GameObject fake_asteroid_OBJ = Instantiate(Asteroid_Prefab, 
                                                        Position_Indicator_TRANSs[1].position, 
                                                        Quaternion.identity);
        //fake_asteroid_OBJ.GetComponent<Missile>().set_target(fake_asteroid_OBJ.transform);
        show_line(TutorialTMP_TRANS.position, fake_asteroid_OBJ.transform.position);
    }

    public void Enemy()
    {
        check_timer(ref inter_timer,ref inter_timer_flag,InterTime);
    }

    public void ToCity()
    {
        inter_timer_flag = true;
        show_next_T_TMP(0);
        show_line(TutorialTMP_TRANS.position, Position_Indicator_TRANSs[2].position);
    }

    public void City()
    {
        check_timer(ref inter_timer,ref inter_timer_flag, InterTime);
    }

    public void ToDE()
    {
        show_next_T_TMP(0);
        StartCoroutine(check_asteroid());
    }

    public void DE()
    {
        AS_script.set_ammo(1);
    }

    private IEnumerator check_asteroid()
    {
        bool check_flag = true;
        while(check_flag)
        {
            yield return new WaitForSeconds(0.1f);
            if (GameObject.FindGameObjectWithTag(MD_StrDefiner.Enemy_tag) == null)
            {
                check_flag = false;
                MDTCAnimator.SetTrigger(MD_StrDefiner.AnimatorNextStepTrigger_str);
                turn_off_line();
                turn_off_T_TMP();
            }
        }
    }

    public void ToReload()
    {
        inter_timer_flag = true;
        show_next_T_TMP(0);
        show_line(TutorialTMP_TRANS.position, Position_Indicator_TRANSs[3].position);
        AS_script.set_ammo(0);
    }

    public void Reload()
    {
        check_timer(ref inter_timer,ref inter_timer_flag, InterTime);
    }

    public void ToReload_DE()
    {
        show_next_T_TMP(0);
        GameObject fake_asteroid_TRANS1 = Instantiate(Asteroid_Prefab,
                                                        Position_Indicator_TRANSs[1].position,
                                                        Quaternion.identity);
        GameObject fake_asteroid_TRANS2 = Instantiate(Asteroid_Prefab,
                                                        Position_Indicator_TRANSs[4].position,
                                                        Quaternion.identity);
        GameObject fake_asteroid_TRANS3 = Instantiate(Asteroid_Prefab,
                                                        Position_Indicator_TRANSs[5].position,
                                                        Quaternion.identity);
        StartCoroutine(check_asteroid());
    }

    public void ToEnd()
    {
        show_next_T_TMP(0);
    }


}
