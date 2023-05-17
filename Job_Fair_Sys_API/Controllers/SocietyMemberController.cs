using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Job_Fair_Sys_API.Models;
using Job_Fair_Sys_Data.Repositories;

namespace Job_Fair_Sys_API.Controllers
{
    public class SocietyMemberController : ApiController
    { 
        private readonly SocietyMemberRepository _societyMemberRespository;

        public SocietyMemberController()
        {
            _societyMemberRespository = new SocietyMemberRepository();
        }

        [Route("api/members")]
        public HttpResponseMessage Get()
        {
            try
            {
                var members = _societyMemberRespository.GetSocietyMembers();
                List<MemberViewModel> models = new List<MemberViewModel>();

                foreach (var item in members)
                {
                    models.Add(MemberViewModel.ToViewModel(item));
                }

                return Request.CreateResponse(HttpStatusCode.OK, models);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("api/member/add")]
        public HttpResponseMessage AddMember(MemberViewModel member)
        {
            try
            {
                if (member == null) return Request.CreateResponse(HttpStatusCode.NotAcceptable);

                var entity = member.ToEntity();                    
                _societyMemberRespository.Add(entity);

                return Request.CreateResponse(HttpStatusCode.OK, member);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("api/member/update")]
        public HttpResponseMessage UpdateMember(MemberViewModel member)
        {
            try
            {
                if (member == null || member.id == 0) 
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable);

                var entity = _societyMemberRespository.GetMember(member.id);

                if (entity == null) return Request.CreateResponse(HttpStatusCode.NotFound);

                entity.UserId = member.userId;
                entity.Name = member.name;
                entity.Gender = member.gender;
                entity.Contact = member.contact;

                _societyMemberRespository.DbContext.Entry(entity).CurrentValues.SetValues(entity);
                _societyMemberRespository.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, member);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
