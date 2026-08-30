using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;

public sealed class ItemMagnet : Item
{
    private readonly List<Material> runtimeMaterials = new List<Material>();
    private bool visualConfigured;

    private void OnEnable()
    {
        ConfigurePickupVisual();
    }

    public override void Reflection(GameObject playerObject)
    {
        if (playerObject == null) return;
        PlayerItemMagnet effect = playerObject.GetComponent<PlayerItemMagnet>();
        if (effect == null) effect = playerObject.AddComponent<PlayerItemMagnet>();

        PhotonView view = playerObject.GetComponent<PhotonView>();
        if (view != null && PhotonNetwork.InRoom)
        {
            if (!view.IsMine) return;
            view.RPC(nameof(PlayerItemMagnet.ActivateSynced), RpcTarget.All,
                PlayerItemMagnet.DefaultDurationSeconds);
            return;
        }

        effect.Activate(PlayerItemMagnet.DefaultDurationSeconds);
    }

    public void ConfigurePickupVisual()
    {
        if (visualConfigured) return;
        visualConfigured = true;
        gameObject.name = "item_magnet(Clone)";

        Renderer baseRenderer = GetComponent<Renderer>();
        if (baseRenderer != null)
        {
            Material baseMaterial = baseRenderer.material;
            runtimeMaterials.Add(baseMaterial);
            if (baseMaterial.HasProperty("_MainTex"))
                baseMaterial.SetTexture("_MainTex", null);
            if (baseMaterial.HasProperty("_Color"))
                baseMaterial.SetColor("_Color", new Color(0.045f, 0.055f, 0.075f));
        }

        CreateMagnetPart("MagnetBase", new Vector3(0f, 0.58f, -0.22f),
            new Vector3(0.64f, 0.12f, 0.16f), new Color(0.72f, 0.76f, 0.82f));
        CreateMagnetPart("MagnetRedArm", new Vector3(-0.24f, 0.58f, 0.04f),
            new Vector3(0.16f, 0.12f, 0.58f), new Color(1f, 0.12f, 0.08f));
        CreateMagnetPart("MagnetBlueArm", new Vector3(0.24f, 0.58f, 0.04f),
            new Vector3(0.16f, 0.12f, 0.58f), new Color(0.06f, 0.72f, 1f));
        CreateMagnetPart("MagnetRedTip", new Vector3(-0.24f, 0.59f, 0.36f),
            new Vector3(0.18f, 0.14f, 0.12f), Color.white);
        CreateMagnetPart("MagnetBlueTip", new Vector3(0.24f, 0.59f, 0.36f),
            new Vector3(0.18f, 0.14f, 0.12f), Color.white);
    }

    private void CreateMagnetPart(string partName, Vector3 localPosition,
        Vector3 localScale, Color color)
    {
        GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
        part.name = partName;
        part.transform.SetParent(transform, false);
        part.transform.localPosition = localPosition;
        part.transform.localRotation = Quaternion.identity;
        part.transform.localScale = localScale;

        Collider partCollider = part.GetComponent<Collider>();
        if (partCollider != null)
        {
            partCollider.enabled = false;
            DestroyRuntimeObject(partCollider);
        }

        Renderer rendererComponent = part.GetComponent<Renderer>();
        rendererComponent.shadowCastingMode = ShadowCastingMode.Off;
        rendererComponent.receiveShadows = false;
        Shader shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        Material material = new Material(shader);
        material.name = partName + " Material (Runtime)";
        material.color = color;
        if (material.HasProperty("_EmissionColor"))
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * 0.35f);
        }
        rendererComponent.sharedMaterial = material;
        runtimeMaterials.Add(material);
    }

    private void OnDestroy()
    {
        foreach (Material material in runtimeMaterials)
        {
            if (material != null) DestroyRuntimeObject(material);
        }
        runtimeMaterials.Clear();
    }

    private static void DestroyRuntimeObject(Object target)
    {
        if (target == null) return;
        if (Application.isPlaying) Destroy(target);
        else DestroyImmediate(target);
    }
}
