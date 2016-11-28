using UnityEngine;
using System.Collections;

namespace Unity_ToolboxCS
{
	public class _2D : MonoBehaviour 
    {

		static public void		LookAt2D(GameObject source, GameObject target)
			{
				Vector3 norTar = (target.transform.position - source.transform.position).normalized;
				float angle = Mathf.Atan2(norTar.y, norTar.x) * Mathf.Rad2Deg;
				Quaternion rotation = new Quaternion ();
				rotation.eulerAngles = new Vector3(0, 0, angle);
				source.transform.rotation = rotation;
			}

		static public void			Follow2D(GameObject ObjectFollowing, GameObject ObjectToFollow, float speed)
		{
				ObjectFollowing.transform.position = Vector3.MoveTowards (ObjectFollowing.transform.position, ObjectToFollow.transform.position, speed);
		}

		static public void			Follow2D_Distance(GameObject ObjectFollowing, GameObject ObjectToFollow, float speed, float distanceToKeep)
		{
			if (Vector3.Distance (ObjectFollowing.transform.position, ObjectToFollow.transform.position) > distanceToKeep) {
				ObjectFollowing.transform.position = Vector3.MoveTowards (ObjectFollowing.transform.position, ObjectToFollow.transform.position, speed);
			}
		}
	}
}
