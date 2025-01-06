using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class map
{
    public Sprite mapImage;
    public string mapName;
    public string sceneName;
    public map(Sprite _mapImage, string _mapName,string _sceneName)
    {
        mapImage = _mapImage;
        mapName = _mapName;
        sceneName = _sceneName;
    }
}

public class MapManager : MonoBehaviour
{
    public Dictionary<string, List<map>> _mapList = new Dictionary<string, List<map>>();
    List<map> list = new List<map>();
    List<map> list1 = new List<map>();
    List<map> list2 = new List<map>();
    List<map> list3 = new List<map>();

    private void Start()
    {
        list.Add(new map(Resources.Load<Sprite>("MapImage/MarketSceneImage"), "����", "GameScene0"));
        list.Add(new map(Resources.Load<Sprite>("MapImage/ColosseumSceneImage"), "�ݷμ���", "GameScene0"));
        list.Add(new map(Resources.Load<Sprite>("MapImage/CaveSceneImage"), "����", "GameScene0"));
        _mapList.Add("������ġ", list);
        list1.Add(new map(Resources.Load<Sprite>("MapImage/MarketSceneImage"), "����", "GameScene0"));
        list1.Add(new map(Resources.Load<Sprite>("MapImage/ColosseumSceneImage"), "�ݷμ���", "GameScene0"));
        list1.Add(new map(Resources.Load<Sprite>("MapImage/CaveSceneImage"), "����", "GameScene0"));
        _mapList.Add("�� ������ġ", list1);

        _mapList.Add("���潺", list2);

        _mapList.Add("���̵�", list3);
    }

}
