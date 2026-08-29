using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public sealed class PlayerInvincibility : MonoBehaviour
{
    public const float DefaultDurationSeconds = 6f;
    public const float VisibleAlpha = 0.36f;
    private const float ExpirationWarningSeconds = 1.5f;

    private sealed class MaterialState
    {
        public Material material;
        public Color color;
        public int renderQueue;
        public float mode;
        public int sourceBlend;
        public int destinationBlend;
        public int zWrite;
        public bool alphaTest;
        public bool alphaBlend;
        public bool alphaPremultiply;
    }

    private readonly List<MaterialState> materialStates = new List<MaterialState>();
    private float expiresAt;

    public bool IsInvincible { get; private set; }
    public float RemainingSeconds => IsInvincible
        ? Mathf.Max(0f, expiresAt - Time.unscaledTime) : 0f;

    public void Activate(float durationSeconds = DefaultDurationSeconds)
    {
        float duration = Mathf.Max(0.05f, durationSeconds);
        expiresAt = Mathf.Max(expiresAt, Time.unscaledTime + duration);
        if (!IsInvincible)
        {
            CaptureMaterials();
            IsInvincible = true;
        }
        ApplyVisualAlpha(VisibleAlpha);
    }

    [PunRPC]
    public void ActivateSynced(float durationSeconds)
    {
        Activate(durationSeconds);
    }

    public void Deactivate()
    {
        if (!IsInvincible && materialStates.Count == 0) return;
        IsInvincible = false;
        expiresAt = 0f;
        RestoreMaterials();
    }

    private void Update()
    {
        if (!IsInvincible) return;
        float remaining = RemainingSeconds;
        if (remaining <= 0f)
        {
            Deactivate();
            return;
        }

        float alpha = VisibleAlpha;
        if (remaining <= ExpirationWarningSeconds)
        {
            float pulse = Mathf.PingPong(Time.unscaledTime * 5f, 1f);
            alpha = Mathf.Lerp(0.22f, 0.62f, pulse);
        }
        ApplyVisualAlpha(alpha);
    }

    private void OnDisable()
    {
        Deactivate();
    }

    private void CaptureMaterials()
    {
        materialStates.Clear();
        foreach (Renderer rendererComponent in GetComponentsInChildren<Renderer>(true))
        {
            foreach (Material material in rendererComponent.materials)
            {
                if (material == null || !material.HasProperty("_Color")) continue;
                MaterialState state = new MaterialState
                {
                    material = material,
                    color = material.color,
                    renderQueue = material.renderQueue,
                    mode = material.HasProperty("_Mode") ? material.GetFloat("_Mode") : 0f,
                    sourceBlend = material.HasProperty("_SrcBlend")
                        ? material.GetInt("_SrcBlend") : 0,
                    destinationBlend = material.HasProperty("_DstBlend")
                        ? material.GetInt("_DstBlend") : 0,
                    zWrite = material.HasProperty("_ZWrite") ? material.GetInt("_ZWrite") : 1,
                    alphaTest = material.IsKeywordEnabled("_ALPHATEST_ON"),
                    alphaBlend = material.IsKeywordEnabled("_ALPHABLEND_ON"),
                    alphaPremultiply = material.IsKeywordEnabled("_ALPHAPREMULTIPLY_ON")
                };
                materialStates.Add(state);
                ConfigureTransparentMaterial(material);
            }
        }
    }

    private static void ConfigureTransparentMaterial(Material material)
    {
        if (material.HasProperty("_Mode")) material.SetFloat("_Mode", 3f);
        if (material.HasProperty("_SrcBlend"))
            material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        if (material.HasProperty("_DstBlend"))
            material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        if (material.HasProperty("_ZWrite")) material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)RenderQueue.Transparent;
    }

    private void ApplyVisualAlpha(float alpha)
    {
        foreach (MaterialState state in materialStates)
        {
            if (state.material == null) continue;
            Color color = state.color;
            color.a *= alpha;
            state.material.color = color;
        }
    }

    private void RestoreMaterials()
    {
        foreach (MaterialState state in materialStates)
        {
            Material material = state.material;
            if (material == null) continue;
            material.color = state.color;
            material.renderQueue = state.renderQueue;
            if (material.HasProperty("_Mode")) material.SetFloat("_Mode", state.mode);
            if (material.HasProperty("_SrcBlend"))
                material.SetInt("_SrcBlend", state.sourceBlend);
            if (material.HasProperty("_DstBlend"))
                material.SetInt("_DstBlend", state.destinationBlend);
            if (material.HasProperty("_ZWrite")) material.SetInt("_ZWrite", state.zWrite);
            SetKeyword(material, "_ALPHATEST_ON", state.alphaTest);
            SetKeyword(material, "_ALPHABLEND_ON", state.alphaBlend);
            SetKeyword(material, "_ALPHAPREMULTIPLY_ON", state.alphaPremultiply);
        }
        materialStates.Clear();
    }

    private static void SetKeyword(Material material, string keyword, bool enabled)
    {
        if (enabled) material.EnableKeyword(keyword);
        else material.DisableKeyword(keyword);
    }
}
