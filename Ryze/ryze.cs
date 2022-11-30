using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;

namespace program
{
    internal class ryze{
        private static Spell Q,W,E,R;
        private static Menu mainMenu;
        public static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }
        public static void OnGameLoad(){
            if(GameObjects.Player.CharacterName != "Ryze") return;
            Q = new Spell(SpellSlot.Q,1000f);
            W = new Spell(SpellSlot.W, 550f);
            E = new Spell(SpellSlot.E, 550f);
            R = new Spell(SpellSlot.R);
            Q.SetSkillshot(.25f,110f,1700f,true,SpellType.Line);
            W.SetTargetted(0.25f,float.MaxValue);
            E.SetTargetted(0,float.MaxValue);
            
         
          
            mainMenu = new Menu("ryze","Simpleryze",true);
            var Combo = new Menu("Combo","Combo Settings");
            Combo.Add(new MenuBool("Quse","Use Q",true));
            Combo.Add(new MenuBool("Wuse","Use W",true));
            Combo.Add(new MenuBool("Euse","Use E",true));
            Combo.Add(new MenuBool("Ruse","Use R",true));
            Combo.Add(new MenuBool("Qstart","wait to hit q alwas",true));
            mainMenu.Add(Combo);
            var Harass = new Menu("Harass","Harass Settings");
            Harass.Add(new MenuBool("Quse","Use Q",true));
            Harass.Add(new MenuBool("Wuse","Use W",true));
            Harass.Add(new MenuSlider("mana%","Mana porcent",50,0,100));
            mainMenu.Add(Harass);
        
            var Draw = new Menu("Draw","Draw Settings");
            Draw.Add(new MenuBool("qRange","Draw Q range",true));
            Draw.Add(new MenuBool("wRange","Draw W range",true));
            Draw.Add(new MenuBool("eRange","Draw E range",true));
            Draw.Add(new MenuBool("lista","Draw only if spell is ready",true));
            mainMenu.Add(Draw);

            mainMenu.Attach();
            GameEvent.OnGameTick += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
           
   
           
        }
        
        
        private static void ComboLogic()
        {
            var startWithQ = !Q.IsReady() || !mainMenu["Combo"].GetValue<MenuBool>("Qstart").Enabled;
            var coinChargeAmount = ObjectManager.Player.HasBuff("ryzeqiconnocharge") ? 0 : ObjectManager.Player.HasBuff("ryzeqiconhalfcharge") ? 1 : 2;
            var target = TargetSelector.GetTarget(-1,DamageType.Magical);
            var qTarget = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var castQ = coinChargeAmount == 0 || coinChargeAmount == 2 || !W.IsReady() && !E.IsReady() || target == null;
            //ObjectManager.Player.HasBuff("RyzeE")
            if (castQ && Q.IsReady() && qTarget != null)
            {
                if (Q.Cast(qTarget) ==  CastStates.SuccessfullyCasted)
                {
                    return;
                }
            }

            if (target != null && startWithQ)
            {
                if (!W.IsReady())
                {
                    if (E.Cast(target) ==  CastStates.SuccessfullyCasted)
                    {
                        return;
                    }
                }

                if (W.Cast(target) ==  CastStates.SuccessfullyCasted)
                {
                    return;
                }

            
            }
        }
        
            
        private static void HarassLogic()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var inputQ = Q.GetPrediction(target);
            
           if(mainMenu["Harass"].GetValue<MenuSlider>("mana%").Value <= GameObjects.Player.ManaPercent)
           {
                 if(mainMenu["Harass"].GetValue<MenuBool>("Quse").Enabled)
                 {
                     if(Q.IsReady() && target.IsValidTarget()&& inputQ.Hitchance >= HitChance.VeryHigh)
                     {
                        Q.Cast(inputQ.CastPosition);
                     }
                 }
           }
        }

       

        private static void OnGameUpdate(EventArgs args){
            if(GameObjects.Player.IsDead) return;
            
            switch (Orbwalker.ActiveMode){
                case OrbwalkerMode.Combo:
                    ComboLogic();
                    break;
                    case OrbwalkerMode.Harass:
                    HarassLogic();
                    break;
                case OrbwalkerMode.LaneClear:
                   
                    break;
            }
        }
        private static void OnDraw(EventArgs args){
            if(mainMenu["Draw"].GetValue<MenuBool>("lista").Enabled){
                if(mainMenu["Draw"].GetValue<MenuBool>("qRange").Enabled){
                    if(Q.IsReady()){
                        Render.Circle.DrawCircle(GameObjects.Player.Position, Q.Range,System.Drawing.Color.Cyan,1);
                    }
                }
                if(mainMenu["Draw"].GetValue<MenuBool>("wRange").Enabled){
                    if(W.IsReady()){
                        Render.Circle.DrawCircle(GameObjects.Player.Position, W.Range,System.Drawing.Color.Silver,1);
                    }
                }
                if(mainMenu["Draw"].GetValue<MenuBool>("eRange").Enabled){
                    if(E.IsReady()){
                        Render.Circle.DrawCircle(GameObjects.Player.Position, E.Range,System.Drawing.Color.Yellow,1);
                    }

                }
            }
        }
    }
}
