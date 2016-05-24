/*
ProjectName: 丧尸危机
Author: 马三小伙儿
Blog: http://www.cnblogs.com/msxh/
Github:https://github.com/XINCGer
Date: 2016/05/18
*/
using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	// Transform组件
	Transform m_transform;
	
	// 动画组件
	Animator m_ani;
	
	// 寻路组件
	NavMeshAgent m_agent;
	
	// 主角
	Player m_player;
	
	// 角色移动速度
	float m_movSpeed = 0.5f;
	
	// 角色旋转速度
	float m_rotSpeed = 30;
	
	//  计时器
	float m_timer = 2;
	
	// 生命值
	int m_life = 15;

	//生成点
	protected EnemySpawn m_spawn;

	// Use this for initialization
	void Start ()
	{
		
		// 获取组件
		m_transform = this.transform;
		m_ani = this.GetComponent<Animator> ();
		m_agent = GetComponent<NavMeshAgent> ();
		
		// 获得主角
		m_player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
		
	}
	
	
	// Update is called once per frame
	void Update ()
	{
		
		// 如果主角生命为0，什么也不做
		if (m_player.m_life <= 0)
			return;
		
		// 获取当前动画状态
		AnimatorStateInfo stateInfo = m_ani.GetCurrentAnimatorStateInfo (0);
		
		// 如果处于待机状态
		if (stateInfo.fullPathHash == Animator.StringToHash ("Base Layer.idle") && !m_ani.IsInTransition (0)) {
			m_ani.SetBool ("idle", false);
			
			// 待机一定时间
			m_timer -= Time.deltaTime;
			if (m_timer > 0)
				return;
			
			// 如果距离主角小于1.5米，进入攻击动画状态
			if (Vector3.Distance (m_transform.position, m_player.m_transform.position) < 1.5f) {
				m_ani.SetBool ("attack", true);
			} else {
				// 重置定时器
				m_timer = 1;
				
				// 设置寻路目标点
				m_agent.SetDestination (m_player.m_transform.position);
				
				// 进入跑步动画状态
				m_ani.SetBool ("run", true);
			}
		}
		
		// 如果处于跑步状态
		if (stateInfo.fullPathHash == Animator.StringToHash ("Base Layer.run") && !m_ani.IsInTransition (0)) {
			
			m_ani.SetBool ("run", false);
			
			
			// 每隔1秒重新定位主角的位置
			m_timer -= Time.deltaTime;
			if (m_timer < 0) {
				m_agent.SetDestination (m_player.m_transform.position);
				
				m_timer = 1;
			}
			
			// 追向主角
			//MoveTo ();
			
			// 如果距离主角小于1.5米，向主角攻击
			if (Vector3.Distance (m_transform.position, m_player.m_transform.position) <= 1.5f) {
				//重置寻路	
				m_agent.ResetPath ();
				// 进入攻击状态
				m_ani.SetBool ("attack", true);
			}
		}
		
		// 如果处于攻击状态
		if (stateInfo.fullPathHash == Animator.StringToHash ("Base Layer.attack") && !m_ani.IsInTransition (0)) {
			
			// 面向主角
			RotateTo ();
			
			m_ani.SetBool ("attack", false);
			// 如果攻击动画播完，重新进入待机状态
			if (stateInfo.normalizedTime >= 1.0f) {
				//进入待机状态
				m_ani.SetBool ("idle", true);
				
				// 重置计时器
				m_timer = 2;
				//更新主角的生命值
				m_player.OnDamage(1);

			}
		}
		//如果处于死亡状态
		if (stateInfo.fullPathHash == Animator.StringToHash ("Base Layer.death") && !m_ani.IsInTransition (0)) {
			//当播放完成死亡动画
			if(stateInfo.normalizedTime>=1.0f){
				//更新敌人数量
				m_spawn.m_enemyCount--;
				//加分
				GameManager.Instance.SetScore(100);
				//销毁自身
				Destroy(this.gameObject);
			}
		}
		
	}
	
	// 转向目标点
	void RotateTo ()
	{
		// 获取目标方向
		Vector3 targetdir = m_player.m_transform.position - m_transform.position;
		// 计算出新方向
		Vector3 newDir = Vector3.RotateTowards (transform.forward, targetdir, m_rotSpeed * Time.deltaTime, 0.0f);
		// 旋转至新方向
		m_transform.rotation = Quaternion.LookRotation (newDir);
	}
	
	// 寻路移动
	void MoveTo ()
	{
		float speed = m_movSpeed * Time.deltaTime;
		m_agent.Move (m_transform.TransformDirection ((new Vector3 (0, 0, speed))));
		
	}

	//伤害函数
	public void OnDamage(int damage){
		m_life -= damage;
		//如果生命值为0,进入死亡状态
		if (m_life <= 0) {
			m_ani.SetBool("death",true);
		}
	}

	//初始化敌人
	public void Init(EnemySpawn spawn){
		m_spawn = spawn;
		//该敌人的出生点增加计数
		m_spawn.m_enemyCount++;
	}
}
