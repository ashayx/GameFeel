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
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Switch Model Parent")] 
	public class CharacterSwitchModelParent : CharacterAbility
    {
        /// the possible orders the next character can be selected from
        public enum NextModelChoices { Sequential, Random }
        public enum Index { Int, Random }

        [Header("Models")]
        [MMInformation("将这个组件添加到一个角色中，当按下SwitchCharacter按钮时，它将能够切换它的模型(默认为P).", MMInformationAttribute.InformationType.Info, false)]

        /// the list of possible characters models to switch to
        [Tooltip("可能切换到的角色模型列表")]
        public Transform CharacterModels;
        /// the order in which to pick the next character
        [Tooltip("选择下一个角色的顺序——可能切换到的角色模型列表")]
        public NextModelChoices NextCharacterChoice = NextModelChoices.Sequential;
        /// the initial (and at runtime, current) index of the character prefab
        public Index IndexMode = Index.Int;
        [MMEnumConditionAttribute("IndexMode", (int)Index.Int)][Tooltip("预制角色的初始(运行时，当前)索引")]
        public int CurrentIndex = 0;


        /// <summary>
        /// On init we disable our models and activate the current one
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            if (CharacterModels.childCount == 0)
            {
                return;
            }

            for (int i = 0; i < CharacterModels.childCount; i++)
            {
                CharacterModels.GetChild(i).gameObject.SetActive(false);
            }

            if (IndexMode == Index.Random)
            {
                CurrentIndex = Random.Range(0, CharacterModels.childCount);
            }

            CharacterModels.GetChild(CurrentIndex).gameObject.SetActive(true);

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
        /// Switches to the next model in line
        /// </summary>
		protected virtual void SwitchModel()
        {
            if (CharacterModels.childCount <= 1)
            {
                return;
            }

            CharacterModels.GetChild(CurrentIndex).gameObject.SetActive(false);

            // we determine the next index
            if (NextCharacterChoice == NextModelChoices.Random)
            {
                CurrentIndex = Random.Range(0, CharacterModels.childCount);
            }
            else
            {
                CurrentIndex = CurrentIndex + 1;
                if (CurrentIndex >= CharacterModels.childCount)
                {
                    CurrentIndex = 0;
                }
            }

            // we activate the new current model
            CharacterModels.GetChild(CurrentIndex).gameObject.SetActive(true);

            // we play our feedback
            PlayAbilityStartFeedbacks();
        }
	}
}
