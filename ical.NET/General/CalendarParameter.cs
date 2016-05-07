using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using ical.NET.Collections;
using Ical.Net.Interfaces.General;

namespace Ical.Net.General
{
    [DebuggerDisplay("{Name}={string.Join(\",\", Values)}")]
    [Serializable]
    public class CalendarParameter : CalendarObject, ICalendarParameter
    {
        #region Private Fields

        List<string> _values;

        #endregion

        #region Constructors

        public CalendarParameter()
        {
            Initialize();
        }

        public CalendarParameter(string name) : base(name)
        {
            Initialize();
        }

        public CalendarParameter(string name, string value) : base(name)
        {
            Initialize();
            AddValue(value);
        }

        public CalendarParameter(string name, IEnumerable<string> values) : base(name)
        {
            Initialize();
            foreach (var v in values)
            {
                AddValue(v);
            }
        }

        void Initialize()
        {
            _values = new List<string>();
        }

        #endregion

        #region Overrides

        protected override void OnDeserializing(StreamingContext context)
        {
            base.OnDeserializing(context);

            Initialize();
        }

        public override void CopyFrom(ICopyable c)
        {
            base.CopyFrom(c);

            var p = c as ICalendarParameter;
            if (p != null)
            {
                if (p.Values != null)
                {
                    _values = new List<string>(p.Values);
                }
            }
        }

        #endregion

        #region IValueObject<string> Members

        [field: NonSerialized]
        public event EventHandler<ValueChangedEventArgs<string>> ValueChanged;

        protected void OnValueChanged(IEnumerable<string> removedValues, IEnumerable<string> addedValues)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, new ValueChangedEventArgs<string>(removedValues, addedValues));
            }
        }

        public virtual IEnumerable<string> Values => _values;

        public virtual bool ContainsValue(string value)
        {
            return _values.Contains(value);
        }

        public virtual int ValueCount => _values != null ? _values.Count : 0;

        public virtual void SetValue(string value)
        {
            if (_values.Count == 0)
            {
                // Our list doesn't contain any values.  Let's add one!
                _values.Add(value);
                OnValueChanged(null, new[] {value});
            }
            else if (value != null)
            {
                // Our list contains values.  Let's set the first value!
                var oldValue = _values[0];
                _values[0] = value;
                OnValueChanged(new[] {oldValue}, new[] {value});
            }
            else
            {
                // Remove all values
                var values = new List<string>(Values);
                _values.Clear();
                OnValueChanged(values, null);
            }
        }

        public virtual void SetValue(IEnumerable<string> values)
        {
            // Remove all previous values
            var removedValues = _values.ToList();
            _values.Clear();
            _values.AddRange(values);
            OnValueChanged(removedValues, values);
        }

        public virtual void AddValue(string value)
        {
            if (value != null)
            {
                _values.Add(value);
                OnValueChanged(null, new[] {value});
            }
        }

        public virtual void RemoveValue(string value)
        {
            if (value != null && _values.Contains(value) && _values.Remove(value))
            {
                OnValueChanged(new[] {value}, null);
            }
        }

        #endregion

        #region ICalendarParameter Members

        public virtual string Value
        {
            get
            {
                if (Values != null)
                {
                    return Values.FirstOrDefault();
                }
                return default(string);
            }
            set { SetValue(value); }
        }

        #endregion
    }
}