using UnityEngine;
using System.Collections;

public class PlayerAnims : MonoBehaviour 
{
public enum anim { None, WalkLeft, WalkRight, RopeLeft, RopeRight, Climb, ClimbStop, StandLeft, StandRight, HangLeft, HangRight, FallLeft, FallRight , ShootLeft, ShootRight }
	
	public Transform spriteParent;
	public tk2dAnimatedSprite playerSprite;
	
	private anim currentAnim;
	private Player player;
	
	// Use this for initialization
	void Start () 
	{
		player = GetComponent<Player>();
	}
	
	void Update() 
	{		
		// run left
		if(player.isLeft && player.grounded == true && currentAnim != anim.WalkLeft)
		{
			currentAnim = anim.WalkLeft;
			playerSprite.Play("run");
			spriteParent.localScale = new Vector3(1,1,1);
		}
		if(!player.isLeft && player.grounded == true && currentAnim != anim.StandLeft && player.facingDir == Player.facing.Left)
		{
			currentAnim = anim.StandLeft;
			playerSprite.Play("stand"); // stand left
			spriteParent.localScale = new Vector3(1,1,1);
		}
		
		// run right
		if(player.isRight && player.grounded && currentAnim != anim.WalkRight)
		{
			currentAnim = anim.WalkRight;
			playerSprite.Play("run");
			spriteParent.localScale = new Vector3(-1,1,1);
		}
		if(!player.isRight && player.grounded && currentAnim != anim.StandRight && player.facingDir == Player.facing.Right)
		{
			currentAnim = anim.StandRight;
			playerSprite.Play("stand"); // stand left
			spriteParent.localScale = new Vector3(-1,1,1);
		}
		
		// falling
		if(player.grounded == false && currentAnim != anim.FallLeft && player.facingDir == Player.facing.Left)
		{
			currentAnim = anim.FallLeft;
			playerSprite.Play("jump"); // fall left
			spriteParent.localScale = new Vector3(1,1,1);
		}
		if(player.grounded == false && currentAnim != anim.FallRight && player.facingDir == Player.facing.Right)
		{
			currentAnim = anim.FallRight;
			playerSprite.Play("jump"); // fall right
			spriteParent.localScale = new Vector3(-1,1,1);
		}
	}
}
