
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using TMPro;
//This script requires you to have setup your animator with 3 parameters, "InputMagnitude", "InputX", "InputZ"
//With a blend tree to control the inputmagnitude and allow blending between animations.
[RequireComponent(typeof(CharacterController))]
public class MovementInput : MonoBehaviour {
	public Transform FollowedCamera;
    public float Velocity;
    [Space]

	public float InputX;
	public float InputZ;

	
	public Vector3 desiredMoveDirection;
	public bool blockRotationPlayer;
	public float desiredRotationSpeed = 0.1f;
	public Animator anim;
	public float Speed;
	public float allowPlayerRotation = 0.1f;
	public Camera cam;
	public CharacterController controller;
	public bool isGrounded;
	AudioSource m_AudioSource;

    [Header("Animation Smoothing")]
    [Range(0, 1f)]
    public float HorizontalAnimSmoothTime = 0.2f;
    [Range(0, 1f)]
    public float VerticalAnimTime = 0.2f;
    [Range(0,1f)]
    public float StartAnimTime = 0.3f;
    [Range(0, 1f)]
    public float StopAnimTime = 0.15f;

    public float verticalVel;
    private Vector3 moveVector;

	public float JumpForce;
	
	public GameObject winTextObject;

	
	Animator m_Animator;
	private int count;
	public TextMeshProUGUI countText;

	
	
	public AudioSource winAudio;

	// Use this for initialization
	void Start () {
		anim = this.GetComponent<Animator> ();
		cam = Camera.main;
		controller = this.GetComponent<CharacterController> ();
		m_AudioSource = GetComponent<AudioSource> ();
		count = 0;
		SetCountText();

		winTextObject.SetActive(false);
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		InputMagnitude ();

		Jump();
		

        isGrounded = controller.isGrounded;
        if (isGrounded)
        {
            verticalVel -= 0;

			
        }
        else
        {
            verticalVel -= 1;
        }
        moveVector = new Vector3(0, verticalVel * .2f * Time.deltaTime, 0);
        controller.Move(moveVector);

		
		
		
    }

	void FixedUpdate ()
    {
		bool hasHorizontalInput = !Mathf.Approximately (InputX, 0f);
        bool hasVerticalInput = !Mathf.Approximately (InputZ, 0f);
        bool Blend = hasHorizontalInput || hasVerticalInput;
		
		
		if (Blend)
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else
        {
            m_AudioSource.Stop ();
        }
	}

	

    void PlayerMoveAndRotation() {
		InputX = Input.GetAxis ("Horizontal");
		InputZ = Input.GetAxis ("Vertical");

		

		var camera = Camera.main;
		var forward = cam.transform.forward;
		var right = cam.transform.right;

		forward.y = 0f;
		right.y = 0f;

		forward.Normalize ();
		right.Normalize ();

		desiredMoveDirection = forward * InputZ + right * InputX;

		if (blockRotationPlayer == false) {
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (desiredMoveDirection), desiredRotationSpeed);
            controller.Move(desiredMoveDirection * Time.deltaTime * Velocity);
		}
	}

    public void LookAt(Vector3 pos)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), desiredRotationSpeed);
    }

    public void RotateToCamera(Transform t)
    {

        var camera = Camera.main;
        var forward = cam.transform.forward;
        var right = cam.transform.right;

        desiredMoveDirection = forward;

        t.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
    }

	void InputMagnitude() {
		//Calculate Input Vectors
		InputX = Input.GetAxis ("Horizontal");
		InputZ = Input.GetAxis ("Vertical");

		

		//anim.SetFloat ("InputZ", InputZ, VerticalAnimTime, Time.deltaTime * 2f);
		//anim.SetFloat ("InputX", InputX, HorizontalAnimSmoothTime, Time.deltaTime * 2f);

		//Calculate the Input Magnitude
		Speed = new Vector2(InputX, InputZ).sqrMagnitude;

        //Physically move player

		if (Speed > allowPlayerRotation) {
			anim.SetFloat ("Blend", Speed, StartAnimTime, Time.deltaTime);
			PlayerMoveAndRotation ();
		} else if (Speed < allowPlayerRotation) {
			anim.SetFloat ("Blend", Speed, StopAnimTime, Time.deltaTime);
		}

		
	}

	void Jump()
	{
		Vector3 mDir = Vector3.zero;
		if(Input.GetKeyDown(KeyCode.Space))

		{
			
			anim.SetTrigger("Jump");
			mDir.y = JumpForce;
			
			
		}
	}

	private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HourGlass")) 
        {
            other.gameObject.SetActive(false);

			count = count + 1;
			SetCountText();
        }
    }

	void SetCountText()
	{
		countText.text = "SCORE: " + count.ToString();

		if (count >= 10) 
		{
                    // Set the text value of your 'winText'
                    winTextObject.SetActive(true);
					winAudio.Play();

		}
	}

	

	
}
