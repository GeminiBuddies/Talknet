using System;
using System.Collections.Generic;
using System.Linq;

namespace Talknet.Plugin.DAG {
    // not a very large map, so...
    internal class DAG<T> {
        private readonly int _nodeCount;
        private readonly Dictionary<T, int> _numbers;
        private readonly T[] _nodes;
        private readonly SortedSet<int>[] _map;

        public DAG(IEnumerable<T> nodes) {
            _nodes = nodes as T[] ?? nodes.ToArray();
            _nodeCount = _nodes.Length;

            int count = 0;
            _numbers = _nodes.ToDictionary(any => any, any => count++);

            _map = new SortedSet<int>[_nodeCount];
        }

        public void AddEdge(T from, T to) {
            if (!_numbers.TryGetValue(from, out var ifrom)) throw new ArgumentOutOfRangeException(nameof(from));
            if (!_numbers.TryGetValue(to, out var ito)) throw new ArgumentOutOfRangeException(nameof(to));

            if (_map[ifrom] == null) _map[ifrom] = new SortedSet<int>();
            if (!_map[ifrom].Contains(ito)) _map[ifrom].Add(ito);
        }
        
        public IEnumerable<T> TopologicalOrder() {
            int[] ind = new int[_nodeCount];
            Queue<int> q = new Queue<int>();
            List<T> cache = new List<T>();
                
            foreach (var u in _map) {
                if (u == null) continue;
                foreach (var v in u)
                    ++ind[v];
            }

            for (int i = 0; i < _nodeCount; ++i) {
                if (ind[i] == 0) q.Enqueue(i);
            }

            while (q.Count > 0) {
                var u = q.Dequeue();

                cache.Add(_nodes[u]);
                if (_map[u] == null) continue;
                foreach (var v in _map[u]) {
                    if (--ind[v] == 0) q.Enqueue(v);
                }
            }

            if (cache.Count < _nodeCount) throw new NotADagException();
            return cache;
        }
    }
}
