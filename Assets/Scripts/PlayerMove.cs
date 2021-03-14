using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;

    /*Sound*/
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    public float maxSpeed;
    public float jumpPower;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsuleCollider;
    AudioSource audioSource;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }
    }
    void Update()//1초에 60회 돌음. 단발적인 키 입력을 받음
    {
        //jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            anim.SetBool("isJumping", true);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            PlaySound("JUMP");
            audioSource.Play();
        }
            

        //stop speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x*0.5f, rigid.velocity.y);
        }

        //이미지의 방향 전환
        if (Input.GetButton("Horizontal"))
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)//플레이어가 낙하 중이면서 몬스터보다 위에 있다면
            {
                OnAttack(collision.transform);
                PlaySound("ATTACK");
                audioSource.Play();
            }
            else 
                OnDamaged(collision.transform.position);
        }
    }


    void OnTriggerEnter2D(Collider2D collision)//trigger은 충돌하지 않고 통과함
    {
        if (collision.gameObject.tag == "Item")
        {
            PlaySound("ITEM");
            audioSource.Play();
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");


            //point얻기
            if(isBronze)
                gameManager.stagePoint += 50;
            else if(isSilver)
                gameManager.stagePoint += 100;
            else if (isGold)
                gameManager.stagePoint += 300;



            //동전 사라지게 하기
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Finish")
        {
            //다음 스테이지로 넘어가기
            gameManager.NextStage();
            PlaySound("FINISH");
            audioSource.Play();
        }
    }


    void OnAttack(Transform enemy)
    {
        //적 밟을 때 player의 반발력
        rigid.AddForce(Vector3.up * 10, ForceMode2D.Impulse);

        //점수 추가
        gameManager.stagePoint += 100;
        
        //enemy죽음
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();


    }

    void OnDamaged(Vector2 targetPos)
    {
        PlaySound("DAMAGED");
        audioSource.Play();
        //health 줄여줌
        gameManager.HealthDown();

        //change layer. enemy와 부딪히지 않는 무적으로 바꿔줌
        gameObject.layer = 11;

        //투명도 올려줌
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //튕겨나가는 효과
        int direction = transform.position.x > targetPos.x ? 1 : -1;
        rigid.AddForce(new Vector2(direction, 1)*7, ForceMode2D.Impulse);

        //에니메이션
        anim.SetTrigger("doDamaged");

        Invoke("offDamaged", 3);
    }



    /*무적을 해제하는 함수*/
    void offDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie() 
    {

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        spriteRenderer.flipY = true;
        capsuleCollider.enabled = false;

        //죽는 모션
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        PlaySound("DIE");
        audioSource.Play();
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
