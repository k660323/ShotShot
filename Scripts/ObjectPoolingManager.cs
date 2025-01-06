using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObjectPoolingManager : MonoBehaviourPunCallbacks
{
    // »ç¿îµå 3d
    public int sound3DPrefabCount;
    public GameObject sound3DPrefab;
    public Queue<GameObject> sound3DQueue = new Queue<GameObject>();

    // ÆøÅº
  //  public int bombPrefabCount;
  //  public GameObject bombPrefab;
    public Queue<GameObject> bombQueue = new Queue<GameObject>();

    // ¸¶ÀÎ
  //  public int minePrefabCount;
  //  public GameObject minePrefab;
    public Queue<GameObject> mineQueue = new Queue<GameObject>();

    // ÃÑ¾Ë
    //  public int bulletPrefabCount;
    //  public GameObject bulletPrefab;
    public Queue<GameObject> bulletQueue = new Queue<GameObject>();

    // ÃÑ±¸ È­¿°
    public int gunEffectPrefabCount;
    public GameObject gunEffectPrefab;
    public Queue<GameObject> gunEffectQueue = new Queue<GameObject>();

    // È­»ìºñ
    public Queue<GameObject> arrowReinQueue = new Queue<GameObject>();
    public GameObject arrowReinPrefab;

    private static ObjectPoolingManager instance;
    public static ObjectPoolingManager Instance
    {
        get
        {
            if(instance == null)
            {
                var obj = FindObjectOfType<ObjectPoolingManager>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    var newObj = new GameObject().AddComponent<ObjectPoolingManager>();
                    instance = newObj;
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        var objs = FindObjectsOfType<ObjectPoolingManager>();
        if(objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }

        // »ç¿îµå
        for (int i = 0; i < sound3DPrefabCount; i++)
        {
            InsertSQueue(Instantiate(sound3DPrefab, Vector3.zero, Quaternion.identity));
        }


        // ÃÑ±¸È­¿°
        for (int i = 0; i < gunEffectPrefabCount; i++)
        {
            InsertGunEffectQueue(Instantiate(gunEffectPrefab, Vector3.zero, Quaternion.identity));
        }
    }

    #region »ç¿îµå Å¥
    public void InsertSQueue(GameObject s_object)
    {
        sound3DQueue.Enqueue(s_object);
        s_object.SetActive(false);
    }

    public GameObject GetSQueue()
    {
        if(sound3DQueue.Count == 0)
        {
            for (int i = 0; i < sound3DPrefabCount / 2; i++)
            {
               InsertSQueue(Instantiate(sound3DPrefab, Vector3.zero, Quaternion.identity));
            }
        }
        return sound3DQueue.Dequeue();
    }
    #endregion

    #region ÆøÅº Å¥ 

   public void InsertBombQueue(GameObject b_object)
    {
        if (b_object.GetComponent<PhotonView>().IsMine)
            bombQueue.Enqueue(b_object);
        b_object.SetActive(false);
    }

    public GameObject GetBombQueue()
    {
        if(bombQueue.Count == 0)
        {
            InsertBombQueue(PhotonNetwork.Instantiate("Weapon/Bomb", Vector3.zero, Quaternion.identity));
        }

        return bombQueue.Dequeue();
    }
    #endregion

    #region Áö·Ú Å¥ 

    public void InsertMineQueue(GameObject m_object)
    {
        if (m_object.GetComponent<PhotonView>().IsMine)
            mineQueue.Enqueue(m_object);
        m_object.SetActive(false);
    }

    public GameObject GetMineQueue()
    {
        if (mineQueue.Count == 0)
        {
            InsertMineQueue(PhotonNetwork.Instantiate("Weapon/LandMine", Vector3.zero, Quaternion.identity));
        }

        return mineQueue.Dequeue();
    }
    #endregion

    #region ÃÑ¾Ë Å¥
    public void InsertBulletQueue(GameObject b_object)
    {
        b_object.SetActive(false);
        if (b_object.GetComponent<PhotonView>().IsMine)
            bulletQueue.Enqueue(b_object);
    }

    public GameObject GetBulletQueue()
    {
        if (bulletQueue.Count == 0)
        {
            InsertBulletQueue(PhotonNetwork.Instantiate("Weapon/" + CharacterScript.Instance.Gun.bullet.name, Vector3.zero, Quaternion.identity));
        }

        return bulletQueue.Dequeue();
    }
    #endregion

    #region ÃÑ±¸ È­¿° Å¥
    public void InsertGunEffectQueue(GameObject b_object)
    {
        gunEffectQueue.Enqueue(b_object);
        b_object.SetActive(false);
    }

    public GameObject GetGunEffectQueue()
    {
        if (gunEffectQueue.Count == 0)
        {
            for (int i = 0; i < gunEffectPrefabCount / 2; i++)
            {
                InsertGunEffectQueue(Instantiate(gunEffectPrefab, Vector3.zero, Quaternion.identity));
            }
        }

        return gunEffectQueue.Dequeue();
    }
    #endregion

    #region È­»ìºñ Å¥
    public void InsertArrowReinQueue(GameObject ar_object)
    {
        if (ar_object.GetComponent<PhotonView>().IsMine)
            arrowReinQueue.Enqueue(ar_object);
        ar_object.SetActive(false);
    }

    public GameObject GetArrowReinQueue()
    {
        if (arrowReinQueue.Count == 0)
        {
            InsertArrowReinQueue(PhotonNetwork.Instantiate("Weapon/ArrowRein", Vector3.zero, arrowReinPrefab.transform.rotation));
        }

        return arrowReinQueue.Dequeue();
    }

    #endregion
}
