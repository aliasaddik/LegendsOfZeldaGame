// This Script is attached to the projectile

using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {

	public GameObject Explosion;
	public float Damage;
	public GameObject Owner;

	void OnCollisionEnter(Collision col) 
	{
		if (col.gameObject.tag != "Player")
		{
			//make the explosion
			Debug.Log(col.gameObject.name);
			GameObject ThisExplosion = Instantiate(Explosion, gameObject.transform.position, gameObject.transform.rotation) as GameObject;

			//destory the projectile
			Destroy(gameObject);
		}
	}	
}
