using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NavMesh
using UnityEngine.AI;

public class zombieController : MonoBehaviour
{
    //��ԁi�X�e�[�g�p�^�[���j
    private int stateNumber = 0;
    //�ėp�^�C�}�[
    private float timeCounter = 0f;

    private Animator myanimator;

    private GameObject player;

    private Rigidbody myRigidbody;
    
    public GameObject attackPrefab;
    
    private GameObject attackPoint;
    
    public GameObject effectPrefab;

    private NavMeshAgent nav;

    //�n�_�i�e�I�u�W�F�N�g�j
    public GameObject navPoints;

    //�U���J�n�̊ԍ���
    private float enemyLength = 2.0f;
    
    /*
    //�z��Ń����_���Ɏw�肷�� �C���X�y�N�^�[�Ō���ݒ肷��
    //public GameObject[] navRandomPoints = new GameObject[7];
    //GetComponent<NavMeshAgent>().destination = navRandomPoints[0].transform.position;
    */

    //�I�u�W�F�N�g��
    private int number = 0;

    //------------------------------------------------------------------------------------------------------------------
    //�X�^�[�g
    //------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        this.myanimator = GetComponent<Animator>();

        this.myRigidbody = GetComponent<Rigidbody>();

        this.nav = GetComponent<NavMeshAgent>();

        this.player = GameObject.Find("player");

        //�i�r�n�_�̎q�I�u�W�F�N�g�̐�
        number = navPoints.GetComponentInChildren<Transform>().childCount;

        int randomStart = Random.Range(0, number);

        attackPoint = transform.Find("attackPoint").gameObject;

        //�O�i�A�j���[�V����
        myanimator.SetInteger("speed", 1);
                
        //���̖ڕW�n�_��
        nav.destination = navPoints.transform.GetChild(randomStart).transform.position;
    }

    //------------------------------------------------------------------------------------------------------------------
    //�����ƕ��������߂�֐�
    //------------------------------------------------------------------------------------------------------------------

    //���������߂�
    float getLength(Vector3 current, Vector3 target)
    {
        return Mathf.Sqrt(((current.x - target.x) * (current.x - target.x)) + ((current.z - target.z) * (current.z - target.z)));
    }

    //���������߂� ���I�C���[�i-180�`0�`+180)
    float getEulerAngle(Vector3 current, Vector3 target)
    {
        Vector3 value = target - current;
        return Mathf.Atan2(value.x, value.z) * Mathf.Rad2Deg; //���W�A�����I�C���[
    }

    //���������߂� �����W�A��
    float getRadian(Vector3 current, Vector3 target)
    {
        Vector3 value = target - current;
        return Mathf.Atan2(value.x, value.z);
    }

    //------------------------------------------------------------------------------------------------------------------
    //�A�b�v�f�[�g
    //------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        if (playerController.isEnd == false)
        {
            //�^�C�}�[���Z
            timeCounter += Time.deltaTime;
            //���������߂�
            float direction = getEulerAngle(this.transform.position, player.transform.position);
            //���������߂�
            float length = getLength(this.transform.position, player.transform.position);

            //**************************************************************************************************************
            //���������ԏ���
            //**************************************************************************************************************
            int randomNav = Random.Range(0, number);

            //�ڕW�n�_�̐؊���
            if (stateNumber == 0)
            {
                if (length > enemyLength && nav.remainingDistance < 0.25f)
                {
                    //���̖ڕW�n�_��
                    nav.destination = navPoints.transform.GetChild(randomNav).transform.position;
                }

                if(Mathf.Abs(Mathf.DeltaAngle(direction,transform.eulerAngles.y))<30f)
                {
                    //�v���[���[���߂���
                    if (length < enemyLength)
                    {
                        //�^�C�}�[���Z�b�g
                        timeCounter = 0f;

                        //�i�r��~
                        nav.isStopped = true;

                        //�A�j���[�V�����U��
                        myanimator.SetTrigger("attack");

                        //��Ԃ̑J�ځi�؂�ւ��X�e�[�g�j
                        stateNumber = 2;
                    }
                }               
            }

            //�i�r�̍ĊJ
            if (stateNumber == 1)
            {
                //�v���C���[�����ꂽ��
                if (length > enemyLength)
                {
                    //�i�r�ĊJ
                    nav.isStopped = false;

                    //�A�j���[�V�����O�i
                    myanimator.SetInteger("speed", 1);

                    //���̖ڕW�n�_��
                    nav.destination = navPoints.transform.GetChild(randomNav).transform.position;

                    //��Ԃ̑J�ځi�ڕW�n�_�̐؊����j
                    stateNumber = 0;
                }

                if (Mathf.Abs(Mathf.DeltaAngle(direction, transform.eulerAngles.y)) < 30f)
                {
                    //�v���[���[���߂���
                    if (length < enemyLength)
                    {
                        //�^�C�}�[���Z�b�g
                        timeCounter = 0f;

                        //�i�r��~
                        nav.isStopped = true;

                        //�A�j���[�V�����U��
                        myanimator.SetTrigger("attack");

                        //��Ԃ̑J�ځi�؂�ւ��X�e�[�g�j
                        stateNumber = 2;
                    }
                }                    
            }

            //���[�V�����؂�ւ�
            else if (stateNumber == 2)
            {
                //�A�j���[�V�����ҋ@
                myanimator.SetInteger("speed", 0);

                //���[�V�����I���
                if (timeCounter > 1.0f)
                {
                    //�^�C�}�[���Z�b�g
                    timeCounter = 0f;
                    //��Ԃ̑J�ځi�i�r�̍ĊJ�j
                    stateNumber = 1;
                }
            }
        }

        //**************************************************************************************************************
        //�Q�[���I������
        //**************************************************************************************************************

        if (playerController.isEnd)
        {
            //�i�r��~
            nav.isStopped = true;

            //�A�j���[�V�����@�ҋ@
            this.myanimator.SetInteger("speed", 0);
            
            //�X�e�[�g�p�^�[�����~
            stateNumber = -1;

            //���R�������~
            myRigidbody.useGravity = false;

            //�Փ˂��Ȃ���
            GetComponent<CapsuleCollider>().enabled = false;

            //myRigidbody.velocity = Vector3.zero;
            //myRigidbody.isKinematic = true;
        }
    }

    //------------------------------------------------------------------------------------------------------------------
    //�U���C�x���g
    //------------------------------------------------------------------------------------------------------------------
    void attackEvent()
    {
        GameObject attack=Instantiate(attackPrefab, attackPoint.transform.position, Quaternion.identity);
        Destroy(attack.gameObject, 0.35f);

        //�G�t�F�N�g����
        Instantiate(effectPrefab, attackPoint.transform.position, Quaternion.identity);
    }
}
