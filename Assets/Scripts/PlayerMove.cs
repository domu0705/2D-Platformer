using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }


    void Update()//1초에 60회 돌음. 단발적인 키 입력을 받음
    {
        //jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            anim.SetBool("isJumping", true);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
            

        //stop speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x*0.5f, rigid.velocity.y);
        }

        //이미지의 방향 전환
        if (Input.GetButtonDown("Horizontal"))
            spriteRenderer.flipX = (Input.GetAxisRaw("Horizontal") == -1);

        //animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }


    void FixedUpdate()//지속적인 키 입력을 받음. 1초에 50회 돌음
    {
        //move by key control
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h*2, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed)//right max speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1))//left max speed
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        //landing platform
        if(rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));//에디터 상에서만 ray를 그려주는 함수

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1,LayerMask.GetMask("Platform")); // 3번째 인자값의 1은 ray의 길이. 4번째 인자값은 어떤 layer만 감지할건지 설정가능(없어도 됨). rayHit은 빔에 맞은 물체를 알려줌.
        
            if(rayHit.collider != null)
            {//빔에 무언가가 맞았을 때
                if (rayHit.distance < 0.5f)
                    anim.SetBool("isJumping", false);
            }

        }
        

            
    }
}
