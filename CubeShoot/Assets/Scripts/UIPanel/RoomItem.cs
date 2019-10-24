/****************************************************
    文件：RoomItem.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/8/28 20:14:45
	功能：房间列表单个实体
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour 
{
    public Text txtName;
    public Text txtVictoryNum;
    public Button btnJoin;



    public void SetRoomData(string name, int vn)
    {
        txtName.text = name;
        txtVictoryNum.text = "胜场:" + vn;
    }

    /// <summary>
    /// 关闭当前房间（UI）
    /// </summary>
    public void DestorySelf()
    {
        Destroy(gameObject);
    }

}