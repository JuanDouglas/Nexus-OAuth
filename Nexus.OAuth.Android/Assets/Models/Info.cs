using System;

namespace Nexus.OAuth.Android.Assets.Models
{
    internal struct Info
    {
        public string Name { get; private set; }
        public string Value { get; private set; }
        public bool Disabled { get; private set; }

        public Info(string name, string value) : this(name, value, false)
        {

        }
        public Info(string name, string value, bool disabled)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Disabled = disabled;
        }
    }
}