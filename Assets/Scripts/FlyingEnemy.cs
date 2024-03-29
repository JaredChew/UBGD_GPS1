﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour {

    [SerializeField] private float directionSwitchDuration;

    private FlyingEnemyVision visionLeft;
    private FlyingEnemyVision visionRight;
    private Animator enemyAnimator;
    private Rigidbody2D enemyRigidbody;

    private float timer;

    private void Awake() {

        timer = 0f;

    }

    private void Start() {

        visionLeft = transform.Find("Vision Left").GetComponent<FlyingEnemyVision>();
        visionRight = transform.Find("Vision Right").GetComponent<FlyingEnemyVision>();
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<Animator>();

        visionRight.gameObject.SetActive(false);

    }

    private void Update() {

        rotateVision();
        setAnimation();

    }

    private void setAnimation()
    {
        if(timer >= directionSwitchDuration)
        {
            enemyAnimator.SetBool("isRight", true);
            enemyAnimator.SetBool("isLeft", true);

        }
       
        
    }

    private void rotateVision() {

        if(timer >= directionSwitchDuration) {

            visionLeft.gameObject.SetActive(!visionLeft.gameObject.activeSelf);
           
            visionRight.gameObject.SetActive(!visionRight.gameObject.activeSelf);
            

            timer = 0f;

        }

        timer += Time.deltaTime;

    }

}
