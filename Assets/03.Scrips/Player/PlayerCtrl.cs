﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//클래스는 System.Serializable이라는 어트리뷰트(Attribute)를 명시해야
//인스펙터 뷰에 노출됨
[System.Serializable]
public class PlayerAnim
{
    public AnimationClip idle;
    public AnimationClip runF;
    public AnimationClip runB;
    public AnimationClip runL;
    public AnimationClip runR;
    public AnimationClip hit;
    public AnimationClip Reload;
}

public class PlayerCtrl : MonoBehaviour
{

    float h = 0; 
    float v = 0; 
    float x = 0;
    float y = 0;

    public bool isDead = false;
    

    //접근해야 하는 컴포넌트
    Transform tr;
    Rigidbody rb;

    //이동속도
    public float moveSpeed = 10f; //이동속도
    public float rotSpeed = 80f; //회전속도
    public float jumpSpeed = 5f; //점프높이
    public bool isGrounded = false;
    public int jumpCount = 2; //점프 횟수

    //인스펙터 뷰에 표시할 애니메이션 클래스 변수
    public PlayerAnim playerAnim;
    //Animation 컴포넌트를 저장하기 위한 변수
    public Animation anim;

    bool button = false;

    void Start()
    {
        tr = GetComponent<Transform>();
        //Animation 컴포넌트를 변수에 할당
        anim = GetComponentInChildren<Animation>();
        rb = GetComponent<Rigidbody>();
        //Animation 컴포넌트의 애니메이션 클립을 지정하고 실행
        anim.clip = playerAnim.idle;
        anim.Play();
        jumpCount = 0;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {            
            isGrounded = true; //Ground에 닿으면 isGround는 true
            jumpCount = 2; //Ground에 닿으면 점프횟수가 2로 초기화
        }
    }

    public void HitDamage()
    {
        anim.CrossFade(playerAnim.hit.name, 0f);
    }    

    public void Playergunreload()
    {
        anim.CrossFade(playerAnim.Reload.name, 0f);
    }


    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        x = Input.GetAxis("Mouse X");
        y -= Input.GetAxis("Mouse Y");        
        if (isGrounded == true)
        {
            if (jumpCount > 0)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    rb.AddForce(0, 300f, 0);
                    jumpCount--; //점프 할때마다 점프횟수 감소
                }
            }
        }
       //전후좌우 이동 방향 벡터 계산
       Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
       //Transform(이동방향 * 속도 * 변위값 * Time.deltaTime, Space.Self)
       tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);
       //좌우 회전
       transform.Rotate(0f, x, 0f);
       //상하 회전
       y = Mathf.Clamp(y, -90, 90);
       Camera.main.transform.localRotation = Quaternion.Euler(y, 0, 0f);
       //키보드 입력값 기준으로 동작할 애니메이션 수행
       if (v >= 0.1f)
       {
           anim.CrossFade(playerAnim.runF.name, 0.3f);
       }
       else if (v <= -0.1f)
       {
           anim.CrossFade(playerAnim.runB.name, 0.3f);
       }
       else if (h >= 0.1f)
       {
           anim.CrossFade(playerAnim.runR.name, 0.3f);
       }
       else if (h <= -0.1f)
       {
           anim.CrossFade(playerAnim.runL.name, 0.3f);
       }
       else
       {
           anim.CrossFade(playerAnim.idle.name, 0.3f);
       }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
