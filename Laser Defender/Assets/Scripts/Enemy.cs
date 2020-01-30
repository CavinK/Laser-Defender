using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float health = 100;
    [SerializeField] float shotCounter;
    [SerializeField] float minTimeBetweenShots = .2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] GameObject explodeVFX;
    [SerializeField] float durationOfExplosion = 1f;
    [SerializeField] AudioClip explodeSound;
    [SerializeField] [Range(0,1)] float explodeSoundVolume = .7f; // range: cap the volume between 0 and 1(make "slide" on Inspector!)
    [SerializeField] AudioClip shootSound;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = .2f;

    // Start is called before the first frame update
    void Start()
    {
        shotCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            Fire();
            shotCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots); // shot counter should be reset in order to make new random shooting rate
        }
    }

    private void Fire()
    {
        GameObject laser = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -projectileSpeed); // why negative projectile speed? because it goes down while player's laser goes up
        AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootSoundVolume);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health < 0)
        {
            Over();
        }
    }

    private void Over()
    {
        // Visual Effects
        Destroy(gameObject);
        GameObject explosion = Instantiate(explodeVFX, transform.position, transform.rotation);
        Destroy(explosion, durationOfExplosion);

        // Sound Effects
        AudioSource.PlayClipAtPoint(explodeSound, Camera.main.transform.position, explodeSoundVolume); // let it sound right in front of camera
    }
}
