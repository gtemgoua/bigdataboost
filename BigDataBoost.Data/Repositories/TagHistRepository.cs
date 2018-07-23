using BigDataBoost.Data.Abstract;
using BigDataBoost.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BigDataBoost.Data.Repositories
{
    public class TagHistRepository:EntityBaseRepository<TagHist>, ITagHistRepository
    {
        public TagHistRepository(BigDataBoostContext context)
            : base(context)
        { }
    }
}
