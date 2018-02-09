using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class EventSceneManager : MonoBehaviour {
    
    enum ImageType
    {
        Angel1,
        Angel2,
        Angel3,
        Reaper1,
        Reaper2
    }

    enum EffectiveEventType
    {
        PLUS_SOUL_STONE,
        ENEMY_SPAWN
    }

    Dictionary<ImageType, Sprite> data = new Dictionary<ImageType, Sprite>();
    
    TextAsset dialog;
    StringReader reader;
    [SerializeField]
    float textDisplaySpeed = 0.3f;

    GameObject parent;
    UISwitch uiSwitch;
    [SerializeField]
    Image[] Pictures;
    [SerializeField]
    Text eventText;
    [SerializeField]
    GameObject chooseButtons;
    [SerializeField]
    Text[] chooseButtonTexts;

    string[] chooseEventText = { "", "" };

    AudioSource audioSource;
    [SerializeField] AudioClip[] sounds;

    EnemyManager eneMn;
    PlayerItem playerItem;

    // 発生したイベントファイル名を覚えておいて
    // 同フロアで二度同じイベントは起きないようにする
    List<string> fileNames = new List<string>();

    // Use this for initialization
    void Start () {

        if ( parent ) return;
        parent = GameObject.Find("GameObjectParent");
        uiSwitch = parent.GetComponentInChildren<UISwitch>();
        audioSource = GetComponent<AudioSource>();
        eneMn = parent.GetComponentInChildren<EnemyManager>();
        playerItem = parent.GetComponentInChildren<PlayerItem>();

        data[ImageType.Angel1] = Resources.Load<Sprite>("Image/EventUI/angel_1") as Sprite;
        data[ImageType.Angel2] = Resources.Load<Sprite>("Image/EventUI/angel_2") as Sprite;
        data[ImageType.Angel3] = Resources.Load<Sprite>("Image/EventUI/angel_3") as Sprite;
        data[ImageType.Reaper1] = Resources.Load<Sprite>("Image/EventUI/reaper_1") as Sprite;
        data[ImageType.Reaper2] = Resources.Load<Sprite>("Image/EventUI/reaper_2") as Sprite;

    }
	
	// Update is called once per frame
	void Update () {

    }

    public bool IsThisEventFinished(string _fileName)
    {
        foreach(string name in fileNames )
        {
            if (_fileName == name )
            {
                return true;
            }
        }
        return false;
    }

    // どの会話イベントが発生するかをどう受け取るか？
    // ファイル名（文字列）または enum EventType
    // ↓イベント開始したい時呼ぶ
    public void EventStart(string _fileName)
    {
        if ( !parent ) Start();

        fileNames.Add(_fileName);

        uiSwitch.SwitchUI((int)DungUIType.EVENT);
        chooseButtons.SetActive(false);

        dialog = Resources.Load<TextAsset>("Dialog/" + _fileName) as TextAsset;
        reader = new StringReader(dialog.text);
        // 最初の行は列の説明なので飛ばす
        reader.ReadLine();

        // 会話の最初の行を表示
        NextMessage();
    }

    void PictureChange(int positionNum, ImageType type)
    {
        Pictures[positionNum].sprite = data[type];
    }

    public void PlayerChooseSelect(int num)
    {
        chooseButtons.SetActive(false);
        // 選択された方のcsvファイル（会話）を読み込む
        EventStart(chooseEventText[num]);
    }

    // 次の行に進む処理
    public void OnTapMessageBox()
    {
        // 選択肢が出ている時は感知しない
        if ( chooseButtons.activeSelf ) return;

        // 次の行がある場合
        if ( 0 == NextMessage() )
        {

        }
        // ダイアログ終了
        else uiSwitch.SwitchUI((int)DungUIType.BATTLE);
    }

    int NextMessage()
    {
        // ダイアログ終点に達した時
        if ( reader.Peek() <= -1 ) return -1;

        // 次の行を取得

        // 区切り指定文字
        char[] delimiterChars = { ',' };

        string str = reader.ReadLine();
        //Debug.Log(str);
        string[] words = str.Split(delimiterChars);

        if ( words.Length < 4 ) return -1;

        int picNum;
        int.TryParse(words[1], out picNum);
        PictureChange(0, (ImageType)picNum);
        int.TryParse(words[2], out picNum);
        PictureChange(1, (ImageType)picNum);
        eventText.text = words[3];

        if ( words.Length == 4 ) return 0;

        int eventNum = 0;
        int.TryParse(words[0], out eventNum);
        switch ( eventNum )
        {
            case 10:
                // 効果音
                int soundNum = 0;
                int.TryParse(words[4], out soundNum);
                audioSource.clip = sounds[soundNum];
                audioSource.Play();
                break;
            case 11:
                // 選択肢
                chooseButtons.SetActive(true);

                chooseButtonTexts[0].text = words[4];
                chooseEventText[0] = words[5];

                chooseButtonTexts[1].text = words[6];
                chooseEventText[1] = words[7];
                break;
            default:
                CauseEffectiveEvent(eventNum);
                break;

        }
        return 0;
    }

    void CauseEffectiveEvent(int eventNum)
    {
        switch ( eventNum )
        {
            case 20:
                // ソウルストーン入手
                playerItem.items[0].kosuu += 10;
                playerItem.items[1].kosuu += 10;
                playerItem.items[2].kosuu += 10;
                break;
            case 21:
                // 敵ポップ
                for ( int i = 0; i < 5; i++ )
                {
                    eneMn.Spawn(false);
                }
                break;
        }

    }
}
