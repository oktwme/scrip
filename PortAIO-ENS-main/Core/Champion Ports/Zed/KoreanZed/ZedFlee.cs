﻿using System;
using EnsoulSharp;

namespace KoreanZed
{
    class ZedFlee
    {
        private readonly ZedMenu zedMenu;

        private readonly ZedShadows zedShadows;

        public ZedFlee(ZedMenu zedMenu, ZedShadows zedShadows)
        {
            this.zedMenu = zedMenu;
            this.zedShadows = zedShadows;

            Game.OnUpdate += Game_OnUpdate;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (zedMenu.GetParamKeyBind("koreanzed.miscmenu.flee"))
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                zedShadows.Cast(Game.CursorPos);
                zedShadows.Switch();
            }
        }
    }
}