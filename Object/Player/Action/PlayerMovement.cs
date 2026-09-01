using UnityEngine;
using Photon.Pun;

[System.Serializable]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public const float MaximumMoveSpeed = 7f;
    private const float PlayableMinimum = 2f;
    private const float BoundaryTolerance = 0.0001f;

    [SerializeField] private float moveSpeed = 3.0f;
    private PlayerAnimation playerAnimation;
    private Rigidbody playerRigidbody;
    private PhotonView photonView;
    private Vector3 requestedDirection;
    private bool requiresLocalOwnership;
    private bool wasLocallyControlled;

    public void Awake()
    {
        playerAnimation = gameObject.AddComponent<PlayerAnimation>();
        playerRigidbody = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
        requiresLocalOwnership = GetComponent<PlayerAction_Online>() != null;
        wasLocallyControlled = CanControlMovement();

        // リモート側では PhotonTransformView が座標を受信する。動的 Rigidbody の
        // 物理解決と競合させると、遅延時に位置が戻ったり振動したりするため、
        // 所有者以外は kinematic として受信座標だけを反映する。
        bool isRemoteOnlinePlayer = requiresLocalOwnership && photonView != null &&
                                    !photonView.IsMine;
        if (isRemoteOnlinePlayer)
        {
            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.isKinematic = true;
            playerRigidbody.collisionDetectionMode =
                CollisionDetectionMode.ContinuousSpeculative;
        }
        else
        {
            // ローカル所有者は物理更新と連続動的衝突判定で外壁抜けを防ぐ。
            playerRigidbody.isKinematic = false;
            playerRigidbody.collisionDetectionMode =
                CollisionDetectionMode.ContinuousDynamic;
        }
        playerRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public void Move(Vector3 direction)
    {
        Vector3 horizontalDirection = new Vector3(direction.x, 0f, direction.z);
        requestedDirection = Vector3.ClampMagnitude(horizontalDirection, 1f);
        if (requestedDirection.sqrMagnitude <= Mathf.Epsilon)
        {
            MoveClear();
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(requestedDirection);
        transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        playerAnimation.SetWalking(true);
    }

    public void MoveClear()
    {
        requestedDirection = Vector3.zero;
        playerAnimation.SetWalking(false);
    }

    private void FixedUpdate()
    {
        bool canControl = CanControlMovement();
        if (!canControl)
        {
            if (wasLocallyControlled)
            {
                playerRigidbody.velocity = Vector3.zero;
                requestedDirection = Vector3.zero;
            }
            wasLocallyControlled = false;
            return;
        }

        wasLocallyControlled = true;
        Vector3 position = ClampToPlayableArea(playerRigidbody.position);
        if ((position - playerRigidbody.position).sqrMagnitude > BoundaryTolerance * BoundaryTolerance)
        {
            playerRigidbody.position = position;
        }

        Vector3 desiredVelocity = requestedDirection * moveSpeed;
        float fixedDeltaTime = Mathf.Max(Time.fixedDeltaTime, Mathf.Epsilon);
        Vector3 predictedPosition = ClampToPlayableArea(position + desiredVelocity * fixedDeltaTime);

        // 物理ステップ1回で外壁の外側へ到達しない速度に制限する。
        desiredVelocity.x = (predictedPosition.x - position.x) / fixedDeltaTime;
        desiredVelocity.z = (predictedPosition.z - position.z) / fixedDeltaTime;
        desiredVelocity.y = playerRigidbody.velocity.y;
        playerRigidbody.velocity = desiredVelocity;
    }

    private void LateUpdate()
    {
        if (!CanControlMovement()) return;

        // 衝突解決や一時的な負荷で境界外へ押し出された場合の最終安全策。
        Vector3 currentPosition = playerRigidbody.position;
        Vector3 clampedPosition = ClampToPlayableArea(currentPosition);
        if ((clampedPosition - currentPosition).sqrMagnitude <= BoundaryTolerance * BoundaryTolerance) return;

        Vector3 velocity = playerRigidbody.velocity;
        if (!Mathf.Approximately(clampedPosition.x, currentPosition.x)) velocity.x = 0f;
        if (!Mathf.Approximately(clampedPosition.z, currentPosition.z)) velocity.z = 0f;
        playerRigidbody.position = clampedPosition;
        playerRigidbody.velocity = velocity;
    }

    private bool CanControlMovement()
    {
        return !requiresLocalOwnership || photonView == null || photonView.IsMine;
    }

    public static Vector3 ClampToPlayableArea(Vector3 position)
    {
        float maximumX = Mathf.Max(PlayableMinimum, GameManager.xmax - 3f);
        float maximumZ = Mathf.Max(PlayableMinimum, GameManager.zmax - 3f);
        position.x = Mathf.Clamp(position.x, PlayableMinimum, maximumX);
        position.z = Mathf.Clamp(position.z, PlayableMinimum, maximumZ);
        return position;
    }

    public void SpeedUp()
    {
        if (moveSpeed < MaximumMoveSpeed)
        {
            moveSpeed += 1f;
        }
    }

    public Vector3 GetCurrentPos()
    {
        return transform.position;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }
}
