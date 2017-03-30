using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using TreeSharpPlus;

public class westgatedemo : MonoBehaviour
{
    public GameObject participant1; //rohit blue
    public GameObject participant2; //Daniel red
    public GameObject participant3; // Shan green
    public Transform wander1;
    public Transform wander2;
    public Transform information;
    private BehaviorAgent behaviorAgent;
    public bool waveFlag = true;
    // Use this for initialization
    void Start()
    {
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
    }


    // Update is called once per frame
    void Update()
    {

    }

    //protected Node BuildMainTreeRoot()
    //{
    //    return new Sequence(this.ST_ApproachAndWait(this.participant2.transform),
    //        this.ST_OrientParticipant(this.participant2.transform),
    //        new LeafWait(4500),
    //        this.ST_Talk_Participant(this.participant1.transform, "Hello! I am Rohit"),
    //        new LeafWait(45000));
    //        //this.ST_Talk_Participant2(this.participant2.transform, "Hello! I am Daniel"));
    //}
    protected Node ST_ApproachAndWait(Transform source, Transform target)
    {
        Val<Vector3> position = Val.V(() => target.position);
        return new Sequence(source.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(position, 2), new LeafWait(1000));
        
    }

    protected Node ST_FaceAcknowledge(Transform target, bool flag)
    {
        //Val<Vector3> position = Val.V(() => target.position);    

        return new Sequence(target.GetComponent<BehaviorMecanim>().Node_FaceAnimation("ACKNOWLEDGE", flag), new LeafWait(1000));
    }
    //protected Node ST_Talk_Participant(Transform target, String text)
    //{
    //    //Val<Vector3> position = Val.V(() => target.position);

    //    return
    //            new SequenceParallel(this.ST_FaceAcknowledge(target.transform,true),
    //            this.ST_Talk(target.transform, text));
    //}
    protected Node ST_Talk_Participant(Transform target, String text, bool flag)
    {
        //Val<Vector3> position = Val.V(() => target.position);
        return new Sequence(this.ST_Talk(target.transform, text, flag));
    }

    protected Node ST_Talk(Transform target, String text, bool flag)
    {
        //InteractiveObject3D io3d = target.GetComponent<InteractiveObject3D>();
        //io3d.messageCanvas.enabled = flag;
        //Text footext = io3d.fooText.GetComponent<Text>();
        //footext.text = text;
        return new Sequence(new LeafWait(1000));
    }
    protected Node ST_Talk_Participant2(String text)
    {
        //Val<Vector3> position = Val.V(() => target.position);

        //InteractiveObject3D io3d = participant2.GetComponent<InteractiveObject3D>();
        //io3d.messageCanvas.enabled = true;
        //Text footext = io3d.fooText.GetComponent<Text>();
        //footext.text = text;
        return new Sequence(participant2.GetComponent<BehaviorMecanim>().Node_FaceAnimation("ACKNOWLEDGE", true), new LeafWait(1000));
    }
    protected Node stopTextCanvas()
    {
        //InteractiveObject3D io3d = participant1.GetComponent<InteractiveObject3D>();
        //io3d.messageCanvas.enabled = false;
        //InteractiveObject3D io3d2 = participant2.GetComponent<InteractiveObject3D>();
        //io3d2.messageCanvas.enabled = false;
        return new Sequence(new LeafWait(1000));
    }
    //protected Node ST_FaceAcknowledgeWander(Transform target, bool flag)
    //{
    //    //Val<Vector3> position = Val.V(() => target.position);       
    //    return new Sequence(wander1.GetComponent<BehaviorMecanim>().Node_FaceAnimation("ACKNOWLEDGE", flag), new LeafWait(1000));
    //}
    protected Node ST_OrientParticipant(Transform source,Transform target)
    {
        Val<Vector3> position = Val.V(() => target.position);
        return new Sequence(source.GetComponent<BehaviorMecanim>().Node_OrientTowards(position));
    }
    //protected Node ST_HeadShake(Transform target, bool flag)
    //{
    //    //Val<Vector3> position = Val.V(() => target.position);       
    //    return new Sequence(wander1.GetComponent<BehaviorMecanim>().Node_FaceAnimation("ACKNOWLEDGE", flag), new LeafWait(1000));
    //}
    protected Node ST_HandWaveWander(Transform target, bool flag)
    {
        //Val<Vector3> position = Val.V(() => target.position);      
        return new Sequence(target.GetComponent<BehaviorMecanim>().Node_HandAnimation("POINTING", flag), new LeafWait(1000));
    }
    //protected Node ST_HandWaveParticipent(Transform target, bool flag)
    //{
    //    //Val<Vector3> position = Val.V(() => target.position);      
    //    return new Sequence(participant.GetComponent<BehaviorMecanim>().Node_HandAnimation("WAVE", flag), new LeafWait(1000));
    //}
    protected Node goToInformation(Transform source)
    {
       return new Sequence(this.ST_ApproachAndWait(source.transform, information.transform));
    }
    protected Node goToStarbucks(Transform target)
    {
        InteractiveObject3D io3d = target.GetComponent<InteractiveObject3D>();
        wander1 = io3d.trans;
        return new Sequence(this.ST_ApproachAndWait(this.participant1.transform, wander1.transform));
    }
    protected Node goToSpa(Transform target)
    {
        InteractiveObject3D io3d = target.GetComponent<InteractiveObject3D>();
        wander2 = io3d.trans;
        return new Sequence(this.ST_ApproachAndWait(this.participant3.transform, wander2.transform));
    }

    protected Node BuildTreeRoot()
    {
        var distance = Vector3.Distance(participant1.transform.position, participant2.transform.position);
        Func<bool> act = () => (waveFlag);
        //Node roaming = new DecoratorLoop(new Sequence(this.ST_ApproachAndWait(this.participant2.transform),
        //                this.ST_OrientParticipant(this.participant2.transform),
        //                this.ST_Talk_Participant1("Hello! I am Rohit"),
        //                stopTextCanvas(),
        //                this.ST_Talk_Participant1("We will run an experiment")));


        return new Sequence(this.ST_ApproachAndWait(this.participant1.transform, this.participant2.transform),
            this.ST_OrientParticipant(this.participant1.transform, this.participant2.transform),
            this.ST_ApproachAndWait(this.participant3.transform, this.participant1.transform),
            this.ST_OrientParticipant(this.participant3.transform, this.participant2.transform),
            this.ST_FaceAcknowledge(this.participant2.transform, true),
            new LeafWait(1000),
            this.ST_FaceAcknowledge(this.participant1.transform, true),
            new LeafWait(1000),
            this.ST_FaceAcknowledge(this.participant2.transform, false),
            this.ST_FaceAcknowledge(this.participant1.transform, false),
            this.goToInformation(this.participant1.transform),
            this.ST_OrientParticipant(this.participant2.transform, this.participant3.transform),
            this.ST_FaceAcknowledge(this.participant2.transform, true),
            new LeafWait(1000),
            this.ST_FaceAcknowledge(this.participant3.transform, true),
            new LeafWait(2000),
            this.ST_FaceAcknowledge(this.participant2.transform, false),
            this.ST_FaceAcknowledge(this.participant3.transform, false),
            new LeafWait(1000),
            this.goToStarbucks(this.participant1.transform),
            this.goToInformation(this.participant3.transform),           
            new LeafWait(1000),
            this.goToSpa(this.participant3.transform),
            new LeafWait(1000),
            this.ST_ApproachAndWait(this.participant1.transform, this.participant2.transform),
            this.ST_OrientParticipant(this.participant1.transform, this.participant2.transform),
            this.ST_ApproachAndWait(this.participant3.transform, this.participant2.transform),
            this.ST_OrientParticipant(this.participant3.transform, this.participant2.transform));



        //return new DecoratorLoop(
        //    new Sequence(
        //    new DecoratorInvert(
        //    new DecoratorLoop((new DecoratorInvert(
        //    new Sequence(this.ST_ApproachAndWait(this.participant2.transform)))))),
        //    this.ST_OrientParticipant(this.participant2.transform),
        //    this.ST_Talk_Participant1("Hello! I am Rohit"),
        //    this.ST_Talk_Participant2("Hello! I am Daniel"),
        //    stopTextCanvas()));
        // return roaming;

        //Node trigger = new DecoratorLoop(new LeafAssert(act));
        //Node root = new DecoratorLoop(new DecoratorForceStatus(RunStatus.Success, new SequenceParallel(trigger, roaming)));
        //return root;
    }
}