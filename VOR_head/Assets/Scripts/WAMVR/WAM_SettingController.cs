﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WAM_SettingController : MonoBehaviour
{
    [SerializeField] private WAMRC RC;
    [SerializeField] private Transform InitPage;

    private List<Transform> Page_TRANSs;


    private void Awake()
    {
        IS = this;

        Page_TRANSs = new List<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        init_pages();

        ToInit();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void init_pages()
    {
        Page_TRANSs.Add(InitPage);
    }

    private void ToInit()
    {
        GeneralMethods.active_UI_page(InitPage, Page_TRANSs);
    }

    public void start_button()
    {
        WAM_GameController.IS.start_game();
    }

    public void quit_button()
    {
        WAM_GameController.IS.quit_game();
    }

    public static WAM_SettingController IS { get; private set; }
}
