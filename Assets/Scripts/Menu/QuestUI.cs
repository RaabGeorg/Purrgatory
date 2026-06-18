using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance { get; private set; }

    [Header("Dialog")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button declineButton;
    [SerializeField] private Button closeButton;
    
    

    [Header("Progress HUD")]
    [SerializeField] private GameObject progressPanel;
    [SerializeField] private TextMeshProUGUI progressText;
    
    private QuestData _pendingQuest;
    
    

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        
        acceptButton.onClick.AddListener(OnAccept);
        declineButton.onClick.AddListener(OnDecline);
        closeButton.onClick.AddListener(OnClose);
    }

    private void OnEnable()
    {
        GameEvents.OnQuestProgressChanged += UpdateProgressText;
        GameEvents.OnQuestCompleted += HandleQuestCompleted;
    }

    private void OnDisable()
    {
        GameEvents.OnQuestProgressChanged -= UpdateProgressText;
        GameEvents.OnQuestCompleted -= HandleQuestCompleted;
    }

    public void ShowDialog(QuestData quest)
    {
        dialogPanel.SetActive(true);
        
        acceptButton.gameObject.SetActive(false);
        declineButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);

        if (QuestManager.Instance.IsQuestComplete)
        {
            dialogText.text = $"{quest.questName} complete!";
            closeButton.gameObject.SetActive(true);
        }
        else if (QuestManager.Instance.IsQuestActive)
        {
            dialogText.text = $"You haven't completed the quest yet!\n{QuestManager.Instance.GetProgressText()}";
            closeButton.gameObject.SetActive(true);
        }
        else
        {
            _pendingQuest = quest;
            dialogText.text = quest.description;
            acceptButton.gameObject.SetActive(true);
            declineButton.gameObject.SetActive(true);
        }
    }

    public void HideDialog()
    {
        dialogPanel.SetActive(false);
        _pendingQuest = null;
    }

    private void OnAccept()
    {
        if (_pendingQuest == null) return;
        QuestManager.Instance.StartQuest(_pendingQuest);
        _pendingQuest = null;
        acceptButton.gameObject.SetActive(false);
        declineButton.gameObject.SetActive(false);
        progressPanel.SetActive(true);
        UpdateProgressText();
        HideDialog();
    }

    private void OnDecline()
    {
        HideDialog();
    }

    private void OnClose()
    {
        HideDialog();
    }

    private void UpdateProgressText()
    {
        if (progressText != null)
            progressText.text = QuestManager.Instance.GetProgressText();
    }

    private void HandleQuestCompleted()
    {
        if (progressPanel != null)
            progressPanel.SetActive(false);
        OnQuestRewardGranted(QuestManager.Instance.ActiveQuest);
    }
    
    private void OnQuestRewardGranted(QuestData quest)
    {
        // TODO: weapon mod reward
    }
}