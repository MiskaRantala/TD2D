﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Singleton<Tower> {

    [SerializeField]
    private string projectileType;

    [SerializeField]
    private float projectileSpeed;

    private Animator myAnimator;

    private SpriteRenderer mySpriteRenderer;

    private Monster target;

    public Queue<Monster> monsters = new Queue<Monster>();

    private bool canAttack = true;

    private AudioSource source;

    private float attackTimer;

    [SerializeField]
    private float attackCooldown;

    [SerializeField]
    private int damage;

    public float ProjectileSpeed
    {
        get
        {
            return projectileSpeed;
        }
    }

    public Monster Target
    {
        get
        {
            return target;
        }

        set
        {
            target = value;
        }
    }

    public int Damage
    {
        get
        {
            return damage;
        }
    }

    // Use this for initialization
    void Awake ()
    {
        myAnimator = transform.parent.GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        Attack();

        Debug.Log(Target);
	}

    public void Select()
    {
        mySpriteRenderer.enabled = !mySpriteRenderer.enabled;
    }

    private void Attack()
    {
        if (!canAttack)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackCooldown)
            {
                canAttack = true;
                attackTimer = 0;
            }
        }

        if ((Target == null && monsters.Count > 0) || monsters.Count == 0)
        {
            Target = monsters.Dequeue();
        }

        if (Target != null && Target.IsActive)
        {
            if (canAttack)
            {
                Shoot();

                source = GetComponent<AudioSource>();

                myAnimator.SetTrigger("Attack");

                canAttack = false;
            }
        }
    }

    private void Shoot()
    {
        Projectile projectile = GameManager.Instance.Pool.GetObject(projectileType).GetComponent<Projectile>();

        projectile.transform.position = transform.position;

        projectile.Initialize(this);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            monsters.Enqueue(other.GetComponent<Monster>());
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            Target = null;
        }
    }
}
