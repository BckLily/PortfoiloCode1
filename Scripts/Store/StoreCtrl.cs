using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StoreCtrl : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Transform originParent;

    GameObject _preButton;
    Color _preColor;

    #region Button List
    public List<MyButton> typeButtonList;
    public List<UnityEngine.UI.Image> typePanelList;

    public List<MyButton> weaponTypeButtonList;
    public List<UnityEngine.UI.Image> weaponTypePanelList;
    public List<MyButton> rifleList;
    public List<MyButton> smgList;
    public List<MyButton> sgList;

    public List<MyButton> itemTypeButtonList;
    public List<UnityEngine.UI.Image> itemTypePanelList;
    public List<MyButton> itemList;
    public List<MyButton> perkList;

    public List<MyButton> defStructTypeButtonList;
    public List<UnityEngine.UI.Image> defStructTypePanelList;
    public List<MyButton> defStructList;
    #endregion

    #region Text UI
    [Header("Text UI")]
    public UnityEngine.UI.Text infoText;
    public UnityEngine.UI.Text _playerPointText;
    #endregion

    void Start()
    {
        _preColor = new Color();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnStoreCloseBtn();
        }
    }

    private void OnEnable()
    {
        originParent = transform.parent.transform;
        CursorState.CursorLockedSetting(false);
    }

    public void PlayerPointSetting()
    {
        PlayerCtrl _playerCtrl = transform.parent.GetComponent<PlayerCtrl>();
        _playerPointText.text = string.Format($"보유 포인트: {_playerCtrl._point}");
    }

    /// <summary>
    /// 상점을 닫는 버튼을 눌렀을 때 동작하는 함수
    /// </summary>
    public void OnStoreCloseBtn()
    {
        Transform playerTr = transform.parent.GetComponent<Transform>();
        transform.SetParent(originParent);
        originParent.GetComponent<Store>().CloseStore(playerTr);
        CursorState.CursorLockedSetting(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        MyButton _myButton = eventData.pointerCurrentRaycast.gameObject.GetComponent<MyButton>();

#if UNITY_EDITOR
        Debug.Log("____ On Pointer Click: " + eventData.pointerCurrentRaycast.gameObject.name + " ____");
#endif

        switch (_myButton.buttonType)
        {
            case StoreButtonType.ButtonType.WeaponType:
                TypePanelActive(0);
                break;
            case StoreButtonType.ButtonType.DefStructType:
                TypePanelActive(1);
                break;
            case StoreButtonType.ButtonType.ItemType:
                TypePanelActive(2);
                break;
            case StoreButtonType.ButtonType.RifleList:
                WeaponPanelActive(0);
                break;
            case StoreButtonType.ButtonType.SMGList:
                WeaponPanelActive(1);
                break;
            case StoreButtonType.ButtonType.SGList:
                WeaponPanelActive(2);
                break;
            case StoreButtonType.ButtonType.DefStructList:
                DefStructPanelActive(0);
                break;
            case StoreButtonType.ButtonType.ItemList:
                ItemPanelActive(0);
                break;
            case StoreButtonType.ButtonType.PerkList:
                ItemPanelActive(1);
                break;
            case StoreButtonType.ButtonType.BuyButton:
                StartCoroutine(CoCheckSuccessBuy(BuyItem(_myButton.GetComponent<MyBuyButton>())));
                break;
            default:

                break;
        }
    }

    IEnumerator CoCheckSuccessBuy(bool _success)
    {
        GameObject _checkObject = _preButton;
        Color _preCheckColor = _preColor;
        if (!_success)
            _checkObject.GetComponent<UnityEngine.UI.Image>().color = Color.red;
        else
            _checkObject.GetComponent<UnityEngine.UI.Image>().color = Color.blue;
        yield return new WaitForSeconds(0.1f);
        _checkObject.GetComponent<UnityEngine.UI.Image>().color = _preCheckColor;
    }


    #region Panel Active 모음
    private void TypePanelActive(int _idx)
    {
        for (int i = 0; i < typePanelList.Count; i++)
        {
            if (i == _idx)
            {
                typePanelList[i].gameObject.SetActive(true);
            }
            else
            {
                typePanelList[i].gameObject.SetActive(false);
            }
        }
    }

    private void WeaponPanelActive(int _idx)
    {

        for (int i = 0; i < weaponTypePanelList.Count; i++)
        {
            if (i == _idx)
            {
                weaponTypePanelList[i].gameObject.SetActive(true);
            }
            else
            {
                weaponTypePanelList[i].gameObject.SetActive(false);
            }
        }
    }

    private void DefStructPanelActive(int _idx)
    {
        for (int i = 0; i < defStructTypePanelList.Count; i++)
        {
            if (i == _idx)
            {
                defStructTypePanelList[i].gameObject.SetActive(true);
            }
            else
            {
                defStructTypePanelList[i].gameObject.SetActive(false);
            }
        }
    }

    private void ItemPanelActive(int _idx)
    {
        for (int i = 0; i < itemTypePanelList.Count; i++)
        {
            if (i == _idx)
            {
                itemTypePanelList[i].gameObject.SetActive(true);
            }
            else
            {
                itemTypePanelList[i].gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region Information Text 설정

    public void OnPointerEnter(PointerEventData eventData)
    {
#if UNITY_EDITOR
        //Debug.Log("___ Pointer Enter: " + eventData.pointerCurrentRaycast.gameObject.name + " ____");
#endif
        try
        {
            _preButton = eventData.pointerCurrentRaycast.gameObject;
            UnityEngine.UI.Image _image = _preButton.GetComponent<UnityEngine.UI.Image>();
            _preColor = _image.color;
            _image.color = Color.gray;

            if (_preButton.GetComponent<MyButton>().buttonType == StoreButtonType.ButtonType.BuyButton)
            {
                MyBuyButton buyButton = _preButton.GetComponent<MyBuyButton>();

                infoText.text = buyButton._info;
                infoText.text += $"   가격: {buyButton._price}";
            }

        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            Debug.LogWarning(_preButton.name + e.Message);
#endif
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        try
        {
            _preButton.GetComponent<UnityEngine.UI.Image>().color = _preColor;
            infoText.text = "".ToString();
        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            //Debug.LogWarning(e);
#endif
        }
    }

    #endregion


    private bool BuyItem(MyBuyButton _myBuyButton)
    {
        bool canBuy = true;

        PlayerCtrl _playerCtrl = transform.parent.GetComponent<PlayerCtrl>();
        if (_playerCtrl._point < _myBuyButton._price)
        {
            _playerCtrl.ActionTextSetting("포인트가 부족합니다.");
            return false;
        }

        string firstUID = _myBuyButton._uid.Substring(0, 2);
        string middleUID = _myBuyButton._uid.Substring(2, 3);
        string lastUID = _myBuyButton._uid.Substring(5, 4);

        if (firstUID == "01")
        {
            canBuy = _playerCtrl.PlayerWeaponChange(_myBuyButton._uid);
        }
        else if (firstUID == "06")
        {
            if (_playerCtrl.isHaveItem == true && middleUID == "000" && lastUID == "0000")
            {
                _playerCtrl.ActionTextSetting("이미 다른 아이템을 보유하고 있습니다..");
                return false;
            }

            canBuy = _playerCtrl.ItemSetting(_myBuyButton._uid);
        }
        else if (firstUID == "07")
        {
            if (_playerCtrl.isHaveItem == true)
            {
                _playerCtrl.ActionTextSetting("이미 다른 아이템을 보유하고 있습니다..");
                return false;
            }

            canBuy = _playerCtrl.ItemSetting(_myBuyButton._uid);
        }

        if (canBuy == true)
        {
            _playerCtrl._point -= _myBuyButton._price;
            _playerCtrl.ActionTextSetting("구매 완료");

            PlayerPointSetting();
        }

        return canBuy;
    }
}
