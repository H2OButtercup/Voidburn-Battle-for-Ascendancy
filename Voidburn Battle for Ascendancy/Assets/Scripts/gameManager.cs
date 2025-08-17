using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    [Header("Menus")]
    [SerializeField] public GameObject menuActive;
    [SerializeField] public GameObject menuPause;
    [SerializeField] public GameObject menuLevelComplete;
    [SerializeField] public GameObject menuWin;
    [SerializeField] public GameObject menuLose;
    [SerializeField] public GameObject menuUnlocks;
    [SerializeField] public GameObject menuTutorial;
    [SerializeField] public GameObject menuMoveList;
    [SerializeField] public GameObject menuSettings;

    public List<GameObject> menuScenes;

    [Header("Settings' Sliders/Tracker")]
    [SerializeField] SettingsTracker tracker;
    [SerializeField] Slider mainVolume;
    [SerializeField] Slider musicVolume;
    [SerializeField] Slider effectVolume;
    [SerializeField] Slider menuVolume;

    [Header("Player Display")]
    public Image playerOneHp;
    public Image playerTwoHp;
    public Image playerOneRoundOne;
    public Image playerTwoRoundOne;
    public Image playerOneRoundTwo;
    public Image playerTwoRoundTwo;

    
    

    public bool isPaused;
    public GameObject player;
    public playerController playerScript;


    float timeScaleOrig;
    float timeScaleNew;


    [Header("Player1")]
    public GameObject playerPrefab;
    Transform playerSpawnPoint;

    [Header("Key")]
    public Transform keySpawnPoint;
    public GameObject keyPrefab;

    [Header("Player2")]
    public GameObject playerPrefab2;
    Transform playerSpawnPoint2;

    private int gameGoalCount;//rounds won per watch


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;

        // Dynamically find the player spawn point
        GameObject spawnPointObj = GameObject.FindWithTag("PlayerSpawn");
        if (spawnPointObj != null)
        {
            playerSpawnPoint = spawnPointObj.transform;
        }

       

        // Find the player at the correct spawn
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            if (playerPrefab != null && playerSpawnPoint != null)
            {
                player = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
                //player.tag = "Player";
            }
        }

        // Grab the player script and enable movement
        if (player != null)
        {
            playerScript = player.GetComponent<playerController>();
            playerScript.enabled = true;
            timeScaleOrig = Time.timeScale;
        }

    }

    private void Start()
    {
        Time.timeScale = 1f;
        //set the min and max volume for the setting sliders
        mainVolume.minValue = 0;
        mainVolume.maxValue = 100;
        musicVolume.minValue = 0;
        musicVolume.maxValue = 100;
        effectVolume.minValue = 0;
        effectVolume.maxValue = 100;
        menuVolume.minValue = 0;
        menuVolume.maxValue = 100;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && menuScenes.Count == 0)
        {
            if (menuActive == null)
            {
                statePause(); 
                menuScenes.Add(menuPause);
                menuActive = menuScenes.Last();
               
                menuActive.SetActive(true);
                
            }
            else if (menuActive == menuPause || menuActive)
            {
                menuScenes.Clear();
                stateUnpause();
            }
        }

    }


    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        timeScaleNew = Time.timeScale;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
       
    }


    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void levelComplete()
    {
        statePause();
        menuActive = menuLevelComplete;
        menuActive.SetActive(true);
    }

    public void applySettings()
    {
        tracker.MasterVol = mainVolume.value;
        tracker.MusicVol  = musicVolume.value;
        tracker.EffectVol = effectVolume.value;
        tracker.MenuVol   = menuVolume.value;
    }


}