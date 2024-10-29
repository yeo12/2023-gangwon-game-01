using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct Bar
{
    public Image image;
    public TextMeshProUGUI text;
    public float baseOffset;
}
public class UiManager : MonoBehaviour
{
    public static UiManager inst;

    public List<AbilityCard> abilityCards = new();
    public Transform abilityCardGrid;
    public GameObject abilityScreen;

    public GameObject bossHpBarGameobject;

    public Bar hpBar;
    public Bar bossHpBar;
    public Bar manaBar;
    public Bar speedBar;
    public Bar expBar;

    public GameObject resultScreen;

    public TextMeshProUGUI level;
    public TextMeshProUGUI timer;
    public TextMeshProUGUI score;

    public GameObject clear;
    public GameObject over;

    public GameObject[] skillManaNotEnoughtImage;

    public Transform scoreRoot;
    public Transform noticeMessageRoot;
    public NoticeMessage[] NoticePrefebs;

    public TMP_InputField inputField;
    public GameObject bossLog;
    public Coroutine scoreAnimation;

    public GameObject stage3Button;
    private void Awake()
    {
        inst = this;
    }
    private void Update()
    {
        if (timer != null)
        {
            float time = StageManager.inst.playTime;
            timer.SetText(string.Format("{0:00} : {1:00}", (int)(time / 60), (int)(time % 60)));
        }
        if (score != null)
        {
            score.SetText(string.Format("{0}", GameManager.inst.score.ToString("N0")));
        }
        for (int i = 0; i < skillManaNotEnoughtImage.Length; i++)
        {
            bool active = PlayerController.inst.currentMana < GameManager.inst.skillManaCoast[i];
            if (skillManaNotEnoughtImage[i].activeSelf != active)
                skillManaNotEnoughtImage[i].SetActive(active);
        }
        if (level != null)
        {
            level.SetText(string.Format("LV.{0}", GameManager.inst.level));
        }
        if (hpBar.image != null)
        {
            hpBar.image.fillAmount = (float)PlayerController.inst.health.currentHealth / PlayerController.inst.health.maxHealth;
            hpBar.text.SetText(string.Format("{0} / {1}", PlayerController.inst.health.currentHealth, PlayerController.inst.health.maxHealth));
        }
        if (manaBar.image != null)
        {
            manaBar.image.fillAmount = (float)PlayerController.inst.currentMana / PlayerController.inst.maxMana;
            manaBar.text.SetText(string.Format("{0} / {1}", PlayerController.inst.currentMana, PlayerController.inst.maxMana));
        }
        if (speedBar.image != null)
        {
            speedBar.image.fillAmount = (float)GameManager.inst.speed / 5;
            speedBar.text.SetText(string.Format("{0} / {1}", GameManager.inst.speed, 5));
        }
        if (expBar.image != null)
        {
            expBar.image.fillAmount = (float)GameManager.inst.currnetExp / GameManager.inst.maxExp;
        }
    }
    public void NextStage()
    {
        StageManager.inst.NextStage();
    }
    public void BossCome()
    {
        StartCoroutine(BossLogOn());
    }
    private IEnumerator BossLogOn()
    {
        bossLog.SetActive(true);
        Time.timeScale = 0;
        GameManager.inst.openUi = true;
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1;
        bossHpBarGameobject.SetActive(true);
        GameManager.inst.openUi = false;
        bossLog.SetActive(false);
    }
    public void Stage3Start()
    {
        stage3Button.SetActive(false);
        Time.timeScale = 1;
        GameManager.inst.openUi = false;
    }
    public void AnimationText(TextMeshProUGUI _text, float _newData, float _oldData,Coroutine _coroutine)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        else
        {
            StartCoroutine(StartAnimationText(_text, _newData, _oldData, _coroutine));
        }
    }
    private IEnumerator StartAnimationText(TextMeshProUGUI _text, float _newData, float _oldData, Coroutine _coroutine)
    {
        float data = _oldData;
        for (float i = 0; i < 1; i+=Time.unscaledDeltaTime)
        {
            data = Mathf.Lerp(_oldData, _newData,i);
            _text.SetText(string.Format("{0}", data.ToString("N0")));
            yield return null;
        }
        _coroutine = null;
    }
    public void OpenAbility()
    {
        abilityScreen.SetActive(true);
        for (int i = 0; i < this.abilityCards.Count; i++)
        {
            this.abilityCards[i].gameObject.SetActive(false);
            this.abilityCards[i].transform.SetParent(transform);
        }
        List<AbilityCard> abilityCards = new();
        for (int i = 0; i < 3; i++)
        {
            int index = Random.Range(0, this.abilityCards.Count);
            GameManager.inst.PlayerAbilities.TryGetValue(this.abilityCards[index].ability, out int level);
            if (abilityCards.Contains(this.abilityCards[index]) || level >= 5)
            {
                i--;
            }
            else
            {
                this.abilityCards[index].gameObject.SetActive(true);
                this.abilityCards[index].transform.SetParent(abilityCardGrid);
                abilityCards.Add(this.abilityCards[index]);
            }
        }
    }
}
