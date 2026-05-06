using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UP._01._01_ShutIKrol.Services
{
    public static class CurrentUser
    {
        public static Users User { get; set; }
        public static bool IsAuthenticated => User != null;
        public static bool IsAdmin => User?.Roles?.RoleName == "Администратор";
        public static bool IsAuthor => User?.Roles?.RoleName == "Автор";
        public static bool IsFrozen => User?.IsFrozen ?? false;
    }
}
