using UnityEngine;

public class ShellExplosion : MonoBehaviour
{            
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;              

	private LayerMask m_TankMask;
	public ParticleSystem m_ExplosionParticles;
	private AudioSource m_ExplosionAudio;  

    private void Start()
    {
		m_TankMask = LayerMask.GetMask ("Players");
		//m_ExplosionParticles = this.GetComponent<ParticleSystem> ();
		m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource> ();

        Destroy(gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.
		Collider[] colls = Physics.OverlapSphere(this.transform.position, m_ExplosionRadius, m_TankMask);
		for (int i = 0; i < colls.Length; i++) {
			Rigidbody targetRigidbody = colls [i].GetComponent<Rigidbody> ();
			if (!targetRigidbody)
				continue;
			targetRigidbody.AddExplosionForce (m_ExplosionForce, this.transform.position, m_ExplosionRadius);
			TankHealth tankhealth = targetRigidbody.GetComponent<TankHealth> ();
			float damage = CalculateDamage (targetRigidbody.position);
			tankhealth.TakeDamage (damage);
		}
		m_ExplosionParticles.transform.parent = null;
		m_ExplosionParticles.Play ();
		m_ExplosionAudio.Play ();
		Destroy (m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
		Destroy (this.gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
		Vector3 explosionDirection = targetPosition - this.transform.position;
		float exploseDistance = explosionDirection.magnitude;
		float relativeDistance = (m_ExplosionRadius - exploseDistance) / m_ExplosionRadius;
		float damage = relativeDistance * m_MaxDamage;
		damage = Mathf.Max (0f, damage);
		return damage;
    }
}