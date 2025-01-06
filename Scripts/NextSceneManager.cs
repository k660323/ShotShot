using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NextSceneManager : MonoBehaviourPunCallbacks
{
    public static List<bool> _loadOk = new List<bool>();
    bool _allLoad;
    [SerializeField]
    Image progressBar;
    [SerializeField]
    PhotonView pv;
    [SerializeField]
    Text mapText;
    [SerializeField]
    Text modeText;
    [SerializeField]
    Image mapImage;
    [SerializeField]
    Text TipText;
    [SerializeField]
    string[] text;

    AsyncOperation a;
    IEnumerator loadScene;
    public static void SceneLoad()
    {
        PhotonNetwork.LoadLevel("LoadingScene");
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    // Start is called before the first frame update
    void Awake()
    {
        MapInfoInit();
    }

    void MapInfoInit()
    {
        modeText.text = SessionInfo.Instance.mode;
        mapText.text = SessionInfo.Instance.mapName;
        mapImage.sprite = SessionInfo.Instance.mapImage;
        TipText.text = text[Random.Range(0, text.Length)];
        a = SceneManager.LoadSceneAsync(SessionInfo.Instance.nowScene);
        a.allowSceneActivation = false; // 로딩 다하면 기다림 

        loadScene = LoadSceneProgress(a);
        StartCoroutine(loadScene);
    }

    IEnumerator LoadSceneProgress(AsyncOperation a)
    {
        float timer = 0f;
        while (!a.isDone)
        {
            yield return null;

            if(a.progress < 0.9f)
            {
                progressBar.fillAmount = a.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                if(progressBar.fillAmount >= 1f)
                {
                    pv.RPC("LoadOkay", RpcTarget.AllBufferedViaServer, true);
                    if (PhotonNetwork.IsMasterClient)
                    {
                        while (_loadOk.Count < PhotonNetwork.CurrentRoom.PlayerCount)
                        {
                            yield return null;
                        }
                        pv.RPC("AllLoad", RpcTarget.AllBufferedViaServer);
                    }
                    else
                    {
                        while (!_allLoad)
                        {
                            yield return null;
                        }
                    }
                    yield return new WaitForSeconds(3.0f);
                    a.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }

    [PunRPC]
    public void LoadOkay(bool ok)
    {
        _loadOk.Add(ok);
    }

    [PunRPC]
    public void AllLoad()
    {
        _allLoad = true;
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if(loadScene != null)
        {
            StopCoroutine(loadScene);
            loadScene = null;
        }
        loadScene = LoadSceneProgress(a);
        StartCoroutine(loadScene);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (loadScene != null)
        {
            StopCoroutine(loadScene);
            loadScene = null;
        }
        loadScene = LoadSceneProgress(a);
        StartCoroutine(loadScene);
    }
}
