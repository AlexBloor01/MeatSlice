using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiSizeGameRender : MonoBehaviour
{
    [Header("Fix the Game Render Texture for Different Size Phones")]
    public RenderTexture GameRenderTexture;
    public float RenderTextureMultiplier = 1.3f;

    private void Awake()
    {
        SetupVariables();
    }

    private void SetupVariables()
    {
        if (GameRenderTexture == null)
        {
            Debug.LogWarning($"Game Render Texture not available...");
            return;
        }

        FixGameRenderTextureForPhone();
    }

    private void FixGameRenderTextureForPhone()
    {
        GameRenderTexture.width = Mathf.RoundToInt(Screen.width * RenderTextureMultiplier);
        GameRenderTexture.height = Mathf.RoundToInt(Screen.height * RenderTextureMultiplier);
    }

}
