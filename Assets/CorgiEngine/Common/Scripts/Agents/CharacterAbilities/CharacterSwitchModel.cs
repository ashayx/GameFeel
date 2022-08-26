using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Add this component to a character and it'll be able to switch its model
    /// when pressing the SwitchCharacter button
    /// Note that this will only change the model, not the prefab. Only the visual representation, not the abilities and settings.
    /// If instead you'd like to change the prefab entirely, look at the CharacterSwitchManager class.
    /// If you want to swap characters between a bunch of characters within a scene, look at the CharacterSwap ability and CharacterSwapManager
    ///添加这个组件到一个角色，它将能够切换它的模型
    ///当按下SwitchCharacter按钮时
    ///注意这只会改变模型，而不是预制件。只有视觉表现，没有能力和设置。
    ///如果你想完全改变预制，看看CharacterSwitchManager类。
    ///如果你想在场景中的一堆角色之间交换角色，看看CharacterSwap功能和CharacterSwapManager
    /// </summary>
    [MMHiddenProperties("AbilityStopFeedbacks")]
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Switch Model")] 
	public class CharacterSwitchModel : CharacterAbility
    {
        /// the possible orders the next character can be selected from
        public enum NextModelChoices { Sequential, Random }

        [Header("Models")]
        [MMInformation("将这个组件添加到一个角色中，当按下SwitchCharacter按钮时，它将能够切换它的模型(默认为P).", MMInformationAttribute.InformationType.Info, false)]

        /// the list of possible characters models to switch to
        [Tooltip("可能切换到的角色模型列表")]
        public GameObject[] CharacterModels;
        /// the order in which to pick the next character
        [Tooltip("选择下一个角色的顺序——可能切换到的角色模型列表")]
        public NextModelChoices NextCharacterChoice = NextModelChoices.Sequential;
        /// the initial (and at runtime, current) index of the character prefab
        [Tooltip("预制角色的初始(运行时，当前)索引")]
        public int CurrentIndex = 0;
        /// if you set this to true, when switching model, the Character's animator will also be bound. This requires your model's animator is at the top level of the model in the hierarchy.
        /// you can look at the MinimalModelSwitch scene for examples of that
        [Tooltip("如果你设置为true，当切换模型时，角色的动画师也会被绑定。这就要求你的模型的动画师位于模型层次结构的最顶层。你可以看看MinimalModelSwitch场景中的例子")]
        public bool AutoBindAnimator = true;

        protected string _bindAnimatorMessage = "BindAnimator";
        protected bool[] _characterModelsFlipped;
        protected CharacterHandleWeapon _characterHandleWeapon;

        /// <summary>
        /// On init we disable our models and activate the current one
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            if (CharacterModels.Length == 0)
            {
                return;
            }

            foreach (GameObject model in CharacterModels)
            {
                model.SetActive(false);
            }

            CharacterModels[CurrentIndex].SetActive(true);
            _characterModelsFlipped = new bool[CharacterModels.Length];
            _characterHandleWeapon = _character?.FindAbility<CharacterHandleWeapon>();
        }

        /// <summary>
        /// At the beginning of each cycle, we check if we've pressed or released the switch button
        /// </summary>
        protected override void HandleInput()
		{
			if (_inputManager.SwitchCharacterButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
			{
                SwitchModel();
			}	
		}

        /// <summary>
        /// On flip we store our state for our current model
        /// </summary>
        public override void Flip()
        {
            if (_characterModelsFlipped == null)
            {
                _characterModelsFlipped = new bool[CharacterModels.Length];
            }
            if (_characterModelsFlipped.Length == 0)
            {
                _characterModelsFlipped = new bool[CharacterModels.Length];
            }
            if (_character == null)
            {
                _character = this.gameObject.GetComponentInParent<Character>();
            }
            
            if (!_character.IsFacingRight)
            {
                _characterModelsFlipped[CurrentIndex] = true;
            }
            else
            {
                _characterModelsFlipped[CurrentIndex] = false;
            }
        }

        /// <summary>
        /// Switches to the next model in line
        /// </summary>
		protected virtual void SwitchModel()
        {
            if (CharacterModels.Length <= 1)
            {
                return;
            }
            
            CharacterModels[CurrentIndex].gameObject.SetActive(false);

            // we determine the next index
            if (NextCharacterChoice == NextModelChoices.Random)
            {
                CurrentIndex = Random.Range(0, CharacterModels.Length);
            }
            else
            {
                CurrentIndex = CurrentIndex + 1;
                if (CurrentIndex >= CharacterModels.Length)
                {
                    CurrentIndex = 0;
                }
            }

            // we activate the new current model
            CharacterModels[CurrentIndex].gameObject.SetActive(true);
            _character.CharacterModel = CharacterModels[CurrentIndex];

            // we bind our animator
            if (AutoBindAnimator)
            {
                _character.CharacterAnimator = CharacterModels[CurrentIndex].gameObject.MMGetComponentNoAlloc<Animator>();
                _character.AssignAnimator();
                SendMessage(_bindAnimatorMessage);

                // we handle weapons
                if (_characterHandleWeapon != null)
                {
                    if (_characterHandleWeapon.CurrentWeapon != null)
                    {
                        _characterHandleWeapon.CharacterAnimator = CharacterModels[CurrentIndex].gameObject.MMGetComponentNoAlloc<Animator>();
                        _characterHandleWeapon.CurrentWeapon.SetOwner(_character, _characterHandleWeapon);
                        _characterHandleWeapon.CurrentWeapon.InitializeAnimatorParameters();
                    }
                }
            } 
            
            // we flip our character's model if needed 
            if (_character.IsFacingRight == _characterModelsFlipped[CurrentIndex])
            {
                _character.FlipModel();
                _characterModelsFlipped[CurrentIndex] = !_character.IsFacingRight;
            }

            // we play our feedback
            PlayAbilityStartFeedbacks();
        }
	}
}
