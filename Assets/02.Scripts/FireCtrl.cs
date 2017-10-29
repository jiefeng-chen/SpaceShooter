using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 声明脚本需要的组件，以防止该组件被删除
[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour
{
    // 子弹预设
    public GameObject bullet;
    // 子弹发射坐标
    public Transform firePos;

    // 子弹发射声音
    public AudioClip fireSfx;
    //保存AudioSource组件的变量
    private AudioSource source = null;
    // 连接MuzzleFlash的MeshRenderer组件
    public MeshRenderer muzzleFlash;


    void Start()
    {
        // 获取AudioSource组件后分配到变量
        source = GetComponent<AudioSource>();
        // 禁用MuzzleFlash MeshRenderer
        muzzleFlash.enabled = false;
    }

	// Update is called once per frame
	void Update () {
        // 使用以下函数在场景中显示射线
        Debug.DrawRay(firePos.position, firePos.forward * 10.0f, Color.green);
        // 鼠标左击时调用Fire函数
	    if (Input.GetMouseButtonDown(0))
	    {
	        Fire();

	        // 获取被射线击中的游戏对象
	        RaycastHit hit;

	        // 通过Raycast函数发射射线，有游戏对象被击中时返回true
	        if (Physics.Raycast(firePos.position, firePos.forward, out hit, 10.0f))
	        {
	            // 判断被射线击中的游戏对象Tag值是否为怪兽
	            if (hit.collider.tag == "Monster")
	            {
	                // SendMessage函数要传递的参数数组
	                object[] _params = new object[2];
	                _params[0] = hit.point; // 被射线击中的位置
	                _params[1] = 20;    // 怪兽将受到的伤害值

	                // 调用怪兽被击中并受伤的处理函数
	                hit.collider.gameObject.SendMessage("OnDamage", _params, SendMessageOptions.DontRequireReceiver);
	            }

                // 查看被射线击中的对象是否为barrel
	            if (hit.collider.tag == "Barrel")
	            {
	                // 为了计算油桶被击中时射线的入射角度，将射线发射原点与击中点传递给onDamage函数
                    object[] _params = new object[2];
	                _params[0] = firePos.position;
	                _params[1] = hit.point;
                    hit.collider.gameObject.SendMessage("OnDamage", _params, SendMessageOptions.DontRequireReceiver);
	            }
	        }
        }

	}

    void Fire()
    {
        // 1. 动态生成子弹的函数
        CreateBullet();
        // 播放声音的函数
        //source.PlayOneShot(fireSfx, 0.9f);
        GameMgr.instance.PlaySfx(firePos.position, fireSfx);
        // 使用例程调用处理MuzzleFlash效果的函数
        StartCoroutine(this.ShowMuzzleFlash());
    }

    void CreateBullet()
    {
        // 2. 动态生成Bullet预设
        Instantiate(bullet, firePos.position, firePos.rotation);
    }

    IEnumerator ShowMuzzleFlash()
    {
        // 随机更改MuzzleFlash大小
        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        // MuzzleFlash以z轴为基准随机旋转
        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360));
        muzzleFlash.transform.localRotation = rot;

        // 激活使其显示
        muzzleFlash.enabled = true;

        // 等待随机时间后再禁用MeshRenderer组件
        yield return new WaitForSeconds(Random.Range(0.05f, 0.3f));

        // 禁用MeshRenderer组件使其不显示
        muzzleFlash.enabled = false;
    }
}
