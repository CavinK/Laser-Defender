using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // configuration parameters
    [Header("Player Movement")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float padding = 1f;
    [SerializeField] int health = 200;
    [SerializeField] AudioClip explodeSound;
    [SerializeField] [Range(0, 1)] float explodeSoundVolume = .7f;
    [SerializeField] AudioClip shootSound;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = .2f;

    [Header("Projectile")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileFiringPeriod = .1f;

    Coroutine firingCoroutine;

    float xMin;
    float xMax;
    float yMin;
    float yMax;

    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
        //StartCoroutine(PrintAndWait());
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
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
        if (health <= 0)
        {
            Over();
        }
    }

    private void Over()
    {
        FindObjectOfType<Level>().LoadGameOver(); // make the event to make game over
        Destroy(gameObject);
        AudioSource.PlayClipAtPoint(explodeSound, Camera.main.transform.position, explodeSoundVolume);
    }

    public int GetHealth()
    {
        return health;
    }

    //IEnumerator PrintAndWait() // method for co-routine
    //{
    //    Debug.Log("First message sent, boss");
    //    yield return new WaitForSeconds(3);
    //    Debug.Log("The second message, yo!");
    //}

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1")) // refered from "Edit>Project Setting" on Unity
        {
            //GameObject laser = Instantiate(
            //    laserPrefab, 
            //    transform.position, 
            //    Quaternion.identity) as GameObject;
            //laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);

            firingCoroutine = StartCoroutine(FireContinuously());
        }
        if (Input.GetButtonUp("Fire1")) // set condition to stop all co-routines <- make laser shot only when pushing the key!
        {
            //StopAllCoroutines();
            StopCoroutine(firingCoroutine);
        }
    }

    IEnumerator FireContinuously() // improve co-routine to make pushing key easier(holding down the push button)
    {
        while (true) // keep being true to continue shooting laser after pushing the key once
        {
            GameObject laser = Instantiate(
                laserPrefab,
                transform.position,
                Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
            AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootSoundVolume);
            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }

    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed; // change in position <- Horizontal: check "Edit>Project Setting" on Unity // Time.deltaTime: reflects change in frame rate
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        //Debug.Log(deltaX);

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax); // var: C# knows which type(float) comes from the method, and automatically converts it into the right type
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax); // Clamp: to make boundary on the game screen

        transform.position = new Vector2(newXPos, newYPos);
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding; // based on the size of camera // padding: prevent player object from being overlapped by boundary
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;
    }
}
