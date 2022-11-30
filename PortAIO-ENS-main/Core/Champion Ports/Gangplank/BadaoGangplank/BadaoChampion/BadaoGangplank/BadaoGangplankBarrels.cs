﻿using System;
using System.Collections.Generic;
using System.Linq;
using BadaoGP;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Rendering;
using PortAIO;
using SharpDX;

namespace BadaoKingdom.BadaoChampion.BadaoGangplank
{
    // 350 range
    public static class BadaoGangplankBarrels
    {
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static List<Barrel> Barrels = new List<Barrel>();
        public static int LastCastE;
        public static Vector2 LastEPos;
        public static int LastCondition;
        public static List<Barrel> SingleBarrel
        {
            get
            {
                return Barrels.Where(x => !Barrels.Any(y => y.Bottle.NetworkId != x.Bottle.NetworkId && y.Bottle.Distance(x.Bottle.Position) <= 700)).ToList();
            }
        }
        public static List<Barrel> ChainedBarrels (Barrel explodeBarrel)
        {
            var level1 = Barrels.Where(x => x.Bottle.Distance(explodeBarrel.Bottle.Position) < 700);
            var level2 = Barrels.Where(x => level1.Any(y => y.Bottle.Distance(x.Bottle.Position) < 700));
            var level3 = Barrels.Where(x => level2.Any(y => y.Bottle.Distance(x.Bottle.Position) < 700));
            return
                level3.ToList();
        }
        public static List<Barrel> AttackableBarrels (int delay = 0)
        {
            var time = Player.Level >= 13 ?
                500 :
                Player.Level >= 7 ?
                1000 :
                2000;
            var meelebarrels = Barrels.Where(x => ObjectManager.Player.InAutoAttackRange(x.Bottle)
           && (Environment.TickCount - x.CreationTime >= 2 * time - Game.Ping - Player.AttackCastDelay * 1000 + 50 - delay
               || (Environment.TickCount - x.CreationTime >=  time - Game.Ping - Player.AttackCastDelay * 1000 + 50  - delay && (int)x.Bottle.Health == 2
                   && Environment.TickCount - x.CreationTime <= time) || (false
                                                                          || (int)x.Bottle.Health == 1))).ToList();
            return meelebarrels;
        }
        public static List<Barrel> QableBarrels(int delay = 0)
        {
            var time = Player.Level >= 13 ?
                500 :
                Player.Level >= 7 ?
                1000 :
                2000;
            var qbarrels = Barrels.Where(x => BadaoMainVariables.Q.IsInRange(x.Bottle)
           && (Environment.TickCount - x.CreationTime >= 2 * time - Game.Ping - 350 + 50 - delay
               || (Environment.TickCount - x.CreationTime >= time - Game.Ping - 350 + 50 - delay && (int)x.Bottle.Health == 2
                   && Environment.TickCount - x.CreationTime < time) || (false
                                                                         || (int)x.Bottle.Health == 1))).ToList();
            return qbarrels;
        }
        public static List<Barrel> DelayedBarrels(int miliseconds)
        {
            var time = Player.Level >= 13 ?
                500 :
                Player.Level >= 7 ?
                1000 :
                2000;
            var qbarrels = Barrels.Where(x => BadaoMainVariables.Q.IsInRange(x.Bottle)
           && (Environment.TickCount - x.CreationTime >= 2 * time - Game.Ping - 350 + 50 - miliseconds
               || (Environment.TickCount - x.CreationTime >= time - Game.Ping - 350 + 50 - miliseconds && (int)x.Bottle.Health == 2
                   && Environment.TickCount - x.CreationTime < time) || (false
                                                                         || (int)x.Bottle.Health == 1))).ToList();
            return qbarrels;
        }
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Spellbook.OnCastSpell += Obj_AI_Base_OnSpellCast;
            AIBaseClient.OnDoCast += Obj_AI_Base_OnProcessSpellCast;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Obj_AI_Base_OnSpellCast(Spellbook sender,  SpellbookCastSpellEventArgs args)
        {
            if (!sender.Owner.IsMe)
                return;
            if (args.Slot != SpellSlot.Q)
                return;

        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (!(sender is AIMinionClient))
                return;
            if ((sender as AIMinionClient).Name != "Barrel")
                return;
            Barrels.RemoveAll(x => x.Bottle.NetworkId == sender.NetworkId);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var barrel in Barrels)
            {
                CircleRender.Draw(barrel.Bottle.Position, 50, Color.Green);
                if (ChainedBarrels(barrel).Count() >= 2)
                    CircleRender.Draw(barrel.Bottle.Position, 100, Color.Pink);
            }
            //var barreled = Barrels.FirstOrDefault();
            //if (barreled!= null)
            //{
            //    var barreledes = ChainedBarrels(barreled);
            //    foreach (var barrel in barreledes)
            //    {
            //        Render.Circle.DrawCircle(barrel.Bottle.Position, 150, Color.Red);
            //    }
            //}
            //var pred = Prediction.GetPrediction(Player, 2);
            //Render.Circle.DrawCircle(pred.UnitPosition, 50, Color.Pinks);
            //Render.Circle.DrawCircle(pred.CastPosition, 50, Color.Yellow);
        }

        private static void Obj_AI_Base_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;
            //Chat.Print(args.SData.Name.ToString());
            if (/*args.Slot != SpellSlot.E*/ args.SData.Name != "GangplankE")
                return;
            LastCastE = Environment.TickCount;
            LastEPos = Helps.To2D(args.To);
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (!(sender is AIMinionClient))
                return;
            if (sender.Name == "Barrel")
            {
                //if (Math.Abs(LastCastE - Environment.TickCount) < 800 && LastEPos.Distance(sender.Position) < 500)
                //{
                Barrels.Add(new Barrel() {Bottle = sender as AIMinionClient, CreationTime = Environment.TickCount});
                LastCastE = 0;
                //}
            }
        }


        private static void Game_OnUpdate(EventArgs args)
        {
            Barrels.RemoveAll(x => !x.Bottle.IsHPBarRendered);
            //if (BadaoMainVariables.Q.IsReady())
            //{
            //    foreach (var barrel in QableBarrels())
            //    {
            //        BadaoMainVariables.Q.Cast(barrel.Bottle);
            //    }
            //}
            //if (Environment.TickCount - LastCondition >= 100 + Game.Ping)
            //{
            //    foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget()))
            //    {
            //        if (BadaoMainVariables.Q.IsReady() && BadaoMainVariables.E.IsReady())
            //        {
            //            foreach (var barrel in QableBarrels())
            //            {
            //                var nbarrels = ChainedBarrels(barrel);
            //                if (nbarrels.Any(x => x.Bottle.Distance(hero.Position) <= 660 + hero.BoundingRadius)
            //                    && !nbarrels.Any(x => x.Bottle.Distance(hero.Position) <= 330 + hero.BoundingRadius))
            //                {
            //                    var pos = barrel.Bottle.Position.Extend(hero.Position, 330);
            //                    BadaoMainVariables.E.Cast(pos);
            //                    if (BadaoMainVariables.Q.Cast(barrel.Bottle) == Spell.CastStates.SuccessfullyCasted)
            //                    {
            //                        LastCondition = Environment.TickCount;
            //                        return;
            //                    }

            //                }
            //            }
            //        }
            //    }
            //    foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget()))
            //    {
            //        if (Orbwalking.CanAttack() && BadaoMainVariables.E.IsReady())
            //        {
            //            foreach (var barrel in AttackableBarrels())
            //            {
            //                var nbarrels = ChainedBarrels(barrel);
            //                if (nbarrels.Any(x => x.Bottle.Distance(hero.Position) <= 660 + hero.BoundingRadius)
            //                    && !nbarrels.Any(x => x.Bottle.Distance(hero.Position) <= 330 + hero.BoundingRadius))
            //                {
            //                    var pos = barrel.Bottle.Position.Extend(hero.Position, 330);
            //                    BadaoMainVariables.E.Cast(pos);
            //                    if (EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, barrel.Bottle))
            //                    {
            //                        LastCondition = Environment.TickCount;
            //                        return;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget()))
            //    {
            //        if (BadaoMainVariables.Q.IsReady())
            //        {
            //            foreach (var barrel in QableBarrels())
            //            {
            //                var nbarrels = ChainedBarrels(barrel);
            //                if (nbarrels.Any(x => x.Bottle.Distance(hero.Position) <= 330 + hero.BoundingRadius))
            //                {
            //                    if (BadaoMainVariables.Q.Cast(barrel.Bottle) == Spell.CastStates.SuccessfullyCasted)
            //                    {
            //                        LastCondition = Environment.TickCount;
            //                        return;
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget()))
            //    {
            //        if (Orbwalking.CanAttack())
            //        {
            //            foreach (var barrel in AttackableBarrels())
            //            {
            //                var nbarrels = ChainedBarrels(barrel);
            //                if (nbarrels.Any(x => x.Bottle.Distance(hero.Position) <= 330 + hero.BoundingRadius))
            //                {
            //                    if (EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, barrel.Bottle))
            //                    {
            //                        LastCondition = Environment.TickCount;
            //                        return;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        }
        public class Barrel
        {
            public AIMinionClient Bottle;
            public int CreationTime;
        }
    }
}