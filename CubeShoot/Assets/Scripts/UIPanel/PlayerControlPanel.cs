/****************************************************
    文件：PlayerControlWnd.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/8/18 17:49:28
	功能：玩家控制窗口
*****************************************************/

using PEProtocol;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerControlPanel : BasePanel
{
    #region Player
    public Image imageTouch;
    public Image imageDirBg;
    public Image imageDirPoint;

    public Image imageSwitchDir;

    public Image FireBtnImg;

    public Image ReloadBtnImg;
    public Button ReloadBtn;

    public Image JumpBtnImg;
    public Button JumpBtn;

    public Image AimBtnImg;
    public Button AimBtn;


    public Button btnExit;

    public Text txtBullet;
    public Image imgReload;
    public Animation imgReloadAni;


    public Image imgSight;      //准星UI
    public Image imgSniperUI; //狙击枪开镜UI

    public Transform HPContent;

    public ItemHP SelfItemHp;

    public bool autoAim;

    public bool isOpenGlass;

    //原点初始位置
    private Vector2 imageDirPointOriginsPos;
    //操纵杆的最大移动距离
    private float controllerMaxDis;
    //操纵杆的最大移动距离
    private float fireButtonMaxDis;

    //原点初始位置
    private Vector2 imageSwitchDirPointOriginsPos;
    private Vector2 lastDrawPointPos;
    #endregion

    public Dictionary<string, GameObject> EnemyGODic = new Dictionary<string, GameObject>();

    private Vector2 sightPos;
    private float lastDis = float.MaxValue;
    private Vector2 finallyPos = Vector2.zero;
    private RaycastHit hit;

    //当前血条的值和目标血条的值。
    private float targetBlend = 1;
    private float currentBlend = 1;

    private int currentHp;
    private PlayerData playerData;

    public Vector2 currentMove;

    public Dictionary<RoleType, ItemHP> playerItemHPDic = new Dictionary<RoleType, ItemHP>();

    private int sensitivity = 50;

    public bool isOver;

    private bool isLoadBullet;
    public bool IsLoadBullet
    {
        get { return isLoadBullet; }
        set {
            isLoadBullet = value;
            if (isLoadBullet==true)
            {
                ReloadBtn.interactable = false;

                AimBtn.interactable = false;
                BattleSys.Instance.StopFire();
            }
            else
            {
                ReloadBtn.interactable = true;
                AimBtn.interactable = true;
            }
        }
    }


    public override void OnEnter()
    {
        base.OnEnter();
        autoAim = GameRoot.Instance.isAutoAim;
        sensitivity = GameRoot.Instance.sensitivity;
        isOver = false;

    }

    public override void OnResume()
    {
        base.OnResume();
        autoAim = GameRoot.Instance.isAutoAim;
        sensitivity = GameRoot.Instance.sensitivity;
        isOver = false;
    }

    public override void OnPause()
    {
        base.OnPause();
        isOver = true;
    }

    public override void OnExit()
    {
        base.OnPause();
        isOver = true;
    }

  

    protected override void InitPanel()
    {
        base.InitPanel();
        PECommon.Log("自动瞄准为：" + autoAim);
        //算出在当前屏幕上操作杆的最大移动距离
        controllerMaxDis = Screen.height * 1.0f / Constants.ScreenStandardHeight * Constants.ControllerMaxDis;
        SetActive(imageDirPoint, false);
        if (isFirst)
        {
            RegisterTouchBtn();
            isFirst = false;
        }
        SetActive(imgReload, false);
        sightPos = new Vector2(Screen.width / 2 * 1.0f, Screen.height / 2 * 1.0f);
        currentHp = BattleSys.Instance.playerHPDic.TryGet(BattleSys.Instance.GetCurrentRoleType());
        SelfItemHp.InitSelfHP(currentHp);
        GameRoot.Instance.SetPlayerHP(currentHp);

    }


    public void SetAutoAim( bool isAutoAim)
    {
        autoAim = isAutoAim;
    }

    public void Setsensitivity(int sensitivity)
    {
        this.sensitivity = sensitivity;
    }

    /// <summary>
    /// 自动瞄准。
    /// </summary>
    private void Update()
    {
        PECommon.Log("自动瞄准" + autoAim);
        //玩家没有设置自动瞄准
        if (!autoAim)
        {
            return;
        }
        if (isOver)
        {
            return;
        }
    
        if (Physics.Raycast(Camera.main.ScreenPointToRay(sightPos), out hit))
        {
            //在人物身上了
            if (UnityTools.FindUpParent(hit.collider.transform).tag == "Player")
            {
                //Debug.Log("瞄准到角色" + UnityTools.FindUpParent(hit.collider.transform).gameObject.name);
                return;
            }
        }
        foreach (var item in EnemyGODic)
        {
            if (item.Value != null)
            {
                Vector3 enemyPosV3 = Camera.main.WorldToScreenPoint(item.Value.transform.position);
                Vector2 enemyPosV2 = new Vector2(enemyPosV3.x, enemyPosV3.y);
                float distance = Vector2.Distance(sightPos, enemyPosV2);
                //PECommon.Log("准星距离敌方：" + distance);
                //进入自动瞄准范围
                if (distance < Constants.SelfAimRange)
                {
                    if (distance <= Constants.SelfAimStopRange)
                    {
                        return;
                    }
                    //PECommon.Log("进入自动瞄准范围,distance为：" + distance+ "lastDis为：" + lastDis);
                    //找到最近的那个目标
                    if (distance < lastDis)
                    {
                        //要瞄准的目标的位置
                        finallyPos = enemyPosV2;
                        lastDis = distance;
                    }
                }
            }
        }
        if (finallyPos != Vector2.zero)
        {
            Vector2 dir = finallyPos - sightPos;
            //改变摄像机朝向
            Camera.main.transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x - (dir.y / 360), Camera.main.transform.eulerAngles.y + (dir.x / 360), Camera.main.transform.eulerAngles.z);
            //人物动作改变：
            BattleSys.Instance.battleMgr.UpdateModelAction();
            finallyPos = Vector2.zero;
            lastDis = float.MaxValue;
        }
    }


    /// <summary>
    /// 注册按钮
    /// </summary>
    public void RegisterTouchBtn()
    {
        #region 圆盘
        OnClickDown(imageTouch.gameObject, (PointerEventData eventData) =>
        {
            imageDirPointOriginsPos = eventData.position;
            imageDirBg.transform.position = eventData.position;
            //原点显示
            SetActive(imageDirPoint);
        });
        OnClickUp(imageTouch.gameObject, (PointerEventData eventData) =>
        {
            //圆盘复原
            imageDirBg.transform.localPosition = Vector2.zero;  //imageDirBgOriginsPos
            //原点隐藏
            SetActive(imageDirPoint, false);
            //再设置一次让人物停止移动
            currentMove = Vector2.zero;
            BattleSys.Instance.SetPlayerMove(currentMove);
        });
        OnDrag(imageTouch.gameObject, (PointerEventData eventData) =>
        {
            Vector2 distance = eventData.position - imageDirPointOriginsPos;
            //计算向量的长度
            float length = distance.magnitude;
            if (length > controllerMaxDis)
            {
                //限制。
                imageDirPoint.transform.position = Vector2.ClampMagnitude(distance, controllerMaxDis) + imageDirPointOriginsPos;
            }
            currentMove = distance.normalized;
            BattleSys.Instance.SetPlayerMove(currentMove);
        });
        #endregion


        #region 滑动转向
        OnClickDown(imageSwitchDir.gameObject, (PointerEventData eventData) =>
        {
            imageSwitchDirPointOriginsPos = eventData.position;
        });
        OnClickUp(imageSwitchDir.gameObject, (PointerEventData eventData) =>
        {
            BattleSys.Instance.SetPlayerDir(Vector2.zero);
        });
        OnDrag(imageSwitchDir.gameObject, (PointerEventData eventData) =>
        {
            
            Vector2 direction =  (eventData.position  - imageSwitchDirPointOriginsPos) * 0.01f;

            //左右转向回正方向 + 防止误操作（防止轻轻碰到了）。
            if (Mathf.Abs(eventData.position.magnitude - lastDrawPointPos.magnitude) < 5.0f)
            {
                lastDrawPointPos = eventData.position;
                imageSwitchDirPointOriginsPos = eventData.position;
                return;
            }
            else
            {
                lastDrawPointPos = eventData.position;
            }
            //计算向量的长度
            BattleSys.Instance.SetPlayerDir(direction * sensitivity / 50.0f);
        });
        #endregion

        #region 开火按钮
        OnClickDown(FireBtnImg.gameObject, (PointerEventData eventData) =>
        {
            BattleSys.Instance.StartFire();
            imageSwitchDirPointOriginsPos = eventData.position;
        });
        OnClickUp(FireBtnImg.gameObject, (PointerEventData eventData) =>
        {
            BattleSys.Instance.StopFire();
        });
   
        OnDrag(FireBtnImg.gameObject, (PointerEventData eventData) =>
        {
            Vector2 direction = (eventData.position - imageSwitchDirPointOriginsPos) * 0.01f;
            //左右转向回正方向 + 防止误操作（防止轻轻碰到了）。
            if (Mathf.Abs(eventData.position.magnitude - lastDrawPointPos.magnitude) < 5.0f)
            {
                lastDrawPointPos = eventData.position;
                imageSwitchDirPointOriginsPos = eventData.position;
                return;
            }
            else
            {
                lastDrawPointPos = eventData.position;
            }
            //计算向量的长度
            BattleSys.Instance.SetPlayerDir(direction * sensitivity / 50.0f);
        });
        #endregion

   
        ReloadBtn.onClick.AddListener(() =>
        {
            BattleSys.Instance.ReloadBullet();
            PECommon.Log("换子弹。");
            //退出开镜状态 禁止瞄准
        });
        AimBtn.onClick.AddListener(() =>
        {
            BattleSys.Instance.SwitchGlass();
            PECommon.Log("瞄准。");
        });
        JumpBtn.onClick.AddListener(() =>
        {
            BattleSys.Instance.CharacterJump();
            PECommon.Log("跳跃。");
        });
        btnExit.onClick.AddListener(() =>
        {
            BattleSys.Instance.ExitGame();
            PECommon.Log("暂停。");
        });

    }


    /// <summary>
    /// 平滑移动动画。
    /// </summary>
    private void SmoothMoveAni()
    {
        //如果当前混合树的数值减去目标混合树的数值 小于 加速度能够一帧就完成的动作
        if (Mathf.Abs(currentBlend - targetBlend) < Constants.AccelerateHPSpeed * Time.deltaTime)
        {
            //直接赋值
            currentBlend = targetBlend;
        }
        //如果当前混合树的数值 大于目标混合树的数值
        else if (currentBlend > targetBlend)
        {
            //每帧减少一点
            currentBlend -= Constants.AccelerateHPSpeed * Time.deltaTime;
        }
        //如果当前混合树的数值 小于目标混合树的数值
        else
        {
            //每帧加一点
            currentBlend += Constants.AccelerateHPSpeed * Time.deltaTime;
        }
    }



    public void CreateAllPlayerHPUI(Dictionary<RoleType, int> HPDic)
    {
        ClearHPItem();
        foreach (var item in HPDic)
        {
            //TODO 创建所有玩家的HP
            GameObject go = resSvc.LoadPrefab(PathDefine.HPContentPath);
            go.transform.SetParent(HPContent);
            go.transform.localScale = Vector3.one;
            ItemHP itemHP = go.GetComponent<ItemHP>();
            itemHP.Init2DHP(item.Value, item.Key.ToString());
            playerItemHPDic.Add(item.Key, itemHP);
        }
    }

    public void SetTextBullet(int weapenFrontBullet, int weapenBackBullet)
    {
        SetText(txtBullet, Constants.GetColorStr(weapenFrontBullet + "/" + weapenBackBullet,TextColor.Blue));
    }

    public void ShowReloadBulletAni(float showTime)
    {
        foreach (AnimationState state in imgReloadAni)
        {
            state.speed = 1 / showTime;
        }
        SetActive(imgReload);
    }

    public void HideReloadBulletAni()
    {
        SetActive(imgReload,false);
    }

    public void SetSniperAim(bool isShow)
    {
        SetActive(imgSniperUI, isShow);
        SetActive(imgSight, !isShow);
    }

    public void SetImgSight(string path)
    {
        imgSight.sprite = resSvc.LoadSprite(path, true);
    }

    public void DestroyOneHPBar(RoleType roleType)
    {
  
        ItemHP itemHP = playerItemHPDic.TryGet(roleType);
        if (itemHP!=null)
        {
            Destroy(itemHP.gameObject);
            playerItemHPDic.Remove(roleType);
        }
     
    }
    public void AddEnemyToDic(string name, GameObject go)
    {
        if (EnemyGODic.TryGet(name) == null)
        {
            EnemyGODic.Add(name, go);
        }
    }
    public void RemoveEnemyInDic(string name)
    {
        if (EnemyGODic.TryGet(name)!=null)
        {
            EnemyGODic.Remove(name);
        }
    }

    public void BattleEnd()
    {
        EnemyGODic.Clear();
        //销毁UI
        DestroyOneHPBar(BattleSys.Instance.GetCurrentRoleType());
        //销毁一下预制体。
        BattleSys.Instance.battleMgr.DestroyPlayerModel(BattleSys.Instance.GetCurrentRoleType());
        playerItemHPDic.Clear();
        PECommon.Log("我的游戏结束,销毁我的游戏物体");
    }

    public void ClearHPItem()
    {
        for (int i = 0; i < HPContent.childCount; i++)
        {
            Destroy(HPContent.GetChild(i).gameObject);
        }
    }
       
}