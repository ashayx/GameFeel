using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this component to a Character and it'll be able to cling to walls when being in the air, 
	// facing a wall, and moving in its direction
	/// Animator parameters : WallClinging (bool)
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Wallclinging")] 
	public class CharacterWallClinging : CharacterAbility 
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "添加这个组件到你的角色，它将能够粘在墙上，减缓其下落。在这里，您可以定义慢系数(接近0:超慢，1:正常下降)和公差(用于解释墙上的小洞。"; }

		[Header("Wall Clinging")]

        /// the slow factor when wall clinging
		[Tooltip("壁挂时的缓慢因素")]
        [Range(0.01f, 1)]
        public float WallClingingSlowFactor = 0.6f;
        /// the vertical offset to apply to raycasts for wall clinging
        [Tooltip("用于贴壁的光线投射的垂直偏移量")]
        public float RaycastVerticalOffset = 0f;
        /// the tolerance applied to compensate for tiny irregularities in the wall (slightly misplaced tiles for example)
        [Tooltip("用于补偿墙中微小不规则的公差(例如，轻微错位的瓷砖)")]
        public float WallClingingTolerance = 0.3f;

        [Header("Automation")]

        /// if this is set to true, you won't need to press the opposite direction to wall cling, it'll be automatic anytime the character faces a wall
        [Tooltip("如果这个设置为真，你就不需要按墙贴的相反方向，它会自动在角色面对墙的任何时候")]
        public bool InputIndependent = false;        

        protected CharacterStates.MovementStates _stateLastFrame;
        protected RaycastHit2D _raycast;
        protected WallClingingOverride _wallClingingOverride;

        // animation parameters
        protected const string _wallClingingAnimationParameterName = "WallClinging";
        protected int _wallClingingAnimationParameter;

        /// <summary>
        /// Checks the input to see if we should enter the WallClinging state
        /// </summary>
        protected override void HandleInput()
		{
			WallClinging();
		}

		/// <summary>
		/// Every frame, checks if the wallclinging state should be exited
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();

			ExitWallClinging();
			WallClingingLastFrame ();
		}

		/// <summary>
	    /// Makes the player stick to a wall when jumping
	    /// </summary>
	    protected virtual void WallClinging()
		{
			if (!AbilityAuthorized
				|| (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
				|| (_controller.State.IsGrounded)
                || (_controller.State.ColliderResized)
                || (_controller.Speed.y >= 0) )
			{
				return;
			}
            
            if (InputIndependent)
            {
                if (TestForWall())
                {
                    EnterWallClinging();
                }
            }
            else
            {
                if (((_controller.State.IsCollidingLeft) && (_horizontalInput <= -_inputManager.Threshold.x))
                                || ((_controller.State.IsCollidingRight) && (_horizontalInput >= _inputManager.Threshold.x)))
                {
                    EnterWallClinging();
                }
            }            
		}

        /// <summary>
        /// Casts a ray to check if we're facing a wall
        /// </summary>
        /// <returns></returns>
        protected virtual bool TestForWall()
        {
            // we then cast a ray to the direction's the character is facing, in a down diagonal.
            // we could use the controller's IsCollidingLeft/Right for that, but this technique 
            // compensates for walls that have small holes or are not perfectly flat
            Vector3 raycastOrigin = transform.position;
            Vector3 raycastDirection;
            if (_character.IsFacingRight)
            {
                raycastOrigin = raycastOrigin + transform.right * _controller.Width() / 2 + transform.up * RaycastVerticalOffset;
                raycastDirection = transform.right - transform.up;
            }
            else
            {
                raycastOrigin = raycastOrigin - transform.right * _controller.Width() / 2 + transform.up * RaycastVerticalOffset;
                raycastDirection = -transform.right - transform.up;
            }

            // we cast our ray 
            _raycast = MMDebug.RayCast(raycastOrigin, raycastDirection, WallClingingTolerance, _controller.PlatformMask & ~(_controller.OneWayPlatformMask | _controller.MovingOneWayPlatformMask), Color.black, _controller.Parameters.DrawRaycastsGizmos);

            // we check if the ray hit anything. If it didn't, or if we're not moving in the direction of the wall, we exit
            return _raycast;
        }

        /// <summary>
        /// Enters the wall clinging state
        /// </summary>
        protected virtual void EnterWallClinging()
        {
            // we check for an override
            if (_controller.CurrentWallCollider != null)
            {
                _wallClingingOverride = _controller.CurrentWallCollider.gameObject.MMGetComponentNoAlloc<WallClingingOverride>();
            }
            else if (_raycast.collider != null)
            {
                _wallClingingOverride = _raycast.collider.gameObject.MMGetComponentNoAlloc<WallClingingOverride>();
            }
            
            if (_wallClingingOverride != null)
            {
                // if we can't wallcling to this wall, we do nothing and exit
                if (!_wallClingingOverride.CanWallClingToThis)
                {
                    return;
                }
                _controller.SlowFall(_wallClingingOverride.WallClingingSlowFactor);
            }
            else
            {
                // we slow the controller's fall speed
                _controller.SlowFall(WallClingingSlowFactor);
            }

            // if we weren't wallclinging before this frame, we start our sounds
            if ((_movement.CurrentState != CharacterStates.MovementStates.WallClinging) && !_startFeedbackIsPlaying)
            {
                // we start our feedbacks
                PlayAbilityStartFeedbacks();
            }

            _movement.ChangeState(CharacterStates.MovementStates.WallClinging);
        }

		/// <summary>
		/// If the character is currently wallclinging, checks if we should exit the state
		/// </summary>
		protected virtual void ExitWallClinging()
		{
			if (_movement.CurrentState == CharacterStates.MovementStates.WallClinging)
			{
				// we prepare a boolean to store our exit condition value
				bool shouldExit = false;
				if ((_controller.State.IsGrounded) // if the character is grounded
					|| (_controller.Speed.y >= 0))  // or if it's moving up
				{
                    // then we should exit
					shouldExit = true;
				}

				// we then cast a ray to the direction's the character is facing, in a down diagonal.
				// we could use the controller's IsCollidingLeft/Right for that, but this technique 
				// compensates for walls that have small holes or are not perfectly flat
				Vector3 raycastOrigin=transform.position;
				Vector3 raycastDirection;
				if (_character.IsFacingRight) 
				{ 
					raycastOrigin = raycastOrigin + transform.right * _controller.Width()/ 2 + transform.up * RaycastVerticalOffset;
					raycastDirection = transform.right - transform.up; 
				}
				else
				{
					raycastOrigin = raycastOrigin - transform.right * _controller.Width()/ 2 + transform.up * RaycastVerticalOffset;
					raycastDirection = - transform.right - transform.up;
				}
                				
				// we check if the ray hit anything. If it didn't, or if we're not moving in the direction of the wall, we exit
                if (!InputIndependent)
                {
                    // we cast our ray 
                    RaycastHit2D hit = MMDebug.RayCast(raycastOrigin, raycastDirection, WallClingingTolerance, _controller.PlatformMask & ~(_controller.OneWayPlatformMask | _controller.MovingOneWayPlatformMask), Color.black, _controller.Parameters.DrawRaycastsGizmos);
                    
                    if (_character.IsFacingRight)
                    {
                        if ((!hit) || (_horizontalInput <= _inputManager.Threshold.x))
                        {
                            shouldExit = true;
                        }
                    }
                    else
                    {
                        if ((!hit) || (_horizontalInput >= -_inputManager.Threshold.x))
                        {
                            shouldExit = true;
                        }
                    }
                }
                else
                {
                    if (_raycast.collider == null)
                    {
                        shouldExit = true;
                    }
                }
				
				if (shouldExit)
				{
                    ProcessExit();
                }
			}

            if ((_stateLastFrame == CharacterStates.MovementStates.WallClinging) 
                && (_movement.CurrentState != CharacterStates.MovementStates.WallClinging)
                && _startFeedbackIsPlaying)
            {
                // we play our exit feedbacks
                StopStartFeedbacks();
                PlayAbilityStopFeedbacks();
            }

            _stateLastFrame = _movement.CurrentState;
        }

        protected virtual void ProcessExit()
        {
            // if we're not wallclinging anymore, we reset the slowFall factor, and reset our state.
            _controller.SlowFall(0f);
            // we reset the state
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
        }

		/// <summary>
		/// This methods tests if we were wallcling previously, and if so, resets the slowfall factor and stops the wallclinging sound
		/// </summary>
		protected virtual void WallClingingLastFrame()
		{
			if ((_movement.PreviousState == CharacterStates.MovementStates.WallClinging) 
                && (_movement.CurrentState != CharacterStates.MovementStates.WallClinging)
                && _startFeedbackIsPlaying)
            {
                _controller.SlowFall (0f);	
				StopStartFeedbacks();
			}
		}
        
        protected override void OnDeath()
        {
            base.OnDeath();
            ProcessExit();
        }

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter (_wallClingingAnimationParameterName, AnimatorControllerParameterType.Bool, out _wallClingingAnimationParameter);
		}

		/// <summary>
		/// Updates the animator with the current wallclinging state
		/// </summary>
		public override void UpdateAnimator()
		{
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _wallClingingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.WallClinging), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
		}
		
		/// <summary>
		/// On reset ability, we cancel all the changes made
		/// </summary>
		public override void ResetAbility()
		{
			base.ResetAbility();
			if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
			{
				ProcessExit();	
			}
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _wallClingingAnimationParameter, false, _character._animatorParameters, _character.PerformAnimatorSanityChecks);
		}
	}
}