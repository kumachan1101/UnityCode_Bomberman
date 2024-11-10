using UnityEngine;

public class JoystickCameraController : MonoBehaviour
{
    public Joystick joystick; // ジョイスティックの参照
    public float moveSpeed = 5f; // カメラの移動速度

    void Update()
    {
		if(joystick == null){
			return;
		}
        // ジョイスティックの入力を取得
        float horizontalInput = joystick.Horizontal;
        float verticalInput = joystick.Vertical;

        // カメラの現在位置を取得
        Vector3 currentPosition = transform.position;

        // ジョイスティックの入力に基づいて新しい位置を計算
        Vector3 newPosition = currentPosition + new Vector3(horizontalInput, 0, verticalInput) * moveSpeed * Time.deltaTime;

        // 新しい位置にカメラを移動
        transform.position = newPosition;
    }
}
