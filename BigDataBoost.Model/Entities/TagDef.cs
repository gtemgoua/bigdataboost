using System;
using System.Collections.Generic;
using System.Text;

namespace BigDataBoost.Model
{
    public class TagDef : IEntityBase
    {
        public int Id { get; set; }

        /// <summary>
        /// Represents the source of the data collected
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// Represents the Tag Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Represents the short Description of a Tag
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Longer description of the Tag read
        /// </summary>
        public string ExtendedDescription { get; set; }
        /// <summary>
        /// Value read for a given Tag
        /// </summary>
        public double Value { get; set; }
        /// <summary>
        /// Status of the Readings from the source
        /// </summary>
        public TagStatus Status { get; set; }
        /// <summary>
        /// Time at wich the measurement is collected
        /// </summary>
        public DateTime TimeStamp { get; set; }


    }
}
