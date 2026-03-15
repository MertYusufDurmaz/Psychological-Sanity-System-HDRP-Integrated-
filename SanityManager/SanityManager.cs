using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Events;

public class SanityManager : MonoBehaviour
{
    public static SanityManager Instance { get; private set; }

    [Header("Settings")]
    public float maxSanity = 100f;
    [SerializeField] private float currentSanity;
    public bool isPlayerHidden = false;

    private bool isDead = false;

    [Header("Warning Settings")]
    public float lowSanityThreshold = 25f;
    private bool hasShownLowSanityWarning = false;

    [Header("UI Colors")]
    public Color fullSanityColor = new Color32(38, 38, 38, 255);
    public Color emptySanityColor = Color.red;

    [Header("Decay Rates")]
    public float passiveDecay = 0.5f;
    public float darknessDecay = 2.0f;
    public float chaseDecay = 10.0f;
    public float enemyNearDecay = 5.0f;

    [Header("Dependencies")]
    public Image sanityBarFill;
    public Volume insanityVolume;
    [Tooltip("Sahnede akıl sağlığını etkileyecek tüm düşmanları buraya ekleyin.")]
    public EnemyAI[] enemies;
    public FlashlightController flashlight;

    [Header("Events (Manager Entegrasyonları)")]
    [Tooltip("Akıl sağlığı 0 olduğunda tetiklenir (Örn: GameOverManager.ShowGameOverScreen)")]
    public UnityEvent onSanityDepleted;
    
    [Tooltip("Akıl sağlığı kritik seviyeye düştüğünde tetiklenir (Örn: NotificationManager.ShowNotification)")]
    public UnityEvent<string> onLowSanityWarning;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        currentSanity = maxSanity;
        isDead = false;
    }

    private void Update()
    {
        if (isDead) return;

        HandleSanityDrain();
        UpdateUI();
        UpdateVisuals();
        CheckLowSanityWarning();
    }

    private void HandleSanityDrain()
    {
        if (isPlayerHidden) return;

        float totalDrain = passiveDecay;

        // Fener kapalıysa ekstra düşüş
        if (flashlight != null && !flashlight.isLightOn)
        {
            totalDrain += darknessDecay;
        }

        // Tüm düşmanları kontrol et (Çoklu düşman desteği)
        if (enemies != null && enemies.Length > 0)
        {
            bool isChased = false;
            bool isSearched = false;

            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;

                if (enemy.CurrentState == EnemyAI.EnemyState.Chase) isChased = true;
                else if (enemy.CurrentState == EnemyAI.EnemyState.Search) isSearched = true;
            }

            // En yüksek tehlikeye göre düşüş uygula
            if (isChased) totalDrain += chaseDecay;
            else if (isSearched) totalDrain += enemyNearDecay;
        }

        currentSanity -= totalDrain * Time.deltaTime;
        currentSanity = Mathf.Clamp(currentSanity, 0f, maxSanity);

        if (currentSanity <= 0 && !isDead)
        {
            TriggerInsanityDeath();
        }
    }

    private void TriggerInsanityDeath()
    {
        isDead = true;
        Debug.Log("Akıl sağlığı tükendi! Oyun Bitti.");

        // Hardcoded GameOverManager yerine Event tetikliyoruz
        onSanityDepleted?.Invoke();
    }

    private void CheckLowSanityWarning()
    {
        if (currentSanity <= lowSanityThreshold && !hasShownLowSanityWarning)
        {
            // Hardcoded NotificationManager yerine Event tetikliyoruz
            onLowSanityWarning?.Invoke("AKIL SAĞLIĞIN ÇOK DÜŞTÜ! HEMEN SAKLAN!");
            hasShownLowSanityWarning = true;
        }
        else if (currentSanity > lowSanityThreshold + 5f) // Flapping (Sürekli uyarı) önleme
        {
            hasShownLowSanityWarning = false;
        }
    }

    public void IncreaseSanity(float amount)
    {
        if (isDead) return;

        currentSanity += amount;
        if (currentSanity > maxSanity) currentSanity = maxSanity;
    }

    private void UpdateUI()
    {
        if (sanityBarFill != null)
        {
            float ratio = currentSanity / maxSanity;
            sanityBarFill.fillAmount = ratio;
            sanityBarFill.color = Color.Lerp(emptySanityColor, fullSanityColor, ratio);
        }
    }

    private void UpdateVisuals()
    {
        if (insanityVolume != null)
        {
            float effectWeight = 1.0f - (currentSanity / maxSanity);
            insanityVolume.weight = effectWeight;
        }
    }
}
