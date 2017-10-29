using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MonsterCtrl : MonoBehaviour {

    // 声明表示怪兽状态信息的Enumerable变量
    public enum MonsterState
    {
        idle,
        trace,
        attack,
        die
    };
    // 保存怪兽当前状态的Enum变量
    public MonsterState monsterState = MonsterState.idle;

    // 为提高速度而向变量分配各种组件
    private Transform monsterTr;

    private Transform playerTr;
    private NavMeshAgent nvAgent;
    private Animator animator;

    // 追击范围
    public float traceDist = 50.0f;
    // 攻击范围
    public float attackDist = 1.5f;
    // 怪兽是否死亡
    private bool isDie = false;

    // 血迹效果预设
    public GameObject bloodEffect;
    // 血迹贴图效果预设
    public GameObject bloodDecal;

    // 怪兽生命值变量
    private int hp = 100;

    // 获取GameUI游戏对象的GameUI脚本
    private GameUI gameUI;

	// Use this for initialization
	void Awake () {
		// 获取怪兽的Transform组件
	    monsterTr = this.gameObject.GetComponent<Transform>();
        // 获取怪兽要追击的对象,玩家的Transform
	    playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        // 获取NavMeshAgent
	    nvAgent = this.gameObject.GetComponent<NavMeshAgent>();

        // 设置要追击对象的位置后，怪兽马上开始追击
	    //nvAgent.destination = playerTr.position;
        // 获取Animator组件
	    animator = this.gameObject.GetComponent<Animator>();

	    gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();

   
	}

    // 脚本开始运行时注册事件
    void OnEnable()
    {
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;

        // 运行定期检查怪兽当前状态的协程函数
        StartCoroutine(this.CheckMonsterState());

        // 运行根据怪兽当前状态执行相应例程的协程函数
        StartCoroutine(this.MonsterAction());
    }

    // 脚本结束运行时解除事件
    void OnDisable()
    {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }


    IEnumerator CheckMonsterState()
    {
        while (!isDie)
        {
            // 等待0.2秒后在执行后续代码
            yield return new WaitForSeconds(0.2f);

            // 测量怪兽与玩家之间的距离
            float dist = Vector3.Distance(playerTr.position, monsterTr.position);

            if (dist <= attackDist)
            {
                // 查看玩家是否进入攻击范围
                monsterState = MonsterState.attack;
            }else if (dist <= traceDist)
            {
                monsterState = MonsterState.trace;
            }
            else
            {
                monsterState = MonsterState.idle;
            }
        }
    }

    // 根据怪兽当前状态执行适当的动作
    IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (monsterState)
            {
                    // idle 状态
                    case MonsterState.idle:
                    // 停止追击
                    nvAgent.isStopped = true;
                    // 将Animator的IsTrace变量设置为false
                    animator.SetBool("IsTrace", false);
                        break;
                    case MonsterState.trace:
                    // 传递要追击对象的位置
                        nvAgent.destination = playerTr.position;
                        nvAgent.isStopped = false;
                    // 将Animator的IsAttack变量设置为false
                    animator.SetBool("IsAttack", false);
                    // 将Animator的IsTrace变量设置为true
                        animator.SetBool("IsTrace", true);
             
                        break;
                    // 攻击状态
                    case MonsterState.attack:
                    // 停止追击
                        nvAgent.isStopped = true;
                    // 将IsAttack设置为true后，转换为attack状态
                    animator.SetBool("IsAttack", true);
                        break;

            }
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            // 调用血迹效果函数
            CreateBloodEffect(collision.transform.position);

            // 获取子弹的伤害力并减少怪兽hp
            hp -= collision.gameObject.GetComponent<BulletCtrl>().damage;
            if (hp <= 0)
            {
                MonsterDie();
            }

            // 删除子弹对象bullet
            Destroy(collision.gameObject);
            // 触发IsHit Trigger，使怪兽从Any State转换为gothit状态
            animator.SetTrigger("IsHit");
        }
    }

    // 怪兽死亡时处理例程
    void MonsterDie()
    {
        // 将死亡的怪兽Tag修改为Untagged
        gameObject.tag = "Untagged";

        // 停止所有协程
        StopAllCoroutines();

        isDie = true;
        monsterState = MonsterState.die;
        nvAgent.isStopped = true;
        animator.SetTrigger("IsDie");

        // 禁用怪兽的Colider
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;

        foreach (SphereCollider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = false;
        }
        // 调用GameUI脚本的处理分数累加与显示的函数
        gameUI.DispScore(50);

        // 调用将怪兽返回对象池的协程函数
        StartCoroutine(this.PushObjectPool());
    }

    void CreateBloodEffect(Vector3 pos)
    {
        // 生成血迹效果
        GameObject blood1 = Instantiate(bloodEffect, pos, Quaternion.identity) as GameObject;
        Destroy(blood1, 2.0f);

        // 贴图生成位置计算，计算在地面以上的位置
        Vector3 decalPos = monsterTr.position + (Vector3.up * 0.05f);
        // 随机设置贴图旋转值
        Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));

        // 生成贴图预设
        GameObject blood2 = Instantiate(bloodDecal, decalPos, decalRot) as GameObject;
        // 调整贴图大小，使其每次生成的尺寸不同
        float scale = Random.Range(1.5f, 3.5f);
        blood2.transform.localScale = Vector3.one * scale;

        // 5秒后删除血迹效果预设
        Destroy(blood2, 5.0f);
    }

    void OnPlayerDie()
    {
        // 停止所有检测怪兽状态的协程函数
        StopAllCoroutines();
        // 停止追击并执行动画
        nvAgent.isStopped = true;
        animator.SetTrigger("IsPlayerDie");
    }

    IEnumerator PushObjectPool()
    {
        yield return new WaitForSeconds(3.0f);

        // 初始化各种变量
        isDie = false;
        hp = 100;
        gameObject.tag = "Monster";
        monsterState = MonsterState.idle;

        // 重新激活怪兽的Collider
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = true;

        foreach (SphereCollider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = true;
        }

        // 禁用怪兽
        gameObject.SetActive(false);
    }

    void OnDamage(object[] _params)
    {
        Debug.Log(string.Format("Hit ray {0} : {1}", _params[0], _params[1]));

        // 调用生成血迹效果的函数
        CreateBloodEffect((Vector3)_params[0]);

        // 获取子弹伤害值，减少怪兽Hp
        hp -= (int) _params[1];
        if (hp <= 0)
        {
            MonsterDie();
        }

        // 触发isHit Trigger，是怪兽从AnyState变为gothit
        animator.SetTrigger("IsHit");
    }
}

