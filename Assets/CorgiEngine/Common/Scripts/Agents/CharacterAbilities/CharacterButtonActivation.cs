﻿using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Add this component to a character and it'll be able to interact with button activated zones and objects
    /// Animator parameters : Activating (bool)
    /// </summary>
    [MMHiddenProperties("AbilityStopFeedbacks")]
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Button Activation")]
	public class CharacterButtonActivation : CharacterAbility
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "该组件允许角色与按钮驱动的对象（对话区域、开关等）交互。 "; }
        /// true if the character is in a dialogue zone
		public bool InButtonActivatedZone {get;set;}
        /// true if the zone is automated
        public bool InButtonAutoActivatedZone { get; set; }
        /// true if the zone prevents jump 
        public bool InJumpPreventingZone { get; set; }
        ///当前按钮激活区
        public ButtonActivated ButtonActivatedZone {get;set;}

        [Header("Button Activation")]
        /// whether or not this character can jump when in a button activated zone
        [Tooltip("当处于按钮激活区域时，该角色是否可以跳")]
        public bool PreventJumpWhenInZone = true;
        /// the duration, in seconds, after an activation, during which no new activation can happen
        [Tooltip("激活后的持续时间（秒），在此期间不会发生新的激活")]
        public float ActivationCooldownDuration = 0f;

        protected bool _activating = false;
        protected float _lastActivatedAt = -10f;

        // animation parameters
        protected const string _activatingAnimationParameterName = "Activating";
        protected int _activatingAnimationParameter;

		/// <summary>
		/// Gets and stores components for further use
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			InButtonActivatedZone = false;
			ButtonActivatedZone = null;
		}

		/// <summary>
		/// Every frame, we check the input to see if we need to pause/unpause the game
		/// </summary>
		protected override void HandleInput()
		{
            if (InButtonActivatedZone && (ButtonActivatedZone != null))
            {
                if (Time.time - _lastActivatedAt < ActivationCooldownDuration)
                {
                    return;
                }

                bool buttonPressed = false;
                switch (ButtonActivatedZone.InputType)
                {
                    case ButtonActivated.InputTypes.Default:
                        buttonPressed = (_inputManager.InteractButton.State.CurrentState == MMInput.ButtonStates.ButtonDown);
                        break;
                    case ButtonActivated.InputTypes.Button:
                        buttonPressed = (Input.GetButtonDown(_character.PlayerID + "_" + ButtonActivatedZone.InputButton)) ;
                        break;
                    case ButtonActivated.InputTypes.Key:
                        buttonPressed = (Input.GetKeyDown(ButtonActivatedZone.InputKey)) ;
                        break;
                }

                if (buttonPressed)
                {
                    ButtonActivation();
                }
            }
		}

		/// <summary>
		/// Tries to activate the button activated zone
		/// </summary>
		protected virtual void ButtonActivation()
		{
			// if the player is in a button activated zone, we handle it
			if ((InButtonActivatedZone)
			    && (ButtonActivatedZone!=null)
				&& (_condition.CurrentState == CharacterStates.CharacterConditions.Normal || _condition.CurrentState == CharacterStates.CharacterConditions.Frozen)
			    && (_movement.CurrentState != CharacterStates.MovementStates.Dashing))
			{
				// if the button can only be activated while grounded and if we're not grounded, we do nothing and exit
				if (ButtonActivatedZone.CanOnlyActivateIfGrounded && !_controller.State.IsGrounded)
				{
					return;
				}
                // if it's an auto activated zone, we do nothing
                if (ButtonActivatedZone.AutoActivation && !ButtonActivatedZone.AutoActivationAndButtonInteraction)
                {
                    return;
                }
				// we trigger a character event
				MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.ButtonActivation);

				ButtonActivatedZone.TriggerButtonAction(_character.gameObject);
                PlayAbilityStartFeedbacks();

                _activating = true;
			}
		}

        /// <summary>
        /// On Death we lose any connection we may have had to a button activated zone
        /// </summary>
        protected override void OnDeath()
        {
            base.OnDeath();
            InButtonActivatedZone = false;
            ButtonActivatedZone = null;
        }

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter (_activatingAnimationParameterName, AnimatorControllerParameterType.Bool, out _activatingAnimationParameter);
		}

		/// <summary>
		/// At the end of the ability's cycle, we send our current crouching and crawling states to the animator
		/// </summary>
		public override void UpdateAnimator()
		{
            MMAnimatorExtensions.UpdateAnimatorBool(_animator,_activatingAnimationParameter, _activating, _character._animatorParameters, _character.PerformAnimatorSanityChecks);
            if (_activating && (ButtonActivatedZone != null) && (ButtonActivatedZone.AnimationTriggerParameterName != ""))
            {
                SetTriggerParameter();
            }
            _activating = false;
        }

        public virtual void SetTriggerParameter()
        {
            if ((ButtonActivatedZone != null) && (ButtonActivatedZone.AnimationTriggerParameterName != ""))
            {
                _animator.SetTrigger(ButtonActivatedZone.AnimationTriggerParameterName);
            }            
        }
    }
}
