using System;

namespace Vookaba.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TooltipAttribute : Attribute
    {
        public TooltipAttribute(string toolTip)
        {
            Tooltip = toolTip;
        }

        public string Tooltip { get; }
    }
}
