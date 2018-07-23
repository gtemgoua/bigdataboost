using BigDataBoost.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BigDataBoost.Data.Abstract
{
    public interface ITagDefRepository : IEntityBaseRepository<TagDef> { }
    public interface ITagHistRepository : IEntityBaseRepository<TagHist> { }
}
