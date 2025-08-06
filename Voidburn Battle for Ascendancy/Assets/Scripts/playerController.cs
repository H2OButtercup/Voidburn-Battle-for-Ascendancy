using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] Transform orientation;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] Animator animator;
    [Header("General Stats")]
    [SerializeField] int HPOrig;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpVel;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;
    [SerializeField] int deathDepth; // Set the height that the player can fall to before dying
    [SerializeField] float mouseSensitivity = 3f;


    [Header("Damage")]


    [Header("Slam")]

    [SerializeField] public float staminaOrig;
    public float stamina;
    [SerializeField] float staminaRegenRate;
    [SerializeField] float sprintStaminaCost;
    bool isSprinting;
    bool canUseStamina;


    [Header("Audio Settings:")]
    [SerializeField] AudioSource playerSounds;
    [SerializeField] AudioClip[] playerSoundsClip;
    [SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] deathClip;
    [SerializeField] float deathVolume;
    [SerializeField] AudioClip[] playerHurtClip;
    [SerializeField] float hurtVol;
    [SerializeField] AudioClip[] playerReloadClip;
    [SerializeField] float reloadVol;
    [SerializeField] AudioClip[] audStep;
    [SerializeField] float audStepVol;
    [SerializeField] AudioClip[] audArena;
    [SerializeField] float audArenaVol;


    private PlayerControls controls;
    private Vector2 moveInput;
    private bool isJumpPressed;
    private bool isSprintHeld;
    private Vector3 playerVel;
    int jumpCount;
    int HP;
    int speedOrig;

    bool isPlayingStep;
    private Vector3 moveDir;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HP = HPOrig;
        canUseStamina = true;
        isSprinting = false;
        stamina = staminaOrig;
        stamina = staminaOrig;

        playerSounds.PlayOneShot(audArena[Random.Range(0, audArena.Length)], audArenaVol);

        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        // Regenerate stamina only if not doing stamina-draining actions
        if (!isSprinting && stamina < staminaOrig)
        {
            stamina += staminaRegenRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, staminaOrig);
            updatePlayerUI();
        }

        if (gameManager.instance != null && gameManager.instance.isPaused)
        {
            return;
        }

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward, Color.red);

        sprint();
        movement();

    }
    void OnEnable()
    {
        controls = new PlayerControls();
        controls.Player.Enable();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => isJumpPressed = true;
        controls.Player.Jump.canceled += ctx => isJumpPressed = false;

        controls.Player.Sprint.performed += ctx => isSprintHeld = true;
        controls.Player.Sprint.canceled += ctx => isSprintHeld = false;
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    void movement()
    {
        // Player instantly dies if he falls to a certain depth on the map
        if (gameObject.transform.position.y <= deathDepth)
        {
            gameManager.instance.youLose();
        }


        if (controller.isGrounded)
        {
            if (!isPlayingStep && moveDir.normalized.magnitude > 0.3f)
            {
                StartCoroutine(playStep());
            }

            playerVel = Vector3.zero;
            jumpCount = 0;

        }

        moveDir = (moveInput.x * transform.right) + (moveInput.y * transform.forward);
        controller.Move(moveDir * speed * Time.deltaTime);

        animator.SetFloat("Horizontal", moveInput.x, 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", moveInput.y, 0.1f, Time.deltaTime);


        jump();

        controller.Move(playerVel * Time.deltaTime);
    }
private void jump()
    {
        if (isJumpPressed && jumpCount < jumpMax)
        {
            playerVel += transform.up * jumpVel;
            jumpCount++;
            playerSounds.PlayOneShot(playerSoundsClip[Random.Range(0, playerSoundsClip.Length)], audJumpVol);
        }
    }
    private void sprint()
    {
        if (isSprintHeld && controller.isGrounded && moveDir.magnitude > 0.1f && stamina > 0)
        {
            if (!isSprinting)
            {
                isSprinting = true;
                speed *= sprintMod;
            }

            //reduce stamina by the cost and clamp so it doesn't go outside of the stamina range
            stamina -= sprintStaminaCost * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0f, staminaOrig);
            updatePlayerUI();

            if (stamina <= 0.1f)
            {
                stamina = 0;
                StopSprinting();
            }
        }
        else if (isSprintHeld)
        {
            StopSprinting();
        }
    }
    private IEnumerator playStep()
    {
        isPlayingStep = true;
        playerSounds.PlayOneShot(audStep[Random.Range(0, audStep.Length)], audStepVol);

        if (isSprinting)
            yield return new WaitForSeconds(0.3f);

        else
            yield return new WaitForSeconds(0.5f);

        isPlayingStep = false;
    }
    private void StopSprinting()
    {
        if (isSprinting)
        {
            speed = speedOrig;
            isSprinting = false;
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        updatePlayerUI();

        StartCoroutine(damageFlashScreen());

        playerSounds.PlayOneShot(playerHurtClip[Random.Range(0, playerHurtClip.Length)], hurtVol);

        if (HP <= 0)
        {
            //you dead!
            gameManager.instance.youLose();
            playerSounds.PlayOneShot(deathClip[Random.Range(0, deathClip.Length)], deathVolume);
        }
    }

    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
        gameManager.instance.playerStaminaBar.fillAmount = (float)stamina / staminaOrig;

    }

    IEnumerator damageFlashScreen()
    {
        gameManager.instance.playerDamagePanel.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamagePanel.SetActive(false);
    }

    public void jumpCountUp()
    {
        jumpMax += 1;
    }

    public void speedUp()
    {
        speed += 1;
        speedOrig = speed;
    }

}