using UnityEngine;

namespace Animations.Rat
{
	public class RatAnimator : MonoBehaviour
	{
		[SerializeField] Animator anim;
	
		private static readonly int Speed = Animator.StringToHash("Speed");

		public void SetMoveSpeed(float speed)
		{
			anim.SetFloat(Speed, speed);
		}
	}
}