using UnityEngine;

namespace TuringPattern
{
    public class InhibitorActivator : MonoBehaviour
    {
        [Range(2, 512)] public int Resolution = 256;
        public Gradient Coloring;

        public int Steps = 100;
        public float Beta = 10f, Dx = 1, Dt = 0.001f, Da = 1, Db = 100, UpdateRate = 1f;
        public float MinAlpha = -5f, MaxAlpha = 5f, ColorTime = 2f;
        public bool IsAnimated = true;

        private Texture2D _texture;
        private float[,] _activator, _inhibitor;
        private int _c;
        private bool _isFirst = true;
        private float _nextTime, _alpha;

        public void SetAlpha(float alpha)
        {
            _alpha = alpha;
        }
        
        public float GetAlpha()
        {
            return _alpha;
        }

        private void Awake()
        {
            _alpha = Random.Range(MinAlpha, MaxAlpha);
            if (Time.deltaTime >= 0.03)
                _isFirst = true;
            else if (Time.time >= ColorTime)
                _isFirst = false;
            if (_isFirst) return;
            _texture = new Texture2D(Resolution, Resolution, TextureFormat.RGB24, true)
            {
                name = "InhivatorActivator"
            };

            if (IsAnimated)
                GetComponent<SkinnedMeshRenderer>().material.mainTexture = _texture;
            else
                GetComponent<MeshRenderer>().material.mainTexture = _texture;
            InitalizeMorphogens();
        }

        private void InitalizeMorphogens()
        {
            _activator = new float[Resolution, Resolution];
            _inhibitor = new float[Resolution, Resolution];
            for (int i = 0; i < Resolution; i++)
            {
                for (int j = 0; j < Resolution; j++)
                {
                    _activator[i, j] = Random.value;
                    _inhibitor[i, j] = Random.value;
                }
            }
        }

        private int[] GetRolledIndices(int i, int j, int h, int v)
        {
            int a = i + h;
            int b = j + v;
            if (a < 0) a = Resolution - 1;
            else if (a >= Resolution) a = 0;

            if (b < 0) b = Resolution - 1;
            else if (b >= Resolution) b = 0;
            return new[] {a, b};
        }

        private float Laplacian(int i, int j, float[,] morphogen)
        {
            float cnt = 0f;
            int[] tmp = GetRolledIndices(i, j, 1, 0);
            cnt += morphogen[tmp[0], tmp[1]];
            tmp = GetRolledIndices(i, j, -1, 0);
            cnt += morphogen[tmp[0], tmp[1]];
            tmp = GetRolledIndices(i, j, 0, 1);
            cnt += morphogen[tmp[0], tmp[1]];
            tmp = GetRolledIndices(i, j, 0, -1);
            cnt += morphogen[tmp[0], tmp[1]];
            return (cnt - 4 * morphogen[i, j]) / Mathf.Pow(Dx, 2);
        }

        private void Update()
        {
            if (_isFirst || _c >= Steps || Time.time < _nextTime) return;
            //if (_c >= Steps) return;
            float[,] tmpA = new float[Resolution, Resolution];
            float[,] tmpB = new float[Resolution, Resolution];
            for (int i = 0; i < Resolution; i++)
            for (int j = 0; j < Resolution; j++)
            {
                tmpA[i, j] = _activator[i, j] +
                             Dt * (Da * Laplacian(i, j, _activator) +
                                   FitzNagumoA(_activator[i, j], _inhibitor[i, j]));
                tmpB[i, j] = _inhibitor[i, j] +
                             Dt * (Db * Laplacian(i, j, _inhibitor) +
                                   FitzNagumoB(_activator[i, j], _inhibitor[i, j]));
            }

            _activator = tmpA;
            _inhibitor = tmpB;
            for (int i = 0; i < Resolution; i++)
            for (int j = 0; j < Resolution; j++)
                _texture.SetPixel(j, i, Coloring.Evaluate(_activator[i, j]));

            _texture.Apply();
            _c++;
            _nextTime += UpdateRate;
        }

        private float FitzNagumoA(float a, float b)
        {
            return a - Mathf.Pow(a, 3) - b + _alpha;
        }

        private float FitzNagumoB(float a, float b)
        {
            return Beta * (a - b);
        }
    }
}