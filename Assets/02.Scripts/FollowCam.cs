using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform targetTr;
    public float dist = 5.0f;
    public float height = 2.0f;
    public float dampTrace = 20.0f;

    private Transform tr;

	// Use this for initialization
	void Start ()
	{
        // 将摄像机本身的Transform组件分配至tr
	    tr = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
        // 将摄像机放置在被追踪目标的后方的dist距离的位置
        // 将摄像机向上抬高height
	    tr.position = Vector3.Lerp(tr.position, targetTr.position - (targetTr.forward * dist) + (Vector3.up * height),
	        Time.deltaTime * dampTrace);

        // 使摄像机朝向游戏对象
        tr.LookAt(targetTr.position);
	}
}
