using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
///     Decals when hiting objects
/// </summary>
public class WeaponDecals : MonoBehaviour
{
    [Header("Impact VFX")]
    public GameObject impactUntagged; // Default
    public GameObject impactEnemyFrontSplash; // Enemy Front blood Splash!
    public GameObject impactEnemyBackSplash; // Enemy Back blood Splash
    public GameObject impactDirt; 
    public GameObject impactConcrete; 
    public GameObject impactSand; 
    public GameObject impactMetal; 
    public GameObject impactWood; 
    public GameObject impactGlass; 
    public GameObject impactPlaster; 
    public GameObject impactPlastic; 
    public GameObject impactPaper; 
    public GameObject impactWater; 
    public GameObject impactMud; 
    public GameObject impactStone; 
    public GameObject impactBrick; 
    public GameObject impactFoliage;
    public GameObject impactCloth;

    [Header("ImpactSounds")]
    //[SerializeField] AudioClip genericImpact_Sound;
    [SerializeField] AudioClip untagged_Sound; // Defa
    [SerializeField] AudioClip flesh_Sound;
    [SerializeField] AudioClip headshot_Sound;
    [SerializeField] AudioClip skullImpact_Sound;
    [SerializeField] AudioClip dirt_Sound; 
    [SerializeField] AudioClip concrete_Sound; 
    [SerializeField] AudioClip sand_Sound; 
    [SerializeField] AudioClip metal_Sound; 
    [SerializeField] AudioClip wood_Sound; 
    [SerializeField] AudioClip glass_Sound; 
    [SerializeField] AudioClip plaster_Sound; 
    [SerializeField] AudioClip plastic_Sound; 
    [SerializeField] AudioClip paper_Sound; 
    [SerializeField] AudioClip water_Sound; 
    [SerializeField] AudioClip mud_Sound; 
    [SerializeField] AudioClip stone_Sound; 
    [SerializeField] AudioClip brick_Sound; 
    [SerializeField] AudioClip foliage_Sound;
    [SerializeField] AudioClip cloth_Sound;

    public void Decals(RaycastHit hit)
    {
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            GameObject impactGO = Instantiate(impactUntagged, hit.point, Quaternion.LookRotation(hit.normal));
            impactGO.transform.parent = hit.transform;
            //Destroy(impactGO, 1f);
            SFXManager.PlaySound_AudioMixer(untagged_Sound, hit.point);
        }
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("EnemyBody"))
        {
            GameObject impactGO = Instantiate(impactEnemyFrontSplash, hit.point, Quaternion.LookRotation(hit.normal)); // front blood
            GameObject impactGO2 = Instantiate(impactEnemyBackSplash, hit.point, Quaternion.LookRotation(-hit.normal)); // Back blood
            Destroy(impactGO, 1f);
            Destroy(impactGO2, 2f);
            SFXManager.PlaySound_AudioMixer(flesh_Sound, hit.point);
        }
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("EnemyHead"))
        {
            GameObject impactGO = Instantiate(impactEnemyFrontSplash, hit.point, Quaternion.LookRotation(hit.normal)); // front blood
            GameObject impactGO2 = Instantiate(impactEnemyBackSplash, hit.point, Quaternion.LookRotation(-hit.normal)); // Back blood
            Destroy(impactGO, 1f);
            Destroy(impactGO2, 2f);
            SFXManager.PlaySound_AudioMixer(headshot_Sound, hit.point);
            SFXManager.PlaySound_AudioMixer(skullImpact_Sound, hit.point);
        }
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Wood"))
        {
            GameObject impactGO = Instantiate(impactWood, hit.point, Quaternion.LookRotation(hit.normal));
            impactGO.transform.parent = hit.transform;
            //Destroy(impactGO, 1f);
            SFXManager.PlaySound_AudioMixer(wood_Sound, hit.point);
        }
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Sand"))
        {
            GameObject impactGO = Instantiate(impactSand, hit.point, Quaternion.LookRotation(hit.normal));
            impactGO.transform.parent = hit.transform;
            //Destroy(impactGO, 1f);
            SFXManager.PlaySound_AudioMixer(sand_Sound, hit.point);
        }
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Metal"))
        {
            GameObject impactGO = Instantiate(impactMetal, hit.point, Quaternion.LookRotation(hit.normal));
            impactGO.transform.parent = hit.transform;
            //Destroy(impactGO, 1f);
            SFXManager.PlaySound_AudioMixer(metal_Sound, hit.point);
        }
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Dirt"))
        {
            GameObject impactGO = Instantiate(impactDirt, hit.point, Quaternion.LookRotation(hit.normal));
            impactGO.transform.parent = hit.transform;
            //Destroy(impactGO, 1f);
            SFXManager.PlaySound_AudioMixer(dirt_Sound, hit.point);
        }
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground") || hit.transform.gameObject.layer == LayerMask.NameToLayer("Concrete"))
        {
            GameObject impactGO = Instantiate(impactConcrete, hit.point, Quaternion.LookRotation(hit.normal));
            impactGO.transform.parent = hit.transform;
            //Destroy(impactGO, 1f);
            SFXManager.PlaySound_AudioMixer(concrete_Sound, hit.point);
        }
        // Extras...
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Glass"))
        {
            GameObject impactGO = Instantiate(impactGlass, hit.point, Quaternion.LookRotation(hit.normal));
            impactGO.transform.parent = hit.transform;
            //Destroy(impactGO, 1f);
            SFXManager.PlaySound_AudioMixer(glass_Sound, hit.point);
        }
        //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Plaster"))
        //{
        //    GameObject impactGO = Instantiate(impactPlaster, hit.point, Quaternion.LookRotation(hit.normal));
        //    Destroy(impactGO, 1f);
        //    AudioSource.PlayClipAtPoint(plaster_Sound, hit.point);
        //}
        //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Plastic"))
        //{
        //    GameObject impactGO = Instantiate(impactPlastic, hit.point, Quaternion.LookRotation(hit.normal));
        //    Destroy(impactGO, 1f);
        //    AudioSource.PlayClipAtPoint(plastic_Sound, hit.point);
        //}
        //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Paper"))
        //{
        //    GameObject impactGO = Instantiate(impactPaper, hit.point, Quaternion.LookRotation(hit.normal));
        //    Destroy(impactGO, 1f);
        //    AudioSource.PlayClipAtPoint(paper_Sound, hit.point);
        //}
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            GameObject impactGO = Instantiate(impactWater, hit.point, Quaternion.LookRotation(hit.normal));
            impactGO.transform.parent = hit.transform;
            //Destroy(impactGO, 1f);
            SFXManager.PlaySound_AudioMixer(water_Sound, hit.point);
        }
        //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Mud"))
        //{
        //    GameObject impactGO = Instantiate(impactMud, hit.point, Quaternion.LookRotation(hit.normal));
        //    Destroy(impactGO, 1f);
        //    AudioSource.PlayClipAtPoint(mud_Sound, hit.point);
        //}
        //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Stone"))
        //{
        //    GameObject impactGO = Instantiate(impactStone, hit.point, Quaternion.LookRotation(hit.normal));
        //    Destroy(impactGO, 1f);
        //    AudioSource.PlayClipAtPoint(stone_Sound, hit.point);
        //}
        //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Brick"))
        //{
        //    GameObject impactGO = Instantiate(impactBrick, hit.point, Quaternion.LookRotation(hit.normal));
        //    Destroy(impactGO, 1f);
        //    AudioSource.PlayClipAtPoint(brick_Sound, hit.point);
        //}
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Foliage"))
        {
            GameObject impactGO = Instantiate(impactFoliage, hit.point, Quaternion.LookRotation(hit.normal));
            impactGO.transform.parent = hit.transform;
            //Destroy(impactGO, 1f);
            SFXManager.PlaySound_AudioMixer(foliage_Sound, hit.point);
        }
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Cloth"))
        {
            GameObject impactGO = Instantiate(impactCloth, hit.point, Quaternion.LookRotation(hit.normal));
            impactGO.transform.parent = hit.transform;
            //Destroy(impactGO, 1f);
            SFXManager.PlaySound_AudioMixer(cloth_Sound, hit.point);
        }
    }
}