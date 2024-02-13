using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NavMesh
using UnityEngine.AI;

public class batMaleController : MonoBehaviour
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

    //配列でランダムに指定する方法 ※インスペクターで個数を設定する
    public GameObject[] navRandomPoints = new GameObject[7];
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

        int randomStart = Random.Range(0, 7);

        if (number > 0)
        {
            //アニメーション
            myanimator.SetInteger("speed", 1);
            //次の目標地点へ
            nav.destination = navRandomPoints[randomStart].transform.position;

        }

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
        //Debug.Log("現在のstatenumber" + stateNumber);        
        //タイマー加算
        timeCounter += Time.deltaTime;
        //方向を求める
        float direction = getEulerAngle(this.transform.position, player.transform.position);
        //距離を求める
        float length = getLength(this.transform.position, player.transform.position);

        //**************************************************************************************************************
        //ここから状態処理
        //**************************************************************************************************************
        int randomNav = Random.Range(0, 7);
        //int stepPattern = Random.Range(1, 3);
        //int enemyPattern = Random.Range(4, 9);

        //目標地点まで行った？ ※ブログでよく見かける条件
        //if (GetComponent<NavMeshAgent>().remainingDistance == 0.0f)
        //ぴったり0.0fにならないことがあるので、0.5f以下が良さそう


        //待機
        if (stateNumber == 0)
        {
            if (length > enemyLength&&nav.remainingDistance < 0.25f)
            {
                //次の目標地点 ※無しチェック
                if (number > 0)
                {
                    //myanimator.SetInteger("speed", 1);
                    //次の目標地点へ
                    nav.destination = navRandomPoints[randomNav].transform.position;
                }
            }

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

        if (stateNumber == 1)
        {
            //次の目標地点 ※無しチェック
            if (length > enemyLength&&number > 0)
            {
                nav.isStopped = false;
                myanimator.SetInteger("speed", 1);
                //次の目標地点へ
                nav.destination = navRandomPoints[randomNav].transform.position;
                stateNumber = 0;

            }

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

        /*
        //後退
        else if (stateNumber == 2)
        {
            //プレーヤーの方向を向く
            this.transform.rotation = Quaternion.Euler(0f, direction, 0f);
            myanimator.SetInteger("speed", -1);
            //移動
            //myRigidbody.velocity = transform.forward * -stepVelocity;
            //myRigidbody.AddForce(transform.forward * stepForce);

            //5秒経過
            if (timeCounter > 0.5f)
            {
                timeCounter = 0f;

                //アニメーション　待機
                this.myanimator.SetInteger("speed", 0);

                //状態の遷移（待機）
                stateNumber = 0;
            }
            //プレーヤーが近い時
            else if (length < enemyLength)
            {
                //タイマーリセット
                timeCounter = 0f;

                //アニメーション　待機
                //this.myanimator.SetInteger("speed", 0);

                //アニメーション　攻撃
                this.myanimator.SetTrigger("rightK");

                //状態の遷移（攻撃)
                stateNumber = 3;
            }
        }

        //モーション切り替え
        else if (stateNumber == 3)
        {
            //モーション終わり
            if (timeCounter > 0.2f)
            {
                //タイマーリセット
                timeCounter = 0f;

                //状態の遷移（待機）
                stateNumber = enemyPattern;
            }
        }

        else if (stateNumber == 4)
        {
            //プレーヤーの方向を向く
            this.transform.rotation = Quaternion.Euler(0f, direction, 0f);

            //1秒経過
            if (timeCounter > 1.0f)
            {
                //タイマーリセット
                timeCounter = 0f;

                // アニメーション　前進
                //this.myanimator.SetInteger("speed", 1);

                //状態の遷移（前進）
                stateNumber = stepPattern;
            }
            //プレーヤーが近い時
            else if (length < enemyLength)
            {
                //タイマーリセット
                timeCounter = 0f;

                //アニメーション　攻撃
                this.myanimator.SetTrigger("leftP");

                //状態の遷移（攻撃）
                stateNumber = 3;
            }

        }

        else if (stateNumber == 5)
        {
            //プレーヤーの方向を向く
            this.transform.rotation = Quaternion.Euler(0f, direction, 0f);

            //1秒経過
            if (timeCounter > 1.0f)
            {
                //タイマーリセット
                timeCounter = 0f;

                // アニメーション　前進
                //this.myanimator.SetInteger("speed", 1);

                //状態の遷移（前進）
                stateNumber = stepPattern;
            }
            //プレーヤーが近い時
            else if (length < enemyLength)
            {
                //タイマーリセット
                timeCounter = 0f;

                //アニメーション　攻撃
                this.myanimator.SetTrigger("rightP");

                //状態の遷移（攻撃）
                stateNumber = 3;
            }
        }

        else if (stateNumber == 6)
        {
            //プレーヤーの方向を向く
            this.transform.rotation = Quaternion.Euler(0f, direction, 0f);

            //1秒経過
            if (timeCounter > 1.0f)
            {
                //タイマーリセット
                timeCounter = 0f;

                // アニメーション　前進
                //this.myanimator.SetInteger("speed", 1);

                //状態の遷移（前進）
                stateNumber = stepPattern;
            }
            //プレーヤーが近い時
            else if (length < enemyLength)
            {
                //タイマーリセット
                timeCounter = 0f;

                //アニメーション　攻撃
                this.myanimator.SetTrigger("leftK");

                //状態の遷移（攻撃）
                stateNumber = 3;
            }
        }

        else if (stateNumber == 7)
        {
            //プレーヤーの方向を向く
            this.transform.rotation = Quaternion.Euler(0f, direction, 0f);

            //1秒経過
            if (timeCounter > 1.0f)
            {
                //タイマーリセット
                timeCounter = 0f;

                // アニメーション　前進
                //this.myanimator.SetInteger("speed", 1);

                //状態の遷移（前進）
                stateNumber = stepPattern;
            }
            //プレーヤーが近い時
            else if (length < enemyLength)
            {
                //タイマーリセット
                timeCounter = 0f;

                //アニメーション　攻撃
                this.myanimator.SetTrigger("rightK");

                //状態の遷移（攻撃）
                stateNumber = 3;
            }
        }

        //後退
        else if (stateNumber == 8)
        {
            //プレーヤーの方向を向く
            this.transform.rotation = Quaternion.Euler(0f, direction, 0f);

            //1秒経過
            if (timeCounter > 1.0f)
            {
                //タイマーリセット
                timeCounter = 0f;

                // アニメーション　前進
                //this.myanimator.SetInteger("speed", 1);

                //状態の遷移（前進）
                stateNumber = stepPattern;
            }
            //プレーヤーが近い時
            else if (length < enemyLength)
            {
                //アニメーション　後退
                myanimator.SetInteger("speed", -1);
                //myRigidbody.velocity = transform.forward * -stepVelocity;
                if (timeCounter > 1.5f)
                {
                    timeCounter = 0f;
                    stateNumber = 3;
                }
                //タイマーリセット
                //timeCounter = 0f;
                //状態の遷移（後退）
                //stateNumber = 2;
            }
        }*/




        /*else if (stateNumber == 9)
        {
            //ダメージモーション終わり
            if (timeCounter > 0.8f)
            {
                //タイマーリセット
                timeCounter = 0f;

                //状態の遷移（待機）
                stateNumber = 0;
            }
        }*/

        //**************************************************************************************************************
        //ゲーム‐オーバー監視（追加）
        //**************************************************************************************************************

        if (playerController.isEnd)
        {
            //アニメーション　待機
            this.myanimator.SetInteger("speed", 0);

            myRigidbody.isKinematic = true;

            //ステートパターンを停止
            stateNumber = -1;
        }
    }
}
