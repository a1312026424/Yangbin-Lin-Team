using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCloseUIControl : MonoBehaviour
{
    public GameObject m_menu0;
    public GameObject m_food;
    public GameObject m_sleep;
    public GameObject m_happy;
    private bool click_menu;
    private bool menu_open = false;

    void Start()
    {
    }

    void Update()
    {

    }

    /*
     * 初始化时隐藏除主菜单之外的图片
     * 当点击时，如果此时菜单是打开的就关闭，菜单是关闭的就打开
     */
    public void clickMenu()
    {
        gameObject.SetActive(false);
        m_menu0.SetActive(true);
        m_food.SetActive(false);
        m_sleep.SetActive(false);
        m_happy.SetActive(false);
    }
}
