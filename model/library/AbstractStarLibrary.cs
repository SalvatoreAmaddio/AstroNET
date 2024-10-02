using Backend.Model;
using FrontEnd.Model;
using System.Data.Common;

namespace AstroNET.model
{
    [Table(nameof(TransitType))]
    public class TransitType : AbstractModel<TransitType> 
    {
        private Int64 _transitTypeId;
        private string _transitTypeName = string.Empty;

        [PK]
        public Int64 TransitTypeId { get => _transitTypeId; set => UpdateProperty(ref value, ref _transitTypeId); }
        [Field]
        public string TransitTypeName { get => _transitTypeName; set => UpdateProperty(ref value, ref _transitTypeName); }
    
        public TransitType() { }
        public TransitType(Int64 id) => _transitTypeId = id;
        public TransitType(DbDataReader reader) 
        {
            _transitTypeId = reader.GetInt64(0);
            _transitTypeName = reader.GetString(1);
        }

        public override string ToString()
        {
            return $"{TransitTypeName}";
        }
    }

    public interface IAbstractPointLibrary 
    {
        Int64 LibraryID { get; }
        string Description { get; }
        TransitType? TransitType { get; }
        Aspect? Aspect { get; set; }
        void Build();
    }

    public interface IStarLibrary : IAbstractPointLibrary
    {
        Star Star { get; }
    }

    public interface IHouseLibrary : IAbstractPointLibrary
    {
        House House { get; }
    }

    public abstract class AbstractPointLibrary<M> : AbstractModel<M>, IAbstractPointLibrary where M : IAbstractModel, new()
    {
        private Int64 _libraryID;
        private string _description = string.Empty;
        protected TransitType? _transitType;

        [PK]
        public Int64 LibraryID { get => _libraryID; set => UpdateProperty(ref value, ref _libraryID); }

        [Field]
        public string Description { get => _description; set => UpdateProperty(ref value, ref _description); }

        [FK]
        public TransitType? TransitType { get => _transitType; set => UpdateProperty(ref value, ref _transitType); }

        public Aspect? Aspect { get; set; }
        public AbstractPointLibrary() : base()
        {
        }

        public AbstractPointLibrary(DbDataReader reader) : this()
        {
            _libraryID = reader.GetInt64(0);
            _description = reader.GetString(2);
            _transitType = new(reader.GetInt64(3));
        }

        public abstract void Build();

        public override string ToString() => $"{LibraryID} {TransitType}";

    }

    public abstract class AbstractStarLibrary<M> : AbstractPointLibrary<M>, IStarLibrary where M : IAbstractModel, new()
    {
        private Star _star = null!;

        
        [FK]
        public Star Star { get => _star; set => UpdateProperty(ref value, ref _star); }
    
        public AbstractStarLibrary() : base()
        {
        }

        public AbstractStarLibrary(DbDataReader reader) : base(reader)
        {
            _star = new Star(reader.GetInt64(1));
        }

        public override void Build() => Star.Build();
        public override string ToString() => $"{LibraryID} Star:{Star.PointId} {TransitType}";

    }

    [Table(nameof(LibraryStarAspects))]
    public class LibraryStarAspects : AbstractStarLibrary<LibraryStarAspects>
    {
        protected Star _star2 = null!;
        private Energy _energy = null!;

        [FK]
        public Energy Energy { get => _energy; set => UpdateProperty(ref value, ref _energy); }

        [FK("StarID")]
        public Star Star2 { get => _star2; set => UpdateProperty(ref value, ref _star2); }

        public LibraryStarAspects()
        {
        }

        public LibraryStarAspects(DbDataReader reader) : base(reader) 
        {
            _energy = new(reader.GetInt64(4));
            _star2 = new Star(reader.GetInt64(5));
        }

        public override void Build()
        {
            base.Build();
            Star2.Build();
        }
    }

    [Table(nameof(LibraryStarHouses))]
    public class LibraryStarHouses : AbstractStarLibrary<LibraryStarHouses>
    {
        protected House _house = null!;

        [FK("HouseId")]
        public House House { get => _house; set => UpdateProperty(ref value, ref _house); }

        public LibraryStarHouses()
        {
        }

        public LibraryStarHouses(Aspect aspect, Star star, House house, string description)
        {
            Aspect = aspect;
            Star = star;
            House = house;
            Description = description;
        }

        public LibraryStarHouses(DbDataReader reader) : base(reader)
        {
            _house = new House(reader.GetInt64(4));
        }

        public override void Build()
        {
            base.Build();
            House.Build();
        }
    }
    
    [Table(nameof(LibraryStarSigns))]
    public class LibraryStarSigns : AbstractStarLibrary<LibraryStarSigns>
    {
        protected Sign _sign = null!;

        [FK]
        public Sign Sign { get => _sign; set => UpdateProperty(ref value, ref _sign); }

        public LibraryStarSigns() : base()
        {
            _transitType = new(1);
        }

        public LibraryStarSigns(DbDataReader reader) : base(reader)
        {
            _sign = new Sign(reader.GetInt64(4));
        }

        public override void Build()
        {
            base.Build();
            Sign.Build();
        }
    }

    [Table(nameof(LibraryHouseSigns))]
    public class LibraryHouseSigns : AbstractPointLibrary<LibraryHouseSigns>, IHouseLibrary 
    {
        private House _house = null!;
        protected Sign _sign = null!;

        [FK]
        public Sign Sign { get => _sign; set => UpdateProperty(ref value, ref _sign); }

        [FK]
        public House House { get => _house; set => UpdateProperty(ref value, ref _house); }

        public LibraryHouseSigns() : base()
        {
            _transitType = new(1);
        }

        public LibraryHouseSigns(DbDataReader reader) : base(reader)
        {
            _house = new House(reader.GetInt64(1));
            _sign = new Sign(reader.GetInt64(4));
        }

        public override void Build() 
        {
            House.Build();
            Sign.Build();
        }

        public override string ToString() => $"{LibraryID} House:{House.PointId} Sign:{Sign.SignId} {TransitType}";
    }

}