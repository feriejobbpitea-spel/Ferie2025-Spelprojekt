using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cone_inc : MonoBehaviour
{
    [Header("Charge Settings")]
    public float maxCharge = 100f;

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

    private Movement playerMovement;

    void Start()
    {
        playerMovement = GetComponentInParent<Movement>();

        if (chargeSlider != null)
        {
            chargeSlider.minValue = 0f;
            chargeSlider.maxValue = 1f;
        }

        if (sliderFillImage != null)
            sliderFillImage.color = normalColor;
    }

    void Update()
    {
        if (playerMovement == null) return;

        float currentCharge = playerMovement.currentCharge;
        float maxCharge = playerMovement.maxCharge;

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
                playerMovement.currentCharge = 0f;
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
                chargeText.fontSize = 40;
                chargeText.rectTransform.anchoredPosition = new Vector2(30, -235);
            }
            else
            {
                chargeText.text = $"{(currentCharge / maxCharge) * 100f:0}%";
                chargeText.fontSize = 50;
                chargeText.rectTransform.anchoredPosition = new Vector2(45, -225);
            }
        }
    }
}
