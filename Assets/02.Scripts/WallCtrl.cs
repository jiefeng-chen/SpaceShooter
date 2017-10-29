using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCtrl : MonoBehaviour {

    // 表示火花粒子对象的变量
    public GameObject sparkEffect;

    private void OnCollisionEnter(Collision collision)
    {
        // 比较发生碰撞的游戏对象的tag值
        if (collision.collider.tag == "Bullet")
        {
            // 生成火花粒子
            GameObject spark = Instantiate(sparkEffect, collision.transform.position, Quaternion.identity) as GameObject;


            Destroy(spark, spark.GetComponent<ParticleSystem>().main.duration + 0.2f);
 
          

            // 删除发生碰撞的游戏对象
            Destroy(collision.gameObject);
        }
    }

}
