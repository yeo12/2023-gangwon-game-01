using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultScreen : MonoBehaviour
{
    public List<TextMeshProUGUI> values = new();
    public List<TextMeshProUGUI> enemies = new();
    public List<TextMeshProUGUI> items = new();

    private void OnEnable()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].SetText("X 0");
        }
        for (int i = 0; i < items.Count; i++)
        {
            items[i].SetText("X 0");
        }      
        for (int i = 0; i < values.Count; i++)
        {
            values[i].SetText("X 0");
        }
        StartCoroutine(ResultUpdate());
    }
    public IEnumerator ResultUpdate()
    {
        if (StageManager.inst.stage == 3 &&(GameManager.inst.rankData.rankElements.Count < 5|| GameManager.inst.rankData.rankElements[4].score >= GameManager.inst.score))
        {
            UiManager.inst.inputField.gameObject.SetActive(true);
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            if (StageManager.inst.enemyDeathCount.TryGetValue((EnemyType)i,out int count))
            {
                enemies[i].SetText("X {0}", count);
            }
            else
            {
                enemies[i].SetText("X 0");
            }
        }
        UiManager.inst.AnimationText(values[0], StageManager.inst.enemyScore, 0, null);
        for (int i = 0; i < items.Count; i++)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            if (StageManager.inst.itemGetCount.TryGetValue((ItemType)i, out int count))
            {
                items[i].SetText("X {0}", count);
            }
            else
            {
                items[i].SetText("X 0");
            }
        }
        UiManager.inst.AnimationText(values[1], StageManager.inst.itemScore, 0, null);
        yield return new WaitForSecondsRealtime(0.1f);

        UiManager.inst.AnimationText(values[2], StageManager.inst.bouseScore, 0, null);
        yield return new WaitForSecondsRealtime(0.1f);

        float timer = StageManager.inst.playTime;
        values[3].SetText(string.Format("{0:00} : {1:00}", (int)(timer / 60), (int)(timer % 60)));
        yield return new WaitForSecondsRealtime(0.1f);

        UiManager.inst.AnimationText(values[4], GameManager.inst.score, 0, null);

    }

}
