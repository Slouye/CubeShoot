/****************************************************
    文件：ResSvc.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/7/30 17:20:20
	功能：资源加载服务
*****************************************************/

using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResSvc : MonoBehaviour
{
    public static ResSvc Instance = null;

    public void InitSvc()
    {
        Instance = this;

        InitRDNameCfg();
        InitWeapenCfg();
        PECommon.Log("Init ResSvc...");

    }


    private Action prgCB = null;
    public void AsyncLoadScene(string sceneName, Action loaded)
    {
        GameRoot.Instance.LoadingPanel =(LoadingPanel)UIManager.Instance.PushPanel(UIPanelType.Loading);

        AsyncOperation sceneAsync = SceneManager.LoadSceneAsync(sceneName);
        prgCB = () => {
            float val = sceneAsync.progress;
            GameRoot.Instance.LoadingPanel.SetProgress(val);
            if (val == 1)
            {
                if (loaded != null)
                {
                    loaded();
                }
                prgCB = null;
                sceneAsync = null;
            }
        };
    }

    private void Update()
    {
        if (prgCB != null)
        {
            prgCB();
        }
    }
    /// <summary>
    /// 声音加载
    /// </summary>
    private Dictionary<string, AudioClip> adDic = new Dictionary<string, AudioClip>();
    public AudioClip LoadAudio(string path, bool cache = false)
    {
        AudioClip au = null;
        if (!adDic.TryGetValue(path, out au))
        {
            au = Resources.Load<AudioClip>(path);
            if (cache)
            {
                adDic.Add(path, au);
            }
        }
        return au;
    }
    /// <summary>
    /// 预制体加载
    /// </summary>
    private Dictionary<string, GameObject> goDic = new Dictionary<string, GameObject>();
    public GameObject LoadPrefab(string path, bool cache = false)
    {
        GameObject prefab = null;
        if (!goDic.TryGetValue(path, out prefab))
        {
            prefab = Resources.Load<GameObject>(path);
            if (cache)
            {
                goDic.Add(path, prefab);
            }
        }
        GameObject go = null;
        if (prefab != null)
        {
            go = Instantiate(prefab);
        }
        return go;
    }

    /// <summary>
    /// 雪碧图加载
    /// </summary>
    private Dictionary<string, Sprite> spriteDic = new Dictionary<string, Sprite>();
    public Sprite LoadSprite(string path, bool cache = false)
    {
        Sprite sprite = null;
        if (!spriteDic.TryGetValue(path, out sprite))
        {
            sprite = Resources.Load<Sprite>(path);
            if (cache)
            {
                spriteDic.Add(path, sprite);
            }
        }
        return sprite;
    }


    #region InitCfgs

    #region 初始化名字配置文件
    private List<string> surnameLst = new List<string>();
    private List<string> manLst = new List<string>();
    private List<string> womanLst = new List<string>();
    private void InitRDNameCfg()
    {
        //因为在家网络不好 ...
        //AssetBundle ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + PathDefine.ABRDNameCfg);
        //TextAsset xml = ab.LoadAsset<TextAsset>(PathDefine.ABRDName);
        TextAsset xml = Resources.Load<TextAsset>(PathDefine.RDNameCfg);
        if (!xml)
        {
            Debug.LogError("xml file:" + PathDefine.RDNameCfg + " not exist");
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++)
            {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                //int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                foreach (XmlElement e in nodLst[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "surname":
                            surnameLst.Add(e.InnerText);
                            break;
                        case "man":
                            manLst.Add(e.InnerText);
                            break;
                        case "woman":
                            womanLst.Add(e.InnerText);
                            break;
                    }
                }

            }

        }

    }

    /// <summary>
    /// 获得一个包括前后的随机数然后得到随机名字  man 是否是男的= = 
    /// </summary>
    public string GetRDNameData(bool man = true)
    {
        //System.Random rd = new System.Random();
        string rdName = surnameLst[PETools.RDInt(0, surnameLst.Count - 1)];
        if (man)
        {
            rdName += manLst[PETools.RDInt(0, manLst.Count - 1)];
        }
        else
        {
            rdName += womanLst[PETools.RDInt(0, womanLst.Count - 1)];
        }

        return rdName;
    }

    #endregion


    #region 初始化枪械XML配置文件
    private Dictionary<int, WeapenCfg> weapenDicCfg = new Dictionary<int, WeapenCfg>();
    private void InitWeapenCfg()
    {
        TextAsset xml = Resources.Load<TextAsset>(PathDefine.WeapenCfg);
        if (!xml)
        {
            Debug.LogError("xml file:" + PathDefine.WeapenCfg + " not exist");
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

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


    #endregion
}