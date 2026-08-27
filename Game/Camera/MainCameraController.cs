using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Camera))]
public class MainCameraController : MonoBehaviour
{
    private static MainCameraController instance;

    [SerializeField] private Vector3 followOffset = new Vector3(0f, 12f, -8f);
    [SerializeField, Min(0f)] private float lookHeight = 0.5f;
    [SerializeField, Min(0.01f)] private float smoothTime = 0.15f;
    [SerializeField, Range(30f, 90f)] private float followFieldOfView = 60f;

    private Camera followCamera;
    private Player_Base followTarget;
    private Vector3 followPosition;
    private Vector3 followVelocity;
    private Vector3 overviewPosition;
    private Quaternion overviewRotation;
    private float overviewFieldOfView;
    private float nextTargetSearchTime;

    private const float TargetSearchInterval = 0.5f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
        followCamera = GetComponent<Camera>();
        overviewPosition = transform.position;
        overviewRotation = transform.rotation;
        overviewFieldOfView = followCamera.fieldOfView;

        // Keep existing prefab references, but give automatic following sole control.
        CameraControlWithButtons buttons = GetComponent<CameraControlWithButtons>();
        if (buttons != null) buttons.enabled = false;
        JoystickCameraController joystick = GetComponent<JoystickCameraController>();
        if (joystick != null) joystick.enabled = false;
    }

    void OnEnable()
    {
        if (instance != this) return;
        Player_Base.onPlayerAdded.AddListener(OnPlayerAdded);
        Player_Base.onPlayerRemoved.AddListener(OnPlayerRemoved);
        SceneManager.sceneLoaded += OnSceneLoaded;
        FindLocalPlayer();
    }

    void OnDisable()
    {
        Player_Base.onPlayerAdded.RemoveListener(OnPlayerAdded);
        Player_Base.onPlayerRemoved.RemoveListener(OnPlayerRemoved);
        SceneManager.sceneLoaded -= OnSceneLoaded;
        followTarget = null;
        followVelocity = Vector3.zero;
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    private static bool IsLocalPlayer(Player_Base player)
    {
        if (player == null || !player.isActiveAndEnabled) return false;
        if (player is Player) return true;
        if (!(player is Player_Online) || player is Player_Online_Dummy) return false;
        PhotonView view = player.GetComponent<PhotonView>();
        return view != null && view.IsMine;
    }

    private void FindLocalPlayer()
    {
        foreach (Player_Base player in FindObjectsOfType<Player_Base>())
        {
            OnPlayerAdded(player);
            if (followTarget != null) break;
        }
    }

    private void OnPlayerAdded(Player_Base player)
    {
        if (followTarget != null || !IsLocalPlayer(player)) return;
        followTarget = player;
        nextTargetSearchTime = 0f;
        followPosition = player.transform.position + Vector3.up * lookHeight;
        followVelocity = Vector3.zero;
        followCamera.fieldOfView = followFieldOfView;
        ApplyFollowPosition();
    }

    private void OnPlayerRemoved(Player_Base player)
    {
        if (followTarget != player) return;
        followTarget = null;
        nextTargetSearchTime = 0f;
        followVelocity = Vector3.zero;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode != LoadSceneMode.Single) return;
        followTarget = null;
        nextTargetSearchTime = 0f;
        followVelocity = Vector3.zero;
        transform.SetPositionAndRotation(overviewPosition, overviewRotation);
        followCamera.fieldOfView = overviewFieldOfView;
        FindLocalPlayer();
    }

    void LateUpdate()
    {
        UpdateFollow(Time.deltaTime);
    }

    private void UpdateFollow(float deltaTime)
    {
        if (followTarget == null)
        {
            if (Time.unscaledTime < nextTargetSearchTime) return;
            nextTargetSearchTime = Time.unscaledTime + TargetSearchInterval;
            FindLocalPlayer();
            if (followTarget == null) return;
        }

        if (!IsLocalPlayer(followTarget))
        {
            followTarget = null;
            followVelocity = Vector3.zero;
            nextTargetSearchTime = Time.unscaledTime + TargetSearchInterval;
            FindLocalPlayer();
            if (followTarget == null) return;
        }

        Vector3 targetPosition = followTarget.transform.position + Vector3.up * lookHeight;
        followPosition = Vector3.SmoothDamp(followPosition, targetPosition,
            ref followVelocity, smoothTime, Mathf.Infinity, deltaTime);
        ApplyFollowPosition();

        // A teleport or fast height change must not leave the player off-screen.
        Vector3 viewport = followCamera.WorldToViewportPoint(targetPosition);
        if (viewport.z <= followCamera.nearClipPlane || viewport.x < 0.15f ||
            viewport.x > 0.85f || viewport.y < 0.15f || viewport.y > 0.85f)
        {
            followPosition = targetPosition;
            followVelocity = Vector3.zero;
            ApplyFollowPosition();
        }
    }

    private void ApplyFollowPosition()
    {
        // Keep north fixed so movement controls do not rotate with the character.
        transform.SetPositionAndRotation(followPosition + followOffset,
            Quaternion.LookRotation(-followOffset, Vector3.up));
    }
}
