using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public sealed class PlayerInvincibility : MonoBehaviour
{
    public const float DefaultDurationSeconds = 6f;
    public const float VisibleAlpha = 0.26f;
    public const float ExpirationWarningSeconds = 1.5f;
    private const float BlinkFrequency = 8f;

    private sealed class RendererState
    {
        public Renderer renderer;
        public bool enabled;
    }

    private sealed class MaterialState
    {
        public Material material;
        public Shader shader;
        public string colorProperty;
        public bool hadColorProperty;
        public Color color;
        public Texture mainTexture;
        public Vector2 mainTextureScale;
        public Vector2 mainTextureOffset;
        public int renderQueue;
        public string renderType;
        public bool hasSurface;
        public float surface;
        public float mode;
        public int sourceBlend;
        public int destinationBlend;
        public int zWrite;
        public bool alphaTest;
        public bool alphaBlend;
        public bool alphaPremultiply;
    }

    private readonly List<RendererState> rendererStates = new List<RendererState>();
    private readonly List<MaterialState> materialStates = new List<MaterialState>();
    private float expiresAt;
    private GameObject auraObject;
    private Renderer auraRenderer;
    private Material auraMaterial;

    public bool IsInvincible { get; private set; }
    public bool WarningBlinkVisible { get; private set; } = true;
    public int CapturedRendererCount => rendererStates.Count;
    public float RemainingSeconds => IsInvincible
        ? Mathf.Max(0f, expiresAt - Time.unscaledTime) : 0f;

    public void Activate(float durationSeconds = DefaultDurationSeconds)
    {
        float duration = Mathf.Max(0.05f, durationSeconds);
        expiresAt = Mathf.Max(expiresAt, Time.unscaledTime + duration);
        if (!IsInvincible)
        {
            CaptureRenderersAndMaterials();
            IsInvincible = true;
        }

        EnsureAura();
        WarningBlinkVisible = true;
        SetRendererVisibility(true);
        ApplyVisualAlpha(VisibleAlpha);
        UpdateAura(VisibleAlpha, true);
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
        WarningBlinkVisible = true;
        RestoreRenderersAndMaterials();
        if (auraObject != null) auraObject.SetActive(false);
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

        bool warning = remaining <= ExpirationWarningSeconds;
        float alpha = VisibleAlpha;
        bool visible = true;
        if (warning)
        {
            float pulse = Mathf.PingPong(Time.unscaledTime * 4f, 1f);
            alpha = Mathf.Lerp(0.16f, 0.48f, pulse);
            visible = Mathf.FloorToInt(Time.unscaledTime * BlinkFrequency) % 2 == 0;
        }

        WarningBlinkVisible = visible;
        SetRendererVisibility(visible);
        ApplyVisualAlpha(alpha);
        UpdateAura(alpha, visible);
    }

    private void OnDisable()
    {
        Deactivate();
    }

    private void OnDestroy()
    {
        if (auraMaterial != null) DestroyRuntimeObject(auraMaterial);
    }

    private void CaptureRenderersAndMaterials()
    {
        rendererStates.Clear();
        materialStates.Clear();
        foreach (Renderer rendererComponent in GetComponentsInChildren<Renderer>(true))
        {
            if (rendererComponent == auraRenderer) continue;
            rendererStates.Add(new RendererState
            {
                renderer = rendererComponent,
                enabled = rendererComponent.enabled
            });

            foreach (Material material in rendererComponent.materials)
            {
                if (material == null) continue;
                string originalColorProperty = material.HasProperty("_BaseColor")
                    ? "_BaseColor" : material.HasProperty("_Color") ? "_Color" : null;
                bool hasMainTexture = material.HasProperty("_MainTex");

                MaterialState state = new MaterialState
                {
                    material = material,
                    shader = material.shader,
                    colorProperty = originalColorProperty,
                    hadColorProperty = originalColorProperty != null,
                    color = originalColorProperty != null
                        ? material.GetColor(originalColorProperty) : Color.white,
                    mainTexture = hasMainTexture ? material.GetTexture("_MainTex") : null,
                    mainTextureScale = hasMainTexture
                        ? material.GetTextureScale("_MainTex") : Vector2.one,
                    mainTextureOffset = hasMainTexture
                        ? material.GetTextureOffset("_MainTex") : Vector2.zero,
                    renderQueue = material.renderQueue,
                    renderType = material.GetTag("RenderType", false, string.Empty),
                    hasSurface = material.HasProperty("_Surface"),
                    surface = material.HasProperty("_Surface")
                        ? material.GetFloat("_Surface") : 0f,
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

                if (originalColorProperty == null)
                {
                    Shader transparentShader = Shader.Find("Standard");
                    if (transparentShader == null) continue;
                    material.shader = transparentShader;
                    state.colorProperty = material.HasProperty("_BaseColor")
                        ? "_BaseColor" : "_Color";
                    if (hasMainTexture && material.HasProperty("_MainTex"))
                    {
                        material.SetTexture("_MainTex", state.mainTexture);
                        material.SetTextureScale("_MainTex", state.mainTextureScale);
                        material.SetTextureOffset("_MainTex", state.mainTextureOffset);
                    }
                    material.SetColor(state.colorProperty, Color.white);
                }

                materialStates.Add(state);
                ConfigureTransparentMaterial(material);
            }
        }
    }

    private static void ConfigureTransparentMaterial(Material material)
    {
        material.SetOverrideTag("RenderType", "Transparent");
        if (material.HasProperty("_Surface")) material.SetFloat("_Surface", 1f);
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

    private void SetRendererVisibility(bool visible)
    {
        foreach (RendererState state in rendererStates)
        {
            if (state.renderer != null) state.renderer.enabled = state.enabled && visible;
        }
    }

    private void ApplyVisualAlpha(float alpha)
    {
        foreach (MaterialState state in materialStates)
        {
            if (state.material == null) continue;
            Color color = Color.Lerp(state.color,
                new Color(0.35f, 1f, 1f, state.color.a), 0.3f);
            color.a = state.color.a * alpha;
            state.material.SetColor(state.colorProperty, color);
        }
    }

    private void RestoreRenderersAndMaterials()
    {
        foreach (RendererState state in rendererStates)
        {
            if (state.renderer != null) state.renderer.enabled = state.enabled;
        }

        foreach (MaterialState state in materialStates)
        {
            Material material = state.material;
            if (material == null) continue;
            if (material.shader != state.shader)
            {
                if (material.HasProperty("_Mode")) material.SetFloat("_Mode", 0f);
                if (material.HasProperty("_SrcBlend"))
                    material.SetInt("_SrcBlend", (int)BlendMode.One);
                if (material.HasProperty("_DstBlend"))
                    material.SetInt("_DstBlend", (int)BlendMode.Zero);
                if (material.HasProperty("_ZWrite")) material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
            }
            material.shader = state.shader;
            if (state.hadColorProperty && material.HasProperty(state.colorProperty))
                material.SetColor(state.colorProperty, state.color);
            if (material.HasProperty("_MainTex"))
            {
                material.SetTexture("_MainTex", state.mainTexture);
                material.SetTextureScale("_MainTex", state.mainTextureScale);
                material.SetTextureOffset("_MainTex", state.mainTextureOffset);
            }
            material.renderQueue = state.renderQueue;
            material.SetOverrideTag("RenderType", state.renderType);
            if (state.hasSurface && material.HasProperty("_Surface"))
                material.SetFloat("_Surface", state.surface);
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
        rendererStates.Clear();
        materialStates.Clear();
    }

    private void EnsureAura()
    {
        if (auraObject != null)
        {
            auraObject.SetActive(true);
            return;
        }

        auraObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        auraObject.name = "GhostShieldAura";
        auraObject.transform.SetParent(transform, false);
        auraObject.transform.localPosition = new Vector3(0f, 0.85f, 0f);
        auraObject.transform.localScale = new Vector3(1.25f, 1.75f, 1.25f);
        Collider auraCollider = auraObject.GetComponent<Collider>();
        if (auraCollider != null)
        {
            auraCollider.enabled = false;
            DestroyRuntimeObject(auraCollider);
        }

        auraRenderer = auraObject.GetComponent<Renderer>();
        auraRenderer.shadowCastingMode = ShadowCastingMode.Off;
        auraRenderer.receiveShadows = false;
        Shader shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        auraMaterial = new Material(shader);
        auraMaterial.name = "Ghost Shield Aura (Runtime)";
        ConfigureTransparentMaterial(auraMaterial);
        if (auraMaterial.HasProperty("_EmissionColor"))
        {
            auraMaterial.EnableKeyword("_EMISSION");
            auraMaterial.SetColor("_EmissionColor", new Color(0.05f, 0.65f, 0.85f));
        }
        auraRenderer.material = auraMaterial;
    }

    private void UpdateAura(float alpha, bool visible)
    {
        if (auraObject == null || auraRenderer == null || auraMaterial == null) return;
        auraObject.SetActive(true);
        auraObject.transform.Rotate(Vector3.up, 45f * Time.unscaledDeltaTime, Space.Self);
        float pulseScale = 1f + Mathf.Sin(Time.unscaledTime * 5f) * 0.04f;
        auraObject.transform.localScale =
            new Vector3(1.25f, 1.75f, 1.25f) * pulseScale;
        auraRenderer.enabled = visible;
        Color color = new Color(0.1f, 0.88f, 1f,
            Mathf.Clamp(alpha * 0.55f, 0.08f, 0.24f));
        if (auraMaterial.HasProperty("_Color")) auraMaterial.SetColor("_Color", color);
        if (auraMaterial.HasProperty("_BaseColor"))
            auraMaterial.SetColor("_BaseColor", color);
    }

    private static void SetKeyword(Material material, string keyword, bool enabled)
    {
        if (enabled) material.EnableKeyword(keyword);
        else material.DisableKeyword(keyword);
    }

    private static void DestroyRuntimeObject(Object target)
    {
        if (target == null) return;
        if (Application.isPlaying) Destroy(target);
        else DestroyImmediate(target);
    }
}
