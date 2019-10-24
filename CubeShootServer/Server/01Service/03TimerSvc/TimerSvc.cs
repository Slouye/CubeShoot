/****************************************************
	文件：TimerSvc.cs
	作者：Solis
	邮箱: zhaotianshinai@gmail.com
	日期：2019/09/25 12:14   	
	功能：
*****************************************************/
using PENet;
using System;
using System.Collections.Generic;

public class TimerSvc
{
    //任务数据包
    class TaskPack
    {
        public int tid;
        public Action<int> cb;
        public TaskPack(int tid, Action<int> cb)
        {
            this.tid = tid;
            this.cb = cb;
        }
    }

    private static TimerSvc instance;
    public static TimerSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TimerSvc();
            }
            return instance;
        }
    }

    private static readonly string obj = "lock";
    Queue<TaskPack> tpQue = new Queue<TaskPack>();
    private PETimer pt;

    public void Init()
    {
        tpQue.Clear();
        //实例化计时类
        //pt = new PETimer();
        //使用工具类中的线程来处理计时，计时完成后再返回主线程进行逻辑的处理。
        //独立线程计时，不在主线程里进行计时。
        pt = new PETimer(100);
       
        //日志工具接口
        pt.SetLog((string info) =>
        {
            PECommon.Log(info);
        });
        //设置回调处理器（满足定时条件后，逻辑回到主线程来进行处理。）
        pt.SetHandle((Action<int> cb, int tid) =>
        {
            if (cb != null)
            {
                lock (obj)
                {
                    tpQue.Enqueue(new TaskPack(tid, cb));
                }
            }
        });
        PETool.LogMsg("TimerSvc Init Down");
    }

    public void Update()
    {
        while (tpQue.Count > 0)
        {
            TaskPack tp = null;
            //处理条件满足的定时任务的逻辑。
            lock (obj)
            {
                tp = tpQue.Dequeue();
            }
            if (tp != null)
            {
                tp.cb(tp.tid);
            }
        }
    }

    public int AddTimeTask(Action<int> callback, double delay, PETimeUnit timeUnit = PETimeUnit.Millisecond, int count = 1)
    {
        return pt.AddTimeTask(callback, delay, timeUnit, count);
    }


    public long GetCurrentTime()
    {
        return (long)pt.GetMillisecondsTime();
    }

}

