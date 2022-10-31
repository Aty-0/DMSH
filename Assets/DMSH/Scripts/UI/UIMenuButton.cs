using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using DMSH.Misc.Animated;

using TMPro;

// TODO: Drag, drop

namespace DMSH.UI
{
    [AddComponentMenu("UI/DMSH/UIMenuButton", 0)]
    public class UIMenuButton : Button, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField]
        private TMP_Text _text;

        private Coroutine _colorCoroutine = null;
        private Coroutine _scaleCoroutine = null;

        protected override void OnEnable()
        {
            if (_text == null)
            {
                _text = GetComponentInChildren<TMP_Text>();
            }

            _text.color = colors.normalColor;

            transform.localScale = Vector3.one;

            if (_colorCoroutine != null)
            {
                StopCoroutine(_colorCoroutine);
            }

            if (_scaleCoroutine != null)
            {
                StopCoroutine(_scaleCoroutine);
            }
        }

        protected Vector3 GetDragDirection(Vector3 dragVector)
        {
            Vector2 draggedDir = Vector2.zero;
            draggedDir += (dragVector.x > 0) ? new Vector2(1, 0) : new Vector2(-1, 0);
            draggedDir += (dragVector.y > 0) ? new Vector2(0, 1) : new Vector2(0, -1);
            return draggedDir;
        }

        public void OnDrag(PointerEventData eventData)
        {

        }

        public void OnBeginDrag(PointerEventData eventData)
        {

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // What's if player just hold the button and 
            // Move cursor to out of bounds button hitbox
            // On this case we need to reset color
            _text.color = colors.normalColor;
        }

        /// <summary>
        /// Cursor intersect with button
        /// </summary>
        /// <param name="eventData"></param>

        public override void OnPointerEnter(PointerEventData eventData)
        {
            // If we are already have coroutine we are disable it 
            if (_colorCoroutine != null)
            {
                StopCoroutine(_colorCoroutine);
            }

            // When we are disable previous animation we are start new
            _colorCoroutine = StartCoroutine(BasicAnimationsPack.SmoothChangeToColorForText(_text, colors.highlightedColor));

            //Same logic for this coroutine 
            if (_scaleCoroutine != null)
            {
                StopCoroutine(_scaleCoroutine);
            }

            _scaleCoroutine = StartCoroutine(BasicAnimationsPack.SmoothResize(transform));
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            // If we are already have coroutine we are disable it 
            if (_colorCoroutine != null)
            {
                StopCoroutine(_colorCoroutine);
            }

            // When we are disable previous animation we are start new
            _colorCoroutine = StartCoroutine(BasicAnimationsPack.SmoothChangeToColorForText(_text, colors.normalColor, 3.5f));

            // Same logic for this coroutine 
            if (_scaleCoroutine != null)
            {
                StopCoroutine(_scaleCoroutine);
            }

            _scaleCoroutine = StartCoroutine(BasicAnimationsPack.SmoothResize(transform, 1.0f));
        }

        /// <summary>
        /// Button click callbacks 
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (_colorCoroutine != null)
            {
                StopCoroutine(_colorCoroutine);
            }

            _text.color = colors.pressedColor;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (_colorCoroutine != null)
            {
                StopCoroutine(_colorCoroutine);
            }
            _text.color = colors.highlightedColor;
        }

        public override void OnPointerClick(PointerEventData eventData)
        { 
            // Invoke the onClick callback
            onClick.Invoke();
        }

    }
}
