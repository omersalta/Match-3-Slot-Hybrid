using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Game {

    public class InputState : MonoBehaviour {
        #region Private Fields
        
        private bool _tap, _swipeLeft, _swipeRight, _swipeUp, _swipeDown;
        private Vector2 _startTouch, _swipeDelta, _currentPos, _downPos, _upPos;
        private bool _isDragging, _mouseUp;
        
        #endregion

        #region Public Properties
        
        [Header("Settings")]
        [Tooltip("Minimum swipe distance to recognize swipe gesture.")]
        public float swipeThreshold = 50f;

        [Header("Events")]
        public UnityEvent onTap;
        public UnityEvent<Vector2,Vector2> onSwipeLeft;
        public UnityEvent<Vector2,Vector2> onSwipeRight;
        public UnityEvent<Vector2,Vector2> onSwipeUp;
        public UnityEvent<Vector2,Vector2> onSwipeDown;
        public UnityEvent onMouseUp;

        // Expose read-only properties for the input states
        public bool Tap => _tap;
        public bool MouseUp => _mouseUp;
        public bool IsDragging => _isDragging;
        public Vector2 SwipeDelta => _swipeDelta;

        #endregion

        private void Update() {
            ResetInputs();
            HandleInput();
            DetectSwipes();
        }

        private void ResetInputs() {
            _tap = _mouseUp = _swipeLeft = _swipeRight = _swipeUp = _swipeDown = false;
        }

        private void HandleInput() {
            _currentPos = Input.mousePosition;

            #region Standalone Inputs (Mouse)
            if (Input.GetMouseButtonDown(0)) {
                BeginTouch(Input.mousePosition);
            } 
            else if (Input.GetMouseButtonUp(0)) {
                EndTouch();
                onMouseUp?.Invoke();
            }
            #endregion

            #region Mobile Input (Touch)
            if (Input.touchCount > 0) {
                Touch touch = Input.GetTouch(0);
                _currentPos = touch.position;

                if (touch.phase == TouchPhase.Began) {
                    BeginTouch(touch.position);
                } 
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
                    EndTouch();
                    onMouseUp?.Invoke();
                }
            }
            #endregion
        }

        private void BeginTouch(Vector2 position) {
            _tap = true;
            _isDragging = true;
            _downPos = _startTouch = position;
            onTap?.Invoke();
        }

        private void EndTouch() {
            _isDragging = false;
            _upPos = _currentPos;
            _mouseUp = true;
            ResetSwipe();
        }

        private void DetectSwipes() {
            _swipeDelta = Vector2.zero;

            if (_isDragging) {
                if (Input.touchCount > 0)
                    _swipeDelta = Input.GetTouch(0).position - _startTouch;
                else if (Input.GetMouseButton(0))
                    _swipeDelta = (Vector2)Input.mousePosition - _startTouch;
            }

            if (_swipeDelta.magnitude > swipeThreshold) {
                float x = _swipeDelta.x;
                float y = _swipeDelta.y;

                if (Mathf.Abs(x) > Mathf.Abs(y)) {
                    if (x < 0) {
                        _swipeLeft = true;
                        onSwipeLeft?.Invoke(_startTouch,Vector2.left);
                    } else {
                        _swipeRight = true;
                        onSwipeRight?.Invoke(_startTouch,Vector2.right);
                    }
                } else {
                    if (y < 0) {
                        _swipeDown = true;
                        onSwipeDown?.Invoke(_startTouch,Vector2.down);
                    } else {
                        _swipeUp = true;
                        onSwipeUp?.Invoke(_startTouch,Vector2.up);
                    }
                }

                ResetSwipe();
            }
        }

        private void ResetSwipe() {
            _startTouch = _swipeDelta = Vector2.zero;
            _isDragging = false;
        }
    }
}
