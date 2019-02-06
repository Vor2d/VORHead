using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadGroup : MonoBehaviour
{
    [Header("Outer-prefab")]
    [SerializeField] MD_GameController MDGC_script;
    [Header("Inner-prefab")]
    [SerializeField] Transform Indicator_TRANS;
    [Header("Variables")]
    [SerializeField] private float ActionTime;
    [SerializeField] private Color ActionColor;

    private bool activated;
    private Color indicator_initcolor;
    private bool reload_acted;

    private void Start()
    {
        this.activated = false;
        this.indicator_initcolor =
                    Indicator_TRANS.GetComponent<MeshRenderer>().material.color;
        this.reload_acted = false;
    }

    // Update is called once per frame
    void Update()
    {
        check_gazing_off();
    }

    private void check_gazing_off()
    {
        if (activated && !MDGC_script.Reload_gazing_flag)
        {
            activated = false;
            toggel_reload_group();
        }
    }

    public void activate()
    {
        activated = true;
        toggel_reload_group();
    }

    private void toggel_reload_group()
    {
        if(activated)
        {
            turn_on_indicator();
        }
        else
        {
            turn_off_indicator();
        }
    }

    private void turn_on_indicator()
    {
        Indicator_TRANS.GetComponent<MeshRenderer>().material.color = indicator_initcolor;
        Indicator_TRANS.GetComponent<MeshRenderer>().enabled = true;
    }

    private void turn_off_indicator()
    {
        Indicator_TRANS.GetComponent<MeshRenderer>().enabled = false;
    }

    public void reload_action()
    {
        if(!reload_acted)
        {
            StartCoroutine(reload_act());
        }
    }

    private IEnumerator reload_act()
    {
        reload_acted = true;
        Indicator_TRANS.GetComponent<MeshRenderer>().material.color = ActionColor;
        yield return new WaitForSeconds(ActionTime);
        Indicator_TRANS.GetComponent<MeshRenderer>().material.color = indicator_initcolor;
        reload_acted = false;
    }
}
