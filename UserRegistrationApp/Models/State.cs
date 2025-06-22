using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UserRegistrationApp.Models
{
    public partial class State
    {
        public State()
        {
            Cities = new HashSet<City>();
            Users = new HashSet<User>();
        }

        [Key]
        public int StateId { get; set; }
        public string StateName { get; set; }

        public virtual ICollection<City> Cities { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
} 