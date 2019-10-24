/****************************************************
    文件：WindowRoot.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/7/30 17:20:20
	功能：UI界面基类
*****************************************************/

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowRoot : MonoBehaviour {
    protected ResSvc resSvc = null;
    protected AudioSvc audioSvc = null;
    protected NetSvc netSvc = null;
    protected TimerSvc timerSvc = null;

    public virtual void SetWndState(bool isActive = true) {
        if (gameObject.activeSelf != isActive) {
            SetActive(gameObject, isActive);
        }
        if (isActive) {
            InitPanel();
        }
        else {
            ClearPanel();
        }
    }

    protected virtual void InitPanel() {
        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;
        netSvc = NetSvc.Instance;
        timerSvc = TimerSvc.Instance;
    }

    protected virtual void ClearPanel() {
        resSvc = null;
        audioSvc = null;
        netSvc = null;
        timerSvc = null;
    }

    #region Tool Functions

    protected void SetActive(GameObject go, bool isActive = true) {
        go.SetActive(isActive);
    }
    protected void SetActive(Transform trans, bool state = true) {
        trans.gameObject.SetActive(state);
    }
    protected void SetActive(RectTransform rectTrans, bool state = true) {
        rectTrans.gameObject.SetActive(state);
    }
    protected void SetActive(Image img, bool state = true) {
        img.transform.gameObject.SetActive(state);
    }
    protected void SetActive(Text txt, bool state = true) {
        txt.transform.gameObject.SetActive(state);
    }
    protected void SetActive(Button btn, bool state = true)
    {
        btn.transform.gameObject.SetActive(state);
    }

    protected void SetText(Text txt, string context = "") {
        txt.text = context;
    }
    protected void SetText(Transform trans, int num = 0) {
        SetText(trans.GetComponent<Text>(), num);
    }
    protected void SetText(Transform trans, string context = "") {
        SetText(trans.GetComponent<Text>(), context);
    }
    protected void SetText(Text txt, int num = 0) {
        SetText(txt, num.ToString());
    }


    /// <summary>
    /// 需要加一个条件WhereT 继承自组件
    /// 泛型查找这个物体是否有这个组件。
    /// </summary>
    protected T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T t = go.GetComponent<T>();
        if (t==null)
        {
            t = go.AddComponent<T>();
        }
        return t;
    }


    /// <summary>
    /// 设置图片到指定的Image组件
    /// </summary>
    /// <param name="image"></param>
    /// <param name="spritePath"></param>
    protected void SetSprite(Image image, string spritePath)
    {
        image.sprite =  resSvc.LoadSprite(spritePath);
    }

    #endregion




    #region 事件注册
    protected void OnClick(GameObject go, Action<object> eventData,object args)
    {
        PEListener peListener = GetOrAddComponent<PEListener>(go);
        peListener.onClick = eventData;
        peListener.args = args;
    }
    protected void OnClickDown(GameObject go, Action<PointerEventData> eventData)
    {
        PEListener peListener = GetOrAddComponent<PEListener>(go);
            peListener.onClickDown = eventData;
    }
    protected void OnClickUp(GameObject go, Action<PointerEventData> eventData)
    {
        PEListener peListener = GetOrAddComponent<PEListener>(go);
            peListener.onClickUp = eventData;       
    }
    protected void OnDrag(GameObject go, Action<PointerEventData> eventData)
    {
        PEListener peListener = GetOrAddComponent<PEListener>(go);
            peListener.onDrag = eventData;
    }
    #endregion
}