using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{

    Rigidbody2D rigid;
    public int nextMove;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        //move
        Think();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //platform 에서 떨어지지 않게 확인하기
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove, rigid.position.y );
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));//에디터 상에서만 ray를 그려주는 함수

        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform")); // 3번째 인자값의 1은 ray의 길이. 4번째 인자값은 어떤 layer만 감지할건지 설정가능(없어도 됨). rayHit은 빔에 맞은 물체를 알려줌.

        if (rayHit.collider == null)
        {//앞이 낭떠러지일 때
            TurnEnemy();
        }
    }

    void Think()//nextMove를 바꿔주는 함수
    {
        //enemy의 움직임을 random하게 조절
        nextMove = Random.Range(-1, 2);
        
        //animation 조정용
        anim.SetInteger("walkSpeed", nextMove);

        //enemy가 좌,우로 움직일 때 이미지 좌우반전
        if(nextMove != 0)
            spriteRenderer.flipX = (nextMove == 1);

        float thinkTime  = Random.Range(2f, 5f);
        Invoke ("Think", thinkTime);
    }

    void TurnEnemy()
    {
        //enemy를 반대방향으로 돌려주기
        nextMove *= -1;

        //enemy가 좌,우로 움직일 때 이미지 좌우반전
        if (nextMove != 0)
            spriteRenderer.flipX = (nextMove == 1);
        
        //Think함수의 5초 간격을 맞춰주기 위해 원래 진행되던 invoke를 취소하고 다시 5초 시작. 
        CancelInvoke();
        Invoke("Think", 5);
    }

    public void OnDamaged()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        spriteRenderer.flipY = true;
        capsuleCollider.enabled = false;

        //죽는 모션
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //적 없애기
        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
