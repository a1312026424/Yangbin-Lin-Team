using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerControl : MonoBehaviour
{
    
    private enum ControlMode
    {
        /*---------------------------------------------定义角色移动的方式---------------------------------------------------*/
        /// <summary>
        /// Character freely moves in the chosen direction from the perspective of the camera
        /// 角色从摄像机的显示范围角度内选择方向移动
        /// </summary>
        Direct
    }

    /// <summary>
    ///声明角色移动速度——方向键控制状态
    /// </summary>
    [SerializeField] private float m_moveSpeed = 2;
    /// <summary>
    ///声明角色移动速度——鼠标点击控制状态
    /// </summary>
    private float click_speed = 0;
    /// <summary>
    ///声明角色旋转速度
    /// </summary>
    [SerializeField] private float m_turnSpeed = 200;
    /// <summary>
    ///声明角色跳跃力度
    /// </summary>
    [SerializeField] private float m_jumpForce = 4;

    /// <summary>
    ///声明舞台上的物体
    /// </summary>
    [SerializeField] private Animator m_animator = null;
    /// <summary>
    ///声明舞台上的刚体
    /// </summary>
    [SerializeField] private Rigidbody m_rigidBody = null;
    /// <summary>
    ///声明控制角色移动的方式
    /// </summary>
    [SerializeField] private ControlMode m_controlMode = ControlMode.Direct;

    /// <summary>
    ///声明当前是上还是下
    /// </summary>
    private float m_currentV = 0;
    /// <summary>
    ///声明当前是左还是右
    /// </summary>
    private float m_currentH = 0;

    /// <summary>
    ///声明
    /// </summary>
    private readonly float m_interpolation = 10;
    
    //奔跑前进的偏移量是1F
    /// <summary>
    ///声明前进行走的偏移量
    /// </summary>
    private readonly float m_walkScale = 0.33f;
    /// <summary>
    ///声明后退行走的偏移量
    /// </summary>
    private readonly float m_backwardsWalkScale = 0.16f;
    /// <summary>
    ///声明后退奔跑的偏移量
    /// </summary>
    private readonly float m_backwardRunScale = 0.66f;

    /// <summary>
    ///声明模型  刚才  是否在地面上
    /// </summary>
    private bool m_wasGrounded;
    /// <summary>
    ///声明模型  现在  是否在地面上
    /// </summary>
    private bool m_isGrounded;
    /// <summary>
    ///声明当前的方向
    /// </summary>
    private Vector3 m_currentDirection = Vector3.zero;

    /// <summary>
    ///声明跳起来的时间
    /// </summary>
    private float m_jumpTimeStamp = 0;
    /// <summary>
    ///声明跳跃CD的时间
    /// </summary>
    private float m_minJumpInterval = 0.25f;
    /// <summary>
    ///声明是否跳跃
    /// </summary>
    private bool m_jumpInput = false;

    /// <summary>
    ///声明舞台上的碰撞体（舞台上的地板啊、花啊、树啊、都会有一个Mesh Collider）
    /// </summary>
    private List<Collider> m_collisions = new List<Collider>();

    /// <summary>
    ///声明射线检测的碰撞体的位置
    /// </summary>
    private RaycastHit hit;
    /// <summary>
    ///声明射线
    /// </summary>
    private Ray ray;

    private NavMeshAgent nav;

    public int scoreCnt;
	public Text score;
	public GameObject gameOver;
	public Animator animator;

    void Start () {
        scoreCnt=0;
		score.text = string.Format("Score:{0}", scoreCnt);
		gameOver.gameObject.SetActive(false);
	}

    /**
    *  收集食物
    */
    void OnTriggerEnter(Collider collider)
	{
		if (collider.tag == "Food")
		{
			scoreCnt++;
			score.text = string.Format("Score:{0}", scoreCnt);
			collider.gameObject.SetActive(false);
		}
	}

    private void Awake()
    {
        if (!m_animator) {
            gameObject.GetComponent<Animator>();
        }
        if (!m_rigidBody) {
            gameObject.GetComponent<Animator>();
        }

        nav = this.GetComponent<NavMeshAgent>();
    }

    /**
     * 进入碰撞
     * ·OnCollisionEnter方法被触发要符合以下条件
     * 1 碰撞双方必须是碰撞体 
     * 2 碰撞的主动方必须是刚体，注意用词是主动方，而不是被动方 
     * 3 刚体不能勾选IsKinematic 
     * 4 碰撞体不能够勾选IsTigger
     */
    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)//---当前的碰撞体立在世界中，就是碰撞体？？？
            {
                /* 不包含遍历的碰撞体 */
                if (!m_collisions.Contains(collision.collider))
                {
                    m_collisions.Add(collision.collider);//***---添加碰撞体到碰撞体数组---***
                }
                m_isGrounded = true;//***---现在在地面上---***
            }
        }
    }

    /**
     * 逗留碰撞
     */
    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;//***---是否是有效的碰撞地面---***
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true;//***---有效的碰撞体---***
                break;
            }
        }

        if (validSurfaceNormal)
        {
            m_isGrounded = true;//***---现在在地面上---***
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);//***---添加碰撞体到碰撞体数组---***
            }
        }
        else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);//***---不是有效的碰撞地面，从碰撞体数组中移除碰撞体---***
            }
            if (m_collisions.Count == 0) {
                m_isGrounded = false;//***---碰撞体数量为0，现在不在地面上---***
            }
        }
    }

    /**
     * 离开碰撞
     */
    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);//***---从碰撞体数组中移除碰撞体---***
        }
        if (m_collisions.Count == 0) {
            m_isGrounded = false;//***---碰撞体数量为0，现在不在地面上---***
        }
    }

    private void Update()
    {
        /* 现在不是跳跃的状态下 按下了空格键 */
        if (!m_jumpInput && Input.GetKey(KeyCode.Space))
        {
            m_jumpInput = true;//***---现在是跳跃状态---***
        }
    }

    /**
     * 固定更新：FixedUpdate的时间间隔可以在项目设置中更改，Edit->Project Setting->time 找到Fixed timestep，就可以修改了。
     */
    private void FixedUpdate()
    {
        m_animator.SetBool("Grounded", m_isGrounded);//***---为Animations传值，  当前  是否在地面上---***

        DirectUpdate();

        m_wasGrounded = m_isGrounded;//***---更新刚刚在地面上的状态---***
        m_jumpInput = false;//***---更新跳跃状态为默认false---***
    }


    private void DirectUpdate()
    {
        float v = Input.GetAxis("Vertical");//***---上下;检测垂直方向键---***
        float h = Input.GetAxis("Horizontal");//***---左右;检测水平方向键---***

        Transform camera = Camera.main.transform;//***---获取相机---***

        /*前进时行走*/
        if (Input.GetKey(KeyCode.LeftShift))
        {
            v *= m_walkScale;
            h *= m_walkScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);//***---上下偏移量---***
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);//***---左右偏移量---***

        Vector3 direction = camera.forward * m_currentV + camera.right * m_currentH;//***---根据摄像机的朝向、X轴、以及当前的左右偏移量  定位当前的方向---***

        float directionLength = direction.magnitude;//***---magnitude返回向量长度---***
        direction.y = 0;
        direction = direction.normalized * directionLength;//***---normalized规范向量---***

        if (direction != Vector3.zero)
        {
            m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);//***---方向---***

            transform.rotation = Quaternion.LookRotation(m_currentDirection);
            transform.position += m_currentDirection * m_moveSpeed * Time.deltaTime;

            m_animator.SetFloat("MoveSpeed", direction.magnitude);//将前或后的移动偏移量赋值到动作上面
        }

        JumpingAndLanding();
    }

    /**
     * 跳起来和站立的时候
     */
    private void JumpingAndLanding()
    {
        /* Time.time 表示从游戏开发到现在的时间，会随着游戏的暂停而停止计算 */
        //print("===Time.time===All===" + Time.time);
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;//***---跳跃冷却时间(游戏开始到现在的时间 减去 跳跃时的时间)是否 大于 设定的跳跃在空中的时间---***

        /* 跳跃冷却之后 有可碰撞地面 键盘输入空格则m_jumpInput = true */
        if (jumpCooldownOver && m_isGrounded && m_jumpInput)
        {
            m_jumpTimeStamp = Time.time;
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);//***---增加下落的重力---***
        }

        /* 刚才没在地面上 现在在地面上 */
        if (!m_wasGrounded && m_isGrounded)
        {
            m_animator.SetTrigger("Land");//***---站立状态---***
        }

        /* 现在不在地面上 刚才在地面上 */
        if (!m_isGrounded && m_wasGrounded)
        {
            m_animator.SetTrigger("Jump");//***---跳跃状态---***
        }
    }
}
