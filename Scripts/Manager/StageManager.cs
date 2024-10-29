using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager inst;
    public List<Enemy> enemies = new();
    public List<Spawner> spawners = new();

    public Dictionary<EnemyType, int> enemyDeathCount = new();
    public int enemyScore;
    public Dictionary<ItemType, int> itemGetCount = new();
    public int itemScore;

    public int stage;
    public float playTime;

    private const float enemySpawnTime = 5f;
    public float enemySpawnTimer;
    private int enemySpawnCount;
    public float enemySpawnDelay;

    public bool onAwakeMouseFixed;
    public int bouseScore;

    public float stageMultifly;

    
    public GameObject bossPrefeb;
    public Transform bossSpawnTransform;
    public Enemy spawnedBoss;
    public bool bossSpawn;

    public float enemyStopTime;

    public bool invincibility;
    public bool timeStop;
    public bool soundOff;
    public bool onAwakeOpenUi;
    void Awake()
    {
        inst = this;
    }
    private void Start()
    {
        
        if (onAwakeMouseFixed)
        {
            GameManager.inst.MouseFixed(true);
        }
        if (onAwakeOpenUi)
        {
            GameManager.inst.MouseFixed(false);
            GameManager.inst.openUi = true;
            Time.timeScale = 0;
        }
        else
        {
            GameManager.inst.openUi = false;
            Time.timeScale = 1;
        }
        if (GameManager.inst.PlayerAbilities.TryGetValue(Ability.Drone,out int count))
        {
            for (int i = 0; i < count; i++)
            {
                CreateDrone();
            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            CreateNoticeMessage(3, string.Format("치트키 스테이지 이동을 사용했습니다."));
            GameManager.inst.stageClear = true;
            NextStage();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            CreateNoticeMessage(3, string.Format("치트키 레벨업을 사용했습니다."));
            GameManager.inst.AddExperience((int)(GameManager.inst.maxExp - GameManager.inst.currnetExp));
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            CreateNoticeMessage(3, string.Format("치트키 무적 켜기를 사용했습니다."));
            invincibility = true;
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            CreateNoticeMessage(3, string.Format("치트키 무적 끄기를 사용했습니다."));
            invincibility = false;
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            CreateNoticeMessage(3, string.Format("치트키 체력 및 마나 회복 사용했습니다."));
            PlayerController.inst.health.Heal(100);
            PlayerController.inst.UpdateMana(50);
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            CreateNoticeMessage(3, string.Format("치트키 랜덤으로 임의의 적들 소환을 사용했습니다."));
            StartCoroutine(SpawnEnemies());
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            CreateNoticeMessage(3, string.Format("치트키 랜덤으로 임의의 아이템 소환을 사용했습니다."));
            Instantiate(GameManager.inst.items[Random.Range(0, GameManager.inst.items.Length)], PlayerController.inst.transform.position + new Vector3(Random.Range(0, 6), 1, Random.Range(0, 6)), Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            CreateNoticeMessage(3, string.Format("치트키 화면 내에 적 처치를 사용했습니다."));
            AllRangeEnemyKill();
        }
        if (Input.GetKeyDown(KeyCode.F9))
        {
            CreateNoticeMessage(3, string.Format("치트키 일시정지를 사용했습니다."));
            TimeStop();
        }
        if (Input.GetKeyDown(KeyCode.F10))
        {
            CreateNoticeMessage(3, string.Format("치트키 현재 스테이지 재시작을 사용했습니다."));
            ReStartCurrentStage();
        }
        if (Input.GetKeyDown(KeyCode.F11))
        {
            CreateNoticeMessage(3, string.Format("치트키 소리 켜기/끄기를 사용했습니다."));
            SoundOnOff();
        }
        if (Input.GetKeyDown(KeyCode.F12))
        {
            CreateNoticeMessage(3, string.Format("치트키 시간 추가를 사용했습니다."));
            playTime += 120;
        }
        if (GameManager.inst.openUi)
        {
            return;
        }
        playTime += Time.deltaTime;
        if ((playTime > 120 || stage == 3 )&& !bossSpawn)
        {
            UiManager.inst.BossCome();
            spawnedBoss = CreateEnemy(bossPrefeb, bossSpawnTransform.position, bossSpawnTransform.rotation).GetComponent<Enemy>();
            bossSpawn = true;            
        }
        else if (enemySpawnTimer < Time.time && !bossSpawn)
        {
            enemySpawnCount = (int)(playTime / 10) + 3;
            enemySpawnTimer = enemySpawnTime + Time.time;
            StartCoroutine(SpawnEnemies());
        }


    }
    public void SoundOnOff()
    {
        soundOff = !soundOff;
        AudioListener.volume = soundOff ? 0 : 1;
    }
    public void TimeStop()
    {
        timeStop = !timeStop;
        Time.timeScale = timeStop ? 0 : 1;
        GameManager.inst.openUi = timeStop;
    }
    public void ReStartCurrentStage()
    {
        SceneManager.LoadScene("Stage" + stage + 1);
    }
    public void NextStage()
    {
        GameManager.inst.time += playTime;
        if (stage >= 3 || !GameManager.inst.stageClear)
        {
            if (UiManager.inst.inputField != null)
            {
                GameManager.inst.AddData(GameManager.inst.score, UiManager.inst.inputField.text, GameManager.inst.time);
            }
            Destroy(GameManager.inst.gameObject);
            SceneManager.LoadScene("Main");
            return;
        }
        else
        {
            SceneManager.LoadScene("Stage" + (stage + 1));            
        }
    }
    public Transform FindNearestEnemy(Transform _tr)
    {
        Transform target =null;
        float shortdis = 1000;
        for (int i = 0; i < enemies.Count; i++)
        {
            float dis = Vector3.Distance(_tr.position, enemies[i].transform.position);
            if (shortdis > dis)
            {
                shortdis = dis;
                target = enemies[i].transform;
            }
        }
        return target;
    }
    public void CreateScoreImage(Vector3 _pos)
    {
        for (int i = 0; i < 10; i++)
        {
            Instantiate(GameManager.inst.scoreImage, _pos + new Vector3(Random.Range(-50,51), Random.Range(-50, 51),0), Quaternion.identity,UiManager.inst.scoreRoot);
        }
    }
    public void CreateDrone()
    {
        GameObject newDrone = new("newDrone");
        Transform droneRoot = PlayerController.inst.droneRoot;
        newDrone.transform.SetParent(droneRoot);
        newDrone.transform.localPosition = Vector3.zero;
        GameObject obj = Instantiate(GameManager.inst.drone, newDrone.transform.position, newDrone.transform.rotation, newDrone.transform);
        for (int i = 0; i < droneRoot.childCount; i++)
        {
            droneRoot.GetChild(i).rotation = Quaternion.Euler(0, 360 / droneRoot.childCount * i, 0);
        }
        obj.transform.localPosition = Vector3.right * 2;


    }
    public void StageClear(bool _clear)
    {
        if (_clear)
        {
            Instantiate(GameManager.inst.win);
        }
        else
        {
            Instantiate(GameManager.inst.lose);
        }
        UiManager.inst.clear.SetActive(_clear);
        UiManager.inst.over.SetActive(!_clear);
        UiManager.inst.bossHpBarGameobject.SetActive(false);
        bouseScore = PlayerController.inst.health.currentHealth + PlayerController.inst.currentMana * 150;
        GameManager.inst.AddScore(bouseScore,0);
        GameManager.inst.MouseFixed(false);
        GameManager.inst.stageClear = _clear;
        GameManager.inst.openUi = true;
        Time.timeScale = 0;
        UiManager.inst.resultScreen.SetActive(true);        
    }
    public void UseCheatKey(int _index)
    {
        switch (_index)
        {
            default:
                CreateNoticeMessage(3,string.Format(" 치트키 사용"));
                break;
        }
    }
    public IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < enemySpawnCount; i++)
        {
            spawners[Random.Range(0, spawners.Count)].SpawnEnemy();
            yield return new WaitForSeconds(enemySpawnDelay);
        }
    }    
    public GameObject CreateBullet(GameObject _obj, Vector3 _pos, Quaternion _rot, int _damage)
    {
        GameObject obj = Instantiate(_obj, _pos, _rot);
        if (_obj.GetComponent<BulletBox>() != null)
        {
            _obj.GetComponent<BulletBox>().SetDamage(_damage);
        }
        else
        {
            obj.GetComponent<Bullet>().damage = _damage;            
        }
        return obj;
    }
    public void AllRangeEnemyKill()
    {
        int index = 0;
        for (int i = 0; i < enemies.Count; i++)
        {
            Vector3 viewPort = Camera.main.WorldToViewportPoint(enemies[index].transform.position);
            if (viewPort.x >= 0 && viewPort.x <= 1 && viewPort.y >= 0 && viewPort.y <= 1 && viewPort.z > 1)
            {
                enemies[index].Die();
                i--;
            }
            else
            {
                index++;
            }
        }
    }
    public GameObject CreateEnemy(GameObject _obj, Vector3 _pos, Quaternion _rot)
    {
        GameObject obj = Instantiate(_obj, _pos, _rot);
        enemies.Add(obj.GetComponent<Enemy>());
        return obj;
    }
    /// <summary>
    /// 0 = 스킬
    /// 1 = 스킬 못 씀
    /// 2 = 아이템
    /// 3 = 치트키
    /// </summary>
    /// <param name="_index"></param>
    /// <param name="_contents"></param>
    public void CreateNoticeMessage(int _index,string _contents)
    {
        Transform root = UiManager.inst.noticeMessageRoot;
        if (root.childCount >= 5)
        {
            Destroy(root.GetChild(0).gameObject);
        }
        GameObject obj = Instantiate(UiManager.inst.NoticePrefebs[_index].gameObject, Vector3.zero, Quaternion.identity, root);
        obj.GetComponent<NoticeMessage>().text.text = _contents;
    }
}
