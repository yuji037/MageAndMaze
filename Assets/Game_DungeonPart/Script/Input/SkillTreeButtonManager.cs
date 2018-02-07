using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeButtonManager : MonoBehaviour
{
    GameObject parent;
    private const int TREE_WID = 3, TREE_HEI = 8, NONE = -1;
    public const int RED = 0, YELLOW = 1, BLUE = 2, TREE_SUU = 3, SET_SUU = 6;
    private int selectSkill = NONE;
    //今選択している登録パネルの番号達
    //private bool[] selectedRegList = new bool[6];
    //UIのgameobject達
    [SerializeField]
    private GameObject SkillButtonPre;
    [SerializeField]
    private GameObject BranchPre;
    [SerializeField]
    private GameObject ScrollPanel;
    [SerializeField]
    private GameObject SyutokuButton;
    [SerializeField]
    private GameObject registPanel;
    [SerializeField]
    private GameObject selectImage;
    [SerializeField]
    private GameObject SkillTreeCanvas;

    private bool deleteMode=false;
    public GameObject SetumeiPanel;

    private GameObject[] SyadanPanel;

    private Dictionary<int, GameObject> skillButtons = new Dictionary<int, GameObject>();
    public GameObject[] SoulStoneImage = new GameObject[TREE_SUU];
    private GameObject[] regSkillButton = new GameObject[SET_SUU];

    private PlayerSkillTree pst;
    private bool stated = false;
    // Use this for initialization
    void Start()
    {
        parent = GameObject.Find("GameObjectParent");
        //プレイヤーからスキルツリーデータを持ってくる
        pst = parent.GetComponentInChildren<PlayerSkillTree>();
        //ツリー背景インスタンス化
        GameObject[] TreeBacks = new GameObject[TREE_SUU];
        SyadanPanel = new GameObject[TREE_SUU];

        for (int i = 0; i < TREE_SUU; i++)
        {
            TreeBacks[i] = ScrollPanel.transform.Find("" + i).gameObject;
            SyadanPanel[i] = TreeBacks[i].transform.Find("Syadan").gameObject;
        }
        //ツリーの配置データを作成
        int[][,] TreeHaiti = new int[TREE_SUU][,]
        {
            //redのツリー配置
        new int[TREE_HEI, TREE_WID]
        {
            {  0,  0,108},
            {  0,105,  0},
            {103,  0,107},
            {  0,109,  0},
            {102,  0,106},
            {  0,104,  0},
            {101,  0,  0},
            {  1,  0,  0}
        },
         //yellowのツリー配置
         new int[TREE_HEI, TREE_WID]
        {
            {  0,  0,208},
            {203,  0,  0},
            {  0,205,207},
            {202,  0,  0},
            {  0,204,  0},
            {  0,  0,206},
            {201,  0,  0},
            {  0,  0,  0}
        },
          //blueのツリー配置
          new int[TREE_HEI, TREE_WID]
        {
            {  0,308,  0},
            {303,  0,306},
            {  0,  0,  0},
            {302,  0,307},
            {  0,305,  0},
            {301,  0,  0},
            {304,  0,  0},
            {  0,  0,  0}
        }
        };
        //スクロールパネルにツリーを配置
        for (int i = 0; i < TREE_SUU; i++)
        {
            SkillTreeUICreate(TreeBacks[i], TreeHaiti[i]);
            TreeBacks[i].transform.localPosition = new Vector3(i * (TreeBacks[i].GetComponent<RectTransform>().sizeDelta.x + 60) + 30, 0);
        }
        //TreeBacks[RED].GetComponent<Image>().color = Color.red;
        //TreeBacks[YELLOW].GetComponent<Image>().color = Color.yellow;
        //TreeBacks[BLUE].GetComponent<Image>().color = Color.blue;
        for (int i = 0; i < SET_SUU; i++)
        {
            regSkillButton[i] = registPanel.transform.Find("" + i).gameObject;
        }
        Debug.Log(pst.Skills[1].Syutoku);
        syutokuKousin();
        RegiButKousin();
        //セレクトimageを前に出す
        //selectImage.transform.SetAsLastSibling();
        //SetumeiPanel.transform.SetAsLastSibling();
        //setImage.transform.SetAsLastSibling();
        //transform.SetAsLastSibling();
        Init();
        stated = true;
    }
    private void Init()
    {
        for (int i = 0; i < SET_SUU; i++)
        {
            regSkillButton[i].transform.Find("SelectImage").gameObject.SetActive(false);
        }
    }
    private void OnEnable()
    {
        if (stated)
        {
            Init();
            syutokuKousin();
            RegiButKousin();
        }
    }
    void SkillTreeUICreate(GameObject treeBack, int[,] haitidata)
    {
        float width = treeBack.GetComponent<RectTransform>().sizeDelta.x;
        float height = treeBack.GetComponent<RectTransform>().sizeDelta.y;
        float offset = SkillButtonPre.GetComponent<RectTransform>().sizeDelta.x / 2 + 10;
        float masukanW = (width - offset * 2) / (TREE_WID - 1);
        float masukanH = (height - offset * 2) / (TREE_HEI - 1);
        Dictionary<int, GameObject> Buttons = new Dictionary<int, GameObject>();
        for (int h = 0; h < TREE_HEI; h++)
        {
            for (int w = 0; w < TREE_WID; w++)
            {
                int num = haitidata[h, w];
                if (num > 0)
                {
                    skillButtons[num] = Instantiate(SkillButtonPre, treeBack.transform);
                    Buttons[num] = skillButtons[num];
                    Buttons[num].transform.localPosition = new Vector3(offset + (w) * masukanW, -offset + (h) * -masukanH);
                    Buttons[num].GetComponent<SkillButton>().init(num);
                    Buttons[num].GetComponentInChildren<Text>().text += num;
                    if (pst.Skills[num].skillImage != null) Buttons[num].transform.Find("skillImage").GetComponent<Image>().sprite = pst.Skills[num].skillImage;
                    else Buttons[num].transform.Find("skillImage").GetComponent<Image>().color = new Color(1, 1, 1, 0);

                    Buttons[num].GetComponent<Button>().onClick.AddListener(() => skillButtonClick(num));
                }
            }
        }
        //前提スキルボタンに対して枝を伸ばす。
        foreach (GameObject i in Buttons.Values)
        {
            if (null == pst.Skills[i.GetComponent<SkillButton>().skillnum].ZenteiSkillId) break;
            foreach (int k in pst.Skills[i.GetComponent<SkillButton>().skillnum].ZenteiSkillId)
            {
                var dis = Vector3.Distance(Buttons[k].transform.localPosition, i.transform.localPosition);
                var diff = (Buttons[k].transform.position - i.transform.position).normalized;
                var br = Instantiate(BranchPre, i.transform.position, Quaternion.FromToRotation(Vector3.up, diff), treeBack.transform);
                br.GetComponent<RectTransform>().sizeDelta = new Vector2(20, dis);
                br.transform.SetAsFirstSibling();
                Buttons[k].GetComponent<SkillButton>().Branch.Add(br);
            }
        }
    }


    //登録しているスキルのアイコンを、setskillsを参照して一括更新
    void RegiButKousin()
    {
        for (int i = 0; i < SET_SUU; i++)
        {
            //セットされていたら
            if (pst.SetSkills[i] != NONE)
            {
                var iconimg = regSkillButton[i].transform.Find("skillImage").GetComponent<Image>();
                iconimg.enabled = true;
                iconimg.sprite = pst.Skills[pst.SetSkills[i]].skillImage;
            }
            else
            {
                regSkillButton[i].transform.Find("skillImage").GetComponent<Image>().enabled = false;
            }
        }

    }

    //全てのスキルボタンの状態を更新,スキル選択した時に更新。
    void syutokuKousin()
    {
        //ボタンの状態を更新
        foreach ( var i in skillButtons.Values )
        {
            i.GetComponent<SkillButton>().jotaiKousin();
        }
        //ソウルストーン個数を更新

        for ( int i = 0; i < TREE_SUU; i++ )
        {
            SoulStoneImage[i].transform.Find("Text").GetComponent<Text>().text = "x" + pst.GetComponent<PlayerItem>().items[i].kosuu;
        }
        //ソウルストーン消費量を更新
        if ( selectSkill != NONE )
        {

            switch ( skillButtons[selectSkill].GetComponent<SkillButton>().Jotai )
            {
                case SkillButton.jotai.GOT:
                    SyutokuButton.GetComponent<Button>().interactable = false;
                    SyutokuButton.GetComponentInChildren<Text>().text = "習得済み";
                    break;
                case SkillButton.jotai.UN_GOT:
                    if ( pst.GetComponent<PlayerItem>().stoneEnoughCheck(pst.Skills[selectSkill].UseSoul[RED], pst.Skills[selectSkill].UseSoul[YELLOW], pst.Skills[selectSkill].UseSoul[BLUE]) )
                    {
                        SyutokuButton.GetComponent<Button>().interactable = true;
                        SyutokuButton.GetComponentInChildren<Text>().text = "習得可能";
                    }
                    else
                    {
                        SyutokuButton.GetComponent<Button>().interactable = false;
                        SyutokuButton.GetComponentInChildren<Text>().text = "ソウルストーン\nが足りません";
                    }

                    int red = pst.Skills[selectSkill].UseSoul[SkillTreeButtonManager.RED];
                    int yellow = pst.Skills[selectSkill].UseSoul[SkillTreeButtonManager.YELLOW];
                    int blue = pst.Skills[selectSkill].UseSoul[SkillTreeButtonManager.BLUE];
                    SoulStoneImage[RED].transform.Find("usesoultext").GetComponent<Text>().text = ( red == 0 ) ? "" : "-" + red;
                    SoulStoneImage[YELLOW].transform.Find("usesoultext").GetComponent<Text>().text = ( yellow == 0 ) ? "" : "-" + yellow;
                    SoulStoneImage[BLUE].transform.Find("usesoultext").GetComponent<Text>().text = ( blue == 0 ) ? "" : "-" + blue;
                    break;
                case SkillButton.jotai.CANT_GET:
                    SyutokuButton.GetComponent<Button>().interactable = false;
                    SyutokuButton.GetComponentInChildren<Text>().text = "習得不可";
                    break;
            }
        }
        else
        {
            foreach ( var i in SoulStoneImage )
            {
                i.transform.Find("usesoultext").GetComponent<Text>().text = "";
            }
        }
    }
    //右下のボタンを押したときの処理
    public void RegClick(int num)
    {
        if ( deleteMode )
        {
            regSkillButton[num].transform.Find("SelectImage").gameObject.SetActive(false);
            pst.SetSkills[num] = NONE;
        }
        else
        {
            if ( selectSkill == NONE ) return;
            pst.SetSkills[num] = selectSkill;
        }
        RegiButKousin();
    }
    //syadaneventが押されたらsyadanpanelを非アクティブに
    public void Syadan()
    {
        foreach (var i in SyadanPanel)
        {
            i.SetActive(false);
        }
        foreach (var i in regSkillButton)
        {
            i.transform.Find("SelectImage").gameObject.SetActive(false);
        }
        SetumeiPanel.SetActive(false);
        selectSkill = NONE;
    }
    //スキルボタンを押したら
    public void skillButtonClick(int num)
    {
        selectSkill = num;
        selectImage.SetActive(true);
        SetumeiPanel.SetActive(true);
        RectTransform but_rect = skillButtons[num].GetComponent<RectTransform>();
        int trans_x = 1, trans_y = 1;
        trans_x = ( GetComponent<RectTransform>().position.x > but_rect.position.x ) ? 1 : -1;
        trans_y = ( GetComponent<RectTransform>().position.y > but_rect.position.y ) ? 1 : -1;
        var setumei_rect = SetumeiPanel.GetComponent<RectTransform>();
        setumei_rect.position = but_rect.position + new Vector3(setumei_rect.sizeDelta.x * setumei_rect.lossyScale.x * 0.6f * trans_x, setumei_rect.sizeDelta.y * setumei_rect.lossyScale.y * 0.5f * trans_y);
        SetumeiPanel.transform.Find("SkillNameText").GetComponent<Text>().text = pst.Skills[num].SkillName;
        SetumeiPanel.transform.Find("SkillSetumei").GetComponent<Text>().text = pst.Skills[num].skillDescription;
        SetumeiPanel.transform.Find("SkillImage").GetComponent<Image>().sprite = pst.Skills[num].skillImage;
        foreach ( var i in SyadanPanel )
        {
            i.SetActive(true);
        }
        selectImage.transform.position = skillButtons[num].transform.position;
        //習得してたらスキルセットのボタンを全てアクティブに。してなかったら非アクティブに
        bool syutokuFlag = pst.Skills[num].Syutoku;
        SkillSetSelectActive(syutokuFlag);
        syutokuKousin();
    }

    void SkillSetSelectActive(bool syutokuFlag)
    {
        deleteMode = false;
        foreach ( var i in regSkillButton )
        {
            var s = i.transform.Find("SelectImage").gameObject;
            s.GetComponent<Image>().color = Color.blue;
            s.SetActive(syutokuFlag);
        }
    }

    public void SyutokuClick()
    {

        bool canSyutoku = true;
        //習得できるか判定
        for (int i = 0; i < TREE_SUU; i++)
        {
            if (pst.GetComponent<PlayerItem>().items[i].kosuu < pst.Skills[selectSkill].UseSoul[i])
            {
                canSyutoku = false;
            }
        }
        if (canSyutoku)
        {
            for (int i = 0; i < TREE_SUU; i++)
            {
                pst.GetComponent<PlayerItem>().items[i].kosuu -= pst.Skills[selectSkill].UseSoul[i];
            }
            pst.Skills[selectSkill].Syutoku = true;
            SkillSetSelectActive(true);
            syutokuKousin();
        }
    }
   
    public void KaijoClick()
    {
        deleteMode = true;
        for (int i = 0; i < SET_SUU; i++)
        {
            var r = regSkillButton[i].transform.Find("SelectImage").gameObject;
            r.SetActive(pst.SetSkills[i] != NONE);
            r.GetComponent<Image>().color = Color.red;
        }
        RegiButKousin();
    }
}
