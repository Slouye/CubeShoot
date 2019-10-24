/****************************************************
    文件：DynamicWnd.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/7/30 17:20:20
	功能：动态UI界面（提示框）
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicPanel : WindowRoot
{
    public Animation tipsAni;
    public Text txtTips;

    private bool isTipsShow = false;
    private Queue<string> tipsQue = new Queue<string>();
    private int tipsCount;
    private float aniSpeed;

    private Dictionary<string, ItemHP> monsterHPDic = new Dictionary<string, ItemHP>();

    protected override void InitPanel()
    {
        base.InitPanel();
        txtTips.fontSize =(int)(80 * (Screen.height * 1.0f / Constants.ScreenStandardHeight));
        SetActive(txtTips, false);
    }

    private void Update()
    {
        if (tipsQue.Count > 0 && isTipsShow == false)
        {
            lock (tipsQue)
            {
                string tips = tipsQue.Dequeue();
                isTipsShow = true;
                SetTips(tips);
            }
        }
    }


    #region Tips

    public void AddTips(string tips)
    {
        lock (tipsQue)
        {
            tipsQue.Enqueue(tips);
            tipsCount++;
        }
    }

    private void SetTips(string tips)
    {
        SetActive(txtTips);
        SetText(txtTips, tips);
        AnimationClip clip = tipsAni.GetClip("TipsShowAni");

        foreach (AnimationState state in tipsAni)
        {
            aniSpeed = tipsCount * 1.0f;
            state.speed = aniSpeed;
        }
        tipsAni.Play();

        //延时关闭
        StartCoroutine(AniPlayDone(clip.length, () =>
        {
            SetActive(txtTips, false);
            isTipsShow = false;
            tipsCount--;
        }));
    }

    private IEnumerator AniPlayDone(float sec, Action cb)
    {
        yield return new WaitForSeconds(sec / aniSpeed);
        if (cb != null)
        {
            cb();
        }
    }
    #endregion

    #region 战斗相关

    public void SetHP(string mName, int damage)
    {
        ItemHP itemHP = null;
        if (monsterHPDic.TryGetValue(mName, out itemHP))
        {
            itemHP.SetHP(damage);
        }
    }
    public void SetBloodBar(string mName, int oldVal, int newVal)
    {
        ItemHP itemHP = null;
        if (monsterHPDic.TryGetValue(mName, out itemHP))
        {
            itemHP.SetBloodBar(oldVal, newVal);
        }
    }
    #endregion




}