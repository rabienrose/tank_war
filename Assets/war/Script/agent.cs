using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class agent : Agent
{
    int total_count=0;
    int stats_count=0;
    int ok_count=0;
    int ep_step=0;
    Rigidbody rBody;
    float target_speed=3;
    // float temp_cul_time=0;
    void Start () {
        rBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate(){
        // temp_cul_time=temp_cul_time+Time.fixedDeltaTime;
        // if (temp_cul_time>1){
        //     temp_cul_time=0;
        //     random_update_target_posi();
        // }
    }

    void random_update_target_posi(){
        Vector3 new_posi =Target.localPosition;
        for(int i=0; i<10; i++){
            float angle_r = Random.value*3.1415926f*2;
            Vector3 angle_dir = new Vector3(Mathf.Cos(angle_r),0, Mathf.Sin(angle_r));
            new_posi =Target.localPosition+angle_dir*target_speed;
            if (Mathf.Abs(new_posi.x)<5 && Mathf.Abs(new_posi.z)<5){
                Target.localPosition=new_posi;
                break;
            }
        }
    }

    public Transform Target;
    public override void OnEpisodeBegin()
    {
        if (this.transform.localPosition.y < 0)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3( 0, 0.5f, 0);
        }
        Target.localPosition = new Vector3(Random.value * 20 - 10, 0.5f, Random.value * 20 - 10);
        if (stats_count%100==0 && total_count!=0){
            float s_rate=ok_count/(float)total_count;
            int avg_step=ep_step/100;
            Debug.Log("Success rate: "+s_rate+" avg steps：  "+avg_step);
            total_count=0;
            ok_count=0;
            ep_step=0;
        }
        total_count=total_count+1;
        stats_count=stats_count+1;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    public float forceMultiplier = 15;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);
        SetReward(-0.003f);
        ep_step=ep_step+1;
        // Reached target
        if (distanceToTarget < 1.42f)
        {
            // Debug.Log("get me!!");
            ok_count=ok_count+1;
            SetReward(1.0f);
            EndEpisode();
        }

        else if (this.transform.localPosition.y < 0)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
