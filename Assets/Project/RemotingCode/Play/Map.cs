﻿using System;
using System.Linq;
using System.Collections.Generic;

using Regulus.Collection;
using Regulus.CustomType;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Extension;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Map : IMapFinder
    {
        public struct Room
        {
            public Vector2 Center;

            public CustomType.Flag<MAZEWALL> Walls;
        }
        public class Visible : IQuadObject
        {
            public Visible(IIndividual noumenon)
            {
                this.Noumenon = noumenon;
            }

            public void Initial()
            {
                this.Noumenon.BoundsEvent += this._Changed; // this -> Noumenon.EventSet
            }

            public void Release()
            {
                this.Noumenon.BoundsEvent -= this._Changed;
            }

            private void _Changed()
            {
                this._BoundsChanged(this, EventArgs.Empty);
            }

            ~Visible()
            {

            }

            Rect IQuadObject.Bounds
            {
                get { return this.Noumenon.Bounds; }
            }

            private event EventHandler _BoundsChanged;
            event EventHandler IQuadObject.BoundsChanged
            {
                add { this._BoundsChanged += value; }
                remove { this._BoundsChanged -= value; }
            }

            public IIndividual Noumenon { get; private set; }
        }

        private readonly QuadTree<Visible> _QuadTree;

        private readonly Dictionary<Guid,Visible> _Set;

        private readonly List<Visible> _EntranceSet;

        private readonly Regulus.Utility.IRandom _Random;


        public Map()
        {
            _Random = Regulus.Utility.Random.Instance;
            _EntranceSet = new List<Visible>();
            this._Set = new Dictionary<Guid, Visible>();
            this._QuadTree = new QuadTree<Visible>(new Size(2, 2), 100);
        }

        public Map(Regulus.Utility.IRandom random) : this()
        {
            _Random = random;            
        }


        public void JoinStaffs(IEnumerable<IIndividual> individuals)
        {
            foreach (var individual in individuals)
            {
                JoinStaff(individual);
            }
        }
        public void JoinStaff(IIndividual individual)
        {
            this._Join(individual);
        }

        public void JoinChallengers(IEnumerable<IIndividual> individuals)
        {
            foreach (var individual in individuals)
            {
                JoinChallenger(individual);
            }
        }
        public void JoinChallenger(IIndividual individual)
        {            
            this._Join(individual);


            var concierges = this._FindConcierges(individual);
            
            var concierge = concierges.Shuffle().FirstOrDefault();
            if(concierge != null)
            {
                Vector2 position = concierge.GetPosition();
                individual.SetPosition(position.X, position.Y);
                individual.AddDirection(_Random.NextInt(0, 360));                
            }            
        }

        private IEnumerable<Concierge> _FindConcierges(IIndividual individual)
        {
            return (from e in this._EntranceSet
                    let concierge = e.Noumenon.GetConcierge()
                    where concierge != null && concierge.IsAcceptsType(individual)
                    select concierge);
        }

        private void _Join(IIndividual individual)
        {
            var v = new Visible(individual);
            v.Initial();
            this._Set.Add(individual.Id , v);
            this._QuadTree.Insert(v);

            if(individual.EntityType == ENTITY.ENTRANCE)
            {
                _EntranceSet.Add(v);
            }
            
        }

        public void Left(IIndividual individual)
        {
            
            Visible visible;
            if (_Set.TryGetValue(individual.Id, out visible))
            {
                this._QuadTree.Remove(visible);
                this._Set.Remove(individual.Id);
                _EntranceSet.Remove(visible);
                visible.Release();
            }
        }

        IIndividual[] IMapFinder.Find(Rect bound)
        {
            var results = this._QuadTree.Query(bound);

            return (from r in results select r.Noumenon).ToArray();
        }
        
    }
}