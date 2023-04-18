using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.TouchPhase;
using UnityEngine.Events;

public enum Road {
    Left = 0,
    Middle = 1,
    Right = 2
}

public class PlayerCar : MonoBehaviour
{
    [SerializeField] private float _minimalSwipeSpeed;

    [SerializeField] private PlayerHitbox[] _hitboxes = new PlayerHitbox[3];
    
    private Vector2 _startSwipe;
    private Vector2 _endSwipe;

    private Camera _camera;
    private int _currentPath;
    private int _previousPath;

    private MainControls _mainControls;

    private TweenerCore<Vector3, Vector3, VectorOptions> _tweener;
    private bool CanMove => _tweener == null || _tweener.IsComplete();    

    private void Awake()
    {
        _mainControls = new MainControls();
        _currentPath = 1;
        _previousPath = 1;
    }

    private void Start()
    {
        _mainControls.Touch.PrimaryTouch.started += ctx => StartSwipe(ctx);
        _mainControls.Touch.PrimaryTouch.canceled += ctx => EndSwipe(ctx);
    }

    private void OnEnable()
    {
        _camera = Camera.main;
        _mainControls.Enable();

        PlayerHitbox.OnContact += HandleHitboxContact;
    }

    private void OnDisable()
    {
        _mainControls.Disable();
    }

    private void Update() {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            SwipeLeft();
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            SwipeRight();
        #endif
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (!other.CompareTag("Obstacle")) return;
    //     Debug.Log("Trigger");
    //     _tweener.Kill();
    // }

    private void SwipeRight() {
        if (_currentPath != 2 && CanMove)
        {
            _previousPath = _currentPath;
            _currentPath++;
            _tweener = transform.DOMoveX(-3 + _currentPath * 3, 0.2f);
        }
    }

    private void SwipeLeft() {
        if (_currentPath != 0 && CanMove)
        {
            _previousPath = _currentPath;
            _currentPath--;
            _tweener = transform.DOMoveX(-3 + _currentPath * 3, 0.2f);
        }
    }

    private void StartSwipe(InputAction.CallbackContext context)
    {
        Debug.Log("Start Swipe");
        _startSwipe = _mainControls.Touch.PrimaryTouchPosition.ReadValue<Vector2>();
    }
    
    private void EndSwipe(InputAction.CallbackContext context) {
        Debug.Log("End Swipe");
        _endSwipe = _mainControls.Touch.PrimaryTouchPosition.ReadValue<Vector2>();
        
        Vector3 normalizedStart = _camera.ScreenToViewportPoint(_startSwipe);
        Vector3 normalizedEnd = _camera.ScreenToViewportPoint(_endSwipe);
        
        if (normalizedEnd.x > normalizedStart.x + 0.1f) {
            SwipeRight();
        } else if (normalizedEnd.x < normalizedStart.x - 0.1f) {
            SwipeLeft();
        }
    }

    private void HandleHitboxContact(HitboxType type) {
        if (type == HitboxType.Front) {
            GameManager.StartPause();
            return;
        }

        if (_tweener == null || _tweener.IsComplete())
        {
            return;
        }

        if (_tweener.ElapsedPercentage(false) >= 0.5f)
        {
            _camera.transform.DOShakePosition(1);
            return;
        }
        
        _tweener.Kill();
        _currentPath = _previousPath;
        _tweener = transform.DOMoveX(-3 + _currentPath * 3, 0.05f);
    }
}