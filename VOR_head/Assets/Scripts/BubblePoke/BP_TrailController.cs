using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BP_TrailController : MonoBehaviour
{
    [SerializeField] private GameObject Path_Prefab;
    [SerializeField] private GameObject Chara_Prefab;
    [SerializeField] private GameObject Bubble_prefab;

    public bool Ready_flag { get; private set; }
    public Transform chara_TRANS { get; private set; }
    private Vector3 start_pos;
    private Color theme_color;
    private BP_RC BPRC;
    private Animator TCAnimator;
    private Transform path_TRANS;
    private Transform chara_save_TRANS;

    private void Awake()
    {
        this.TCAnimator = GetComponent<Animator>();
        this.start_pos = new Vector3();
        this.theme_color = Color.white;
        this.BPRC = null;
        this.chara_save_TRANS = null;
        this.path_TRANS = null;
        this.chara_TRANS = null;
        this.Ready_flag = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        GeneralMethods.check_ref<BP_RC>(ref BPRC, BP_StrDefiner.RC_name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void init_TC(BP_RC _BPRC, Vector3 _start_pos, Color _theme_color)
    {
        BPRC = _BPRC;
        start_pos = _start_pos;
        theme_color = _theme_color;
    }

    public void start_trail()
    {
        TCAnimator.SetTrigger(BP_StrDefiner.AniStartTrigger_str);
    }

    public void ToInit()
    {

        TCAnimator.SetTrigger(BP_StrDefiner.AniNextStepTrigger_str);
    }

    public void ToInstPathNChara()
    {
        path_TRANS = instantiate_path();
        chara_TRANS = instantiate_chara(path_TRANS);
    }

    private Transform instantiate_path()
    {
        GameObject path_OBJ = Instantiate(Path_Prefab, Vector3.zero, Quaternion.identity);
        path_OBJ.GetComponent<BP_Path>().init_path(BPRC,theme_color,start_pos);
        return path_OBJ.transform;
    }

    private Transform instantiate_chara(Transform path_TRANS)
    {
        GameObject charactor_OBJ = Instantiate(Chara_Prefab, start_pos, Quaternion.identity);
                                    charactor_OBJ.GetComponent<BP_Charactor>().
                                    init_chara(BPRC, path_TRANS,theme_color,this);
        return charactor_OBJ.transform;
    }

    public void ToStartTrail()
    {
        chara_TRANS.GetComponent<BP_Charactor>().start_chara();

        TCAnimator.SetTrigger(BP_StrDefiner.AniNextStepTrigger_str);
    }

    public void ToSaveState()
    {
        chara_save_TRANS = Instantiate(chara_TRANS);
        chara_save_TRANS.name = chara_TRANS.name + "#backup";
        chara_save_TRANS.GetComponent<BP_Charactor>().
                        set_state(chara_TRANS.GetComponent<BP_Charactor>());
        chara_save_TRANS.gameObject.SetActive(false);

        TCAnimator.SetTrigger(BP_StrDefiner.AniNextStepTrigger_str);
    }

    public void ToSpawnBubble()
    {
        if(BPRC.GC_script.RandomTrailFlag)
        {
            Vector2 RRange = BPRC.GC_script.RandomPredictRange;
            Vector3 position = chara_TRANS.GetComponent<BP_Charactor>().
                simulate_move(Random.Range(RRange.x, RRange.y),BPRC.GC_script.PredictInterval);

            Transform bubble_obj =
                    Instantiate(Bubble_prefab, position, new Quaternion()).transform;
            //bubble_obj.GetComponent<Bubble>().start_bubble(BPRC);
            bubble_obj.GetComponent<Bubble>().start_bubble(BPRC, chara_TRANS);
            BPRC.Bubble_TRANSs.Add(bubble_obj);
        }

        TCAnimator.SetTrigger(BP_StrDefiner.AniNextStepTrigger_str);
    }

    public void ToReverseState()
    {
        GeneralMethods.reset_animator_triggers(TCAnimator);
        chara_TRANS.GetComponent<BP_Charactor>().set_state(chara_save_TRANS);

        TCAnimator.SetTrigger(BP_StrDefiner.AniNextStepTrigger_str);
    }

    public void bubble_collided()
    {
        set_revers_ani();
    }

    private void set_revers_ani()
    {
        TCAnimator.SetTrigger(BP_StrDefiner.AniToCheckPoint_str);
    }

    public void ToWaitForSignal()
    {
        if(BPRC.GC_script.UsingSynchronizedBubble)
        {
            Ready_flag = true;
        }
        else
        {
            TCAnimator.SetTrigger(BP_StrDefiner.AniNextStepTrigger_str);
        }
    }

    public void WaitForSignal()
    {
        if(BPRC.GC_script.UsingSynchronizedBubble && BPRC.GC_script.Bubble_ready_flag)
        {
            Ready_flag = false;
            TCAnimator.SetTrigger(BP_StrDefiner.AniNextStepTrigger_str);
        }
    }

    public void ToCollisionAnimation()
    {
        StartCoroutine(CA_temp());
    }

    private IEnumerator CA_temp()
    {
        chara_TRANS.GetComponent<BP_Charactor>().set_theme_color(Color.red);
        chara_TRANS.GetComponent<BP_Charactor>().pause_chara();
        yield return new WaitForSeconds(1.0f);
        chara_TRANS.GetComponent<BP_Charactor>().set_theme_color(theme_color);
        TCAnimator.SetTrigger(BP_StrDefiner.AniNextStepTrigger_str);
    }
}
