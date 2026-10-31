
using UnityEngine;

public class camera : MonoBehaviour
{
    public Transform player;

    // 鏡頭與車子的相對位置
    public Vector3 offset = new Vector3(0, -10, -10);

    void LateUpdate()
    {
        if (player == null) return;

        // 跟隨玩家，保持固定距離
        transform.position = player.position + offset;
    }
}
