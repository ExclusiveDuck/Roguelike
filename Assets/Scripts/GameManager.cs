using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
//using UnityEngine.UIElements;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Transform> Spawners = new List<Transform>();
    public List<GameObject> Enemies = new List<GameObject>();
    [SerializeField] TextMeshProUGUI soulsHUD;
    [SerializeField] TextMeshProUGUI fragmentSHUD;
    [SerializeField] TextMeshProUGUI potionHUD;
    [SerializeField] PlayerStatsSO pStatsSO;
    public SingleValuesSO soulsCount;
    public SingleValuesSO fragmentsCount;
    GunSystem gunsystem;

    [SerializeField] SingleValuesSO roomsEntered;
    [SerializeField] WeaponSO weaponSO;
    public int randomSpawner;
    public int randomEnemy;
    public float numberOfEnemiesLeft;
    public float spawnedEnemies;
    public int randomScene;
    private int potionCounter;
    public bool waveInProgress;


    public List<GameObject> rooms = new List<GameObject>();
    public List<Transform> roomSpawnPoints = new List<Transform>();
    public float amountOfRooms;
    public GameObject shopSelectionRoom;
    bool doorsActive = false;
    [SerializeField] TextMeshProUGUI enemiesLeftHUD;
    public GameObject openChestHUD;

    public GameObject pauseScreen;
    public GameObject deathScreen;
    PlayerMovement pMovement;

    public GameObject shopRoom;
    public GameObject waveRoom;
    public GameObject manual;
    public bool wavesCompleted = false;


    AudioManager audioManager;
    private void Start()
    {
        gunsystem = FindObjectOfType<GunSystem>();
        audioManager = FindObjectOfType<AudioManager>();
        pMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        Physics.IgnoreLayerCollision(8, 10);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            Pause();
        }



        potionCounter = pStatsSO.PotionCounter;

        soulsHUD.text = "" + soulsCount.Value;
        fragmentSHUD.text = "" + fragmentsCount.Value;
        potionHUD.text = "" + potionCounter;
        enemiesLeftHUD.text = "Enemies Left: " + numberOfEnemiesLeft;

        if (numberOfEnemiesLeft <= 0 && !waveInProgress && !doorsActive && wavesCompleted)
        {
            doorsActive = true;
            InstantiateRooms();

        }
    }

    private void InstantiateRooms()
    {
        randomScene = Random.Range(1, 4);
        if (roomsEntered.Value <= 1 || roomsEntered.Value > 2 && roomsEntered.Value <= 3)
        {
            randomScene = Random.Range(1, 4);
            int randomRoom = Random.Range(0, rooms.Count);
            audioManager.Play("PortalSpawn");
            for (int i = 0; i < roomSpawnPoints.Count + 1; i++)
            {
                Instantiate(rooms[randomRoom], roomSpawnPoints[Random.Range(0, roomSpawnPoints.Count)].position, Quaternion.Euler(0, 90, 0));
                roomSpawnPoints.RemoveAt(randomRoom);
                
            }


            //Instantiate(rooms[Random.Range(0, rooms.Count)], roomSpawnPoints[Random.Range(0, roomSpawnPoints.Count)].position, Quaternion.identity);
        }

        if (roomsEntered.Value == 2)
        {
            audioManager.Play("PortalSpawn");
            Instantiate(shopRoom, roomSpawnPoints[1].position, Quaternion.Euler(0, 90, 0));
        }

        if (roomsEntered.Value == 4)
        {
            Instantiate(waveRoom, roomSpawnPoints[1].position, Quaternion.Euler(0, 90, 0));
            audioManager.Play("PortalSpawn");
        }
        Debug.Log(randomScene);
    }

    private void Pause()
    {
        pauseScreen.SetActive(true);
        pMovement.canMove = false;

        Time.timeScale = 0;
    }

    public void Resume()
    {
        pauseScreen.SetActive(false);
        Time.timeScale = 1;
        pMovement.canMove = true;
    }

    public void Restart()
    {
        ResetStats();
        SceneManager.LoadScene("Start");
        Time.timeScale = 1f;
    }   

    public void Manual()
    {
        manual.SetActive(true);
    }

    public void UnselectManual()
    {
        manual.SetActive(false);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void DeathScreen()
    {
        deathScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResetStats()
    {
        pStatsSO.PlayerMaxHealth = 100;
        pStatsSO.PlayerHealth = 100;
        HealthBar hbar = GameObject.Find("HealthBar").GetComponent<HealthBar>();
        hbar.healthText.text = pStatsSO.PlayerHealth + "/" + pStatsSO.PlayerMaxHealth;
        pStatsSO.DashBomb = false;
        pStatsSO.TeleportDash = false;
        pStatsSO.InvisibleAbility = false;
        gunsystem.canMelee = true;
        pStatsSO.DashCooldwon = 0.96f;
        pStatsSO.PotionCounter = 0;

        weaponSO.ProjectileDamage = 16f;
        weaponSO.ProjectileFireRate = 0.2f;
        weaponSO.ProjectileSpeed = 35f;
        weaponSO.ProjectileLifetime = 0.8f;
        weaponSO.ExplodingBullets = false;
        weaponSO.ExplodingDamage = 9f;
        weaponSO.BeamCooldown = 0.7f;
        weaponSO.BeamCooldown = 4;
        weaponSO.BeamDamage = 90;
        weaponSO.OrbDamage = 22;
        weaponSO.OrbRadius = 5;
        weaponSO.OrbLifeTime = 3.5f;
        weaponSO.OrbSlowStrength = 15f;
        weaponSO.OrbCooldown = 5;
        weaponSO.OrbSpeed = 6f;

        roomsEntered.Value = 0f;
        soulsCount.Value = 0f;
        fragmentsCount.Value = 0f;
    }

}
