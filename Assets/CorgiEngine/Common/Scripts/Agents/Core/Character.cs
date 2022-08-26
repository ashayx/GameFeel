using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using System;

namespace MoreMountains.CorgiEngine
{	
	[SelectionBase]
	/// <summary>
	/// This class will pilot the CorgiController component of your character.
	/// This is where you'll implement all of your character's game rules, like jump, dash, shoot, stuff like that.
	/// Animator parameters : Grounded (bool), xSpeed (float), ySpeed (float), 
	/// CollidingLeft (bool), CollidingRight (bool), CollidingBelow (bool), CollidingAbove (bool), Idle (bool)
    /// Random : a random float between 0 and 1, updated every frame, useful to add variance to your state entry transitions for example
    /// RandomConstant : a random int (between 0 and 1000), generated at Start and that'll remain constant for the entire lifetime of this animator, useful to have different characters of the same type 
    /// behave differently
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Core/Character")] 
	public class Character : MonoBehaviour
	{		
		/// the possible character types : player controller or AI (controlled by the computer)
		public enum CharacterTypes { Player, AI }
		/// the possible initial facing direction for your character
		public enum FacingDirections { Left, Right }
		/// the possible directions you can force your character to look at after its spawn
		public enum SpawnFacingDirections { Default, Left, Right }

		[MMInformation("角色脚本是所有角色能力的必备基础。你的角色可以是由AI控制的非玩家角色，也可以是由玩家控制的玩家角色。在这种情况下，您需要指定一个PlayerID，该ID必须与InputManager中指定的ID匹配。通常为“Player1”、“Player2”等", MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]

		/// Is the character player-controlled or controlled by an AI ?
		[Tooltip("角色扮演者是由人工智能控制还是由人工智能控制？")]
		public CharacterTypes CharacterType = CharacterTypes.AI;
		/// Only used if the character is player-controlled. The PlayerID must match an input manager's PlayerID. It's also used to match Unity's input settings. So you'll be safe if you keep to Player1, Player2, Player3 or Player4
		[Tooltip("仅当角色由玩家控制时使用。PlayerID必须与输入管理器的PlayerID匹配。它还用于匹配Unity的输入设置。所以如果你坚持玩1、2、3或4，你就会安全")]
		public string PlayerID = "";				
		/// the various states of the character
		public CharacterStates CharacterState { get; protected set; }
	
		[Header("Direction")]
		[MMInformation("通常，塑造所有角色都面朝右是一种很好的做法。如果该字符不是这种情况，请选择“左”", MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]

		/// true if the player is facing right
		[Tooltip("如果玩家面向右侧，则为true")]
		public FacingDirections InitialFacingDirection = FacingDirections.Right;

		[MMInformation("在这里，您可以强制角色在繁殖时应该面对的方向。如果设置为默认值，它将匹配模型的初始朝向。", MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]

		/// the direction the character will face on spawn
		[Tooltip("角色在繁殖时将面对的方向")]
		public SpawnFacingDirections DirectionOnSpawn = SpawnFacingDirections.Default;
        /// if this is true, the character is currently facing right
		public bool IsFacingRight { get; set; }
	
		[Header("Animator")]
        [MMInformation("引擎将尝试为该角色找到一个动画师。如果它在同一个游戏对象上，它应该找到它。如果它嵌套在某处，则需要将其绑定到下面。你也可以决定完全去掉它，在这种情况下，只需取消选中“使用Mecaim”", MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]

		/// the character animator
		[Tooltip("the character animator")]
		public Animator CharacterAnimator;
		/// Set this to false if you want to implement your own animation system
		[Tooltip("如果要实现自己的动画系统，请将其设置为false")]
		public bool UseDefaultMecanim = true;
		/// If this is true, sanity checks will be performed to make sure animator parameters exist before updating them. Turning this to false will increase performance but will throw errors if you're trying to update non existing parameters. Make sure your animator has the required parameters.
        [Tooltip("如果这是真的，则将执行健全性检查，以确保在更新动画参数之前存在动画参数。将此设置为false将提高性能，但如果您试图更新不存在的参数，则会引发错误。确保动画制作者具有所需的参数。")]
        public bool PerformAnimatorSanityChecks = true;
        
		[Header("Model")]
		[MMInformation("如果这是一个普通的、基于精灵的角色，并且精灵渲染器和角色位于同一个游戏对象上，请将其保持为未绑定状态。如果没有，则需要将实际模型作为角色对象的父对象，并将其绑定到下面。请参阅3D演示角色以获取示例。这背后的想法是，模型可以移动、翻转，但对撞机将保持不变。", MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]

		/// the 'model' (can be any gameobject) used to manipulate the character. Ideally it's separated (and nested) from the collider/corgi controller/abilities, to avoid messing with collisions.
		[Tooltip("用于操纵角色的“模型”（可以是任何游戏对象）。理想情况下，它与碰撞器/corgi控制器/功能分离（并嵌套），以避免碰撞。")]
		public GameObject CharacterModel;
		/// the object to use as the camera target for this character
		[Tooltip("用作此角色的相机目标的对象")]
		public GameObject CameraTarget;
		/// the speed at which the Camera Target moves
		[Tooltip("摄像机目标移动的速度")]
		public float CameraTargetSpeed = 5f;

        [Header("Abilities")]
        /// A list of gameobjects (usually nested under the Character) under which to search for additional abilities
		[Tooltip("游戏对象列表（通常嵌套在角色下），用于搜索其他能力")]
        public List<GameObject> AdditionalAbilityNodes;

        [MMInformation("您还可以决定角色是否必须在后退时自动翻转。此外，如果不使用精灵，可以在此处定义角色模型的局部比例将如何受到翻转的影响。默认情况下，它在x轴上翻转，但您可以更改它以适合您的模型。", MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]

		/// whether we should flip the model's scale when the character changes direction or not
		[Tooltip("当角色改变方向时，我们是否应该翻转模型的比例")]
		public bool FlipModelOnDirectionChange = true;
		/// the FlipValue will be used to multiply the model's transform's localscale on flip. Usually it's -1,1,1, but feel free to change it to suit your model's specs
		[MMCondition("FlipModelOnDirectionChange", true)]
		[Tooltip("翻转值将用于在翻转时乘以模型变换的局部比例。通常为-1,1,1，但可以随意更改以适合您的模型规格")]
		public Vector3 ModelFlipValue = new Vector3(-1,1,1);
		/// whether we should rotate the model on direction change or not
		[Tooltip("我们是否应该根据方向变化旋转模型")]
		public bool RotateModelOnDirectionChange;
		/// the rotation to apply to the model when it changes direction
		[MMCondition("RotateModelOnDirectionChange", true)]
		[Tooltip("当模型改变方向时应用于模型的旋转")]
		public Vector3 ModelRotationValue = new Vector3(0f,180f,0f);
		/// the speed at which to rotate the model when changing direction, 0f means instant rotation
		[MMCondition("RotateModelOnDirectionChange", true)]
		[Tooltip("改变方向时旋转模型的速度，0f表示瞬间旋转")]
		public float ModelRotationSpeed = 0f;

        [Header("Events")]
		[MMInformation("在这里，您可以定义在更改状态时是否希望该角色触发事件。有关更多信息，请参阅MMTools的状态机文档。", MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]

		/// If this is true, the Character's state machine will emit events when entering/exiting a state
		[Tooltip("如果这是真的，角色的状态机将在进入/退出状态时发出事件")]
		public bool SendStateChangeEvents = true;
		/// If this is true, a state machine processor component will be added and it'll emit events on updates (see state machine processor's doc for more details)
		[Tooltip("如果这是真的，则将添加一个状态机处理器组件，并在更新时发出事件（有关更多详细信息，请参阅状态机处理器的文档）")]
		public bool SendStateUpdateEvents = true;

        [Header("Airborne")]

		/// The distance after which the character is considered airborne
		[Tooltip("角色被视为悬空的距离")]
		public float AirborneDistance = 0.5f;
        /// Whether or not the character is airborne this frame
        public bool Airborne { get { return ((_controller.DistanceToTheGround > AirborneDistance) || (_controller.DistanceToTheGround == -1)); } }

        [Header("AI")]
        /// The brain currently associated with this character, if it's an Advanced AI. By default the engine will pick the one on this object, but you can attach another one if you'd like
        [Tooltip("如果是高级人工智能，那么当前与这个角色相关的大脑。默认情况下，引擎将拾取此对象上的一个，但如果愿意，可以附加另一个")]
        public AIBrain CharacterBrain;

        // State Machines
        /// the movement state machine 
        public MMStateMachine<CharacterStates.MovementStates> MovementState;
        /// the condition state machine
		public MMStateMachine<CharacterStates.CharacterConditions> ConditionState;

		// associated camera 
	    public CameraController SceneCamera { get; protected set; }
        /// associated input manager
		public InputManager LinkedInputManager { get; protected set; }
        /// associated animator
	    public Animator _animator { get; protected set; }
        /// a list of animator parameters to update
		public HashSet<int> _animatorParameters { get; set; }
        /// whether or not the character can flip this frame
        public bool CanFlip { get; set; }
        /// the Health script associated to this Character
        public Health _health { get; protected set; }

        // animation parameters
        protected const string _groundedAnimationParameterName = "Grounded";
		protected const string _airborneAnimationParameterName = "Airborne";
		protected const string _xSpeedAnimationParameterName = "xSpeed";
		protected const string _ySpeedAnimationParameterName = "ySpeed";
		protected const string _worldXSpeedAnimationParameterName = "WorldXSpeed";
		protected const string _worldYSpeedAnimationParameterName = "WorldYSpeed";
		protected const string _collidingLeftAnimationParameterName = "CollidingLeft";
		protected const string _collidingRightAnimationParameterName = "CollidingRight";
		protected const string _collidingBelowAnimationParameterName = "CollidingBelow";
		protected const string _collidingAboveAnimationParameterName = "CollidingAbove";
		protected const string _idleSpeedAnimationParameterName = "Idle";
		protected const string _aliveAnimationParameterName = "Alive";
		protected const string _facingRightAnimationParameterName = "FacingRight";
        protected const string _randomAnimationParameterName = "Random";
        protected const string _randomConstantAnimationParameterName = "RandomConstant";

        protected int _groundedAnimationParameter;
		protected int _airborneSpeedAnimationParameter;
		protected int _xSpeedSpeedAnimationParameter;
		protected int _ySpeedSpeedAnimationParameter;
		protected int _worldXSpeedSpeedAnimationParameter;
		protected int _worldYSpeedSpeedAnimationParameter;
		protected int _collidingLeftAnimationParameter;
		protected int _collidingRightAnimationParameter;
		protected int _collidingBelowAnimationParameter;
		protected int _collidingAboveAnimationParameter;
		protected int _idleSpeedAnimationParameter;
		protected int _aliveAnimationParameter;
		protected int _facingRightAnimationParameter;
        protected int _randomAnimationParameter;
        protected int _randomConstantAnimationParameter;

        protected CorgiController _controller;
		protected SpriteRenderer _spriteRenderer;
	    protected Color _initialColor;
	    protected CharacterAbility[] _characterAbilities; 
	    protected float _originalGravity;		
		protected bool _spawnDirectionForced = false;
        protected Vector3 _targetModelRotation;
        protected DamageOnTouch _damageOnTouch;
        protected Vector3 _cameraTargetInitialPosition;
        protected Vector3 _cameraOffset = Vector3.zero;
        protected bool _abilitiesCachedOnce = false;
        protected float _animatorRandomNumber;
        protected CharacterPersistence _characterPersistence;

        /// <summary>
        /// Initializes this instance of the character
        /// </summary>
        protected virtual void Awake()
		{		
			Initialization();
		}

		/// <summary>
		/// Gets and stores input manager, camera and components
		/// </summary>
		public virtual void Initialization()
		{
			// we initialize our state machines
			MovementState = new MMStateMachine<CharacterStates.MovementStates>(this.gameObject, SendStateChangeEvents);
			ConditionState = new MMStateMachine<CharacterStates.CharacterConditions>(this.gameObject, SendStateChangeEvents);

			if (InitialFacingDirection == FacingDirections.Left)
			{
				IsFacingRight = false;
			}
			else
			{
				IsFacingRight = true;
			}

            // instantiate camera target
            if (CameraTarget == null)
            {
                CameraTarget = new GameObject();
                CameraTarget.transform.SetParent(this.transform);
                CameraTarget.transform.localPosition = Vector3.zero;
                CameraTarget.name = "CameraTarget";
            }
            _cameraTargetInitialPosition = CameraTarget.transform.localPosition;

            // we get the current input manager
            SetInputManager();
            GetMainCamera();
			// we store our components for further use 
			CharacterState = new CharacterStates();
			_spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
			_controller = this.gameObject.GetComponent<CorgiController>();
			_characterPersistence = this.gameObject.GetComponent<CharacterPersistence>();
            CacheAbilitiesAtInit();
            if (CharacterBrain == null)
            {
                CharacterBrain = this.gameObject.GetComponent<AIBrain>(); 
            }			
			_health = this.gameObject.GetComponent<Health> ();
            _damageOnTouch = this.gameObject.GetComponent<DamageOnTouch>();
            CanFlip = true;
            AssignAnimator();           

			_originalGravity = _controller.Parameters.Gravity;		

			ForceSpawnDirection ();
		}

		/// <summary>
		/// Gets the main camera and stores it
		/// </summary>
		public virtual void GetMainCamera()
		{
			if (Camera.main != null)
			{
				SceneCamera = Camera.main.GetComponent<CameraController>();
			}		
		}

		/// <summary>
		/// Caches abilities if not already cached
		/// </summary>
        protected virtual void CacheAbilitiesAtInit()
        {
            if (_abilitiesCachedOnce)
            {
                return;
            }
            CacheAbilities();
        }

		/// <summary>
        /// Grabs abilities and caches them for further use
        /// Make sure you call this if you add abilities at runtime
        /// Ideally you'll want to avoid adding components at runtime, it's costly,
        /// and it's best to activate/disable components instead.
        /// But if you need to, call this method.
        /// </summary>
		public virtual void CacheAbilities()
        {
            // we grab all abilities at our level
			_characterAbilities = this.gameObject.GetComponents<CharacterAbility>();

            // if the user has specified more nodes
            if ((AdditionalAbilityNodes != null) && (AdditionalAbilityNodes.Count > 0))
            {
                // we create a temp list
                List<CharacterAbility> tempAbilityList = new List<CharacterAbility>();

                // we put all the abilities we've already found on the list
                for (int i = 0; i < _characterAbilities.Length; i++)
                {
                    tempAbilityList.Add(_characterAbilities[i]);
                }

                // we add the ones from the nodes
                for (int j = 0; j < AdditionalAbilityNodes.Count; j++)
                {
                    CharacterAbility[] tempArray = AdditionalAbilityNodes[j].GetComponentsInChildren<CharacterAbility>();
                    foreach(CharacterAbility ability in tempArray)
                    {
                        tempAbilityList.Add(ability);
                    }
                }

                _characterAbilities = tempAbilityList.ToArray();
            }
            _abilitiesCachedOnce = true;
        }

		/// <summary>
		/// A method to check whether a Character has a certain ability or not
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
        public T FindAbility<T>() where T:CharacterAbility
        {
            CacheAbilitiesAtInit();

            Type searchedAbilityType = typeof(T);
            
            foreach (CharacterAbility ability in _characterAbilities)
            {
                if (ability is T characterAbility)
                {
                    return characterAbility;
                }
            }

            return null;
        }

        /// <summary>
        /// Binds an animator to this character
        /// </summary>
        public virtual void AssignAnimator()
        {
            if (CharacterAnimator != null)
            {
                _animator = CharacterAnimator;
            }
            else
            {
                _animator = this.gameObject.GetComponent<Animator>();
            }

            if (_animator != null)
            {
                InitializeAnimatorParameters();
            }
        }
        
		/// <summary>
		/// Gets (if it exists) the InputManager matching the Character's Player ID
		/// </summary>
		public virtual void SetInputManager()
		{
			if (CharacterType == CharacterTypes.AI)
			{
                LinkedInputManager = null;
                UpdateInputManagersInAbilities();
				return;
			}

			// we get the corresponding input manager
			if (!string.IsNullOrEmpty (PlayerID))
			{
				LinkedInputManager = null;
				InputManager[] foundInputManagers = FindObjectsOfType(typeof(InputManager)) as InputManager[];
				foreach (InputManager foundInputManager in foundInputManagers) 
		        {
					if (foundInputManager.PlayerID == PlayerID)
					{
						LinkedInputManager = foundInputManager;
					}
		        }
			}
            UpdateInputManagersInAbilities();
        }

        /// <summary>
        /// Sets a new input manager for this Character and all its abilities
        /// </summary>
        /// <param name="inputManager"></param>
        public virtual void SetInputManager(InputManager inputManager)
        {
            LinkedInputManager = inputManager;
            UpdateInputManagersInAbilities();
        }

        /// <summary>
        /// Updates the linked input manager for all abilities
        /// </summary>
        protected virtual void UpdateInputManagersInAbilities()
        {
            if (_characterAbilities == null)
            {
                return;
            }
            for (int i=0; i<_characterAbilities.Length; i++)
            {
                _characterAbilities[i].SetInputManager(LinkedInputManager);
            }            
        }
                
        /// <summary>
        /// Resets the input for all abilities
        /// </summary>
        public virtual void ResetInput()
        {
            if (_characterAbilities == null)
            {
                return;
            }
            foreach (CharacterAbility ability in _characterAbilities)
            {
                ability.ResetInput();
            }
        }

        /// <summary>
        /// Sets the player ID
        /// </summary>
        /// <param name="newPlayerID">New player ID.</param>
        public virtual void SetPlayerID(string newPlayerID)
		{
			PlayerID = newPlayerID;
			SetInputManager();
		}
		
		/// <summary>
		/// This is called every frame.
		/// </summary>
		protected virtual void Update()
		{		
			EveryFrame();				
		}

		/// <summary>
		/// We do this every frame. This is separate from Update for more flexibility.
		/// </summary>
		protected virtual void EveryFrame()
		{
			HandleCharacterStatus();

            // we process our abilities
            EarlyProcessAbilities();

            if (Time.timeScale != 0f)
            {
                ProcessAbilities();
                LateProcessAbilities();
                HandleCameraTarget();
            }

			// we send our various states to the animator.		 
			UpdateAnimators ();
            RotateModel();	
		}

        protected virtual void RotateModel()
        {
            if (!RotateModelOnDirectionChange)
            {
                return;
            }
            
            if (ModelRotationSpeed > 0f)
            {
                CharacterModel.transform.localEulerAngles = Vector3.Lerp(CharacterModel.transform.localEulerAngles, _targetModelRotation, Time.deltaTime * ModelRotationSpeed);
            }
            else
            {
                CharacterModel.transform.localEulerAngles = _targetModelRotation;
            }
        }

		/// <summary>
		/// Calls all registered abilities' Early Process methods
		/// </summary>
		protected virtual void EarlyProcessAbilities()
        {
            foreach (CharacterAbility ability in _characterAbilities)
			{
				if (ability.enabled && ability.AbilityInitialized)
                {
                    ability.EarlyProcessAbility();
				}
			}
		}

		/// <summary>
		/// Calls all registered abilities' Process methods
		/// </summary>
		protected virtual void ProcessAbilities()
		{
			foreach (CharacterAbility ability in _characterAbilities)
			{
				if (ability.enabled && ability.AbilityInitialized)
				{
					ability.ProcessAbility();
				}
			}
		}

		/// <summary>
		/// Calls all registered abilities' Late Process methods
		/// </summary>
		protected virtual void LateProcessAbilities()
		{
			foreach (CharacterAbility ability in _characterAbilities)
			{
				if (ability.enabled && ability.AbilityInitialized)
				{
					ability.LateProcessAbility();
				}
			}
		}

        /// <summary>
        /// Initializes the animator parameters.
        /// </summary>
        protected virtual void InitializeAnimatorParameters()
        {
            if (_animator == null) { return; }

            _animatorParameters = new HashSet<int>();

            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _groundedAnimationParameterName, out _groundedAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _airborneAnimationParameterName, out _airborneSpeedAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _xSpeedAnimationParameterName, out _xSpeedSpeedAnimationParameter, AnimatorControllerParameterType.Float, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _ySpeedAnimationParameterName, out _ySpeedSpeedAnimationParameter, AnimatorControllerParameterType.Float, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _worldXSpeedAnimationParameterName, out _worldXSpeedSpeedAnimationParameter, AnimatorControllerParameterType.Float, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _worldYSpeedAnimationParameterName, out _worldYSpeedSpeedAnimationParameter, AnimatorControllerParameterType.Float, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _collidingLeftAnimationParameterName, out _collidingLeftAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _collidingRightAnimationParameterName, out _collidingRightAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _collidingBelowAnimationParameterName, out _collidingBelowAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _collidingAboveAnimationParameterName, out _collidingAboveAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _idleSpeedAnimationParameterName, out _idleSpeedAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _aliveAnimationParameterName, out _aliveAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _facingRightAnimationParameterName, out _facingRightAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _randomAnimationParameterName, out _randomAnimationParameter, AnimatorControllerParameterType.Float, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _randomConstantAnimationParameterName, out _randomConstantAnimationParameter, AnimatorControllerParameterType.Int, _animatorParameters);

            // we update our constant float animation parameter
            int randomConstant = UnityEngine.Random.Range(0, 1000);
            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _randomConstantAnimationParameter, randomConstant, _animatorParameters);
        }

        /// <summary>
        /// This is called at Update() and sets each of the animators parameters to their corresponding State values
        /// </summary>
        protected virtual void UpdateAnimators()
        {
            if ((UseDefaultMecanim) && (_animator!= null))
            {
                MMAnimatorExtensions.UpdateAnimatorBool(_animator, _groundedAnimationParameter, _controller.State.IsGrounded, _animatorParameters, PerformAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorBool(_animator, _airborneSpeedAnimationParameter, Airborne, _animatorParameters, PerformAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorBool(_animator, _aliveAnimationParameter, (ConditionState.CurrentState != CharacterStates.CharacterConditions.Dead),_animatorParameters, PerformAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _xSpeedSpeedAnimationParameter, _controller.Speed.x, _animatorParameters, PerformAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _ySpeedSpeedAnimationParameter, _controller.Speed.y, _animatorParameters, PerformAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _worldXSpeedSpeedAnimationParameter, _controller.WorldSpeed.x, _animatorParameters, PerformAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _worldYSpeedSpeedAnimationParameter, _controller.WorldSpeed.y, _animatorParameters, PerformAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorBool(_animator, _collidingLeftAnimationParameter, _controller.State.IsCollidingLeft, _animatorParameters, PerformAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorBool(_animator, _collidingRightAnimationParameter, _controller.State.IsCollidingRight, _animatorParameters, PerformAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorBool(_animator, _collidingBelowAnimationParameter, _controller.State.IsCollidingBelow, _animatorParameters, PerformAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorBool(_animator, _collidingAboveAnimationParameter, _controller.State.IsCollidingAbove, _animatorParameters, PerformAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorBool(_animator, _idleSpeedAnimationParameter, (MovementState.CurrentState == CharacterStates.MovementStates.Idle), _animatorParameters, PerformAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorBool(_animator, _facingRightAnimationParameter, IsFacingRight, _animatorParameters);

                UpdateAnimationRandomNumber();
                MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _randomAnimationParameter, _animatorRandomNumber, _animatorParameters, PerformAnimatorSanityChecks);

                foreach (CharacterAbility ability in _characterAbilities)
				{
					if (ability.enabled && ability.AbilityInitialized)
					{	
						ability.UpdateAnimator();
					}
				}
	        }
	    }

        /// <summary>
        /// Generates a random number to send to the animator
        /// </summary>
        protected virtual void UpdateAnimationRandomNumber()
        {
            _animatorRandomNumber = UnityEngine.Random.Range(0f, 1f);
        }

        /// <summary>
        /// Handles the character status.
        /// </summary>
        protected virtual void HandleCharacterStatus()
		{
			// if the character is dead, we prevent it from moving horizontally		
			if (ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
			{
                if (_health != null)
                {
                    if (_health.GravityOffOnDeath)
                    {
                        _controller.GravityActive(false);
                    }
                    if (_health.ApplyDeathForce && (_health.DeathForce.x == 0f))
                    {
                        _controller.SetHorizontalForce(0);
                        return;
                    }
                }
                else
                {
                    _controller.SetHorizontalForce(0);
                    return;
                }
			}

			// if the character is frozen, we prevent it from moving
			if (ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen)
			{
				_controller.GravityActive(false);
				_controller.SetForce(Vector2.zero);			
			}
		}

		/// <summary>
		/// Freezes this character.
		/// </summary>
		public virtual void Freeze()
		{
			_controller.GravityActive(false);
			_controller.SetForce(Vector2.zero);	
			ConditionState.ChangeState(CharacterStates.CharacterConditions.Frozen);
		}

		/// <summary>
		/// Unfreezes this character
		/// </summary>
		public virtual void UnFreeze()
		{
			_controller.GravityActive(true);
			ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
		}	    
		
		/// <summary>
		/// Use this method to force the controller to recalculate the rays, especially useful when the size of the character has changed.
		/// </summary>
		public virtual void RecalculateRays()
		{
			_controller.SetRaysParameters();
		}

		/// <summary>
		/// Called to disable the player (at the end of a level for example. 
		/// It won't move and respond to input after this.
		/// </summary>
		public virtual void Disable()
		{
			enabled = false;
			_controller.enabled = false;
			this.gameObject.MMGetComponentNoAlloc<Collider2D>().enabled = false;		
		}
		
		/// <summary>
		/// Makes the player respawn at the location passed in parameters
		/// </summary>
		/// <param name="spawnPoint">The location of the respawn.</param>
		public virtual void RespawnAt(Transform spawnPoint, FacingDirections facingDirection)
		{
			if (!gameObject.activeInHierarchy)
			{
				//Debug.LogError("Spawn : your Character's gameobject is inactive");
				return;
			}

            UnFreeze();

			// we make sure the character is facing right
			Face(facingDirection);

			// we raise it from the dead (if it was dead)
			ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
			// we re-enable its 2D collider
			this.gameObject.MMGetComponentNoAlloc<Collider2D>().enabled = true;
			// we make it handle collisions again
			_controller.CollisionsOn();
			
			
			transform.position = spawnPoint.position;
			
			if (_health != null)
			{
				if (_characterPersistence != null)
				{
					if (_characterPersistence.Initialized)
					{
						if (_health != null)
						{
							_health.UpdateHealthBar(false);
						}
						return;
					}
				}
				
				_health.ResetHealthToMaxHealth();
				_health.Revive ();
			}
		}
		
		/// <summary>
		/// Flips the character and its dependencies (jetpack for example) horizontally
		/// </summary>
		public virtual void Flip(bool IgnoreFlipOnDirectionChange = false)
		{
			// if we don't want the character to flip, we do nothing and exit
			if (!FlipModelOnDirectionChange && !RotateModelOnDirectionChange && !IgnoreFlipOnDirectionChange)
            {
				return;
			}

            if (!CanFlip)
            {
                return;
            }

            if (!FlipModelOnDirectionChange && !RotateModelOnDirectionChange && IgnoreFlipOnDirectionChange)
            {
                if (CharacterModel != null)
                {
                    CharacterModel.transform.localScale = Vector3.Scale(CharacterModel.transform.localScale, ModelFlipValue);
                }
                else
                {
                    // if we're sprite renderer based, we revert the flipX attribute
                    if (_spriteRenderer != null)
                    {
                        _spriteRenderer.flipX = !_spriteRenderer.flipX;
                    }
                }
            }

            // Flips the character horizontally
            FlipModel();

			IsFacingRight = !IsFacingRight;

			// we tell all our abilities we should flip
			foreach (CharacterAbility ability in _characterAbilities)
			{
				if (ability.enabled)
				{
					ability.Flip();
				}
			}
		}

        /// <summary>
        /// Flips the model only, no impact on weapons or attachments
        /// </summary>
        public virtual void FlipModel()
        {
            if (FlipModelOnDirectionChange)
            {
                if (CharacterModel != null)
                {
                    CharacterModel.transform.localScale = Vector3.Scale(CharacterModel.transform.localScale, ModelFlipValue);
                }
                else
                {
                    // if we're sprite renderer based, we revert the flipX attribute
                    if (_spriteRenderer != null)
                    {
                        _spriteRenderer.flipX = !_spriteRenderer.flipX;
                    }
                }
            }
            if (RotateModelOnDirectionChange)
            {
                if (CharacterModel != null)
                {
                    _targetModelRotation += ModelRotationValue;
                    _targetModelRotation.x = _targetModelRotation.x % 360;
                    _targetModelRotation.y = _targetModelRotation.y % 360;
                    _targetModelRotation.z = _targetModelRotation.z % 360;
                }
            }
        }

		/// <summary>
		/// Forces the character to face left or right on spawn (and respawn)
		/// </summary>
		protected virtual void ForceSpawnDirection()
		{
			if ((DirectionOnSpawn == SpawnFacingDirections.Default) || _spawnDirectionForced)
			{
				return;
			}
			else
			{
				_spawnDirectionForced = true;
				if (DirectionOnSpawn == SpawnFacingDirections.Left)
				{
					Face (FacingDirections.Left);
				}
				if (DirectionOnSpawn == SpawnFacingDirections.Right)
				{
					Face (FacingDirections.Right);					
				}
			}
		}

		/// <summary>
		/// Forces the character to face right or left
		/// </summary>
		/// <param name="facingDirection">Facing direction.</param>
		public virtual void Face(FacingDirections facingDirection)
		{
            if (!CanFlip)
            {
                return;
            }

			// Flips the character horizontally
			if (facingDirection == FacingDirections.Right)
			{
				if (!IsFacingRight)
				{
					Flip(true);
				}
			}
			else
			{
				if (IsFacingRight)
				{
					Flip(true);
				}
			}
		}

        /// <summary>
        /// Called every frame, makes the camera target move
        /// </summary>
        protected virtual void HandleCameraTarget()
        {
            CameraTarget.transform.localPosition = Vector3.Lerp(CameraTarget.transform.localPosition, _cameraTargetInitialPosition + _cameraOffset, Time.deltaTime * CameraTargetSpeed);
        }

        /// <summary>
        /// Sets a new offset for the camera target
        /// </summary>
        /// <param name="offset"></param>
        public virtual void SetCameraTargetOffset(Vector3 offset)
        {
            _cameraOffset = offset;
        }

		/// <summary>
		/// Called when the Character dies. 
		/// Calls every abilities' Reset() method, so you can restore settings to their original value if needed
		/// </summary>
		public virtual void Reset()
		{
			_spawnDirectionForced = false;
			if (_characterAbilities == null)
			{
				return;
			}
			if (_characterAbilities.Length == 0)
			{
				return;
			}
			foreach (CharacterAbility ability in _characterAbilities)
			{
				if (ability.enabled)
				{
					ability.ResetAbility();
				}
			}
		}

		/// <summary>
		/// On revive, we force the spawn direction
		/// </summary>
		protected virtual void OnRevive()
		{
			ForceSpawnDirection ();
            if (CharacterBrain != null)
            {
                CharacterBrain.enabled = true;
            }
            if (_damageOnTouch != null)
            {
                _damageOnTouch.enabled = true;
            }
        }

        /// <summary>
        /// On character death, disables the brain and any damage on touch area 
        /// </summary>
        protected virtual void OnDeath()
        {
            if (CharacterBrain != null)
            {
                CharacterBrain.TransitionToState("");
                CharacterBrain.enabled = false;
            }
            if (_damageOnTouch != null)
            {
                _damageOnTouch.enabled = false;
            }
        }

		/// <summary>
		/// OnEnable, we register our OnRevive event
		/// </summary>
		protected virtual void OnEnable ()
		{
			if (_health != null)
			{
				_health.OnRevive += OnRevive;
                _health.OnDeath += OnDeath;
			}
		}

		/// <summary>
		/// OnDisable, we unregister our OnRevive event
		/// </summary>
		protected virtual void OnDisable()
		{
			if (_health != null)
			{
				//_health.OnRevive -= OnRevive;
                _health.OnDeath -= OnDeath;
            }			
		}
	}
}