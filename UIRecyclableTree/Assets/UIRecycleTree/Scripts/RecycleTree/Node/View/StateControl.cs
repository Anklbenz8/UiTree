using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UIRecycleTreeNamespace {
	public class StateControl : Selectable, IPointerClickHandler {
		public event Action ClickedEvent;

		[SerializeField] private Image targetImage;
		private Icon _noChild, _expanded, _collapsed;
		private RectTransform _rectTransform;

		public ExpandIcons icons {
			set {
				_noChild = value.noChildren;
				_expanded = value.expanded;
				_collapsed = value.collapsed;
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
		public ExpandedState state {
			get => _currentState;
			set {
				_currentState = value;
				Refresh();
			}
		}

		private ExpandedState _currentState;

		public void OnPointerClick(PointerEventData eventData) =>
				ClickedEvent?.Invoke();

		private void Refresh() {
			switch (state) {
				case ExpandedState.NoChild:
					targetImage.sprite = _noChild.sprite;
					targetImage.color = _noChild.color;
					break;
				case ExpandedState.Expanded:
					targetImage.sprite = _expanded.sprite;
					targetImage.color = _expanded.color;
					break;
				case ExpandedState.Collapsed:
					targetImage.sprite = _collapsed.sprite;
					targetImage.color = _collapsed.color;
					break;
				default:
					throw new Exception($"State {state} not implemented");
			}
		}

		protected override void Awake() {
			base.Awake();
			_rectTransform = (RectTransform)transform;
		}
	}
}