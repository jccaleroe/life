using UnityEngine;

namespace Boids
{
    public class FishFood : MonoBehaviour
    {
        public float MinEnergy = 1, MaxEnergy = 4, ChargeRate = 0.5f, ChargeDelta = 1;
        private float _energyCapacity, _energy, _nextTime, _waste;

        public float Eat(float bite)
        {
            float tmp = _energy;
            _energy = Mathf.Max(0, _energy - bite);
            return tmp - _energy;
        }

        private void Start()
        {
            _energyCapacity = Random.Range(MinEnergy, MaxEnergy);
        }

        private void Update()
        {
            if (Time.time >= _nextTime)
            {
                _nextTime = Time.time + ChargeRate;                
                _energy = Mathf.Min(_energy + ChargeDelta, _energyCapacity);
                _waste = Mathf.Max(0, _waste - ChargeDelta);
            }
        }

        public void AddWaste(float waste)
        {
            _waste += waste;
        }
        
        public float GetEnergy()
        {
            return _energy;
        }

        public float GetWaste()
        {
            return _waste;
        }
    }
}