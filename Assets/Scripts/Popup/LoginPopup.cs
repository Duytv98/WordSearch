using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtName;
    [SerializeField] private TextMeshProUGUI txtCoin;
    [SerializeField] private TextMeshProUGUI txtKey;
    [SerializeField] private GameObject btnLogInFB;
    [SerializeField] private GameObject btnLogOutFB;
    [SerializeField] private GameObject btnLogInGG;
    [SerializeField] private GameObject btnLogOutGG;
    [SerializeField] private Sprite avatarDefault;
    [SerializeField] private Image avatar;

    public void OnShowing()
    {
        txtName.text = SaveableManager.Instance.GetDisplayNameUser();
        txtCoin.text = DataController.Instance.Coins.ToString();
        txtKey.text = DataController.Instance.Keys.ToString();

        if (GameManager.Instance.IsLogIn)
        {
            btnLogOutFB.SetActive(false);
            btnLogOutGG.SetActive(false);
            btnLogInFB.SetActive(false);
            btnLogInGG.SetActive(false);
            if (SaveableManager.Instance.GetProvidersLogin().Equals(GameDefine.KEY_PROVIDERS_GG))
            {
                Debug.Log("Dang login GG");

                btnLogOutGG.SetActive(true);
            }
            else if (SaveableManager.Instance.GetProvidersLogin().Equals(GameDefine.KEY_PROVIDERS_FB))
            {
                btnLogOutFB.SetActive(true);
                Debug.Log("Dang login Favebook");
            }
            var strAvatar = SaveableManager.Instance.GetAvatar();
            if (!string.IsNullOrEmpty(strAvatar))
            {
                Texture2D tex = Convert.Base64ToTexture(strAvatar);
                avatar.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            }
            else
            {
                avatar.sprite = avatarDefault;
            }
        }
        else
        {
            avatar.sprite = avatarDefault;
            btnLogOutFB.SetActive(false);
            btnLogOutGG.SetActive(false);
            btnLogInFB.SetActive(true);
            btnLogInGG.SetActive(true);
            Debug.Log("Chua Login");
        }
    }
}
