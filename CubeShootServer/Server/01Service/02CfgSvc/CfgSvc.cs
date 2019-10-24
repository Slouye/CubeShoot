/****************************************************
	文件：CfgSvc.cs
	作者：Solis
	邮箱: zhaotianshinai@gmail.com
	日期：2019/09/25 12:14   	
	功能：
*****************************************************/
using PENet;
using System;
using System.Collections.Generic;
using System.Xml;

public class CfgSvc
{
    private static CfgSvc instance;
    public static CfgSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CfgSvc();
            }
            return instance;
        }
    }

    public void Init()
    {
        InitWeapenCfg();
        PETool.LogMsg("CfgSvc Init Down");
    }


    public class BaseCfg
    {
        public int id;
    }

    #region 初始化枪械XML配置文件
    private Dictionary<int, WeapenCfg> weapenDicCfg = new Dictionary<int, WeapenCfg>();
    private void InitWeapenCfg()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(PECommon.weapenCfgPath);

        XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

        for (int i = 0; i < nodLst.Count; i++)
        {
            XmlElement ele = nodLst[i] as XmlElement;

            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
            WeapenCfg weapenCfg = new WeapenCfg
            {
                id = ID,
            };
            foreach (XmlElement e in nodLst[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "weapenType":
                        weapenCfg.weapenType = e.InnerText;
                        break;
                    case "weapenDamage":
                        weapenCfg.weapenDamage = int.Parse(e.InnerText);
                        break;
                    case "weapenShootRate":
                        weapenCfg.weapenShootRate = int.Parse(e.InnerText);
                        break;
                    case "weapenFrontBullet":
                        weapenCfg.weapenFrontBullet = int.Parse(e.InnerText);
                        break;
                    case "weapenBackBullet":
                        weapenCfg.weapenBackBullet = int.Parse(e.InnerText);
                        break;
                    case "weapenReload":
                        weapenCfg.weapenReload = int.Parse(e.InnerText);
                        break;
                    case "weapenRocoil":
                        weapenCfg.weapenRocoil = int.Parse(e.InnerText);
                        break;
                    case "weapenRocoilTime":
                        weapenCfg.weapenRocoilTime = int.Parse(e.InnerText);
                        break;
                    case "BulletPrefab":
                        weapenCfg.BulletPrefab = e.InnerText;
                        break;
                    case "FireEffect":
                        weapenCfg.FireEffect = e.InnerText;
                        break;
                    case "ShellEffect":
                        weapenCfg.ShellEffect = e.InnerText;
                        break;
                }
            }
            weapenDicCfg.Add(weapenCfg.id, weapenCfg);
        }
    }
    /// <summary>
    /// 根据武器ID获得一个武器的资料  
    /// </summary>
    public WeapenCfg GetWeapenCfgData(int weapenID)
    {
        WeapenCfg weapenCfg = null;
        if (weapenDicCfg.TryGetValue(weapenID, out weapenCfg))
        {
            return weapenCfg;
        }
        else
        {
            return null;
        }
    }
    #endregion

}



