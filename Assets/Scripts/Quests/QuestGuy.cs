using UnityEngine;

public class QuestGuy : MonoBehaviour
{
    [SerializeField] private QuestData weaponQuest;
    [SerializeField] private QuestData bossQuest;
    private bool _playerInRange;
    private PlayerControls _controls;

    private void Awake()
    {
        _controls = new PlayerControls();
    }

    private void OnEnable() => _controls.Enable();

    private void OnDisable()
    {
        _controls.Disable();
        if (_playerInRange) QuestUI.Instance.HideDialog();
    }

    private void OnDestroy() => _controls.Dispose();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) _playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
            QuestUI.Instance.HideDialog();
        }
    }

    private void Update()
    {
        if (!_playerInRange) return;
        if (!_controls.Player.Interact.WasPressedThisFrame()) return;
        
        QuestData giving = QuestManager.Instance.LastCompletedQuest == weaponQuest
            ? weaponQuest : QuestManager.Instance.QuestCompleted >= 1 ? bossQuest : weaponQuest;
        QuestUI.Instance.ShowDialog(giving);
    }
}