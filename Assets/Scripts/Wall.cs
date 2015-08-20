using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour
{
	public AudioClip chopSound1;
	public AudioClip chopSound2; 
	public Sprite damageSprite;
	public int hp = 4;
	private  SpriteRenderer spriteRenderer;
	
	void Awake ()
	{
		spriteRenderer = GetComponent<SpriteRenderer> ();	
	}

	public void DamageWall (int loss)
	{
		SoundManager.instance.PlayRandowSfx (chopSound1, chopSound2);
		spriteRenderer.sprite = damageSprite;
		hp -= loss;
		if (hp <= 0)
			gameObject.SetActive (false);
	}
}
