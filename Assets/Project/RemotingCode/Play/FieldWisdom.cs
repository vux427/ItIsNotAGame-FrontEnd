using System.Collections.Generic;
using System;

using Regulus.BehaviourTree;
using Regulus.Project.ItIsNotAGame1.Data;
using System.Linq;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class FieldWisdom : Wisdom
    {
        private readonly ENTITY[] _Types;

        private readonly Entity _Owner;

        private readonly IMapGate _Gate;

        private readonly IMapFinder _Finder;

        private Queue<Guid> _Ids;

        public FieldWisdom(ENTITY[] types, Entity owner, IMapGate gate, IMapFinder finder)
        {
            _Ids = new Queue<Guid>();
            _Types = types;
            _Owner = owner;
            _Gate = gate;
            _Finder = finder;
        }

        protected override ITicker _Launch()
        {
            var builder = new Regulus.BehaviourTree.Builder();
            var ticker = builder
                .Sequence()
                    .Action(() => new WaitSecondStrategy(10f))
                    .Selector()
                        .Sequence()
                            .Action(_Pass)
                        .End()
                        .Sequence()
                            .Action(_NeedSpawn)
                            .Action(_Spawn)
                        .End()
                    .End()

                .End().Build();

            return ticker;
        }

        private TICKRESULT _Pass(float arg)
        {
            if (_Ids.Count > 0)
            {
                var id = _Ids.Dequeue();
                _Gate.Pass(_Owner.GetPosition(), id);
                return TICKRESULT.SUCCESS;
            }
            return TICKRESULT.FAILURE;

        }

        protected override void _Update(float delta)
        {
            
        }

        protected override void _Shutdown()
        {
            
        }

        private TICKRESULT _NeedSpawn(float arg)
        {
            IIndividual individual = _Owner;
            var actors = _Finder.Find(individual.Bounds);
            var anyActor = (from actor in actors where EntityData.IsActor(actor.EntityType) select actor).Any();
            if (anyActor)
                return TICKRESULT.FAILURE;
            return TICKRESULT.SUCCESS;
        }

        private TICKRESULT _Spawn(float arg)
        {

            var ids = _Gate.SpawnField(_Types);
            if (ids.Length > 0)
            {
                foreach (var id in ids)
                {
                    _Ids.Enqueue(id);
                }
                
                return TICKRESULT.SUCCESS;
            }
            return TICKRESULT.FAILURE;
        }
    }
}