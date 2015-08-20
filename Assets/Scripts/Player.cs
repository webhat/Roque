﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MovingObject
{
	public int wallDamage = 1;
	public int pointsPerFood = 10;
	public int pointsPerSoda = 20;
	public float restartLevelDelay = 1f;
	public Text foodText;
	private Animator animator;
	private int food;

	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;

	protected override void Start ()
	{
		animator = GetComponent<Animator> ();

		food = GameManager.instance.playerFoodPoints;

		foodText.text = "Food: " + food;
		base.Start ();
	}

	private void OnDisable ()
	{
		GameManager.instance.playerFoodPoints = food;
	}

	private void Update ()
	{
		if (!GameManager.instance.playersTurn)
			return;

		int horizontal = 0;
		int vertical = 0;

		horizontal = (int)Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");

		if (horizontal != 0)
			vertical = 0;

		if (horizontal != 0 || vertical != 0) {
			AttemptMove<Wall> (horizontal, vertical);
		}
	}

	protected override void AttemptMove <T> (int xDir, int yDir)
	{
		food--;
		foodText.text = "Food: " + food;
	
		base.AttemptMove<T> (xDir, yDir);

		RaycastHit2D hit;

		if(Move(xDir,yDir,out hit)) {
			SoundManager.instance.PlayRandowSfx(moveSound1,moveSound2);
		}

		CheckIfGameOver ();

		GameManager.instance.playersTurn = false;
	}

	private void OnTriggerEnter2D (Collider2D other)
	{
		if (other.CompareTag ("Exit")) {
			Invoke ("Restart", restartLevelDelay);
			enabled = false;
		} else if (other.CompareTag ("Food")) {
		    SoundManager.instance.PlayRandowSfx(eatSound1,eatSound2);
			food += pointsPerFood;
			foodText.text = "+" + pointsPerFood + " Food: " + food;
			other.gameObject.SetActive (false);
		} else if (other.CompareTag ("Soda")) {
			SoundManager.instance.PlayRandowSfx(drinkSound1,drinkSound2);
			food += pointsPerSoda;
			foodText.text = "+" + pointsPerSoda + " Food: " + food;
			other.gameObject.SetActive (false);
		}
	}

	protected override void OnCantMove<T> (T component)
	{
		Wall hitWall = component as Wall;
		hitWall.DamageWall (wallDamage);
		animator.SetTrigger ("PlayerChop");
	}

	private void Restart ()
	{
		Application.LoadLevel (Application.loadedLevel);
	}

	public void LoseFood (int loss)
	{
		animator.SetTrigger ("PlayerHit");
		food -= loss;
		foodText.text = "-" + loss + " Food: " + food;
		CheckIfGameOver ();
	}

	private void CheckIfGameOver ()
	{
		if (food <= 0) {
			SoundManager.instance.PlaySingle(gameOverSound);
			SoundManager.instance.musicSource.Stop();
			GameManager.instance.GameOver ();
		}
	}
}
