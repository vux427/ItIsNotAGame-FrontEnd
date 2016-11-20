﻿using System;
using System.Security.Policy;

using Regulus.Framework;
using Regulus.Game;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Project.ItIsNotAGame1.Game.Data;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Center : IUpdatable , Remoting.ICore
    {
        private readonly IAccountFinder _AccountFinder;
        private readonly IGameRecorder _GameRecorder;
        private readonly Hall _Hall;
        private readonly Updater _Updater;
        

        private readonly Zone _Zone;
        public Center(IAccountFinder account_finder, IGameRecorder game_recorder  )
        {
            
            _AccountFinder = account_finder;
            _GameRecorder = game_recorder;
            _Hall = new Hall();
            _Updater = new Updater();
            _Zone = new Zone(new []
            {
                new RealmInfomation
                {
                    Name = "maze1",
                    Maze = new MazeInfomation()
                    {
                        Dimension = 10 , Width = 15 , Height = 15 ,
                        MazeUnits = new []
                        {
                            new MazeUnitInfomation { Name = "enterance1" , Type = LEVEL_UNIT.ENTERANCE1 },
                            new MazeUnitInfomation { Name = "enterance2" , Type = LEVEL_UNIT.ENTERANCE2 },
                            new MazeUnitInfomation { Name = "enterance3" , Type = LEVEL_UNIT.ENTERANCE3 },
                            new MazeUnitInfomation { Name = "enterance4" , Type = LEVEL_UNIT.ENTERANCE4 },
                            new MazeUnitInfomation { Name = "wall" , Type = LEVEL_UNIT.WALL},
                            new MazeUnitInfomation { Name = "pool" , Type = LEVEL_UNIT.POOL},
                            new MazeUnitInfomation { Name = "field" , Type = LEVEL_UNIT.FIELD},
                            new MazeUnitInfomation { Name = "thickwall" , Type = LEVEL_UNIT.GATE},
                            new MazeUnitInfomation { Name = "chest" , Type = LEVEL_UNIT.CHEST},
                            new MazeUnitInfomation { Name = "exit1" , Type = LEVEL_UNIT.EXIT}
                        }
                    },
                    Town = new TownInfomation() {Name = ""}
                    
                },
                new RealmInfomation
                {
                    Name = "maze2",
                    Maze = new MazeInfomation()
                    {
                        Dimension = 5 , Width = 15 , Height = 15 ,
                        MazeUnits = new []
                        {
                            new MazeUnitInfomation { Name = "enterance1" , Type = LEVEL_UNIT.ENTERANCE1 },
                            new MazeUnitInfomation { Name = "enterance2" , Type = LEVEL_UNIT.ENTERANCE2 },
                            new MazeUnitInfomation { Name = "enterance3" , Type = LEVEL_UNIT.ENTERANCE3 },
                            new MazeUnitInfomation { Name = "enterance4" , Type = LEVEL_UNIT.ENTERANCE4 },
                            new MazeUnitInfomation { Name = "wall" , Type = LEVEL_UNIT.WALL},
                            new MazeUnitInfomation { Name = "pool" , Type = LEVEL_UNIT.POOL},
                            new MazeUnitInfomation { Name = "field" , Type = LEVEL_UNIT.FIELD},
                            new MazeUnitInfomation { Name = "thickwall" , Type = LEVEL_UNIT.GATE},
                            new MazeUnitInfomation { Name = "chest" , Type = LEVEL_UNIT.CHEST},
                            new MazeUnitInfomation { Name = "exit2" , Type = LEVEL_UNIT.EXIT}
                        }
                    },
                    Town = new TownInfomation() {Name = ""}

                },
                new RealmInfomation
                {
                    Name = "town1",
                    Maze = new MazeInfomation()
                    {
                        Dimension = 0 , Width = 0 , Height = 0 ,
                        MazeUnits = new MazeUnitInfomation[0],                        
                    },
                    Town = new TownInfomation() {Name = "town1"}
                },

                new RealmInfomation
                {
                    Name = "town2",
                    Maze = new MazeInfomation()
                    {
                        Dimension = 0 , Width = 0 , Height = 0 ,
                        MazeUnits = new MazeUnitInfomation[0],
                    },
                    Town = new TownInfomation() {Name = "town2"}
                }
            });
            
            

        }
        public void Join(Remoting.ISoulBinder binder)
        {
            this._Hall.PushUser(new User(binder , this._AccountFinder , this._GameRecorder , this._Zone));
        }

        void IBootable.Launch()
        {
            this._Updater.Add(this._Hall);
            this._Updater.Add(this._Zone);
           
        }

        void IBootable.Shutdown()
        {
            this._Updater.Shutdown();
        }

        bool IUpdatable.Update()
        {
            this._Updater.Working();
            return true;
        }

        void Remoting.ICore.AssignBinder(Remoting.ISoulBinder binder)
        {
            this.Join(binder);
        }
    }
}