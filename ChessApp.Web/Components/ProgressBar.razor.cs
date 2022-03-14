using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ChessApp.Web.Components
{
    public partial class ProgressBar: ComponentBase
    {
        [Parameter] public bool IsLoading { get; set; }
        [Parameter] public bool IsError { get; set; }
    }
}
