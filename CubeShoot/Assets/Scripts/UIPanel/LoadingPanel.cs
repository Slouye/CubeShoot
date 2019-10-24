/****************************************************
    文件：LoadingWnd.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/7/30 17:20:20
	功能：加载进度界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : BasePanel
{
    public Text txtTips;
    public Image imgFG;
    public Image imgPoint;
    public Text txtPrg;


    public override void OnEnter()
    {
        InitPanel();
        SetActive(gameObject);
    }
    public override void OnExit()
    {
        SetActive(gameObject, false);
        ClearPanel();
    }
    public override void OnPause()
    {
        SetActive(gameObject, false);
    }
    public override void OnResume()
    {
        ResetUI();
        SetActive(gameObject);
    }

    private void ResetUI()
    {
        SetText(txtTips, "游戏正在加载中。。");
        SetText(txtPrg, "0%");
        imgFG.fillAmount = 0;
    }

    protected override void InitPanel()
    {
        base.InitPanel();
        ResetUI();
    }

    public void SetProgress(float prg)
    {
        SetText(txtPrg, (int)(prg * 100) + "%");
        imgFG.fillAmount = prg;
    }
}