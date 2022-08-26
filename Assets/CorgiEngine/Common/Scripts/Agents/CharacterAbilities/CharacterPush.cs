using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// Add this class to a Character and it'll be able to push objects around. It's an optional class as you can push objects without it,
    /// but it'll allow you to have a "pushing" animation. For the animation to work, your pushable objects will need to have a 
    /// Pushable component.
    /// Animator parameters : Pushing (bool)
    ///添加这个类到一个字符，它将能够推动对象周围。它是一个可选的类，因为你可以在没有它的情况下推送对象，
    ///但是它允许你有一个“push”动画。为了使动画工作，你的可推对象需要有一个
    /// Pushable组件。
    ///动画参数:push (bool)
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Push")]
    public class CharacterPush : CharacterAbility
    {
        public override string HelpBoxText() { return "这个组件允许你的角色推方块。这不是一个强制组件，它只会覆盖CorgiController的推送设置，并允许你有一个专用的推送动画。"; }

        /// If this is set to true, the Character will be able to push blocks
        [Tooltip("If this is set to true, the Character will be able to push blocks")]
        public bool CanPush = true;
        /// the (x) force applied to the pushed object
        [Tooltip("the (x) force applied to the pushed object")]
        public float PushForce = 2f;
        /// if this is true, the Character will only be able to push objects while grounded
        [Tooltip("如果这是真的，角色将只能推动物体在地面上")]
        public bool PushWhenGroundedOnly = true;
        /// the length of the raycast used to detect if we're colliding with a pushable object. Increase this if your animation is flickering.
        [Tooltip("光线投射的长度，用于检测我们是否与一个可推的物体碰撞。如果你的动画正在闪烁，请增加这个值。")]
        public float DetectionRaycastLength = 0.2f;
        /// the minimum horizontal speed below which we don't consider the character pushing anymore
        [Tooltip("低于这个最小水平速度我们就不考虑角色推动了")]
        public float MinimumPushSpeed = 0.05f;

        protected bool _collidingWithPushable = false;
        protected Vector3 _raycastDirection;
        protected Vector3 _raycastOrigin;

        // animation parameters
        protected const string _pushingAnimationParameterName = "Pushing";
        protected int _pushingAnimationParameter;

        /// <summary>
        /// On Start(), we initialize our various flags
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            _controller.Parameters.Physics2DInteraction = CanPush;
            _controller.Parameters.Physics2DPushForce = PushForce;
        }

        /// <summary>
        /// Every frame we override parameters if needed and cast a ray to see if we're actually pushing anything
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();

            if (!CanPush || !AbilityAuthorized)
            {
                return;
            }
            // if we can only push when grounded and we're not grounded we turn our push force off
            if (PushWhenGroundedOnly && !_controller.State.IsGrounded)
            {
                _controller.Parameters.Physics2DPushForce = 0f;
                return;
            }
            else
            {
                _controller.Parameters.Physics2DPushForce = PushForce;
            }

            // we set our flag to false
            _collidingWithPushable = false;

            // we cast a ray in front of us to see if we're colliding with a pushable object
            _raycastDirection = _character.IsFacingRight ? transform.right : -transform.right;
            _raycastOrigin = _controller.ColliderCenterPosition + _raycastDirection * (_controller.Width() / 2);


            // we cast our ray to see if we're hitting something
            RaycastHit2D hit = MMDebug.RayCast(_raycastOrigin, _raycastDirection, DetectionRaycastLength, _controller.PlatformMask, Color.green, _controller.Parameters.DrawRaycastsGizmos);
            if (hit)
            {
                if (hit.collider.gameObject.MMGetComponentNoAlloc<Pushable>() != null)
                {
                    _collidingWithPushable = true;
                }
            }

            if (_controller.State.IsGrounded
                && _collidingWithPushable
                && (Mathf.Abs(_controller.Speed.x) > MinimumPushSpeed)
                && (_movement.CurrentState != CharacterStates.MovementStates.Pushing)
                && (_movement.CurrentState != CharacterStates.MovementStates.Jumping)
                && (_movement.CurrentState != CharacterStates.MovementStates.Crouching)
                && (_movement.CurrentState != CharacterStates.MovementStates.Crawling)
                && !_startFeedbackIsPlaying)
            {
                PlayAbilityStartFeedbacks();
                _movement.ChangeState(CharacterStates.MovementStates.Pushing);
            }

            if ((!_collidingWithPushable && _movement.CurrentState == CharacterStates.MovementStates.Pushing)
                || (_collidingWithPushable && Mathf.Abs(_controller.Speed.x) <= MinimumPushSpeed && _movement.CurrentState == CharacterStates.MovementStates.Pushing))
            {
                // we reset the state
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
                StopStartFeedbacks();
                PlayAbilityStopFeedbacks();

            }

            if ((_movement.CurrentState != CharacterStates.MovementStates.Pushing) && _startFeedbackIsPlaying)
            {
                StopStartFeedbacks();
                PlayAbilityStopFeedbacks();
            }
        }

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_pushingAnimationParameterName, AnimatorControllerParameterType.Bool, out _pushingAnimationParameter);
        }

        /// <summary>
        /// Sends the current state of the Diving state to the character's animator
        /// </summary>
        public override void UpdateAnimator()
        {
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _pushingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Pushing), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
        }

        /// <summary>
        /// On reset ability, we cancel all the changes made
        /// </summary>
        public override void ResetAbility()
        {
            base.ResetAbility();
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _pushingAnimationParameter, false, _character._animatorParameters, _character.PerformAnimatorSanityChecks);
        }
    }
}