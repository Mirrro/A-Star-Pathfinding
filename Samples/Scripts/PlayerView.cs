using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mirrro.Pathfinding
{
    public class PlayerView : MonoBehaviour
    {
        public Vector2Int CurrentGridPosition { get; private set; }

        public void MoveAlongPath(List<Node> path, Action callback)
        {
            if (path == null || path.Count == 0)
            {
                callback?.Invoke();
                return;
            }

            var positions = path.Select(n => new Vector3(n.X, 0, n.Y)).ToList();
            StartCoroutine(MoveRoutine(positions, callback));
        }

        private IEnumerator MoveRoutine(List<Vector3> worldPath, Action onDone)
        {
            foreach (var point in worldPath)
            {
                Vector3 start = transform.position;
                Vector3 end = point;
                float elapsed = 0f;
                float duration = 0.3f;

                while (elapsed < duration)
                {
                    transform.position = Vector3.Lerp(start, end, elapsed / duration);
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                transform.position = end;
                CurrentGridPosition = new Vector2Int(Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.z));
            }

            onDone?.Invoke();
        }

    }
}