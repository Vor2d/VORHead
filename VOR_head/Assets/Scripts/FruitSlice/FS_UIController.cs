using UnityEngine;
using System.Collections.Generic;

public class FS_UIController : MonoBehaviour
{
    [SerializeField] private FS_RC FSRC;
    [SerializeField] private Animator UIAnimator;
    [Header("Pages")]
    [SerializeField] private Transform InitPage_TRANS;
    [SerializeField] private Transform InGamePage_TRANS;

    private List<Transform> Pages_TRANSs;

    private void Start()
    {
        this.Pages_TRANSs = new List<Transform>();

        init_page_list();
    }

    private void init_page_list()
    {
        Pages_TRANSs.Add(InitPage_TRANS);
    }

    #region Animator
    public void ToInit()
    {
        GeneralMethods.active_UI_page(InitPage_TRANS,Pages_TRANSs);
    }

    public void ToStartGame()
    {
        GeneralMethods.active_UI_page(InGamePage_TRANS, Pages_TRANSs);
        UIAnimator.SetTrigger(FS_SD.AniNextStep_str);
    }

    public void ToInGame()
    {

    }
    #endregion

    #region Buttons
    public void restart_button()
    {
        FSRC.GC_script.restart();
    }

    public void start_button()
    {
        FSRC.GC_script.start_game();
        UIAnimator.SetTrigger(FS_SD.AniStart_str);
    }

    public void quit_button()
    {
        FSRC.GC_script.quit_game();
    }
    #endregion

}
