namespace Scripts.Utils.Unity
{
    using System;

    using UnityEngine;

    [Serializable]
    public struct Layer
    {
        [SerializeField]
        private int m_layer;

        public int Index
        {
            get => m_layer;
            set => m_layer = value;
        }

        public int Mask => 1 << m_layer;

        public string Name
        {
            get => LayerMask.LayerToName(m_layer);
            set => m_layer = LayerMask.NameToLayer(value);
        }

        public static implicit operator int(Layer l)
        {
            return l.Index;
        }

        public static implicit operator Layer(int i)
        {
            return new Layer {Index = i};
        }

        public Layer(int index)
        {
            m_layer = index;
        }

        public Layer(string name)
        {
            m_layer = LayerMask.NameToLayer(name);
        }
    }
}