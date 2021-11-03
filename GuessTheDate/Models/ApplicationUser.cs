using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuessTheDate.Models
{
    public class ApplicationUser : IdentityUser
    {
        public long ExPoints { get; set; }
        public int Level { get; set; }
        public long PointsNextLevel { get; set; }
    }
}
