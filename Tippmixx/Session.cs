using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tippmixx
{
    public static class Session
    {
        public static string Username { get; set; }
        public static string Password { get; set; }
        public static string Email { get; set; }
        public static int Balance { get; set; }
        public static bool IsActive { get; set; }
        public static string Class {  get; set; }

    }

}