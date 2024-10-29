using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public enum Ability
{
    Vaccine,
    Shield,
    Speed,
    HealSkill,
    NomalAttack,
    GuidedAttack,
    BombAttack,
    Drone,
    Healing,
    Mana,
    Score,
    Item
}
[System.Serializable]
public class RankData
{
    public List<RankElement> rankElements = new();
}
[System.Serializable]
public struct RankElement
{
    public int score;
    public float time;
    public string name;
}
public class GameManager : MonoBehaviour
{
    public static GameManager inst;


    [Header("플레이어 정보")]
    public Dictionary<Ability, int> PlayerAbilities = new();
    public int damage;
    public float moveSpeed;
    public float sprintSpeed;
    public int level;
    public float maxExp;
    public float currnetExp;
    public int speed;
    public int[] skillManaCoast = new int[4] { 50, 30, 20, 20 };
    public int levelUpCount;

    [Header("게임 정보")]
    public bool stageClear;
    public RankData rankData = new();
    private string filePath;
    public int score;
    public bool openUi;
    public float time;
    public GameObject[] items;
    public GameObject drone;
    public GameObject dieParticle;
    public GameObject hitParticle;
    public GameObject getParticle;
    public GameObject scoreImage;    

    public GameObject attack;
    public GameObject levelUp;
    public GameObject win;
    public GameObject lose;    
    public GameObject button;    
    void Awake()
    {
        filePath = Application.dataPath + "/" + "rank.txt";
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            if (inst != this)
            {
                Destroy(gameObject);
                return;
            }
        }
        MouseFixed(false);
        LoadData();
    }
    private void Update()
    {        
        if (levelUpCount > 0 &&!openUi)
        {
            Time.timeScale = 0;
            MouseFixed(false);
            openUi = true;
            UiManager.inst.OpenAbility();
            levelUpCount--;
        }
    }
    public void AddScore(int _amount, float _time)
    {
        int origin = score;
        score += _amount;
        StartCoroutine(AnimationScore(origin,_time));
    }
    private IEnumerator AnimationScore(float _origin, float _timer)
    {
        yield return new WaitForSeconds(_timer);
        AnimationText(UiManager.inst.score, score, _origin);
    }
    public void AnimationText(TextMeshProUGUI _text, float _newData, float _oldData)
    {
        if (UiManager.inst.scoreAnimation != null)
        {
            StopCoroutine(UiManager.inst.scoreAnimation);
        }
        else
        {
            StartCoroutine(StartAnimationText(_text, _newData, _oldData));
        }
    }
    private IEnumerator StartAnimationText(TextMeshProUGUI _text, float _newData, float _oldData)
    {
        float data = _oldData;
        for (float i = 0; i < 1; i += Time.unscaledDeltaTime)
        {
            data = Mathf.Lerp(_oldData, _newData, i);
            _text.SetText(string.Format("{0}", data.ToString("N0")));
            yield return null;
        }
        UiManager.inst.scoreAnimation = null;
    }
    public void MouseFixed(bool _fixed)
    {
        Cursor.lockState = _fixed ? CursorLockMode.Locked : CursorLockMode.None;
    }
    public void SubExp(int _amount)
    {
        currnetExp -= _amount;
        currnetExp = Mathf.Clamp(currnetExp, 0, maxExp);
    }
    public void AddExperience(int _amount)
    {
        currnetExp += _amount;
        if (currnetExp >= maxExp)
        {
            LevelUp();
        }
    }
    public void LevelUp()
    {
        Instantiate(levelUp, Vector3.zero, Quaternion.identity);
        currnetExp -= maxExp;
        level++;
        maxExp = (int)(maxExp * 1.25f);
        levelUpCount++;        
    }
    public void SaveData()
    {
        string jsonData = JsonUtility.ToJson(rankData);
        File.WriteAllText(filePath, jsonData);
    }
    public void LoadData()
    {
        if (!File.Exists(filePath))
        {
            SaveData();
        }
        else
        {
            string jsonData = File.ReadAllText(filePath);
            rankData = JsonUtility.FromJson<RankData>(jsonData);

            rankData.rankElements.Sort((a, b) => b.score.CompareTo(a.score));
        }
    }
    public void AddData(int _score, string _name, float _time)
    {
        RankElement rankElement = new();
        rankElement.score = _score;
        rankElement.name = _name;
        rankElement.time = _time;
        rankData.rankElements.Add(rankElement);
        rankData.rankElements.Sort((a, b) => b.score.CompareTo(a.score));
        SaveData();
    }

    
}
