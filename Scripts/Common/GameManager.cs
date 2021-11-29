using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public BunkerDoor bunkerDoor;

    [Header("Player 특전")]
    public bool perk0_Active = false;
    public bool perk1_Active = false;
    public bool perk2_Active = false;

    [Header("Stage")]
    internal int maxStage = 5;
    public int _stage;
    public int _remainEnemyCount;

    [Header("Players")]
    public List<PlayerCtrl> players;
    public bool useCheat = false;

    Transform[] enemyPoints;

    UnityEngine.UI.Text stageText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        _stage = 1;

    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F12))
        {
            ExitGame();
        }
        // 윈도우에서 동작
#elif UNITY_STANDALONE_WIN
        if (Input.GetKeyDown(KeyCode.F12))
        {
            ExitGame();
        }
#endif

    }

    public void ExitGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        // 윈도우에서 동작
#elif UNITY_STANDALONE_WIN
        Application.Quit();
#endif
    }

    public void GameOver()
    {
        PlayerPrefs.Save();

        GameObject gameCanvas = GameObject.Find("GameWorldCanvas");
        gameCanvas.transform.Find("GameOverPanel").gameObject.SetActive(true);

        StartCoroutine(CoFadeOut());
    }

    IEnumerator CoFadeOut()
    {
        GameObject gameCanvas = GameObject.Find("GameWorldCanvas");
        UnityEngine.UI.Image fadein = gameCanvas.transform.Find("GameOverPanel").Find("GameOverFadeIn").GetComponent<UnityEngine.UI.Image>();

        while (fadein.color.a < 1f)
        {
            Color _color = fadein.color;
            _color.a += Time.deltaTime;
            fadein.color = _color;
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        try
        {
            gameCanvas.GetComponent<ButtonCtrl>().OnMainMenuButtonClick();
        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            Debug.Log(e);
#endif
        }
        finally
        {
            GameFail();
        }
    }


    /// <summary>
    /// 게임 씬으로 넘어갔을 때 실행될 함수
    /// </summary>
    public void GameStart(UnityEngine.AsyncOperation _operation, string _playerNickName, string _className)
    {
        string _class;
        if (_className == "소총병")
        {
            _class = "Soldier";
        }
        else if (_className == "의무병")
        {
            _class = "Medic";
        }
        else if (_className == "공병")
        {
            _class = "Engineer";
        }
        else
        {
            _class = null;
        }

        StartCoroutine(CoGameStart(_operation, _playerNickName, _class));
    }

    IEnumerator CoGameStart(UnityEngine.AsyncOperation _operation, string _playerNickName, string _className)
    {
        yield return _operation;

        bunkerDoor = GameObject.FindGameObjectWithTag("BUNKERDOOR").GetComponent<BunkerDoor>();

        Transform[] points = GameObject.Find("PlayerSpawnPoints").GetComponentsInChildren<Transform>();

        int idx = UnityEngine.Random.Range(1, points.Length);

        GameObject _playerPref = Resources.Load<GameObject>("Prefabs/Player/Player");
        players.Add(Instantiate(_playerPref, points[idx].position, Quaternion.identity).GetComponent<PlayerCtrl>());

        Debug.Log($"Players Count: {players.Count}");

        players[0].playerName = _playerNickName;
        players[0].playerClass = (PlayerClass.ePlayerClass)System.Enum.Parse(typeof(PlayerClass.ePlayerClass), _className);

        enemyPoints = GameObject.Find("EnemySpawnPoints").GetComponentsInChildren<Transform>();

#if UNITY_EDITOR
        Debug.Log("EnemyPoints: " + enemyPoints.Length);
#endif

        stageText = GameObject.Find("StageText").GetComponent<UnityEngine.UI.Text>();
        stageText.text = _stage.ToString();

        ObjectCounting.zombieSpwanCount = 15;
        ObjectCounting.spiderSpwanCount = 0;
        ObjectCounting.clutchSpwanCount = 0;
        ObjectCounting.MovidicSpwanCount = 0;

        yield return new WaitForSeconds(1f);
    }


    internal void GameFail()
    {
        _stage = 1;

        useCheat = false;

        perk0_Active = false;
        perk1_Active = false;
        perk2_Active = false;

        players.Clear();
        bunkerDoor = null;
        SpwanManager.Instance.enemies.Clear();

        CursorState.CursorLockedSetting(false);
    }

    /// <summary>
    /// 스테이지 클리어시 플레이어에게 포인트와 경험치를 주는 함수
    /// </summary>
    public void StageClear()
    {
        if (_stage > maxStage)
        {
            GameClear();
            return;
        }
        StartCoroutine(StageClearCo());
    }

    /// <summary>
    /// 게임 클리어시 실행될 함수.
    /// </summary>
    private void GameClear()
    {
        Time.timeScale = 0f;
        CursorState.CursorLockedSetting(false);
        GameObject.Find("GameWorldCanvas").transform.Find("GameClearPanel").gameObject.SetActive(true);
    }

    IEnumerator StageClearCo()
    {
        foreach (var player in players)
        {
            Debug.Log("Stage Clear Coroutine");
            PlayerCtrl _playerCtrl = player.GetComponent<PlayerCtrl>();
            _playerCtrl._point += 800;
            _playerCtrl._playerExp += 400f;
            _playerCtrl.CheckLevelUp();
            yield return null;
        }
    }

    #region 씬 로딩 함수
    /// <summary>
    /// 다른 씬으로 넘어갈 때 로딩씬을 사용할 경우 사용하는 함수.
    /// </summary>
    /// <param name="_sceneName">Scene Name</param>
    public void SceneLoadingFunction(string _sceneName)
    {
        StartCoroutine(SceneLoadingCoroutine(_sceneName));
    }

    private IEnumerator SceneLoadingCoroutine(string _sceneName)
    {
        Time.timeScale = 1f;
        AsyncOperation _loadingOperation = SceneManager.LoadSceneAsync("LoadingScene");

        while (_loadingOperation.progress < 0.9f) { yield return null; }
        yield return new WaitForSeconds(0.5f);
        UnityEngine.UI.Slider _slider = GameObject.Find("LoadingSlider").GetComponent<UnityEngine.UI.Slider>();

        AsyncOperation _operation = SceneManager.LoadSceneAsync(_sceneName);
        _operation.allowSceneActivation = false;

#if UNITY_EDITOR
        Debug.Log("___ Loading... ___");
#endif
        while (_operation.progress < 0.9f)
        {
            _slider.value = (float)_operation.progress;

            yield return null;
        }
#if UNITY_EDITOR
        Debug.Log("____ Loading almost complete ____ ");
#endif
        _slider.value = 0.9f;


        if (_sceneName == "MapScene")
        {
            GameManager.instance.GameStart(_operation,
                PlayerPrefs.GetString("Player_NickName"),
                PlayerPrefs.GetString("Player_Class"));
        }
        else if (_sceneName == "MainMenuScene")
        {
            GameFail();
        }

        int count = 0;
        while (count < 10)
        {
            _slider.value += 1 / 100f;
            count++;
            yield return new WaitForSeconds(0.5f);
        }

        _operation.allowSceneActivation = true;
    }
    #endregion

}
