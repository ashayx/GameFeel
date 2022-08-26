using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	///添加这个类到一个角色，它将能够通过按下+ dash按钮潜水，在过程中撞击地面。 
	///动画参数:Diving (bool)
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Dive")] 
	public class CharacterDive : CharacterAbility
	{	
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "这个组件允许你的角色俯冲(在空中按下破折号按钮+向下方向)。在这里你可以定义多少相机应该动摇冲击，以及如何快速潜水应该。"; }

		/// Shake parameters : intensity, duration (in seconds) and decay
		[Tooltip("Shake parameters : intensity, duration (in seconds) and decay")]
		public Vector3 ShakeParameters = new Vector3(1.5f,0.5f,1f);
		/// the vertical acceleration applied when diving
		[Tooltip("the vertical acceleration applied when diving")]
		public float DiveAcceleration = 2f;

        // animation parameters
        protected const string _divingAnimationParameterName = "Diving";
        protected int _divingAnimationParameter;

        /// <summary>
        /// Every frame, we check input to see if we should dive
        /// </summary>
        protected override void HandleInput()
		{
			if ((_inputManager.DashButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
				&& (_verticalInput < -_inputManager.Threshold.y))
			{
				// we start the dive coroutine
				InitiateDive();
			}
		}

		protected virtual void InitiateDive()
		{
			if ( !AbilityAuthorized // if the ability is not permitted
				|| (_controller.State.IsGrounded) // or if the character is grounded
				|| (_movement.CurrentState == CharacterStates.MovementStates.Gripping) // or if it's gripping
				|| (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)) // or if we're not under normal conditions
			{
				// we do nothing and exit
				return;
			}
			// we start the dive coroutine
			StartCoroutine(Dive());
		}

		/// <summary>
	    /// Coroutine used to make the player dive vertically
	    /// </summary>
	    protected virtual IEnumerator Dive()
		{
            // we start our sounds
            PlayAbilityStartFeedbacks();

			// we make sure collisions are on
			_controller.CollisionsOn();
			// we set our current state to Diving
			_movement.ChangeState(CharacterStates.MovementStates.Diving);

			// while the player is not grounded, we force it to go down fast
			while (!_controller.State.IsGrounded)
			{
				_controller.SetVerticalForce(-Mathf.Abs(_controller.Parameters.Gravity)*DiveAcceleration);
				yield return null; //go to next frame
			}
			
			// once the player is grounded, we shake the camera, and restore the diving state to false
			if (_sceneCamera != null)
			{
				_sceneCamera.Shake(ShakeParameters);	
			}

            // we play our exit sound
            StopStartFeedbacks();
			PlayAbilityStopFeedbacks();

			_movement.ChangeState(CharacterStates.MovementStates.Idle);
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter (_divingAnimationParameterName, AnimatorControllerParameterType.Bool, out _divingAnimationParameter);
		}

		/// <summary>
		/// Sends the current state of the Diving state to the character's animator
		/// </summary>
		public override void UpdateAnimator()
		{
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _divingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Diving),_character._animatorParameters, _character.PerformAnimatorSanityChecks);
		}
	}
}
