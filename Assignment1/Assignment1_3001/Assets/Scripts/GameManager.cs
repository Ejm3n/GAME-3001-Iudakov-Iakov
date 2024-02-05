using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[System.Serializable]
public class PresetPositions
{
    public Vector2 ActorPosition;
    public Vector2 TargetPosition;
    public bool Obstacle;
   public PresetPositions(Vector2 ActorPosition, Vector2 TargetPosition, bool Obstacle) 
    { 
        this.ActorPosition = ActorPosition;
        this.TargetPosition = TargetPosition;
        this.Obstacle = Obstacle;
    }
}


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] GameObject Actor;
    [SerializeField] GameObject Target;
    [SerializeField] GameObject Obstacle;
    Dictionary<ActorState, PresetPositions> actorPresets = new Dictionary<ActorState, PresetPositions> {
    {ActorState.Seeking,new PresetPositions(new Vector2(-9,-3.5f),new Vector2(9,3),false) },
    {ActorState.Arrival,new PresetPositions(new Vector2(-5,-0.5f),new Vector2(9,2),false) },
    {ActorState.Fleeing,new PresetPositions(new Vector2(0,0f),new Vector2(0,2),false) },
    {ActorState.Avoidance,new PresetPositions(new Vector2(-8,3f),new Vector2(9,0),true) }};
    private ActorMovement actorMovement;
    private Quaternion actorRotation;
    private void Awake()
    {
        if(Instance==null)
            Instance = this;
        else
            Destroy(this);
        actorMovement = Actor.GetComponent<ActorMovement>();
        actorRotation = Actor.transform.rotation;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            CreateScene(ActorState.Seeking);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            CreateScene(ActorState.Fleeing);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CreateScene(ActorState.Arrival);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CreateScene(ActorState.Avoidance);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
           ResetScene();
        }
    }
    private void CreateScene(ActorState actorState)
    {
        Obstacle.SetActive(actorPresets[actorState].Obstacle);
        Target.transform.position = actorPresets[actorState].TargetPosition;
        Target.SetActive(true);
        Actor.SetActive(true);
        actorMovement.UpdateActor(actorState, Target.transform);
        Actor.transform.position = actorPresets[actorState].ActorPosition;
        Actor.transform.rotation = actorRotation;
        
     
        SoundManager.Instance.PlaySound("OnClick");
        SoundManager.Instance.PlaySound("EngineStart");
    }
    private void ResetScene()
    {
        Actor.SetActive(false);
        Target.SetActive(false);
        Obstacle.SetActive(false);
    }
}
