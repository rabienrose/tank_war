using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody rigidb;
    [HideInInspector]
    public TankAgent1 onwer;
    float fly_distance=0;
    Vector3 last_posi;
    Vector3 lastBouncePos;
    SphereCollider sphereCol;
    int bounce_count=0;
    public GameObject HitFx;
    public GameObject MuzzleFx;

    void Start(){
        rigidb= GetComponent<Rigidbody>();
        last_posi=transform.localPosition;
        lastBouncePos=transform.localPosition;
        sphereCol=GetComponent<SphereCollider>();
        rigidb.velocity = transform.forward * onwer.bullet_spd;
        PlayMuzzleFx();
    }
    public void OnCreate(TankAgent1 onwer_){
        onwer=onwer_;
    }

    void FixedUpdate(){
        float t_dis=(last_posi-transform.localPosition).magnitude;
        last_posi=transform.localPosition;
        fly_distance=fly_distance+t_dis;
        if (fly_distance>onwer.range){
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter (Collider co) {
        if (onwer.dead==true){
            return;
        }
        GameObject obj = co.gameObject;
        if (obj.tag=="wall"){
            if (bounce_count>=onwer.bounce_count){
                Destroy(gameObject);
            }else{
                PlayHitFx();
                if(rigidb!=null){
                    Ray ray = new Ray(lastBouncePos - transform.forward * 0.5f, transform.forward);
                    RaycastHit hit;
                    if(Physics.SphereCast(ray, sphereCol.radius, out hit, Mathf.Infinity, 1 << 0))
                    {
                        if (Vector3.Distance(transform.position, lastBouncePos) > 0.05f){
                            lastBouncePos = hit.point;
                            bounce_count++;
                            Vector3 dir = Vector3.Reflect(ray.direction, hit.normal);
                            transform.position = hit.point;
                            transform.rotation = Quaternion.LookRotation(dir);
                            rigidb.velocity = transform.forward * onwer.bullet_spd;
                        }
                    }
                }
            }
        }
        if (obj.tag=="Player"){
            TankAgent1 tar_tank = obj.GetComponent<TankAgent1>();
            if (tar_tank!=onwer && tar_tank.dead==false){
                PlayHitFx();
                Destroy(gameObject);
                int damage=onwer.atk;
                float o_rand = UnityEngine.Random.value;
                float t_rand = UnityEngine.Random.value;
                if (o_rand<onwer.luk){
                }else{
                    damage=(int)(damage*(1-tar_tank.def));
                }
                if (t_rand<tar_tank.luk){
                    damage=0;
                }
                tar_tank.ApplyDamage(damage, onwer);
                o_rand = UnityEngine.Random.value;
                t_rand = UnityEngine.Random.value;
                if (o_rand<onwer.luk && t_rand>tar_tank.luk){
                    tar_tank.AddBuf(onwer.bullet_buf,onwer.bullet_buf_time);
                }
            }
        }
    }
    void PlayHitFx(){
        if (onwer.battle.TrainMode==true){
            return;
        }
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, transform.forward);
        var hitVFX = Instantiate(HitFx, transform.localPosition, rot,transform.parent) as GameObject;
        var ps = hitVFX.GetComponent<ParticleSystem>();
        if (ps == null){
            var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
            Destroy(hitVFX, psChild.main.duration);
        }else{
            Destroy(hitVFX, ps.main.duration);
        }
    }
    void PlayMuzzleFx(){
        if (onwer.battle.TrainMode==true){
            return;
        }
        var muzzleVFX = Instantiate (MuzzleFx, transform.position, Quaternion.identity,transform.parent);
        muzzleVFX.transform.forward = gameObject.transform.forward;
        var ps = muzzleVFX.GetComponent<ParticleSystem>();
        if (ps != null){
            Destroy (muzzleVFX, ps.main.duration);
        }else{
            var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
            Destroy (muzzleVFX, psChild.main.duration);
        }
    }
}
