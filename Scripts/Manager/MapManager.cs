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
        list.Add(new map(Resources.Load<Sprite>("MapImage/MarketSceneImage"), "마켓", "GameScene0"));
        list.Add(new map(Resources.Load<Sprite>("MapImage/ColosseumSceneImage"), "콜로세움", "GameScene0"));
        list.Add(new map(Resources.Load<Sprite>("MapImage/CaveSceneImage"), "동굴", "GameScene0"));
        _mapList.Add("데스매치", list);
        list1.Add(new map(Resources.Load<Sprite>("MapImage/MarketSceneImage"), "마켓", "GameScene0"));
        list1.Add(new map(Resources.Load<Sprite>("MapImage/ColosseumSceneImage"), "콜로세움", "GameScene0"));
        list1.Add(new map(Resources.Load<Sprite>("MapImage/CaveSceneImage"), "동굴", "GameScene0"));
        _mapList.Add("팀 데스매치", list1);

        _mapList.Add("디펜스", list2);

        _mapList.Add("레이드", list3);
    }

}
