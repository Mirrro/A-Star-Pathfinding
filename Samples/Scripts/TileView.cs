using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Mirrro.Pathfinding
{
    public class TileView : MonoBehaviour, IPointerClickHandler
    {
        public event Action<TileView> OnLeftClick;
        public event Action<TileView> OnRightClick;

        [SerializeField] private Renderer renderer;
        public Vector2Int GridPosition { get; private set; }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                OnLeftClick?.Invoke(this);
            }

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                OnRightClick?.Invoke(this);
            }
        }

        public void UpdateColor(Color color)
        {
            renderer.material.color = color;
        }

        public void Initialize(Vector2Int vector2Int)
        {
            GridPosition = vector2Int;
        }
    }
}