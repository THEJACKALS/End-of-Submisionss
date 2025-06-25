using UnityEngine;

public class BombThrower : MonoBehaviour
{
    public GameObject bombPrefab;      // Assign bomb prefab via Inspector
    public float throwPower = 7f;      // Force applied to the bomb
    public float bombCooldown = 3f;    // Delay between throws

    private float lastBombTime = -Mathf.Infinity;
    private bool facingRight = true;   // Ganti sesuai sistem arah karakter kamu

    void Update()
    {
        // Contoh sederhana: update arah berdasarkan input horizontal
        float move = Input.GetAxisRaw("Horizontal");
        if (move > 0) facingRight = true;
        else if (move < 0) facingRight = false;

        HandleBombThrow();
    }

    void HandleBombThrow()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (Time.time - lastBombTime >= bombCooldown)
            {
                ThrowBomb();
                lastBombTime = Time.time;
            }
            else
            {
                float remaining = bombCooldown - (Time.time - lastBombTime);
                Debug.Log($"Tunggu {remaining:F1} detik sebelum melempar bomb lagi!");
            }
        }
    }

    void ThrowBomb()
    {
        if (bombPrefab == null)
        {
            Debug.LogError("Bomb prefab belum di-assign!");
            return;
        }

        Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
        spawnPos.x += facingRight ? 1f : -1f;

        GameObject bomb = Instantiate(bombPrefab, spawnPos, Quaternion.identity);

        Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 dir = new Vector2(facingRight ? 1f : -1f, 0.8f).normalized;
            rb.AddForce(dir * throwPower, ForceMode2D.Impulse);
        }
    }
}