using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	[AddComponentMenu("Corgi Engine/Environment/Character Dash Override")]
	/// <summary>
	///添加这个组件到一个触发区域，它将覆盖所有穿过它的字符的CharacterDash设置
    /// </summary>
	public class CharacterDashOverride : MonoBehaviour 
	{

		/// <summary>
		/// Triggered when something collides with the override zone
		/// </summary>
		/// <param name="collider">Something colliding with the override zone.</param>
		protected virtual void OnTriggerEnter2D(Collider2D collider)
		{
			CharacterDash characterDash = collider.GetComponent<Character>()?.FindAbility<CharacterDash>();
			if (characterDash == null)
			{
				return;
			}
			characterDash.AbilityPermitted = false;
		}

		/// <summary>
		/// Triggered when something exits the water
		/// </summary>
		/// <param name="collider">Something colliding with the water.</param>
		protected virtual void OnTriggerExit2D(Collider2D collider)
		{
			CharacterDash characterDash = collider.GetComponent<Character>()?.FindAbility<CharacterDash>();
			if (characterDash == null)
			{
				return;
			}
			characterDash.AbilityPermitted = true;
		}
	}
}
