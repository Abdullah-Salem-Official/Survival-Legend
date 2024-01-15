using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 0.8f;
    public float moreSpeed = 1.7f;
    public float jumpForce = 2.5f;
    private bool isJumping = false;
    private bool isFalling = false;

    public float fireCooldown = .25f;
    public float rangeAttackCooldown = 2f;
    private bool isRangeAttack = false;
    private float lastFireTime;


    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public GameObject projectilePrefab;  // Prefab of the projectile object
    public float projectileSpeed = 5f;  // Speed of the projectile

    public GameObject wallPrefab;     // Prefab of the wall object
    public float throwForce = 2.5f;    // Force applied to the thrown wall

    public float dashDistance = 0.5f;     // Distance to dash forward

    private bool isDashing = false;       // Flag to track if the player is currently dashing

    public GameObject cooldownBarPrefab;  // Prefab of the cooldown bar UI element
    private GameObject cooldownBarInstance;  // Instance of the cooldown bar
    public Canvas canvas;

    public float dashDuration = 0.2f;  // Duration of the dash in seconds
    private float dashTimer = 0f;       // Timer for tracking the dash duration

    public Vector3 cooldownBarOffset = new Vector3(0f, 1f, 0f);  // Offset for the cooldown bar position

    public Slider energySlider;
    public float rechargeRate;

    public Slider healthSlider;
    private int maxHealth = 100;  // Maximum health of the player
    private int currentHealth;    // Current health of the player

    public GameObject smokeVFXPrefab; // Prefab of the smoke VFX
    public GameObject deathSmokePrefab;
    public float smokeYOffset = 0f;
    public float deathSmokeYOffset = 0f;

    public float invincibleTime = 1;
    private float invincibleTimeLeft = 0;

    public float blinkDuration = 0.2f;
    public Color hitColor = Color.red;
    private Color defaultColor = Color.white;

    public GameObject rangeAttackPrefab; // Prefab of the Range Attack
    public float rangeAttackXOffset = 0f;

    private bool isHealing = false;
    public GameObject healPrefab;
    public float healingTime = 1;
    private float healingTimeLeft;
    public float healingYOffset = 0f;
    private float healingTimer;
    private GameObject heal;
    public GameObject healingBarPrefab;  // Prefab of the healing bar UI element
    private GameObject healingBarInstance;  // Instance of the healing bar

    public int appleCount = 0;
    public TMP_Text appleCountText;  // Reference to the UI Text object for displaying the apple count

    public float xRangeLeft = 20f;
    public float xRangeRight = 22f;
    /*public float moveXRangeLeft = 22f;
    public float moveXRangeRight = 24f;*/

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        UpdateHealthUI();

        appleCountText.text = ": " + appleCount;
    }

    private void UpdateHealthUI()
    {
        healthSlider.value = currentHealth;
    }

    private void Update()
    {
        if(invincibleTimeLeft > 0)
        {
            invincibleTimeLeft -= Time.deltaTime;
        }
        if (healingTimeLeft > 0)
        {
            healingTimeLeft -= Time.deltaTime;
        }

        if (!isHealing)
        {
            // Move right
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector2.left * GetMoveSpeed() * Time.deltaTime);
                spriteRenderer.flipX = true; // Flip the sprite to face left
            }

            // Move left
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector2.right * GetMoveSpeed() * Time.deltaTime);
                spriteRenderer.flipX = false; // Do not flip the sprite, facing right
            }

            // Jump
            if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
            {
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
                animator.SetTrigger("Jump");
                isJumping = true;
            }

            if (rb.velocity.y < 0 && isJumping)
            {
                isFalling = true;
                animator.SetBool("Fall", isFalling);
            }

            

            // Rnage Attack
            if (Input.GetMouseButtonDown(1) && canAttack() && energySlider.value >= 25)
            {
                isRangeAttack = true;
                SpawnRangeAttack();
                lastFireTime = Time.time;
            }

            //wall
            /*if (Input.GetKeyDown(KeyCode.F))
            {
                ThrowWall();
            }*/
            if (Input.GetKeyDown(KeyCode.C) && !isDashing && energySlider.value >= 25)
            {
                if (spriteRenderer.flipX && transform.position.x > xRangeLeft)
                {
                    StartDash();
                }
                else if (!spriteRenderer.flipX && transform.position.x < xRangeRight)
                {
                    StartDash();
                }
                else if (!spriteRenderer.flipX && transform.position.x < xRangeLeft)
                {
                    StartDash();
                }
                else if (spriteRenderer.flipX && transform.position.x > xRangeRight)
                {
                    StartDash();
                }
            }
            // Check if the dash ability is not on cooldown and the player is not currently dashing
            

            if (Input.GetKeyDown(KeyCode.F) && !isHealing && !isDashing && canHeal())
            {
                Healing();
            }

            // Check for input to shoot the projectile upward
            if (Input.GetKey(KeyCode.W) && Input.GetMouseButtonDown(0) && canAttack())
            {
                Debug.Log("fire upward");

                lastFireTime = Time.time;
                // Spawn the projectile
                ShootProjectileUpward();
            }
            else // Shoot projectile
            if (Input.GetMouseButtonDown(0) && canAttack() && !isRangeAttack)
            {
                FireProjectile();
                lastFireTime = Time.time;
            }

            if (Input.GetKeyDown(KeyCode.V) && appleCount > 0)
            {
                AddEnergy(25);
                appleCount--;
                appleCountText.text = ": " + appleCount;
            }


            float horizontalInput = Input.GetAxis("Horizontal");
            animator.SetBool("Run", horizontalInput != 0);
            animator.SetBool("Grounded", !isJumping);
        }

        // Dash recharge
        /*if (energySlider.value < 99)
        {
            energySlider.value += Time.deltaTime * rechargeRate;
        }*/


    }
    private void FixedUpdate()
    {
        // Perform the dash movement if the player is currently dashing
        if (isDashing)
        {
            PerformDash();
        }

        if (isHealing)
        {
            TakeHeal(25);
        }
    }

    private float GetMoveSpeed()
    {
        // Check if Shift key is pressed to apply boosted speed
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            return moreSpeed;
        }

        return moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if player is touching the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            isFalling = false;
            animator.SetBool("Fall", isFalling);
        }

    }

    public bool canAttack()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        if (isRangeAttack)
        {
            if(Time.time >= lastFireTime + rangeAttackCooldown)
            {
                isRangeAttack = false;
            }
            return horizontalInput == 0 && !isJumping && Time.time >= lastFireTime + rangeAttackCooldown;
        }
        else
        {
            return horizontalInput == 0 && !isJumping && Time.time >= lastFireTime + fireCooldown;
        }
    }

    public bool canHeal()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        return horizontalInput == 0 && !isJumping && energySlider.value >= 50;
    }

    private void FireProjectile()
    {
        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // Change the sprite renderer x
        SpriteRenderer projectileSpriteRenderer = projectile.GetComponent<SpriteRenderer>();
        projectileSpriteRenderer.flipX = spriteRenderer.flipX ? false : true;

        // Get the direction to shoot based on the player's facing direction
        Vector2 shootDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;

        // Apply force to the projectile in the shoot direction
        Rigidbody2D projectileRigidbody = projectile.GetComponent<Rigidbody2D>();
        projectileRigidbody.velocity = shootDirection * projectileSpeed;
        projectileRigidbody.gravityScale = 0f;
    }

    private void ShootProjectileUpward()
    {
        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // Set the projectile's rotation to face upward
        projectile.transform.rotation = Quaternion.Euler(0f, 0f, -90f);

        // Apply force to the projectile in the shoot direction
        Rigidbody2D projectileRigidbody = projectile.GetComponent<Rigidbody2D>();
        projectileRigidbody.velocity = Vector2.up * projectileSpeed;
        projectileRigidbody.gravityScale = 0f;
    }

    private void ThrowWall()
    {
        GameObject wall = Instantiate(wallPrefab, transform.position, Quaternion.identity);
        Rigidbody2D wallRb = wall.GetComponent<Rigidbody2D>();

        Vector2 shootDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;

        // Apply force to throw the wall
        wallRb.AddForce(shootDirection * throwForce, ForceMode2D.Impulse);
    }
    private void StartDash()
    {
        invincibleTimeLeft = invincibleTime;
        //-energy value
        energySlider.value -= 25;

        // Instantiate the cooldown bar prefab as a child of the Canvas
        cooldownBarInstance = Instantiate(cooldownBarPrefab, canvas.transform);
        RectTransform cooldownBarRectTransform = cooldownBarInstance.GetComponent<RectTransform>();
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + cooldownBarOffset);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition, null, out Vector2 localPosition);
        cooldownBarRectTransform.anchoredPosition = localPosition;

        // Start the dash
        isDashing = true;
        dashTimer = 0f;

        // Spanw Smoke
        SpawnSmoke();

        // Move the player forward by dashDistance
        Vector2 Direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        transform.Translate(Direction * dashDistance);
    }

    private void PerformDash()
    {
        // Increment the dash timer
        dashTimer += Time.deltaTime;

        // Update the UI slider to represent the remaining dash duration
        if (cooldownBarInstance != null)
        {
            Slider slider = cooldownBarInstance.GetComponent<Slider>();
            slider.maxValue = dashDuration;
            slider.value = dashDuration - dashTimer;
        }

        // Check if the dash duration is complete
        if (dashTimer >= dashDuration)
        {
            // End the dash
            isDashing = false;

            // Reset the dash timer
            dashTimer = 0f;

            // Destroy the UI slider
            Destroy(cooldownBarInstance.gameObject);

            // Re-enable input or control
            // Add code here to re-enable player control/input if necessary
        }

        // Update the position of the cooldown bar instance
        if (cooldownBarInstance != null)
        {
            // Calculate the position of the cooldown bar relative to the player's position
            Vector3 cooldownBarPosition = transform.position + cooldownBarOffset;

            // Convert the world position to screen space
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(cooldownBarPosition);

            // Convert the screen position to UI canvas space
            Vector2 canvasPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition, null, out canvasPosition);

            // Set the cooldown bar's anchored position within the canvas
            RectTransform cooldownBarRectTransform = cooldownBarInstance.GetComponent<RectTransform>();
            cooldownBarRectTransform.anchoredPosition = canvasPosition;
        }
    }

    public void AddEnergy(float energyValue)
    {
        energySlider.value += energyValue;
    }

    public void TakeDamage(int damageAmount)
    {

        if (invincibleTimeLeft <= 0)
        {

            ShowHitEffect();

            currentHealth -= damageAmount;
            UpdateHealthUI();
        }

        // Check if the player's health reaches zero or below
        if (currentHealth <= 0)
        {

            if (cooldownBarInstance != null)
            {
                Destroy(cooldownBarInstance);
                isDashing = false;
            }
            SpawnDeathSmoke();

            Destroy(gameObject);
        }

        if (healingBarInstance != null)
        {
            Destroy(healingBarInstance);
            Destroy(heal);
            isHealing = false;
        }

    }

    private void SpawnSmoke()
    {
        // Instantiate the smoke
        Vector3 spawnPosition = transform.position + new Vector3(0f, smokeYOffset, 0f);
        GameObject smoke = Instantiate(smokeVFXPrefab, spawnPosition, Quaternion.identity);

        SpriteRenderer smokeSpriteRenderer = smoke.GetComponent<SpriteRenderer>();

        if (spriteRenderer.flipX)
        {
            smokeSpriteRenderer.flipX = true;
        }
        else if (!spriteRenderer.flipX)
        {
            smokeSpriteRenderer.flipX = false;
        }
    }

    public void ShowHitEffect()
    {
        StartCoroutine(BlinkCoroutine());
    }

    private System.Collections.IEnumerator BlinkCoroutine()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(blinkDuration);
        spriteRenderer.color = defaultColor;
    }

    private void SpawnRangeAttack()
    {
        //-energy value
        energySlider.value -= 25;

        // Instantiate the attack
        Vector3 spawnPosition = transform.position + new Vector3(rangeAttackXOffset, 0f, 0f);
        if (spriteRenderer.flipX)
        {
            spawnPosition = transform.position + new Vector3(-rangeAttackXOffset, 0f, 0f);
        }
        else if (!spriteRenderer.flipX)
        {
            spawnPosition = transform.position + new Vector3(rangeAttackXOffset, 0f, 0f);
        }
        
        GameObject rangeAttack = Instantiate(rangeAttackPrefab, spawnPosition, Quaternion.identity);

        SpriteRenderer rangeAttackSpriteRenderer = rangeAttack.GetComponent<SpriteRenderer>();

        if (spriteRenderer.flipX)
        {
            rangeAttackSpriteRenderer.flipX = true;
        }
        else if (!spriteRenderer.flipX)
        {
            rangeAttackSpriteRenderer.flipX = false;
        }
    }

    private void SpawnDeathSmoke()
    {
        // Instantiate the smoke
        Vector3 spawnPosition = transform.position + new Vector3(0f, deathSmokeYOffset, 0f);
        GameObject smoke = Instantiate(deathSmokePrefab, spawnPosition, Quaternion.identity);
    }

    private void Healing()
    {
        healingTimeLeft = healingTime;

        isHealing = true;

        healingTimer = 0f;

        energySlider.value -= 50;

        // Instantiate the effect
        Vector3 spawnPosition = transform.position + new Vector3(0f, healingYOffset, 0f);
        heal = Instantiate(healPrefab, spawnPosition, Quaternion.identity);

        // Instantiate the healing bar prefab as a child of the Canvas
        healingBarInstance = Instantiate(healingBarPrefab, canvas.transform);
        RectTransform healingBarRectTransform = healingBarInstance.GetComponent<RectTransform>();
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + cooldownBarOffset);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition, null, out Vector2 localPosition);
        healingBarRectTransform.anchoredPosition = localPosition;
    }

    private void TakeHeal(float healAmount)
    {
        healingTimer += Time.deltaTime;

        // Update the UI slider to represent the remaining heal duration
        if (healingBarInstance != null)
        {
            Slider slider = healingBarInstance.GetComponent<Slider>();
            slider.maxValue = healingTime;
            slider.value = healingTime - healingTimer;
        }

        if (healingTimeLeft <= 0)
        {
            healthSlider.value += healAmount;
            isHealing = false;

            // Reset the dash timer
            healingTimer = 0f;

            // Destroy the UI slider
            Destroy(healingBarInstance.gameObject);

            // Destroy healing animation
            Destroy(heal);
        }

        // Update the position of the cooldown bar instance
        if (healingBarInstance != null)
        {
            // Calculate the position of the healing bar relative to the player's position
            Vector3 healingBarPosition = transform.position + cooldownBarOffset;

            // Convert the world position to screen space
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(healingBarPosition);

            // Convert the screen position to UI canvas space
            Vector2 canvasPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition, null, out canvasPosition);

            // Set the cooldown bar's anchored position within the canvas
            RectTransform healingBarRectTransform = healingBarInstance.GetComponent<RectTransform>();
            healingBarRectTransform.anchoredPosition = canvasPosition;
        }
    }
}
