using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    ///给一个角色添加这个能力，让它可以通过按下游泳按钮在水中游泳(默认绑定和跳跃按钮相同，但为了方便分开)
    ///
    ///动画参数:Swimming (bool)， SwimmingIdle (bool)
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Swim")]
    public class CharacterSwim : CharacterAbility
    {
        public override string HelpBoxText() { return "该组件允许角色通过按下游泳按钮在水中游泳(默认情况下与跳跃按钮绑定相同，但为了方便分开)。" +
                "在这里，你可以定义要应用的游泳力，相关动画的持续时间，以及进入/退出水时要实例化的VFX，以及退出时要应用的力。"; }

        /// whether or not the character is in water
        [MMReadOnly]
        [Tooltip("whether or not the character is in water")]
        public bool InWater = false;

        [Header("Swim")]

        /// defines how high the character can jump
		[Tooltip("定义角色可以跳多高")]
        public float SwimHeight = 3.025f;
        /// the duration (in seconds) of the swim animation before it reverts back to swim idle
        [Tooltip("游泳动画恢复到空闲状态之前的持续时间(以秒为单位)")]
        public float SwimAnimationDuration = 0.8f;

        [Header("Splash Effects")]

        /// the effect that will be instantiated everytime the character enters the water
        [Tooltip("每次角色进入水中都会被实例化的效果")]
        public GameObject WaterEntryEffect;
        /// the effect that will be instantiated everytime the character exits the water
        [Tooltip("每次角色离开水的时候都会实例化的效果")]
        public GameObject WaterExitEffect;
        /// the force to apply to the character when exiting water
        [Tooltip("出水时施加在角色上的力")]
        public Vector2 WaterExitForce = new Vector2(0f, 12f);

        protected float _swimDurationLeft = 0f;

        /// <summary>
        /// On Update we decrease our counter
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            _swimDurationLeft -= Time.deltaTime;
        }

        /// <summary>
		/// At the beginning of each cycle we check if we've just pressed or released the swim button
		/// </summary>
		protected override void HandleInput()
        {
            if (!InWater)
            {
                return;
            }

            if (_inputManager.SwimButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {
                Swim();
            }
        }

        /// <summary>
        /// When swimming we apply our swim force
        /// </summary>
        protected virtual void Swim()
        {
            _movement.ChangeState(CharacterStates.MovementStates.SwimmingIdle);
            _controller.SetVerticalForce(Mathf.Sqrt(2f * SwimHeight * Mathf.Abs(_controller.Parameters.Gravity)));
            _swimDurationLeft = SwimAnimationDuration;
        }

        /// <summary>
        /// When entering the water we instantiate a splash if needed and change our state
        /// </summary>
        public virtual void EnterWater()
        {
            InWater = true;
            PlayAbilityStartFeedbacks();
            _movement.ChangeState(CharacterStates.MovementStates.SwimmingIdle);
            if (WaterEntryEffect != null)
            {
                Instantiate(WaterEntryEffect, this.transform.position, Quaternion.identity);
            }            
        }

        /// <summary>
        /// When exiting the water we instantiate a splash if needed and change our state
        /// </summary>
        public virtual void ExitWater()
        {
            InWater = false;
            StopStartFeedbacks();
            PlayAbilityStopFeedbacks();
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
            _controller.SetForce(WaterExitForce);

            if (WaterExitEffect != null)
            {
                Instantiate(WaterExitEffect, this.transform.position, Quaternion.identity);
            }            
        }

        // animation parameters
        protected const string _swimmingAnimationParameterName = "Swimming";
        protected const string _swimmingIdleAnimationParameterName = "SwimmingIdle";
        protected int _swimmingAnimationParameter;
        protected int _swimmingIdleAnimationParameter;

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_swimmingAnimationParameterName, AnimatorControllerParameterType.Bool, out _swimmingAnimationParameter);
            RegisterAnimatorParameter(_swimmingIdleAnimationParameterName, AnimatorControllerParameterType.Bool, out _swimmingIdleAnimationParameter);
        }

        /// <summary>
        /// At the end of each cycle, we send our Running status to the character's animator
        /// </summary>
        public override void UpdateAnimator()
        {
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _swimmingAnimationParameter, (_swimDurationLeft > 0f), _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _swimmingIdleAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.SwimmingIdle), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
        }

        protected override void OnDeath()
        {
            base.OnDeath();

            InWater = false;
        }

        /// <summary>
        /// On reset ability, we cancel all the changes made
        /// </summary>
        public override void ResetAbility()
        {
            base.ResetAbility();
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _swimmingAnimationParameter, false, _character._animatorParameters, _character.PerformAnimatorSanityChecks);
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _swimmingIdleAnimationParameter, false, _character._animatorParameters, _character.PerformAnimatorSanityChecks);
        }
    }
}
