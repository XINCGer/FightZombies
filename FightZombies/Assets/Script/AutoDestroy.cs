/*
ProjectName: 丧尸危机
Author: 马三小伙儿
Blog: http://www.cnblogs.com/msxh/
Github:https://github.com/XINCGer
Date: 2016/05/18
*/
using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour {

	public float m_timer=1.0f;

	// Use this for initialization
	void Start () {
		Destroy (this.gameObject,m_timer);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
