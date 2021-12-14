using System;
using UnityEngine;

public class GamePool:MonoBehaviour
{
    public GameObject explode_fx_prefab;
    public GameObject default_bullet_fab;
    public Transform fx_root;
    public Transform bullet_root;
    Battle battle;
    void Awake(){
        battle=GetComponent<Battle>();
    }
    public void ShowExplosion(Vector3 position){
        var explodeVFX = Instantiate (explode_fx_prefab, position, Quaternion.identity,fx_root);
        var ps = explodeVFX.GetComponent<ParticleSystem>();
        if (ps != null){
            Destroy (explodeVFX, ps.main.duration);
        }
    }
    public void CreateBullet(string bullet_type, Vector3 position, Vector3 forward, PlayerAttr onwer){
        GameObject bullet_fab=default_bullet_fab;
        if (bullet_type!=""){
            int bullet_id = battle.bullet_id_table[bullet_type];
            bullet_fab=battle.bullet_list[bullet_id].gameObject;
        }
        GameObject p = Instantiate(bullet_fab, bullet_root);
        Bullet b=p.GetComponent<Bullet>();
        b.OnCreate(battle);
        b.Init(onwer, position, forward);
    }
    public void ShowHitFx(Vector3 position, Vector3 forward, GameObject fx_prefab){
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, forward);
        GameObject hitVFX = Instantiate(fx_prefab, position, rot,fx_root);
        ParticleSystem ps = hitVFX.GetComponent<ParticleSystem>();
        if (ps == null){
            var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
            Destroy(hitVFX, psChild.main.duration);
        }else{
            Destroy(hitVFX, ps.main.duration);
        }
    }
    public void ShowMuzzleFx(Vector3 position, Vector3 forward, GameObject fx_prefab){
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, forward);
        var muzzleVFX = Instantiate (fx_prefab, position, Quaternion.identity,fx_root);
        muzzleVFX.transform.forward = forward;
        var ps = muzzleVFX.GetComponent<ParticleSystem>();
        if (ps == null){
            var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
            Destroy(muzzleVFX, psChild.main.duration);
        }else{
            Destroy(muzzleVFX, ps.main.duration);
        }
    }
    public void ClearDynObjs(){
        for (int i =0; i<bullet_root.childCount; i++){
            Transform t_child=bullet_root.GetChild(i);
            Bullet bullet= t_child.GetComponent<Bullet>();
            if (bullet!=null){
                Destroy(t_child.gameObject);
            }
        }
        for (int i =0; i<fx_root.childCount; i++){
            Transform t_child=fx_root.GetChild(i);
            ParticleSystem ps= t_child.GetComponent<ParticleSystem>();
            if (ps!=null){
                Destroy(t_child.gameObject);
            }
        }
    }

    public Transform[] GetAllBullets(){
        Transform[] bullets=new Transform[bullet_root.childCount];
        for (int i=0; i<bullet_root.childCount; i++){
            bullets[i]=bullet_root.GetChild(i);
        }
        return bullets;
    }
}
