using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class MainManager : MonoBehaviour
{
    public List<TextMeshProUGUI> names = new();
    public List<TextMeshProUGUI> times = new();
    public List<TextMeshProUGUI> scores = new();

    public List<GameObject> pages = new();
    public void StartGame()
    {
        SceneManager.LoadScene("Stage1");
    }
    public void StartPage()
    {
        Close();
        pages[0].SetActive(true);
    }
    public void GuidePage()
    {
        Close();
        pages[1].SetActive(true);
    }
    public void IntroPage()
    {
        Close();
        pages[2].SetActive(true);
    }
    public void RankPage()
    {
        Close();
        RankUpdate();
        pages[3].SetActive(true);
    }
    public void Close()
    {
        Instantiate(GameManager.inst.button);
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(false);
        }
    }
    public void RankUpdate()
    {
        for (int i = 0; i < 5; i++)
        {
            names[i].SetText(string.Format("_ _ _"));
            times[i].SetText(string.Format("00 : 00"));
            scores[i].SetText(string.Format("0"));
        }
        for (int i = 0; i < GameManager.inst.rankData.rankElements.Count; i++)
        {
            names[i].SetText(string.Format("{0}", GameManager.inst.rankData.rankElements[i].name));
            float timer = GameManager.inst.rankData.rankElements[i].time;
            times[i].SetText(string.Format("{0:00} : {1:00}", (int)(timer / 60), (int)(timer % 60)));
            scores[i].SetText(string.Format("{0}", GameManager.inst.rankData.rankElements[i].score));
        }
    }
    public void ExitPage()
    {
        Application.Quit();
    }

}
