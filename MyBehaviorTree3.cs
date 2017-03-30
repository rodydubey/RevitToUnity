using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;

public class MyBehaviorTree3 : MonoBehaviour
{
	public GameObject wander1;	
	public GameObject participant;
    private BehaviorAgent behaviorAgent;
    public bool waveFlag = true;
	// Use this for initialization
	void Start ()
	{
		behaviorAgent = new BehaviorAgent (this.BuildTreeRoot ());
		BehaviorManager.Instance.Register (behaviorAgent);
		behaviorAgent.StartBehavior ();
    }

	
	// Update is called once per frame
    void Update ()
    {
        
    }

    protected Node ST_ApproachAndWait(Transform target)
    {
	    Val<Vector3> position = Val.V (() => target.position);
	    return new Sequence( participant.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(position,2), new LeafWait(1000));
    }

    protected Node ST_FaceAcknowledge(Transform target, bool flag)
    {
        //Val<Vector3> position = Val.V(() => target.position);       
        return new Sequence(participant.GetComponent<BehaviorMecanim>().Node_FaceAnimation("ACKNOWLEDGE", flag), new LeafWait(1000));
    }
    protected Node ST_FaceAcknowledgeWander(Transform target, bool flag)
    {
        //Val<Vector3> position = Val.V(() => target.position);       
        return new Sequence(wander1.GetComponent<BehaviorMecanim>().Node_FaceAnimation("ACKNOWLEDGE", flag), new LeafWait(1000));
    }
    protected Node ST_OrientParticipant(Transform target)
    {
        Val<Vector3> position = Val.V(() => target.position);
        return new Sequence(participant.GetComponent<BehaviorMecanim>().Node_OrientTowards(position), new LeafWait(1000));
    }
    protected Node ST_HeadShake(Transform target, bool flag)
    {
        //Val<Vector3> position = Val.V(() => target.position);       
        return new Sequence(wander1.GetComponent<BehaviorMecanim>().Node_FaceAnimation("ACKNOWLEDGE", flag), new LeafWait(1000));
    }
    protected Node ST_HandWaveWander(Transform target, bool flag)
    {
        //Val<Vector3> position = Val.V(() => target.position);      
        return new Sequence(wander1.GetComponent<BehaviorMecanim>().Node_HandAnimation("POINTING", flag), new LeafWait(1000));
    }
    protected Node ST_HandWaveParticipent(Transform target, bool flag)
    {
        //Val<Vector3> position = Val.V(() => target.position);      
        return new Sequence(participant.GetComponent<BehaviorMecanim>().Node_HandAnimation("WAVE", flag), new LeafWait(1000));
    }
 
    protected Node BuildTreeRoot()
    {
        var distance = Vector3.Distance(wander1.transform.position, participant.transform.position);
        Func<bool> act = () => (waveFlag);
        Node roaming = new DecoratorLoop(1,
                        new Sequence(
                        this.ST_ApproachAndWait(this.wander1.transform)));
        return roaming;
        
        //Node trigger = new DecoratorLoop(new LeafAssert(act));
        //Node root = new DecoratorLoop(new DecoratorForceStatus(RunStatus.Success, new SequenceParallel(trigger, roaming)));
        //return root;
    }
}