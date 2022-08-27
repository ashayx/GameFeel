using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    ///切换武器
    /// </summary>
    [MMHiddenProperties("AbilityStopFeedbacks")]
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Switch Weapon")]
    public class CharacterSwitchWeapon : CharacterAbility
    {
        /// the possible orders the next character can be selected from
        public enum NextModelChoices { Sequential, Random }
        public enum Index { Int, Random }

        [Header("Models")]
        [MMInformation("将这个组件添加到一个角色中，当按下SwitchCharacter按钮时，它将能够切换它的模型(默认为P).", MMInformationAttribute.InformationType.Info, false)]

        /// the list of possible characters models to switch to
        [Tooltip("可能切换到的角色模型列表")]
        public List<Transform> CharacterModels;
        [Tooltip("可能切换到的角色模型列表")]
        public List<Weapon> WeaponList;
        /// the order in which to pick the next character
        [Tooltip("选择下一个角色的顺序——可能切换到的角色模型列表")]
        public NextModelChoices NextCharacterChoice = NextModelChoices.Sequential;
        /// the initial (and at runtime, current) index of the character prefab
        public Index IndexMode = Index.Int;
        [MMEnumConditionAttribute("IndexMode", (int)Index.Int)]
        [Tooltip("预制角色的初始(运行时，当前)索引")]
        public int CurrentIndex = 0;

        private CharacterHandleWeapon characterHandleWeapon;

        /// <summary>
        /// On init we disable our models and activate the current one
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            characterHandleWeapon = _character.FindAbility<CharacterHandleWeapon>();
            //if (CharacterModels.childCount == 0)
            //{
            //    return;
            //}

            //for (int i = 0; i < CharacterModels.childCount; i++)
            //{
            //    CharacterModels.GetChild(i).gameObject.SetActive(false);
            //}

            //if (IndexMode == Index.Random)
            //{
            //    CurrentIndex = Random.Range(0, CharacterModels.childCount);
            //}

            //CharacterModels.GetChild(CurrentIndex).gameObject.SetActive(true);

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
            if (CharacterModels.Count <= 1)
            {
                return;
            }

            CharacterModels[CurrentIndex].gameObject.SetActive(false);


            // we determine the next index
            if (NextCharacterChoice == NextModelChoices.Random)
            {
                CurrentIndex = Random.Range(0, CharacterModels.Count);
            }
            else
            {
                CurrentIndex = CurrentIndex + 1;
                if (CurrentIndex >= CharacterModels.Count)
                {
                    CurrentIndex = 0;
                }
            }

            // we activate the new current model
            CharacterModels[CurrentIndex].gameObject.SetActive(true);

            if (WeaponList[CurrentIndex])
            {
                characterHandleWeapon.ChangeWeapon(WeaponList[CurrentIndex], null);
            }
            // we play our feedback
            PlayAbilityStartFeedbacks();
        }
    }
}
