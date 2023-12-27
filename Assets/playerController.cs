using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class playerController : MonoBehaviour
{
    Animator myanimator;
    Rigidbody myRigidbody;

    //private float stepForce = 2000.0f;
    //private float maxStepSpeed = 2.0f;
    //private float stepVelocity = 5f;
    //ジャンプ速度減衰
    private float damping = -3000f;
    private float groundLevel = 0.1f;
    private float jumpforce = 30000.0f;


    private Vector3 previousPosition;

    private float maxHP = 100;
    private float nowHP;
    //public Slider slider;
    private int itemCounter = 0;

    static public bool isEnd = false;

    //GameObject guardPoint;
    //public GameObject guardPrefab;
    public GameObject effectPrefab;


    //衝突判定スキップ時間
    //private float skipCollision = 0.0f;

    void Start()
    {
        Application.targetFrameRate = 60;

        this.myanimator = GetComponent<Animator>();

        this.myRigidbody = GetComponent<Rigidbody>();

        //guardPoint = transform.Find("guardPoint").gameObject;

        //Sliderを最大にする
        //slider.value = 1;
        nowHP = maxHP;

    }

    //自由落下を強化
    void FixedUpdate()
    {
        //Vector3 gravity = -9.81f * 2.0f * Vector3.up;
        //this.myRigidbody.AddForce(gravity, ForceMode.Acceleration);
    }

    void Update()
    {
        //衝突判定スキップ時間を減ずる
        //skipCollision -= Time.deltaTime;

        //timeCounter += Time.deltaTime;
        //bool isGround = (transform.position.y > this.groundLevel) ? false : true;
        //myanimator.SetBool("isground", isGround);

        if (isEnd == false)
        {
            //プレーヤーの速度
            float speedx = Mathf.Abs(this.myRigidbody.velocity.x);
            float speedz = Mathf.Abs(this.myRigidbody.velocity.z);

            bool isGround = (transform.position.y > this.groundLevel) ? false : true;

            //**********************************************************************
            //移動と待機
            //**********************************************************************
            if (Input.GetKey(KeyCode.RightArrow))
            {
                //key = 1;
                myRigidbody.velocity = new Vector3(3.0f, 0f, 0f);
                myanimator.SetInteger("speed", 1);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                //key = -1;
                myRigidbody.velocity = new Vector3(-3.0f, 0f, 0f);
                myanimator.SetInteger("speed", 1);
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                //key = 1;
                myRigidbody.velocity = new Vector3(0f, 0f, 3.0f);
                myanimator.SetInteger("speed", 1);
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                //key = -1;
                myRigidbody.velocity = new Vector3(0f, 0f, -3.0f);
                myanimator.SetInteger("speed", 1);
            }

            /*if (Input.GetKey(KeyCode.Space))
            {
                //key = -1;
                myRigidbody.AddForce(0f, 100000f, 0f);
            }*/

            if (speedx < 0.1f && speedz < 0.1f)
            {
                myanimator.SetInteger("speed", 0);
            }

            Vector3 direction = transform.position - previousPosition;
            previousPosition = transform.position;
            if(direction.magnitude>0.01f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            //if (transform.position.y > -0.1f)
            //{
                if (Input.GetKeyDown(KeyCode.Space) && isGround)
                {
                    myRigidbody.AddForce(0, this.jumpforce,0);
                }

                if (Input.GetKey(KeyCode.Space) == false)
                {
                    if (myRigidbody.velocity.y > 0)
                    {
                        myRigidbody.AddForce(0, this.damping,0);
                    }
                }
            //}

            //**********************************************************************
            //攻撃、ガード、ノックダウンのアニメーション
            //**********************************************************************
            /*
            if (Input.GetKeyDown(KeyCode.Y))
            {
                myanimator.SetTrigger("leftP");
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                myanimator.SetTrigger("rightP");
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                myanimator.SetTrigger("leftK");
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                myanimator.SetTrigger("rightK");
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                myanimator.SetTrigger("guard");
                myanimator.SetBool("guardEnd", false);
            }
            if (Input.GetKeyUp(KeyCode.L))
            {
                myanimator.SetBool("guardEnd", true);
            }
            */
            if (nowHP <= 0)
            {
                isEnd = true;
                myanimator.SetBool("die", true);
                //GameObject.Find("Canvas").GetComponent<UIController>().gameLose();
            }
        }

        if (isEnd)
        {
            myanimator.SetInteger("speed", 0);
            myRigidbody.isKinematic = true;

            if (Input.GetKeyDown(KeyCode.Return))
            {
                isEnd = false;
                SceneManager.LoadScene("Maze 1");
            }
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (isEnd == false)
        {
            //ガードのモーションの時はダメージを無視
            if (myanimator.GetCurrentAnimatorStateInfo(0).IsName("bb_front_A"))
            {

            }
            else
            {
                if (other.gameObject.tag == "enemy" && nowHP > 0)
                {
                    //Debug.Log("ダメージ: " + Time.time);

                    myanimator.SetTrigger("damage");

                    nowHP -= 1f;

                    /*foreach (ContactPoint point in other.contacts)
                    {
                        Instantiate(effectPrefab, (Vector3)point.point, Quaternion.identity);
                    }*/

                    //GetComponent<AudioSource>().Play();

                    //HPをSliderに反映
                    //slider.value = (float)nowHP / (float)maxHP;
                    //}
                }

                /*if (other.gameObject.tag == "food")
                {
                    itemCounter++;
                    Destroy(other.gameObject, 0.5f);
                    foreach (ContactPoint point in other.contacts)
                    {
                        Instantiate(effectPrefab, (Vector3)point.point, Quaternion.identity);
                    }
                }*/
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isEnd == false)
        {
            if (other.gameObject.tag == "food")
            {
                itemCounter++;
                Destroy(other.gameObject, 0.1f);
                Instantiate(effectPrefab, other.gameObject.transform.position, Quaternion.identity);
            }
        }
    }
}
