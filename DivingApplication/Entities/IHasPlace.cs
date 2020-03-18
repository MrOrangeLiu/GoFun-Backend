using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Entities
{
    public interface IHasPlace
    {
        public Place Place { get; set; }
    }
}
