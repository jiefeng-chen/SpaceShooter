using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{

    public int damage = 20;
    public float speed = 3000.0f;

	// Use this for initialization
	void Start ()
	{
	    GetComponent<Rigidbody>().AddForce(transform.forward * speed);
	}

}
