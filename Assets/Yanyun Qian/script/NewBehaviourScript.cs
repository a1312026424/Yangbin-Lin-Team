using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    public float turnSpeed = 20f;
    private int count;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public GameObject loseTextObject;
    private AudioSource source;
    public AudioClip impact;
    public AudioClip victory;
    Animator m_Animator;
    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    public float fadeDuration = 1f;
    public float displayImageDuration = 1f;
    public CanvasGroup exitBackgroundImageCanvasGroup;
    public CanvasGroup CanvasGroup;
    bool m_IsPlayerAtExit;
    bool m_Exit;
    float m_Timer;

    void Start()
    {
        
        m_Animator = GetComponent<Animator> ();
        m_Rigidbody = GetComponent<Rigidbody> ();
        count = 0;
        SetCountText();
        //winTextObject.SetActive(false);
        //loseTextObject.SetActive(false);
    
    }

    void SetCountText()
    {
            countText.text = "Count:" + count.ToString();
            if(count >= 7)
            {
                //winTextObject.SetActive(true);
                m_IsPlayerAtExit = true;
                GetComponent<AudioSource>().PlayOneShot(victory);
            }
    }
    void Update ()
    {
        if(m_IsPlayerAtExit)
        {
            EndLevel ();
        }
        if(m_Exit)
        {
            EndL ();
        }
    }

    void EndLevel ()
    {
        m_Timer += Time.deltaTime;

        exitBackgroundImageCanvasGroup.alpha = m_Timer / fadeDuration;

        if(m_Timer > fadeDuration + displayImageDuration)
        {
            
            Application.Quit ();
        }
    }
   


    void EndL ()
    {
        m_Timer += Time.deltaTime;

        CanvasGroup.alpha = m_Timer / fadeDuration;
        if (m_Timer > fadeDuration + displayImageDuration)
        {
            
                SceneManager.LoadScene ("Yanyun Qian Game");
            
        }
        
    }
    
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        m_Movement.Set(horizontal,0f,vertical);
        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool ("IsWalking", isWalking);

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            GetComponent<AudioSource>().PlayOneShot(impact);
            count= count+1;
            SetCountText();
        }
        if(other.gameObject.CompareTag("Enemy"))
        {
            //loseTextObject.SetActive(true);
            m_Exit = true;
        }
        
    }
    void OnAnimatorMove()
    {
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * 0.02f);
        m_Rigidbody.MoveRotation(m_Rotation);
    }
}