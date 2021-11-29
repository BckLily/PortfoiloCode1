using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{
    public GameObject storeCanvas;
l    public bool isUsed;

    void Start()
    {
        isUsed = false;
    }

    /// <summary>
    /// 플레이어가 상점과 상호작용을 통해서 상점을 열었을 때 동작할 함수.
    /// </summary>
    public bool OpenStore(Transform _playerTr)
    {
        if (isUsed == true) { return false; }
        isUsed = true;
        storeCanvas.SetActive(true);
        storeCanvas.transform.SetParent(_playerTr);
        _playerTr.GetComponent<PlayerCtrl>().isUIOpen = true;

        StoreCtrl storeCtrl = storeCanvas.GetComponent<StoreCtrl>();
        storeCtrl.PlayerPointSetting();

        Canvas _canvas = storeCtrl.GetComponent<Canvas>();
        _canvas.worldCamera = _playerTr.Find("UICamera").GetComponent<Camera>();
        _canvas.planeDistance = 0.0105f;

        return true;
    }

    /// <summary>
    /// 플레이어가 상점을 닫았을 때 동작할 함수.
    /// </summary>
    public void CloseStore(Transform _playerTr)
    {
        storeCanvas.SetActive(false);
        isUsed = false;
        _playerTr.GetComponent<PlayerCtrl>().isUIOpen = false;
    }
}
