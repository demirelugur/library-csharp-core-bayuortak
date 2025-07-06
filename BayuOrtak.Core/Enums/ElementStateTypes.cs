namespace BayuOrtak.Core.Enums
{
    using BayuOrtak.Core.Extensions;
    using Microsoft.AspNetCore.Mvc.Rendering;
    /// <summary>
    /// <see cref="RazorExtensions.SetHtmlAttribute(IHtmlHelper, bool, ElementStateTypes)"/> için oluşturulmuş enum
    /// </summary>
    public enum ElementStateTypes : byte
    {
        Checked = 1,
        Selected,
        Disabled
    }
}