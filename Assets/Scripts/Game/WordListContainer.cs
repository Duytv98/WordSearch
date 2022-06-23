using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using DG.Tweening;

public class WordListContainer : MonoBehaviour
{
    [SerializeField] private RectTransform wordListItemPrefab = null;

    [SerializeField] private RectTransform wordListContainer = null;
    private Dictionary<string, WordListItem> wordListItems;
    private List<RectTransform> rowWordLists = null;
    private List<string> listWordUse = null;

    public HashSet<string> listWordDeleted;
    [SerializeField] private Text textPlus = null;
    [SerializeField] private TextMeshProUGUI textPlusPro = null;
    [SerializeField] private Transform star = null;
    [SerializeField] private Transform starPrefab = null;

    private List<Transform> listStar;
    private HashSet<string> unusedWords;

    public HashSet<string> UnusedWords { get => unusedWords; set => unusedWords = value; }

    private int indexWordMin = 0;

    // Start is called before the first frame update
    public void Initialize()
    {
        wordListItems = new Dictionary<string, WordListItem>();
        rowWordLists = new List<RectTransform>();
        listWordUse = new List<string>();
        listStar = new List<Transform>();
        // UnusedWords = new HashSet<string>();
        // listWordDeleted = new HashSet<string>();
    }
    public void Setup(Board board)
    {
        UnusedWords = new HashSet<string>();
        listWordDeleted = board.listWordDeleted;
        // GameManager.Instance.LogHashSetString(listWordDeleted);
        Clear();
        board.ShuffleListString();
        foreach (var word in board.words)
        {
            CreateWordListItem(word);
        }
        // Debug.Log("board.words.count: " + board.words.Count + "   wordListItems.count: " + wordListItems.Count);
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
                widthWordList += (_wordItemScript.GetWidthSize() + 40f);
                if (widthWordList >= rowWordLists[indexRow].rect.width || index > (indexRow * 4) + 3)
                {
                    indexRow++;
                    widthWordList = (_wordItemScript.GetWidthSize() + 40f);
                }
                if (indexRow < rowWordLists.Count)
                {
                    _wordItemScript.SetParent(rowWordLists[indexRow]);
                    _wordItemScript.SetAlpha(true);
                }
                else
                {
                    _wordItemScript.gameObject.SetActive(false);
                    UnusedWords.Add(_wordItemScript.Word);
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

                widthWordList += (_wordItemScript.GetWidthSize() + 40f);
                if (widthWordList >= rowWordLists[0].rect.width)
                {
                    indexRow++;
                    widthWordList = (_wordItemScript.GetWidthSize() + 40f);
                }
                if (indexRow < rowWordLists.Count)
                {
                    _wordItemScript.SetParent(rowWordLists[indexRow]);
                    _wordItemScript.SetAlpha(true);
                }
                else
                {
                    _wordItemScript.gameObject.SetActive(false);
                    UnusedWords.Add(_wordItemScript.Word);
                }
                index++;
            }
        }

        // Debug.Log("Tổng số chữ: " + wordListItems.Count);
        // Debug.Log("Số lượng chữ chưa sử dụng: " + UnusedWord.Count);

        // Debug.Log("row: " + rowWordLists.Count);
        var VLG = wordListContainer.GetComponent<VerticalLayoutGroup>();
        if (rowWordLists.Count == 3)
        {
            VLG.padding.top = 35;
            VLG.padding.bottom = 25;
        }
        CreateStar(UnusedWords.Count);
        // textPlus.text = "+ " + UnusedWords.Count;
        textPlusPro.text  = "+ " + UnusedWords.Count;

    }

    public void SetWordFound(string word)
    {
        if (wordListItems.ContainsKey(word))
        {
            wordListItems[word].SetWordFound();
            var position = wordListItems[word].transform.position;
        }
        else
        {
            Debug.LogError("[WordList] Word does not exist in the word list: " + word);
        }
    }
    public void SetWordRecommend(string word, int indexColor)
    {
        if (wordListItems.ContainsKey(word)) wordListItems[word].SetRecommendWord(indexColor);
        else Debug.LogError("[WordList] Word does not exist in the word list: " + word);
    }
    public Vector3 GetPositionWord(string word)
    {
        if (wordListItems.ContainsKey(word))
        {
            return wordListItems[word].transform.TransformPoint(Vector3.zero);
        }
        else
        {
            Debug.LogError("[WordList] Word does not exist in the word list: " + word);
            return Vector3.zero;
        }
    }
    public void PlusWord(HashSet<string> foundWords)
    {
        if (UnusedWords.Count <= 0)
        {
            return;
        }
        List<string> listWordDie = new List<string>();
        int soTuDuaRa = UnusedWords.Count >= 3 ? 3 : UnusedWords.Count;
        // Debug.Log("foundWords.Count: " + foundWords.Count + "       soTuDuaRa * amountMinus: " + (soTuDuaRa * amountMinus));
        if (foundWords.Count >= indexWordMin + soTuDuaRa)
        {
            int w = 0;
            // Debug.Log("min: " + (indexWordMin) + "  max: " + (indexWordMin + soTuDuaRa));
            foreach (var word in foundWords)
            {
                // Debug.Log("word: " + word);
                if (w < indexWordMin || w >= indexWordMin + soTuDuaRa)
                {
                    w++;
                    continue;
                }
                if (wordListItems.ContainsKey(word)) listWordDie.Add(word);
                w++;
            }
            // Debug.Log("listPositionEnd.Count: " + listWordDie.Count);
            // Debug.Log("listStar.Count: " + listStar.Count);
            int index = 0;

            indexWordMin += soTuDuaRa;
            foreach (var reuseWord in UnusedWords)
            {
                if (index >= listWordDie.Count) return;
                WordListItem wordDieScript = wordListItems[listWordDie[index]];
                // Debug.Log("word: " + wordDieScript.Word);
                WordListItem reuseWordScript = wordListItems[reuseWord];
                // SwapWord(wordDieScript, reuseWord, wordDieScript.transform.parent, position);
                MoveStar(wordDieScript, reuseWordScript, index, index == listWordDie.Count - 1);

                index++;
            }
            // Debug.Log(55555);
        }

    }
    private void MoveStar(WordListItem wordDieScript, WordListItem reuseWordScript, int index, bool isLastStar)
    {
        // Debug.Log("word remove: " + wordDieScript.Word + "    new Word: " + reuseWordScript.Word + "   index: " + index + "   sao cuối: " + isLastStar);
        Vector3 position = wordDieScript.transform.position;
        Transform transformStar = listStar[index];
        transformStar.DOMove(position, 1f)
                                  .SetEase(Ease.InCubic)
                                  .OnComplete(() =>
                                  {
                                      //   Debug.Log("i: " + index);
                                      Destroy(transformStar.gameObject);
                                      SwapWord(wordDieScript, reuseWordScript);
                                      if (isLastStar)
                                      {
                                          listStar.RemoveRange(0, index + 1);
                                          GameManager.Instance.SaveCurrentBoard();
                                        //   Debug.Log("listStar count: " + listStar.Count);
                                      }
                                  });
    }



    private void CreateWordListItem(string word)
    {
        if (listWordDeleted.Contains(word)) return;
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
        foreach (var item in rowWordLists)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform star in listStar)
        {
            Destroy(star.gameObject);
        }
        wordListItems.Clear();
        listStar.Clear();
        rowWordLists.Clear();
        indexWordMin = 0;


    }

    private bool IsVisible(RectTransform placeholder)
    {
        RectTransform viewport = wordListContainer.GetComponent<RectTransform>();

        float placeholderTop = -placeholder.anchoredPosition.y - placeholder.rect.height / 2f;
        float placeholderbottom = -placeholder.anchoredPosition.y + placeholder.rect.height / 2f;

        float viewportTop = viewport.anchoredPosition.y;
        float viewportbottom = viewport.anchoredPosition.y + viewport.rect.height;
        // Debug.Log("viewportTop: " + viewportTop + "   viewportbottom: " + viewportbottom + "  placeholder.rect.height: " + placeholder.rect.height);
        // Debug.Log("placeholderTop: " + placeholderTop + "   placeholderbottom: " + placeholderbottom + "   viewport.rect.height: " + viewport.rect.height);

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
        // Debug.Log("row: " + row);
        for (int i = 0; i < row; i++)
        {
            RectTransform rowWordList = CreateContainer("Row_Word_List_" + i, typeof(RectTransform));
            HorizontalLayoutGroup horizontalLayoutGroup = rowWordList.gameObject.AddComponent<HorizontalLayoutGroup>();
            horizontalLayoutGroup.childControlHeight = false;
            horizontalLayoutGroup.childControlWidth = true;
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
            Transform starTr = Instantiate(starPrefab, Vector3.zero, Quaternion.identity, star);
            starTr.localPosition = Vector3.zero;
            listStar.Add(starTr);
        }
    }
    private void SwapWord(WordListItem wordDieScript, WordListItem reuseWordScript)
    {
        Vector3 position = wordDieScript.transform.position;
        wordDieScript.gameObject.SetActive(false);
        Destroy(wordDieScript.gameObject);
        listWordDeleted.Add(wordDieScript.Word);
        GameManager.Instance.AddWordDeleted(wordDieScript.Word);
        // Debug.Log("2.  listWordDeleted.Count: " + listWordDeleted.Count);
        wordListItems.Remove(wordDieScript.Word);

        reuseWordScript.gameObject.SetActive(true);
        reuseWordScript.transform.localPosition = position;
        reuseWordScript.SetParent(wordDieScript.transform.parent);
        reuseWordScript.SetAlpha(true);

        UnusedWords.Remove(reuseWordScript.Word);
        textPlus.text = "+ " + UnusedWords.Count;
        textPlusPro.text = "+ " + UnusedWords.Count;


    }
}
