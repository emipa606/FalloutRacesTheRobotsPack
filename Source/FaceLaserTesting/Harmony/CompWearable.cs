using System.Collections.Generic;
using Verse;

namespace FaceLaserTesting;

public abstract class CompWearable : ThingComp
{
    public virtual IEnumerable<Gizmo> CompGetGizmosWorn()
    {
        // return no Gizmos
        return new List<Gizmo>();
    }
}