using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonPartManager : MonoBehaviour {

    [SerializeField] GameObject parent;
    Player player;
    public static DungeonPartManager Instance;
    public MapManager mapManager;
    UIMiniMap miniMap;
    public DungeonInitializer dungeonInitializer;
    public EnemyManager enemyManager;
    public MoveButtonManager moveButtonManager;
    public int dungeonType = 0;
    public int floor = 1;
    UISwitch ui;
    SceneTransitionManager sceneTransitionManager;
    EventSceneManager eventSceneManager;
    SESet seSet;
    TutorialManager tutorialMn;

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 30;
        // デバッグ用
        //SaveDataReset();
    }
    // Use this for initialization
    void Start () {
        if (!parent) parent = GameObject.Find("GameObjectParent");
        player = parent.GetComponentInChildren<Player>();

        floor = SaveData.GetInt("Floor", 1);
        if ( floor % 8 >= 5 && floor % 8 <= 7 && floor != 30)
        {
            dungeonType = 1;
        }
        mapManager = parent.GetComponentInChildren<MapManager>();
        miniMap = parent.GetComponentInChildren<UIMiniMap>();
        dungeonInitializer = parent.GetComponentInChildren<DungeonInitializer>();
        enemyManager = parent.GetComponentInChildren<EnemyManager>();
        moveButtonManager = parent.GetComponentInChildren<MoveButtonManager>();
        sceneTransitionManager = parent.GetComponentInChildren<SceneTransitionManager>();
        eventSceneManager = parent.GetComponentInChildren<EventSceneManager>();
        seSet = parent.GetComponentInChildren<SESet>();
        tutorialMn = parent.GetComponentInChildren<TutorialManager>();

        mapManager.d_initializer = dungeonInitializer;
        mapManager.DungeonGenerate();
        //dungeonInitializer.Init();
        int _bgmNum = dungeonType;
        if (floor % 8 == 0 || floor == 30) _bgmNum = 2;
        parent.GetComponentInChildren<BGMSet>().SetBGM(_bgmNum);
        player.LoadPlayerInfo();
        player.Init();
        miniMap.MiniMapInit();
        tutorialMn.StartBehaviour();

        ui = parent.GetComponentInChildren<UISwitch>();
        ui.SwitchUI((int)DungUIType.BATTLE);

        StartCoroutine(sceneTransitionManager.FadeIn());

        // 階の最初にイベント発生する際
        int random = Random.Range(0, 100);
        // 今は確率0％
        if ( random < 0 )
        {
            eventSceneManager.EventStart("EventText2");
        }
    }

	// Update is called once per frame
	void Update () {
	}

    public void NextFloor()
    {
        StartCoroutine(NextFloorCoroutine());
    }

    IEnumerator NextFloorCoroutine()
    {
        seSet.PlaySE(SESet.Type.STAIRS_DOWN);
        yield return StartCoroutine(sceneTransitionManager.FadeOut());

        if ( floor == 30 )
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameClear");
            yield break;
        }
        floor++;
        SaveData.SetInt("Floor", floor);
        // 中断フラグOFF
        SaveData.SetInt("IsInterrupt", 0);
        SaveData.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Dungeon1");
    }

    public void DownFloor(int i)
    {
        floor = ( i - 1 );
        NextFloor();
    }

    public void SaveDataReset()
    {
        Debug.Log("セーブデータリセット");
        
        // リセットで消してはならないデータを残してリセット
        int isTutorialON = SaveData.GetInt("IsTutorialON", 1);
        SaveData.Clear();
        SaveData.SetInt("IsTutorialON", isTutorialON);

        SaveData.Save();
    }
}
