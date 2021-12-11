#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class PlayerAgent : Agent
{
    public Battle battle;
    PlayerAttr attr;
    PlayerAction player_action;
    int debug_count=0;

    void Start () {
        attr=GetComponent<PlayerAttr>();
        player_action=GetComponent<PlayerAction>();
    }

    public override void OnEpisodeBegin(){
        attr.ResetAttr();
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        float[] obs = attr.GetPlayerObs(attr);
        sensor.AddObservation(obs);
        PlayerAttr[] players = battle.GetAllPlayers();
        for (int i=0; i<players.Length; i++){
            if (players[i]==attr){
                continue;
            }
            obs = players[i].GetPlayerObs(attr);
            sensor.AddObservation(obs);
        }
        sensor.AddObservation(player_action.GetBulletObs());        
        sensor.AddObservation(player_action.GetCollectableObs());
        // float[] ret=battle.GetCollectableObs();
        // debug_count=debug_count+1;
        // if (gameObject.name=="Red" && debug_count%5==0){
        //     string debug_str="";
        //     for (int i=0; i<ret.Length;i++){
        //         debug_str=debug_str+" "+ret[i].ToString();
        //     }
        //     Debug.Log(debug_str);
        // }
    }
    // public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    // {
    //     if (attr.mp<10){
    //         actionMask.SetActionEnabled(0, 1, false);
    //     }
    // }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        PlayerAction.Action act = new PlayerAction.Action();
        act.mov=(PlayerAction.Action.DIR)actionBuffers.DiscreteActions[0];
        act.fire=actionBuffers.DiscreteActions[1];
        player_action.OnAction(act);   
    }
    
    public void AddRewardCustom(float inc, string desc){
        AddReward(inc);
        // Debug.Log(name+": "+Math.Round(inc,4)+"/"+desc+" ( "+Math.Round(GetCumulativeReward(), 2)+" )");
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        PlayerAction.Action action = player_action.GetAction();
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = (int)action.mov;
        discreteActionsOut[1] = action.fire;
    }
    
}
