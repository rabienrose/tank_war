#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
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
        // for (int i=0; i<px_size; i++){
        //     dataBuffer[i]=0;
        // }
        // if (detectedObject.tag=="bullet"){
        //     Bullet bullet=detectedObject.GetComponent<Bullet>();
        //     int bul_buf_id = (int)bullet.onwer.bullet_buf;
        //     dataBuffer[tag_offset+0]=1;
        //     dataBuffer[type_offset+bul_buf_id]=1;
        //     Rigidbody rigid=detectedObject.GetComponent<Rigidbody>();
        //     if (detectedObject==owner){
        //         dataBuffer[owner_offset]=1;
        //     }
        //     dataBuffer[spd_mag_offset]=rigid.velocity.magnitude/40f;
        //     if (dataBuffer[spd_mag_offset]>1){
        //         dataBuffer[spd_mag_offset]=1;
        //     }
        //     dataBuffer[spd_dir_offset]=ClampAngle(detectedObject.transform.eulerAngles.y)/360f;
        // }else if (detectedObject.tag=="Player"){
        //     PlayerAttr tank=detectedObject.GetComponent<PlayerAttr>();
        //     Rigidbody rigid=detectedObject.GetComponent<Rigidbody>();
        //     dataBuffer[type_offset+tank.player_id]=1;
        //     dataBuffer[owner_offset]=1;
        //     dataBuffer[tag_offset+1]=1;
        //     dataBuffer[spd_mag_offset]=rigid.velocity.magnitude/40f;
        //     if (dataBuffer[spd_mag_offset]>1){
        //         dataBuffer[spd_mag_offset]=1;
        //     }
        //     dataBuffer[spd_dir_offset]=ClampAngle(detectedObject.transform.eulerAngles.y)/360f;
        //     dataBuffer[hp_offset]=(float)tank.hp/(float)tank.max_hp;
        //     dataBuffer[mp_offset]=(float)tank.mp/(float)tank.max_mp;
        // }else if (detectedObject.tag=="wall"){
        //     dataBuffer[tag_offset+2]=1;
        // }else if (detectedObject.tag=="collect"){
        //     Collectable Collectable=detectedObject.GetComponent<Collectable>();
        //     dataBuffer[type_offset+(int)Collectable.collectable_type]=1;
        //     dataBuffer[tag_offset+3]=1;
        // }
    }

    protected override bool IsDataNormalized(){
        return true;
    }

}
