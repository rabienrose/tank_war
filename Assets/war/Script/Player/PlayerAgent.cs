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

    void Start () {
        attr=GetComponent<PlayerAttr>();
    }

    public override void OnEpisodeBegin(){
        attr.ResetAttr();
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        if(attr.dead){
            return;
        }
        // GiveRewardOnLoc();
        float[] obs = attr.GetPlayerObs();
        PlayerAttr[] players = battle.GetAllPlayers();
        for (int i=0; i<players.Length; i++){
            if (players[i]==attr){
                continue;
            }
            obs = players[i].GetPlayerObs();
            sensor.AddObservation(obs);
        }
    }
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        if (attr.dead){
            return;
        }
        if (attr.mp<10){
            actionMask.SetActionEnabled(0, 1, false);
        }
    }
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
