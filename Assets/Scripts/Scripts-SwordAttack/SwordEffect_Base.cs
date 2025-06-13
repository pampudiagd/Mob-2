using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordEffect_Base : MonoBehaviour
{
    public float damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Sets attributes to given values in data
    public virtual void Initialize(SwordData data)
    {
        damage = data.damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Sword Trigger");

        // Check if collision has Enemy_Base script attached, and damages if it does.
        Enemy_Base enemy = collision.GetComponent<Enemy_Base>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, "sword", "Normal");
        }
    }
}
