/****************************************************
    文件：ItemHP.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/8/21 3:13:48
	功能：单个怪物的血条ID
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class ItemHP : MonoBehaviour
{
    private int hp;

    public int currentHP;

    public Image imgGray;
    public Image imgRed;

    public Text txtHP;
    public Text txtName;

    public Animation HPAni;

    private Transform hpRoot;
    private RectTransform rect;

    //当前血条的值和目标血条的值。
    private float targetBlend;
    private float currentBlend;

    public bool is3DHP;

    void Update()
    {
        if (is3DHP)
        {
            //UI自适应。。。
            float scaleRate = 1.0f * Constants.ScreenStandardHeight / Screen.height;
            rect.anchoredPosition = Camera.main.WorldToScreenPoint(hpRoot.position) * scaleRate;
        }
     
        if (targetBlend != currentBlend)
        {
            SmoothMoveAni();
        }
        imgGray.fillAmount = currentBlend;
    }

    public void Init3DHP(Transform hpRoot, int hp)
    {
        rect = gameObject.GetComponent<RectTransform>();
        this.hpRoot = hpRoot;
        imgGray.fillAmount = 1;
        imgRed.fillAmount = 1;
        this.hp = hp;
        currentHP = hp;
    }

    public void Init2DHP(int hp,string name)
    {
        imgGray.fillAmount = 1;
        imgRed.fillAmount = 1;
        this.hp = hp;
        txtName.text = name;
        currentHP = hp;
    }

    public void InitSelfHP(int hp)
    {
        imgGray.fillAmount = 1;
        imgRed.fillAmount = 1;
        this.hp = hp;
        currentHP = hp;
        txtHP.text = hp.ToString();
    }

    public void SetHP(int damage)
    {
        HPAni.Stop();
        txtHP.text = "-" + damage;
        HPAni.Play();
    }

    /// <summary>
    /// 设置血条。
    /// </summary>
    public void SetBloodBar(int oldVal, int newVal)
    {
        imgRed.fillAmount = newVal * 1.0f / hp;
        targetBlend = newVal * 1.0f / hp;
        currentHP = newVal;
        //下方血条独有
        if (txtHP!=null)
        {
            txtHP.text = newVal.ToString();
        }
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


}