using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;


public class SetPanel : BasePanel {


    public Text txtBgAudio;
    public Text txtClickAudio;
    public Text txtCanvasSensitivity;

    public Slider bgAudioSlider;
    public Slider clickAudioSlider;
    public Slider canvasSensitivitySlider;
    public Toggle autoAimToggle;
    public Dropdown rateDropdown;


    protected override void InitPanel()
    {
        base.InitPanel();

        autoAimToggle.isOn = GameRoot.Instance.isAutoAim;
        bgAudioSlider.value = GameRoot.Instance.bgAudioVoleum;
        ChangeBGAudio(bgAudioSlider.value);
        clickAudioSlider.value = GameRoot.Instance.uiAudioVoleum;
        ChangeUIAudio(clickAudioSlider.value);
        canvasSensitivitySlider.value = GameRoot.Instance.sensitivity / 100.0f;
        ChangeSensitivity(canvasSensitivitySlider.value);

        switch (GameRoot.Instance.frameRate)
        {
            //60帧
            case 60:
                rateDropdown.value = 0;
                break;
            //90帧
            case 90:
                rateDropdown.value = 1;
                break;
            //120帧
            case 120:
                rateDropdown.value = 2;
                break;
        }
       
        if (isFirst)
        {
            RegisterEvent();
            isFirst = false;
        }

    }


    public void RegisterEvent()
    {
        audioSvc.PlayAudioInUI(audioSvc.uiAudio);
        autoAimToggle.onValueChanged.AddListener((bool isAutoAim) =>
        {
            int temp = 0;
            if (isAutoAim)
            {
                temp = 1;
                GameRoot.Instance.isAutoAim = true;
            }
            else
            {
                GameRoot.Instance.isAutoAim = false;
            }
            PlayerPrefs.SetInt("AutoAim", temp);
        });
        bgAudioSlider.onValueChanged.AddListener((Single value) =>
        {
          
            ChangeBGAudio(value);
            GameRoot.Instance.bgAudioVoleum = value;
            PlayerPrefs.SetFloat("BGAudio", value);

        });
        clickAudioSlider.onValueChanged.AddListener((Single value) =>
        {
            ChangeUIAudio(value);
            GameRoot.Instance.uiAudioVoleum = value;
            PlayerPrefs.SetFloat("UIAudio", value);
        });
        canvasSensitivitySlider.onValueChanged.AddListener((Single value) =>
        {
            ChangeSensitivity(value);
            GameRoot.Instance.sensitivity = (int)(value * 100);
            PlayerPrefs.SetInt("Sensitivity", (int)(value * 100));
        });

        rateDropdown.onValueChanged.AddListener((int value) =>
        {
            audioSvc.PlayAudioInUI(audioSvc.uiAudio);
            switch (value)
            {
                //60帧
                case 0:
                    PlayerPrefs.SetInt("FrameRate", 60);
                    PECommon.Log("修改帧率为：60");
                    break;
                //90帧
                case 1:
                    PlayerPrefs.SetInt("FrameRate", 90);
                    PECommon.Log("修改帧率为：90");
                    break;
                //120帧
                case 2:
                    PlayerPrefs.SetInt("FrameRate", 120);
                    PECommon.Log("修改帧率为：120");
                    break;
            }
            GameRoot.Instance.frameRate = value;
        });

    }


    public void ChangeBGAudio(float value)
    {
        txtBgAudio.text = ((int)(value * 100)).ToString() + "%";
        audioSvc.SetBGAidioVolume(value);
    }
    public void ChangeUIAudio(float value)
    {
        txtClickAudio.text = ((int)(value * 100)).ToString() + "%";
        audioSvc.SetUIAidioVolume(value);

    }
    public void ChangeSensitivity(float value)
    {
        txtCanvasSensitivity.text = ((int)(value * 100)).ToString() + "%";
    }

    public void CloseSetingPanel()
    {
        UIManager.Instance.PopPanel();
    }

}
