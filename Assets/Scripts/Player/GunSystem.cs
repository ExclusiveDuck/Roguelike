using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;

public class GunSystem : MonoBehaviour
{
    [Header("References")]
    PlayerMovement playerMovement;
    PlayerStats pStats;
    public WeaponSO weaponSO;
    HealthBar healthBar;
    Animator anim;
    //public GameObject gun;
    public Transform shootPos;
    public GameObject normalProjectile;
    public GameObject explodingProjectile;
    //public GameObject sniperBullet;
    public bool canShoot = true;

    [Header("Primary Stats")]
    public bool canShootPrimary;
    //public ParticleSystem muzzle;
    //public float currentAmmo;


    [Header("Beam Stats")]
    public GameObject lazerBeam;
    public float beamLifespan;
    //public float secondarySpeed;
    public bool canShootBeam;
    //public float beamCooldown;
    //private bool beamActive;

    [Header("Orb Stats")]
    public GameObject orb;
    public Transform orbSpawnPoint;
    public float orbSpeed;
    //public float orbCooldown;
    public bool canShootOrb;

    [Header("Beam Cooldown")]
    private Image beamImage;
    public bool beamActive = false;

    [Header("Orb Cooldown")]
    private Image orbImage;
    public bool orbActive = false;
    AudioManager audioManager;
    //public List<Transform> gunBarrels = new List<Transform>();


    public int barMana;

    public float normalProjMana;
    public float explodProjMana;
    public bool currentlyShooting = false;
    public bool regenMana = false;

    public BoxCollider meleeCollider;
    public float meleeDamageBase;
    public float meleeDamageCurrent;
    public ParticleSystem meleeParticle;
    public float meleeDistance;
    public bool canMelee = true;
    public float meleeTime;

    public float meleeCombatTimer;
    public float meleeCombatTotalTime;
    public int meleeCombatStreak;




    private void Start()
    {
        pStats = GetComponent<PlayerStats>();
        healthBar = FindObjectOfType<HealthBar>();
        audioManager = FindObjectOfType<AudioManager>();
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        canShootPrimary = true;
        canShootBeam = true;
        canShootOrb = true;
        orbImage = GameObject.Find("orbCD").GetComponent<Image>();
        beamImage = GameObject.Find("beamCD").GetComponent<Image>();
        orbImage.fillAmount = 0;
        beamImage.fillAmount = 0;
        meleeCollider.enabled = false;

        weaponSO.CurrentMana = weaponSO.MaxMana;
        barMana = Mathf.RoundToInt(weaponSO.CurrentMana);

        //currentAmmo = weaponSO.AmmoCapacity;
    }




    private void Update()
    {
        Shoot();
        barMana = Mathf.RoundToInt(weaponSO.CurrentMana);

        meleeCombatStreak = Mathf.Clamp(meleeCombatStreak, 0, 3);
        if (Input.GetKeyDown(KeyCode.Mouse1) && canMelee)
        {
            meleeCombatTimer = meleeCombatTotalTime;
            meleeCombatStreak++;
            StartCoroutine("MeleeAttack");
            if (meleeCombatStreak == 2)
            {
                Debug.Log("Streak");
                meleeDamageCurrent += 10;
            }
            if (meleeCombatStreak == 3)
            {
                Debug.Log("COMBOOOO");
                meleeDamageCurrent += 25;
                canMelee = false;
                Invoke("ResetCombatCombo", 1.5f);
            }
        }

        if (meleeCombatTimer > 0)
        {
            meleeCombatTimer -= Time.deltaTime;

        }

        if (meleeCombatTimer <= 0)
        {
            meleeCombatTimer = 0;
            meleeDamageCurrent = meleeDamageBase;
            meleeCombatStreak = 0;
        }

        if (!regenMana && weaponSO.CurrentMana < weaponSO.MaxMana)
        {
            Invoke("ManaRegen", 1.5f);
        }

        if (regenMana)
        {
            weaponSO.CurrentMana+= Time.deltaTime * 15;
            //weaponSO.CurrentMana = Mathf.RoundToInt(weaponSO.CurrentMana);
            
            healthBar.SetMana(barMana);
        }


        if (weaponSO.CurrentMana >= weaponSO.MaxMana)
        {
            weaponSO.CurrentMana = weaponSO.MaxMana;
        }

        if (!canShootOrb)
        {
            orbImage.fillAmount -= 1 / weaponSO.OrbCooldown * Time.deltaTime;

            if (orbImage.fillAmount <= 0)
            {
                orbImage.fillAmount = 0;
                canShootOrb = true;
            }
        }
        if (!canShootBeam)
        {
            beamImage.fillAmount -= 1 / weaponSO.BeamCooldown * Time.deltaTime;

            if (beamImage.fillAmount <= 0)
            {
                beamImage.fillAmount = 0;
                canShootBeam = true;
            }
        }
    }

    private IEnumerator MeleeAttack()
    {
        float startTime = Time.time;

        while (Time.time < startTime + meleeTime)
        {
            canMelee = false;
            canShoot = false;
            meleeCollider.enabled = true;
            playerMovement.canMove = false;
            playerMovement.canDash = false;
            meleeParticle.Play();
            playerMovement.controller.Move(gameObject.transform.forward * meleeDistance * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(meleeTime);

        canMelee = true;
        canShoot = true;
        meleeCollider.enabled = false;
        playerMovement.canMove = true;
        playerMovement.canDash = true;

    }

    public void ResetCombatCombo()
    {
        meleeCombatTimer = 0;
        canMelee = true;
    }
    public void ManaRegen()
    {
        regenMana = true;
    }

    private void Shoot()
    {
        if (canShoot)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && canShootPrimary)
            {
                canShootPrimary = false;


                Invoke("ResetPrimary", weaponSO.ProjectileFireRate);

                if (weaponSO.ExplodingBullets && weaponSO.CurrentMana > explodProjMana)
                {
                    //currentlyShooting = true;
                    regenMana = false;
                    var explodingBullets = Instantiate(explodingProjectile, shootPos.position, shootPos.rotation);
                    explodingBullets.GetComponent<Rigidbody>().velocity = shootPos.forward * weaponSO.ProjectileSpeed;
                    audioManager.Play("Shoot");
                    weaponSO.CurrentMana -= explodProjMana;
                    healthBar.SetMana(barMana);
                }
                else
                {
                    if (weaponSO.CurrentMana > normalProjMana)
                    {
                        //currentlyShooting = true;
                        regenMana = false;
                        var currentBullet = Instantiate(normalProjectile, shootPos.position, shootPos.rotation);
                        currentBullet.GetComponent<Rigidbody>().velocity = shootPos.forward * weaponSO.ProjectileSpeed;
                        //muzzle.Play();
                        audioManager.Play("Shoot");
                        weaponSO.CurrentMana -= normalProjMana;
                        healthBar.SetMana(barMana);
                    }

                }
            }
        }


        if (Input.GetKeyDown(KeyCode.E) && canShootBeam)
        {
            LazerBeamAbility();
            audioManager.Play("Shoot");
        }

        if (Input.GetKeyDown(KeyCode.R) && canShootOrb)
        {
            OrbAbility();
            audioManager.Play("Shoot");
        }
    }

    private void LazerBeamAbility()
    {
        canShootBeam = false;
        playerMovement.canMove = false;

        beamImage.fillAmount = 1;
        lazerBeam.SetActive(true);
        StartCoroutine(LazerBeam());
        Invoke("BeamCooldown", weaponSO.BeamCooldown);
        beamActive = true;
        CameraShake.Instance.ShakeCamera(1.5f, 0.35f);
        audioManager.Play("Beam");
    }

    private void OrbAbility()
    {
        var currentOrb = Instantiate(orb, orbSpawnPoint.position, Quaternion.identity);
        currentOrb.GetComponent<Rigidbody>().velocity = transform.forward * weaponSO.OrbSpeed;

        orbActive = true;
        orbImage.fillAmount = 1;

        canShootOrb = false;
        Invoke("OrbCooldown", weaponSO.OrbCooldown);
    }

    IEnumerator LazerBeam()
    {
        yield return new WaitForSeconds(beamLifespan);
        lazerBeam.SetActive(false);
        playerMovement.canMove = true;
    }


    private void ResetPrimary()
    {
        canShootPrimary = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.GetComponent<EnemyStats>().TakeDamage(meleeDamageCurrent);
            CameraShake.Instance.ShakeCamera(2.5f, 0.2f);
        }
    }

}
