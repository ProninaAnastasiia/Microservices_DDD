namespace DddService.Common;

public abstract class Entity<Tid> {
    
    int? _requestedHashCode;
    
    Tid? _Id;
    public virtual Tid Id
    {
        get
        {
            return _Id;
        }
        protected set
        {
            _Id = value;
        }
    }
    
    public bool IsTransient() => Id.Equals(default(Tid));
    
    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Entity<Tid>))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (GetType() != obj.GetType())
        {
            return false;
        }

        var item = (Entity<Tid>) obj;

        if (item.IsTransient() || IsTransient())
        {
            return false;
        }
        else
        {
            return item.Id.Equals(Id);
        }
    }
    
    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            if (!_requestedHashCode.HasValue)
                _requestedHashCode = this.Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

            return _requestedHashCode.Value;
        }
        else
            return base.GetHashCode();

    }
    
    public static bool operator == (Entity<Tid> left, Entity<Tid> right)
    {
        if (Equals(left, null))
        {
            return (Equals(right, null)) ? true : false;
        }
        else
        {
            return left.Equals(right);
        }
    }
    
    public static bool operator != (Entity<Tid> left, Entity<Tid> right)
    {
        return !(left == right);
    }
    
    public bool IsDeleted { get; set; }
}



    // Aggregates
    public abstract class Aggregate<TId> : Entity<TId>, IAggregate
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.AsReadOnly();
    }

    public IEvent[] ClearDomainEvents()
    {
        IEvent[] dequeuedEvents = _domainEvents.ToArray();

        _domainEvents.Clear();

        return dequeuedEvents;
    }
}