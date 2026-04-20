using UnityEngine;

public class BoidDamageScript : MonoBehaviour
{
    public int hp;

    public void Damage(int amount) {
        hp -= amount;
        if (hp <= 0) {
            Die();
        }
    }
    public void Die(bool particles = true) {
        Destroy(gameObject);
        if (particles) {
            PooledParticleScript.Trigger(PooledParticleType.BoidDie, transform.localPosition, Quaternion.identity);
        }
    }
}
