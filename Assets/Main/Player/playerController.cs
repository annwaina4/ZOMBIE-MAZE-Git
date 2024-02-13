using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class playerController : MonoBehaviour
{
    Animator myanimator;

    Rigidbody myRigidbody;

    //進む速度
    private float stepVelocity = 2.3f;

    //HP設定
    private float maxHP = 3f;    
    private float nowHP;
    public Slider slider;
    
    static public bool isEnd = false;

    public GameObject effectPrefab;

    //private Vector3 previousPosition;

    //------------------------------------------------------------------------------------------------------------------
    //スタート
    //------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        Application.targetFrameRate = 60;

        this.myanimator = GetComponent<Animator>();

        this.myRigidbody = GetComponent<Rigidbody>();

        //Sliderを最大にする
        slider.value = 1;
        nowHP = maxHP;
    }

    //------------------------------------------------------------------------------------------------------------------
    //アップデート
    //------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        if (isEnd == false)
        {
            //**********************************************************************
            //移動と停止
            //**********************************************************************
            
            //右へ移動
            if (Input.GetKey(KeyCode.RightArrow))
            {
                myRigidbody.velocity = new Vector3(stepVelocity, myRigidbody.velocity.y, 0f);
                
                //アニメーション前進
                myanimator.SetInteger("speed", 1);

                //右を向く
                transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
            }

            //左へ移動
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                myRigidbody.velocity = new Vector3(-stepVelocity, myRigidbody.velocity.y, 0f);
                
                //アニメーション前進
                myanimator.SetInteger("speed", 1);
                
                //左を向く
                transform.rotation = Quaternion.Euler(0.0f, 270.0f, 0.0f);
            }

            //奥へ移動
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                myRigidbody.velocity = new Vector3(0f, myRigidbody.velocity.y, stepVelocity);

                //アニメーション前進
                myanimator.SetInteger("speed", 1);

                //奥を向く
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            }

            //手前へ移動
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                myRigidbody.velocity = new Vector3(0f, myRigidbody.velocity.y, -stepVelocity);

                //アニメーション前進
                myanimator.SetInteger("speed", 1);

                //手前を向く
                transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            }

            //停止
            else
            {
                myRigidbody.velocity = new Vector3(0f, myRigidbody.velocity.y, 0f);

                //アニメーション待機
                myanimator.SetInteger("speed", 0);
            }

            /*
            Vector3 direction = transform.position - previousPosition;
            previousPosition = transform.position;
            if(direction.magnitude>0.01f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
            */

            //**********************************************************************
            //HPゼロ時の処理
            //**********************************************************************

            if (nowHP <= 0)
            {
                //アニメーション死亡
                myanimator.SetBool("die", true);

                isEnd = true;

                //ゲームオーバー表示
                GameObject.Find("Canvas").GetComponent<UIController>().gameOver();
            }
        }

        //アイテムを全て獲得時
        if(GameObject.Find("Canvas").GetComponent<UIController>().itemCounter==7)
        {
            isEnd = true;
        }

        if (isEnd)
        {
            //アニメーション待機
            myanimator.SetInteger("speed", 0);

            myRigidbody.isKinematic = true;

            //タイトルへ戻る
            if (Input.GetKeyDown(KeyCode.Return))
            {
                isEnd = false;
                SceneManager.LoadScene("title");
            }
        }
    }

    //------------------------------------------------------------------------------------------------------------------
    //衝突判定
    //------------------------------------------------------------------------------------------------------------------
    private void OnCollisionEnter(Collision other)
    {
        if (isEnd == false)
        {
            //敵の攻撃を受けた時の処理
            if (other.gameObject.tag == "enemy" && nowHP > 0)
            {
                //ダメージアニメーション
                myanimator.SetTrigger("damage");

                nowHP -= 1f;

                GetComponent<AudioSource>().Play();

                //HPをSliderに反映
                slider.value = (float)nowHP / (float)maxHP;
            }            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isEnd == false)
        {
            //アイテム獲得時の処理
            if (other.gameObject.tag == "food")
            {
                Destroy(other.gameObject, 0.1f);

                //獲得エフェクト生成
                Instantiate(effectPrefab, other.gameObject.transform.position, Quaternion.identity);
                
                //獲得数表示
                GameObject.Find("Canvas").GetComponent<UIController>().itemCounter++;
                GameObject.Find("Canvas").GetComponent<UIController>().Addscore();

                //獲得音の発生
                GameObject.Find("ItemBGM").GetComponent<AudioSource>().Play();
            }
        }
    }
}
