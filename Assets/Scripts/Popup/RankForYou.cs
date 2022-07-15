using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankForYou : MonoBehaviour
{

    [SerializeField] private Sprite[] spritesBorder = null;
    [SerializeField] private Sprite[] spritesMedal = null;

    [SerializeField] private Image medal;
    [SerializeField] private Image border;
    [SerializeField] private TextMeshProUGUI txtSTT;
    [SerializeField] private TextMeshProUGUI txtName;
    [SerializeField] private TextMeshProUGUI score;
    public void SetData(int index, PlayerLB data)
    {
        SetActiveDefault();

        if (index < 3)
        {
            medal.sprite = spritesMedal[index];
            medal.SetNativeSize();
            border.sprite = spritesBorder[index];
            border.SetNativeSize();
        }
        else
        {
            medal.gameObject.SetActive(false);
            border.gameObject.SetActive(false);
            txtSTT.gameObject.SetActive(true);
            txtSTT.text = (index + 1).ToString();
        }

        txtName.text = data.name;
        score.text = data.score.ToString();
    }
    private void SetActiveDefault()
    {
        medal.gameObject.SetActive(true);
        border.gameObject.SetActive(true);
        txtSTT.gameObject.SetActive(false);
        txtName.gameObject.SetActive(true);
        score.gameObject.SetActive(true);
    }
}
