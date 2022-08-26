using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    ///为角色添加这个类，这样它就可以使用武器
    ///注意这个组件将触发动画(如果他们的参数在Animator中存在)，基于
    ///当前武器的动画
    ///动画参数:从武器的检查器中定义
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Handle Secondary Weapon")]
    public class CharacterHandleSecondaryWeapon : CharacterHandleWeapon
    {
        ///这个CharacterHandleWeapon的ID /索引。这将用来决定什么样的处理武器能力应该装备武器。
        ///如果你创建了更多的Handle Weapon技能，请确保覆盖并增加它
        public override int HandleWeaponID { get { return 2; } }

        /// <summary>
        /// Gets input and triggers methods based on what's been pressed
        /// </summary>
        protected override void HandleInput()
        {
            if ((_inputManager.SecondaryShootButton.State.CurrentState == MMInput.ButtonStates.ButtonDown) || (_inputManager.SecondaryShootAxis == MMInput.ButtonStates.ButtonDown))
            {
                ShootStart();
            }

            if (CurrentWeapon != null)
            {
                if (ContinuousPress && (CurrentWeapon.TriggerMode == Weapon.TriggerModes.Auto) && (_inputManager.SecondaryShootButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed))
                {
                    ShootStart();
                }
                if (ContinuousPress && (CurrentWeapon.TriggerMode == Weapon.TriggerModes.Auto) && (_inputManager.SecondaryShootAxis == MMInput.ButtonStates.ButtonPressed))
                {
                    ShootStart();
                }
            }

            if (_inputManager.ReloadButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {
                Reload();
            }

            if ((_inputManager.SecondaryShootButton.State.CurrentState == MMInput.ButtonStates.ButtonUp) || (_inputManager.SecondaryShootAxis == MMInput.ButtonStates.ButtonUp))
            {
                ShootStop();
            }

            if (CurrentWeapon != null)
            {
                if ((CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBetweenUses)
                && ((_inputManager.SecondaryShootAxis == MMInput.ButtonStates.Off) && (_inputManager.SecondaryShootButton.State.CurrentState == MMInput.ButtonStates.Off)))
                {
                    CurrentWeapon.WeaponInputStop();
                }
            }
        }
    }
}