using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NavMesh
using UnityEngine.AI;

public class batMaleController : MonoBehaviour
{
    //��ԁi�X�e�[�g�p�^�[���j
    private int stateNumber = 0;
    //�ėp�^�C�}�[
    private float timeCounter = 0f;
    private Animator myanimator;
    private float enemyLength = 2.0f;
    private GameObject player;

    private Rigidbody myRigidbody;
    private NavMeshAgent nav;
    //�n�_�i�e�I�u�W�F�N�g�j
    public GameObject navPoints;

    //�z��Ń����_���Ɏw�肷����@ ���C���X�y�N�^�[�Ō���ݒ肷��
    public GameObject[] navRandomPoints = new GameObject[7];
    //��: GetComponent<NavMeshAgent>().destination = navRandomPoints[0].transform.position;

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

        //�q�I�u�W�F�N�g�̐�
        number = navPoints.GetComponentInChildren<Transform>().childCount;

        int randomStart = Random.Range(0, 7);

        if (number > 0)
        {
            //�A�j���[�V����
            myanimator.SetInteger("speed", 1);
            //���̖ڕW�n�_��
            nav.destination = navRandomPoints[randomStart].transform.position;

        }

        //�ŏ��̖ڕW�n�_ �������`�F�b�N
        /*if (number > 0)
        {
            nav.destination = navPoints.transform.GetChild(0).transform.position;
        } */
    }

    //------------------------------------------------------------------------------------------------------------------
    //�I���W�i���֐�
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
        //Debug.Log("���݂�statenumber" + stateNumber);        
        //�^�C�}�[���Z
        timeCounter += Time.deltaTime;
        //���������߂�
        float direction = getEulerAngle(this.transform.position, player.transform.position);
        //���������߂�
        float length = getLength(this.transform.position, player.transform.position);

        //**************************************************************************************************************
        //���������ԏ���
        //**************************************************************************************************************
        int randomNav = Random.Range(0, 7);
        //int stepPattern = Random.Range(1, 3);
        //int enemyPattern = Random.Range(4, 9);

        //�ڕW�n�_�܂ōs�����H ���u���O�ł悭�����������
        //if (GetComponent<NavMeshAgent>().remainingDistance == 0.0f)
        //�҂�����0.0f�ɂȂ�Ȃ����Ƃ�����̂ŁA0.5f�ȉ����ǂ�����


        //�ҋ@
        if (stateNumber == 0)
        {
            if (length > enemyLength&&nav.remainingDistance < 0.25f)
            {
                //���̖ڕW�n�_ �������`�F�b�N
                if (number > 0)
                {
                    //myanimator.SetInteger("speed", 1);
                    //���̖ڕW�n�_��
                    nav.destination = navRandomPoints[randomNav].transform.position;
                }
            }

            //�v���[���[���߂���
            if (length < enemyLength)
            {
                //�^�C�}�[���Z�b�g
                timeCounter = 0f;

                //�A�j���[�V�����@�U��
                nav.isStopped = true;

                myanimator.SetTrigger("attack");
                //myanimator.SetInteger("speed", 0);

                //Debug.Log("�U��: " + Time.time);

                //��Ԃ̑J�ځi�؂�ւ��X�e�[�g�j
                stateNumber = 2;
            }
        }

        if (stateNumber == 1)
        {
            //���̖ڕW�n�_ �������`�F�b�N
            if (length > enemyLength&&number > 0)
            {
                nav.isStopped = false;
                myanimator.SetInteger("speed", 1);
                //���̖ڕW�n�_��
                nav.destination = navRandomPoints[randomNav].transform.position;
                stateNumber = 0;

            }

            //�v���[���[���߂���
            if (length < enemyLength)
            {
                //�^�C�}�[���Z�b�g
                timeCounter = 0f;

                //�A�j���[�V�����@�U��
                nav.isStopped = true;

                myanimator.SetTrigger("attack");
                //myanimator.SetInteger("speed", 0);

                //Debug.Log("�U��: " + Time.time);

                //��Ԃ̑J�ځi�؂�ւ��X�e�[�g�j
                stateNumber = 2;
            }
        }

        //���[�V�����؂�ւ�
        else if (stateNumber == 2)
        {
            //myanimator.SetTrigger("attack");
            myanimator.SetInteger("speed", 0);

            //���[�V�����I���
            if (timeCounter > 1.0f)
            {
                //�^�C�}�[���Z�b�g
                timeCounter = 0f;
                //��Ԃ̑J�ځi�ҋ@�j
                stateNumber = 1;
            }
        }

        /*
        //���
        else if (stateNumber == 2)
        {
            //�v���[���[�̕���������
            this.transform.rotation = Quaternion.Euler(0f, direction, 0f);
            myanimator.SetInteger("speed", -1);
            //�ړ�
            //myRigidbody.velocity = transform.forward * -stepVelocity;
            //myRigidbody.AddForce(transform.forward * stepForce);

            //5�b�o��
            if (timeCounter > 0.5f)
            {
                timeCounter = 0f;

                //�A�j���[�V�����@�ҋ@
                this.myanimator.SetInteger("speed", 0);

                //��Ԃ̑J�ځi�ҋ@�j
                stateNumber = 0;
            }
            //�v���[���[���߂���
            else if (length < enemyLength)
            {
                //�^�C�}�[���Z�b�g
                timeCounter = 0f;

                //�A�j���[�V�����@�ҋ@
                //this.myanimator.SetInteger("speed", 0);

                //�A�j���[�V�����@�U��
                this.myanimator.SetTrigger("rightK");

                //��Ԃ̑J�ځi�U��)
                stateNumber = 3;
            }
        }

        //���[�V�����؂�ւ�
        else if (stateNumber == 3)
        {
            //���[�V�����I���
            if (timeCounter > 0.2f)
            {
                //�^�C�}�[���Z�b�g
                timeCounter = 0f;

                //��Ԃ̑J�ځi�ҋ@�j
                stateNumber = enemyPattern;
            }
        }

        else if (stateNumber == 4)
        {
            //�v���[���[�̕���������
            this.transform.rotation = Quaternion.Euler(0f, direction, 0f);

            //1�b�o��
            if (timeCounter > 1.0f)
            {
                //�^�C�}�[���Z�b�g
                timeCounter = 0f;

                // �A�j���[�V�����@�O�i
                //this.myanimator.SetInteger("speed", 1);

                //��Ԃ̑J�ځi�O�i�j
                stateNumber = stepPattern;
            }
            //�v���[���[���߂���
            else if (length < enemyLength)
            {
                //�^�C�}�[���Z�b�g
                timeCounter = 0f;

                //�A�j���[�V�����@�U��
                this.myanimator.SetTrigger("leftP");

                //��Ԃ̑J�ځi�U���j
                stateNumber = 3;
            }

        }

        else if (stateNumber == 5)
        {
            //�v���[���[�̕���������
            this.transform.rotation = Quaternion.Euler(0f, direction, 0f);

            //1�b�o��
            if (timeCounter > 1.0f)
            {
                //�^�C�}�[���Z�b�g
                timeCounter = 0f;

                // �A�j���[�V�����@�O�i
                //this.myanimator.SetInteger("speed", 1);

                //��Ԃ̑J�ځi�O�i�j
                stateNumber = stepPattern;
            }
            //�v���[���[���߂���
            else if (length < enemyLength)
            {
                //�^�C�}�[���Z�b�g
                timeCounter = 0f;

                //�A�j���[�V�����@�U��
                this.myanimator.SetTrigger("rightP");

                //��Ԃ̑J�ځi�U���j
                stateNumber = 3;
            }
        }

        else if (stateNumber == 6)
        {
            //�v���[���[�̕���������
            this.transform.rotation = Quaternion.Euler(0f, direction, 0f);

            //1�b�o��
            if (timeCounter > 1.0f)
            {
                //�^�C�}�[���Z�b�g
                timeCounter = 0f;

                // �A�j���[�V�����@�O�i
                //this.myanimator.SetInteger("speed", 1);

                //��Ԃ̑J�ځi�O�i�j
                stateNumber = stepPattern;
            }
            //�v���[���[���߂���
            else if (length < enemyLength)
            {
                //�^�C�}�[���Z�b�g
                timeCounter = 0f;

                //�A�j���[�V�����@�U��
                this.myanimator.SetTrigger("leftK");

                //��Ԃ̑J�ځi�U���j
                stateNumber = 3;
            }
        }

        else if (stateNumber == 7)
        {
            //�v���[���[�̕���������
            this.transform.rotation = Quaternion.Euler(0f, direction, 0f);

            //1�b�o��
            if (timeCounter > 1.0f)
            {
                //�^�C�}�[���Z�b�g
                timeCounter = 0f;

                // �A�j���[�V�����@�O�i
                //this.myanimator.SetInteger("speed", 1);

                //��Ԃ̑J�ځi�O�i�j
                stateNumber = stepPattern;
            }
            //�v���[���[���߂���
            else if (length < enemyLength)
            {
                //�^�C�}�[���Z�b�g
                timeCounter = 0f;

                //�A�j���[�V�����@�U��
                this.myanimator.SetTrigger("rightK");

                //��Ԃ̑J�ځi�U���j
                stateNumber = 3;
            }
        }

        //���
        else if (stateNumber == 8)
        {
            //�v���[���[�̕���������
            this.transform.rotation = Quaternion.Euler(0f, direction, 0f);

            //1�b�o��
            if (timeCounter > 1.0f)
            {
                //�^�C�}�[���Z�b�g
                timeCounter = 0f;

                // �A�j���[�V�����@�O�i
                //this.myanimator.SetInteger("speed", 1);

                //��Ԃ̑J�ځi�O�i�j
                stateNumber = stepPattern;
            }
            //�v���[���[���߂���
            else if (length < enemyLength)
            {
                //�A�j���[�V�����@���
                myanimator.SetInteger("speed", -1);
                //myRigidbody.velocity = transform.forward * -stepVelocity;
                if (timeCounter > 1.5f)
                {
                    timeCounter = 0f;
                    stateNumber = 3;
                }
                //�^�C�}�[���Z�b�g
                //timeCounter = 0f;
                //��Ԃ̑J�ځi��ށj
                //stateNumber = 2;
            }
        }*/




        /*else if (stateNumber == 9)
        {
            //�_���[�W���[�V�����I���
            if (timeCounter > 0.8f)
            {
                //�^�C�}�[���Z�b�g
                timeCounter = 0f;

                //��Ԃ̑J�ځi�ҋ@�j
                stateNumber = 0;
            }
        }*/

        //**************************************************************************************************************
        //�Q�[���]�I�[�o�[�Ď��i�ǉ��j
        //**************************************************************************************************************

        if (playerController.isEnd)
        {
            //�A�j���[�V�����@�ҋ@
            this.myanimator.SetInteger("speed", 0);

            myRigidbody.isKinematic = true;

            //�X�e�[�g�p�^�[�����~
            stateNumber = -1;
        }
    }
}
