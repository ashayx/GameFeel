using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this class to a character and it'll be able to slow down time when pressing down the TimeControl button
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Time Control")] 
	public class CharacterTimeControl : CharacterAbility
	{
        [Header("Time Control")]
        [MMInformation("这个能力允许角色在按下TimeControl按钮时改变时间刻度。", MMInformationAttribute.InformationType.Info, false)]

        /// the time scale to switch to when the time control button gets pressed
        [Tooltip("按下时间控制按钮时切换到的时间刻度")]
        public float TimeScale = 0.5f;
        /// the duration for which to keep the timescale changed
        [Tooltip("the duration for which to keep the timescale changed")]
        public float Duration = 1f;
        /// whether or not the timescale should get lerped
        [Tooltip("是否应该改变时间表")]
        public bool LerpTimeScale = true;
        /// the speed at which to lerp the timescale
        [Tooltip("加速时间刻度的速度")]
        public float LerpSpeed = 5f;
        /// the cooldown for this ability
        [Tooltip("这个能力的冷却时间是时间刻度的速度")]
        public MMCooldown Cooldown;

        protected bool _timeControlled = false;

        /// <summary>
        /// Watches for input press
        /// </summary>
        protected override void HandleInput()
        {
            base.HandleInput();
            if (_inputManager.TimeControlButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {
                TimeControlStart();
            }
            if (_inputManager.TimeControlButton.State.CurrentState == MMInput.ButtonStates.ButtonUp)
            {
                TimeControlStop();
            }
        }

        /// <summary>
        /// On initialization, we init our cooldown
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            Cooldown.Initialization();
        }

        /// <summary>
        /// Starts the time scale modification
        /// </summary>
        public virtual void TimeControlStart()
        {
            if (Cooldown.Ready())
            {
                PlayAbilityStartFeedbacks();
                MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, TimeScale, Duration, LerpTimeScale, LerpSpeed, true);
                Cooldown.Start();
                _timeControlled = true;
            }
        }

        /// <summary>
        /// Stops the time control
        /// </summary>
        public virtual void TimeControlStop()
        {
            StopStartFeedbacks();
            PlayAbilityStopFeedbacks();
            Cooldown.Stop();
        }

        /// <summary>
        /// On update, we unfreeze time if needed
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            Cooldown.Update();

            if ((Cooldown.CooldownState != MMCooldown.CooldownStates.Consuming) && _timeControlled)
            {
                _timeControlled = false;
                MMTimeScaleEvent.Trigger(MMTimeScaleMethods.Unfreeze, 1f, 0f, false, 0f, false);
            }
        }

        /// <summary>
        /// On reset ability, we cancel all the changes made
        /// </summary>
        public override void ResetAbility()
        {
            base.ResetAbility();

            if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
            {
                TimeControlStop();    
            }
        }
    }
}