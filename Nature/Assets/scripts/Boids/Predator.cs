using UnityEngine;

namespace Boids
{
    public class Predator : MonoBehaviour
    {
        public float MinSpeed = 0.2f, MaxSpeed = 3f, RotationSpeed = 0.14f, MinAge = 30, MaxAge = 120;
        public float MinVision = 3, MaxVision = 10, MinCapacity = 10, MaxCapacity = 20, KillEnergy = 10;
        public float MinMetabolism = 0.01f, MaxMetabolism = 1, Inertia = 1, LevyChance = 5, FertilityChance = 0.64f;
        public float MateDistance = 0.1f, MutationFactor = 0.5f, RestReproduction = 44;

        private float _maxSpeed, _vision, _age, _expectedLife, _nextAge, _capacity, _metabolism, _speed;
        private float _energy, _nextReproduction;        
        private bool _isMale, _mating, _isAlpha;
        private Flock _globalFlock;
        private GameObject _couple;        
        public static int MalesAlive, FemalesAlive;

        private void Start()
        {
            if (MalesAlive < FemalesAlive)
            {
                _isMale = true;
                MalesAlive++;
            }
            else
            {
                _isMale = false;
                FemalesAlive++;
            }            
            _maxSpeed = Random.Range(MinSpeed + 2, MaxSpeed);
            _speed = _maxSpeed;
            _vision = Random.Range(MinVision, MaxVision);
            _expectedLife = Random.Range(MinAge, MaxAge);
            _metabolism = Random.Range(MinMetabolism, MaxMetabolism);
            _capacity = Random.Range(MinCapacity, MaxCapacity);
            _energy = MinCapacity;
            foreach (GameObject food in Flock.FishFoods)
                Physics.IgnoreCollision(food.GetComponent<Collider>(), GetComponent<Collider>());

            foreach (GameObject food in Flock.Trash)
                Physics.IgnoreCollision(food.GetComponent<Collider>(), GetComponent<Collider>());

            foreach (GameObject food in Flock.Predators)
                Physics.IgnoreCollision(food.GetComponent<Collider>(), GetComponent<Collider>());
        }

        private void FixedUpdate()
        {
            GetComponent<Rigidbody>().velocity = transform.forward * _speed;
        }

        private void Update()
        {
            if (Time.time > _nextAge)
            {
                _nextAge = Time.time + 1;
                _age += 1;
                if (_age > _expectedLife)
                    Die();
                _energy -= _metabolism;
                if (_energy <= 0)
                    Die();
            }

            if (Vector3.Distance(transform.position, Vector3.zero) >= _globalFlock.EnvironmentSize)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-transform.position),
                    RotationSpeed);
            
            else if (_isAlpha)
            {
                _speed = MinSpeed;
            }
            else if (_mating)
            {
                transform.LookAt(_couple.transform);
                _speed = _maxSpeed;
                SearchCouple();
            }
            else if (_energy >= _capacity)
            {
                _speed = MinSpeed;
            }
            else if (Random.Range(0, LevyChance) < 1)
                ApplyRules();
        }

        private void ApplyRules()
        {
            Vector3 goalPos = Random.insideUnitSphere * 4;
            float min = float.MaxValue;
            foreach (GameObject fish in Flock.Fishes)
            {
                float tmp = Vector3.Distance(transform.position, fish.transform.position);
                if (tmp > _vision)
                    continue;
                tmp += Inertia;
                if (tmp < min)
                {
                    min = tmp;
                    goalPos = fish.transform.position;
                }
            }

            _speed = _maxSpeed;
            goalPos -= transform.position;
            if (goalPos == Vector3.zero)
                return;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(goalPos),
                RotationSpeed);
        }

        public void Eat()
        {
            _energy += KillEnergy;
        }

        private void Awake()
        {
            _globalFlock = GameObject.FindGameObjectWithTag("GameController").GetComponent<Flock>();
        }

        private void Die()
        {
            if (_isMale)
                MalesAlive--;
            else
                FemalesAlive--;
            Flock.Predators.Remove(gameObject);
            Destroy(gameObject);
        }

        private void SearchCouple()
        {
            if (Vector3.Distance(_couple.transform.position, transform.position) <= MateDistance)
            {
                _mating = false;
                Reproduce(_couple.GetComponent<Predator>());                
            }
        }
        
        public void Mate(GameObject target)
        {
            if (Time.time > _nextReproduction)
            {
                _couple = target;
                _mating = true;
            }
        }

        public void SetAlpha(bool isAlpha)
        {
            _isAlpha = isAlpha;
        }
        
        private void Reproduce(Predator other)
        {
            float aux = _globalFlock.EnvironmentSize - 2;
            Vector3 pos = Random.insideUnitSphere * aux;
            GameObject whale = Instantiate(_globalFlock.PredatorPrefab, pos, Quaternion.identity);
            Flock.Predators.Add(whale);            
            whale.GetComponent<Predator>().Inherit(     
                Random.value >= 0.5f ? _vision : other._vision,
                Random.value >= 0.5f ? _expectedLife : other._expectedLife,
                Random.value >= 0.5f ? _capacity : other._capacity,
                Random.value >= 0.5f ? _metabolism : other._metabolism,                
                _energy + other._energy                
            );
            _nextReproduction = Time.time + RestReproduction * Random.value >= FertilityChance ? 0.5f : 1f;
        }
        
        private void Inherit(float vision, float expectedLife, float capacity, float metabolism,
            float energy)
        {
            float mutation = Random.Range(1 - MutationFactor, 1 + MutationFactor);            
            _vision = vision * mutation;
            _expectedLife = expectedLife * mutation;
            _capacity = capacity * mutation;
            _metabolism = metabolism * mutation;            
            _energy = energy * mutation;            
        }
        
        public bool IsMale => _isMale;

        public float Energy
        {
            get { return _energy; }
            set { _energy = value; }
        }

        public bool Mating
        {            
            set { _mating = value; }
        }

        public float Capacity
        {
            get { return _capacity; }
            set { _capacity = value; }
        }
    }    
}