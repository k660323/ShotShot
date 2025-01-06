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
            if (inputField.text != "") // 채팅 글 존재
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

    void ClearInputField(bool isActive) // 인풋필드 활성화/비활성화 -> 비활성화/활성화
    {
        inputField.text = "";
        inputField.gameObject.SetActive(!isActive);
        scrollbar.gameObject.SetActive(!isActive);

        if (isActive)
            inputField.Select(); // 포커스 o->x x->o로 해줌 // 꺼줌으로 씀  
        else
            inputField.ActivateInputField(); // 포커스 켜줌         
    }

    void SetCounter()
    {
        chatCount++;
        chatCountCool = 3.0f;
        isChat = true;
    }

    #region 채팅
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
                    // 텍스트가 있으면 위로 올려줌
                    chatText[i + 1].GetComponentInChildren<Text>().text = chatText[i].GetComponentInChildren<Text>().text;
                }
            }
        }
        chatText[0].GetComponentInChildren<Text>().color = new Color(50f/255f, 50f/255f, 50f/255f);
        chatText[0].GetComponentInChildren<Text>().text = text;
    }
    #endregion

    #region 벙어리

    IEnumerator PiggyBank()
    {
        ClearInputField(inputField.gameObject.activeSelf);

        if (!isPiggyBank)
            SystemChat("너무 많은 메세지를 보내고 있습니다.\n 나중에 다시 시도해주세요.");

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
                else // 텍스트가 있으면 위로 올려줌
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
