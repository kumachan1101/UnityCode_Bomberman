using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class PlayerItemMagnet : MonoBehaviour
{
    public const float DefaultDurationSeconds = 8f;
    public const float AttractionRadius = 3.6f;
    public const float PullSpeed = 6f;
    private const int NearbyColliderCapacity = 96;
    private const int BlockingHitCapacity = 24;

    private static readonly List<PlayerItemMagnet> activeMagnets =
        new List<PlayerItemMagnet>();
    private static readonly Collider[] nearbyColliders =
        new Collider[NearbyColliderCapacity];
    private static readonly RaycastHit[] blockingHits =
        new RaycastHit[BlockingHitCapacity];

    private readonly HashSet<Item> processedItems = new HashSet<Item>();
    private float expiresAt;

    public bool IsActive { get; private set; }
    public int AttractedItemCountLastFrame { get; private set; }
    public float RemainingSeconds => IsActive
        ? Mathf.Max(0f, expiresAt - Time.unscaledTime) : 0f;

    public void Activate(float durationSeconds = DefaultDurationSeconds)
    {
        float duration = Mathf.Max(0.05f, durationSeconds);
        expiresAt = Mathf.Max(expiresAt, Time.unscaledTime + duration);
        if (IsActive) return;

        IsActive = true;
        if (!activeMagnets.Contains(this)) activeMagnets.Add(this);
    }

    [PunRPC]
    public void ActivateSynced(float durationSeconds)
    {
        Activate(durationSeconds);
    }

    public void Deactivate()
    {
        IsActive = false;
        expiresAt = 0f;
        AttractedItemCountLastFrame = 0;
        activeMagnets.Remove(this);
    }

    private void Update()
    {
        if (!IsActive) return;
        if (RemainingSeconds <= 0f)
        {
            Deactivate();
            return;
        }

        PullNearbyItems();
    }

    private void OnDisable()
    {
        Deactivate();
    }

    private void OnDestroy()
    {
        activeMagnets.Remove(this);
    }

    private void PullNearbyItems()
    {
        AttractedItemCountLastFrame = 0;
        processedItems.Clear();
        int count = Physics.OverlapSphereNonAlloc(transform.position,
            AttractionRadius, nearbyColliders, Physics.AllLayers,
            QueryTriggerInteraction.Collide);

        for (int index = 0; index < count; index++)
        {
            Collider nearby = nearbyColliders[index];
            if (nearby == null) continue;
            Item item = nearby.GetComponentInParent<Item>();
            if (item == null || !item.isActiveAndEnabled || !processedItems.Add(item))
                continue;
            if (!IsNearestActiveMagnet(item.transform.position)) continue;
            if (HasSolidObstacle(item)) continue;

            Vector3 target = transform.position;
            target.y = item.transform.position.y;
            item.transform.position = Vector3.MoveTowards(item.transform.position,
                target, PullSpeed * Time.deltaTime);
            AttractedItemCountLastFrame++;
        }
    }

    private bool IsNearestActiveMagnet(Vector3 itemPosition)
    {
        float ownDistance = HorizontalSquaredDistance(transform.position, itemPosition);
        int ownId = GetInstanceID();
        foreach (PlayerItemMagnet magnet in activeMagnets)
        {
            if (magnet == null || !magnet.IsActive || magnet == this) continue;
            float otherDistance =
                HorizontalSquaredDistance(magnet.transform.position, itemPosition);
            if (otherDistance < ownDistance - 0.0001f) return false;
            if (Mathf.Abs(otherDistance - ownDistance) <= 0.0001f &&
                magnet.GetInstanceID() < ownId)
                return false;
        }
        return true;
    }

    private bool HasSolidObstacle(Item item)
    {
        Vector3 start = transform.position + Vector3.up * 0.35f;
        Vector3 end = item.transform.position + Vector3.up * 0.1f;
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        if (distance <= 0.01f) return false;

        int hitCount = Physics.RaycastNonAlloc(start, direction / distance,
            blockingHits, distance, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        for (int index = 0; index < hitCount; index++)
        {
            Collider hitCollider = blockingHits[index].collider;
            if (hitCollider == null) continue;
            Transform hitTransform = hitCollider.transform;
            if (hitTransform == transform || hitTransform.IsChildOf(transform)) continue;
            Item hitItem = hitCollider.GetComponentInParent<Item>();
            if (hitItem == item) continue;
            return true;
        }
        return false;
    }

    private static float HorizontalSquaredDistance(Vector3 first, Vector3 second)
    {
        float x = first.x - second.x;
        float z = first.z - second.z;
        return x * x + z * z;
    }
}
