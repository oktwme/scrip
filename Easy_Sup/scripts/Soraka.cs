using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Utility;
using SharpDX;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

namespace Easy_Sup.scripts
{
    class Soraka
    {
        private static Spell Q, W, E, R;
        private static Menu IsMenu;
        public static int focus;
        private static EnsoulSharp.SDK.Geometry.Rectangle QRectangle { get; set; }
        private static readonly Dictionary<int, List<Vector2>> _waypoints = new Dictionary<int, List<Vector2>>();

        internal static void Load()
        {
            if (GameObjects.Player.CharacterName != "Soraka") return;
            Q = new Spell(SpellSlot.Q, 970);
            W = new Spell(SpellSlot.W, 550);
            E = new Spell(SpellSlot.E, 925);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.283f, 210, 1100, false, SpellType.Circle);
            W.SetTargetted(0.4f, 210);
            E.SetSkillshot(0.4f, 70f, 1750, false, SpellType.Circle);

            LoadMenu();
            Game.OnUpdate += OnTick;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnTick(EventArgs args)
        {
            if (ObjectManager.Player.IsDead) return;

            AutoW();
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    break;
            }
        }


        private static void AutoW()
        {
            
            if (!W.IsReady())
            {
                return;
            }

            if (ObjectManager.Player.HealthPercent < Menubase.soraka_heal.myHeal.Value)
            {
                return;
            }

            var al = GameObjects.AllyHeroes.Where(p => p.IsAlly && p.DistanceToPlayer() < W.Range && p.HealthPercent <= Menubase.soraka_heal.allyHeal.Value);
            var alvos = GameObjects.AllyHeroes.Where(x => x.IsAlly && x.IsValidTarget(W.Range) && x.HealthPercent <= Menubase.soraka_heal.allyHeal.Value && !x.IsRecalling()).Cast<AIHeroClient>().ToList();
            switch (focus)
            {
                case 0:
                    alvos = al.OrderByDescending(x => x.TotalAttackDamage).ToList();
                    break;
                case 1:
                    alvos = al.OrderByDescending(x => x.TotalMagicalDamage).ToList();
                    break;
                case 2:
                    alvos = al.OrderBy(x => x.Health).ToList();
                    break;
            }
            var target = al.FirstOrDefault(x => !x.InFountain() && x.CharacterName != "Soraka");
            if (target != null && !ObjectManager.Player.IsRecalling() && Menubase.soraka_heal.W.Enabled)
            {
                W.Cast(target);
            }
        }

        internal static void OnLoad()
        {
            throw new NotImplementedException();
        }

        private static void Combo()
        {
            var useQ = Menubase.soraka_combat.Q;
            var useE = Menubase.soraka_combat.E;

            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target == null)
            {
                return;
            }

            var prediction = GetQPrediction(target);
            if (Q.IsReady() && ObjectManager.Player.Distance(target.Position) < Q.Range)
            {
                Q.Cast(prediction.CastPosition);
            }

            if (E.IsReady() && ObjectManager.Player.Distance(target.Position) < Q.Range)
            {
                E.Cast(target);
            }
        }

        private static void Harass()
        {
            var useQ = Menubase.soraka_harass.Qh;
            var useE = Menubase.soraka_harass.Eh;

            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target == null)
            {
                return;
            }

            var prediction = GetQPrediction(target);
            if (Q.IsReady() && ObjectManager.Player.Distance(target.Position) < Q.Range)
            {
                Q.Cast(prediction.CastPosition);
            }

            if (E.IsReady() && ObjectManager.Player.Distance(target.Position) < E.Range)
            {
                E.Cast(target);
            }
        }

        public static PredictionOutput GetQPrediction(AIBaseClient target)
        {
            float divider = target.Position.Distance(ObjectManager.Player.Position) / Q.Range;
            Q.Delay = 0.2f + 0.8f * divider;
            var prediction = Q.GetPrediction(target, true);
            return prediction;
        }

        public static void LoadMenu()
        {
            IsMenu = new Menu("Easy_Sup.Soraka", "Easy_Sup.Soraka", true);
            var Combo = new Menu("Combo", "Combo Config");
            Combo.Add(Menubase.soraka_combat.Q);
            Combo.Add(Menubase.soraka_combat.E);

            var harass = new Menu("Harass", "Harass Config");
            harass.Add(Menubase.soraka_harass.Qh);
            harass.Add(Menubase.soraka_harass.Eh);



            var heal = new Menu("Heal", "Heal Config");
            heal.Add(Menubase.soraka_heal.W);
            heal.Add(Menubase.soraka_heal.allyHeal);
            heal.Add(Menubase.soraka_heal.myHeal);
            heal.Add(new MenuList("wuse", "Healing Priority(Press F5 after change) : ", new[] { "Most AD", "Most AP", " Lower HP%" }));


            var Draw = new Menu("Draw", "Draw Spells");
            Draw.Add(Menubase.soraka_draw.Dq);
            Draw.Add(Menubase.soraka_draw.Dw);
            Draw.Add(Menubase.soraka_draw.De);

            IsMenu.Add(Combo);
            IsMenu.Add(harass);
            IsMenu.Add(heal);
            IsMenu.Add(Draw);
            IsMenu.Attach();

            switch (heal.GetValue<MenuList>("wuse").Index)
            {
                case 0:
                    focus = 0;
                    break;
                case 1:
                    focus = 1;
                    break;
                case 2:
                    focus = 2;
                    break;
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || ObjectManager.Player.IsRecalling() || MenuGUI.IsChatOpen || ObjectManager.Player.IsWindingUp)
            {
                return;
            }
            if (Menubase.soraka_draw.Dq.Enabled && Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.LightGreen);
            }
            if (Menubase.soraka_draw.Dw.Enabled && W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.LightGreen);
            }
            if (Menubase.soraka_draw.De.Enabled && E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.LightGreen);
            }
        }
    }
}
