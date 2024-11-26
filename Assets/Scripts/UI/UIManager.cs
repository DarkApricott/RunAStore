using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(AudioSource))]
public class UIManager : MonoBehaviour
{
    private Animator[] animators;
    private Tutorial tut;
    private bool furnitureState;
    private bool hiringPanelState;

    [Header("UI Objects")]
    [SerializeField] private List<GameObject> menus;
    [SerializeField] private WorkerLibrary workerLibrary;
    [SerializeField] private Toggle playLoadingScreenToggle;
    [SerializeField] private Button stockerbutton;

    [Space]
    [Header("Money")]
    [SerializeField] private TextMeshProUGUI[] finalPrices;
    [SerializeField] private TextMeshProUGUI[] currMoneyText;
    [SerializeField] private TextMeshProUGUI[] currMoneyMinusText;
    [SerializeField] private TMP_Text stockerRequiredText;

    [Space]
    [Header("Happiness")]
    [SerializeField] public Image mainHappinessEmoji;
    [SerializeField] public TextMeshProUGUI mainHapinessPerc;

    #region Stock
    private List<Transform> stockPages = new();
    private int stockPageCounter;

    private int totalStockPrice;
    private int currMoney = 7000;
    public int EarnedMoney;
    private StockItemManager[] stockItems;

    [HideInInspector] public List<int> NewlyBoughtProducts = new();

    private Coroutine scrollCoroutine = null;

    [HideInInspector] public int AmountItemsBought;
    [HideInInspector] public bool HasCheckedOut;
    #endregion

    private List<RectTransform> furniturePages = new();
    private Coroutine swappingFurniturePages;

    private bool skipLoadingUponRestart;

    public bool IsColorPicking;
    [Space]
    [Header("Sound")]
    private AudioSource source;
    [SerializeField] private AudioClip buySound;

    // Initializes time scale
    private void Awake()
    {
        // Bjorn was hier 25/06/2024 13:49
        Time.timeScale = 1;
    }

    // Initializes components and settings
    private void Start()
    {
        source = GetComponent<AudioSource>();
        animators = GetComponentsInChildren<Animator>();
        animators[1] = animators[2];

        tut = FindObjectOfType<Tutorial>();

        if (PlayerPrefs.GetInt("SkipStartLoading") == 1)
        {
            playLoadingScreenToggle.isOn = true;
            PlayAnimation("SkipLoading");
            SkipLoadingScreen();
        }

        foreach (GameObject _screen in menus)
        {
            _screen.SetActive(false);
        }

        // Turns on Play Menu
        menus[0].SetActive(true);

        #region Stock
        stockItems = FindObjectsOfType<StockItemManager>(true);

        for (int i = 1; i < menus[1].transform.GetChild(0).childCount; i++)
        {
            stockPages.Add(menus[1].transform.GetChild(0).GetChild(i));
        }

        stockPages.Reverse();
        #endregion

        for (int i = 2; i < 5; i++)
        {
            furniturePages.Add(menus[2].transform.GetChild(i).GetComponent<RectTransform>());
        }

        UpdateCurrentMoney();

        GainMoney(0);
    }

    // Updates stock page scrolling
    private void Update()
    {
        if (menus[1].activeInHierarchy && scrollCoroutine == null)
        {
            if (Input.mouseScrollDelta.y > 0.2f)
            {
                ScrollStockUpFunction();
            }

            if (Input.mouseScrollDelta.y < -0.2f)
            {
                ScrollStockDownFunction();
            }
        }
    }

    // Scrolls stock page up
    public void ScrollStockUpFunction()
    {
        if (stockPageCounter < stockPages.Count - 1 && menus[1].activeInHierarchy && scrollCoroutine == null)
        {
            scrollCoroutine = StartCoroutine(RotateStockPage(new(1, 0, 0, 0)));
        }
    }

    // Scrolls stock page down
    public void ScrollStockDownFunction()
    {
        if (stockPageCounter > 0 && menus[1].activeInHierarchy && scrollCoroutine == null)
        {
            stockPageCounter--;
            scrollCoroutine = StartCoroutine(RotateStockPage(Quaternion.identity));
        }
    }

    // Changes the active UI screen
    public void ChangeScreen(string _whichScreenOn)
    {
        for (int i = 0; i < menus.Count; i++)
        {
            if (menus[i].name == _whichScreenOn)
            {
                foreach (GameObject _screen in menus)
                {
                    _screen.SetActive(false);
                }

                menus[i].SetActive(true);
            }
        }
    }

    // Plays an animation based on a trigger name
    public void PlayAnimation(string _animTriggerName)
    {
        animators[0].SetTrigger(_animTriggerName);
    }

    // Skips the loading screen
    public void SkipLoadingScreen()
    {
        StartCoroutine(SkipLoadingSceneCoroutine());
    }

    // Coroutine to skip the loading screen
    private IEnumerator SkipLoadingSceneCoroutine()
    {
        animators[1].enabled = false;
        yield return new WaitForSeconds(0.5f);
        animators[1].gameObject.SetActive(false);
    }

    // Updates the current money UI
    private void UpdateCurrentMoney()
    {
        foreach (var _currMoneyText in currMoneyText)
        {
            _currMoneyText.text = "€" + currMoney.ToString();
        }
    }

    // Resets the states of the menu
    public void ResetMenuStates()
    {
        furnitureState = false;
        hiringPanelState = false;
    }

    // Opens the furniture store UI
    public void OpenFurnitureStore()
    {
        hiringPanelState = false;
        furnitureState = !furnitureState;
        UpdateCurrentMoney();

        if (furnitureState)
        {
            ChangeScreen("Furniture");
            menus[0].SetActive(true);
            PlayAnimation("OpenFurniture");
        }
        else
        {
            PlayAnimation("CloseFurniture");
            StartCoroutine(DelayedPlaymenuLoad());
        }
    }

    // Coroutine to delay loading of the play menu
    private IEnumerator DelayedPlaymenuLoad()
    {
        yield return new WaitForSeconds(0.4f);
        ChangeScreen("PlayMenu");
    }

    // Opens the hiring UI
    public void OpenHiring()
    {
        furnitureState = false;
        hiringPanelState = !hiringPanelState;
        UpdateCurrentMoney();

        if (hiringPanelState)
        {
            ChangeScreen("Workers");
            menus[0].SetActive(true);
            PlayAnimation("OpenHiring");
        }
        else
        {
            PlayAnimation("CloseHiring");
            StartCoroutine(DelayedPlaymenuLoad());
        }
    }

    // Hires a worker if there's enough money
    public void HireWorker(string _job)
    {
        for (int i = 0; i < workerLibrary.Workers.Length; i++)
        {
            if (_job == workerLibrary.Workers[i].WorkerJob && currMoney >= workerLibrary.Workers[i].WorkerPrice)
            {
                SpendMoney(workerLibrary.Workers[i].WorkerPrice);

                if (currMoney < workerLibrary.Workers[i].WorkerPrice)
                {
                    ResetMinusTexts();
                }
            }
        }
    }

    // Spends money and updates the UI
    public void SpendMoney(int _price)
    {
        currMoney -= _price;
        currMoneyMinusText[1].text = "-" + _price.ToString();
        PlayAnimation("SpendMoney");
        UpdateCurrentMoney();
    }

    // Resets the minus money texts
    public void ResetMinusTexts()
    {
        foreach (var _text in currMoneyMinusText)
        {
            _text.text = "";
        }
    }

    // Restarts the game by reloading the scene
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    // Checks if there's enough money for a given price
    public bool HasEnoughMoney(int _price)
    {
        return _price <= currMoney;
    }

    // Gains money and updates the UI
    public void GainMoney(int _price)
    {
        currMoney += _price;
        EarnedMoney += _price;

        int stocekrHireLock = 250;

        if (EarnedMoney >= stocekrHireLock)
        {
            stockerbutton.interactable = true;
            stockerRequiredText.gameObject.SetActive(false);
        }
        else
        {
            stockerRequiredText.text = "Earn €" + (stocekrHireLock - EarnedMoney) + ",- more to unlock!";
        }

        foreach (FurnitureButton _furnButton in FindObjectsOfType<FurnitureButton>())
        {
            _furnButton.LockedFurnitureUpdater();
        }

        foreach (StockItemManager _stockItem in FindObjectsOfType<StockItemManager>())
        {
            _stockItem.LockedStockUpdater();
        }

        UpdateCurrentMoney();
    }

    #region Stock
    // Coroutine to rotate the stock page
    private IEnumerator RotateStockPage(Quaternion _endRot)
    {
        Quaternion _startRot = stockPages[stockPageCounter].transform.rotation;

        var t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime / 1f;
            stockPages[stockPageCounter].transform.rotation = Quaternion.Lerp(_startRot, _endRot, t);
            yield return null;
        }

        transform.rotation = _endRot;
        stockPageCounter += (int)_endRot.x;
        yield return new WaitForEndOfFrame();
        scrollCoroutine = null;
    }

    // Updates the total stock price
    public void TotalStockPriceUpdater(int _addedPrice)
    {
        totalStockPrice += _addedPrice;
        foreach (var _price in finalPrices)
        {
            _price.text = "Total Price        €" + totalStockPrice.ToString();
        }
    }

    // Handles the stock checkout process
    public void CheckoutStock()
    {
        if (currMoney >= totalStockPrice)
        {
            currMoney -= totalStockPrice;
            UpdateCurrentMoney();

            if (totalStockPrice > 0)
            {
                currMoneyMinusText[0].text = "-" + totalStockPrice.ToString();

                if (tut.DoingTutorial)
                {
                    tut.StepDone(1);
                    tut.NextStep(3);
                    tut.HelpingTexts(1);
                }
            }

            PlayAnimation("CloseStock");
            ResetMenuStates();
            HasCheckedOut = true;

            source.PlayOneShot(buySound);
        }
        else
        {
            HasCheckedOut = false;
        }
    }

    // Resets the stock settings and UI
    public void ResetStock()
    {
        UpdateCurrentMoney();
        AmountItemsBought = 0;
        totalStockPrice = 0;
        foreach (TextMeshProUGUI _finalPrice in finalPrices)
        {
            _finalPrice.text = "Total Price        €";
        }

        foreach (var _stockItemMan in stockItems)
        {
            _stockItemMan.Reset();
        }
    }

    // Cancels the current stock order
    public void CancelOrder()
    {
        PlayAnimation("CloseStock");
        AmountItemsBought = 0;
        ResetMenuStates();
    }
    #endregion

    // Swaps between furniture categories
    public void SwapFurnCatagory(string _catagory)
    {
        RectTransform _currFurniturePage = GetComponent<RectTransform>();
        RectTransform _nextFurniturePage = GetComponent<RectTransform>();
        int _lastFurnitureCat = 0;
        int _nextFurnitureCat = 0;

        for (int i = 0; i < furniturePages.Count; i++)
        {
            if (furniturePages[i].gameObject.activeInHierarchy)
            {
                _currFurniturePage = furniturePages[i];
                _lastFurnitureCat = i;
            }

            if (furniturePages[i].name == _catagory)
            {
                _nextFurniturePage = furniturePages[i];
                _nextFurnitureCat = i;
            }
        }


        if (!_nextFurniturePage.gameObject.activeInHierarchy && swappingFurniturePages == null)
        {
            if (_lastFurnitureCat > _nextFurnitureCat)
            {
                swappingFurniturePages = StartCoroutine(SwapFurnitureCatagory(_currFurniturePage, _nextFurniturePage, false));
            }
            else
            {
                swappingFurniturePages = StartCoroutine(SwapFurnitureCatagory(_currFurniturePage, _nextFurniturePage, true));
            }
        }
    }

    // Coroutine to swap between furniture categories
    private IEnumerator SwapFurnitureCatagory(RectTransform _currFurniturePage, RectTransform _nextFurniturePage, bool _swipeLeft)
    {
        Vector3 _endPosCur;
        Vector3 _endPosNext;

        if (_swipeLeft)
        {
            _endPosCur = new(0, 1745, 0);
            _endPosNext = new(0, -1745, 0);
        }
        else
        {
            _endPosCur = new(0, -1745, 0);
            _endPosNext = new(0, 1745, 0);
        }

        _nextFurniturePage.gameObject.SetActive(true);

        var t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime / 0.35f;
            _currFurniturePage.anchoredPosition3D = Vector3.Lerp(Vector3.zero, _endPosCur, t);
            _nextFurniturePage.anchoredPosition3D = Vector3.Lerp(_endPosNext, Vector3.zero, t);
            yield return null;
        }

        _currFurniturePage.anchoredPosition3D = _endPosCur;
        _nextFurniturePage.anchoredPosition3D = Vector3.zero;

        _currFurniturePage.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();

        swappingFurniturePages = null;
    }

    // Toggles the skip loading preference
    public void SetIfSkipLoading()
    {
        skipLoadingUponRestart = !skipLoadingUponRestart;
        PlayerPrefs.SetInt("SkipStartLoading", Convert.ToInt32(skipLoadingUponRestart));
    }

    // Toggles color picker mode
    public void DoColorPicker()
    {
        IsColorPicking = !IsColorPicking;
    }

    // Pauses or resumes the game
    public void PauseGame()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }
}