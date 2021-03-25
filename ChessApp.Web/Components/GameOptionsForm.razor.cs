using ChessApp.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessApp.Web.Components
{
    public class GameOptionsFormBase: ComponentBase
    {
        public GameOptionsFormBase()
        {
            
        }
        public GameOptions GameOptions = new GameOptions();

        protected void onSubmitClinked()
        {
            Console.WriteLine("sub");
        }

        protected void onCancelClicked()
        {
            Console.WriteLine("can");
        }
    }
}
