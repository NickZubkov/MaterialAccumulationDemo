using UnityEngine;
using Zenject;

namespace MaterialAccumulation.Presentation
{
    /// <summary>
    /// Создаёт вью инстанцированием префаба через контейнер.
    /// Одна реализация на все вью: они отличаются типами, а не способом создания.
    /// </summary>
    public sealed class PrefabViewFactory<TView, TComponent> : IViewFactory<TView>
        where TComponent : Component, TView
    {
        private readonly DiContainer _container;
        private readonly TComponent _prefab;

        public PrefabViewFactory(DiContainer container, TComponent prefab)
        {
            _container = container;
            _prefab = prefab;
        }

        public TView Create() => _container.InstantiatePrefabForComponent<TComponent>(_prefab);
    }
}
