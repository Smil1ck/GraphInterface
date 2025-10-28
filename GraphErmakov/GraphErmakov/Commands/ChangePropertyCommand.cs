// GraphErmakov/Commands/ChangePropertyCommand.cs
using System;

namespace GraphErmakov.Commands
{
    public class ChangePropertyCommand<T> : ICommand
    {
        private readonly object _target;
        private readonly string _propertyName;
        private readonly T _oldValue;
        private readonly T _newValue;

        public ChangePropertyCommand(object target, string propertyName, T newValue)
        {
            _target = target;
            _propertyName = propertyName;
            _newValue = newValue;

            var property = _target.GetType().GetProperty(_propertyName);
            _oldValue = (T)property.GetValue(_target);
        }

        public void Execute()
        {
            var property = _target.GetType().GetProperty(_propertyName);
            property.SetValue(_target, _newValue);
        }

        public void Undo()
        {
            var property = _target.GetType().GetProperty(_propertyName);
            property.SetValue(_target, _oldValue);
        }
    }
}