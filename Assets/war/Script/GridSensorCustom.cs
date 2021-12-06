using UnityEngine;
using Unity.MLAgents.Sensors;

public class GridSensorCustom : GridSensorBase
{
    
    static int tag_offset=0;
    static int type_offset=tag_offset+4;
    static int owner_offset=type_offset+7;
    static int hp_offset=owner_offset+1;
    static int mp_offset=hp_offset+1;
    static int spd_dir_offset=mp_offset+1;
    static int spd_mag_offset=spd_dir_offset+1;
    // static int hp_recover_offset=spd_mag_offset+1;
    // static int mp_recover_offset=hp_recover_offset+1;
    // static int def_offset=mp_recover_offset+1;
    // static int atk_offset=def_offset+1;
    // static int luk_offset=atk_offset+1;
    // static int range_offset=luk_offset+1;
    // static int mov_spd_offset=range_offset+1;
    static int px_size=spd_mag_offset+1;
    GameObject owner;

    public GridSensorCustom(
        string name,
        Vector3 cellScale,
        Vector3Int gridSize,
        string[] detectableTags,
        SensorCompressionType compression,
        GameObject owner_
    ) : base(name, cellScale, gridSize, detectableTags, compression)
    {
        owner=owner_;
    }

    protected override int GetCellObservationSize(){
        return px_size;
    }

    float ClampAngle(float angle){
        if (angle<0){
            angle=angle+360f;
        }
        if (angle>360){
            angle=angle-360f;
        }
        return angle;
    }

    protected override void GetObjectData(GameObject detectedObject, int tagIndex, float[] dataBuffer){
        for (int i=0; i<px_size; i++){
            dataBuffer[i]=0;
        }
        if (detectedObject.tag=="bullet"){
            Bullet bullet=detectedObject.GetComponent<Bullet>();
            int bul_buf_id = (int)bullet.onwer.bullet_buf;
            dataBuffer[tag_offset+0]=1;
            dataBuffer[type_offset+bul_buf_id]=1;
            Rigidbody rigid=detectedObject.GetComponent<Rigidbody>();
            if (detectedObject==owner){
                dataBuffer[owner_offset]=1;
            }
            dataBuffer[spd_mag_offset]=rigid.velocity.magnitude/40f;
            if (dataBuffer[spd_mag_offset]>1){
                dataBuffer[spd_mag_offset]=1;
            }
            dataBuffer[spd_dir_offset]=ClampAngle(detectedObject.transform.eulerAngles.y)/360f;
        }else if (detectedObject.tag=="Player"){
            TankAgent1 tank=detectedObject.GetComponent<TankAgent1>();
            Rigidbody rigid=detectedObject.GetComponent<Rigidbody>();
            dataBuffer[type_offset+tank.player_id]=1;
            dataBuffer[owner_offset]=1;
            dataBuffer[tag_offset+1]=1;
            dataBuffer[spd_mag_offset]=rigid.velocity.magnitude/40f;
            if (dataBuffer[spd_mag_offset]>1){
                dataBuffer[spd_mag_offset]=1;
            }
            dataBuffer[spd_dir_offset]=ClampAngle(detectedObject.transform.eulerAngles.y)/360f;
            dataBuffer[hp_offset]=(float)tank.hp/(float)tank.max_hp;
            dataBuffer[mp_offset]=(float)tank.mp/(float)tank.max_mp;
            // dataBuffer[hp_recover_offset]=(float)tank.hp_recover/20f;
            // dataBuffer[mp_recover_offset]=(float)tank.mp_recover/10f;
            // dataBuffer[def_offset]=tank.def;
            // dataBuffer[atk_offset]=(float)tank.atk/20f;
            // dataBuffer[luk_offset]=tank.luk;
            // dataBuffer[range_offset]=(float)tank.range/10f;
            // dataBuffer[mov_spd_offset]=(float)tank.mov_spd/10f;
        }else if (detectedObject.tag=="wall"){
            dataBuffer[tag_offset+2]=1;
        }else if (detectedObject.tag=="collect"){
            Collectable1 collectable1=detectedObject.GetComponent<Collectable1>();
            dataBuffer[type_offset+(int)collectable1.collectable_type]=1;
            dataBuffer[tag_offset+3]=1;
        }
    }

    protected override bool IsDataNormalized(){
        return true;
    }

}
