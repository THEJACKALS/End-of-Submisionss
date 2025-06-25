using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float explodeDelay = 5f;

    void Start()
    {
        Invoke(nameof(Explode), explodeDelay);
    }

    void Explode()
    {
        // Tambahkan efek ledakan di sini jika ada
        Destroy(gameObject);
    }
}