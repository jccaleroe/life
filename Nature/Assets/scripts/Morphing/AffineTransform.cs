using UnityEngine;

namespace Morphing
{
    public class AffineTransform : MonoBehaviour
    {

        public float[] Xs = {1, 1, 1};
        public float[] Ys = {1, 1, 1};
        public float[] Zs = {1, 1, 1};
        public float Rate = 0.5f;
        private float _nextTime;        
    
        private void Update(){
            if (Time.time >= _nextTime)
            {
                _nextTime = Time.time + Rate;
                Mesh mesh= GetComponent<MeshFilter>().mesh;
                Vector3[] vertices = mesh.vertices;                
                for (int i = 0; i < vertices.Length; i++)
                {
                    Vector3 a = vertices[i];
                    vertices[i] = new Vector3(
                        a.x + Random.Range(-0.16f, 0.16f)*Xs[0] + Xs[1]*Random.Range(-1,1)*a.y + Xs[2]*Random.Range(-1,1)*a.z,
                        a.y + Random.Range(-0.16f, 0.16f)*Ys[0] + Ys[1]*Random.Range(-1,1)*a.x + Ys[2]*Random.Range(-1,1)*a.z,
                        a.z + Random.Range(-0.16f, 0.16f)*Zs[0] + Zs[1]*Random.Range(-1,1)*a.x + Zs[2]*Random.Range(-1,1)*a.y);
                }
                mesh.vertices = vertices;
                mesh.RecalculateBounds();
            }
        }
    }
}