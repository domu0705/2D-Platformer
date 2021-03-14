using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;

    public Image[] UIhelth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject RestartBtn;

    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }
    public void NextStage()
    {
        //다음 스테이지로 전환해주기
        if(stageIndex < Stages.Length-1 )
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        else// 모든 stage clear
        {
            Time.timeScale = 0; //시간을 멈춰둠.
            Debug.Log("게임 클리어");
            RestartBtn.SetActive(true);
            Text btnText = RestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Clear!";
            RestartBtn.SetActive(true);
        }
        

        //현재 stage의 point를 전체 point에 합쳐주기
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //player을 원래 위치로 돌려놓기+낙하속도 없애주기
            if (health > 1)
            {
                PlayerReposition();
            }
             //health감소
            HealthDown();
           
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(-8, 0.5f, -1);
        player.VelocityZero();
    }


    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            UIhelth[health].color = new Color(1, 0, 0, 0.4f);
        }
        else
        {
            UIhelth[0].color = new Color(1, 0, 0, 0.4f);
            //player Die효과
            player.OnDie();
            //result UI
            Debug.Log("죽었습니다.");
            //retry Button UI
            RestartBtn.SetActive(true);
        }
    }

    public void Restart()
    {
        Time.timeScale = 1; // 멈춘 시간을 다시 복구
        SceneManager.LoadScene(0);
    }
}
