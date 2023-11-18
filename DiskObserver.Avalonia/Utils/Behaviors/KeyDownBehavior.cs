using Avalonia.Input;
using Avalonia;
using Avalonia.Reactive;
using Avalonia.Xaml.Interactivity;
using System;
using System.Globalization;
using System.Reflection;

namespace DiskObserver.Avalonia.Utils.Behaviors {
    public class KeyDownBehavior : Trigger {
        public static readonly StyledProperty<Key?> KeyProperty =
            AvaloniaProperty.Register<KeyDownBehavior, Key?>(nameof(Key));

        public Key? Key {
            get => GetValue(KeyProperty);
            set => SetValue(KeyProperty, value);
        }

        static KeyDownBehavior() {
            KeyProperty.Changed.Subscribe(
                new AnonymousObserver<AvaloniaPropertyChangedEventArgs<Key?>>(KeyChangedProperty));
        }

        private static void KeyChangedProperty(AvaloniaPropertyChangedEventArgs<Key?> e) {
            if (e.Sender is not KeyDownBehavior behavior) {
                return;
            }

            var oldKeyName = e.OldValue.GetValueOrDefault();
            var newKeyName = e.NewValue.GetValueOrDefault();

            if (oldKeyName is { }) {
                behavior.UnregisterEvent(oldKeyName);
            }

            if (newKeyName is { }) {
                behavior.RegisterEvent(newKeyName);
            }
        }


        private object? _resolvedSource;
        private Delegate? _eventHandler;

        /// <summary>
        /// Called after the behavior is attached to the <see cref="Behavior.AssociatedObject"/>.
        /// </summary>
        protected override void OnAttached() {
            base.OnAttached();
            SetResolvedSource(AssociatedObject);

            foreach (var item in Actions) {
                if (item is IBehavior behavior) {
                    behavior.Attach(AssociatedObject);
                }
            }
        }

        /// <summary>
        /// Called when the behavior is being detached from its <see cref="Behavior.AssociatedObject"/>.
        /// </summary>
        protected override void OnDetaching() {
            base.OnDetaching();
            SetResolvedSource(null);

            foreach (var item in Actions) {
                if (item is IBehavior behavior) {
                    behavior.Detach();
                }
            }
        }

        private void SetResolvedSource(object? newSource) {
            if (AssociatedObject is null || _resolvedSource == newSource) {
                return;
            }

            if (_resolvedSource is { }) {
                UnregisterEvent(Key);
            }

            _resolvedSource = newSource;

            if (_resolvedSource is { }) {
                RegisterEvent(Key);
            }
        }

        private void RegisterEvent(Key? key) {
            if (key == null) {
                return;
            }

            if (_resolvedSource is null) {
                return;
            }

            var sourceObjectType = _resolvedSource.GetType();
            var eventInfo = sourceObjectType.GetRuntimeEvent("KeyDown");
            if (eventInfo is null) {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    "Cannot find an event named KeyDown on type {1}.",
                    sourceObjectType.Name));
            }

            var methodInfo = typeof(KeyDownBehavior).GetTypeInfo().GetDeclaredMethod("AttachedToVisualTree");
            if (methodInfo is { }) {
                var eventHandlerType = eventInfo.EventHandlerType;
                if (eventHandlerType is { }) {
                    _eventHandler = methodInfo.CreateDelegate(eventHandlerType, this);
                    if (_eventHandler is { }) {
                        eventInfo.AddEventHandler(_resolvedSource, _eventHandler);
                    }
                }
            }
        }

        private void UnregisterEvent(Key? key) {
            if (key == null) {
                return;
            }

            if (_eventHandler is null) {
                return;
            }

            if (_resolvedSource is { }) {
                var eventInfo = _resolvedSource.GetType().GetRuntimeEvent("KeyDown");
                eventInfo?.RemoveEventHandler(_resolvedSource, _eventHandler);
            }
            _eventHandler = null;
        }

        /// <summary>
        /// Raised when the control is attached to a rooted visual tree.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="eventArgs">The event args.</param>
        protected virtual void AttachedToVisualTree(object? sender, object eventArgs) {
            if (eventArgs is KeyEventArgs keyEventArgs && keyEventArgs.Key == Key) {
                Interaction.ExecuteActions(_resolvedSource, Actions, eventArgs);
            }
        }
    }

}
