using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BigDataBoost.Data.Abstract;
using BigDataBoost.Model;
using BigDataBoost.API.ViewModels;
using AutoMapper;
using BigDataBoost.API.Core;
using System.Net;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BigDataBoost.API.Controllers
{
    [Route("api/[controller]")]
    public class TagHistsController : Controller
    {
        private ITagDefRepository _tagdefRepository;
        private ITagHistRepository _taghistRepository;

        int page = 1;
        int pageSize = 1000;
        public TagHistsController(ITagDefRepository tagdefRepository,
                                ITagHistRepository taghistRepository)
        {
            _tagdefRepository = tagdefRepository;
            _taghistRepository = taghistRepository;
        }

        public IActionResult Get()
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
            var totalRecs = _taghistRepository.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecs / pageSize);

            IEnumerable<TagHist> _taghists = _taghistRepository
                .GetAll()
                .OrderBy(u => u.Id)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();

            if (_taghists != null)
            {
                IEnumerable<TagHistViewModel> _taghistVM = Mapper.Map<IEnumerable<TagHist>, IEnumerable<TagHistViewModel>>(_taghists);

                Response.AddPagination(page, pageSize, totalRecs, totalPages);

                var cal = currentPage * currentPageSize;
                if (cal >= totalRecs)
                    return new OkObjectResult(_taghistVM);
                else
                {
                    var result = new ObjectResult(_taghistVM);

                    result.StatusCode = (int)HttpStatusCode.PartialContent;
                    return result;
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{id}", Name = "GetHistoryByTagId")]
        public IActionResult Get(int id)
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
            var totalRecs = _taghistRepository.GetAll().Where(x => x.TagDefId == id).Count();
            var totalPages = (int)Math.Ceiling((double)totalRecs / pageSize);

            IEnumerable<TagHist> _tagHist = _taghistRepository.GetAll()
                .Where(u => u.TagDefId == id)
                .OrderBy(x => x.TimeStamp)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();

            if (_tagHist != null)
            {
                IEnumerable<TagHistViewModel> _taghistVM = Mapper.Map<IEnumerable<TagHist>, IEnumerable<TagHistViewModel>>(_tagHist);

                Response.AddPagination(page, pageSize, totalRecs, totalPages);

                var cal = currentPage * currentPageSize;
                if (cal >= totalRecs)
                    return new OkObjectResult(_taghistVM);
                else
                {
                    var result = new ObjectResult(_taghistVM);

                    result.StatusCode = (int)HttpStatusCode.PartialContent;
                    return result;
                }
            }
            else
            {
                return NotFound();
            }
        }


        private IEnumerable<TagHist> GetSnapshotValues(string TagName, string StartTime, string EndTime, string Frequency)
        {
            if (string.IsNullOrEmpty(TagName))
                return null;
            if (string.IsNullOrEmpty(StartTime))
                return null;
            if (string.IsNullOrEmpty(EndTime))
                return null;

            DateTime pstart = DateTime.Parse(StartTime);
            DateTime pend = DateTime.Parse(EndTime);

            IEnumerable<TagHist> _taghist = _taghistRepository.GetAll().Where(u => (u.TagName.Equals(TagName, StringComparison.CurrentCultureIgnoreCase))
                                                                            && (u.TimeStamp >= pstart && u.TimeStamp <= pend));

            //Check if we have frequency. If frequency is not specified then we should just return raw values 
            List<TagHist> snapshotData = new List<TagHist>();
            if (!string.IsNullOrEmpty(Frequency))
            {
                if (_taghist == null)
                    return null;
                if (_taghist.Count() < 1)
                    return null;

                var temp = _taghist.ToList();
                // we have a freq value set 
                int freq = Convert.ToInt32(Frequency);

                DateTime cTime = pstart;

                var previous = 0;
                var cursor = 0;
                while (cTime <= pend)
                {
                    // find a record near this time
                    if (temp[cursor].TimeStamp >= cTime)
                    {
                        TagHist nTag = new TagHist()
                        {
                            Id = temp[cursor].Id,
                            TagDefId = temp[cursor].TagDefId,
                            TagName = temp[cursor].TagName,
                            Value = temp[cursor].Value,
                            TimeStamp = cTime,
                            Status = temp[cursor].Status
                        };
                        snapshotData.Add(nTag);
                    }
                    else
                    {
                        previous = cursor;
                        if (cursor < temp.Count - 1)
                            cursor++;
                        if (temp[cursor].TimeStamp > cTime)
                        {
                            TagHist nTag = new TagHist()
                            {
                                Id = temp[previous].Id,
                                TagDefId = temp[previous].TagDefId,
                                TagName = temp[previous].TagName,
                                Value = temp[previous].Value,
                                TimeStamp = cTime,
                                Status = temp[previous].Status
                            };
                            snapshotData.Add(nTag);
                        }
                        else
                        {
                            TagHist nTag = new TagHist()
                            {
                                Id = temp[cursor].Id,
                                TagDefId = temp[cursor].TagDefId,
                                TagName = temp[cursor].TagName,
                                Value = temp[cursor].Value,
                                TimeStamp = cTime,
                                Status = temp[cursor].Status
                            };
                            snapshotData.Add(nTag);
                        }
                    }
                    cTime = cTime.AddSeconds(freq);
                }

                return snapshotData.AsEnumerable<TagHist>();
            }

            if (_taghist != null)
            {
                return _taghist;
            }
            else
            {
                return null;
            }
        }

        [HttpGet("TagDataRange/{TagName?}/{StartTime?}/{EndTime?}/{Frequency?}", Name = "GetHistValue")]
        public IActionResult GetHistValue(string TagName, string StartTime, string EndTime, string Frequency)
        {
            IEnumerable<TagHist> _taghistSnapshot = GetSnapshotValues(TagName, StartTime, EndTime, Frequency);

            if (_taghistSnapshot == null)
                return NotFound();

            var pagination = Request.Headers["Pagination"];

            if (!string.IsNullOrEmpty(pagination))
            {
                string[] vals = pagination.ToString().Split(',');
                int.TryParse(vals[0], out page);
                int.TryParse(vals[1], out pageSize);
            }

            int currentPage = page;
            int currentPageSize = pageSize;
            var totalRecs = _taghistSnapshot.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecs / pageSize);

            IEnumerable<TagHist> _tagHist = _taghistSnapshot
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();

            if (_tagHist != null)
            {
                IEnumerable<TagHistViewModel> _taghistVM = Mapper.Map<IEnumerable<TagHist>, IEnumerable<TagHistViewModel>>(_tagHist);

                Response.AddPagination(page, pageSize, totalRecs, totalPages);

                var cal = currentPage * currentPageSize;
                if (cal >= totalRecs)
                    return new OkObjectResult(_taghistVM);
                else
                {
                    var result = new ObjectResult(_taghistVM);

                    result.StatusCode = (int)HttpStatusCode.PartialContent;
                    return result;
                }
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPost("TagDataRange", Name = "GetHistValuePost")]
        public IActionResult GetHistValuePost([FromBody] IDictionary<string,object> paramObject)
        {
            try
            {
                string TagName, StartTime, EndTime, Frequency;
                
                TagName = StartTime = EndTime = Frequency = string.Empty;

                if (paramObject.ContainsKey("TagName"))
                    TagName = Convert.ToString(paramObject["TagName"]);
                if (paramObject.ContainsKey("StartTime"))
                    StartTime = Convert.ToString(paramObject["StartTime"]);
                if (paramObject.ContainsKey("EndTime"))
                    EndTime = Convert.ToString(paramObject["EndTime"]);
                if (paramObject.ContainsKey("Frequency"))
                    Frequency = Convert.ToString(paramObject["Frequency"]);

                return GetHistValue(TagName, StartTime, EndTime, Frequency);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        //[HttpPost]
        //public IActionResult Create([FromBody]TagDefViewModel tagdef)
        //{

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    TagDef _newTag = new TagDef { Name = tagdef.Name, Description = tagdef.Description, ExtendedDescription = tagdef.ExtendedDescription };

        //    _tagdefRepository.Add(_newTag);
        //    _tagdefRepository.Commit();

        //    tagdef = Mapper.Map<TagDef, TagDefViewModel>(_newTag);

        //    CreatedAtRouteResult result = CreatedAtRoute("GetTagDef", new { controller = "TagDefs", id = tagdef.Id }, tagdef);
        //    return result;
        //}

        //[HttpPut("{id}")]
        //public IActionResult Put(int id, [FromBody]TagDefViewModel tagdef)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    TagDef _tagdefDb = _tagdefRepository.GetSingle(id);

        //    if (_tagdefDb == null)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        _tagdefDb.Name = tagdef.Name;
        //        _tagdefDb.Description = tagdef.Description;
        //        _tagdefDb.ExtendedDescription = tagdef.ExtendedDescription;
        //        _tagdefRepository.Commit();
        //    }

        //    tagdef = Mapper.Map<TagDef, TagDefViewModel>(_tagdefDb);

        //    return new NoContentResult();
        //}

        //[HttpDelete("{id}")]
        //public IActionResult Delete(int id)
        //{
        //    TagDef _tagdefDb = _tagdefRepository.GetSingle(id);

        //    if (_tagdefDb == null)
        //    {
        //        return new NotFoundResult();
        //    }
        //    else
        //    {
        //        _taghistRepository.DeleteWhere(a => a.TagId == id);

        //        _tagdefRepository.Delete(_tagdefDb);

        //        _tagdefRepository.Commit();

        //        return new NoContentResult();
        //    }
        //}
    }
}
