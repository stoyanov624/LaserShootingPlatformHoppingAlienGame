﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour {

    [Header("Patrol")]
    [SerializeField] private float movementSpeed;
    private float moveDirection = 1;
    private bool facingRight = true;
    [SerializeField] Transform edgeCheckPoint;
    [SerializeField] Transform obstacleCheckPoint;
    [SerializeField] float checkRadius;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask obstacleLayer;
    private bool isNearEdge;
    private bool isNearObstacle;

    [Header("Jump Attack")]
    [SerializeField] float jumpForce;
    [SerializeField] Transform player;
    
    [Header("Seeing Player")]
    [SerializeField] Vector2 lineOfSite;
    [SerializeField] LayerMask playerLayer;
    private bool canSeePlayer;
    private bool isAbovePlayer;
    private bool shouldAttack;

    [Header("Components")]
    [SerializeField] Animator animator;
    private BoxCollider2D enemyBoxCollider2D;
    private Rigidbody2D enemyRb;

    void Start() {
        enemyRb = GetComponent<Rigidbody2D>();
        enemyBoxCollider2D = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate() {
        isNearEdge = !(Physics2D.OverlapCircle(edgeCheckPoint.position,checkRadius,groundLayer));
        isNearObstacle = (Physics2D.OverlapCircle(obstacleCheckPoint.position,checkRadius,obstacleLayer));
        canSeePlayer = Physics2D.OverlapBox(transform.position,lineOfSite,0,playerLayer);
        isAbovePlayer = transform.position.y > (player.position.y + 0.5f);
        shouldAttack = canSeePlayer && isNearEdge && isAbovePlayer;

        AnimateMovement();
        
        if(shouldAttack) {
           JumpAttack();
        }else {
           Patrol();
        }
    }

    void JumpAttack() {
        FlipTowardsPlayer();
        float distanceFromPlayer = Vector2.Distance(transform.position,player.position);
        if(isGrounded()) {
            enemyRb.AddForce(new Vector2(distanceFromPlayer,jumpForce),ForceMode2D.Impulse);
        }
    }

    void AnimateMovement() {
        animator.SetFloat("vertical",Mathf.Abs(enemyRb.velocity.y));
        animator.SetFloat("horizontal", Mathf.Abs(enemyRb.velocity.x));
        animator.SetBool("isGrounded",isGrounded());
    }
    
    private void Patrol() {
        if(isGrounded()) {
            if((isNearEdge || isNearObstacle) && facingRight) {
                Flip();
            }else if((isNearEdge || isNearObstacle) && !facingRight) {
                Flip();
            }
            enemyRb.velocity = new Vector2(movementSpeed*moveDirection,enemyRb.velocity.y);
        }
    }

    private void Flip() {
        moveDirection *= -1;
        facingRight = !facingRight;
        transform.Rotate(0,180,0);
    }

    private void FlipTowardsPlayer() {
        float playerPosition = player.position.x - transform.position.x;
        if(playerPosition < 0 && facingRight) {
            Flip();
        }else if(playerPosition > 0 && !facingRight) {
            Flip();
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(edgeCheckPoint.position,checkRadius);
        Gizmos.DrawWireSphere(obstacleCheckPoint.position,checkRadius);
        Gizmos.DrawWireCube(transform.position,lineOfSite);
    }

    private bool isGrounded() {
        RaycastHit2D reycastHit2d = Physics2D.BoxCast(enemyBoxCollider2D.bounds.center,enemyBoxCollider2D.bounds.size,0f,Vector2.down,.1f,groundLayer);
        return reycastHit2d.collider != null;
    }
}