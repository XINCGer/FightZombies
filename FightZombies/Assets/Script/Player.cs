/*
ProjectName: 丧尸危机
Author: 马三小伙儿
Blog: http://www.cnblogs.com/msxh/
Github:https://github.com/XINCGer
Date: 2016/05/18
*/
using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

	public Transform m_transform;
	//枪口Transform
	Transform m_muzzlepoint;
	//射击时,射线能射到的碰撞层
	public LayerMask m_layer;
	//射中目标后的粒子效果
	public Transform m_fx;
	//射击音效
	public AudioClip m_audio;
	//射击间隔时间
	float m_shootTimer = 0;
	//摄像机的Transform
	Transform m_camTransform;
	//摄像机旋转角度
	Vector3 m_camRot;
	//摄像机高度(即表示主角的身高)
	float m_camHeight = 1.4f;
	//角色控制器
	CharacterController m_ch;
	//角色移动速度
	float m_moveSpeed = 3.0f;
	//重力
	float m_gravity = 2.0f;
	//生命值
	public int m_life = 5;
	
	// Use this for initialization
	void Start ()
	{
		m_transform = this.transform;
		//获取角色控制器组件
		m_ch = this.GetComponent<CharacterController> ();

		//获取摄像机
		m_camTransform = Camera.main.transform;
		//设置摄像机初始位置
		Vector3 pos = m_transform.position;
		pos.y += m_camHeight;
		m_camTransform.position = pos;
		//设置摄像机的旋转方向和主角一致
		m_camTransform.rotation = m_transform.rotation;
		m_camRot = m_transform.eulerAngles;
		//锁定鼠标
		/*5.0之前的函数
		Screen.lockCursor = true; */
		/*5.0之后的函数*/
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		// 查找muzzlepoint
		m_muzzlepoint = m_camTransform.FindChild("M16/weapon/muzzlepoint").transform;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//如果生命值为0，则什么也不做
		if (m_life <= 0)
			return;
		Control ();
	}

	void Control ()
	{
		//获取鼠标移动距离
		float rh = Input.GetAxis ("Mouse X");
		float rv = Input.GetAxis ("Mouse Y");
		//旋转摄像机
		m_camRot.x -= rv;
		m_camRot.y += rh;
		m_camTransform.eulerAngles = m_camRot;

		//使主角的面向方向与摄像机一致
		Vector3 camrot = m_camTransform.eulerAngles;
		camrot.x = 0;
		camrot.z = 0;
		m_transform.eulerAngles = camrot;

		//定义3个值控制移动
		float xm = 0, ym = 0, zm = 0;
		//重力运动
		ym -= m_gravity * Time.deltaTime;
		//上下左右运动
		if (Input.GetKey (KeyCode.W)) {
			zm += m_moveSpeed * Time.deltaTime;
		} else if (Input.GetKey (KeyCode.S)) {
			zm -= m_moveSpeed * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.A)) {
			xm -= m_moveSpeed * Time.deltaTime;
		} else if (Input.GetKey (KeyCode.D)) {
			xm += m_moveSpeed * Time.deltaTime;
		}
		// 更新射击间隔时间
		m_shootTimer -= Time.deltaTime;
		
		// 鼠标左键射击
		if (Input.GetMouseButton(0) && m_shootTimer<=0)
		{
			m_shootTimer = 0.1f;
			
			this.GetComponent<AudioSource>().PlayOneShot(m_audio);
			
			// 减少弹药，更新弹药UI
			GameManager.Instance.SetAmmo(1);
			
			// RaycastHit用来保存射线的探测结果
			RaycastHit info;
			
			// 从muzzlepoint的位置，向摄像机面向的正方向射出一根射线
			// 射线只能与m_layer所指定的层碰撞
			bool hit = Physics.Raycast(m_muzzlepoint.position, m_camTransform.TransformDirection(Vector3.forward), out info, 100, m_layer);
			if (hit)
			{
				// 如果射中了tag为enemy的游戏体
				if (info.transform.tag.CompareTo("enemy") == 0)
				{
					Enemy enemy = info.transform.GetComponent<Enemy>();
					
					// 敌人减少生命
					enemy.OnDamage(1);
				}
				
				// 在射中的地方释放一个粒子效果
				Instantiate(m_fx, info.point, info.transform.rotation);
			}
		}
		//使用角色控制器提供的Move函数进行移动，它会自动检测碰撞
		m_ch.Move (m_transform.TransformDirection (new Vector3 (xm, ym, zm)));

		//使摄像机的位置与主角一致
		Vector3 pos = m_transform.position;
		pos.y += m_camHeight;
		m_camTransform.position = pos;
	}

	//在编辑器中为主角显示一个图标
	void OnDrawGizmos ()
	{
		Gizmos.DrawIcon (this.transform.position, "Spawn.tif");
	}

	//减少主角的生命,更新UI界面
	public void OnDamage(int damage)
	{
		m_life -= damage;
		
		// 更新UI
		GameManager.Instance.SetLife(m_life);
		
		// 如果生命为0，解锁鼠标显示
		if (m_life <= 0)
			Screen.lockCursor = false;
	}
}
