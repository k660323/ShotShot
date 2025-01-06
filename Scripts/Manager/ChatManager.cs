using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class ChatManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    PhotonView PV;

    [SerializeField]
    Scrollbar scrollbar;

    [SerializeField]
    GameObject[] chatText;
    [SerializeField]
    InputField inputField;

    bool isChat;
    bool isPiggyBank;
    float chatCountCool;
    int chatCount;

    private void Awake()
    {
        scrollbar.value = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isChat)
        {
            chatCountCool -= Time.deltaTime;
            if (chatCountCool <= 0)
            {
                isChat = false;
                chatCount = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (inputField.text != "") // ä�� �� ����
            {   
                if (chatCount < 5 && !isPiggyBank)
                    Send();
                else
                {
                    StopCoroutine(PiggyBank());
                    StartCoroutine(PiggyBank());
                }
            }
            else
            {
                ClearInputField(inputField.gameObject.activeSelf);
            }
        }
    }

    void ClearInputField(bool isActive) // ��ǲ�ʵ� Ȱ��ȭ/��Ȱ��ȭ -> ��Ȱ��ȭ/Ȱ��ȭ
    {
        inputField.text = "";
        inputField.gameObject.SetActive(!isActive);
        scrollbar.gameObject.SetActive(!isActive);

        if (isActive)
            inputField.Select(); // ��Ŀ�� o->x x->o�� ���� // �������� ��  
        else
            inputField.ActivateInputField(); // ��Ŀ�� ����         
    }

    void SetCounter()
    {
        chatCount++;
        chatCountCool = 3.0f;
        isChat = true;
    }

    #region ä��
    void Send()
    {
        string msg = PhotonNetwork.NickName + " : " + inputField.text;
       
        if(msg.Length > 19)
        {
            msg = msg.Substring(0, 19) + "\n" + msg.Substring(19, msg.Length-19);
        }
        
        PV.RPC("ChatRPC", RpcTarget.AllBufferedViaServer, msg);
        SetCounter();
        ClearInputField(inputField.gameObject.activeSelf);
    }

    [PunRPC]
    void ChatRPC(string text)
    {
        if (chatText[0].GetComponentInChildren<Text>().text != "")
        {
            for (int i = chatText.Length - 2; i >= 0; i--)
            {
                if (chatText[i].GetComponentInChildren<Text>().text == "")
                    continue;
                else
                {
                    chatText[i + 1].GetComponentInChildren<Text>().color = chatText[i].GetComponentInChildren<Text>().color;
                    // �ؽ�Ʈ�� ������ ���� �÷���
                    chatText[i + 1].GetComponentInChildren<Text>().text = chatText[i].GetComponentInChildren<Text>().text;
                }
            }
        }
        chatText[0].GetComponentInChildren<Text>().color = new Color(50f/255f, 50f/255f, 50f/255f);
        chatText[0].GetComponentInChildren<Text>().text = text;
    }
    #endregion

    #region ���

    IEnumerator PiggyBank()
    {
        ClearInputField(inputField.gameObject.activeSelf);

        if (!isPiggyBank)
            SystemChat("�ʹ� ���� �޼����� ������ �ֽ��ϴ�.\n ���߿� �ٽ� �õ����ּ���.");

        isPiggyBank = true;
        yield return new WaitForSeconds(5.0f);
        isPiggyBank = false;
    }

    void SystemChat(string text)
    {
        if (chatText[0].GetComponentInChildren<Text>().text != "")
        {
            for (int i = chatText.Length - 2; i >= 0; i--)
            {
                if (chatText[i].GetComponentInChildren<Text>().text == "")
                    continue;
                else // �ؽ�Ʈ�� ������ ���� �÷���
                {
                    chatText[i + 1].GetComponentInChildren<Text>().color = chatText[i].GetComponentInChildren<Text>().color;
                    chatText[i + 1].GetComponentInChildren<Text>().text = chatText[i].GetComponentInChildren<Text>().text;
                }
            }
        }
        chatText[0].GetComponentInChildren<Text>().color = Color.red;
        chatText[0].GetComponentInChildren<Text>().text = text;
    }
    #endregion
}
