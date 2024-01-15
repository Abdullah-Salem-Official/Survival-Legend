using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float xRange = 10f;
    public GameObject hitPrefab;
    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(transform.position.x) > xRange)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Collectable") && !collision.gameObject.CompareTag("BossProjectile"))
        {

            
            if(!collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("Boss"))
            {

                Instantiate(hitPrefab, transform.position, Quaternion.identity);
            }

            // Detach the child objects from the parent before destroying it
            Transform[] childObjects = GetComponentsInChildren<Transform>();
            foreach (Transform child in childObjects)
            {
                child.SetParent(null);
            }

            Destroy(gameObject);
        }
    }
}
