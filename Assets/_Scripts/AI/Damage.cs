using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class deals damage to the player, while creating invincibility frames for the player.
/// </summary>
public class Damage : MonoBehaviour {

    private PlayerController player;
    int damageCount = 0;
    float invicibilityTimer = 2f;
    float startTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && invicibilityCheck())
        {
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			
            //player.Damage(1);
            StartCoroutine(player.Knockback(CommonUtils.NormalVector(transform.position, player.transform.position)));
            startTime = Time.time;
            damageCount++;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && invicibilityCheck())
        {
            Debug.Log("Damage");
            //player.Damage(1);
            StartCoroutine(player.Knockback(CommonUtils.NormalVector(transform.position, player.transform.position)));
            startTime = Time.time;
            damageCount++;
        }
    }

    private bool invicibilityCheck()
    {
        if (damageCount == 0)
        {
            return true;
        }
        else
        {
            return Time.time - startTime > invicibilityTimer;
        }
    }

}
