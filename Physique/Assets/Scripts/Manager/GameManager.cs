using System.Collections.Generic;
using UnityEngine;

namespace a
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance{get; private set;}

        [SerializeField] private Transform poolOriginSpawn;
        [SerializeField] private GameObject ballsPrefabs;
        
        public BallPlayer _ballPlayer;
        private UIManager managerUI;
        
        private readonly Dictionary<GameObject, Queue<GameObject>> _pools =
            new Dictionary<GameObject, Queue<GameObject>>();
        private readonly List<KeyValuePair<GameObject, GameObject>> _active =
            new List<KeyValuePair<GameObject, GameObject>>();
        
        
        void Awake()
        {
            Instance = this;
            Init();
        }

        private void Init()
        {
            managerUI = GetComponent<UIManager>();
            SpawnTriangle(ballsPrefabs, poolOriginSpawn);
        }


        #region PoolBallsSpawn

        public void SpawnTriangle(GameObject prefab, Transform origin, int rows = 5, float gap = 0.01f)
        {
            float diameter = prefab.transform.localScale.x + gap;
            float rowOffset = diameter * Mathf.Sqrt(3f) / 2f;

            for (int row = 0; row < rows; row++)
            {
                for (int i = 0; i <= row; i++)
                {
                    float lateral = (i - row / 2f) * diameter;
                    Vector3 pos = origin.position
                                  + origin.forward * (row * rowOffset)
                                  + origin.right * lateral;

                    GameObject ball = GetFromPool(prefab);
                    ball.transform.SetPositionAndRotation(pos, origin.rotation);

                    if (ball.TryGetComponent(out IBall iBall))
                        iBall.Velocity = Vector3.zero;

                    ball.SetActive(true);
                    _active.Add(new KeyValuePair<GameObject, GameObject>(prefab, ball));
                }
            }
        }
        
        public void ReleaseAll()
        {
            foreach (var pair in _active)
            {
                pair.Value.SetActive(false);
                _pools[pair.Key].Enqueue(pair.Value);
            }
            _active.Clear();
        }

        private GameObject GetFromPool(GameObject prefab)
        {
            if (!_pools.TryGetValue(prefab, out Queue<GameObject> queue))
            {
                queue = new Queue<GameObject>();
                _pools[prefab] = queue;
            }

            if (queue.Count > 0)
                return queue.Dequeue();

            GameObject instance = Instantiate(prefab, transform);
            instance.SetActive(false);
            return instance;
        }
        #endregion
    }
}