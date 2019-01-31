using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MD_UIController : MonoBehaviour
{

    [SerializeField] private MD_GameController MDGC_script;
    [SerializeField] private Animator UIC_Animator;
    [SerializeField] private Transform MenuTMP_TRANS;
    [SerializeField] private Transform BackButton_TRANS;
    [SerializeField] private Transform StartButton_TRANS;
    [SerializeField] private Transform PauseButton_TRANS;
    [SerializeField] private Transform ContinueButton_TRANS;
    [SerializeField] private Transform ReStartButton_TRANS;
    [SerializeField] private Transform YesButton_TRANS;
    [SerializeField] private Transform NoButton_TRANS;

    [TextArea]
    [SerializeField] private string TutorialText;
    [TextArea]
    [SerializeField] private string ConfirmText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToInit()
    {
        MenuTMP_TRANS.GetComponent<TextMeshPro>().text = TutorialText;
        MenuTMP_TRANS.GetComponent<MeshRenderer>().enabled = true;
        BackButton_TRANS.gameObject.SetActive(true);
        StartButton_TRANS.gameObject.SetActive(true);
        PauseButton_TRANS.gameObject.SetActive(false);
        ContinueButton_TRANS.gameObject.SetActive(false);
        ReStartButton_TRANS.gameObject.SetActive(false);
        YesButton_TRANS.gameObject.SetActive(false);
        NoButton_TRANS.gameObject.SetActive(false);
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
        BackButton_TRANS.gameObject.SetActive(false);
        StartButton_TRANS.gameObject.SetActive(false);
        PauseButton_TRANS.gameObject.SetActive(true);
        ContinueButton_TRANS.gameObject.SetActive(false);
        ReStartButton_TRANS.gameObject.SetActive(false);
        YesButton_TRANS.gameObject.SetActive(false);
        NoButton_TRANS.gameObject.SetActive(false);
    }

    public void ToPause()
    {
        MenuTMP_TRANS.GetComponent<MeshRenderer>().enabled = false;
        BackButton_TRANS.gameObject.SetActive(true);
        StartButton_TRANS.gameObject.SetActive(false);
        PauseButton_TRANS.gameObject.SetActive(false);
        ContinueButton_TRANS.gameObject.SetActive(true);
        ReStartButton_TRANS.gameObject.SetActive(true);
        YesButton_TRANS.gameObject.SetActive(false);
        NoButton_TRANS.gameObject.SetActive(false);

        MDGC_script.pause_game();
    }

    public void ToConfirm()
    {
        MenuTMP_TRANS.GetComponent<TextMeshPro>().text = ConfirmText;
        MenuTMP_TRANS.GetComponent<MeshRenderer>().enabled = true;
        BackButton_TRANS.gameObject.SetActive(false);
        StartButton_TRANS.gameObject.SetActive(false);
        PauseButton_TRANS.gameObject.SetActive(false);
        ContinueButton_TRANS.gameObject.SetActive(false);
        ReStartButton_TRANS.gameObject.SetActive(false);
        YesButton_TRANS.gameObject.SetActive(true);
        NoButton_TRANS.gameObject.SetActive(true);
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
        UIC_Animator.SetTrigger(MD_StrDefiner.AnimatorConfirmTrigger_str);
        UIC_Animator.SetBool(MD_StrDefiner.AnimatorBackBool_str, true);
    }

    public void restart_button()
    {
        UIC_Animator.SetTrigger(MD_StrDefiner.AnimatorConfirmTrigger_str);
        UIC_Animator.SetBool(MD_StrDefiner.AnimatorReStartBool_str, true);
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
        UIC_Animator.SetBool(MD_StrDefiner.AnimatorBackBool_str, false);
        UIC_Animator.SetBool(MD_StrDefiner.AnimatorReStartBool_str, false);
    }
}
