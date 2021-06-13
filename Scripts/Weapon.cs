using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject weapon;
    public AudioClip _shootSound;    // this lets you drag in an audio file in the inspector
    public AudioClip _noBulletSound;
    public ParticleSystem shootParticle;
    public ParticleSystem bulletCapsuleEffect;

    public GameObject impactEffect;
    public GameObject enemyImpactEffect;

    private AudioSource audio;

    public float range = 100f;
    public int bulletsPerMag = 30;
    public int bulletsLeft;
    public int magsLeft;

    public Transform shootPoint;
    Animator weaponAnimator;

    public float fireRate = 0.6f;

    private bool Aim = false;
    float fireTimer;

    // Start is called before the first frame update
    void Start()
    {
        bulletsLeft = bulletsPerMag;

        weaponAnimator = weapon.GetComponent<Animator>();
        if (_shootSound == null || _noBulletSound == null)
        {
            Debug.Log("You haven't specified sounds through the inspector");
            this.enabled = false; //disables this script cause there is no sound loaded to play
        }

        audio = gameObject.AddComponent<AudioSource>(); //adds an AudioSource to the game object this script is attached to
        audio.playOnAwake = false;
        audio.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }

        if (Input.GetMouseButtonDown(1)) //Aim
        {
            Aim = !Aim;
            weaponAnimator.SetBool("Aimed", Aim);

        }

        if (fireTimer < fireRate)
            fireTimer += Time.deltaTime;
    }

    void FixedUpdate()
    {
        AnimatorStateInfo info = weaponAnimator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("Fire")) weaponAnimator.SetBool("Fire", false);
        if (info.IsName("Aimed_Fire")) weaponAnimator.SetBool("Fire", false);
    }

    private void Fire()
    {
        if (fireTimer < fireRate || bulletsLeft <= 0) {
            audio.clip = _noBulletSound; 
            audio.Play(); 
            return;
        } 
        audio.clip = _shootSound;
        Debug.Log("Fire");
        bulletsLeft = bulletsLeft - 1;
        shootParticle.Play();
        bulletCapsuleEffect.Play();
        audio.Play();
        RaycastHit hit;
        if(Physics.Raycast(shootPoint.position, shootPoint.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name + " found!");
            if (hit.transform.name == "Target")
            {
                hit.transform.GetComponent<Rigidbody>().AddForce(hit.transform.forward * 200f);
                Instantiate(enemyImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            } else
            {
                Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

        }
        weaponAnimator.SetBool("Fire", true);
        fireTimer = 0.0f;
    }


}
