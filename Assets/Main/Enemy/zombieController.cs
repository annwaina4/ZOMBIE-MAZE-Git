using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NavMesh
using UnityEngine.AI;

public class zombieController : MonoBehaviour
{
    //状態（ステートパターン）
    private int stateNumber = 0;
    //汎用タイマー
    private float timeCounter = 0f;

    private Animator myanimator;

    private GameObject player;

    private Rigidbody myRigidbody;
    
    public GameObject attackPrefab;
    
    private GameObject attackPoint;
    
    public GameObject effectPrefab;

    private NavMeshAgent nav;

    //地点（親オブジェクト）
    public GameObject navPoints;

    //攻撃開始の間合い
    private float enemyLength = 2.0f;
    
    /*
    //配列でランダムに指定する インスペクターで個数を設定する
    //public GameObject[] navRandomPoints = new GameObject[7];
    //GetComponent<NavMeshAgent>().destination = navRandomPoints[0].transform.position;
    */

    //オブジェクト数
    private int number = 0;

    //------------------------------------------------------------------------------------------------------------------
    //スタート
    //------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        this.myanimator = GetComponent<Animator>();

        this.myRigidbody = GetComponent<Rigidbody>();

        this.nav = GetComponent<NavMeshAgent>();

        this.player = GameObject.Find("player");

        //ナビ地点の子オブジェクトの数
        number = navPoints.GetComponentInChildren<Transform>().childCount;

        int randomStart = Random.Range(0, number);

        attackPoint = transform.Find("attackPoint").gameObject;

        //前進アニメーション
        myanimator.SetInteger("speed", 1);
                
        //次の目標地点へ
        nav.destination = navPoints.transform.GetChild(randomStart).transform.position;
    }

    //------------------------------------------------------------------------------------------------------------------
    //距離と方向を求める関数
    //------------------------------------------------------------------------------------------------------------------

    //距離を求める
    float getLength(Vector3 current, Vector3 target)
    {
        return Mathf.Sqrt(((current.x - target.x) * (current.x - target.x)) + ((current.z - target.z) * (current.z - target.z)));
    }

    //方向を求める ※オイラー（-180～0～+180)
    float getEulerAngle(Vector3 current, Vector3 target)
    {
        Vector3 value = target - current;
        return Mathf.Atan2(value.x, value.z) * Mathf.Rad2Deg; //ラジアン→オイラー
    }

    //方向を求める ※ラジアン
    float getRadian(Vector3 current, Vector3 target)
    {
        Vector3 value = target - current;
        return Mathf.Atan2(value.x, value.z);
    }

    //------------------------------------------------------------------------------------------------------------------
    //アップデート
    //------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        if (playerController.isEnd == false)
        {
            //タイマー加算
            timeCounter += Time.deltaTime;
            //方向を求める
            float direction = getEulerAngle(this.transform.position, player.transform.position);
            //距離を求める
            float length = getLength(this.transform.position, player.transform.position);

            //**************************************************************************************************************
            //ここから状態処理
            //**************************************************************************************************************
            int randomNav = Random.Range(0, number);

            //目標地点の切換え
            if (stateNumber == 0)
            {
                if (length > enemyLength && nav.remainingDistance < 0.25f)
                {
                    //次の目標地点へ
                    nav.destination = navPoints.transform.GetChild(randomNav).transform.position;
                }

                if(Mathf.Abs(Mathf.DeltaAngle(direction,transform.eulerAngles.y))<30f)
                {
                    //プレーヤーが近い時
                    if (length < enemyLength)
                    {
                        //タイマーリセット
                        timeCounter = 0f;

                        //ナビ停止
                        nav.isStopped = true;

                        //アニメーション攻撃
                        myanimator.SetTrigger("attack");

                        //状態の遷移（切り替えステート）
                        stateNumber = 2;
                    }
                }               
            }

            //ナビの再開
            if (stateNumber == 1)
            {
                //プレイヤーが離れた時
                if (length > enemyLength)
                {
                    //ナビ再開
                    nav.isStopped = false;

                    //アニメーション前進
                    myanimator.SetInteger("speed", 1);

                    //次の目標地点へ
                    nav.destination = navPoints.transform.GetChild(randomNav).transform.position;

                    //状態の遷移（目標地点の切換え）
                    stateNumber = 0;
                }

                if (Mathf.Abs(Mathf.DeltaAngle(direction, transform.eulerAngles.y)) < 30f)
                {
                    //プレーヤーが近い時
                    if (length < enemyLength)
                    {
                        //タイマーリセット
                        timeCounter = 0f;

                        //ナビ停止
                        nav.isStopped = true;

                        //アニメーション攻撃
                        myanimator.SetTrigger("attack");

                        //状態の遷移（切り替えステート）
                        stateNumber = 2;
                    }
                }                    
            }

            //モーション切り替え
            else if (stateNumber == 2)
            {
                //アニメーション待機
                myanimator.SetInteger("speed", 0);

                //モーション終わり
                if (timeCounter > 1.0f)
                {
                    //タイマーリセット
                    timeCounter = 0f;
                    //状態の遷移（ナビの再開）
                    stateNumber = 1;
                }
            }
        }

        //**************************************************************************************************************
        //ゲーム終了処理
        //**************************************************************************************************************

        if (playerController.isEnd)
        {
            //ナビ停止
            nav.isStopped = true;

            //アニメーション　待機
            this.myanimator.SetInteger("speed", 0);
            
            //ステートパターンを停止
            stateNumber = -1;

            //自由落下を停止
            myRigidbody.useGravity = false;

            //衝突をなくす
            GetComponent<CapsuleCollider>().enabled = false;

            //myRigidbody.velocity = Vector3.zero;
            //myRigidbody.isKinematic = true;
        }
    }

    //------------------------------------------------------------------------------------------------------------------
    //攻撃イベント
    //------------------------------------------------------------------------------------------------------------------
    void attackEvent()
    {
        GameObject attack=Instantiate(attackPrefab, attackPoint.transform.position, Quaternion.identity);
        Destroy(attack.gameObject, 0.35f);

        //エフェクト生成
        Instantiate(effectPrefab, attackPoint.transform.position, Quaternion.identity);
    }
}
