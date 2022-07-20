using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UnlockCategoryPopup : MonoBehaviour
{


    private CategoryInfo category = null;

    public void OnShowing(CategoryInfo categoryInfo)
    {
        this.category = categoryInfo;
    }
    public void OnClickUnlockCategoryButton()
    {
        GameManager.Instance.UnlockCategory(this.category);
    }

}
