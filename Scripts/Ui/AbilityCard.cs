using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Ability ability;
    public bool readOnly;
    public bool leveless;
    public GameObject[] stars;

    public void OnEnable()
    {
        if (leveless) return;
        if (GameManager.inst.PlayerAbilities.TryGetValue(ability, out int count))
        {
            for (int i = 0; i < count; i++)
            {
                stars[i].SetActive(true);
            }
        }
        transform.localScale = Vector3.one;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (readOnly) return;
        UiManager.inst.abilityScreen.SetActive(false);
        Instantiate(GameManager.inst.button);
        GameManager.inst.openUi = false;
        Time.timeScale = 1;
        GameManager.inst.MouseFixed(true);

        if (leveless)
        {
            switch (ability)
            {
                case Ability.Healing:
                    PlayerController.inst.health.Heal(Random.Range(10,21));
                    break;
                case Ability.Mana:
                    PlayerController.inst.UpdateMana(Random.Range(10, 21));
                    break;
                case Ability.Score:
                    GameManager.inst.AddScore(Random.Range(100, 1001),0);
                    break;
                case Ability.Item:
                    Instantiate(GameManager.inst.items[Random.Range(0, GameManager.inst.items.Length)], PlayerController.inst.transform.position + new Vector3(Random.Range(0, 6), 1, Random.Range(0, 6)), Quaternion.identity);
                    break;
            }
        }
        else
        {
            if (ability == Ability.Drone)
            {
                StageManager.inst.CreateDrone();
            }
            if (GameManager.inst.PlayerAbilities.ContainsKey(ability))
            {
                GameManager.inst.PlayerAbilities[ability]++;
            }
            else
            {
                GameManager.inst.PlayerAbilities.Add(ability, 1);

            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (readOnly) return;
        transform.localScale = Vector3.one * 1.1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (readOnly) return;
        transform.localScale = Vector3.one;
    }
}
