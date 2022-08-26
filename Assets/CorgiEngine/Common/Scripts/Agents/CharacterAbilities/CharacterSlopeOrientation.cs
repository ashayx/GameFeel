using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Add this component to a Character and it'll rotate according to the current slope angle.
    /// Animator parameters : none
    /// 加这个组件到一个字符，它会根据当前的倾斜角度旋转。
    ///动画参数:无
    /// </summary>
    [MMHiddenProperties("AbilityStartFeedbacks", "AbilityStopFeedbacks")]
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Slope Orientation")] 
	public class CharacterSlopeOrientation : CharacterAbility 
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "这个组件将引导角色的模型，使它垂直于它行走的斜坡。注意，这只在你的模型不是在角色的顶层，而是嵌套在它下面的情况下有效。"; }
        /// The object to rotate when walking on slopes. A good hierarchy is like so :
        /// - top level : Corgi Controller, collider, character, abilities, etc
        /// - - slope object to rotate
        /// - - - model 
        [Tooltip("在斜坡上行走时旋转的物体。一个好的等级制度是这样的::\n" +
            "- top level : Corgi Controller, collider, character, abilities, etc\n" +
            "- - slope object to rotate\n" +
            "- - - model ")]
        public GameObject ObjectToRotate;

		[Header("Rotation")]
		[MMInformation("在这里你可以定义角色旋转的速度以垂直于斜率。0表示瞬时旋转，低值为慢，高值为快，10为默认值。你也可以指定你的角色旋转被夹住的最小和最大角度。", MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]

        /// the rotation at which to rotate the object
        [Tooltip("旋转物体的旋转角度")]
        public float CharacterRotationSpeed = 10f;
        /// the minimum angle the rotation will be clamped at
		[Tooltip("旋转被夹紧的最小角度")]
        public float MinimumAllowedAngle = -90f;
        /// the maximum angle the rotation will be clamped at
		[Tooltip("旋转被夹紧的最大角度")]
        public float MaximumAllowedAngle = 90f;
        /// should the rotation be reset when the character jumps
		[Tooltip("角色跳跃时旋转是否应该重置")]
        public bool ResetAngleInTheAir = true;
        /// should the weapon rotate as well
		[Tooltip("武器也应该旋转吗")]
        public bool RotateWeapon = true;
        /// the slope detection raycast length
        [Tooltip("坡度检测射线投射长度")]
        public float RaycastLength = 1f;

        protected GameObject _model;
		protected Quaternion _newRotation;
		protected float _currentAngle;
		protected CharacterHandleWeapon _handleWeapon;
		protected WeaponAim _weaponAim;

        protected float _rayLength;
        protected RaycastHit2D _raycastLeft;
        protected RaycastHit2D _raycastMid;
        protected RaycastHit2D _raycastRight;
        protected Vector3 _slopeAngleCross;

        /// <summary>
        /// On Start(), we set our tunnel flag to false
        /// </summary>
        protected override void Initialization()
		{
			base.Initialization();

            if (ObjectToRotate != null)
            {
                _model = ObjectToRotate;
            }
            else
            {
                _model = _character.CharacterModel;
            }			

			_handleWeapon = _character?.FindAbility<CharacterHandleWeapon> ();
			if (_handleWeapon != null)
			{
				if (_handleWeapon.CurrentWeapon != null)
				{
					_weaponAim = _handleWeapon.CurrentWeapon.GetComponent<WeaponAim> ();
				}
			}
		}
        
        /// <summary>
        /// Every frame, we check if we're crouched and if we still should be
        /// </summary>
        public override void ProcessAbility()
		{
			base.ProcessAbility();

			// if we don't have a model, we do nothing and exit
			if (_model == null)
			{
				return;
			}

            _currentAngle = DetermineAngle();
            
			if (_characterGravity != null)
			{
				_currentAngle += _characterGravity.GravityAngle;
			}

			// we determine the new rotation
			_newRotation = Quaternion.Euler (_currentAngle * Vector3.forward);

			// if we want instant rotation, we apply it directly
			if (CharacterRotationSpeed == 0)
			{
				_model.transform.rotation = _newRotation;	
			}
			// otherwise we lerp the rotation
			else
			{				
				_model.transform.rotation = Quaternion.Lerp (_model.transform.rotation, _newRotation, CharacterRotationSpeed * Time.deltaTime);
			}

            if ((_weaponAim == null) && (_handleWeapon != null))
            {
                if (_handleWeapon.CurrentWeapon != null)
                {
                    _weaponAim = _handleWeapon.CurrentWeapon.GetComponent<WeaponAim>();
                }
            }

			// if we're supposed to also rotate the weapon
			if (RotateWeapon && (_weaponAim != null))
			{
				if (_characterGravity != null)
				{
					_currentAngle -= _characterGravity.GravityAngle;
				}
                _weaponAim.ResetAdditionalAngle();
                _weaponAim.AddAdditionalAngle (_currentAngle);
			}
		}

        /// <summary>
        /// Determines the angle to consider when orientating
        /// </summary>
        /// <returns></returns>
        protected virtual float DetermineAngle()
        {
            float currentAngle;

            _raycastLeft = MMDebug.RayCast(_controller.BoundsLeft, -transform.up, RaycastLength, _controller.PlatformMask, MMColors.LightBlue, _controller.Parameters.DrawRaycastsGizmos);
            _raycastMid = MMDebug.RayCast(_controller.BoundsCenter, -transform.up, RaycastLength, _controller.PlatformMask, MMColors.LightBlue, _controller.Parameters.DrawRaycastsGizmos);
            _raycastRight = MMDebug.RayCast(_controller.BoundsRight, -transform.up, RaycastLength, _controller.PlatformMask, MMColors.LightBlue, _controller.Parameters.DrawRaycastsGizmos);

            float leftAngle = _raycastLeft ? ComputeSlopeAngle(_raycastLeft.normal) : 0f;
            float midAngle = _raycastMid ? ComputeSlopeAngle(_raycastMid.normal) : 0f;
            float rightAngle = _raycastRight ? ComputeSlopeAngle(_raycastRight.normal) : 0f;
            float meanAngle = 0f;

            meanAngle = (leftAngle + midAngle + rightAngle) / 3f;

            currentAngle = meanAngle;

            // if we're in the air and if we should be resetting the angle, we reset it
            if ((!_controller.State.IsGrounded) && ResetAngleInTheAir)
            {
                currentAngle = 0;
            }
            
            // we clamp our angle
            currentAngle = Mathf.Clamp(currentAngle, MinimumAllowedAngle, MaximumAllowedAngle);

            return currentAngle;
        }

        /// <summary>
        /// Computes the angle of the slope
        /// </summary>
        /// <param name="normal"></param>
        /// <returns></returns>
        protected virtual float ComputeSlopeAngle(Vector2 normal)
        {
            float slopeAngle;
            slopeAngle = Vector2.Angle(normal, transform.up);
            _slopeAngleCross = Vector3.Cross(transform.up, normal);
            if (_slopeAngleCross.z < 0)
            {
                slopeAngle = -slopeAngle;
            }
            return slopeAngle;
        }
	}
}