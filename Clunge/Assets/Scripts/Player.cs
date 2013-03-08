using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	
	[HideInInspector] public enum facing { Right, Left }
	[HideInInspector] public facing facingDir;
	
	[HideInInspector] public enum moving { Right, Left, None }
	[HideInInspector] public moving movingDir;
	
	[HideInInspector] public bool isLeft;
	[HideInInspector] public bool isRight;
	[HideInInspector] public bool isJump;
	
	[HideInInspector] public bool jumping = false;
	[HideInInspector] public bool grounded = false;
	
	[HideInInspector] public bool blockedRight;
	[HideInInspector] public bool blockedLeft;
	[HideInInspector] public bool blockedUp;
	[HideInInspector] public bool blockedDown;
	
	[HideInInspector] public bool alive = true;
	[HideInInspector] public Vector3 spawnPos;
	
	protected Transform thisTransform;
	
	private float moveVel;
	public float runVel = 4f;
	private Vector3 vel2;
	private Vector3 vel;
	
	public float jumpVel = 16f;
	public float jump2Vel = 14f;
	public float fallVel = 18f;
	
	private int jumps = 0;
    private int maxJumps = 2; // set to 2 for double jump
	
	private float gravityY;// = 52f;
	public float gravity = 52f;
	private float maxVelY = 0f;
		
	private RaycastHit hitInfo;
	private float halfMyX = 0.325f; //0.25f;
	private float halfMyY = 0.5f;//0.375f;
	[HideInInspector] public float rayDistUp = 0.375f;
	
	private float absVel2X;
	private float absVel2Y;
	
	// layer masks
	protected int groundMask = 1 << 9 | 1 << 8; // Ground, Block
	protected int blockMask = 1 << 8; //Block
	
	public virtual void Awake()
	{
		thisTransform = transform;
	}
	
	// Use this for initialization
	public virtual void Start () 
	{
		moveVel = runVel;
		maxVelY = fallVel;
		vel.y = 0;
		StartCoroutine(StartGravity());
		
		spawnPos = thisTransform.position;
	}
	
	IEnumerator StartGravity()
	{
		// wait for things to settle before applying gravity
		yield return new WaitForSeconds(0.1f);
		//gravityY = 52f;
		gravityY = gravity;
	}
	
	// Update is called once per frame
	public virtual void UpdateMovement() 
	{		
		vel.x = 0;
		
		// pressed right button
		if(isRight == true)
		{
			vel.x = moveVel;
		}
		
		// pressed left button
		if(isLeft == true)
		{			
			vel.x = -moveVel;
		}
		
		// pressed jump button
		if (isJump == true)
		{
			if (jumps < maxJumps)
		    {
				jumps += 1;
				jumping = true;
				if(jumps == 1)
				{
					vel.y = jumpVel;
				}
				if(jumps == 2)
				{
					vel.y = jump2Vel;
				}
		    }
		}
		
		// landed from fall/jump
		if(grounded == true && vel.y == 0)
		{
			jumping = false;
			jumps = 0;
		}
		
		UpdateRaycasts();
		
		// apply gravity while airborne
		if(grounded == false)
		{
			vel.y -= gravityY * Time.deltaTime;
		}
		
		// velocity limiter
		if(vel.y < -maxVelY)
		{
			vel.y = -maxVelY;
		}
		
		// apply movement 
		vel2 = vel * Time.deltaTime;
		thisTransform.position += new Vector3(vel2.x,vel2.y,0f);
	}
	
	// ============================== RAYCASTS ============================== 
	
	void UpdateRaycasts()
	{
		blockedRight = false;
		blockedLeft = false;
		blockedUp = false;
		blockedDown = false;
		grounded = false;		
		
		absVel2X = Mathf.Abs(vel2.x);
		absVel2Y = Mathf.Abs(vel2.y);
		
		if (Physics.Raycast(new Vector3(thisTransform.position.x-0.25f,thisTransform.position.y,thisTransform.position.z), -Vector3.up, out hitInfo, 0.6f+absVel2Y, groundMask) 
			|| Physics.Raycast(new Vector3(thisTransform.position.x+0.25f,thisTransform.position.y,thisTransform.position.z), -Vector3.up, out hitInfo, 0.6f+absVel2Y, groundMask))
		{			
			// not while jumping so he can pass up thru platforms
			if(vel.y <= 0)
			{
				grounded = true;
				vel.y = 0f; // stop falling			
				thisTransform.position = new Vector3(thisTransform.position.x,hitInfo.point.y+halfMyY,0f);
			}
		}
		
		// blocked up
		if (Physics.Raycast(new Vector3(thisTransform.position.x-0.2f,thisTransform.position.y,thisTransform.position.z), Vector3.up, out hitInfo, rayDistUp+absVel2Y, groundMask)
			|| Physics.Raycast(new Vector3(thisTransform.position.x+0.2f,thisTransform.position.y,thisTransform.position.z), Vector3.up, out hitInfo, rayDistUp+absVel2Y, groundMask))
		{
			BlockedUp();
		}
		
		// blocked on right
		if (Physics.Raycast(new Vector3(thisTransform.position.x,thisTransform.position.y,thisTransform.position.z), Vector3.right, out hitInfo, halfMyX+absVel2X, groundMask))
		{
			BlockedRight();
		}
		
		// blocked on left
		if(Physics.Raycast(new Vector3(thisTransform.position.x,thisTransform.position.y,thisTransform.position.z), -Vector3.right, out hitInfo, halfMyX+absVel2X, groundMask))
		{
			BlockedLeft();
		}
	}
	
	void BlockedUp()
	{
		if(vel.y > 0)
		{
			vel.y = 0f;
			blockedUp = true;
		}
	}

	void BlockedRight()
	{
		if(facingDir == facing.Right || movingDir == moving.Right)
		{
			blockedRight = true;
			vel.x = 0f;
			thisTransform.position = new Vector3(hitInfo.point.x-(halfMyX-0.01f),thisTransform.position.y, 0f); // .01 less than collision width.
		}
	}
	
	void BlockedLeft()
	{
		if(facingDir == facing.Left || movingDir == moving.Left)
		{
			blockedLeft = true;
			vel.x = 0f;
			thisTransform.position = new Vector3(hitInfo.point.x+(halfMyX-0.01f),thisTransform.position.y, 0f); // .01 less than collision width.
		}
	}
	
	public void Update()
	{
		// these are false unless one of keys is pressed
		isLeft = false;
		isRight = false;
		isJump = false;
		
		movingDir = moving.None;
		
		// keyboard input
		if(Input.GetKey(KeyCode.A)) 
		{ 
			isLeft = true; 
			facingDir = facing.Left;
		}
		if (Input.GetKey(KeyCode.D) && isLeft == false) 
		{ 
			isRight = true; 
			facingDir = facing.Right;
		}
		
		if (Input.GetKeyDown(KeyCode.W)) 
		{ 
			isJump = true; 
		}
		
		if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.T))
		{
			//print ("reload level");
			Application.LoadLevel(0);
		}
		
		UpdateMovement();
	}
	
	// ============================== PLAYER VISIBILITY. ==============================
	
	public virtual void HideMe()
	{
		print ("hide me");
		alive = false;
		thisTransform.position = new Vector3(20,0,0);
	}
	
	public virtual void ShowMe()
	{
		alive = true;
		thisTransform.position = spawnPos;
	}
}
