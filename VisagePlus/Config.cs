﻿using System;
using System.Windows.Input;

using Ensage;
using Ensage.Common.Menu;
using Ensage.Common.Objects.UtilityObjects;

using SharpDX;

using VisagePlus.Features;

namespace VisagePlus
{
    internal class Config : IDisposable
    {
        public VisagePlus Main { get; }

        public Vector2 Screen { get; }

        public MultiSleeper MultiSleeper { get; }

        public MenuManager Menu { get; }

        public Extensions Extensions { get; }

        public UpdateMode UpdateMode { get; }

        public DamageCalculation DamageCalculation { get; }

        public LinkenBreaker LinkenBreaker { get; }

        private AutoKillSteal AutoKillSteal { get; }

        private FamiliarsControl FamiliarsControl { get; }

        private FamiliarsCombo FamiliarsCombo { get; }

        private FamiliarsLastHit FamiliarsLastHit { get; }

        private AutoAbility AutoAbility { get; }

        private Mode Mode { get; }

        private Renderer Renderer { get; }

        private bool Disposed { get; set; }

        public Config(VisagePlus main)
        {
            Main = main;

            ActivatePlugins();
            Screen = new Vector2(Drawing.Width - 160, Drawing.Height);
            MultiSleeper = new MultiSleeper();

            Menu = new MenuManager(this);
            Extensions = new Extensions(this);

            UpdateMode = new UpdateMode(this);
            DamageCalculation = new DamageCalculation(this);
            LinkenBreaker = new LinkenBreaker(this);
            AutoKillSteal = new AutoKillSteal(this);
            FamiliarsControl = new FamiliarsControl(this);
            FamiliarsCombo = new FamiliarsCombo(this);
            FamiliarsLastHit = new FamiliarsLastHit(this);
            AutoAbility = new AutoAbility(this);

            Menu.ComboKeyItem.Item.ValueChanged += ComboKeyChanged;
            var key = KeyInterop.KeyFromVirtualKey((int)Menu.ComboKeyItem.Value.Key);
            Mode = new Mode(key, this);
            Main.Context.Orbwalker.RegisterMode(Mode);

            Renderer = new Renderer(this);
        }

        private void ActivatePlugins()
        {
            var orbwalker = Main.Context.Orbwalker;
            if (!orbwalker.IsActive)
            {
                orbwalker.Activate();
            }

            var targetSelector = Main.Context.TargetSelector;
            if (!targetSelector.IsActive)
            {
                targetSelector.Activate();
            }

            var prediction = Main.Context.Prediction;
            if (!prediction.IsActive)
            {
                prediction.Activate();
            }
        }

        private void ComboKeyChanged(object sender, OnValueChangeEventArgs e)
        {
            Menu.FollowKeyItem.Item.SetValue(new KeyBind(Menu.FollowKeyItem.Value, KeyBindType.Toggle));
            Menu.LastHitItem.Item.SetValue(new KeyBind(Menu.LastHitItem.Value, KeyBindType.Toggle));

            var keyCode = e.GetNewValue<KeyBind>().Key;
            if (keyCode == e.GetOldValue<KeyBind>().Key)
            {
                return;
            }

            var key = KeyInterop.KeyFromVirtualKey((int)keyCode);
            Mode.Key = key;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }

            if (disposing)
            {
                Renderer.Dispose();

                Main.Context.Orbwalker.UnregisterMode(Mode);
                Menu.ComboKeyItem.Item.ValueChanged -= ComboKeyChanged;

                AutoAbility.Dispose();
                FamiliarsLastHit.Dispose();
                FamiliarsCombo.Dispose();
                FamiliarsControl.Dispose();
                AutoKillSteal.Dispose();
                DamageCalculation.Dispose();
                UpdateMode.Dispose();

                Main.Context.Particle.Dispose();

                Menu.Dispose();
            }

            Disposed = true;
        }
    }
}
