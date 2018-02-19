using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class EventSceneManager : MonoBehaviour {
    
    enum ImageType
    {
        NONE = 0,
        Ange1,
        Ange2,
        Grim1,
        Grim2,
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

    [SerializeField] GameObject narrationWindow;
    Text narrationText;

    AudioSource audioSource;
    [SerializeField] AudioClip[] sounds;

    EnemyManager eneMn;
    PlayerItem playerItem;
    TutorialManager tutorialMn;
    NPCEventManager npcEventManager;

    // 発生したイベントファイル名を覚えておいて
    // 同フロアで二度同じイベントは起きないようにする
    List<string> fileNames = new List<string>();

    DungUIType lastUIType;

    // Use this for initialization
    void Start () {
        
        if ( parent ) return;
        parent = GameObject.Find("GameObjectParent");
        uiSwitch = parent.GetComponentInChildren<UISwitch>();
        audioSource = GetComponent<AudioSource>();
        eneMn = parent.GetComponentInChildren<EnemyManager>();
        playerItem = parent.GetComponentInChildren<PlayerItem>();
        tutorialMn = parent.GetComponentInChildren<TutorialManager>();
        npcEventManager = parent.GetComponentInChildren<NPCEventManager>();

        data[ImageType.Ange1] = Resources.Load<Sprite>("Image/EventUI/ange1") as Sprite;
        data[ImageType.Ange2] = Resources.Load<Sprite>("Image/EventUI/ange2") as Sprite;
        data[ImageType.Grim1] = Resources.Load<Sprite>("Image/EventUI/grim1") as Sprite;
        data[ImageType.Grim2] = Resources.Load<Sprite>("Image/EventUI/grim2") as Sprite;

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
        Debug.Log(_fileName);
        if ( !parent ) Start();

        // 以前がEventUIでなければ記憶しておく
        if (uiSwitch.UIType != DungUIType.EVENT) lastUIType = uiSwitch.UIType;

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
        if (type == ImageType.NONE )
        {
            Pictures[positionNum].enabled = false;
        }
        else
        {
            Pictures[positionNum].enabled = true;
            Pictures[positionNum].sprite = data[type];
            // 画像原寸大だと大きすぎる
            //Pictures[positionNum].SetNativeSize();
        }
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
        else
        {
            if (uiSwitch.UIType == DungUIType.EVENT) uiSwitch.SwitchUI((int)lastUIType);
            narrationWindow.SetActive(false);
        }
    }

    int NextMessage()
    {
        // ダイアログ終点に達した時
        if ( reader.Peek() <= -1 ) return -1;

        // 次の行を取得

        // 区切り指定文字
        char[] delimiterChars = { ',' };

        string str = reader.ReadLine();
        Debug.Log(str);
        string[] words = str.Split(delimiterChars);

        //if ( words.Length < 4 ) return -1;

        // 効果音、選択肢などイベントの種類を表す
        int eventNum = 0;
        int.TryParse(words[0], out eventNum);

        int picNum;
        int.TryParse(words[1], out picNum);
        PictureChange(0, (ImageType)picNum);
        int.TryParse(words[2], out picNum);
        PictureChange(1, (ImageType)picNum);
        SetEventText(words[3]);

        if ( words.Length == 4 ) return 0;

        if (eventNum!= 99 )
        {
            narrationWindow.SetActive(false);
        }

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
            case 99:
                // ナレーションウィンドウにテキスト表示
                uiSwitch.SwitchUI((int)lastUIType);
                narrationWindow.SetActive(true);
                if ( !narrationText ) narrationText = narrationWindow.GetComponentInChildren<Text>();
                SetNarrationText(words[3]);
                StartCoroutine(NarrationWindowBehaviour());
                break;
            default:
                npcEventManager.CauseEffectiveEvent(eventNum);
                break;

        }
        return 0;
    }

    IEnumerator NarrationWindowBehaviour()
    {
        yield return new WaitForSeconds(0.5f);
        // ナレーションが先へ進む又は閉じる処理
        if ( tutorialMn.IsTutorialON )
        {
            int tutoNumber = tutorialMn.TutorialNumber;
            // チュートリアル番号が変化したら先へ
            while ( tutorialMn.TutorialNumber == tutoNumber )
            {
                yield return null;
            }
        }
        else
        {
            // タップしたら先へ
            yield return StartCoroutine(WaitUntilFingerUp());
        }
        OnTapMessageBox();
    }

    IEnumerator WaitUntilFingerUp()
    {
        while ( Input.touchCount == 0 && !Input.GetMouseButton(0) )
        {
            yield return null;
        }
        while ( Input.touchCount != 0 && !Input.GetMouseButtonUp(0) )
        {
            yield return null;
        }
    }

    void SetNarrationText(string str)
    {
        // 会話文の中の改行は 'B' で判別
        char[] delimiterChars = { 'B' };
        string[] words = str.Split(delimiterChars);
        string txt = "";
        foreach ( string word in words )
        {
            txt += word;
            txt += "\n";
        }

        narrationText.text = txt;
    }

    void SetEventText(string str)
    {
        // 会話文の中の改行は 'B' で判別
        char[] delimiterChars = { 'B' };
        string[] words = str.Split(delimiterChars);
        string txt = "";
        foreach(string word in words )
        {
            txt += word;
            txt += "\n";
        }

        eventText.text = txt;
    }


}
