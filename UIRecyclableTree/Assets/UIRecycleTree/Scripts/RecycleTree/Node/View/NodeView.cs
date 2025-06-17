using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace UIRecycleTreeNamespace {
	public class NodeView : RecycleItem, IPointerClickHandler {
		private const float DOUBLE_TAP_MAX_DELAY = 0.4f;
		private const float DOUBLE_TAP_MIN_DELAY = 0.07f;

		public event Action ClickedEvent, DoubleClickedEvent, CheckboxClickedEvent, ExpandClickEvent;
		public event Action<float> NodeWidthReadyEvent;

		[SerializeField] private IndentBox indentBox;
		[SerializeField] private StateControl expandToggleControl, imageControl;
		[SerializeField] private TextControl textControl;
		[SerializeField] private CheckboxControl checkboxControl;
		[SerializeField] private RectTransform content;
		[SerializeField] private Image fullRectSelectionImage, contentSelectionImage;
		[SerializeField] private CanvasGroup itemCanvasGroup;
		[SerializeField] private CanvasGroup contentCanvasGroup;
		public override RectTransform rectTransform => (RectTransform)transform;
		public TreePrefs treePrefs {
			set {
				_fullRectSelect = value.fullRectSelect;

				_componentSpacing = value.spacing;
				_childIndentPixels = value.childIndent;

				_leftPadding = value.leftPadding;
				_rightPadding = value.rightPadding;

				//-- Under test
				_hideExpandToggleWhenHasNoChildren = value.hideExpandToggleWhenHasNoChildren;
				//-- Under test

				expandToggleControl.width = value.toggleWidth;
				expandToggleControl.iconSize = value.toggleIconSize;

				var checkboxEnabled = value.checkboxEnabled;
				checkboxControl.isActive = checkboxEnabled;

				if (checkboxEnabled) {
					checkboxControl.width = value.checkedWidth;
					checkboxControl.iconSize = value.checkedIconSize;
				}

				var iconEnabled = value.iconEnabled;
				imageControl.isActive = iconEnabled;
				if (iconEnabled) {
					imageControl.width = value.iconWidth;
					imageControl.iconSize = value.iconSize;
				}
			}
		}
		public float fadedAlpha {
			set => _fadedAlpha = value;
		}

		public ExpandIcons toggleIcons {
			set => expandToggleControl.icons = value;
		}
		public ExpandIcons imageIcons {
			set => imageControl.icons = value;
		}

		public CheckboxIcons checkboxIcons {
			set => checkboxControl.icons = value;
		}

		public NodeTextStyle textStyle {
			set => textControl.style = value;
		}
		public Background backgroundStyle {
			set {
				imageForSelect.sprite = value.backgroundImage.sprite;
				imageForSelect.type = value.imageType;
				imageForSelect.pixelsPerUnitMultiplier = value.pixelPerUnitMultiplier;
				imageForSelect.color = value.backgroundImage.color;
			}
		}

		public float indent {
			set => indentBox.indent = value * _childIndentPixels;
		}
		public string text {
			set => textControl.text = value;
		}

		public ExpandedState state {
			set {
				expandToggleControl.state = value;
				imageControl.state = value;
			}
		}
		public bool isChecked {
			set => checkboxControl.isChecked = value;
		}
		public bool isFaded {
			set => contentCanvasGroup.alpha = value ? _fadedAlpha : 1;
		}

		private bool isDoubleClick {
			get {
				var currentTapTime = Time.realtimeSinceStartup;
				var tapTimeDelta = currentTapTime - _lastClickTime;
				if (tapTimeDelta is < DOUBLE_TAP_MAX_DELAY and > DOUBLE_TAP_MIN_DELAY)
					return true;
				_lastClickTime = currentTapTime;
				return false;
			}
		}

		public bool hasChildren {
			set {
				if (_hideExpandToggleWhenHasNoChildren)
					expandToggleControl.isActive = value;
			}
		}
		private Image imageForSelect => _fullRectSelect ? fullRectSelectionImage : contentSelectionImage;
		private float _childIndentPixels, _lastClickTime, _fadedAlpha;
		private bool _fullRectSelect, _initialized;
		private float _leftPadding, _rightPadding, _componentSpacing;
		private bool _hideExpandToggleWhenHasNoChildren;

		//We need arrange content manually, without standard components, for better performance
		private IEnumerator ArrangeContent() {
			yield return null;
			//ArrangeContent

			float trackPosition = _leftPadding;

			foreach (RectTransform child in content) {
				if (!child.gameObject.activeInHierarchy)
					continue;
				child.anchoredPosition = new Vector2(trackPosition, child.anchoredPosition.y);
				trackPosition += child.sizeDelta.x + _componentSpacing;
			}
			trackPosition += _rightPadding;
			content.sizeDelta = new Vector2(trackPosition, content.sizeDelta.y);

			//Arrange Content and indent  

			trackPosition = 0;
			foreach (RectTransform child in rectTransform) {
				child.anchoredPosition = new Vector2(trackPosition, child.anchoredPosition.y);
				trackPosition += child.sizeDelta.x + _componentSpacing;
			}

			rectTransform.sizeDelta = new Vector2(trackPosition, rectTransform.sizeDelta.y);
			NodeWidthReadyEvent?.Invoke(trackPosition);

			//When first created, the size of the content elements is unknown, so an flicker appears, 
			if (!_initialized)
				SetVisible(true);
			_initialized = true;
		}

		public void ClearPreviousSubscribes() {
			ClickedEvent = null;
			DoubleClickedEvent = null;
			ExpandClickEvent = null;
			CheckboxClickedEvent = null;
		}

		private void ClickNotify() =>
				ClickedEvent?.Invoke();

		private void DoubleClickNotify() =>
				DoubleClickedEvent?.Invoke();

		private void OnExpandToggleClick() =>
				ExpandClickEvent?.Invoke();

		protected void OnEnable() {
			expandToggleControl.ClickedEvent += OnExpandToggleClick;
			checkboxControl.ClickedEvent += OnCheckedClick;
		}

		protected void OnDisable() {
			expandToggleControl.ClickedEvent -= OnExpandToggleClick;
			checkboxControl.ClickedEvent -= OnCheckedClick;
		}
		private void OnCheckedClick() =>
				CheckboxClickedEvent?.Invoke();

		private void OnDestroy() =>
				ClearPreviousSubscribes();

		public void Refresh() =>
				StartCoroutine(ArrangeContent());

		public void OnPointerClick(PointerEventData eventData) {
			ClickNotify();
			if (isDoubleClick)
				DoubleClickNotify();
		}

		private void Awake() =>
				SetVisible(false);

		private void SetVisible(bool isVisible) =>
				itemCanvasGroup.alpha = isVisible ? 1 : 0;
	}
}
