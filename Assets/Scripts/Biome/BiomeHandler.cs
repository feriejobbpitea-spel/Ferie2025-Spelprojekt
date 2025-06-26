using UnityEngine;
using DG.Tweening;

public class BiomeHandler : Singleton<BiomeHandler>
{
    public Biome_SO StartBiome;
    public Biome_SO CurrentBiome { get; private set; }

    public float colorTweenDuration = 1f;

    private Tween backgroundTween;

    private void Start()
    {
        UpdateBiome(StartBiome);
    }

    public void UpdateBiome(Biome_SO newBiome)
    {
        if (newBiome == CurrentBiome)
            return;

        CurrentBiome = newBiome;

        // Kill any existing tween before starting a new one
        if (backgroundTween != null && backgroundTween.IsActive())
        {
            backgroundTween.Kill();
        }

        Color currentColor = Camera.main.backgroundColor;

        backgroundTween = DOTween.To(() => currentColor, x => {
            Camera.main.backgroundColor = x;
            currentColor = x;
        }, newBiome.backgroundColor, colorTweenDuration)
        .SetEase(Ease.InOutQuad)
        .OnComplete(() =>
        {
            // Explicitly set to final color to avoid rounding errors or drift
            Camera.main.backgroundColor = newBiome.backgroundColor;
        });
        PlayerHealthV2.Instance.UpdateHearts();
    }
}
