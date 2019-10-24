/****************************************************
    文件：TimerSvc.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/8/15 2:4:32
	功能：Nothing
*****************************************************/

using System;
using UnityEngine;

public class TimerSvc : SystemRoot
{
    public static TimerSvc Instance = null;
    private PETimer pt;
    public void InitSvc()
    {
        Instance = this;
        //实例化计时类
        pt = new PETimer();
        //日志工具接口
        pt.SetLog((string info) =>
        {
            PECommon.Log(info);
        });

        PECommon.Log("Init TimerSvc...");

    }

    public void Update()
    {
        pt.Update();
    }

    public int AddTimeTask(Action<int> callback, double delay, PETimeUnit timeUnit = PETimeUnit.Millisecond, int count = 1)
    {
        return pt.AddTimeTask(callback, delay, timeUnit, count);
    }

    public double GetNowTime()
    {
        return pt.GetMillisecondsTime();
    }

    public void RemoveTake(int tid)
    {
        pt.DeleteTimeTask(tid);
    }

}