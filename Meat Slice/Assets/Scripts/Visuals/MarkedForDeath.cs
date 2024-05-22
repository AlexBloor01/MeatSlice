using UnityEngine;

public class MarkedForDeath : MonoBehaviour
{

    //Destroys an object after making in so tiny it cannot be seen. This is useful for a cartoonish look  as objects fall out of the game.
    float duration = 2;

    private void Awake()
    {
        StartCoroutine(MovementLibrary.LerpOnce(gameObject, duration, transform.localScale, Vector3.zero, false, MovementLibrary.ObjectLerpType.LocalScale, DestroyTarget));
    }

    void DestroyTarget(GameObject target)
    {
        Destroy(target);
    }
}


/*

    Color clear = new Color(1, 1, 1, 0);

    //Fade to nothing then destroy the object.
    IEnumerator FadeToDestruction(GameObject gameObject)
    {
        Material[] materials = null;
        MeshRenderer meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
        if (meshRenderer != null)
        {
            materials = meshRenderer.materials;
        }
        else
        {
            SkinnedMeshRenderer skinnedMeshRenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null)
            {
                materials = skinnedMeshRenderer.materials;
            }
        }

        // Exit the coroutine if no materials are found.
        if (materials == null)
        {
            Debug.LogError("No MeshRenderer or SkinnedMeshRenderer found on the object.");
            yield break;
        }

        foreach (Material mat in materials)
        {
            SetMaterialRenderingMode(mat, RenderingMode.Transparent);
        }

        float percentage = 0;
        float elapsedTime = 0;

        while (percentage < 1)
        {
            elapsedTime += Time.unscaledDeltaTime;
            percentage = elapsedTime / duration;

            Debug.Log("Working");

            foreach (Material mat in materials)
            {
                mat.color = Color.Lerp(mat.color, clear, percentage);
            }

            yield return null;
        }

        Destroy(gameObject);

        yield return null;
    }

    public enum RenderingMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }

    public void SetMaterialRenderingMode(Material material, RenderingMode renderingMode)
    {
        switch (renderingMode)
        {
            case RenderingMode.Opaque:
                material.SetFloat("_Mode", 0);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case RenderingMode.Cutout:
                material.SetFloat("_Mode", 1);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 2450;
                break;
            case RenderingMode.Fade:
                material.SetFloat("_Mode", 2);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
            case RenderingMode.Transparent:
                material.SetFloat("_Mode", 3);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
        }
    }

*/