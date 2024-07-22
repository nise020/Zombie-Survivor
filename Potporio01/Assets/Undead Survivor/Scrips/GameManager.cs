using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //스크립트 등록
    Enemy enemy;
    Item item;
    AutoWeaponDron AutoWeaponDron;
    //PlayerStatas playerStatas;
    
    //PlayerStatas player;
    //public PlayerStatas _Player
    //{
    //    get { return player; }
    //    set { player = value; }
    //}
    public static GameManager Instance;

    [Header("플레이어 위치")]
    [SerializeField] public Transform trsTarget;

    [Header("몬스터 생성")]
    [SerializeField] public List<GameObject> mobList;//몹 리스트
    [SerializeField] public List<Transform> mobTrs;//몹 상태
    [SerializeField] public Transform CreatTab;//생성탭
    [SerializeField] List<Vector2> createLine;
    public bool MobstatasUp = false;
    public int PluseHp = 0;
    GameObject mob;
     public int mobSpowncount = 0;
    int mobSpownMaxcount = 8;


    [Header("적 생성시간")]
    float mobSpawnTimer = 0.0f;// 타이머
    [SerializeField, Range(0.1f, 5f)] float mobSpawnTime = 1.0f;

    [Header("아이템 종류, 확률")]
    [SerializeField] List <GameObject> ItemKind;//아이템 종류
    [SerializeField] List <GameObject> WeaponKind;//아이템 종류
    [SerializeField] List <GameObject> buffKind;//아이템 종류
    [SerializeField, Range(0.0f, 100f)] float IteamProbability;//외부에서 조정 가능
    //bool imItem = false;//아이템 생성 여부
    [SerializeField] Transform creatItemTab;
    int priority = 0;

    [Header("스킬 버튼")]
    [SerializeField] Image skillImg;
    [SerializeField] Button skillbtn;
    [SerializeField] Image skillCoolImg;
    float skillCoolTimer = 0f;
    float skillCoolTime = 10f;//처음에 10초
    bool skillbtnFill = true;
    public bool skillBulletOn = false;
    float skillBulletTimer = 0f;
    float skillBulletTime = 5f;
    float skillCoolBulletTime = 10f;

    [Header("플레이 시간")]
    [SerializeField] TMP_Text timeText;
    public float minuteTimer = 0f;
    float minuteTime = 60f;
    public float hour = 0f;

    [Header("점수")]
    [SerializeField] TMP_Text scoreText;
    int score = 0;

    [Header("게임오버메뉴")]
    [SerializeField] GameObject objGameOverMenu;
    [SerializeField] TMP_Text GameOverMenuScoreText;
    [SerializeField] TMP_Text GameOverMenuRankText;
    [SerializeField] TMP_Text GameOverMenuBtnText;
    [SerializeField] TMP_InputField IFGameOverMenuRank;
    [SerializeField] Button btnGameOverMenu;

    [Header("게임 시작 안내")]
    [SerializeField] TMP_Text textStart;
    bool GameStart = false;
    bool on = true;

    [Header("일시정지")]
    [SerializeField] Button StopBtn;
    [SerializeField] GameObject stopImg;
    [SerializeField] Button continuBtn;
    [SerializeField] Button exitBtn;
    [SerializeField] public bool objStop = false;


    private void Awake()
    {
        if (RankIn.isStating == false)//메뉴에서 시작 하기
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        if (Instance == null)
        {
            Instance = this;//=GameManager가 된다
        }
        else 
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        //playerStatas = FindObjectOfType<PlayerStatas>();
        Color color = textStart.color;
        color.a = 0f;
        textStart.color = color;

        skillbtn.onClick.AddListener(skillOn);
        skillbtn.interactable = false;
        skillCoolImg.color = color;
        skillImg.fillAmount = 0;

        stopImg.SetActive(false);
        StopBtn.onClick.AddListener(StopBtnOn);
        continuBtn.onClick.AddListener(continuBtnOn);
        exitBtn.onClick.AddListener(exitGame);
        //skillImg.rectTransform.position.x = 60f;
        //skillImg.rectTransform.position = new Vector2(60,0);
    }
    private void continuBtnOn()
    {
        stopImg.SetActive(false);
        objStop = false;
    }
    private void StopBtnOn() 
    {
        stopImg.SetActive(true);
        objStop = true; 
    }
    private void exitGame() 
    {
        stopImg.SetActive(false);
        objStop = false;
        FaidInOut.Instance.ActiveFade(true, () =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            FaidInOut.Instance.ActiveFade(false, null);
        });

    }
    private void Update()
    {
        if (objStop==true) { return; }
        gameStart();
        if (GameStart == false) {return; }
        runTime();
        skiilCool();
        createEnemy();//활성화 필요
        
    }
    private void skillOn() 
    {
        skillbtn.interactable = false;
        skillBulletOn = true;
    }
    private void SkillCoolStart()//스킬 지속중일때 컬러 
    {
        Color color = skillCoolImg.color;
        color.a -= Time.deltaTime / skillBulletTime; ;
        if (color.a < 0)
        {
           color.a = 0;
        }
        skillCoolImg.color = color;
        if (skillCoolImg.color.a <= 0) { return; }
    }
    private void SkillCoolRuning()//시각적으로 보이는 쿨타임
    {
        Color color = skillCoolImg.color;
        color.a += Time.deltaTime / skillCoolBulletTime;
        if (color.a > 1)
        {
            color.a = 1;
        }
        skillCoolImg.color = color;
        if (skillCoolImg.color.a > 1) { return; }
    }

    private void skiilCool()//스킬 쿨 타임 조절
    {
        if (skillbtn.interactable == true || trsTarget == null) { return; }

        if (skillbtn.interactable == false)//타이머 등 수정필요
        {
            skillBulletTimer += Time.deltaTime;
            if (skillBulletTimer > skillBulletTime)//스킬 지속시간
            {
                skillBulletOn = false;//스킬버튼
            }

            if (skillBulletOn == true)//스킬 On
            {
                SkillCoolStart();
                skillImg.fillAmount -= Time.deltaTime / skillBulletTime;
                //skillImg.fillAmount = 1 - skillBulletTimer / skillBulletTime;
            }
            else//skillBulletOn == false,스킬 Off
            {
                SkillCoolRuning();
                skillImg.fillAmount += Time.deltaTime / skillCoolBulletTime;
            }

            skillCoolTimer += Time.deltaTime;
            if (skillCoolTimer > skillCoolTime && skillCoolImg.color.a <= 1f)//스킬 쿨타임
            {
                skillCoolTime = 15f;//이후에는 15초 유지
                skillCoolTimer = 0f;
                skillbtn.interactable = true;
                skillBulletTimer = 0f;
            }
            
        }
        

    }

    private void gameStart()//게임 시작
    {
        if (textStart == null) { return; }
        if (on == true)
        {
            Color color = textStart.color;
            color.a += Time.deltaTime/ 1.0f;
            textStart.color = color;
            if (color.a > 1)
            {
                color.a = 1f;
                on = false;
            }
        }
        else
        {
            Color color = textStart.color;
            color.a -= Time.deltaTime / 1.0f;
            textStart.color = color;
            if (color.a < 0)
            {
                color.a = 0f;
                Destroy(textStart.gameObject);
            }
        }
        if (textStart.color.a <= 0) 
        {
            Destroy(textStart.gameObject);
            GameStart = true;
        }
        
    }

    public void ScorePluse(int number)//점수 증가
    {
        score += number;
        scoreText.text = $"{score.ToString("d6")}";
    }

    private void runTime()//플레이 타임
    {
        timeText.text = $"{((int)hour).ToString("d2")}:{((int)minuteTimer).ToString("d2")}";

        if (trsTarget == null) { return; }
        minuteTimer += 1 * Time.deltaTime;

        if (minuteTimer> minuteTime) 
        {
            minuteTimer = 0;
            hour += 1;
            PluseHp += 1;
            mobSpawnTime -= 0.2f;
            mobSpownMaxcount += 3;
        }
    }

    /// <summary>
    /// 플레이어가 움직여도 일정거리 밖에서 스폰하는 기능
    /// </summary>
    private void createEnemy()
    {
        if (trsTarget == null || mobSpowncount >= mobSpownMaxcount)//타이머가 스폰 쿨타임을 넘었을 경우
        { return; }

        mobSpawnTimer += Time.deltaTime;//타이머 ON
        if (mobSpawnTimer > mobSpawnTime && mobSpowncount <= mobSpownMaxcount)//타이머가 스폰 쿨타임을 넘었을 경우
        {
            mobSpawnTimer = 0.0f;
            mobSpowncount += 1;
            int Iroad = Random.Range(0, 4);//스폰할 위치의 경계선 선정
            //Random.Range(1, 4)1,2,3 

            Vector2 playerPos = trsTarget.position;//player 위치
            Vector2 done = new Vector2(0, 0); //디폴트 생성

            float Randamx = Random.Range(-8.0f, 8.0f);//x값 랜덤위치 스폰
            float Randamy = Random.Range(-4.0f, 4.0f);//y값 랜덤위치 스폰

            if (Iroad == 0) //10
            {
                PlusMinas(playerPos.x);//음수를 정수로 전환
                PlusMinas(playerPos.y);//음수를 정수로 전환
                done = new Vector2(createLine[0].x + playerPos.x, Randamy + playerPos.y);
            }
            else if (Iroad == 1)//-10
            {
                PlusMinas(playerPos.x);
                PlusMinas(playerPos.y);
                done = new Vector2(createLine[1].x + playerPos.x, Randamy + playerPos.y);
            }
            else if (Iroad == 2)//7
            {
                PlusMinas(playerPos.x);
                PlusMinas(playerPos.y);
                done = new Vector2(Randamx + playerPos.x, createLine[2].y + playerPos.y);
            }
            else//Iroad == 3//-7
            {
                PlusMinas(playerPos.x);
                PlusMinas(playerPos.y);
                done = new Vector2(Randamx + playerPos.x, createLine[3].y + playerPos.y);
            }
            //done.x = Mathf.Clamp(done.x, -10, 10);
            //done.y = Mathf.Clamp(done.y, -7, 7);

            int mobcount = mobList.Count;
            int mobiRoad = Random.Range(0, mobcount);

            mob = Instantiate(mobList[mobiRoad], done, Quaternion.identity, CreatTab);
            //Enemy enemy = mob.GetComponent<Enemy>();
            //enemy.pluse();

        }
    }


    /// <summary>
    /// 아이템 생성 확률 계산기
    /// </summary>
    /// <param name="trs"></param>
    public void CreateItemProbability(Vector3 trs)
    {
        float randam = Random.Range(0f, 100f);
        if (IteamProbability > randam) 
        {
            float randamKind = Random.Range(0f, 2f);
            if (randamKind == 0) 
            {
                CreateItem(trs, ItemKind, out int value);
                IteamProbability = 0.0f;
            }
            else if (randamKind > 0)
            {
                if (priority < 2)//우선도 설정
                {
                    CreateItem(trs, WeaponKind, out int number);
                    priority += 1;
                    WeaponKind.RemoveAt(number);//지우기
                    return;
                }

                CreateItem(trs, buffKind, out int value);
                IteamProbability = 0.0f;
            }
            else //확률업
            {
                if (WeaponKind == null) { IteamProbability += 0.5f; }
                else { IteamProbability += 2f; } 
            }
        }

        
    }
    /// <summary>
    /// 아이템 생성
    /// </summary>
    /// <param name="위치"></param>
    /// <param name="오브젝트 List"></param>
    /// <param name="value"></param>
    private void CreateItem(Vector3 trs, List<GameObject> obj, out int value) 
    {
        int count = obj.Count;
        int Number = Random.Range(0, count);

        GameObject go = Instantiate(WeaponKind[Number],
            trs, Quaternion.identity, creatItemTab);
        IteamProbability = 0.0f;
        value = Number;
    }
    public void rankCheck()
    {
        List<UserData> listUserData =
           JsonConvert.DeserializeObject<List<UserData>>
           (PlayerPrefs.GetString(RankIn.rankKey));


        int rank = -1;
        if(listUserData.Count < RankIn.rankCount) 
        {
            while (listUserData.Count < RankIn.rankCount)//단순 박복문
            {
                listUserData.Add(new UserData() { Name = "None", Score = 0 });

            }
        }
        int count = listUserData.Count;
        
        for (int iNum = 0; iNum < count; iNum++) 
        {
            UserData userDate = listUserData[iNum];
            if (userDate.Score<score) 
            {
                rank = iNum;
                break;
            }
        }
        GameOverMenuScoreText.text = $"점수 : {score.ToString("d6")} ";
        if (rank != -1)
        {
            GameOverMenuRankText.text = $"랭킹 : {rank + 1}등";
            IFGameOverMenuRank.gameObject.SetActive(true);
            GameOverMenuBtnText.text = "등록";
        }
        else
        {
            GameOverMenuRankText.text = "랭크인 하지 못했습니다";
            IFGameOverMenuRank.gameObject.SetActive(false);
            GameOverMenuBtnText.text = "메인메뉴로";
        }

        btnGameOverMenu.onClick.AddListener(() => 
        {
            if (rank != -1)//랭크에 등록할때
            {
                string name = IFGameOverMenuRank.text;
                if (name == string.Empty) 
                {
                    name = "aaa";
                }
                UserData newRank = new UserData();
                newRank.Score = score;
                newRank.Name = name;

                listUserData.Insert(rank, newRank);
                listUserData.RemoveAt(listUserData.Count - 1);
                string value = JsonConvert.SerializeObject(listUserData);
                PlayerPrefs.SetString((RankIn.rankKey), value);
            }

            FaidInOut.Instance.ActiveFade(true, () =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                FaidInOut.Instance.ActiveFade(false, null);
            });
            
        });
        objGameOverMenu.SetActive(true);
    }

    /// <summary>
    /// 정수를 내보내는 계산기
    /// </summary>
    /// <param name="pos"></param>
    /// <returns>정수</returns>
    public float PlusMinas(float pos)
    {
        if (pos > 0) 
        {
            return pos;
        }
        else if (pos < 0) 
        {
            pos = Mathf.Abs(pos);
        }
        return pos;
    }
    
    /// <summary>
    /// player의 transform에 대한 정보를 밖으로 꺼낸다
    /// </summary>
    /// <param name="_pos"></param>
    public void PlayerTrsPosiTion(out Vector3 _pos)
    {
        _pos = trsTarget.transform.position;
    }

    //public void EnomyTrsPosiTion(out Vector3 _pos)
    //{
    //    _pos = mobList[1].transform.position;
    //}


}
