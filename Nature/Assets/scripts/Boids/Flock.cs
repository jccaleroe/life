using System.Collections.Generic;
using UnityEngine;

namespace Boids
{
    public class Flock : MonoBehaviour
    {
        public int NumFishes = 30, SingleFoodSize = 44, SourcesFood = 44, PredatorsNum = 2;
        public static List<GameObject> FishFoods, Fishes, Predators, Trash;
        public GameObject FishPrefab, FoodPrefab, PredatorPrefab;
        public Transform[] SourcesTransforms;
        public float SourcesRadius = 1.2f, FoodDistance = 0.28f, EnvironmentSize = 6, SeasonRate = 10f;
        public float SourceChance = 0.4f, AppearanceTime = 10f, MatingSeason = 36f, MatingDuration = 4f;
        private List<List<GameObject>> _soureces;
        private float _nextSeason, _nextMating;
        private bool _predators = true, _matingSeason;
        private Predator _bestWhale;

        private bool IsInRadious(Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b) <= FoodDistance;
        }

        private void Awake()
        {
            _soureces = new List<List<GameObject>>(SourcesTransforms.Length);
            FishFoods = new List<GameObject>(SourcesFood * SourcesTransforms.Length);
            Fishes = new List<GameObject>(NumFishes);
            Trash = new List<GameObject>(SingleFoodSize);
            float aux = EnvironmentSize - 3f;
            for (int i = 0; i < NumFishes; i++)
            {
                Vector3 pos = new Vector3(
                    Random.Range(-aux, aux),
                    Random.Range(-aux, aux),
                    Random.Range(-aux, aux));
                Fishes.Add(Instantiate(FishPrefab, pos, Quaternion.identity));
            }
            
            Predators = new List<GameObject>(PredatorsNum);
            _nextMating += AppearanceTime + MatingSeason;

            int c = 0;
            foreach (Transform source in SourcesTransforms)
            {
                _soureces.Add(new List<GameObject>(SourcesFood));
                for (int j = 0; j < SourcesFood; j++)
                {
                    Vector3 tmp = Random.insideUnitSphere * SourcesRadius + source.transform.position;
                    bool alone = true;
                    foreach (GameObject v in FishFoods)
                        if (IsInRadious(v.transform.position, tmp))
                        {
                            alone = false;
                            break;
                        }

                    if (alone)
                    {
                        GameObject plant = Instantiate(FoodPrefab, tmp, Quaternion.identity);
                        plant.SetActive(false);
                        _soureces[c].Add(plant);
                        FishFoods.Add(plant);
                    }
                }

                c++;
            }

            float aux1 = EnvironmentSize - 1f;
            for (int i = 0; i < SingleFoodSize; i++)
            {
                Vector3 tmp = new Vector3(
                    Random.Range(-aux1, aux1),
                    Random.Range(-aux1, aux1),
                    Random.Range(-aux1, aux1));
                bool alone = true;
                foreach (GameObject v in FishFoods)
                    if (IsInRadious(v.transform.position, tmp))
                    {
                        alone = false;
                        break;
                    }

                if (alone)
                    Trash.Add(Instantiate(FoodPrefab, tmp, Quaternion.identity));
            }
        }

        private void Update()
        {
            if (Time.time >= _nextSeason)
            {
                _nextSeason = Time.time + SeasonRate;
                int a = Mathf.RoundToInt(Random.Range(0, _soureces.Count - 0.51f));
                int i = 0;
                foreach (List<GameObject> l in _soureces)
                {
                    bool active = Random.value <= SourceChance || a == i;
                    foreach (GameObject plant in l)
                        plant.SetActive(active);
                    i++;
                }
            }

            if (!_predators && Predator.MalesAlive + Predator.FemalesAlive < 2)
                Predators.Add(Instantiate(PredatorPrefab, Vector3.zero, Quaternion.identity));
            
            if (_predators && Time.time >= AppearanceTime)
            {
                _predators = false;
                float aux1 = EnvironmentSize - 2f;
                for (int i = 0; i < PredatorsNum; i++)
                {
                    Vector3 tmp = new Vector3(
                        Random.Range(-aux1, aux1),
                        Random.Range(-aux1, aux1),
                        Random.Range(-aux1, aux1));
                    Predators.Add(Instantiate(PredatorPrefab, tmp, Quaternion.identity));
                }
            }            

            if (Time.time >= _nextMating)
            {
                if (_matingSeason)
                {
                    _matingSeason = false;
                    _nextMating += MatingSeason;
                    foreach (GameObject whale in Predators)
                    {
                        Predator predator = whale.GetComponent<Predator>();
                        if (!predator.IsMale)
                            predator.Mating = false;
                    }
                    if (_bestWhale != null)
                        _bestWhale.SetAlpha(false);
                }
                else
                {
                    float maxEnergy = 0;                    
                    foreach (GameObject whale in Predators)
                    {
                        Predator predator = whale.GetComponent<Predator>();
                        if (predator.IsMale && predator.Energy > maxEnergy)
                        {
                            maxEnergy = predator.Energy;
                            _bestWhale = predator;
                        }
                    }

                    if (_bestWhale != null)
                    {
                        foreach (GameObject whale in Predators)
                        {
                            Predator predator = whale.GetComponent<Predator>();
                            if (!predator.IsMale)
                                predator.Mate(_bestWhale.gameObject);
                        }
                        _bestWhale.SetAlpha(true);
                        _bestWhale.Energy /= 2;
                        _matingSeason = true;
                        _nextMating += MatingDuration;
                    }
                    else
                    {
                        _nextMating += MatingSeason;
                    }
                }
            }
        }
    }
}