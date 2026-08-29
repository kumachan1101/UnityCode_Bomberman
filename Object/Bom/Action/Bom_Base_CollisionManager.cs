using UnityEngine;

public class Bom_Base_CollisionManager : MonoBehaviour
{
    private const float CellProbeRadius = 0.35f;

    public bool CheckForCollision()
    {
        Vector3 direction = BomGridRules.GetCardinalDirection(transform.forward);
        Vector3 nextCell = BomGridRules.GetCellInDirection(transform.position, direction, 1);
        return CheckForCollisionAtCell(nextCell);
    }

    public bool CheckForCollisionAtCell(Vector3 cell)
    {
        if (BomGridRules.IsBombAtCell(cell, gameObject))
        {
            return true;
        }

        Collider[] colliders = Physics.OverlapSphere(
            BomGridRules.ToCell(cell), CellProbeRadius, ~0, QueryTriggerInteraction.Collide);
        foreach (Collider candidateCollider in colliders)
        {
            if (candidateCollider == null ||
                candidateCollider.transform == transform ||
                candidateCollider.transform.IsChildOf(transform))
            {
                continue;
            }

            if (IsBlockingObject(candidateCollider.gameObject))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsBlockingObject(GameObject candidate)
    {
        if (candidate == null)
        {
            return false;
        }

        Transform current = candidate.transform;
        while (current != null)
        {
            if (current.GetComponent<Bom_Base>() != null)
            {
                return true;
            }

            string objectName = current.name;
            if (objectName.StartsWith("Broken") ||
                objectName.StartsWith("FixedWall") ||
                objectName.StartsWith("Wall") ||
                objectName.StartsWith("Bom") ||
                objectName.StartsWith("Bombigban"))
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }

    private void OnTriggerExit(Collider other)
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider != null)
        {
            sphereCollider.isTrigger = false;
        }
    }
}
