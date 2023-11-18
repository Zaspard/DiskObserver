using Avalonia.Xaml.Interactivity;
using Avalonia;
using System;
using System.Linq;
using System.Reflection;
using Avalonia.Data;
using Avalonia.Reactive;
using Avalonia.Input;
using Avalonia.Interactivity;


namespace DiskObserver.Avalonia.Utils.Behaviors
{
    public class UpdateValueInProperty : AvaloniaObject, IBehavior, IAction
    {
        public static readonly StyledProperty<string?> PropetryNameProperty =
                AvaloniaProperty.Register<UpdateValueInProperty, string?>(nameof(PropertyName));

        public string? PropertyName
        {
            get => GetValue(PropetryNameProperty);
            set => SetValue(PropetryNameProperty, value);
        }

        public static readonly StyledProperty<object?> ValueProperty =
            AvaloniaProperty.Register<UpdateValueInProperty, object?>(nameof(Value), defaultBindingMode: BindingMode.TwoWay);

        public object? Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly StyledProperty<Type> TypeProperty =
            AvaloniaProperty.Register<UpdateValueInProperty, Type>(nameof(Type), defaultValue: typeof(string));

        public Type Type
        {
            get => GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public AvaloniaObject? AssociatedObject { get; set; }
        static UpdateValueInProperty()
        {
            ValueProperty.Changed.Subscribe(
                new AnonymousObserver<AvaloniaPropertyChangedEventArgs<object?>>(KeyChangedProperty));
        }

        public object Execute(object? sender, object? parameter)
        {
            return SetValueInModel();
        }

        void InputElement_LostFocus(object? sender, RoutedEventArgs e)
        {
            SetValueInModel();
        }

        bool SetValueInModel()
        {
            if (AssociatedObject == null || PropertyName == null)
            {
                return false;
            }

            var avaloniaProperty = GetAvaloniaProperty(AssociatedObject, PropertyName);
            if (avaloniaProperty != null && TryConvertValue(AssociatedObject.GetValue(avaloniaProperty), Type, out var baseValue))
            {
                SetCurrentValue(ValueProperty, baseValue);
            }
            else
            {
                avaloniaProperty = GetAvaloniaProperty(AssociatedObject, PropertyName);
                if (avaloniaProperty != null && TryConvertValue(Value, avaloniaProperty.PropertyType, out var result))
                {
                    AssociatedObject.SetCurrentValue(avaloniaProperty, result);
                }
            }

            return true;
        }

        public void Attach(AvaloniaObject? associatedObject)
        {
            AssociatedObject = associatedObject;
            if (AssociatedObject is InputElement inputElement)
            {
                inputElement.LostFocus += InputElement_LostFocus;
            }
        }


        public void Detach()
        {
            if (AssociatedObject is InputElement inputElement)
            {
                inputElement.LostFocus -= InputElement_LostFocus;
            }
            AssociatedObject = null;
        }

        static void KeyChangedProperty(AvaloniaPropertyChangedEventArgs<object?> e)
        {
            if (e.Sender is not UpdateValueInProperty behavior || behavior.AssociatedObject == null)
            {
                return;
            }

            object? newValue = e.NewValue.Value;
            if (behavior.AssociatedObject == null || string.IsNullOrEmpty(behavior.PropertyName))
            {
                return;
            }

            var avaloniaProperty = GetAvaloniaProperty(behavior.AssociatedObject, behavior.PropertyName);
            if (avaloniaProperty != null && TryConvertValue(newValue, avaloniaProperty.PropertyType, out var result))
            {
                behavior.AssociatedObject.SetCurrentValue(avaloniaProperty, result);
            }
        }


        static AvaloniaProperty? GetAvaloniaProperty(object? sender, string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return null;
            }

            Type? type = sender?.GetType();
            var fields = type?.GetFields();
            FieldInfo? field = fields?.FirstOrDefault(x => x.Name.Contains($"{propertyName}Property"));
            object? value = field?.GetValue(sender);
            return value as AvaloniaProperty;
        }

        static bool TryConvertValue(object? value, Type type, out object? result)
        {
            if (value == null)
            {
                result = null;
                return true;
            }


            //Говнокод
            try
            {
                Type realType = GetRealType(type);
                if (realType == typeof(string))
                {
                    result = value.ToString();
                }
                else if (realType == typeof(int))
                {
                    result = Convert.ToInt32(value);
                }
                else if (realType == typeof(double))
                {
                    result = Convert.ToDouble(value);
                }
                else if (realType == typeof(long))
                {
                    result = Convert.ToInt64(value);
                }
                else if (realType == typeof(decimal))
                {
                    result = Convert.ToDecimal(value);
                }
                else
                {
                    result = value;
                }
            }
            catch (Exception ex)
            {
                _ = ex;
                result = null;
                return false;
            }
            return true;
        }

        private static Type GetRealType(Type type)
        {
            if (type.IsGenericType)
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }
    }

}
