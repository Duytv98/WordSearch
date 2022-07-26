using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UnlockCategoryPopup : MonoBehaviour
{


    private CategoryInfo _category = null;
    [SerializeField] Text txtUnLock;

    public void OnShowing(CategoryInfo categoryInfo)
    {
        _category = categoryInfo;
        var str =  _category.unlockAmount.ToString();
        txtUnLock.text = string.Format("U  {0} K\nT Z   ?", _category.unlockAmount);
    }
    public void OnClickUnlockCategoryButton()
    {
        GameManager.Instance.UnlockCategory(_category);
    }

}
