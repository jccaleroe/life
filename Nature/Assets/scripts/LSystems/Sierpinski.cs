using UnityEngine;

namespace LSystems
{
	public class Sierpinski : MonoBehaviour {

		public Mesh mesh;
		public Material material;
		private int _depth;
		public int maxDepth;

		private static Vector3[] childDirections = {
			Vector3.up*2,
			(Vector3.up * 2) - Vector3.right,
			(Vector3.up * 2) + Vector3.right			
		};

		private static Quaternion[] childOrientations = {
			Quaternion.identity,
			Quaternion.Euler(0f, 0f, 60f),
			Quaternion.Euler(0f, 0f, -60f)			
		};
	
		void Start () {
			if (_depth == 0){
				CreateA(this, 0, _depth+1);			
			}
			gameObject.AddComponent<MeshFilter>().mesh = mesh;
			gameObject.AddComponent<MeshRenderer>().material = material;
		}

		private Sierpinski CreateA(Sierpinski parent, int form, int depth){
			maxDepth = parent.maxDepth;
			_depth = depth;
			if (depth < maxDepth){
				Sierpinski tree = CreateB(parent, form, depth+1);
				Sierpinski tree2 = CreateA(tree, 2, depth+1);
				Sierpinski tree3 = CreateB(tree2, 2, depth+1);
				return tree3;
			} 
			return new GameObject("A"+depth).AddComponent<Sierpinski>().Create(parent, form, depth);
		}

		private Sierpinski CreateB(Sierpinski parent, int form, int depth){
			maxDepth = parent.maxDepth;
			_depth = depth;
			if (depth < maxDepth){
				Sierpinski tree = CreateA(parent, form, depth+1);
				Sierpinski tree2 = CreateB(tree, 1, depth+1);
				Sierpinski tree3 = CreateA(tree2, 1, depth+1);
				return tree3;
			}		 
			return new GameObject("B"+depth).AddComponent<Sierpinski>().Create(parent, form, depth);
		}

		private Sierpinski Create(Sierpinski parent, int form, int depth){
			maxDepth = parent.maxDepth;
			this._depth = depth;		
			mesh = parent.mesh;
			material = parent.material;
			transform.parent = parent.transform;		
			transform.localPosition = childDirections[form];
			transform.localRotation = childOrientations[form];
			transform.localScale = Vector3.one;
			return this;
		}
	}
}
