using TuringPattern;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boids
{
    public class Fish : MonoBehaviour
    {
        public float MinSpeed = 0.2f, MaxSpeed = 3f, RotationSpeed = 0.12f, NeighborDistance = 2.4f;
        public float MinBite = 2, MaxBite = 4, MinVision = 3, MaxVision = 10, MinCapacity = 10, MaxCapacity = 20;
        public float MinMetabolism = 0.01f, MaxMetabolism = 1, Inertia = 1, LevyChance = 3, Stop = 0.4f;
        public float DeltaScale = 0.4f, MinReproductionRate = 7f, MaxReproductionRate = 10f, ScapeDistance = 2;
        public float ReproductionAge = 15, MutationFactor = 0.5f, NeighborAvoidance = 0.5f, MinAge = 30, MaxAge = 60;
        public float MaxTDifference = 0.5f, MaxSDifference = 1, DoubleChildProb = 0.2f, NoDiscriminationChance = 0.1f;

        private float _speed, _bite, _vision, _age, _expectedLife, _nextAge, _capacity, _metabolism, _energy;
        private float _reproductionRate, _nextReproduction;
        private Vector3 _transform;
        private Flock _globalFlock;
        private FishFood _lastTree;
        private const float GenreRatio = 0.5f;
        private bool _isMale;

        private void Start()
        {
            _speed = Random.Range(MinSpeed + 2, MaxSpeed);
            _isMale = Random.value >= GenreRatio;
            _nextReproduction = Time.time + ReproductionAge;
            _energy = (_capacity + MinCapacity) / 2;
            if (Time.time > 2.0f)
                return;
            _reproductionRate = Random.Range(MinReproductionRate, MaxReproductionRate);
            _bite = Random.Range(MinBite, MaxBite);
            _vision = Random.Range(MinVision, MaxVision);
            _expectedLife = Random.Range(MinAge, MaxAge);
            _metabolism = Random.Range(MinMetabolism, MaxMetabolism);
            _capacity = Random.Range(MinCapacity, MaxCapacity);
            _transform = new Vector3(
                transform.localScale.x - Random.Range(-DeltaScale, DeltaScale),
                transform.localScale.x - Random.Range(-DeltaScale, DeltaScale),
                transform.localScale.x - Random.Range(-DeltaScale, DeltaScale));
            transform.localScale = _transform;
        }

        private void Inherit(float bite, float vision, float expectedLife, float capacity, float metabolism,
            float reproductionRate, float energy, Vector3 transformation, float alpha)
        {
            float mutation = Random.Range(1 - MutationFactor, 1 + MutationFactor);
            _bite = bite * mutation;
            _vision = vision * mutation;
            _expectedLife = expectedLife * mutation;
            _capacity = capacity * mutation;
            _metabolism = metabolism * mutation;
            _reproductionRate = reproductionRate * mutation;
            _energy = energy * mutation;
            _transform = transformation * mutation;
            transform.localScale = _transform;
            transform.GetChild(0).gameObject.GetComponent<InhibitorActivator>().SetAlpha(alpha * mutation);
        }

        private void Reproduce(Fish other)
        {
            float aux = _globalFlock.EnvironmentSize - 3;
            Vector3 pos = Random.insideUnitSphere * aux;
            GameObject fish = Instantiate(_globalFlock.FishPrefab, pos, Quaternion.identity);
            Flock.Fishes.Add(fish);
            _energy /= 2;
            other._energy /= 2;
            fish.GetComponent<Fish>().Inherit(
                Random.value >= 0.5f ? _bite : other._bite,
                Random.value >= 0.5f ? _vision : other._vision,
                Random.value >= 0.5f ? _expectedLife : other._expectedLife,
                Random.value >= 0.5f ? _capacity : other._capacity,
                Random.value >= 0.5f ? _metabolism : other._metabolism,
                Random.value >= 0.5f ? _reproductionRate : other._reproductionRate,
                _energy + other._energy,
                Random.value >= 0.5f ? _transform : other._transform,
                Random.value >= 0.5f
                    ? transform.GetChild(0).gameObject.GetComponent<InhibitorActivator>().GetAlpha()
                    : other.transform.GetChild(0).gameObject.GetComponent<InhibitorActivator>().GetAlpha()
            );
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Tree"))
            {
                _energy = Mathf.Min(_energy + other.GetComponent<FishFood>().Eat(_bite), _capacity);
                _lastTree = other.gameObject.GetComponent<FishFood>();
            }
            else if (other.CompareTag("Predator"))
            {
                Predator predator = other.GetComponent<Predator>();
                if (predator.Energy >= predator.Capacity)
                    return;
                predator.Eat();
                Die();
            }
            else if (!_isMale && other.CompareTag("Fish"))
            {
                Fish fish = other.gameObject.GetComponent<Fish>();
                if (fish._isMale == _isMale || Time.time < _nextReproduction || Time.time < fish._nextReproduction
                    || fish._energy <= fish._capacity / 2.0f || _energy <= _capacity / 2.0f)
                    return;
                
                if (Random.value > NoDiscriminationChance)
                {
                    float alpha1 = transform.GetChild(0).gameObject.GetComponent<InhibitorActivator>().GetAlpha();
                    float alpha2 = other.transform.GetChild(0).gameObject.GetComponent<InhibitorActivator>().GetAlpha();
                    float skinFactor = Mathf.Abs(alpha1 - alpha2);
                    float dist = Vector3.Distance(transform.localScale, other.transform.localScale);
                    if (skinFactor > MaxSDifference || dist > MaxTDifference)
                        return;
                }

                fish.UpdateNexReproduction();
                UpdateNexReproduction();
                Reproduce(fish);
                if (Random.value <= DoubleChildProb)
                    Reproduce(fish);
            }
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
                if (_lastTree != null)
                    _lastTree.AddWaste(_metabolism);
                if (_energy <= 0)
                    Die();
            }

            Vector3 vavoid = Vector3.zero;
            bool run = false;
            foreach (GameObject predator in Flock.Predators)
                if (Vector3.Distance(predator.transform.position, transform.position) <= ScapeDistance)
                {
                    vavoid += transform.position - predator.transform.position;
                    run = true;
                }

            if (run)
            {
                _speed = MaxSpeed;
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(vavoid - transform.position),
                    RotationSpeed);
            }
            else if (Vector3.Distance(transform.position, Vector3.zero) >= _globalFlock.EnvironmentSize)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-transform.position),
                    RotationSpeed);
            else if (Random.Range(0, LevyChance) < 1)
                ApplyRules();
        }

        private void ApplyRules()
        {
            Vector3 vavoid = Vector3.zero, vcenter = Vector3.zero;
            float groupSpeed = 0;
            int groupSize = 0;
            foreach (GameObject fish in Flock.Fishes)
            {
                float dist = Vector3.Distance(fish.transform.position, transform.position);
                if (dist <= NeighborDistance)
                {
                    vcenter += fish.transform.position;
                    groupSize++;
                    if (dist <= NeighborAvoidance && dist > 0)
                        vavoid += transform.position - fish.transform.position;
                    groupSpeed += fish.GetComponent<Fish>()._speed;
                }
            }

            vcenter /= groupSize;
            vcenter += vavoid;
            Vector3 goalPos = vcenter;

            float min = float.MaxValue;
            foreach (GameObject food in Flock.FishFoods)
            {
                if (!food.activeSelf) continue;
                float tmp = Vector3.Distance(transform.position, food.transform.position);
                if (tmp > _vision)
                    continue;
                FishFood fishFood = food.GetComponent<FishFood>();
                tmp -= fishFood.GetEnergy() - fishFood.GetWaste() + Inertia;
                if (tmp < min)
                {
                    min = tmp;
                    goalPos = food.transform.position;
                }
            }

            if (Vector3.Distance(transform.position, goalPos) <= Stop)
                _speed = MinSpeed;
            else
                _speed = MaxSpeed - 1;

            groupSpeed += _speed;
            vcenter += goalPos - transform.position;
            vcenter -= transform.position;
            if (vcenter == Vector3.zero) return;
            _speed = Mathf.Clamp(groupSpeed / groupSize, MinSpeed, MaxSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(vcenter), RotationSpeed);
        }

        private void Awake()
        {
            _globalFlock = GameObject.FindGameObjectWithTag("GameController").GetComponent<Flock>();
        }

        private void Die()
        {
            Flock.Fishes.Remove(gameObject);
            Destroy(gameObject);
        }

        private void UpdateNexReproduction()
        {
            _nextReproduction = Time.deltaTime + _reproductionRate;
        }
    }
}