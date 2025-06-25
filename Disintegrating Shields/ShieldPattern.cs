using System.Collections;
using UnityEngine;

public class ShieldPattern : MonoBehaviour
{
    [Header("Player Assignment")]
    public Transform playerTransform; // Assign player manually atau otomatis detect
    
    [Header("Shield Settings")]
    public GameObject shieldPrefab; // Prefab kubah shield
    public float maxDurability = 100f;
    public float durabilityDecayRate = 10f; // Durability berkurang per detik
    public float regenRate = 5f; // Regenerasi durability per detik saat tidak aktif
    public float regenDelay = 2f; // Delay sebelum mulai regenerasi
    
    [Header("Visual Effects")]
    public Color shieldColor = Color.cyan;
    public float shieldAlpha = 0.3f;
    
    [Header("Audio")]
    public AudioClip shieldActivateSound;
    public AudioClip shieldDeactivateSound;
    public AudioClip shieldHitSound;
    
    private GameObject currentShield;
    private float currentDurability;
    private bool isShieldActive = false;
    private Coroutine regenCoroutine;
    private AudioSource audioSource;
    private Renderer shieldRenderer;
    private Material shieldMaterial;
    
    void Start()
    {
        currentDurability = maxDurability;
        audioSource = GetComponent<AudioSource>();
        
        // Jika tidak ada AudioSource, tambahkan
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Auto-detect player jika belum di-assign
        if (playerTransform == null)
        {
            AutoDetectPlayer();
        }
        
        // Buat shield prefab jika belum ada
        if (shieldPrefab == null)
        {
            CreateDefaultShieldPrefab();
        }
        
        Debug.Log("ShieldPattern initialized for player: " + (playerTransform != null ? playerTransform.name : "NONE"));
    }
    
    void Update()
    {
        HandleShieldInput();
        
        if (isShieldActive)
        {
            DrainDurability();
            UpdateShieldVisual();
        }
    }
    
    void HandleShieldInput()
    {
        bool altPressed = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        
        if (altPressed && !isShieldActive && currentDurability > 0)
        {
            ActivateShield();
        }
        else if (!altPressed && isShieldActive)
        {
            DeactivateShield();
        }
    }
    
    void ActivateShield()
    {
        if (currentDurability <= 0) return;
        if (playerTransform == null)
        {
            Debug.LogWarning("Player not assigned! Cannot activate shield.");
            return;
        }
        
        isShieldActive = true;
        
        // Stop regeneration
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }
        
        // Create shield visual
        if (currentShield == null)
        {
            currentShield = Instantiate(shieldPrefab, playerTransform.position, Quaternion.identity);
            currentShield.transform.SetParent(playerTransform);
            currentShield.transform.localPosition = Vector3.zero; // Pastikan posisi tepat di player
            
            // Setup renderer and material
            shieldRenderer = currentShield.GetComponent<Renderer>();
            if (shieldRenderer != null)
            {
                shieldMaterial = new Material(shieldRenderer.material); // Buat material baru
                shieldRenderer.material = shieldMaterial;
                UpdateShieldColor();
            }
        }
        
        currentShield.SetActive(true);
        
        // Debug untuk memastikan shield ada dan terlihat
        Debug.Log("Shield Activated! Position: " + currentShield.transform.position + 
                  ", Scale: " + currentShield.transform.localScale + 
                  ", Active: " + currentShield.activeInHierarchy);
        
        // Play activate sound
        PlaySound(shieldActivateSound);
        
        Debug.Log("Shield Activated! Durability: " + currentDurability);
    }
    
    void DeactivateShield()
    {
        if (!isShieldActive) return;
        
        isShieldActive = false;
        
        if (currentShield != null)
        {
            currentShield.SetActive(false);
        }
        
        // Play deactivate sound
        PlaySound(shieldDeactivateSound);
        
        // Start regeneration after delay
        if (regenCoroutine == null)
        {
            regenCoroutine = StartCoroutine(RegenerateDurability());
        }
        
        Debug.Log("Shield Deactivated! Durability: " + currentDurability);
    }
    
    void DrainDurability()
    {
        currentDurability -= durabilityDecayRate * Time.deltaTime;
        currentDurability = Mathf.Max(0, currentDurability);
        
        if (currentDurability <= 0)
        {
            DeactivateShield();
        }
    }
    
    IEnumerator RegenerateDurability()
    {
        yield return new WaitForSeconds(regenDelay);
        
        while (currentDurability < maxDurability && !isShieldActive)
        {
            currentDurability += regenRate * Time.deltaTime;
            currentDurability = Mathf.Min(maxDurability, currentDurability);
            yield return null;
        }
        
        regenCoroutine = null;
    }
    
    void UpdateShieldVisual()
    {
        if (shieldMaterial == null) return;
        
        // Update alpha based on durability
        float alpha = Mathf.Lerp(0.1f, shieldAlpha, currentDurability / maxDurability);
        Color color = shieldColor;
        color.a = alpha;
        shieldMaterial.color = color;
    }
    
    void UpdateShieldColor()
    {
        if (shieldMaterial == null) return;
        
        Color color = shieldColor;
        color.a = Mathf.Max(0.3f, shieldAlpha); // Minimal alpha 0.3 biar terlihat
        shieldMaterial.color = color;
        
        // Debug warna yang di-set
        Debug.Log("Shield color updated: " + color);
    }
    
    void CreateDefaultShieldPrefab()
    {
        // Buat sphere sebagai default shield
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "DefaultShield";
        
        // Scale sesuai player
        sphere.transform.localScale = Vector3.one * 2.5f;
        
        // Remove collider (kita tidak ingin shield menghalangi movement)
        Collider sphereCollider = sphere.GetComponent<Collider>();
        if (sphereCollider != null)
        {
            DestroyImmediate(sphereCollider);
        }
        
        // Setup material untuk transparansi - coba beberapa shader
        Renderer renderer = sphere.GetComponent<Renderer>();
        Material mat = null;
        
        // Coba shader Legacy/Transparent/Diffuse dulu
        if (Shader.Find("Legacy Shaders/Transparent/Diffuse") != null)
        {
            mat = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
        }
        // Fallback ke Unlit/Transparent
        else if (Shader.Find("Unlit/Transparent") != null)
        {
            mat = new Material(Shader.Find("Unlit/Transparent"));
        }
        // Fallback terakhir ke Standard
        else
        {
            mat = new Material(Shader.Find("Standard"));
            // Setup Standard untuk transparansi
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }
        
        // Set warna dengan alpha yang lebih terlihat
        Color shieldColorWithAlpha = shieldColor;
        shieldColorWithAlpha.a = 0.5f; // Lebih terlihat saat pertama kali
        mat.color = shieldColorWithAlpha;
        
        renderer.material = mat;
        
        // Set as prefab reference
        shieldPrefab = sphere;
        sphere.SetActive(false);
        
        Debug.Log("Default shield created with shader: " + mat.shader.name);
    }
    
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    // Method untuk damage dari luar (optional)
    public void TakeDamage(float damage)
    {
        if (!isShieldActive) return;
        
        currentDurability -= damage;
        currentDurability = Mathf.Max(0, currentDurability);
        
        PlaySound(shieldHitSound);
        
        if (currentDurability <= 0)
        {
            DeactivateShield();
        }
        
        Debug.Log("Shield took damage! Remaining durability: " + currentDurability);
    }
    
    // Getter untuk UI atau sistem lain
    public float GetDurabilityPercentage()
    {
        return currentDurability / maxDurability;
    }
    
    public bool IsShieldActive()
    {
        return isShieldActive;
    }
    
    public float GetCurrentDurability()
    {
        return currentDurability;
    }
    
    void OnDestroy()
    {
        if (currentShield != null)
        {
            Destroy(currentShield);
        }
    }
    
    void AutoDetectPlayer()
    {
        // Method 1: Cari berdasarkan tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            Debug.Log("Player auto-detected by tag: " + playerObj.name);
            return;
        }
        
        // Method 2: Cari berdasarkan nama yang mengandung "Player"
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.ToLower().Contains("player"))
            {
                playerTransform = obj.transform;
                Debug.Log("Player auto-detected by name: " + obj.name);
                return;
            }
        }
        
        // Method 3: Cari berdasarkan komponen yang biasa ada di player
        MonoBehaviour[] playerScripts = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour script in playerScripts)
        {
            if (script.GetType().Name.ToLower().Contains("player") || 
                script.GetType().Name.ToLower().Contains("character"))
            {
                playerTransform = script.transform;
                Debug.Log("Player auto-detected by script: " + script.GetType().Name + " on " + script.gameObject.name);
                return;
            }
        }
        
        // Method 4: Jika script ini attached ke player langsung
        if (playerTransform == null)
        {
            playerTransform = this.transform;
            Debug.Log("Using script owner as player: " + this.gameObject.name);
        }
        
        if (playerTransform == null)
        {
            Debug.LogError("Could not find player! Please assign playerTransform manually in inspector.");
        }
    }
    
    // Method untuk manual assign player dari script lain
    public void SetPlayer(Transform player)
    {
        playerTransform = player;
        Debug.Log("Player manually assigned: " + (player != null ? player.name : "NULL"));
    }
}