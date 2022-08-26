using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Add this ability to a Character and it'll be able to glide through the air, slowing its fall when pressing the Glide button (by default the same binding as the Jump button, but separated for convenience)
    /// 
	/// Animator parameters : Gliding (bool)
    /// </summary>
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Glide")]
    public class CharacterGlide : CharacterAbility
    {
        public override string HelpBoxText() { return "这个组件允许角色在空中滑翔，当按下滑翔按钮时减慢下落速度(默认绑定与跳跃按钮相同，但为了方便分开)。" +
                "在这里你可以定义用于减缓下落的力量，以及是否应该等待角色用尽所有跳跃(否则它将优先于第一次跳跃之后的任何跳跃)。"; }

        /// the force to apply when gliding
        [Tooltip("滑行时施加的力")]
        public float VerticalForce = 0.1f;
        /// whether or not the glide will wait for jumps to be exhausted
        [Tooltip("滑翔是否会等待跳跃被耗尽")]
        public bool GlideOnlyIfNoJumpsLeft = true;

        protected bool _gliding;
        protected CharacterJump _characterJump;
        protected CharacterWalljump _characterWallJump;
        protected CharacterSwim _characterSwim;
        
        // animation parameters
        protected const string _glidingAnimationParameterName = "Gliding";
        protected int _glidingAnimationParameter;

        /// <summary>
        /// On Start we grab our components
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            _characterJump = _character?.FindAbility<CharacterJump>();
            _characterWallJump = _character?.FindAbility<CharacterWalljump>();
            _characterSwim = _character?.FindAbility<CharacterSwim>();
        }

        /// <summary>
        /// Looks for glide related inputs
        /// </summary>
        protected override void HandleInput()
        {
            base.HandleInput();
            if (_inputManager.GlideButton.State.CurrentState == MMInput.ButtonStates.ButtonDown )
            {
                GlideStart();
            }

            if (_inputManager.GlideButton.State.CurrentState == MMInput.ButtonStates.ButtonUp && _gliding)
            {
                GlideStop();
            }
        }

        /// <summary>
        /// When pressing the glide button we make sure we can glide, and initiate it
        /// </summary>
        protected virtual void GlideStart()
        {
            if ((!AbilityAuthorized) // if the ability is not permitted
                || (_controller.State.IsGrounded) // or if we're on the ground
                || (_movement.CurrentState == CharacterStates.MovementStates.Dashing) // or if we're dashing
                || (_movement.CurrentState == CharacterStates.MovementStates.WallClinging) // or if we're wallclinging
                || (_movement.CurrentState == CharacterStates.MovementStates.Gripping) // or if we're in the gripping state
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)) // or if we're not in normal conditions
            {
                return;
            }

            if (_characterSwim != null)
            {
                if (_characterSwim.InWater)
                {
                    return;
                }                
            }

            // if we're walljumping, we prevent the character from gliding
            if (_characterWallJump != null)
            {
                if ((_movement.CurrentState == CharacterStates.MovementStates.WallJumping) && _characterWallJump.WallJumpHappenedThisFrame)
                {
                    return;
                }
            }

            // if we want to wait for the character to not have any jumps left, and if conditions are met, we prevent it from gliding
            if (GlideOnlyIfNoJumpsLeft 
                && (_characterJump != null))
            { 
                if ((_characterJump.NumberOfJumpsLeft > 0) || (_characterJump.JumpHappenedThisFrame))
                {
                    return;
                }
            }

            // if this is the first time we're here, we trigger our sounds
            if (_movement.CurrentState != CharacterStates.MovementStates.Gliding)
            {
                // we play the gliding start sound 
                PlayAbilityStartFeedbacks();
                _gliding = true;
            }

            _movement.ChangeState(CharacterStates.MovementStates.Gliding);
        }

        /// <summary>
        /// Stops the character from gliding
        /// </summary>
        protected virtual void GlideStop()
        {           
            // we play our stop sound
            if (_movement.CurrentState == CharacterStates.MovementStates.Gliding)
            {
                StopStartFeedbacks();
                PlayAbilityStopFeedbacks();
            }
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
            _gliding = false;
        }
    
        /// <summary>
        /// Stops the character from gliding if needed
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();

            // if we're not gliding anymore, we stop our walking sound
            if (_movement.CurrentState != CharacterStates.MovementStates.Gliding && _startFeedbackIsPlaying)
            {
                StopStartFeedbacks();
            }

            // if we're not in the gliding state anymore
            if (_movement.CurrentState != CharacterStates.MovementStates.Gliding && _gliding)
            {
                GlideStop();
            }

            // if we're touching the ground
            if (_controller.State.IsCollidingBelow && _gliding)
            {
                GlideStop();
            }

            // if we're colliding with something above (which shouldn't happen for regular glides but can happen when applying high forces)
            if (_controller.State.IsCollidingAbove && (_movement.CurrentState == CharacterStates.MovementStates.Gliding))
            {
                _controller.SetVerticalForce(0);
            }

            // if we're gliding, we apply our force
            if (_gliding)
            {
                _controller.SetVerticalForce(VerticalForce);
            }
        }

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_glidingAnimationParameterName, AnimatorControllerParameterType.Bool, out _glidingAnimationParameter);
        }

        /// <summary>
        /// At the end of each cycle, we send our character's animator the current gliding status
        /// </summary>
        public override void UpdateAnimator()
        {
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _glidingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Gliding), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
        }

        /// <summary>
        /// On reset ability, we cancel all the changes made
        /// </summary>
        public override void ResetAbility()
        {
            base.ResetAbility();
            GlideStop();
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _glidingAnimationParameter, false, _character._animatorParameters, _character.PerformAnimatorSanityChecks);
        }
    }
}
