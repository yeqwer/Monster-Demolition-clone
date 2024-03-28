using System.Collections;
using UnityEngine;

namespace VoxelDestruction
{
    public class Demo_Bomb : MonoBehaviour
    {
        public float radius;
        public float force;
        public float overrideMax;

        public float triggerDelay;
        public ParticleSystem particle;

        private void Start()
        {
            StartCoroutine(CountDown());
        }

        private IEnumerator CountDown()
        {
            yield return new WaitForSeconds(triggerDelay);
        
            Explode();
        }
    
        private void Explode()
        {
            particle.Play();
            particle.transform.parent = null;
            Destroy(particle.gameObject, 2f);
            
            Collider[] collider = Physics.OverlapSphere(transform.position, radius);

            for (int i = 0; i < collider.Length; i++)
            {
                if (collider[i].transform.GetComponentInParent<VoxelObject>())
                {
                    collider[i].transform.GetComponentInParent<VoxelObject>().AddDestruction(force, 
                        transform.position, 
                        -(transform.position - collider[i].transform.position).normalized, 
                        overrideMax);
                }

                if (collider[i].transform.GetComponentInParent<Rigidbody>() && collider[i].transform.root != transform)
                {
                    collider[i].transform.GetComponentInParent<Rigidbody>().AddForce(-force * 
                        (transform.position - collider[i].transform.position).normalized 
                        * (1 / Vector3.Distance(transform.position, collider[i].transform.position)), ForceMode.Impulse);
                }
            }
        
            Destroy(gameObject);
        }
    }
}
