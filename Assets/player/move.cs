
using UnityEngine;
using UnityEngine.InputSystem;

public class move : MonoBehaviour
{
    [Header("移動速度")]
    public float forwardSpeed = 10f;
    public float sideSpeed = 5f;
    public float heightSpeed = 5f;

    [Header("移動範圍")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = 1f;
    public float maxY = 20f;

    [Header("障礙物")]
    public Transform[] obstacles;

    [Header("音效")]
    public AudioClip drivingSound;
    public AudioClip passSound;
    public AudioClip crashSound;

    private Rigidbody rb;
    private AudioSource audioSource;

    private bool gameOver = false;
    private float previousZ;

    private bool[] passedObstacles;

    void Start()
    {
        Application.targetFrameRate = 60;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        audioSource = GetComponent<AudioSource>();

        previousZ = rb.position.z;

        passedObstacles = new bool[obstacles.Length];

        // 開始播放引擎音效
        if (audioSource != null && drivingSound != null)
        {
            audioSource.clip = drivingSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void FixedUpdate()
    {
        if (gameOver) return;

        // 到達終點
        if (rb.position.z >= 45f)
        {
            gameOver = true;

            if (audioSource != null)
                audioSource.Stop();

            Debug.Log("遊戲成功！");
            return;
        }

        // 讀取鍵盤
        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed)
                horizontal = -1f;

            if (Keyboard.current.dKey.isPressed)
                horizontal = 1f;

            if (Keyboard.current.wKey.isPressed)
                vertical = 1f;

            if (Keyboard.current.sKey.isPressed)
                vertical = -1f;
        }

        // 計算移動
        Vector3 movement = new Vector3(
            horizontal * sideSpeed,
            vertical * heightSpeed,
            forwardSpeed
        );

        Vector3 nextPosition =
            rb.position + movement * Time.fixedDeltaTime;

        // 限制移動範圍
        nextPosition.x = Mathf.Clamp(nextPosition.x, minX, maxX);
        nextPosition.y = Mathf.Clamp(nextPosition.y, minY, maxY);

        // 檢查是否通過障礙物
        CheckPassedObstacles(nextPosition.z);

        // 移動 Player
        rb.MovePosition(nextPosition);

        previousZ = nextPosition.z;
    }

    void CheckPassedObstacles(float currentZ)
    {
        for (int i = 0; i < obstacles.Length; i++)
        {
            if (obstacles[i] == null || passedObstacles[i])
                continue;

            float obstacleZ = obstacles[i].position.z;

            // Player 從障礙物前方移動到障礙物後方
            if (previousZ < obstacleZ && currentZ >= obstacleZ)
            {
                passedObstacles[i] = true;

                if (audioSource != null && passSound != null)
                    audioSource.PlayOneShot(passSound);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            gameOver = true;

            if (audioSource != null)
            {
                audioSource.Stop();

                if (crashSound != null)
                    audioSource.PlayOneShot(crashSound);
            }

            Debug.Log("遊戲失敗！");
        }
    }
}
