using UnityEngine;

public class BoidDamageScript : MonoBehaviour
{
    public int hp;

    public void Damage(int amount) {
        hp -= amount;
        if (hp <= 0) {
            Destroy(gameObject);
            PooledParticleScript.Trigger(PooledParticleType.BoidDie, transform.localPosition, Quaternion.identity);
        }
    }
}
