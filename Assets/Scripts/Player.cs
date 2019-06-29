﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class Player : MonoBehaviour {

    [SerializeField] private float jumpForce = 100f;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float groundCheckRadius = 0.5f;
    [SerializeField] [Range(0.1f, 0.9f)] private float crouchSpeedDemultiplier = 0.1f;

    [SerializeField] private float aimMaxDistane;
    [SerializeField] private float boxSeperationDistance = 5f;
    [SerializeField] private float boxDetectDistance = 0.2f;

    [SerializeField] private bool lookingRight = true;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask boxLayer;

    [SerializeField] private Box arif;
    [SerializeField] private AimingScript boxSilhouette;

    [SerializeField] private bool debug;

    private SpriteRenderer playerSpriteRenderer;
    private CapsuleCollider2D playerCollider;
    private Rigidbody2D playerRigidBody;
    private Transform playerTransform;
    private Transform groundCheck;
    private Transform eyes;

    private PlayerController movementControl = null;
    private Vector2 facingDirection;

    private bool isCrouching;
    private bool isJumping;
    private bool isHiding;
    private bool isDead;

    // Throwing
    private float torque;
    private float firingAngle = 45.0f;

    private void Awake() {

        isCrouching = false;
        isJumping = false;
        isHiding = false;
        isDead = false;

        facingDirection = lookingRight ? Vector2.right : Vector2.left;

        Physics2D.queriesStartInColliders = false;

    }

    // Start is called before the first frame update
    private void Start() {

        playerTransform = GetComponent<Transform>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();

        groundCheck = transform.Find("Ground Check").GetComponent<Transform>();
        eyes = transform.Find("Eyes").GetComponent<Transform>();

        playerRigidBody.freezeRotation = true;

        boxSilhouette.setMaxDistance(aimMaxDistane);

        movementControl = new PlayerController(ref playerRigidBody, ref playerTransform, ref facingDirection);
        
    }

    private void Update() {
        
        movement();

        if (arif.getIsStored()) {

            throwAim();
            boxThrow();

        }
        else {

            if (!isHiding) {
                boxReturn();
            }

            if (Physics2D.Raycast(playerTransform.position, facingDirection, boxDetectDistance, boxLayer)) {
                hideInBox();
            }

        }

        if (debug) { debugVision(); }

    }

    void OnCollisionEnter2D(Collision2D collision) {

        if (Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) && isJumping) {
            isJumping = false;
            //switch to standing animation
        }

    }

    // !! NEW !! //
    private void hideInBox() {

        if (Input.GetButtonDown(Global.controlsHide) && arif.getIsOnGround()) {

            isHiding = !isHiding;
                
            arif.getCollider().isTrigger = !arif.getCollider().isTrigger;
            arif.getRigidbody().velocity = Vector2.zero;
            arif.getRigidbody().isKinematic = !arif.getRigidbody().isKinematic; //not let box slide when hiding

            playerRigidBody.velocity = Vector2.zero;
            playerCollider.enabled = !playerCollider.enabled;
            playerRigidBody.isKinematic = !playerRigidBody.isKinematic;
            playerSpriteRenderer.enabled = !playerSpriteRenderer.enabled;

            /*
            playerRigidBody.drag = 12.0f; //not let player slide after hiding

            playerRigidBody.gravityScale = 0f;

            arif.getRigidbody().drag = 12.0f; //not let box slide when hiding
            arif.getRigidbody().gravityScale = 0f; //when change to trigger not fall through level

            arif.getCollider().isTrigger = true;  //change to trigger
            Physics2D.IgnoreLayerCollision(9, 11, true);

            playerSpriteRenderer.sortingOrder = 0; //change sorting order to make player go behind box

            playerCollider.isTrigger = true;

            playerTransform.position = new Vector2(arif.transform.position.x, arif.transform.position.y); //teleports player to middle of box when hiding                                            
            playerTransform.localScale = new Vector3(0.1f, 0.1f, 1f);//shrinks sprite to make fit in box
            */
            /*
            //change back everything
            isHiding = false;

            playerTransform.localScale = new Vector3(0.3f, 0.3f, 1f);

            playerRigidBody.drag = 0f;
            playerRigidBody.gravityScale = 1f;

            playerCollider.isTrigger = false;

            arif.getRigidbody().drag = 3f;
            arif.getRigidbody().gravityScale = 1f;

            arif.getCollider().isTrigger = false;

            Physics2D.IgnoreLayerCollision(9, 11, false);

            playerSpriteRenderer.sortingOrder = 2;
            */

        }

    }

    private void boxThrow() {

        if (Input.GetButtonUp(Global.controlsThrow) && boxSilhouette.gameObject.activeSelf) {

            boxSilhouette.gameObject.SetActive(false);

            float target_Distance = Vector3.Distance(boxSilhouette.transform.position, playerTransform.position);

            // Calculate the velocity needed to throw the object to the target at specific angle
            float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / (Physics2D.gravity.y * -1));

            // Extract the X Y component of velocity
            float velocityX = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
            float velocityY = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

            Vector2 direction = (boxSilhouette.transform.position - playerTransform.position).normalized;

            Vector2 vel = new Vector2(velocityX * direction.x, velocityY);

            arif.thrown();

            arif.transform.position = new Vector2(playerTransform.position.x + ((playerSpriteRenderer.size.x + 0.1f) * facingDirection.x), playerTransform.position.y);

            arif.getRigidbody().AddForce(vel, ForceMode2D.Impulse);
            arif.getRigidbody().AddTorque(torque);

        }

    }

    private void throwAim() {

        if (Input.GetButtonDown(Global.controlsThrow)) {

            boxSilhouette.gameObject.SetActive(true);

            //boxSilhouette.getPlayerPosition(playerTransform.position);

        }

    }

    private void movement() {

        if (!isHiding){
            movementControl.horizontalMovement(isCrouching ? movementSpeed * crouchSpeedDemultiplier : movementSpeed, ref facingDirection);
            movementControl.Jump(ref isJumping, jumpForce);
            movementControl.crouch(ref isCrouching);
        }

    }

    private void boxReturn() {

        if (Input.GetButtonDown(Global.controlsRecall) || (arif.transform.position - playerTransform.position).magnitude > boxSeperationDistance) {
            arif.store();
        }

    }

    public bool getIsDead() {
        return isDead;
    }

    public void setPlayerPosition(Vector3 position) {

        playerTransform.position = position;

    }

    public Vector3 getPosition() {

        return playerTransform.position;

    }

    public bool[] getBoxUpgradeStatus() {

        bool[] boxAbilities = new bool[Enum.GetNames(typeof(Global.BoxAbilities)).Length];

        for(int i = 0; i < Enum.GetNames(typeof(Global.BoxAbilities)).Length; i++) {

            boxAbilities[i] = arif.getIsAbilityUnlocked((Global.BoxAbilities)i);

        }

        return boxAbilities;

    }

    public void upgradeBox(Global.BoxAbilities ability) {

        arif.unlockAbility(ability);

    }

    public void killPlayer() {
        isDead = true;
    }

    private void debugVision() {

        Debug.DrawRay(playerTransform.position, boxDetectDistance * facingDirection, Color.red);

    }

}
