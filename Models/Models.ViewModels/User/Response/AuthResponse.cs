using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels.User.Response
{
    public class AuthResponse
    {
        public bool IsCompleted { get; set; }
        public string Message { get; set; }
        public string? NextStep { get; set; }
        public bool IsMigrated { get; set; } = false;

    }
}
