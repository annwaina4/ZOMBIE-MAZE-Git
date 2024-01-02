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
    private float enemyLength = 2.0f;
    private GameObject player;

    private Rigidbody myRigidbody;
    private NavMeshAgent nav;
    //地点（親オブジェクト）
    public GameObject navPoints;
    public GameObject attackPrefab;
    private GameObject attackPoint;
    public GameObject effectPrefab;

    //配列でランダムに指定する方法 ※インスペクターで個数を設定する
    //public GameObject[] navRandomPoints = new GameObject[7];
    //例: GetComponent<NavMeshAgent>().destination = navRandomPoints[0].transform.position;

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

        //子オブジェクトの数
        number = navPoints.GetComponentInChildren<Transform>().childCount;

        int randomStart = Random.Range(0, number);

        attackPoint = transform.Find("attackPoint").gameObject;

        //if (number > 0)
        //{
        //アニメーション
        myanimator.SetInteger("speed", 1);
        //次の目標地点へ
        nav.destination = navPoints.transform.GetChild(randomStart).transform.position;
        //}

        //最初の目標地点 ※無しチェック
        /*if (number > 0)
        {
            nav.destination = navPoints.transform.GetChild(0).transform.position;
        } */
    }

    //------------------------------------------------------------------------------------------------------------------
    //オリジナル関数
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
            //int stepPattern = Random.Range(1, 3);
            //int enemyPattern = Random.Range(4, 9);

            //目標地点まで行った？ ※ブログでよく見かける条件
            //if (GetComponent<NavMeshAgent>().remainingDistance == 0.0f)
            //ぴったり0.0fにならないことがあるので、0.5f以下が良さそう


            //待機
            if (stateNumber == 0)
            {
                if (length > enemyLength && nav.remainingDistance < 0.25f)
                {
                    //次の目標地点 ※無しチェック
                    //if (number > 0)
                    //{
                    //myanimator.SetInteger("speed", 1);
                    //次の目標地点へ
                    nav.destination = navPoints.transform.GetChild(randomNav).transform.position;
                    //}
                }

                if(Mathf.Abs(Mathf.DeltaAngle(direction,transform.eulerAngles.y))<30f)
                {
                    //プレーヤーが近い時
                    if (length < enemyLength)
                    {
                        //タイマーリセット
                        timeCounter = 0f;

                        //アニメーション　攻撃
                        nav.isStopped = true;

                        myanimator.SetTrigger("attack");
                        //myanimator.SetInteger("speed", 0);

                        //Debug.Log("攻撃: " + Time.time);

                        //状態の遷移（切り替えステート）
                        stateNumber = 2;
                    }
                }
                
            }

            if (stateNumber == 1)
            {
                //プレイヤーが離れた？
                if (length > enemyLength)
                {
                    nav.isStopped = false;
                    myanimator.SetInteger("speed", 1);
                    //次の目標地点へ
                    nav.destination = navPoints.transform.GetChild(randomNav).transform.position;
                    stateNumber = 0;

                }

                if (Mathf.Abs(Mathf.DeltaAngle(direction, transform.eulerAngles.y)) < 30f)
                {
                    //プレーヤーが近い時
                    if (length < enemyLength)
                    {
                        //タイマーリセット
                        timeCounter = 0f;

                        //アニメーション　攻撃
                        nav.isStopped = true;

                        myanimator.SetTrigger("attack");
                        //myanimator.SetInteger("speed", 0);

                        //Debug.Log("攻撃: " + Time.time);

                        //状態の遷移（切り替えステート）
                        stateNumber = 2;
                    }
                }                    
            }

            //モーション切り替え
            else if (stateNumber == 2)
            {
                //myanimator.SetTrigger("attack");
                myanimator.SetInteger("speed", 0);

                //モーション終わり
                if (timeCounter > 1.0f)
                {
                    //タイマーリセット
                    timeCounter = 0f;
                    //状態の遷移（待機）
                    stateNumber = 1;
                }
            }
        }

        //**************************************************************************************************************
        //ゲーム‐オーバー監視（追加）
        //**************************************************************************************************************

        if (playerController.isEnd)
        {
            nav.isStopped = true;
            //アニメーション　待機
            this.myanimator.SetInteger("speed", 0);
            //ステートパターンを停止
            stateNumber = -1;
            //myRigidbody.velocity = Vector3.zero;
            //myRigidbody.isKinematic = true;
            //自由落下を停止
            myRigidbody.useGravity = false;
            //衝突をなくす
            GetComponent<CapsuleCollider>().enabled = false;
        }        
    }

    void attackEvent()
    {
        GameObject attack=Instantiate(attackPrefab, attackPoint.transform.position, Quaternion.identity);
        Destroy(attack.gameObject, 0.35f);
        Instantiate(effectPrefab, attackPoint.transform.position, Quaternion.identity);
    }
}
