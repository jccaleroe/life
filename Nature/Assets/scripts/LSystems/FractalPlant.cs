using UnityEngine;

namespace LSystems
{
	public class FractalPlant : MonoBehaviour {

		public Mesh mesh;
		public Material material;
		private int _depth;
		public int maxDepth;

		private static Vector3[] childDirections = {
			Vector3.up*2,
			(Vector3.up * 2) + Vector3.right,
			(Vector3.up * 2) - Vector3.right,
			(Vector3.up * 1) + Vector3.right,
			(Vector3.up * 1) - Vector3.right		
		};

		private static Quaternion[] childOrientations = {
			Quaternion.identity,
			Quaternion.Euler(0f, 0f, -25f),
			Quaternion.Euler(0f, 0f, 25f),
			Quaternion.Euler(0f, 0f, -25f),
			Quaternion.Euler(0f, 0f, 25f),
		};
	
		void Start () {
			if (_depth == 0){			
				CreateZero(this, _depth+1, 0);
			}
			else
			{
				gameObject.AddComponent<MeshFilter>().mesh = mesh;
				gameObject.AddComponent<MeshRenderer>().material = material;
			}
		}

		private void CreateZero(FractalPlant parent, int form, int depth){
			maxDepth = parent.maxDepth;
			_depth = depth;				
			if (depth < maxDepth){
				FractalPlant tree1 = new GameObject("Fractal Child").AddComponent<FractalPlant>().CreateOne(parent, 0+form, depth+1);
				CreateZero(tree1, 1, depth+1);
				CreateZero(tree1, 2, depth+1);
				FractalPlant tree2 = new GameObject("Fractal Child").AddComponent<FractalPlant>().CreateOne(tree1, 0+form, depth+1);
				CreateZero(tree2, 1, depth+1);
				FractalPlant tree3 = new GameObject("Fractal Child").AddComponent<FractalPlant>().CreateOne(tree2, 1+form, depth+1);
				CreateZero(tree3, 1, depth+1);
			}
			else if (depth == maxDepth){
				new GameObject("Fractal Child").AddComponent<FractalPlant>().CreateOne(parent, 0, depth+1);
				new GameObject("Fractal Child").AddComponent<FractalPlant>().CreateOne(parent, 0, depth+1);
				new GameObject("Fractal Child").AddComponent<FractalPlant>().CreateOne(parent, 2, depth+1);
			}		
		}
	
		private FractalPlant CreateOne(FractalPlant parent, int form, int depth){
			maxDepth = parent.maxDepth;
			_depth = depth;		
			mesh = parent.mesh;
			material = parent.material;
			transform.parent = parent.transform;		
			transform.localPosition = childDirections[form];
			transform.localRotation = childOrientations[form];
			transform.localScale = Vector3.one;
			if (depth < maxDepth){
				FractalPlant tree = new GameObject("Fractal Child").AddComponent<FractalPlant>().CreateOne(this, 0, depth+1);
				return new GameObject("Fractal Child").AddComponent<FractalPlant>().CreateOne(tree, 0, depth+2);
			}
			return this;
		}
	}
}
