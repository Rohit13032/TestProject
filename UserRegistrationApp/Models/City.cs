using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UserRegistrationApp.Models
{
    public partial class City
    {
        public City()
        {
            Users = new HashSet<User>();
        }

        [Key]
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int StateId { get; set; }

        public virtual State State { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
} 