using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class SampleGraphView : GraphView
{
    public SampleGraphView() : base()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new SelectionDragger());

        nodeCreationRequest += context =>
        {
            AddElement(new SampleNode());
        };

    }

    public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
    {
        return ports.ToList();
    }
}