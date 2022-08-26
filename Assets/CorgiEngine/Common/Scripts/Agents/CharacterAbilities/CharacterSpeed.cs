using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// A class used to store possible speeds for any state you need
    /// </summary>
    [System.Serializable]
    public class CharacterSpeedState
    {
        /// the selected movement state
        [Tooltip("the selected movement state")]
        public CharacterStates.MovementStates State;
        /// the speed modifier to apply when in that state
        [Tooltip("the speed modifier to apply when in that state")]
        public float SpeedModifier;
    }

    /// <summary>
    /// Add this ability to a Character and you'll be able to define speed modifiers for each of its possible states
    /// This modifier will be applied to the horizontal speed of the character as long as the character is in that state
    /// You can also define a default speed multiplier to apply if none of the defined states were found
    /// Animator parameters : none
    ///添加这个能力到一个角色，你将能够定义速度修改每一个可能的状态
    ///这个修饰符将应用于角色的水平速度，只要角色处于那个状态
    ///你也可以定义一个默认的速度乘数来应用，如果没有找到定义的状态
    ///动画参数:无
    /// </summary>
    [MMHiddenProperties("AbilityStartFeedbacks", "AbilityStopFeedbacks")]
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Speed")]
    public class CharacterSpeed : CharacterAbility
    {
        /// a list of states and their corresponding speed modifiers
        [Tooltip("状态及其相应的速度修饰符的列表")]
        public List<CharacterSpeedState> States;
        /// whether or not to apply the DefaultSpeedMultiplier when none of the above states is found
        [Tooltip("当没有找到上述状态时，是否应用DefaultSpeedMultiplier")]
        public bool ApplyDefaultSpeedMultiplier = true;
        /// the default speed multiplier to apply when no other state is found
        [Tooltip("当没有找到其他状态时应用的默认速度倍率")]
        public float DefaultSpeedMultiplier = 1f;
                
        /// <summary>
        /// On late update we check our states to see if we need to apply a speed multiplier
        /// </summary>
        protected virtual void LateUpdate()
        {
            CheckStates();
        }

        /// <summary>
        /// Compares the current state against our list of speed modifiers and apply it if needed
        /// </summary>
        protected virtual void CheckStates()
        {
            bool stateFound = false;

            foreach(CharacterSpeedState state in States)
            {
                if (state.State == _movement.CurrentState)
                {
                    stateFound = true;
                    _characterHorizontalMovement.StateSpeedMultiplier = state.SpeedModifier;
                }
            }

            if (!stateFound && ApplyDefaultSpeedMultiplier)
            {
                _characterHorizontalMovement.StateSpeedMultiplier = DefaultSpeedMultiplier;
            }
        }
    }
}