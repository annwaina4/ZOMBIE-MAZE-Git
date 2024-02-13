using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class playerController : MonoBehaviour
{
    Animator myanimator;

    Rigidbody myRigidbody;

    //�i�ޑ��x
    private float stepVelocity = 2.3f;

    //HP�ݒ�
    private float maxHP = 3f;    
    private float nowHP;
    public Slider slider;
    
    static public bool isEnd = false;

    public GameObject effectPrefab;

    //private Vector3 previousPosition;

    //------------------------------------------------------------------------------------------------------------------
    //�X�^�[�g
    //------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        Application.targetFrameRate = 60;

        this.myanimator = GetComponent<Animator>();

        this.myRigidbody = GetComponent<Rigidbody>();

        //Slider���ő�ɂ���
        slider.value = 1;
        nowHP = maxHP;
    }

    //------------------------------------------------------------------------------------------------------------------
    //�A�b�v�f�[�g
    //------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        if (isEnd == false)
        {
            //**********************************************************************
            //�ړ��ƒ�~
            //**********************************************************************
            
            //�E�ֈړ�
            if (Input.GetKey(KeyCode.RightArrow))
            {
                myRigidbody.velocity = new Vector3(stepVelocity, myRigidbody.velocity.y, 0f);
                
                //�A�j���[�V�����O�i
                myanimator.SetInteger("speed", 1);

                //�E������
                transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
            }

            //���ֈړ�
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                myRigidbody.velocity = new Vector3(-stepVelocity, myRigidbody.velocity.y, 0f);
                
                //�A�j���[�V�����O�i
                myanimator.SetInteger("speed", 1);
                
                //��������
                transform.rotation = Quaternion.Euler(0.0f, 270.0f, 0.0f);
            }

            //���ֈړ�
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                myRigidbody.velocity = new Vector3(0f, myRigidbody.velocity.y, stepVelocity);

                //�A�j���[�V�����O�i
                myanimator.SetInteger("speed", 1);

                //��������
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            }

            //��O�ֈړ�
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                myRigidbody.velocity = new Vector3(0f, myRigidbody.velocity.y, -stepVelocity);

                //�A�j���[�V�����O�i
                myanimator.SetInteger("speed", 1);

                //��O������
                transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            }

            //��~
            else
            {
                myRigidbody.velocity = new Vector3(0f, myRigidbody.velocity.y, 0f);

                //�A�j���[�V�����ҋ@
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
            //HP�[�����̏���
            //**********************************************************************

            if (nowHP <= 0)
            {
                //�A�j���[�V�������S
                myanimator.SetBool("die", true);

                isEnd = true;

                //�Q�[���I�[�o�[�\��
                GameObject.Find("Canvas").GetComponent<UIController>().gameOver();
            }
        }

        //�A�C�e����S�Ċl����
        if(GameObject.Find("Canvas").GetComponent<UIController>().itemCounter==7)
        {
            isEnd = true;
        }

        if (isEnd)
        {
            //�A�j���[�V�����ҋ@
            myanimator.SetInteger("speed", 0);

            myRigidbody.isKinematic = true;

            //�^�C�g���֖߂�
            if (Input.GetKeyDown(KeyCode.Return))
            {
                isEnd = false;
                SceneManager.LoadScene("title");
            }
        }
    }

    //------------------------------------------------------------------------------------------------------------------
    //�Փ˔���
    //------------------------------------------------------------------------------------------------------------------
    private void OnCollisionEnter(Collision other)
    {
        if (isEnd == false)
        {
            //�G�̍U�����󂯂����̏���
            if (other.gameObject.tag == "enemy" && nowHP > 0)
            {
                //�_���[�W�A�j���[�V����
                myanimator.SetTrigger("damage");

                nowHP -= 1f;

                GetComponent<AudioSource>().Play();

                //HP��Slider�ɔ��f
                slider.value = (float)nowHP / (float)maxHP;
            }            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isEnd == false)
        {
            //�A�C�e���l�����̏���
            if (other.gameObject.tag == "food")
            {
                Destroy(other.gameObject, 0.1f);

                //�l���G�t�F�N�g����
                Instantiate(effectPrefab, other.gameObject.transform.position, Quaternion.identity);
                
                //�l�����\��
                GameObject.Find("Canvas").GetComponent<UIController>().itemCounter++;
                GameObject.Find("Canvas").GetComponent<UIController>().Addscore();

                //�l�����̔���
                GameObject.Find("ItemBGM").GetComponent<AudioSource>().Play();
            }
        }
    }
}
