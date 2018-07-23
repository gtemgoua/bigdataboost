using System;
using System.Collections.Generic;
using System.Text;

namespace BigDataBoost.Model
{
    public class TagHist : IEntityBase
    {
        public int Id { get; set; }

        public int TagDefId { get; set; }
        public string TagName { get; set; }
        public DateTime TimeStamp { get; set; }
        public double Value { get; set; }
        public TagStatus Status { get; set; }
    }
}
