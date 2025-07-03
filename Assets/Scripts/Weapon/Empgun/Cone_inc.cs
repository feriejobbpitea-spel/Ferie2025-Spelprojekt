using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cone_inc : MonoBehaviour
{
    [Header("Charge UI")]
    public Slider chargeSlider;
    public TextMeshProUGUI chargeText;
    public Image sliderFillImage;

    public Color normalColor = Color.white;
    public Color blinkColor = Color.blue;
    public float blinkSpeed = 2f;

    [Header("Shooting")]
    public GameObject laserPrefab;
    public Transform firePoint;
    public AudioClip shootSound; // Lägg till detta
    private AudioSource audioSource; // Och detta

    private float _defaultFontSize;

    void Start()
    {
        _defaultFontSize = chargeText.fontSize;

        if (chargeSlider != null)
        {
            chargeSlider.minValue = 0f;
            chargeSlider.maxValue = 1f;
        }

        if (sliderFillImage != null)
            sliderFillImage.color = normalColor;

        // Hämta AudioSource-komponenten
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("Ingen AudioSource-komponent hittades på detta GameObject.");
        }
    }

    void Update()
    {
        float currentCharge = ChargeManager.currentCharge;
        float maxCharge = ChargeManager.maxCharge;

        UpdateChargeUI(currentCharge, maxCharge);

        if (currentCharge >= maxCharge)
        {
            if (sliderFillImage != null)
            {
                float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);
                sliderFillImage.color = Color.Lerp(normalColor, blinkColor, t);
            }

            if (Input.GetMouseButtonDown(0))
            {
                ShootLaser();
                ChargeManager.currentCharge = 0f;  // Reset laddningen via ChargeManager
            }
        }
        else
        {
            if (sliderFillImage != null)
                sliderFillImage.color = normalColor;
        }
    }

    void ShootLaser()
    {
        if (laserPrefab != null && firePoint != null)
        {
            Instantiate(laserPrefab, firePoint.position, firePoint.rotation);

            // Spela ljud
            if (shootSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(shootSound);
            }
        }
        else
        {
            Debug.LogWarning("Laser Prefab or FirePoint is not assigned.");
        }
    }

    void UpdateChargeUI(float currentCharge, float maxCharge)
    {
        if (chargeSlider != null)
            chargeSlider.value = currentCharge / maxCharge;

        if (chargeText != null)
        {
            if (currentCharge >= maxCharge)
            {
                chargeText.text = "Ready";
                chargeText.fontSize = _defaultFontSize + 5;
            }
            else
            {
                chargeText.text = $"{(currentCharge / maxCharge) * 100f:0}%";
                chargeText.fontSize = _defaultFontSize;
            }
        }
    }
}

