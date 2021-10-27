using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GuessTheDate.Models
{
    public class Quiz
    {
        public string Event { get; set; }

        public int EventYear { get; set; }

        [Display(Name ="Year of the event:")]
        [Required(ErrorMessage = "Try to guess the date of the above event.")]
        public int Answer { get; set; }
    }
}
