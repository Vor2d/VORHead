using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputController : MonoBehaviour
{
    public static InputController IS;

    public Action KeyQ;
    public Action KeyW;
    public Action KeyE;
    public Action KeyR;
    public Action KeyT;
    public Action KeyY;
    public Action KeyU;
    public Action KeyI;
    public Action KeyO;
    public Action KeyA;
    public Action KeyS;
    public Action KeyESC;

    private void Awake()
    {
        IS = this;

        this.KeyQ = null;
        this.KeyW = null;
        this.KeyE = null;
        this.KeyR = null;
        this.KeyT = null;
        this.KeyY = null;
        this.KeyU = null;
        this.KeyI = null;
        this.KeyO = null;
        this.KeyA = null;
        this.KeyS = null;
        this.KeyESC = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && KeyQ != null) { KeyQ(); }
        if (Input.GetKeyDown(KeyCode.W) && KeyW != null) { KeyW(); }
        if (Input.GetKeyDown(KeyCode.E) && KeyE != null) { KeyE(); }
        if (Input.GetKeyDown(KeyCode.R) && KeyR != null) { KeyR(); }
        if (Input.GetKeyDown(KeyCode.T) && KeyT != null) { KeyT(); }
        if (Input.GetKeyDown(KeyCode.Y) && KeyY != null) { KeyY(); }
        if (Input.GetKeyDown(KeyCode.U) && KeyU != null) { KeyU(); }
        if (Input.GetKeyDown(KeyCode.I) && KeyI != null) { KeyI(); }
        if (Input.GetKeyDown(KeyCode.O) && KeyO != null) { KeyO(); }
        if (Input.GetKeyDown(KeyCode.A) && KeyA != null) { KeyA(); }
        if (Input.GetKeyDown(KeyCode.S) && KeyO != null) { KeyS(); }
        if (Input.GetKeyDown(KeyCode.Escape) && KeyESC != null) { KeyESC(); }
    }
}
