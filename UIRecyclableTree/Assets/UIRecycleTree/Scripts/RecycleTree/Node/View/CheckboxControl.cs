using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UIRecycleTreeNamespace {
	public class CheckboxControl : Selectable, IPointerClickHandler {
		public event Action ClickedEvent;

		[SerializeField] private Image targetImage;

		private Icon _checked, _unchecked;
		private RectTransform _rectTransform;

		public bool isChecked {
			set {
			    targetImage.sprite =value ? _checked.sprite : _unchecked.sprite; ;
				targetImage.color = value ? _checked.color : _unchecked.color;
			}
		}

		public CheckboxIcons icons {
			set {
				_checked = value.checkedState;
				_unchecked = value.uncheckedState;
			}
		}

		public float width {
			set => _rectTransform.sizeDelta = new Vector2(value, _rectTransform.sizeDelta.y);
		}

		public Vector2 iconSize {
			get => targetImage.rectTransform.sizeDelta;
			set => targetImage.rectTransform.sizeDelta = value;
		}

		public bool isActive {
			get => gameObject.activeInHierarchy;
			set {
				if (gameObject.activeInHierarchy == value) return;
				gameObject.SetActive(value);
			}
		}
		
		public void OnPointerClick(PointerEventData eventData) =>
				ClickedEvent?.Invoke();

		protected override void Awake() {
			base.Awake();
			_rectTransform = (RectTransform)transform;
		}
	}
}