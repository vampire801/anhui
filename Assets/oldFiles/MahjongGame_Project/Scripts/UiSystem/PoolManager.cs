using UnityEngine;
using System.Collections.Generic;
using XLua;
namespace anhui
{
    [Hotfix]
    [LuaCallCSharp]
    /// <summary>
    /// 对象池管理器
    /// </summary>
    public class PoolManager
    {
        // 对象池容器
        private static Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();




        /// <summary>
        /// 通过预制的路径和名称创建一个对象，或者从对象池中拿取一个对象
        /// </summary>
        /// <param name="path">克隆对象所在路径</param>
        /// <param name="name">预置体名称</param>
        /// <returns></returns>
        public static GameObject Spawn(string path, string name)
        {
            GameObject go = null;
            Pool pool = null;

            if (_pools.TryGetValue(name, out pool))
            {
                go = pool.Spawn(path, name);
            }
            else
            {
                pool = new Pool();
                _pools.Add(name, pool);

                go = pool.Spawn(path, name);
            }

            return go;
        }

        /// <summary>
        /// 回收一个游戏对象到对象池中
        /// </summary>
        /// <param name="go">需要消失的克隆体</param>
        public static void Unspawn(GameObject go)
        {
            Pool pool = null;
            if (go == null)
            {
                Debug.LogError("回收游戏对象为空");
                return;
            }
            string name = go.GetComponent<PoolAgent>().agentName;
            if (_pools.TryGetValue(name, out pool))
            {
                pool.UnSpawn(go);
            }
        }

        public static void Clear()
        {
            foreach (Pool pool in _pools.Values)
            {
                pool.Clear();
            }
            _pools.Clear();
        }
    }

    /// <summary>
    /// 对象池代理
    /// </summary>
    public class PoolAgent : MonoBehaviour
    {
        public string agentName;
        //定义对象生存期

        //private float _lifeTime;

        //public float lifeTime
        //{
        //    get
        //    {
        //        return _lifeTime;
        //    }
        //    set
        //    {
        //        _lifeTime = value;
        //    }
        //}

        //private void Update()
        //{
        //    if (_lifeTime <= 0)
        //    {
        //        _lifeTime = 0;
        //        return;
        //    }

        //    _lifeTime -= Time.deltaTime;
        //}
    }

    /// <summary>
    /// 对象池
    /// </summary>
    public class Pool
    {
        // 对象池容器
        private List<GameObject> _available = new List<GameObject>();


        /// <summary>
        /// 从池中拿取一个对象或者创建一个对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject Spawn(string path, string name)
        {
            GameObject go = null;

            // 如果对象池中有这种类型的对象，则直接从池中拿取一个对象
            if (_available.Count > 0)
            {
                go = _available[0];
                if (go == null)
                {
                    Debug.LogError("对象池中对象报空============================");
                }
                go.transform.localScale = Vector3.one;
                go.SetActive(true);

                _available.Remove(go);
            }
            // 如果池中没有这种类型的对象，则重新创建一个对象并添加到池中
            else
            {
                string fullpath = path + name;
                Object obj = Resources.Load(fullpath);
                go = GameObject.Instantiate(obj) as GameObject;
                go.transform.position = Vector3.zero;
                go.transform.rotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                //go.lifeTime = 2f;
                go.name = name;

            }

            PoolAgent agent = go.GetComponent<PoolAgent>();
            if (agent == null)
                agent = go.AddComponent<PoolAgent>();

            //agent.lifeTime = 2f;
            agent.agentName = name;

            return go;
        }

        public void UnSpawn(GameObject go)
        {

            _available.Add(go);
            go.SetActive(false);
            go.transform.position = new UnityEngine.Vector3(10000, 10000, 10000);
        }

        public void Clear()
        {
            _available.Clear();
            _available = null;
        }
    }
}