using UnityEngine;

public class MeleeAnimEventHandler : MonoBehaviour
{
    public void CheckForHit()
    {
        GetComponentInParent<Weapon>().CheckForHit();
    }
    public void ExecuteHit()
    {
        GetComponentInParent<Weapon>().ExecuteHit();
    }
    public void PlaySwingSound()
    {
        GetComponentInParent<Weapon>().GetComponentInChildren<AudioSource>().PlayOneShot(GetComponentInParent<Weapon>().weaponData.reloadSound);
    }
    public void PlayHitSound()
    {
        GetComponentInParent<Weapon>().GetComponentInChildren<AudioSource>().PlayOneShot(GetComponentInParent<Weapon>().weaponData.shootSound);
    }
}
