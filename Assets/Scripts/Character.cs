using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Unit {
	[SerializeField]
	private int lives = 10;

	public int Lives{
		get{ return lives;}
		set{ 
			if (value < 10)
				lives = value;
			livesBar.Refresh ();
		}
	}

	LivesBar livesBar;
	[SerializeField]
	private float speed = 6.0F;
	[SerializeField]
	private float jumpForce = 105.0f	;

	private bool isGrounded = false;

	private Bullet bullet;

	new private Rigidbody2D rigidbody;
	private Animator animator;
	private SpriteRenderer sprite;

	private CharState State{
		get{ return (CharState)animator.GetInteger ("State");}
		set{ animator.SetInteger ("State", (int)value);}
	}

	private void Awake(){

		livesBar = FindObjectOfType<LivesBar> ();
		rigidbody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		sprite = GetComponentInChildren<SpriteRenderer>();

		bullet = Resources.Load<Bullet> ("Bullet");
	}

	private void Update(){

		if (Input.GetButtonDown ("Fire1"))
			Shoot ();

		if(isGrounded)
			State = CharState.Idle;
		if(Input.GetButton("Horizontal")){
			Run();
		}
		if(isGrounded && Input.GetButton("Jump")){
			Jump ();
		} 
	}


	private void Run(){
		Vector3 direction = transform.right * Input.GetAxis("Horizontal");
		transform.position = Vector3.MoveTowards (transform.position, transform.position + direction, speed * Time.deltaTime);

		sprite.flipX = direction.x < 0.0F;
		if(isGrounded)
			State = CharState.Run;
	}

	private void Jump(){
		rigidbody.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);

	}

	public override void RecieveDamage(){

		rigidbody.velocity = Vector3.zero;

		rigidbody.AddForce (transform.up * 8.0f, ForceMode2D.Impulse);
		rigidbody.AddForce (transform.right * 3.0f*(sprite.flipX ? 1.0F : -1.0F), ForceMode2D.Impulse);

		--Lives;
		if (Lives < 1) {
			Destroy (gameObject);
		}
		Debug.Log (lives);
	}

	private void Shoot(){
		Vector3 position = transform.position;
		position.y += 0.7F;
		position.x += 0.5F*(sprite.flipX ? -1.0F : 1.0F);
		Bullet newBullet = Instantiate (bullet, position, bullet.transform.rotation) as Bullet;
		newBullet.Parent = gameObject;
		newBullet.Direction = newBullet.transform.right * (sprite.flipX ? -1.0F : 1.0F);
	}

	private void CheckGround(){
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.3F);

		isGrounded = colliders.Length > 1;

		if(!isGrounded)
			State = CharState.Jump;
	}

	private void FixedUpdate(){
		CheckGround ();
	}

	private void OnTriggerEnter2D(Collider2D collider){
		Unit unit = collider.gameObject.GetComponent<Unit> ();
		if (unit)
			RecieveDamage ();
		
	}


}


public enum CharState{
	Idle,
	Run,
	Jump,
	Died
}