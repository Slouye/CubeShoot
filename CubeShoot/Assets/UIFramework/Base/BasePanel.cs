using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class BasePanel : WindowRoot {

    protected bool isFirst = true;

    /// <summary>
    /// 界面被显示出来
    /// </summary>
    public virtual void OnEnter()
    {
        InitPanel();
        transform.localPosition = new Vector3(1300, 0, 0);
        transform.localScale = Vector3.zero;
        RightEnter();
    }

    /// <summary>
    /// 界面暂停
    /// </summary>
    public virtual void OnPause()
    {
        LeftLeave();
    }

    /// <summary>
    /// 界面继续
    /// </summary>
    public virtual void OnResume()
    {
        RightEnter();
    }

    /// <summary>
    /// 界面不显示,退出这个界面，界面被关系
    /// </summary>
    public virtual void OnExit()
    {
        LeftLeave();
        ClearPanel();
    }


    /// <summary>
    /// 面板从右边进
    /// </summary>
    protected void RightEnter()
    {
        gameObject.SetActive(true);
        transform.DOScale(Vector3.one, 0.2f);
        transform.DOLocalMove(Vector3.zero, 0.3f);
    }
    /// <summary>
    /// 面板从左边离开
    /// </summary>
    protected void LeftLeave()
    {
        transform.DOScale(Vector3.zero, 0.2f);
        transform.DOLocalMove(new Vector3(-1300, 0, 0), 0.3f).OnComplete(() => gameObject.SetActive(false));
    }

}
