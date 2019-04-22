using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
public class MD_TutorialController : MonoBehaviour
{
    [SerializeField] private MD_UIController MDUIC_script;
    [SerializeField] private Transform TutorialTMP_TRANS;
    [SerializeField] private Transform[] Position_Indicator_TRANSs;
    [SerializeField] private Transform LineRender_TRANS;
    [SerializeField] private Transform HeadCursor_TRANS;
    [SerializeField] private GameObject Asteroid_Prefab;
    [SerializeField] private AmmoSystem AS_script;
    [SerializeField] private Transform[] City_Shield_TRANSs;
    [SerializeField] private Transform[] Reload_TRANSs;

    [SerializeField] private float blink_inter_time = 0.1f;
    [TextArea]
    [SerializeField] private string[] TutorialStrings;

    public float WaitTime { get; private set; }

    private Animator MDTCAnimator;
    private int text_index;
    private float wait_timer;
    private bool wait_timer_flag;
    private List<IEnumerator> coroutine_insts;
    private Color city_s_init_color;
    private Color reload_init_color;

    // Start is called before the first frame update
    void Start()
    {
        this.MDTCAnimator = GetComponent<Animator>();

        this.text_index = 0;
        this.wait_timer = 0.0f;
        this.wait_timer_flag = false;
        this.WaitTime = 0.0f;
        this.coroutine_insts = new List<IEnumerator>();
        this.city_s_init_color = 
                    City_Shield_TRANSs[0].GetComponent<MeshRenderer>().material.color;
        this.reload_init_color =
                        Reload_TRANSs[0].GetComponent<MeshRenderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if(wait_timer_flag)
        {
            wait_timer += Time.deltaTime;
        }
    }

    public void set_wait_time(float wait_time)
    {
        WaitTime = wait_time;
    }

    public void ToInit()
    {
        TutorialTMP_TRANS.GetComponent<MeshRenderer>().enabled = false;
        AS_script.set_ammo(1);
    }

    private void start_wait_time(ref bool timer_flag)
    {
        timer_flag = true;
    }

    public void ToWelcome()
    {
        show_next_T_TMP(0);
        start_wait_time(ref wait_timer_flag);
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
        fade_out(wait_timer,5.0f);
        check_timer(ref wait_timer,ref wait_timer_flag, 5.0f);
    }

    private void fade_out(float timer, float target_time)
    {
        TutorialTMP_TRANS.GetComponent<TextMeshPro>().faceColor = 
                                new Color(1.0f,1.0f,1.0f,(1.0f - timer/target_time));
    }

    public void ExitWelcome()
    {
        TutorialTMP_TRANS.GetComponent<TextMeshPro>().faceColor =
                                                        new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    private void check_timer(ref float timer,ref bool timer_flag, float target_time)
    {
        if (timer > target_time)
        {
            timer = 0.0f;
            timer_flag = false;
            MDTCAnimator.SetTrigger(MD_StrDefiner.AnimatorNextStepTrigger_str);
            turn_off_line();
            turn_off_T_TMP();
        }
    }

    public void ToCursor()
    {
        start_wait_time(ref wait_timer_flag);
        show_next_T_TMP(0);
    }

    public void Cursor()
    {
        show_line(Position_Indicator_TRANSs[6].position, HeadCursor_TRANS.position);
        check_timer(ref wait_timer,ref wait_timer_flag, WaitTime);
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
        start_wait_time(ref wait_timer_flag);
        show_next_T_TMP(0);
        GameObject fake_asteroid_OBJ = Instantiate(Asteroid_Prefab, 
                                                        Position_Indicator_TRANSs[1].position, 
                                                        Quaternion.identity);
        fake_asteroid_OBJ.GetComponent<Missile>().set_target(City_Shield_TRANSs[0]);
        fake_asteroid_OBJ.GetComponent<Missile>().start_move(0.001f);
        show_line(Position_Indicator_TRANSs[6].position, fake_asteroid_OBJ.transform.position);
    }

    public void Enemy()
    {
        check_timer(ref wait_timer,ref wait_timer_flag,WaitTime);
    }

    public void ToCity()
    {
        start_wait_time(ref wait_timer_flag);
        show_next_T_TMP(0);
        //show_line(Position_Indicator_TRANSs[6].position, Position_Indicator_TRANSs[2].position);
        blink_cities();
    }

    public void blink_cities()
    {
        foreach(Transform city_shield in City_Shield_TRANSs)
        {
            coroutine_insts.Add(GeneralMethods.blink_object(city_shield.gameObject,
                                                            blink_inter_time, Color.white));
        }
        start_insts_list();
    }

    private void start_insts_list()
    {
        foreach(IEnumerator c_inst in coroutine_insts)
        {
            StartCoroutine(c_inst);
        }
    }

    public void City()
    {
        check_timer(ref wait_timer,ref wait_timer_flag, WaitTime);
    }

    public void ExitCity()
    {
        stop_insts_list();
        set_back_city_color();
    }

    private void set_back_city_color()
    {
        foreach(Transform city_TRANS in City_Shield_TRANSs)
        {
            city_TRANS.GetComponent<MeshRenderer>().material.color = city_s_init_color;
        }
    }

    private void stop_insts_list()
    {
        foreach (IEnumerator c_inst in coroutine_insts)
        {
            StopCoroutine(c_inst);
        }
        coroutine_insts = new List<IEnumerator>();
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
        start_wait_time(ref wait_timer_flag);
        show_next_T_TMP(7);
        //show_line(Position_Indicator_TRANSs[6].position, Position_Indicator_TRANSs[3].position);
        AS_script.set_ammo(0);
        blink_reload();
    }

    private void blink_reload()
    {
        foreach(Transform reload_TRANS in Reload_TRANSs)
        {
            coroutine_insts.Add(GeneralMethods.blink_object(reload_TRANS.gameObject, 
                                                                blink_inter_time, Color.red));
        }
        start_insts_list();
    }

    public void Reload()
    {
        check_timer(ref wait_timer,ref wait_timer_flag, WaitTime);
    }

    private void set_back_reload_color()
    {
        foreach (Transform reload_TRANS in Reload_TRANSs)
        {
            reload_TRANS.GetComponent<MeshRenderer>().material.color = reload_init_color;
        }
    }

    public void ToReload_DE()
    {
        show_next_T_TMP(7);
        spawn_enemies_reload();
        StartCoroutine(check_asteroid());
    }

    private void spawn_enemies_reload()
    {
        GameObject fake_asteroid_TRANS1 = Instantiate(Asteroid_Prefab,
                                                        Position_Indicator_TRANSs[1].position,
                                                        Quaternion.identity);
        GameObject fake_asteroid_TRANS2 = Instantiate(Asteroid_Prefab,
                                                        Position_Indicator_TRANSs[4].position,
                                                        Quaternion.identity);
        GameObject fake_asteroid_TRANS3 = Instantiate(Asteroid_Prefab,
                                                        Position_Indicator_TRANSs[5].position,
                                                        Quaternion.identity);
        fake_asteroid_TRANS1.GetComponent<Missile>().set_target(City_Shield_TRANSs[0]);
        fake_asteroid_TRANS2.GetComponent<Missile>().set_target(City_Shield_TRANSs[1]);
        fake_asteroid_TRANS3.GetComponent<Missile>().set_target(City_Shield_TRANSs[2]);
        fake_asteroid_TRANS1.GetComponent<Missile>().start_move(0.001f);
        fake_asteroid_TRANS2.GetComponent<Missile>().start_move(0.001f);
        fake_asteroid_TRANS3.GetComponent<Missile>().start_move(0.001f);
    }

    public void ExitReload_DE()
    {
        stop_insts_list();
        set_back_reload_color();
    }

    public void ToEnd()
    {
        show_next_T_TMP(0);
        MDUIC_script.tutorial_finished();
    }


}
