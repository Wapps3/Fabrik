using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Skeleton : MonoBehaviour
{
    private LineRenderer lr;

    [SerializeField]
    private List<Vector2> list_jointPoint;

    [SerializeField]
    private Vector2 target;

    [SerializeField]
    private GameObject t;

    [SerializeField]
    private float tol;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Setlr(List<Vector2> list)
    {
        lr.SetVertexCount(list.Count);

        for(int k=0;k<list.Count; k++)
        {
     
            lr.SetPosition(k, list[k]);
        }
    }

    void OnGUI()
    {
        Vector3 point = new Vector3();
        Event currentEvent = Event.current;
        Vector2 mousePos = new Vector2();

        Camera cam = Camera.main;


        mousePos.x = currentEvent.mousePosition.x;
        mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;

        point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
        target = new Vector2(point.x,point.y);
    }

    void Fabrik()
    {
        // Vector2 target = new Vector2(t.transform.position.x, t.transform.position.y);
        Vector2 target = this.target;
        Debug.Log("thistarget =" + this.target);
        Debug.Log("target =" + target);

        //Vector2 target = new Vector2(this.target.x, this.target.y);

        int n = list_jointPoint.Count;
        
        //Initialize the di
        List<float> distance = new List<float>();

        for(int i = 0 ; i < n - 1 ; i++)
        {
            distance.Add( Vector2.Distance(list_jointPoint[i + 1], list_jointPoint[i]) );
        }

        //the distance between the root and the target
        float dist = Vector2.Distance(list_jointPoint[0], target);

        float sumDistance = 0;
        for (int i = 0; i < distance.Count; i++)
            sumDistance += distance[i];

        //check wheter the target is within reach
        if(dist > sumDistance )
        {
            //the target is unreachable
            for(int i=0; i < n - 1; i++)
            {
                //Find the distance ri  between the target t and the joint
                float r = Vector2.Distance(target, list_jointPoint[i]);
                float lambda = distance[i]/r;

                //Find the new joint position pi
                list_jointPoint[i + 1] = (1 - lambda) * list_jointPoint[i] + lambda * target;
            }
        }
        else
        {
            //The target is reachable; thus, set as b the inital postion of the joint p1
            Vector2 b = list_jointPoint[0];

            //Check whether the distance between the end effector pn and the target t is greater than a tolerance
            float difA = Vector2.Distance(list_jointPoint[n - 1], target);

            while(difA > tol)
            {
                //STAGE1 forward reaching
                //set the end effector pn as target t
                list_jointPoint[n - 1] = target;

                for(int i = n-2; i >= 0; i--)
                {
                    //Find the new joint position pi
                    list_jointPoint[i] = Reach(i+1,i,distance[i]);
                }

                //STAGE2 backward reaching
                //set the root p1 its initial position

                list_jointPoint[0] = b;

                for(int i=0; i < n-1; i++)
                {
                    //Find the new positions pi
                    list_jointPoint[i+1] = Reach(i, i+1, distance[i]);
                }
                difA = Vector2.Distance(list_jointPoint[n-1],target);
            } 
        }
    }

    Vector2 Reach(int a,int b,float d)
    {
        float r = Vector2.Distance(list_jointPoint[a], list_jointPoint[b]);
        float lambda = d / r;

        return (1 - lambda) * list_jointPoint[a] + lambda * list_jointPoint[b];
    }


    // Update is called once per frame
    void Update()
    {
        Fabrik();
        Setlr(list_jointPoint);
    }
}
