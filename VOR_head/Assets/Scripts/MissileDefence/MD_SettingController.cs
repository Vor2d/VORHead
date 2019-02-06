using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MD_SettingController : MonoBehaviour
{
    [SerializeField] private MD_GameController MDGC_script;
    [SerializeField] private Transform ObserverPage_TRANS;
    [SerializeField] private Transform SettingPage_TRANS;

    [SerializeField] private InputField MissileISpeedIF;
    [SerializeField] private Toggle OneMissileTToggle;
    [SerializeField] private InputField MissileITimeIF;
    [SerializeField] private InputField ExplodeRadiusIF;
    [SerializeField] private Toggle RandomSeedToggle;
    [SerializeField] private InputField RandomSeedIF;
    [SerializeField] private Toggle InfiniteWavesToggle;
    [SerializeField] private InputField MSpeedIcreaseIF;
    [SerializeField] private InputField MRateIncreaseIF;
    [SerializeField] private InputField MNumberEachWaveIF;
    [SerializeField] private Toggle UseAutoAmmoNToggle;
    [SerializeField] private InputField AmmoOffsetIF;
    [SerializeField] private InputField ConstantAmmoIF;
    [SerializeField] private Toggle DualHeadIndicatorToggle;
    [SerializeField] private Toggle ReloadAutoNumberToggle;
    [SerializeField] private InputField ReloadAmmoOffsetIF;
    [SerializeField] private InputField ReloadAmmoConstantIF;

    private MD_DataController MDDC_script;

    private void Awake()
    {
        if (MDDC_script == null)
        {
            MDDC_script = GameObject.Find(MD_StrDefiner.DataController_name).
                                            GetComponent<MD_DataController>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        to_observe_page();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void update_data()
    {
        MissileISpeedIF.text = MDDC_script.MissileSpeed.ToString();
        OneMissileTToggle.isOn = MDDC_script.OneMissilePreTime;
        MissileITimeIF.text = MDDC_script.MissileInterTime.ToString();
        ExplodeRadiusIF.text = MDDC_script.ExplodeRaduis.ToString();
        RandomSeedToggle.isOn = MDDC_script.UsingRandomSeed;
        RandomSeedIF.text = MDDC_script.RandomSeed.ToString();
        InfiniteWavesToggle.isOn = MDDC_script.InfiniteWaves;
        MSpeedIcreaseIF.text = MDDC_script.MSDifficultyIncrease.ToString();
        MRateIncreaseIF.text = MDDC_script.MRDifficultyIncrease.ToString();
        MNumberEachWaveIF.text = MDDC_script.MissileNumberEachWave.ToString();
        UseAutoAmmoNToggle.isOn = MDDC_script.UsingAutoAmmoNumber;
        AmmoOffsetIF.text = MDDC_script.AmmoOffSet.ToString();
        ConstantAmmoIF.text = MDDC_script.AmmoConstant.ToString();
        DualHeadIndicatorToggle.isOn = MDDC_script.UsingDualHeadIndicator;
        ReloadAutoNumberToggle.isOn = MDDC_script.UsingReloadAutoNumber;
        ReloadAmmoOffsetIF.text = MDDC_script.ReloadAmmoOffset.ToString();
        ReloadAmmoConstantIF.text = MDDC_script.ReloadAmmoNumber.ToString();
    }

    public void apply_change()
    {
        try
        {
            MDDC_script.MissileSpeed = float.Parse(MissileISpeedIF.text);
            MDDC_script.OneMissilePreTime = OneMissileTToggle.isOn;
            MDDC_script.MissileInterTime = float.Parse(MissileITimeIF.text);
            MDDC_script.ExplodeRaduis = float.Parse(ExplodeRadiusIF.text);
            MDDC_script.UsingRandomSeed = RandomSeedToggle.isOn;
            MDDC_script.RandomSeed = int.Parse(RandomSeedIF.text);
            MDDC_script.InfiniteWaves = InfiniteWavesToggle.isOn;
            MDDC_script.MSDifficultyIncrease = float.Parse(MSpeedIcreaseIF.text);
            MDDC_script.MRDifficultyIncrease = float.Parse(MRateIncreaseIF.text);
            MDDC_script.MissileNumberEachWave = int.Parse(MNumberEachWaveIF.text);
            MDDC_script.UsingAutoAmmoNumber = UseAutoAmmoNToggle.isOn;
            MDDC_script.AmmoOffSet = int.Parse(AmmoOffsetIF.text);
            MDDC_script.AmmoConstant = int.Parse(ConstantAmmoIF.text);
            MDDC_script.UsingDualHeadIndicator = DualHeadIndicatorToggle.isOn;
            MDDC_script.UsingReloadAutoNumber = ReloadAutoNumberToggle.isOn;
            MDDC_script.ReloadAmmoOffset = int.Parse(ReloadAmmoOffsetIF.text);
            MDDC_script.ReloadAmmoNumber = int.Parse(ReloadAmmoConstantIF.text);

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void SettingButton()
    {
        ObserverPage_TRANS.gameObject.SetActive(false);
        SettingPage_TRANS.gameObject.SetActive(true);
        update_data();
    }

    private void to_observe_page()
    {
        ObserverPage_TRANS.gameObject.SetActive(true);
        SettingPage_TRANS.gameObject.SetActive(false);
    }

    public void BackButton()
    {
        to_observe_page();
    }

    public void ApplyButton()
    {
        apply_change();
    }

    public void RecenterButton()
    {
        MDGC_script.recenter();
    }

    public void RestartButton()
    {
        MDGC_script.restart();
    }
}
