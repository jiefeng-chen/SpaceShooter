using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Anim
{
    public AnimationClip idle;
    public AnimationClip runForward;
    public AnimationClip runBackward;
    public AnimationClip runRight;
    public AnimationClip runLeft;
}

public class PlayerCtrl : MonoBehaviour
{

    private float h = 0.0f;
    private float v = 0.0f;

    // 必须先分配变量，之后才能使用需要访问的组件
    private Transform tr;

    // 移动速度变量
    public float moveSpeed = 10.0f;

    // 旋转速度变量
    public float rotSpeed = 100.0f;

    // 要显示到检视视图的动画类变量
    public Anim anim;

    // 要访问下列3D模型Animation组件对象的变量
    public Animation _animation;

    // 表示玩家生命值的变量
    public int hp = 100;

    // player的生命初始值
    private int initHp;
    // player的生命条图像
    public Image imgHpbar;
    // 访问游戏管理器的变量
    private GameMgr gameMgr;

    // 声明委派和事件
    public delegate void PlayerDieHandler();

    public static event PlayerDieHandler OnPlayerDie;


    // Use this for initialization
    void Start () {
        // 设置生命初始值
        initHp = hp;
		// 向脚本初始部分分配Transform组件
	    tr = GetComponent<Transform>();

        // 获取GameMgr脚本
        gameMgr = GameObject.Find("GameManager").GetComponent<GameMgr>();

        // 查找谓语自身下级的Animation组件并分配到变量
	    _animation = GetComponentInChildren<Animation>();

        // 保存并运行Animation组件的动画片段
	    _animation.clip = anim.idle;
	    _animation.Play();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    h = Input.GetAxis("Horizontal");
	    v = Input.GetAxis("Vertical");


        // 计算前后左右移动方向向量
	    Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        // Translate（移动方向*速度*位移值*Time.deltaTime, 基础坐标）
	    tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);
	   
        // 以Vector3.up轴为准基，以rotSpeed速度旋转
        tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));

	    if (v >= 0.1f)
	    {
	        // 前进动画
            _animation.CrossFade(anim.runForward.name, 0.3f);
	    }else if (v <= -0.1f)
	    {
	        _animation.CrossFade(anim.runBackward.name, 0.3f);
	    }else if (h >= 0.1f)
	    {
	        _animation.CrossFade(anim.runRight.name, 0.3f);
	    }else if (h <= -0.1f)
	    {
	        _animation.CrossFade(anim.runLeft.name, 0.3f);
        }
	    else
	    {
	        _animation.CrossFade(anim.idle.name, 0.3f);
	    }
	}

    private void OnTriggerEnter(Collider other)
    {
        // 如果发生碰撞的Collider为怪兽的Punch，则减少玩家的hp
        if (other.gameObject.tag == "Punch")
        {
            hp -= 10;
            // 调整Image UI元素的fillAmount属性，以调整生命条长度
            imgHpbar.fillAmount = (float)hp / (float)initHp;
            Debug.Log("Player HP = " + hp.ToString());

            // 玩家生命值小于0时进行死亡处理
            if (hp <= 0)
            {
                PlayerDie();
            }
        }
    }

    void PlayerDie()
    {
        Debug.Log("Player Die!!");

        //// 获取所有拥有Monster tag的游戏对象
        //GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");

        //// 依次调用所有怪兽的OnPlayerDie函数
        //foreach (GameObject monster in monsters)
        //{
        //    monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        //}
        
        // 触发事件
        OnPlayerDie();

        // 更新游戏管理器的isGameOver变量值以停止生成怪兽
        //gameMgr.isGameOver = true;

        // 访问GameMgr的单例并更改其isGameOver变量
        GameMgr.instance.isGameOver = true;
    }
}
