using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class SanityManager : MonoBehaviour
{
    public static SanityManager Instance;

    [Header("Settings")]
    public float maxSanity = 100f;
    [SerializeField] public float currentSanity;
    public bool isPlayerHidden = false;

    private bool isDead = false; // Oyunun sürekli Game Over çaðýrmamasý için kontrol

    [Header("Warning Settings (Uyarý Ayarlarý)")]
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

    [Header("References")]
    public Image sanityBarFill;
    public Volume insanityVolume;
    public EnemyAI enemyReference;
    public FlashlightController flashlight;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        currentSanity = maxSanity;
        isDead = false; // Baþlangýçta yaþýyoruz
    }

    private void Update()
    {
        // Eðer öldüysek akýl saðlýðý düþmeye veya kontrol edilmeye devam etmesin
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

        if (flashlight != null && !flashlight.isLightOn)
            totalDrain += darknessDecay;

        if (enemyReference != null)
        {
            if (enemyReference.CurrentState == EnemyAI.EnemyState.Chase)
                totalDrain += chaseDecay;
            else if (enemyReference.CurrentState == EnemyAI.EnemyState.Search)
                totalDrain += enemyNearDecay;
        }

        currentSanity -= totalDrain * Time.deltaTime;
        currentSanity = Mathf.Clamp(currentSanity, 0f, maxSanity);

        // --- YENÝ EKLENEN KISIM: ÖLÜM KONTROLÜ ---
        if (currentSanity <= 0 && !isDead)
        {
            TriggerInsanityDeath();
        }
    }

    private void TriggerInsanityDeath()
    {
        isDead = true;
        Debug.Log("Akýl saðlýðý tükendi! Oyun Bitti.");

        if (NotificationManager.Instance != null)
        {
            // Ýstersen bildirimi kapatabilirsin, zaten koca ekran çýkacak
            // NotificationManager.Instance.ShowNotification(NotificationType.Warning, "AKLINI YÝTÝRDÝN...");
        }

        // --- GÜNCELLEME BURADA ---
        // Artýk parantez içine istediðimiz mesajý yazýyoruz.
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.ShowGameOverScreen("AKIL SAÐLIÐINI KAYBETTÝN...");
        }
    }

    private void CheckLowSanityWarning()
    {
        if (currentSanity <= lowSanityThreshold && !hasShownLowSanityWarning)
        {
            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.ShowNotification(NotificationType.Warning, "AKIL SAÐLIÐIN ÇOK DÜÞTÜ! HEMEN SAKLAN!");
                hasShownLowSanityWarning = true;
            }
        }
        else if (currentSanity > lowSanityThreshold + 5f)
        {
            hasShownLowSanityWarning = false;
        }
    }

    public void IncreaseSanity(float amount)
    {
        if (isDead) return; // Ölüye can basamazsýn :)

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