// using UnityEditor.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode, ImageEffectAllowedInSceneView] //[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class PostEffectController : MonoBehaviour
{
    public Material mat;


    private void Awake()
    {
        SetupVairables();
    }
    void SetupVairables()
    {
        EnableDepthTextures();
    }

    //Required for material.
    void EnableDepthTextures()
    {
        Camera.main.depthTextureMode = DepthTextureMode.DepthNormals;
    }


    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        RenderTexture renderTexture = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);
        Graphics.Blit(src, renderTexture, mat, 0);
        Graphics.Blit(renderTexture, dest);
        RenderTexture.ReleaseTemporary(renderTexture);
    }

}