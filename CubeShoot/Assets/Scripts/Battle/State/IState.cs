/****************************************************
	文件：IState.cs
	作者：Solis
	邮箱: zhaotianshinai@gmail.com
	日期：2019/08/18 19:28   	
	功能：所有状态的需要实现的接口
*****************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public enum AniState
{
    None,
    Born,
    Idle,
    Move,
    Rot,
    Attack,
    Reload,
    Hit,
    Die,
    Win,
    Jump,
    Fall
}


public interface IState
{
    void Enter(EntityBase entityBase, params object[] para); //第二个参数是可选参数。
    void Process(EntityBase entityBase, params object[] para);
    void Exit(EntityBase entityBase, params object[] para);

}