using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BigDataBoost.Data.Abstract;
using BigDataBoost.Model;
using BigDataBoost.API.ViewModels;
using AutoMapper;
using BigDataBoost.API.Core;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BigDataBoost.API.Controllers
{
    [Route("api/[controller]")]
    public class TagDefsController : Controller
    {
        private ITagDefRepository _tagdefRepository;
        private ITagHistRepository _taghistRepository;

        int page = 1;
        int pageSize = 1000;
        public TagDefsController(ITagDefRepository tagdefRepository,
                                ITagHistRepository taghistRepository)
        {
            _tagdefRepository = tagdefRepository;
            _taghistRepository = taghistRepository;
        }

        public IActionResult Get()
        {
            if (Request.Query.Count() > 0)
            {
                //List<TagDef> result;
                foreach (var param in Request.Query)
                {
                    TagDef _tagdef = _tagdefRepository.GetSingle(u => u.Name.Equals(param.Value, StringComparison.CurrentCultureIgnoreCase));

                    if (_tagdef != null)
                    {
                        TagDefViewModel _tagdefVM = Mapper.Map<TagDef, TagDefViewModel>(_tagdef);
                        return new OkObjectResult(_tagdefVM);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            var pagination = Request.Headers["Pagination"];

            if (!string.IsNullOrEmpty(pagination))
            {
                string[] vals = pagination.ToString().Split(',');
                int.TryParse(vals[0], out page);
                int.TryParse(vals[1], out pageSize);
            }

            int currentPage = page;
            int currentPageSize = pageSize;
            var totalRecs = _tagdefRepository.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecs / pageSize);

            IEnumerable<TagDef> _tagdefs = _tagdefRepository
                .GetAll()
                .OrderBy(u => u.Id)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();

            IEnumerable<TagDefViewModel> _tagdefsVM = Mapper.Map<IEnumerable<TagDef>, IEnumerable<TagDefViewModel>>(_tagdefs);

            Response.AddPagination(page, pageSize, totalRecs, totalPages);

            return new OkObjectResult(_tagdefsVM);
        }

        [HttpGet("{id}", Name = "GetTagDef")]
        public IActionResult GetTagDef(int id)
        {
            TagDef _tagdef = _tagdefRepository.GetSingle(u => u.Id == id);

            if (_tagdef != null)
            {
                TagDefViewModel _tagdefVM = Mapper.Map<TagDef, TagDefViewModel>(_tagdef);
                return new OkObjectResult(_tagdefVM);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("Definition/{TagName?}", Name = "GetTaxonomybyTagName")]
        public IActionResult GetTaxonomybyTagName(string TagName)
        {
            if (string.IsNullOrEmpty(TagName))
            {
                var pagination = Request.Headers["Pagination"];

                if (!string.IsNullOrEmpty(pagination))
                {
                    string[] vals = pagination.ToString().Split(',');
                    int.TryParse(vals[0], out page);
                    int.TryParse(vals[1], out pageSize);
                }

                int currentPage = page;
                int currentPageSize = pageSize;
                var totalRecs = _tagdefRepository.Count();
                var totalPages = (int)Math.Ceiling((double)totalRecs / pageSize);

                IEnumerable<TagDef> _tagdefs = _tagdefRepository
                    .GetAll()
                    .OrderBy(u => u.Id)
                    .Skip((currentPage - 1) * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                IEnumerable<TagDefViewModel> _tagdefsVM = Mapper.Map<IEnumerable<TagDef>, IEnumerable<TagDefViewModel>>(_tagdefs);

                Response.AddPagination(page, pageSize, totalRecs, totalPages);

                return new OkObjectResult(_tagdefsVM);
            }
            TagDef _tagdef = _tagdefRepository.GetSingle(u => u.Name.Equals(TagName,StringComparison.CurrentCultureIgnoreCase));

            if (_tagdef != null)
            {
                TagDefViewModel _tagdefVM = Mapper.Map<TagDef, TagDefViewModel>(_tagdef);
                return new OkObjectResult(_tagdefVM);
            }
            else
            {
                return NotFound();
            }
        }

        //Get definition for a list of points
        [HttpPost]
        [Route("Definition")]
        public IActionResult GetTagsDefsMultiple([FromBody] string[] ptNames)
        {
            List<TagDef> result = new List<TagDef>();
            foreach (var pt in ptNames)
            {
                var item = _tagdefRepository.GetSingle(u => u.Name.Equals(pt, StringComparison.CurrentCultureIgnoreCase));

                if (item != null)
                    result.Add(item);
            }

            if (result.Count > 0)
            {
                IEnumerable <TagDefViewModel> _userVM = Mapper.Map<IEnumerable<TagDef>, IEnumerable<TagDefViewModel>>(result.AsEnumerable<TagDef>());
                return new OkObjectResult(_userVM);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("TagsNameList/{PlantName}", Name = "GetTagsNameList")]
        public IActionResult GetTagsNameList(string PlantName)
        {
            IEnumerable<TagDef> _tagdef = _tagdefRepository.GetAll().Where(u => u.Source.Equals(PlantName,StringComparison.CurrentCultureIgnoreCase));

            if (_tagdef != null)
            {
                IEnumerable <TagDefViewModel> _tagdefVM = Mapper.Map<IEnumerable<TagDef>, IEnumerable<TagDefViewModel>>(_tagdef);
                return new OkObjectResult(_tagdefVM);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("TagsCurrentValue/{TagName?}", Name = "GetTagsCurrentValue")]
        public IActionResult GetTagsCurrentValue(string TagName)
        {
            if (string.IsNullOrEmpty(TagName))
                return NotFound();

            TagDef _tagdef = _tagdefRepository.GetSingle(u => u.Name.Equals(TagName,StringComparison.CurrentCultureIgnoreCase));

            if (_tagdef != null)
            {
                TagDefViewModel _userVM = Mapper.Map<TagDef, TagDefViewModel>(_tagdef);
                return new OkObjectResult(_userVM);
            }
            else
            {
                return NotFound();
            }
        }

        //Get values for a list of points
        [HttpPost]
        [Route("Values")]
        public IActionResult GetTagsValuesMultiple([FromBody] string[] ptNames)
        {
            List<TagDef> result = new List<TagDef>();
            foreach (var pt in ptNames)
            {
                var item = _tagdefRepository.GetSingle(u => u.Name.Equals(pt, StringComparison.CurrentCultureIgnoreCase));

                if (item != null)
                    result.Add(item);
            }

            if (result.Count > 0)
            {
                IEnumerable<TagDefViewModel> _userVM = Mapper.Map<IEnumerable<TagDef>, IEnumerable<TagDefViewModel>>(result.AsEnumerable<TagDef>());
                return new OkObjectResult(_userVM);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody]TagDefViewModel tagdef)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TagDef _newTag = new TagDef { Name = tagdef.Name, Description = tagdef.Description, ExtendedDescription = tagdef.ExtendedDescription };

            _tagdefRepository.Add(_newTag);
            _tagdefRepository.Commit();

            tagdef = Mapper.Map<TagDef, TagDefViewModel>(_newTag);

            CreatedAtRouteResult result = CreatedAtRoute("GetTagDef", new { controller = "TagDefs", id = tagdef.Id }, tagdef);
            return result;
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]TagDefViewModel tagdef)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TagDef _tagdefDb = _tagdefRepository.GetSingle(id);

            if (_tagdefDb == null)
            {
                return NotFound();
            }
            else
            {
                _tagdefDb.Source = tagdef.Source;
                _tagdefDb.Name = tagdef.Name;
                _tagdefDb.Description = tagdef.Description;
                _tagdefDb.ExtendedDescription = tagdef.ExtendedDescription;
                _tagdefRepository.Commit();
            }

            tagdef = Mapper.Map<TagDef, TagDefViewModel>(_tagdefDb);

            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            TagDef _tagdefDb = _tagdefRepository.GetSingle(id);

            if (_tagdefDb == null)
            {
                return new NotFoundResult();
            }
            else
            {
                _taghistRepository.DeleteWhere(a => a.TagDefId == id);

                _tagdefRepository.Delete(_tagdefDb);

                _tagdefRepository.Commit();

                return new NoContentResult();
            }
        }

    }

}
