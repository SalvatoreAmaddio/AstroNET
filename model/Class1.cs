using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.model
{
    public class TransitGroup
    {
        public DateTime TransitDate { get; set; }
        public string Header { get; set; } = string.Empty;
        public IEnumerable<Aspect> Aspects { get; set; } = null!;
    }

    public class Class1
    {

        public static List<Aspect> Filter(List<Aspect> aspects)
        {
            List<Aspect> filteredAspects = [];
            LinkedList<Aspect> Copy = new(aspects);

            for (int c = 0; c <= aspects.Count; c++)
            {
                var subChunkToFilter = Copy.TakeWhile(s => s.AspectId == aspects[c].AspectId && s.PointAR() == aspects[c].PointAR()).ToList();
                c += subChunkToFilter.Count - 1;
                var toAdd = Pick(subChunkToFilter);
                filteredAspects.AddRange(toAdd);

                foreach (var node in subChunkToFilter) Copy.Remove(node);
                if (Copy.Count == 0) break;
            }

            return filteredAspects;
        }

        private static List<Aspect> Pick(List<Aspect> chunk)
        {
            List<Aspect> r = [];
            Aspect? a;
            Aspect? b;
            Aspect? c;
            Aspect? d;

            a = chunk.FirstOrDefault();
            if (a != null)
                r.Add(a);

            b = chunk.Where(s => s.OrbDiff > 0).LastOrDefault();
            if (b != null && !r.Any(s => s.IsSame(b)))
                r.Add(b);

            c = chunk.Where(s => s.OrbDiff < 0).FirstOrDefault();
            if (c != null && !r.Any(s => s.IsSame(c)))
                r.Add(c);

            d = chunk.LastOrDefault();
            if (d != null && !r.Any(s => s.IsSame(d)))
                r.Add(d);

            return r;
        }
    }
}
