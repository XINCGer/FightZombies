/*
ProjectName: 丧尸危机
Author: 马三小伙儿
Blog: http://www.cnblogs.com/msxh/
Github:https://github.com/XINCGer
Date: 2016/05/18
*/
using UnityEngine;
using System.Collections;

public class MiniCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	//获得屏幕分辨率
		float ratio = (float)Screen.width / (float)Screen.height;
		//使摄像机永远是一个正方形，rect的前两个参数是xy位置，后两个参数是xy大小
		this.GetComponent<Camera>().rect = new Rect((1 - 0.2f), (1 - 0.2f * ratio), 0.2f, 0.2f * ratio);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
