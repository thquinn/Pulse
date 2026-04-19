using UnityEngine;

public class DashCooldownScript : MonoBehaviour
{
    public PlayerScript playerScript;
    public SpriteRenderer sr;

    void Update() {
        transform.rotation = Quaternion.identity;
        float cooldown = 1 - playerScript.GetDashCooldown();
        if (cooldown <= 0 || cooldown >= 1) {
            cooldown = -1;
        }
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetFloat("_Revealed", cooldown);
        sr.SetPropertyBlock(propertyBlock);
    }
}
