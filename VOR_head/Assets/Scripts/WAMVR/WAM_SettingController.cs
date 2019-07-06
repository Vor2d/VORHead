using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WAM_SettingController : MonoBehaviour
{
    [SerializeField] private Animator SCAnimator;
    [SerializeField] private Transform InitPage;
    [SerializeField] private Transform InGamePage;

    private List<Transform> Page_TRANSs;

    public static WAM_SettingController IS { get; private set; }

    private void Awake()
    {
        IS = this;

        Page_TRANSs = new List<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        init_pages();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void init_pages()
    {
        Page_TRANSs.Add(InitPage);
        Page_TRANSs.Add(InGamePage);
    }

    public void ToInit()
    {
        GeneralMethods.active_UI_page(InitPage, Page_TRANSs);
    }

    public void start_button()
    {
        WAM_GameController.IS.start_game();
        start_game();
    }

    public void quit_button()
    {
        WAM_GameController.IS.quit_game();
    }

    public void restart_button()
    {
        WAM_GameController.IS.restart();
    }

    public void ToStartGame()
    {
        GeneralMethods.active_UI_page(InGamePage, Page_TRANSs);
        SCAnimator.SetTrigger(WAMSD.AniNextStep_trigger);
    }

    private void start_game()
    {
        SCAnimator.SetTrigger(WAMSD.AniStart_trigger);
    }

}
