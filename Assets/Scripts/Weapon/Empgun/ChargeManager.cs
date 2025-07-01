using UnityEngine;

public class ChargeManager : MonoBehaviour
{
    public static float currentCharge = 0f;
    public static float maxCharge = 100f;
    public static float chargeRate = 10f;

    private void Update()
    {
        if (currentCharge < maxCharge)
        {
            currentCharge += Time.deltaTime * chargeRate;
            currentCharge = Mathf.Min(currentCharge, maxCharge);
        }
    }
}
