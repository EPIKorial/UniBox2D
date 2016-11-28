using UnityEngine;
using System.Collections;

public class _2D_SmoothFollowingCamera : MonoBehaviour {

	public GameObject player;

    /// <summary>
    /// Smooth following Camera
    ///
    /// </summary>
	void Update () {
        if (GameObject.Find("Player(Clone)") && !player)
            player = GameObject.Find("Player(Clone)");
        if (player)
            this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(player.transform.position.x, player.transform.position.y + 0.155f, player.transform.position.z - 8), 0.1f);
        if (Vector3.Distance(this.transform.position, player.transform.position) > 10)
            this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.155f, player.transform.position.z - 8);
	}
}
