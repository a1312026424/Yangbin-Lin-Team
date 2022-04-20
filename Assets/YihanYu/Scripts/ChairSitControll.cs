using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairSitControll : MonoBehaviour
{
    private Animator m_Animator = null;
    private RaycastHit hit;//射线检测的碰撞体的位置
    private Ray ray;//声明射线

    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))//当点击鼠标左键时（左键为0，右键为1）
        {
            if (Physics.Raycast(ray, out hit))//Physics.Raycast()表示当射线（ray）与任何碰撞体发生接触时返回true，否则返回false
            {
                if (hit.collider.gameObject.tag == "Chair")//当射线碰撞到的是Plane（此if语句限制鼠标点击位置在Plane上有效）
                {
                    print("点击椅子");
                    m_Animator.SetBool("Sit", true);
                }
            }
        }
    }
}
