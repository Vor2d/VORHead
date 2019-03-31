using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class MD_UIController : MonoBehaviour
{

    [SerializeField] private MD_GameController MDGC_script;
    [SerializeField] private Animator UIC_Animator;
    [SerializeField] private Transform MenuTMP_TRANS;
    [SerializeField] private MD_TutorialController MDTC_script;

    [SerializeField] private Transform InitPage_TRANS;
    [SerializeField] private Transform InGamePage_TRANS;
    [SerializeField] private Transform PausePage_TRANS;
    [SerializeField] private Transform TutorialPage_TRANS;
    [SerializeField] private Transform ConfirmPage_TRANS;

    //[SerializeField] private Transform BackButton_TRANS;
    //[SerializeField] private Transform StartButton_TRANS;
    //[SerializeField] private Transform PauseButton_TRANS;
    //[SerializeField] private Transform ContinueButton_TRANS;
    //[SerializeField] private Transform ReStartButton_TRANS;
    //[SerializeField] private Transform YesButton_TRANS;
    //[SerializeField] private Transform NoButton_TRANS;
    //[SerializeField] private Transform TutorialButton_TRANS;
    //[SerializeField] private Transform EndTutorialButton_TRANS;

    [TextArea]
    [SerializeField] private string TutorialText;
    [TextArea]
    [SerializeField] private string ConfirmText;

    private Dictionary<string, Transform> page_dictionary;

    // Start is called before the first frame update
    void Start()
    {
        this.page_dictionary = new Dictionary<string, Transform>();

        init_dictionary();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void init_dictionary()
    {
        page_dictionary.Add(MD_StrDefiner.UIInitPage, InitPage_TRANS);
        page_dictionary.Add(MD_StrDefiner.UIConfirmPage, ConfirmPage_TRANS);
        page_dictionary.Add(MD_StrDefiner.UIInGamePage, InGamePage_TRANS);
        page_dictionary.Add(MD_StrDefiner.UIPausePage, PausePage_TRANS);
        page_dictionary.Add(MD_StrDefiner.UITutorialPage, TutorialPage_TRANS);
    }

    public void ToInit()
    {
        MenuTMP_TRANS.GetComponent<TextMeshPro>().text = TutorialText;
        MenuTMP_TRANS.GetComponent<MeshRenderer>().enabled = true;
        set_active_page(MD_StrDefiner.UIInitPage);
        UIC_Animator.SetTrigger(MD_StrDefiner.AnimatorNextStepTrigger_str);
    }

    private void set_active_page(string page_name)
    {
        foreach(Transform p_TRANS in page_dictionary.Values)
        {
            p_TRANS.gameObject.SetActive(false);
        }
        page_dictionary[page_name].gameObject.SetActive(true);
    }

    public void ToStartGame()
    {
        MDGC_script.start_game();

        UIC_Animator.SetTrigger(MD_StrDefiner.AnimatorNextStepTrigger_str);
    }

    public void ToInGame()
    {
        MDGC_script.resume_game();
        MenuTMP_TRANS.GetComponent<MeshRenderer>().enabled = false;
        set_active_page(MD_StrDefiner.UIInGamePage);
    }

    public void ToPause()
    {
        MenuTMP_TRANS.GetComponent<MeshRenderer>().enabled = false;
        set_active_page(MD_StrDefiner.UIPausePage);

        MDGC_script.pause_game();
    }

    public void ToConfirm()
    {
        MenuTMP_TRANS.GetComponent<TextMeshPro>().text = ConfirmText;
        MenuTMP_TRANS.GetComponent<MeshRenderer>().enabled = true;
        set_active_page(MD_StrDefiner.UIConfirmPage);
    }

    public void ToTutorial()
    {
        MenuTMP_TRANS.GetComponent<MeshRenderer>().enabled = false;
        set_active_page(MD_StrDefiner.UITutorialPage);
        MDTC_script.GetComponent<Animator>().SetTrigger(MD_StrDefiner.AnimatorStartTrigger_str);
    }

    public void ToBack()
    {
        MDGC_script.back_to_start_scene();
    }

    public void ToReStart()
    {
        MDGC_script.restart();
    }

    public void pause_button()
    {
        UIC_Animator.SetTrigger(MD_StrDefiner.AnimatorPauseTrigger_str);
    }

    public void start_button()
    {
        UIC_Animator.SetTrigger(MD_StrDefiner.AnimatorStartTrigger_str);
    }

    public void back_button()
    {
        UIC_Animator.SetTrigger(MD_StrDefiner.AnimatorBackTrigger_str);
    }

    public void restart_button()
    {
        UIC_Animator.SetTrigger(MD_StrDefiner.AnimatorReStartTrigger_str);
    }

    public void continue_button()
    {
        UIC_Animator.SetTrigger(MD_StrDefiner.AnimatorContinueTrigger_str);
    }

    public void yes_button()
    {
        UIC_Animator.SetTrigger(MD_StrDefiner.AnimatorYesTrigger_str);
    }

    public void no_button()
    {
        UIC_Animator.SetTrigger(MD_StrDefiner.AnimatorNoTrigger_str);
    }

    public void tutorial_button()
    {
        UIC_Animator.SetTrigger(MD_StrDefiner.AnimatorTutorialTrigger_str);
        start_tutorial();
    }

    private void start_tutorial()
    {
        MDGC_script.start_tutorial();
    }

    public void end_tutorial_button()
    {
        UIC_Animator.SetTrigger(MD_StrDefiner.AnimatorEndTutoTrigger_str);
    }


}
