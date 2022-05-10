using UnityEngine;
using UnityEngine.UI;
using PolyAndCode.UI;

public class TestNewCategory : MonoBehaviour, ICell
{
    // Start is called before the first frame update
    [SerializeField] private Text levelText = null;
    private void Start()
    {
        //Can also be done in the inspector
        GetComponent<Button>().onClick.AddListener(ButtonListener);
    }

    // //This is called from the SetCell method in DataSource
    // public void ConfigureCell(ContactInfo contactInfo,int cellIndex)
    // {
    //     levelText.text = contactInfo.Name;
    //     // genderLabel.text = contactInfo.Gender;
    //     // idLabel.text = contactInfo.id;
    // }

    
    private void ButtonListener()
    {
        // Debug.Log("Index : " + _cellIndex +  ", Name : " + _contactInfo.Name  + ", Gender : " + _contactInfo.Gender);
    }
   
}
