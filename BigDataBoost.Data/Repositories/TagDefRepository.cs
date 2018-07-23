using BigDataBoost.Data.Abstract;
using BigDataBoost.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BigDataBoost.Data.Repositories
{
    public class TagDefRepository : EntityBaseRepository<TagDef>, ITagDefRepository
    {
        public TagDefRepository(BigDataBoostContext context)
            : base(context)
        { }
    }
}
