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

    public void OnShowing(int keys = 0, int coins = 0)
    {
        Debug.Log("OnShowing login popup");
        txtName.text = SaveableManager.Instance.GetDisplayNameUser();
        txtCoin.text = coins == 0 ? DataController.Instance.Coins.ToString() : coins.ToString();
        txtKey.text = keys == 0 ? DataController.Instance.Keys.ToString() : keys.ToString();
        Debug.Log(GameManager.Instance.IsLogIn);

        if (GameManager.Instance.IsLogIn)
        {
            btnLogOutFB.SetActive(false);
            btnLogOutGG.SetActive(false);
            btnLogInFB.SetActive(false);
            btnLogInGG.SetActive(false);
            Debug.Log(SaveableManager.Instance.GetProvidersLogin());
            if (SaveableManager.Instance.GetProvidersLogin().Equals(GameDefine.KEY_PROVIDERS_GG)) btnLogOutGG.SetActive(true);
            else if (SaveableManager.Instance.GetProvidersLogin().Equals(GameDefine.KEY_PROVIDERS_FB)) btnLogOutFB.SetActive(true);
            
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
