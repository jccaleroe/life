using UnityEngine;

namespace LSystems
{
    public class FractalBinaryTree : MonoBehaviour
    {
        public Mesh Mesh;
        public Material Material;
        private int _depth;
        public int MaxDepth;

        private static readonly Vector3[] ChildDirections =
        {
            Vector3.up * 2,
            (Vector3.up * 2f) + Vector3.right,
            (Vector3.up * 2f) - Vector3.right,
            (Vector3.up * 1) + Vector3.right,
            (Vector3.up * 1) - Vector3.right
        };

        private static readonly Quaternion[] ChildOrientations =
        {
            Quaternion.identity,
            Quaternion.Euler(0f, 0f, -45f),
            Quaternion.Euler(0f, 0f, 45f),
            Quaternion.Euler(0f, 0f, -45f),
            Quaternion.Euler(0f, 0f, 45f)
        };

        private void Start()
        {
            if (_depth == 0)
            {
                FractalBinaryTree tree = new GameObject("Fractal Child").AddComponent<FractalBinaryTree>()
                    .CreateOne(this, 0, _depth + 1);
                CreateZero(tree, _depth + 1);
            }

            gameObject.AddComponent<MeshFilter>().mesh = Mesh;
            gameObject.AddComponent<MeshRenderer>().material = Material;
        }

        private void CreateZero(FractalBinaryTree parent, int depth)
        {
            MaxDepth = parent.MaxDepth;
            _depth = depth;
            if (depth < MaxDepth)
            {
                FractalBinaryTree tree = new GameObject("Fractal Child").AddComponent<FractalBinaryTree>()
                    .CreateOne(parent, 3, depth + 1);
                CreateZero(tree, depth + 1);
                FractalBinaryTree tree2 = new GameObject("Fractal Child").AddComponent<FractalBinaryTree>()
                    .CreateOne(parent, 4, depth + 1);
                CreateZero(tree2, depth + 1);
            }
            else if (depth == MaxDepth)
            {
                new GameObject("Fractal Child").AddComponent<FractalBinaryTree>().CreateOne(parent, 0, depth + 1);
                new GameObject("Fractal Child").AddComponent<FractalBinaryTree>().CreateOne(parent, 1, depth + 1);
                new GameObject("Fractal Child").AddComponent<FractalBinaryTree>().CreateOne(parent, 2, depth + 1);
            }
        }

        private FractalBinaryTree CreateOne(FractalBinaryTree parent, int form, int depth)
        {
            MaxDepth = parent.MaxDepth;
            _depth = depth;
            Mesh = parent.Mesh;
            Material = parent.Material;
            transform.parent = parent.transform;
            transform.localPosition = ChildDirections[form];
            transform.localRotation = ChildOrientations[form];
            transform.localScale = Vector3.one;
            if (depth < MaxDepth)
            {
                FractalBinaryTree tree = new GameObject("Fractal Child").AddComponent<FractalBinaryTree>()
                    .CreateOne(this, 0, depth + 1);
                return new GameObject("Fractal Child").AddComponent<FractalBinaryTree>().CreateOne(tree, 0, depth + 2);
            }

            return this;
        }
    }
}