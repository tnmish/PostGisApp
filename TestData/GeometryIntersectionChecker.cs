using NetTopologySuite.Geometries;
using NetTopologySuite.Index.Strtree;

namespace TestData
{
    public static class GeometryIntersectionChecker
    {
        public static bool IntercectsAnyWithIndex(Geometry? target, IEnumerable<Geometry?> geometries)
        {
            if (geometries == null) return false;

            if (target == null) return false;

            var tree = new STRtree<Geometry>();
            foreach (var geom in geometries.Where(x => x != null))
            {
                tree.Insert(geom!.Envelope.EnvelopeInternal, geom);

            }
            tree.Build();

            var candidates = tree.Query(target.EnvelopeInternal);
            foreach (var geom in candidates)
            {
                if (target.Intersects(geom))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
