using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class WordListContainer : MonoBehaviour
{
    [SerializeField] private RectTransform wordListItemPrefab = null;

    [SerializeField] private RectTransform wordListContainer = null;


    private Dictionary<string, WordListItem> wordListItems;
    private List<RectTransform> rowWordLists = null;
    private List<string> listWordUse = null;


    [SerializeField] private Text textPlus = null;
    [SerializeField] private Transform star = null;
    [SerializeField] private Transform starPrefab = null;

    private List<Transform> listStar;
    private HashSet<string> unusedWord;

    public HashSet<string> UnusedWord { get => unusedWord; set => unusedWord = value; }

    // Start is called before the first frame update


    public void Initialize()
    {
        wordListItems = new Dictionary<string, WordListItem>();
        rowWordLists = new List<RectTransform>();
        listWordUse = new List<string>();
        listStar = new List<Transform>();
        UnusedWord = new HashSet<string>();
    }
    public void Setup(Board board)
    {

        UnusedWord = new HashSet<string>();
        Debug.Log("Số lượng chữ: " + board.words.Count);
        Clear();
        // Debug.Log("WordListContainer Setup");
        board.ShuffleListString();
        foreach (var word in board.words)
        {
            CreateWordListItem(word);
        }
        Canvas.ForceUpdateCanvases();
        CreateRowWordList(3);
        float phantram = GetTotalWidthWordList() / (wordListContainer.rect.width * 3);

        if (board.words.Count <= 12)
        {
            int index = 0;
            float widthWordList = 0f;
            int indexRow = 0;
            foreach (var item in wordListItems)
            {
                WordListItem _wordItemScript = item.Value;
                RectTransform _wordItemRecT = _wordItemScript.GetComponent<RectTransform>();
                widthWordList += (_wordItemRecT.sizeDelta.x + 40f);
                if (widthWordList >= rowWordLists[indexRow].rect.width || index > (indexRow * 4) + 3)
                {
                    indexRow++;
                    widthWordList = (_wordItemRecT.sizeDelta.x + 40f);
                }
                if (indexRow < rowWordLists.Count)
                {
                    _wordItemScript.SetParent(rowWordLists[indexRow]);
                    _wordItemScript.SetAlpha(true);
                }
                else
                {
                    _wordItemScript.gameObject.SetActive(false);
                    UnusedWord.Add(_wordItemScript.Word);
                }
                index++;
            }
        }
        else
        {
            int index = 0;
            float widthWordList = 0f;
            int indexRow = 0;
            foreach (var item in wordListItems)
            {
                WordListItem _wordItemScript = item.Value;
                RectTransform _wordItemRecT = _wordItemScript.GetComponent<RectTransform>();
                widthWordList += (_wordItemRecT.sizeDelta.x + 40f);
                if (widthWordList >= rowWordLists[0].rect.width)
                {
                    indexRow++;
                    widthWordList = (_wordItemRecT.sizeDelta.x + 40f);
                }
                if (indexRow < rowWordLists.Count)
                {
                    _wordItemScript.SetParent(rowWordLists[indexRow]);
                    _wordItemScript.SetAlpha(true);
                }
                else
                {
                    _wordItemScript.gameObject.SetActive(false);
                    UnusedWord.Add(_wordItemScript.Word);
                }
                index++;
            }
        }

        Debug.Log("Tổng số chữ: " + wordListItems.Count);
        Debug.Log("Số lượng chữ chưa sử dụng: " + UnusedWord.Count);

        Debug.Log("row: " + rowWordLists.Count);
        var VLG = wordListContainer.GetComponent<VerticalLayoutGroup>();
        if (rowWordLists.Count == 3)
        {
            VLG.padding.top = 35;
            VLG.padding.bottom = 25;
        }
        CreateStar(UnusedWord.Count);
        textPlus.text = "+ " + UnusedWord.Count;

    }

    public void SetWordFound(string word)
    {
        if (wordListItems.ContainsKey(word))
        {
            wordListItems[word].SetWordFound();
            var position = wordListItems[word].transform.position;
            // Debug.Log("key: " + word + "  wort: " + wordListItems[word].Word + "  position: " + position);
            // plus.transform.DOJump(position, 30, 1, 0.5f, true);
        }
        else
        {
            Debug.LogError("[WordList] Word does not exist in the word list: " + word);
        }
    }
    public void PlusWord(HashSet<string> foundWords)
    {
        if (UnusedWord.Count > 5) Debug.Log("Có hơn 5 từ chưa được hiển thị");
        if (foundWords.Count >= UnusedWord.Count || foundWords.Count >= 3 && UnusedWord.Count > 5)
        {
            // Debug.Log("số từ đã tìm thấy: " + foundWords.Count);
            // Debug.Log("Kích hoạt sao");
        }
    }



    private void CreateWordListItem(string word)
    {

        if (!wordListItems.ContainsKey(word))
        {
            RectTransform _wordItem = Instantiate(wordListItemPrefab, Vector3.zero, Quaternion.identity, wordListContainer);
            // _wordItem.gameObject.SetActive(false);

            _wordItem.localPosition = Vector3.zero;

            WordListItem _wordItemScript = _wordItem.GetComponent<WordListItem>();
            _wordItemScript.Setup(word);
            _wordItemScript.SetAlpha(false);

            wordListItems.Add(word, _wordItemScript);
        }
        else
        {
            Debug.LogWarning("[WordList] Board contains duplicate words. Word: " + word);
        }

        // return _wordItemScript;
    }
    // Update is called once per frame
    public void Clear()
    {
        foreach (KeyValuePair<string, WordListItem> item in wordListItems)
        {
            Destroy(item.Value.gameObject);
        }
        wordListItems.Clear();
        foreach (var item in rowWordLists)
        {
            Destroy(item.gameObject);
        }

        rowWordLists.Clear();


    }

    private bool IsVisible(RectTransform placeholder)
    {
        RectTransform viewport = wordListContainer.GetComponent<RectTransform>();

        float placeholderTop = -placeholder.anchoredPosition.y - placeholder.rect.height / 2f;
        float placeholderbottom = -placeholder.anchoredPosition.y + placeholder.rect.height / 2f;

        float viewportTop = viewport.anchoredPosition.y;
        float viewportbottom = viewport.anchoredPosition.y + viewport.rect.height;
        Debug.Log("viewportTop: " + viewportTop + "   viewportbottom: " + viewportbottom + "  placeholder.rect.height: " + placeholder.rect.height);
        Debug.Log("placeholderTop: " + placeholderTop + "   placeholderbottom: " + placeholderbottom + "   viewport.rect.height: " + viewport.rect.height);

        return placeholderTop < viewportbottom && placeholderbottom > viewportTop;
    }

    private RectTransform CreateContainer(string name, params System.Type[] types)
    {
        GameObject containerObj = new GameObject(name, types);
        RectTransform container = containerObj.GetComponent<RectTransform>();

        container.SetParent(wordListContainer, false);
        container.anchoredPosition = Vector2.zero;
        container.anchorMin = Vector2.up;
        container.anchorMax = Vector2.one;
        container.offsetMin = Vector2.zero;
        container.offsetMax = Vector2.zero;
        container.sizeDelta = new Vector2(container.sizeDelta.x, 54f);


        return container;
    }
    private float GetTotalWidthWordList()
    {
        float totalWidthWordList = 0f;
        foreach (var item in wordListItems)
        {
            WordListItem _wordItemScript = item.Value;
            RectTransform _wordItemRecT = _wordItemScript.GetComponent<RectTransform>();
            totalWidthWordList += (_wordItemRecT.sizeDelta.x + 100f);
        }
        return totalWidthWordList;
    }
    private void CreateRowWordList(int row)
    {
        Debug.Log("row: " + row);
        for (int i = 0; i < row; i++)
        {
            RectTransform rowWordList = CreateContainer("Row Word List", typeof(RectTransform));
            HorizontalLayoutGroup horizontalLayoutGroup = rowWordList.gameObject.AddComponent<HorizontalLayoutGroup>();
            horizontalLayoutGroup.childControlHeight = false;
            horizontalLayoutGroup.childControlWidth = false;
            horizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            horizontalLayoutGroup.spacing = 50f;
            rowWordLists.Add(rowWordList);
        }
    }
    private void RemoveEmptyRow(List<RectTransform> listRows)
    {
        for (int i = 0; i < listRows.Count; i++)
        {
            var row = listRows[i];
            if (row.childCount == 0)
            {
                Destroy(row.gameObject);
                listRows.RemoveAt(i);
            }
        }

    }
    private void CreateStar(int count)
    {
        for (int i = 0; i < count; i++)
        {

        }
    }

}
