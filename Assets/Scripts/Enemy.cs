using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Mover
{
    // Experience
    public int xpValue = 1;

    // Logic
    public float triggerLenght = 1;
    public float chaseLenght = 5;
    private bool chasing;
    private bool collidingWithPlayer;
    private Transform playerTransform;
    private Vector3 startingPosition;

    // Hitbox
    public ContactFilter2D filter;
    private BoxCollider2D hitbox;
    private Collider2D[] hits = new Collider2D[10]; // cant inherit from Collidable

    protected override void Start()
    {
        base.Start();
        playerTransform = GameObject.Find("Player").transform;
        startingPosition = transform.position;
        hitbox = transform.GetChild(0).GetComponent<BoxCollider2D>(); // Get child to get actual hitbox, not the collider2D of an enemy itself
    }
    private void FixedUpdate()
    {
        // Is the player in range?
        if (Vector3.Distance(playerTransform.position, startingPosition) < chaseLenght)
        {
            if (Vector3.Distance(playerTransform.position, startingPosition) < triggerLenght)
            {
                chasing = true;
            }

            if (chasing)
            {
                if (!collidingWithPlayer)
                {
                    UpdateMotor((playerTransform.position - transform.position).normalized);
                }
            }
            else
            {
                UpdateMotor(startingPosition - transform.position);
            }
        }
        else
        {
            UpdateMotor(startingPosition - transform.position);
            chasing = false;
        }

        // Check for overlaps
        collidingWithPlayer = false;
        boxCollider.OverlapCollider(filter, hits);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
            {
                continue;
            }

            if (hits[i].tag == "Fighter" && hits[i].name == "Player")
            {
                collidingWithPlayer = true;
            }

            //The array is not cleaned up by itself, so to trigger it:
            hits[i] = null;
        }
    }

    protected override void Death()
    {
        Destroy(gameObject);
        GameManager.instance.experience += xpValue;
        GameManager.instance.ShowText("+" + xpValue + " xp", 30, Color.magenta, transform.position, Vector3.up * 40, 1.0f);
    }

}