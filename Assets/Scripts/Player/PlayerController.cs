using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// Manages a first person character
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(AudioSource))]
public partial class PlayerController : MonoBehaviour
{
#pragma warning disable 649
	[Header("Arms")]
    [Tooltip("The transform component that holds the gun camera."), SerializeField]
    private Transform arms;

    [Tooltip("The position of the arms and gun camera relative to the fps controller GameObject."), SerializeField]
    private Vector3 armPosition;

	[Header("Audio Clips")]
    [Tooltip("The audio clip that is played while walking."), SerializeField]
    private AudioClip walkingSound;

    [Tooltip("The audio clip that is played while running."), SerializeField]
    private AudioClip runningSound;

	[Header("Movement Settings")]
    [Tooltip("How fast the player moves while walking and strafing."), SerializeField]
    private float walkingSpeed = 5f;

    [Tooltip("How fast the player moves while running."), SerializeField]
    private float runningSpeed = 9f;

    [Tooltip("Approximately the amount of time it will take for the player to reach maximum running or walking speed."), SerializeField]
    private float movementSmoothness = 0.125f;

    [Tooltip("Amount of force applied to the player when jumping."), SerializeField]
    private float jumpForce = 35f;

    [Tooltip("Approximately the amount of time it will take for the fps controller to reach maximum rotation speed."), SerializeField]
    private float rotationSmoothness = 0.05f;

    [Tooltip("Minimum rotation of the arms and camera on the x axis."),
        SerializeField]
    private float minVerticalAngle = -90f;

    [Tooltip("Maximum rotation of the arms and camera on the axis."),
        SerializeField]
    private float maxVerticalAngle = 90f;

    [Tooltip("The names of the axes and buttons for Unity's Input Manager."), SerializeField]
    private FpsInput input;

    [Header("Score")]
    public float totalScore;


    [Header("Health System")]
    public float currentHealth;
    public float maxHealth = 100.0f;
    public Image healthBar;
    public Canvas gameOverCanvas;


#pragma warning restore 649

    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;
    private AudioSource _audioSource;
    private SmoothRotation _rotationX;
    private SmoothRotation _rotationY;
    private SmoothVelocity _velocityX;
    private SmoothVelocity _velocityZ;
    private bool _isGrounded;

    private readonly RaycastHit[] _groundCastResults = new RaycastHit[8];
    private readonly RaycastHit[] _wallCastResults = new RaycastHit[8];


    private void Awake()
    {
        currentHealth = maxHealth;
        healthBar.fillAmount = 0.5f;
        InputManager.Instance.KeyPressed += OnKeyPressed;
    }

    /// Initializes the FpsController on start.
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _collider = GetComponent<CapsuleCollider>();
        _audioSource = GetComponent<AudioSource>();
		arms = AssignCharactersCamera();
        _audioSource.clip = walkingSound;
        _audioSource.loop = true;
        _rotationX = new SmoothRotation(RotationXRaw);
        _rotationY = new SmoothRotation(RotationYRaw);
        _velocityX = new SmoothVelocity();
        _velocityZ = new SmoothVelocity();
        Cursor.lockState = CursorLockMode.Locked;
        ValidateRotationRestriction();
        totalScore = 0;
    }
			
    private Transform AssignCharactersCamera()
    {
        var t = transform;
		arms.SetPositionAndRotation(t.position, t.rotation);
		return arms;
    }
        
    /// Clamps <see cref="minVerticalAngle"/> and <see cref="maxVerticalAngle"/> to valid values and
    /// ensures that <see cref="minVerticalAngle"/> is less than <see cref="maxVerticalAngle"/>.
    private void ValidateRotationRestriction()
    {
        minVerticalAngle = ClampRotationRestriction(minVerticalAngle, -90, 90);
        maxVerticalAngle = ClampRotationRestriction(maxVerticalAngle, -90, 90);
        if (maxVerticalAngle >= minVerticalAngle) return;
        Debug.LogWarning("maxVerticalAngle should be greater than minVerticalAngle.");
        var min = minVerticalAngle;
        minVerticalAngle = maxVerticalAngle;
        maxVerticalAngle = min;
    }

    private static float ClampRotationRestriction(float rotationRestriction, float min, float max)
    {
        if (rotationRestriction >= min && rotationRestriction <= max) return rotationRestriction;
        var message = string.Format("Rotation restrictions should be between {0} and {1} degrees.", min, max);
        Debug.LogWarning(message);
        return Mathf.Clamp(rotationRestriction, min, max);
    }
			
    /// Checks if the character is on the ground.
    private void OnCollisionStay()
    {
        var bounds = _collider.bounds;
        var extents = bounds.extents;
        var radius = extents.x - 0.01f;
        Physics.SphereCastNonAlloc(bounds.center, radius, Vector3.down,
            _groundCastResults, extents.y - radius * 0.5f, ~0, QueryTriggerInteraction.Ignore);
        if (!_groundCastResults.Any(hit => hit.collider != null && hit.collider != _collider)) return;
        for (var i = 0; i < _groundCastResults.Length; i++)
        {
            _groundCastResults[i] = new RaycastHit();
        }

        _isGrounded = true;
    }
			
    /// Processes the character movement and the camera rotation every fixed framerate frame.
    private void FixedUpdate()
    {
        // FixedUpdate is used instead of Update because this code is dealing with physics and smoothing.
        RotateCameraAndCharacter();
        MoveCharacter();
        _isGrounded = false;
    }
			
    /// Moves the camera to the character, processes jumping and plays sounds every frame.
    private void Update()
    {
		arms.position = transform.position + transform.TransformVector(armPosition);
        PlayFootstepSounds();

        if (currentHealth < 0.1f && gameOverCanvas)
        {
            gameOverCanvas.gameObject.SetActive(true);
            Time.timeScale = 0.0f;
        }
    }

    private void OnKeyPressed(KeyCode keyCode)
    {
        //Debug.Log($"Pressed {keyCode}");
        if (keyCode == KeyCode.Space)
        {
            Jump();
        }
        else if (keyCode == KeyCode.X)
        {
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(arms.transform.position, arms.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                Debug.Log($"Interacting with {hit.transform.name} at a distance of {hit.distance}");
                if(hit.transform.GetComponent<Interactable>() != null)
                {
                    if(hit.distance <= hit.transform.GetComponent<Interactable>().Radius)
                    {
                        hit.transform.GetComponent<Interactable>().Interact();
                    }
                }
            }
        }
        else if(keyCode == KeyCode.Escape)
        {
            Debug.Log("Opening menu");
            GameObject menu = GameObject.FindGameObjectWithTag("Menu");
            if(menu != null)
            {
                menu.GetComponent<GameMenu>().EnableMenu();
            }
        }
    }

    private void OnDrawGizmos()
    {

        RaycastHit hit;
        Physics.Raycast(arms.transform.position, arms.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity);
        Debug.DrawRay(arms.transform.position, arms.transform.TransformDirection(Vector3.forward) * Mathf.Infinity, Color.red);
        //Debug.Log($"Interacting with {hit.transform.name}");
    }

    private void RotateCameraAndCharacter()
    {
        var rotationX = _rotationX.Update(RotationXRaw, rotationSmoothness);
        var rotationY = _rotationY.Update(RotationYRaw, rotationSmoothness);
        var clampedY = RestrictVerticalRotation(rotationY);
        _rotationY.Current = clampedY;
		var worldUp = arms.InverseTransformDirection(Vector3.up);
		var rotation = arms.rotation *
                        Quaternion.AngleAxis(rotationX, worldUp) *
                        Quaternion.AngleAxis(clampedY, Vector3.left);
        transform.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
		arms.rotation = rotation;
    }
			
    /// Returns the target rotation of the camera around the y axis with no smoothing.
    private float RotationXRaw
    {
        get { return input.RotateX * Settings.MouseXSensitivity; }
    }
			
    /// Returns the target rotation of the camera around the x axis with no smoothing.
    private float RotationYRaw
    {
        get { return input.RotateY * Settings.MouseYSensitivity; }
    }
			
    /// Clamps the rotation of the camera around the x axis
    /// between the <see cref="minVerticalAngle"/> and <see cref="maxVerticalAngle"/> values.
    private float RestrictVerticalRotation(float mouseY)
    {
		var currentAngle = NormalizeAngle(arms.eulerAngles.x);
        var minY = minVerticalAngle + currentAngle;
        var maxY = maxVerticalAngle + currentAngle;
        return Mathf.Clamp(mouseY, minY + 0.01f, maxY - 0.01f);
    }
			
    /// Normalize an angle between -180 and 180 degrees.
    /// <param name="angleDegrees">angle to normalize</param>
    /// <returns>normalized angle</returns>
    private static float NormalizeAngle(float angleDegrees)
    {
        while (angleDegrees > 180f)
        {
            angleDegrees -= 360f;
        }

        while (angleDegrees <= -180f)
        {
            angleDegrees += 360f;
        }

        return angleDegrees;
    }

    private void MoveCharacter()
    {
        var direction = new Vector3(input.Move, 0f, input.Strafe).normalized;
        var worldDirection = transform.TransformDirection(direction);
        var velocity = worldDirection * (input.Run ? runningSpeed : walkingSpeed);
        //Checks for collisions so that the character does not stuck when jumping against walls.
        var intersectsWall = CheckCollisionsWithWalls(velocity);
        if (intersectsWall)
        {
            _velocityX.Current = _velocityZ.Current = 0f;
            return;
        }

        var smoothX = _velocityX.Update(velocity.x, movementSmoothness);
        var smoothZ = _velocityZ.Update(velocity.z, movementSmoothness);
        var rigidbodyVelocity = _rigidbody.velocity;
        var force = new Vector3(smoothX - rigidbodyVelocity.x, 0f, smoothZ - rigidbodyVelocity.z);
        _rigidbody.AddForce(force, ForceMode.VelocityChange);
    }

    private bool CheckCollisionsWithWalls(Vector3 velocity)
    {
        if (_isGrounded) return false;
        var bounds = _collider.bounds;
        var radius = _collider.radius;
        var halfHeight = _collider.height * 0.5f - radius * 1.0f;
        var point1 = bounds.center;
        point1.y += halfHeight;
        var point2 = bounds.center;
        point2.y -= halfHeight;
        Physics.CapsuleCastNonAlloc(point1, point2, radius, velocity.normalized, _wallCastResults,
            radius * 0.04f, ~0, QueryTriggerInteraction.Ignore);
        var collides = _wallCastResults.Any(hit => hit.collider != null && hit.collider != _collider);
        if (!collides) return false;
        for (var i = 0; i < _wallCastResults.Length; i++)
        {
            _wallCastResults[i] = new RaycastHit();
        }

        return true;
    }

    private void Jump()
    {
        if (!_isGrounded || !input.Jump) return;
        _isGrounded = false;
        _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void PlayFootstepSounds()
    {
        if (_isGrounded && _rigidbody.velocity.sqrMagnitude > 0.1f)
        {
            _audioSource.clip = input.Run ? runningSound : walkingSound;
            if (!_audioSource.isPlaying)
            {
                _audioSource.Play();
            }
        }
        else
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Pause();
            }
        }
    }

    public void Hit()
    {
        StartCoroutine(hittedColor());
    }

    private IEnumerator hittedColor()
    {
        healthBar.color = new Color(255, 255, 255, 200);
        yield return new WaitForSeconds(0.25f);
        healthBar.color = new Color(0, 0, 0, 200);
    }

    public void AddHealth(int hp)
    {
        currentHealth += hp;
        healthBar.fillAmount += hp * 0.01f / 2.0f;
    }

    /// A helper for assistance with smoothing the camera rotation.
    private class SmoothRotation
    {
        private float _current;
        private float _currentVelocity;

        public SmoothRotation(float startAngle)
        {
            _current = startAngle;
        }
				
        /// Returns the smoothed rotation.
        public float Update(float target, float smoothTime)
        {
            return _current = Mathf.SmoothDampAngle(_current, target, ref _currentVelocity, smoothTime);
        }

        public float Current
        {
            set { _current = value; }
        }
    }
			
    /// A helper for assistance with smoothing the movement.
    private class SmoothVelocity
    {
        private float _current;
        private float _currentVelocity;

        /// Returns the smoothed velocity.
        public float Update(float target, float smoothTime)
        {
            return _current = Mathf.SmoothDamp(_current, target, ref _currentVelocity, smoothTime);
        }

        public float Current
        {
            set { _current = value; }
        }
    }
}