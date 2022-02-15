using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float playerSpeed;
    public float runSpeed;
    private Rigidbody rb;
    public float playerJumpValue;
    private bool isGrounded;
    private CapsuleCollider capsulecollider;
    public GameObject cam;
    private Quaternion camRotation;
    private Quaternion playerRotation;
    public float mouseSens;
    public float minX = -90.0f;
    public float maxX = 90.0f;
    public Animator anim;
    public AudioSource MedBoxAudio;
    public AudioSource AmmoBoxAudio;
    public AudioSource playerDeathAudio;
    public Transform gunShootPos;

    //Inventory Section
    private int ammo = 0;
    private int reserveAmmo = 0;
    private int maxAmmo = 15;
    private int health = 0;
    private int maxHealth = 15;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsulecollider = GetComponent<CapsuleCollider>();
    }
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        ammo = maxAmmo;
        camRotation = cam.transform.localRotation;
        playerRotation = transform.localRotation;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            anim.SetBool("Aiming", !anim.GetBool("Aiming"));
        }
        if (Input.GetMouseButtonDown(0) && !anim.GetBool("Firing"))
        {
            if (ammo > 0)
            {
                ammo = Mathf.Clamp(ammo - 1, 0, maxAmmo);
                //ammo--;
                Debug.Log("ammo left : " + ammo);
                //anim.SetBool("Firing", true);
                anim.SetTrigger("Firing");

                ZombieHit();


            }

        }
        //if (Input.GetMouseButtonUp(0))
        //{
        //    //anim.SetBool("Firing", false);
        //}
        if (Input.GetKeyDown(KeyCode.R))
        {
            //anim.SetTrigger("Reload");
            int ammoNeeded = maxAmmo - ammo;
            print("ammo needed : " + ammoNeeded);
            print("Reserve ammo : " + reserveAmmo);
            //if(ammoNeeded<ammo)
            //{
            //    ammo = ammo + ammoNeeded;
            //    print("current ammo : "+ammo);
            //}
            //else if(ammoNeeded>ammo)
            //{
            //    ammo = ammo + ammoNeeded;
            //    print("current ammo : " + ammo);
            //}
            if (ammoNeeded < 5 && ammoNeeded > 0)
            {
                if (reserveAmmo > 0 && reserveAmmo >= ammoNeeded)
                {
                    anim.SetTrigger("Reload");
                    reserveAmmo = reserveAmmo - ammoNeeded;
                    ammo = ammo + ammoNeeded;
                }
                else if (reserveAmmo > 0 && reserveAmmo < ammoNeeded)
                {
                    anim.SetTrigger("Reload");
                    ammo = ammo + reserveAmmo;
                    reserveAmmo = reserveAmmo - reserveAmmo;
                }

            }
            else if (ammoNeeded >= 5)
            {
                anim.SetTrigger("Reload");
                reserveAmmo = reserveAmmo - 5;
                ammo = ammo + 5;
            }
        }

    }

    private void ZombieHit()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(gunShootPos.position, gunShootPos.forward, out hitInfo, 100f))
        {
            GameObject tempZombieHit = hitInfo.collider.gameObject;
            if (tempZombieHit.tag == "Zombie")
            {
                GameObject tempRagDollZombiePrefab = tempZombieHit.GetComponent<ZombieController>().zombieRagdollPrefab;
                GameObject tempNewRagdollPrefab = Instantiate(tempRagDollZombiePrefab, tempZombieHit.transform.position, tempZombieHit.transform.rotation);
                tempNewRagdollPrefab.transform.Find("Hips").GetComponent<Rigidbody>().AddForce(gunShootPos.forward * 1000);
                Destroy(tempZombieHit);
            }
            else
            {
                tempZombieHit.GetComponent<ZombieController>().ZombieKill();
            }
        }
    }




    // Update is called once per frame
    void FixedUpdate()
    {
        PlayerMovement();
        PlayerJumpMovement();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            PlayerRunMovement();
        }
        float mousex = Input.GetAxis("Mouse X") * mouseSens;
        float mousey = Input.GetAxis("Mouse Y") * mouseSens;
        camRotation = camRotation * Quaternion.Euler(-mousey, 0, 0);
        playerRotation = playerRotation * Quaternion.Euler(0, mousex, 0);
        transform.localRotation = playerRotation;
        cam.transform.localRotation = camRotation;
        camRotation = ClampRotationOnXaxis(camRotation);
    }



    Quaternion ClampRotationOnXaxis(Quaternion value)
    {
        value.x /= value.w;
        value.y /= value.w;
        value.z /= value.w;
        value.w = 1.0f;
        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(value.x);
        angleX = Mathf.Clamp(angleX, minX, maxX);
        value.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);
        return value;

    }

    bool PlayerGrounded()
    {
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, capsulecollider.radius, Vector3.down, out hitInfo, (capsulecollider.height / 2 - capsulecollider.radius + 0.1f)))
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    void PlayerMovement()
    {
        float horizontalMovement = Input.GetAxis("Horizontal") * playerSpeed;
        float forwardMovement = Input.GetAxis("Vertical") * playerSpeed;
        // transform.position += new Vector3(horizontalMovement, 0, forwardMovement);
        transform.position += cam.transform.forward * forwardMovement + cam.transform.right * horizontalMovement;
        if (horizontalMovement != 0 || forwardMovement != 0)
        {
            anim.SetBool("walkrifle", true);
            if (Input.GetMouseButtonDown(0))
            {
                anim.SetTrigger("walkfire");
            }
        }
        else
        {
            anim.SetBool("walkrifle", false);
        }

    }
    void PlayerRunMovement()
    {
        float horizontalMovement = Input.GetAxis("Horizontal") * runSpeed;
        float forwardMovement = Input.GetAxis("Vertical") * runSpeed;
        // transform.position += new Vector3(horizontalMovement, 0, forwardMovement);
        transform.position += cam.transform.forward * forwardMovement + cam.transform.right * horizontalMovement;
        if (horizontalMovement != 0 || forwardMovement != 0)
        {
            anim.SetBool("runrifle", true);
            if (Input.GetMouseButtonDown(0))
            {
                anim.SetTrigger("runfire");
            }
        }
        else
        {
            anim.SetBool("runrifle", false);
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ammo" && reserveAmmo < maxAmmo)
        {
            print("Ammo Collected");
            //ammoPickUp += 5;
            //ammo = Mathf.Clamp(ammo + 5, 0, maxAmmo);
            reserveAmmo = Mathf.Clamp(reserveAmmo + 5, 0, maxAmmo);
            AmmoBoxAudio.Play();
            Destroy(collision.gameObject);
            Debug.Log("reserveAmmo = " + reserveAmmo);
        }
        else if (collision.gameObject.tag == "MedBox" && health < maxHealth)
        {
            print("MedBox Collected");
            health = Mathf.Clamp(health + 5, 0, maxHealth);
            MedBoxAudio.Play();
            Destroy(collision.gameObject);
            Debug.Log("Health : " + health);
        }
        else if (collision.gameObject.tag == "Lava")
        {
            //health = health - 5;
            //player death when health reaches zero we need to apply.
            //player death sound needs to be applied
            health = Mathf.Clamp(health - 5, 0, maxHealth);
            if (health == 0)
            {
                playerDeathAudio.Play();
            }
            print("Health : " + health);
        }
    }

    void PlayerJumpMovement()
    {
        if (Input.GetKeyDown(KeyCode.Space) && PlayerGrounded())
        {
            rb.AddForce(0, playerJumpValue, 0);
        }

    }

}