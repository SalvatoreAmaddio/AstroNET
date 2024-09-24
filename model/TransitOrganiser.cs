
namespace WpfApp1.model
{
    public class SubTransitKeyGroup(int id, Aspect aspect) 
    {
        public int Id { get; set; } = id;
        public Aspect Aspect { get; set; } = aspect;

        public override bool Equals(object? obj)
        {
            return obj is SubTransitKeyGroup key &&
                   Id == key.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }

    public class TransitSubGroup 
    {
        public SubTransitKeyGroup KeyGroup { get; set; } = null!;
        public string SubHeader => $"{KeyGroup.Aspect.AspectName} {KeyGroup.Aspect.IsRetrograde()}From {SubAspects.FirstOrDefault()?.DateOf.ToString("dd/MM/yyyy")} to {SubAspects.LastOrDefault()?.DateOf.ToString("dd/MM/yyyy")}";
        public IEnumerable<Aspect?> SubAspects { get; set; } = null!;
    }

    public class TransitGroup
    {
        public string Header { get; set; } = string.Empty;
        public IEnumerable<TransitSubGroup?> SubTransits { get; set; } = null!;
    }

    public class TransitOrganiser
    {
        private static int id = 0;
        public static List<Aspect?> Filter(List<Aspect> aspects)
        {
            List<Aspect?> filteredAspects = [];
            LinkedList<Aspect> Copy = new(aspects);

            for (int c = 0; c <= aspects.Count; c++)
            {
                var subChunkToFilter = Copy.TakeWhile(s => s.AspectId == aspects[c].AspectId && s.PointAR() == aspects[c].PointAR()).ToList();
                c += subChunkToFilter.Count - 1;
                var toAdd = Pick(subChunkToFilter);
                filteredAspects.AddRange(toAdd);

                foreach (var node in subChunkToFilter) Copy.Remove(node);
                if (Copy.Count == 0) 
                {
                    break;
                } else 
                {
                    //if (toAdd.Count > 0)
                        //filteredAspects.Add(null);
                }
            }

            return filteredAspects;
        }

        private static List<Aspect?> Pick(List<Aspect> chunk)
        {
            List<Aspect?> r = [];
            Aspect? a;
            Aspect? b;
            Aspect? c;
            Aspect? d;
            id++;

            a = chunk.FirstOrDefault();
            if (a != null) 
            {
                a.TransitBundle = id;
                r.Add(a);
            }

            b = chunk.Where(s => s.OrbDiff > 0).LastOrDefault();
            if (b != null && !r.Any(s => s != null && s.IsSame(b))) 
            {
                b.TransitBundle = id;
                r.Add(b);
            }

            c = chunk.Where(s => s.OrbDiff < 0).FirstOrDefault();
            if (c != null && !r.Any(s => s != null && s.IsSame(c))) 
            {
                c.TransitBundle = id;
                r.Add(c);
            }

            d = chunk.LastOrDefault();
            if (d != null && !r.Any(s => s != null && s.IsSame(d))) 
            {
                d.TransitBundle = id;
                r.Add(d);
            }

            return r;
        }
    }
}
