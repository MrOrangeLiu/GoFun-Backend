using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Entities
{
    public class Place
    {
        [Key]
        public Guid Id { get; set; }
        public string AdministrativeArea { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Locality { get; set; }
        public string Name { get; set; }
        public string PostalCode { get; set; }
        public string SubAdministrativeArea { get; set; }
        public string SubLocality { get; set; }
        public string SubThoroughfare { get; set; }
        public string Thoroughfare { get; set; }
    }
}
