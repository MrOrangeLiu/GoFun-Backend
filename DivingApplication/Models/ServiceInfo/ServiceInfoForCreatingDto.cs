using DivingApplication.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.ServiceInfo
{
    public class ServiceInfoForCreatingDto
    {

        public List<string> ServiceImageUrls { get; set; }

        [MaxLength(128)]
        public string CenterName { get; set; }

        [MaxLength(128)]
        public string LocalCenterName { get; set; }

        [MaxLength(64)]
        public string PhoneNumber { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }


        [MaxLength(2048)]
        public string Website { get; set; }

        [MaxLength(256)]
        public string SocailMedia { get; set; }

        [MaxLength(256)]
        public string SocialMediaAccount { get; set; }
        public DateTime CenterOpeningDate { get; set; }

        [MaxLength(2048)]
        public string LocationAddress { get; set; }
        public Place Place { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }

        [MaxLength(1024)]
        public string CenterDescription { get; set; }
        public string ServiceCenterType { get; set; }
        public bool ProvideAccommodation { get; set; }
        public List<string> ProvidServices { get; set; }
        public List<string> CenterFacilites { get; set; }
        public List<string> DiveAssociations { get; set; }
        public List<string> SupportedLanguages { get; set; }
        public List<string> SupportedPayment { get; set; }
    }
}
