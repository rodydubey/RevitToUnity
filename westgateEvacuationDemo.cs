using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using TreeSharpPlus;

public class westgateEvacuationDemo : MonoBehaviour
{
    public GameObject participant1; //rohit blue
    public GameObject participant2;
    public GameObject participant3;
    public GameObject participant4;
    public GameObject participant5;
    public GameObject participant6;
    public GameObject participant7;
    public GameObject participant8;
    public GameObject participant9;
    public GameObject participant10;
    public GameObject participant11;
    public GameObject participant12;
    public GameObject participant13;
    public GameObject participant14;
    public GameObject participant15;
    public GameObject participant16;
    public GameObject participant17;
    public GameObject participant18;
    public GameObject participant19;
    public GameObject participant20;
    public GameObject participant21; 
    public GameObject participant22;
    public GameObject participant23;
    public GameObject participant24;
    public GameObject participant25;
    public GameObject participant26;
    public GameObject participant27;
    public GameObject participant28;
    public GameObject participant29;
    public GameObject participant30;
    public GameObject participant31;
    public GameObject participant32;
    public GameObject participant33;
    public GameObject participant34;
    public GameObject participant35;
    public GameObject participant36;
    public GameObject participant37;
    public GameObject participant38;
    public GameObject participant39;
    public GameObject participant40;

    private BehaviorAgent behaviorAgent;
    public bool waveFlag = true;
    public Transform information;
    public Transform nearestExit;
    public Transform nearestExitCommon;
    public Transform nearestExit2;
    public Transform nearestExit3;
    public Transform nearestExit4;
    public Transform nearestExit5;
    public Transform nearestExit6;
    public Transform nearestExit7;
    public Transform nearestExit8;
    public Transform nearestExit9;
    public Transform nearestExit10;
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
    
    protected Node ST_ApproachAndWait(Transform source, Transform target)
    {
        Val<Vector3> position = Val.V(() => target.position);
        return new Sequence(source.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(position, 2), new LeafWait(1000));        
    }
    protected Node goToExit(Transform target)
    {
        evacuatingAgent eA = target.GetComponent<evacuatingAgent>();
        nearestExit = eA.nearestTrans;
        return new Sequence(this.ST_ApproachAndWait(target.transform, nearestExit.transform));
    }
    protected Node goToExit2(Transform target)
    {
        evacuatingAgent eA = target.GetComponent<evacuatingAgent>();
        nearestExit2 = eA.nearestTrans;
        return new Sequence(this.ST_ApproachAndWait(target.transform, nearestExit2.transform));
    }
    protected Node goToExit3(Transform target)
    {
        evacuatingAgent eA = target.GetComponent<evacuatingAgent>();
        nearestExit3 = eA.nearestTrans;
        return new Sequence(this.ST_ApproachAndWait(target.transform, nearestExit3.transform));
    }
    protected Node goToExit4(Transform target)
    {
        evacuatingAgent eA = target.GetComponent<evacuatingAgent>();
        nearestExit4 = eA.nearestTrans;
        return new Sequence(this.ST_ApproachAndWait(target.transform, nearestExit4.transform));
    }
    protected Node goToExit5(Transform target)
    {
        evacuatingAgent eA = target.GetComponent<evacuatingAgent>();
        nearestExit5 = eA.nearestTrans;
        return new Sequence(this.ST_ApproachAndWait(target.transform, nearestExit5.transform));
    }
    protected Node goToExit6(Transform target)
    {
        evacuatingAgent eA = target.GetComponent<evacuatingAgent>();
        nearestExit6 = eA.nearestTrans;
        return new Sequence(this.ST_ApproachAndWait(target.transform, nearestExit6.transform));
    }
    protected Node goToExit7(Transform target)
    {
        evacuatingAgent eA = target.GetComponent<evacuatingAgent>();
        nearestExit7 = eA.nearestTrans;
        return new Sequence(this.ST_ApproachAndWait(target.transform, nearestExit7.transform));
    }
    protected Node goToExit8(Transform target)
    {
        evacuatingAgent eA = target.GetComponent<evacuatingAgent>();
        nearestExit8 = eA.nearestTrans;
        return new Sequence(this.ST_ApproachAndWait(target.transform, nearestExit8.transform));
    }
    protected Node goToExit9(Transform target)
    {
        evacuatingAgent eA = target.GetComponent<evacuatingAgent>();
        nearestExit9 = eA.nearestTrans;
        return new Sequence(this.ST_ApproachAndWait(target.transform, nearestExit9.transform));
    }
    protected Node goToExit10(Transform target)
    {
        evacuatingAgent eA = target.GetComponent<evacuatingAgent>();
        nearestExit10 = eA.nearestTrans;
        return new Sequence(this.ST_ApproachAndWait(target.transform, nearestExit10.transform));
    }
    protected Node goToExitCommon(Transform target)
    {
        evacuatingAgent eA = target.GetComponent<evacuatingAgent>();
        nearestExitCommon = eA.nearestTrans;
        return new Sequence(this.ST_ApproachAndWait(target.transform, nearestExitCommon.transform));
    }


    protected Node BuildTreeRoot()
    {
        //return new Sequence(this.goToExit1(this.participant1.transform));

        Node roaming = new DecoratorLoop(
                        new SequenceParallel(
                        goToExit(this.participant1.transform),
                        goToExit2(this.participant2.transform), 
                        goToExit3(this.participant3.transform),
                        goToExit4(this.participant4.transform),
                        goToExit5(this.participant5.transform),
                        goToExit6(this.participant6.transform),
                        goToExit7(this.participant7.transform),
                        goToExit8(this.participant8.transform),
                        goToExit9(this.participant9.transform),
                        goToExit10(this.participant10.transform),
                        goToExitCommon(this.participant11.transform),
                        goToExitCommon(this.participant12.transform),
                        goToExitCommon(this.participant13.transform),
                        goToExitCommon(this.participant14.transform),
                        goToExitCommon(this.participant15.transform),
                        goToExitCommon(this.participant16.transform),
                        goToExitCommon(this.participant17.transform),
                        goToExitCommon(this.participant18.transform),
                        goToExitCommon(this.participant19.transform),
                        goToExitCommon(this.participant20.transform),
                        goToExit(this.participant21.transform),
                        goToExit2(this.participant22.transform),
                        goToExit3(this.participant23.transform),
                        goToExit4(this.participant24.transform),
                        goToExit5(this.participant25.transform),
                        goToExit6(this.participant26.transform),
                        goToExit7(this.participant27.transform),
                        goToExit8(this.participant28.transform),
                        goToExit9(this.participant29.transform),
                        goToExit10(this.participant30.transform),
                        goToExitCommon(this.participant31.transform),
                        goToExitCommon(this.participant32.transform),
                        goToExitCommon(this.participant33.transform),
                        goToExitCommon(this.participant34.transform),
                        goToExitCommon(this.participant35.transform),
                        goToExitCommon(this.participant36.transform),
                        goToExitCommon(this.participant37.transform),
                        goToExitCommon(this.participant38.transform),
                        goToExitCommon(this.participant39.transform),
                        goToExitCommon(this.participant40.transform)));
        return roaming;

    }
}