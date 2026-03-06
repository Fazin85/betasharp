using Beta3D.Util;

namespace Beta3D.FrameGraph;

public class FrameGraphBuilder
{
    private readonly List<InternalVirtualResource> _internalResources = [];
    private readonly List<VirtualResource> _externalResources = [];
    private readonly List<Pass> _passes = [];

    public IFramePass AddPass(string name)
    {
        var pass = new Pass(this, _passes.Count, name);
        _passes.Add(pass);
        return pass;
    }

    public IResourceHandle<T> ImportExternal<T>(string name, T resource)
    {
        var holder = new ExternalResource<T>(name, createdBy: null, resource);
        _externalResources.Add(holder);
        return (Handle<T>)holder.Handle;
    }

    public IResourceHandle<T> CreateInternal<T>(string name, IResourceDescriptor<T> descriptor)
        => (Handle<T>)CreateInternalResource(name, descriptor, createdBy: null).Handle;

    public void Execute(IGraphicsResourceAllocator resourceAllocator)
        => Execute(resourceAllocator, Inspector.None);

    public void Execute(IGraphicsResourceAllocator resourceAllocator, IInspector inspector)
    {
        BitList passesToKeep = IdentifyPassesToKeep();
        var passesInOrder = new List<Pass>(passesToKeep.Cardinality());
        var visiting = new BitList(_passes.Count);

        foreach (Pass pass in _passes)
        {
            ResolvePassOrder(pass, passesToKeep, visiting, passesInOrder);
        }

        AssignResourceLifetimes(passesInOrder);

        foreach (Pass pass in passesInOrder)
        {
            foreach (InternalVirtualResource resource in pass.ResourcesToAcquire)
            {
                inspector.AcquireResource(resource.Name);
                resource.Acquire(resourceAllocator);
            }

            inspector.BeforeExecutePass(pass.Name);
            pass.Task();
            inspector.AfterExecutePass(pass.Name);

            for (int id = pass.ResourcesToRelease.NextSetBit(0); id >= 0; id = pass.ResourcesToRelease.NextSetBit(id + 1))
            {
                InternalVirtualResource resource = _internalResources[id];
                inspector.ReleaseResource(resource.Name);
                resource.Release(resourceAllocator);
            }
        }
    }

    private InternalVirtualResource<T> CreateInternalResource<T>(string name, IResourceDescriptor<T> descriptor, Pass? createdBy)
    {
        int id = _internalResources.Count;
        var resource = new InternalVirtualResource<T>(id, name, createdBy, descriptor);
        _internalResources.Add(resource);
        return resource;
    }

    private BitList IdentifyPassesToKeep()
    {
        var scratchQueue = new Queue<Pass>(_passes.Count);
        var passesToKeep = new BitList(_passes.Count);

        foreach (VirtualResource resource in _externalResources)
        {
            Pass? pass = resource.Handle.CreatedBy;
            if (pass != null)
            {
                DiscoverAllRequiredPasses(pass, passesToKeep, scratchQueue);
            }
        }

        foreach (Pass pass in _passes)
        {
            if (pass.IsCullingDisabled)
            {
                DiscoverAllRequiredPasses(pass, passesToKeep, scratchQueue);
            }
        }

        return passesToKeep;
    }

    private void DiscoverAllRequiredPasses(Pass sourcePass, BitList visited, Queue<Pass> passesToTrace)
    {
        passesToTrace.Enqueue(sourcePass);

        while (passesToTrace.Count > 0)
        {
            Pass pass = passesToTrace.Dequeue();
            if (!visited[pass.Id])
            {
                visited.Set(pass.Id);

                for (int id = pass.RequiredPassIds.NextSetBit(0); id >= 0; id = pass.RequiredPassIds.NextSetBit(id + 1))
                {
                    passesToTrace.Enqueue(_passes[id]);
                }
            }
        }
    }

    private void ResolvePassOrder(Pass pass, BitList passesToFind, BitList visiting, List<Pass> output)
    {
        if (visiting[pass.Id])
        {
            string involvedPasses = string.Join(", ", Enumerable.Range(0, visiting.Count)
                .Where(i => visiting[i])
                .Select(i => _passes[i].Name));
            throw new InvalidOperationException($"Frame graph cycle detected between {involvedPasses}");
        }

        if (!passesToFind[pass.Id])
        {
            return;
        }

        visiting.Set(pass.Id);
        passesToFind[pass.Id] = false;

        for (int id = pass.RequiredPassIds.NextSetBit(0); id >= 0; id = pass.RequiredPassIds.NextSetBit(id + 1))
        {
            ResolvePassOrder(_passes[id], passesToFind, visiting, output);
        }

        foreach (Handle handle in pass.WritesFrom)
        {
            for (int id = handle.ReadBy.NextSetBit(0); id >= 0; id = handle.ReadBy.NextSetBit(id + 1))
            {
                if (id != pass.Id)
                {
                    ResolvePassOrder(_passes[id], passesToFind, visiting, output);
                }
            }
        }

        output.Add(pass);
        visiting[pass.Id] = false;
    }

    private void AssignResourceLifetimes(IEnumerable<Pass> passesInOrder)
    {
        var lastPassByResource = new Pass?[_internalResources.Count];

        foreach (Pass pass in passesInOrder)
        {
            for (int id = pass.RequiredResourceIds.NextSetBit(0); id >= 0; id = pass.RequiredResourceIds.NextSetBit(id + 1))
            {
                InternalVirtualResource resource = _internalResources[id];
                Pass? lastPass = lastPassByResource[id];
                lastPassByResource[id] = pass;

                if (lastPass == null)
                {
                    pass.ResourcesToAcquire.Add(resource);
                }
                else
                {
                    lastPass.ResourcesToRelease[id] = false;
                }

                pass.ResourcesToRelease.Set(id);
            }
        }
    }

    public interface IInspector
    {
        void AcquireResource(string name) { }
        void ReleaseResource(string name) { }
        void BeforeExecutePass(string name) { }
        void AfterExecutePass(string name) { }
    }

    public static class Inspector
    {
        public static readonly IInspector None = new InspectorNone();
        private sealed class InspectorNone : IInspector { }
    }

    private abstract class VirtualResource(string name)
    {
        public string Name { get; } = name;
        public Handle Handle { get; set; } = null!;

        public override string ToString() => Name;
    }

    private sealed class ExternalResource<T> : VirtualResource
    {
        private readonly T _resource;

        public ExternalResource(string name, Pass? createdBy, T resource) : base(name)
        {
            _resource = resource;
            Handle = new Handle<T>(this, version: 0, createdBy);
        }

        public T Get() => _resource;
    }

    private abstract class Handle
    {
        public abstract VirtualResource Holder { get; }
        public abstract Pass? CreatedBy { get; }
        public BitList ReadBy { get; } = new();
    }

    private sealed class Handle<T>(VirtualResource holder, int version, Pass? createdBy) : Handle, IResourceHandle<T>
    {
        private Handle<T>? _aliasedBy;

        public override VirtualResource Holder => holder;
        public override Pass? CreatedBy { get; } = createdBy;

        public T Value => holder switch
        {
            InternalVirtualResource<T> r => r.Get(),
            ExternalResource<T> r => r.Get(),
            _ => throw new InvalidOperationException($"Unexpected holder type {holder.GetType()}")
        };

        public Handle<T> WriteAndAlias(Pass pass)
        {
            if (holder.Handle != this)
            {
                throw new InvalidOperationException($"Handle {this} is no longer valid, as its contents were moved into {_aliasedBy}");
            }

            var newHandle = new Handle<T>(holder, version + 1, pass);
            holder.Handle = newHandle;
            _aliasedBy = newHandle;
            return newHandle;
        }

        public override string ToString() => CreatedBy != null
            ? $"{holder}#{version} (from {CreatedBy})"
            : $"{holder}#{version}";
    }

    private abstract class InternalVirtualResource(int id, string name) : VirtualResource(name)
    {
        public int Id { get; } = id;

        public abstract void Acquire(IGraphicsResourceAllocator allocator);
        public abstract void Release(IGraphicsResourceAllocator allocator);
    }

    private sealed class InternalVirtualResource<T> : InternalVirtualResource
    {
        private readonly IResourceDescriptor<T> _descriptor;
        private T? _physicalResource;

        public InternalVirtualResource(int id, string name, Pass? createdBy, IResourceDescriptor<T> descriptor)
            : base(id, name)
        {
            _descriptor = descriptor;
            Handle = new Handle<T>(this, version: 0, createdBy);
        }

        public T Get() => _physicalResource
            ?? throw new InvalidOperationException("Resource is not currently available");

        public override void Acquire(IGraphicsResourceAllocator allocator)
        {
            if (_physicalResource != null)
            {
                throw new InvalidOperationException("Tried to acquire physical resource, but it was already assigned");
            }
            _physicalResource = allocator.Acquire(_descriptor);
        }

        public override void Release(IGraphicsResourceAllocator allocator)
        {
            if (_physicalResource == null)
            {
                throw new InvalidOperationException("Tried to release physical resource that was not allocated");
            }
            allocator.Release(_descriptor, _physicalResource);
            _physicalResource = default;
        }
    }

    private sealed class Pass(FrameGraphBuilder outer, int id, string name) : IFramePass
    {

        public int Id { get; } = id;
        public string Name { get; } = name;
        public List<Handle> WritesFrom { get; } = [];
        public BitList RequiredResourceIds { get; } = new();
        public BitList RequiredPassIds { get; } = new();
        public Action Task { get; private set; } = () => { };
        public List<InternalVirtualResource> ResourcesToAcquire { get; } = [];
        public BitList ResourcesToRelease { get; } = new();
        public bool IsCullingDisabled { get; private set; }

        private void MarkResourceRequired(Handle handle)
        {
            if (handle.Holder is InternalVirtualResource resource)
            {
                RequiredResourceIds.Set(resource.Id);
            }
        }

        private void MarkPassRequired(Pass pass)
            => RequiredPassIds.Set(pass.Id);

        public IResourceHandle<T> CreatesInternal<T>(string name, IResourceDescriptor<T> descriptor)
        {
            InternalVirtualResource<T> resource = outer.CreateInternalResource(name, descriptor, this);
            RequiredResourceIds.Set(resource.Id);
            return (Handle<T>)resource.Handle;
        }

        public void Reads<T>(IResourceHandle<T> handle)
            => InternalReads((Handle<T>)handle);

        private void InternalReads(Handle handle)
        {
            MarkResourceRequired(handle);
            if (handle.CreatedBy != null)
            {
                MarkPassRequired(handle.CreatedBy);
            }
            handle.ReadBy.Set(Id);
        }

        public IResourceHandle<T> ReadsAndWrites<T>(IResourceHandle<T> handle)
            => InternalReadsAndWrites((Handle<T>)handle);

        public void Requires(IFramePass pass)
            => RequiredPassIds.Set(((Pass)pass).Id);

        public void DisableCulling()
            => IsCullingDisabled = true;

        private Handle<T> InternalReadsAndWrites<T>(Handle<T> handle)
        {
            WritesFrom.Add(handle);
            InternalReads(handle);
            return handle.WriteAndAlias(this);
        }

        public void Executes(Action task)
            => Task = task;

        public override string ToString() => Name;
    }
}
